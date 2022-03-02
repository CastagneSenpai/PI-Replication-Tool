using NLog;
using OSIsoft.AF.PI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Models
{
    public sealed class PIAttributesUpdateManager
    {
        static readonly Logger Logger = LogManager.GetLogger("PIReplicationToolLogger");

        public List<IDictionary<string, object>> AttributesTagsList { get; set; } = new List<IDictionary<string, object>>();
        public PIAttributesUpdateManager()
        {

        }
        public void LoadTagsAttributes(PIServer p_PIServer, List<string> p_PITagNames)
        {
            List<OSIsoft.AF.PI.PIPoint> v_PIPointList = new List<OSIsoft.AF.PI.PIPoint>();

            foreach (string piTagNames in p_PITagNames)
            {
                var v_PIPoint = OSIsoft.AF.PI.PIPoint.FindPIPoint(p_PIServer, piTagNames);
                v_PIPointList.Add(v_PIPoint);
            }

            foreach (var v_PIPoint in v_PIPointList)
            {
                AttributesTagsList.Add(v_PIPoint.GetAttributes());
            }
        }
        public void Clear()
        {
            AttributesTagsList.Clear();
        }

        // Main method of PIAttributeUpdateManager, called to update the AttributesTagsList before pushing to target server
        public void UpdateTagsAttributes(PIServer p_PISourceServer, PIServer p_PITargetServer)
        {
            string digitalPointSource = "", numericalPointSource = "";
            this.FindPointSourcesToUse(p_PISourceServer, p_PITargetServer, ref digitalPointSource, ref numericalPointSource);

            // Apply update for each tag using List<T>.ForEach() method (most efficient way to proceed)
            AttributesTagsList.ForEach(o => UpdateTagAttributes(p_PISourceServer, p_PITargetServer, o, digitalPointSource, numericalPointSource));
        }
        private void UpdateTagAttributes(PIServer p_PISourceServer, PIServer p_PITargetServer, IDictionary<string, object> tagAttributes, string p_DigitalPointSource, string p_NumericalPointSource)
        {
            try
            {
                this.UpdatePointSourceAttributes(p_PISourceServer, p_PITargetServer, tagAttributes, p_DigitalPointSource, p_NumericalPointSource);
                this.UpdateCompressionExceptionAttributes(tagAttributes);
                this.UpdateSecurityAttributes(tagAttributes);
                this.VerifyTypicalValues(tagAttributes);
                this.UpdateTagNameAndInstrumentTag(p_PISourceServer, tagAttributes);
            }
            catch (Exception e)
            {
                Logger.Error($"Error updating tags attributes. {e.Message}");
            }
        }
        private void UpdatePointSourceAttributes(PIServer p_PISourceServer, PIServer p_PITargetServer, IDictionary<string, object> tagAttributes, string p_DigitalPointSource, string p_NumericalPointSource)
        {
            try
            {
                if ((string)tagAttributes["pointtype"] == "digital" || (string)tagAttributes["pointtype"] == "string")
                {
                    tagAttributes["pointsource"] = p_DigitalPointSource;
                }
                else
                {
                    tagAttributes["pointsource"] = p_NumericalPointSource;
                }

            }
            catch (Exception e)
            {
                Logger.Error($"Error updating compression and exception attributes. {e.Message}");
            }
        }
        private void UpdateCompressionExceptionAttributes(IDictionary<string, object> tagAttributes)
        {
            try
            {
                // Put compression & exception parameter to 0
                tagAttributes["compressing"] = 0;
                tagAttributes["compdev"] = 0;
                tagAttributes["compmin"] = 0;
                tagAttributes["compmax"] = 0;
                tagAttributes["compdevpercent"] = 0;

                tagAttributes["excdev"] = 0;
                tagAttributes["excmin"] = 0;
                tagAttributes["excmax"] = 0;
                tagAttributes["excdevpercent"] = 0;

            }
            catch (Exception e)
            {
                Logger.Error($"Error updating compression and exception attributes. {e.Message}");
            }
        }
        private void UpdateSecurityAttributes(IDictionary<string, object> tagAttributes)
        {
            try
            {
                tagAttributes["datasecurity"] = Constants.PISecurityConfiguration;
                tagAttributes["ptsecurity"] = Constants.PISecurityConfiguration;
            }
            catch (Exception e)
            {
                Logger.Error($"Error updating security attributes. {e.Message}");
            }
        }
        private void VerifyTypicalValues(IDictionary<string, object> tagAttributes)
        {
            try
            {
                float zero, typicalvalue, span;

                zero = (float)tagAttributes["zero"];
                typicalvalue = (float)tagAttributes["typicalvalue"];
                span = (float)tagAttributes["span"];

                // If typicalvalue do not respect PI rules in source server, remove typical value
                if (typicalvalue < zero || typicalvalue > zero + span)
                {
                    tagAttributes["typicalvalue"] = null;
                }

            }
            catch (Exception e)
            {
                Logger.Error($"Error verifying typicalValue, zero or span attributes. {e.Message}");
            }
        }
        private void UpdateTagNameAndInstrumentTag(PIServer p_PISourceServer, IDictionary<string, object> tagAttributes)
        {
            try
            {
                // Update InstrumentTag
                tagAttributes["instrumenttag"] = tagAttributes["tag"];

                // Update TagName if source server is TEPNL, TEPUK or TEPGB
                string Trigramme = this.GetTrigrammeFromPIServer(p_PISourceServer);
                if (Trigramme == "NLD")
                    tagAttributes["tag"] = "NLD_" + tagAttributes["tag"]; // NL : Prefixe NLD_

                else if (Trigramme == "ABZ")
                    tagAttributes["tag"] = "UK_" + tagAttributes["tag"]; // UK : Prefixe UK_

                else if (Trigramme == "POG")
                    tagAttributes["tag"] = "POG_" + tagAttributes["tag"]; // GB : Prefixe POG_
            }
            catch (Exception e)
            {
                Logger.Error($"Error updating TagName and Instrumenttag attributes. {e.Message}");
            }
        }
        private void FindPointSourcesToUse(PIServer p_PISourceServer, PIServer p_PITargetServer, ref string digitalPointSource, ref string numericalPointSource)
        {
            long? currentDigitalPointCount = null, currentNumericalPointCount = null;
            string v_Trigramme = this.GetTrigrammeFromPIServer(p_PISourceServer);

            if (v_Trigramme != null)
            {
                try
                {
                    // Get the PointSource of PI Target Server
                    ICollection<PIPointSource> allPointSources = p_PITargetServer.PointSources;

                    foreach (PIPointSource ps in allPointSources)
                    {
                        // Select the PointSource which contains the trigramme - Remove the others
                        if (!ps.Name.Contains(v_Trigramme))
                        {
                            allPointSources.Remove(ps);
                        }
                    }

                    foreach (PIPointSource ps in allPointSources)
                    {
                        // Select the PointSource N & D with minimal PointCount
                        if (ps.Name.Contains("D0"))
                        {
                            if (currentDigitalPointCount is null || currentDigitalPointCount > ps.PointCount)
                            {
                                digitalPointSource = ps.Name;
                                currentDigitalPointCount = ps.PointCount;
                            }
                        }
                        else if (ps.Name.Contains("N0"))
                        {
                            if (currentNumericalPointCount is null || currentNumericalPointCount > ps.PointCount)
                            {
                                numericalPointSource = ps.Name;
                                currentNumericalPointCount = ps.PointCount;
                            }
                        }
                        else { } // do nothing and go for the next PointSource }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error($"Error selecting the point sources to use from PI target server point source list. {e.Message}");
                }
            }
            else
            {
                Logger.Warn($"The PI server {p_PISourceServer} does not contain an alias with the trigramme of site, which cause the application to not retrieve the Point sources to use.");
            }
        }
        public string GetTrigrammeFromPIServer(PIServer p_PIServer)
        {
            string v_Trigramme = "";

            foreach (string aliasServer in p_PIServer.AliasNames)
            {
                if (aliasServer.Contains("PI-DA-"))
                {
                    // Cut the Alias server name to get the trigramme (Example : PI-DA-LAD-AO >> LAD)
                    v_Trigramme = aliasServer.Substring(6, 3);
                }
            }
            return v_Trigramme;
        }
    }
}
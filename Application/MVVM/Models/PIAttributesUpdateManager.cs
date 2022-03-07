using NLog;
using OSIsoft.AF.PI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Models
{
    public sealed class PIAttributesUpdateManager
    {
        // ATTRIBUTES
        static readonly Logger Logger = LogManager.GetLogger("PIReplicationToolLogger");
        public List<IDictionary<string, object>> AttributesTagsList { get; set; } = new List<IDictionary<string, object>>();
        public ICollection<PIPointSource> PointSources_Digital { get; set; }
        public ICollection<PIPointSource> PointSources_Numerical { get; set; }
        public string Trigram { get; set; }

        // CONSTRUCTOR
        public PIAttributesUpdateManager() { }

        // METHODS
        public void LoadTagsAttributes(PIServer p_PIServer, List<string> p_PITagNames)
        {
            List<PIPoint> v_PIPointList = new List<PIPoint>();

            foreach (string piTagNames in p_PITagNames)
            {
                var v_PIPoint = PIPoint.FindPIPoint(p_PIServer, piTagNames);
                v_PIPointList.Add(v_PIPoint);
            }

            foreach (var v_PIPoint in v_PIPointList)
            {
                AttributesTagsList.Add(v_PIPoint.GetAttributes());
            }
        }
        // Main method of PIAttributeUpdateManager, called to update the AttributesTagsList before pushing to target server
        public void UpdateTagsAttributes(PIServer p_PISourceServer, PIServer p_PITargetServer)
        {
            // string v_DigitalPointSource = "", v_NumericalPointSource = "";
            // this.FindPointSourcesToUse(p_PISourceServer, p_PITargetServer, ref v_DigitalPointSource, ref v_NumericalPointSource);
            if (this.IsThereEnoughtPointSourcesAvailable(p_PITargetServer))
            {
                // Apply update for each tag using List<T>.ForEach() method (most efficient way to proceed)
                AttributesTagsList.ForEach(TagAttributes => UpdateTagAttributes(p_PISourceServer, TagAttributes));
            }
            else
            {
                throw new Exception("Not enought PointSources available for this replication, please create more PItoPI interfaces before continue");
            }

        }

        private bool IsThereEnoughtPointSourcesAvailable(PIServer p_PITargetServer)
        {
            int v_NbNumerical = 0, v_NbDigital = 0;

            // Count how many digital & numerical tags have to be replicated
            foreach (IDictionary<string, object> v_AttributesTag in AttributesTagsList)
            {
                if ((string)v_AttributesTag["pointtype"] == "digital" || (string)v_AttributesTag["pointtype"] == "string")
                    v_NbDigital++;
                else
                    v_NbNumerical++;
            }

            // Update local variables PointSources_Numerical & PointSources_Digital using the PointSources list
            foreach (PIPointSource v_PS in p_PITargetServer.PointSources)
            {
                if (v_PS.Name.Contains(this.Trigram) && v_PS.Name.Contains("N0"))
                {
                    this.PointSources_Numerical.Add(v_PS);
                }

                else if (v_PS.Name.Contains(this.Trigram) && v_PS.Name.Contains("D0"))
                {
                    this.PointSources_Digital.Add(v_PS);
                }
            }

            // Count how many space is available for numerical and digital Pointsources list
            long v_AvailableNumericalPointSpace = this.PointSources_Numerical.Sum(v_PS =>
            {
                return int.Parse(ConfigurationManager.AppSettings["NumericalMaxPointCountAllowed"]) - v_PS.PointCount;
            });

            long v_AvailableDigitalPointSpace = this.PointSources_Digital.Sum(v_PS =>
            {
                return int.Parse(ConfigurationManager.AppSettings["DigitalMaxPointCountAllowed"]) - v_PS.PointCount;
            });

            // return true if there is enought space for the replication, else return false
            if (v_AvailableNumericalPointSpace >= v_NbNumerical && v_AvailableDigitalPointSpace >= v_NbDigital)
                return true;
            else
                return false;
        }

        private void UpdateTagAttributes(PIServer p_PISourceServer, IDictionary<string, object> p_TagAttributes)
        {
            try
            {
                this.UpdatePointSourceAttributes(ref p_TagAttributes);
                this.UpdateCompressionExceptionAttributes(ref p_TagAttributes);
                this.UpdateSecurityAttributes(ref p_TagAttributes);

                // Actions on digital tags
                if ((string)p_TagAttributes["pointtype"] == "digital" || (string)p_TagAttributes["pointtype"] == "string")
                {
                    this.UpdateTagNameAndInstrumentTag(ref p_TagAttributes, p_PISourceServer);
                }
                // Action on numerical tags
                else
                {
                    this.VerifyTypicalValues(ref p_TagAttributes);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error updating tag {p_TagAttributes["tag"]} attributes. {e.Message}");
            }
        }
        private void UpdatePointSourceAttributes(ref IDictionary<string, object> p_TagAttributes)
        {
            try
            {
                //if ((string)p_TagAttributes["pointtype"] == "digital" || (string)p_TagAttributes["pointtype"] == "string")
                //{
                //    p_TagAttributes["pointsource"] = p_DigitalPointSource;
                //}
                //else
                //{
                //    p_TagAttributes["pointsource"] = p_NumericalPointSource;
                //}
            }
            catch (Exception e)
            {
                Logger.Error($"Error updating PointSource attribute. {e.Message}");
            }
        }
        private void UpdateCompressionExceptionAttributes(ref IDictionary<string, object> p_TagAttributes)
        {
            try
            {
                // Put compression & exception parameter to 0
                p_TagAttributes["compressing"] = 0;
                p_TagAttributes["compdev"] = 0;
                p_TagAttributes["compmin"] = 0;
                p_TagAttributes["compmax"] = 0;
                p_TagAttributes["compdevpercent"] = 0;

                p_TagAttributes["excdev"] = 0;
                p_TagAttributes["excmin"] = 0;
                p_TagAttributes["excmax"] = 0;
                p_TagAttributes["excdevpercent"] = 0;

            }
            catch (Exception e)
            {
                Logger.Error($"Error updating compression and exception attributes. {e.Message}");
            }
        }
        private void UpdateSecurityAttributes(ref IDictionary<string, object> p_TagAttributes)
        {
            try
            {
                p_TagAttributes["datasecurity"] = ConfigurationManager.AppSettings["PISecurityConfiguration"];
                p_TagAttributes["ptsecurity"] = ConfigurationManager.AppSettings["PISecurityConfiguration"];
            }
            catch (Exception e)
            {
                Logger.Error($"Error updating security attributes. {e.Message}");
            }
        }
        private void VerifyTypicalValues(ref IDictionary<string, object> p_TagAttributes)
        {
            try
            {
                float v_Zero, v_Typicalvalue, v_Span;

                v_Zero = (float)p_TagAttributes["zero"];
                v_Typicalvalue = (float)p_TagAttributes["typicalvalue"];
                v_Span = (float)p_TagAttributes["span"];

                // If typicalvalue do not respect PI rules in source server, remove typical value
                if (v_Typicalvalue < v_Zero || v_Typicalvalue > v_Zero + v_Span)
                {
                    p_TagAttributes["typicalvalue"] = null;
                }

            }
            catch (Exception e)
            {
                Logger.Error($"Error verifying typicalValue, zero or span attributes. {e.Message}");
            }
        }
        private void UpdateTagNameAndInstrumentTag(ref IDictionary<string, object> p_TagAttributes, PIServer p_PISourceServer)
        {
            try
            {
                // Update TagName if source server is TEPNL, TEPUK or TEPGB
                if (this.Trigram == "NLD")
                {
                    p_TagAttributes["instrumenttag"] = p_TagAttributes["tag"];
                    p_TagAttributes["tag"] = "NLD_" + p_TagAttributes["tag"]; // NL : Prefixe NLD_
                }
                else if (this.Trigram == "ABZ")
                {
                    p_TagAttributes["instrumenttag"] = p_TagAttributes["tag"];
                    p_TagAttributes["tag"] = "UK_" + p_TagAttributes["tag"]; // UK : Prefixe UK_
                }
                else if (this.Trigram == "POG")
                {
                    p_TagAttributes["instrumenttag"] = p_TagAttributes["tag"];
                    p_TagAttributes["tag"] = "POG_" + p_TagAttributes["tag"]; // GB : Prefixe POG_
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error updating TagName and Instrumenttag attributes. {e.Message}");
            }
        }
        //private void FindPointSourcesToUse(PIServer p_PISourceServer, PIServer p_PITargetServer, ref string p_DigitalPointSource, ref string p_NumericalPointSource)
        //{
        //    long? v_CurrentDigitalPointCount = null, v_CurrentNumericalPointCount = null;
        //    string v_Trigramme = this.GetTrigrammeFromPIServer(p_PISourceServer);

        //    if (v_Trigramme != null)
        //    {
        //        try
        //        {
        //            // Get the PointSource of PI Target Server
        //            ICollection<PIPointSource> v_AllPointSources = p_PITargetServer.PointSources;

        //            foreach (PIPointSource v_ps in v_AllPointSources)
        //            {
        //                // Select the PointSource which contains the trigramme - Remove the others
        //                if (!v_ps.Name.Contains(v_Trigramme))
        //                {
        //                    v_AllPointSources.Remove(v_ps);
        //                }
        //            }

        //            foreach (PIPointSource v_ps in v_AllPointSources)
        //            {
        //                // Select the PointSource N & D with minimal PointCount
        //                if (v_ps.Name.Contains("D0"))
        //                {
        //                    if (v_CurrentDigitalPointCount is null || v_CurrentDigitalPointCount > v_ps.PointCount)
        //                    {
        //                        p_DigitalPointSource = v_ps.Name;
        //                        v_CurrentDigitalPointCount = v_ps.PointCount;
        //                    }
        //                }
        //                else if (v_ps.Name.Contains("N0"))
        //                {
        //                    if (v_CurrentNumericalPointCount is null || v_CurrentNumericalPointCount > v_ps.PointCount)
        //                    {
        //                        p_NumericalPointSource = v_ps.Name;
        //                        v_CurrentNumericalPointCount = v_ps.PointCount;
        //                    }
        //                }
        //                else { } // do nothing and go for the next PointSource }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Logger.Error($"Error selecting the point sources to use from PI target server point source list. {e.Message}");
        //        }
        //    }
        //    else
        //    {
        //        Logger.Warn($"The PI server {p_PISourceServer} does not contain an alias with the trigramme of site, which cause the application to not retrieve the Point sources to use.");
        //    }
        //}

        // TODO : Modifier pour aller chercher le trigramme dans le fichier de config
        public void GetTrigrammeFromPIServer(PIServer p_PIServer)
        {
            // TODO : Use a config/csv file instead of alias method
            foreach (string v_AliasServer in p_PIServer.AliasNames)
            {
                if (v_AliasServer.Contains("PI-DA-"))
                {
                    // Cut the Alias server name to get the trigramme (Example : PI-DA-LAD-AO >> LAD)
                    Trigram = v_AliasServer.Substring(6, 3);
                }
            }
        }
        public void Clear()
        {
            AttributesTagsList.Clear();
        }
    }
}
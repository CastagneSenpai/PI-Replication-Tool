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

        public List<IDictionary<string, object>> AttributesTagsList { get; set; } = new List<IDictionary<string, object>>() ;
        public PIAttributesUpdateManager()
        {

        }

        // TODO : Replace p_ListAttributes by the attribute in local instance this.AttributesTagsList
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
        internal void Clear()
        {
            AttributesTagsList.Clear();
        }

        // Main method of PIAttributeUpdateManager, called to update the AttributesTagsList before pushing to target server
        public void UpdateTagsAttributes (PIServer p_PISourceServer, PIServer p_PITargetServer)
        {
            this.UpdatePointSourceAttributes(p_PISourceServer, p_PITargetServer);
            this.UpdateCompressionExceptionAttributes();
            this.UpdateSecurityAttributes();
            this.VerifyTypicalValues();
        }

        private void UpdatePointSourceAttributes(PIServer p_PISourceServer, PIServer p_PITargetServer)
        {
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

                    long? currentDigitalPointCount = null, currentNumericalPointCount = null;
                    string digitalPointSource, numericalPointSource;

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

                // Foreach tag, if Numerical, replace PointSource by N0X, else D0X.
                try
                {
                    foreach (IDictionary<string, object> tagAttributes in AttributesTagsList)
                    {
                        if((string)tagAttributes["pointtype"] == "test" )
                        {

                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error($"Error updating compression and exception attributes. {e.Message}");
                }
            }
            else
            {
                Logger.Warn($"The PI server {p_PISourceServer} does not contain an alias with the trigramme of site, which cause the application to not retrieve the Point sources to use.");
            }
        }

        private string GetTrigrammeFromPIServer(PIServer p_PIServer)
        {
            string v_Trigramme = "";

            foreach(string aliasServer in p_PIServer.AliasNames)
            {
                if (aliasServer.Contains("PI-DA-"))
                {
                    // Cut the Alias server name to get the trigramme (Example : PI-DA-LAD-AO >> LAD)
                    v_Trigramme = aliasServer.Substring(6, 3);
                }
            }
            return v_Trigramme;
        }

        private void UpdateCompressionExceptionAttributes()
        {
            try
            {
                foreach (IDictionary<string, object> tagAttributes in AttributesTagsList)
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
            }
            catch(Exception e)
            {
                Logger.Error($"Error updating compression and exception attributes. {e.Message}");
            }
        }

        private void UpdateSecurityAttributes()
        {
            try
            {
                foreach (IDictionary<string, object> tagAttributes in AttributesTagsList)
                {
                    tagAttributes["datasecurity"] = Constants.PISecurityConfiguration;
                    tagAttributes["ptsecurity"] = Constants.PISecurityConfiguration;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error updating security attributes. {e.Message}");
            }
        }

        private void VerifyTypicalValues()
        {
            try
            {
                float zero, typicalvalue, span;

                foreach (IDictionary<string, object> tagAttributes in AttributesTagsList)
                {
                    zero = (float)tagAttributes["zero"];
                    typicalvalue = (float)tagAttributes["typicalvalue"];
                    span = (float)tagAttributes["span"];
                    
                    // If typicalvalue do not respect PI rules in source server, remove typical value
                    if (typicalvalue < zero || typicalvalue > zero+span)
                    {
                        tagAttributes["typicalvalue"] = null;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error verifying typicalValue, zero or span attributes. {e.Message}");
            }
        }
    }
}

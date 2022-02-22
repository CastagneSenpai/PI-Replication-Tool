using NLog;
using OSIsoft.AF.PI;
using System;
using System.Collections.Generic;

namespace Models
{
    public sealed class PIAttributesUpdateManager
    {
        static readonly Logger Logger = LogManager.GetLogger("PIReplicationToolLogger");

        List<IDictionary<string, object>> AttributesTagsList = new List<IDictionary<string, object>>();
        public PIAttributesUpdateManager()
        {

        }

        // TODO : Replace p_ListAttributes by the attribute in local instance this.AttributesTagsList
        public void LoadTagsAttributes(PIServer p_PIServer, List<string> p_PITagNames, ref List<IDictionary<string, object>> p_ListAttributes)
        {
            List<OSIsoft.AF.PI.PIPoint> v_PIPointList = new List<OSIsoft.AF.PI.PIPoint>();            

            foreach (string piTagNames in p_PITagNames)
            {
                var v_PIPoint = OSIsoft.AF.PI.PIPoint.FindPIPoint(p_PIServer, piTagNames);
                v_PIPointList.Add(v_PIPoint);
            }

            foreach (var v_PIPoint in v_PIPointList)
            {
                p_ListAttributes.Add(v_PIPoint.GetAttributes());
            }
        }

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
                // Get the PointSource of PI Target Server

                // Select the PointSource which contains the trigramme - Remove the others

                // Select the PointSource N & D with minimal PointCount - Remove the others

                // Foreach tag, if Numerical, replace PointSource by N, else D.
                // TODO : use AttributesTagsList;

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

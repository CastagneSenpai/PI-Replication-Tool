using OSIsoft.AF.PI;
using System;
using System.Collections.Generic;

namespace Models
{
    public sealed class PIAttributesUpdateManager
    {
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
            this.UpdateCompressionAttributes();
            this.UpdateSecurityAttributes();
            this.VerifyTypicalValues();
        }

        

        

        private void UpdatePointSourceAttributes(PIServer p_PISourceServer, PIServer p_PITargetServer)
        {
            throw new NotImplementedException();
        }

        private void UpdateCompressionAttributes()
        {
            throw new NotImplementedException();
        }

        private void UpdateSecurityAttributes()
        {
            throw new NotImplementedException();
        }


        private void VerifyTypicalValues()
        {
            throw new NotImplementedException();
        }
    }
}

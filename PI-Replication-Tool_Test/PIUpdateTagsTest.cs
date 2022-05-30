using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace PI_Replication_Tool_Test
{
    [TestClass]
    public class PIUpdateTags
    {
        [TestMethod]
        public void TestWrongUpdateCompressionExceptionAttribute(ref IDictionary<string, object> p_TagAttributes)
        {
            //Arrange
            var PITargetServer = "PI-CENTER-HQ";
                  p_TagAttributes["compressing"] = 0;
                  p_TagAttributes["compdev"] = 0;
                  p_TagAttributes["compmin"] = 0;
                  p_TagAttributes["compmax"] = 0;
                  p_TagAttributes["compdevpercent"] = 0;
                  p_TagAttributes["excdev"] = 0;
                  p_TagAttributes["excmin"] = 0;
                  p_TagAttributes["excmax"] = 0;
                  p_TagAttributes["excdevpercent"] = 0;


            //Act


            //Assert
            resultat.



        }
    }
}

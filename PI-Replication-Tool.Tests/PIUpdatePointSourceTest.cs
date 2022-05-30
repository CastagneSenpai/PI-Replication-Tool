using Models;
using OSIsoft.AF.PI;
using System.Collections.Generic;
using Xunit;


namespace PI_Replication_Tool.Tests
{
    public class PIUpdatePointSourceTest
    {
        PIServer PISourceServer = PIServer.FindPIServer("PI-DA-LAD-AO");
        PIServer PITargetServer = PIServer.FindPIServer("PI-CENTER-HQ");
        internal static IDictionary<string,object> load()
        {
            var server = PIServer.FindPIServer("PI-DA-LAD-AO");
            // On cherche un tag provenant de PI-CENTER-HQ.
            var tag = PIPoint.FindPIPoint(server, "CLV_HIC27002A.PV");
            var attributes = tag.GetAttributes();
            // On récupère les attributs.

            return attributes;
            // On retourne les attributs
        }

        public static IDictionary<string, object> dicList = load();
        public static IEnumerable<object[]> enumList = new List<object[]> { new object[] { dicList } };
        PIReplicationManager PIReplicationManager = new PIReplicationManager();

        [Theory]
        [MemberData(nameof(enumList))]

        public void Checking_HQAngolaPointShouldContainTrigramme(IDictionary<string, object> dic)
        {
            //ARRANGE
            PIReplicationManager dicList = new PIReplicationManager();
            dicList.PIAttributesUpdateManager.AttributesTagsList.Add(dic);
            dicList.PIAttributesUpdateManager.UpdateTagsAttributes(PISourceServer, PITargetServer);

            //ACT
            bool isValid = ["pointsource"].ToString().Contains("LAD_");

            //ASSERT
            //Assert.Equal(isValid.ToString(), "true");
            Assert.True(isValid, "Point source from Luanda LAD is not correct");

        }

    }
}

using OSIsoft.AF.Asset;
using OSIsoft.AF.PI;
using System;
using System.Collections.Generic;

namespace TestPIReplicationTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var PIServerTest = PIServer.FindPIServer("PI-DA-Q20-HQ");
            var PIServerHQ = PIServer.FindPIServer("PI-CENTER-HQ");

            var toto = PIServerHQ.StateSets.GetEnumerator();
            //while (toto.MoveNext())
            //{
            //    Console.WriteLine(toto.Current);
            //}

            var tag = "CGI_PIReplicationTool_Test_D01";

            var yolo = PIPoint.FindPIPoint(PIServerTest, tag);
            var digitalset = yolo.GetStateSet();
            Console.WriteLine(digitalset);
            //var titi = "ONOFF";

            Dictionary<string, object> tata = new Dictionary<string, object>
            {
                [PICommonPointAttributes.PointType] = PIPointType.Digital,
                [PICommonPointAttributes.DigitalSetName] = digitalset.Name
            };

            PIPoint pipoint = null;
            bool trouve = PIPoint.TryFindPIPoint(PIServerHQ, tag, out pipoint);
            //var tata = yolo.GetAttributes();

            if (trouve)
            {
                Console.WriteLine("je suis laaaaaaaaaa");
                Console.WriteLine(pipoint.GetStateSet());
            }
            else
            {
                Console.WriteLine("wazaaaaaaaaaaaaaaaaaaa");
                //Console.ReadKey();
                //tata["digitalset"] = titi;
                if (!PIServerHQ.StateSets.Contains(digitalset))
                {
                    Console.WriteLine("ds nexiste pas on lke cree");
                    PIServerHQ.StateSets.Add(digitalset);
                    //AFEnumerationSet tutu = PIServerHQ.StateSets.Add()
                    //PIServerHQ.StateSets.Refresh();
                    digitalset.CheckIn();
                    digitalset.ApplyChanges();
                }
                // on reteste
                if (PIServerHQ.StateSets.Contains(digitalset))
                {
                    PIServerHQ.CreatePIPoint(tag, tata);
                    Console.WriteLine("creation OKKKKKKKKKKKKKKKKK");
                }
            }
            Console.ReadKey();
        }
    }
}

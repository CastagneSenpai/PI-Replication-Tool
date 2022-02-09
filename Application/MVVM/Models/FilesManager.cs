using Microsoft.VisualBasic.FileIO;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class FilesManager
    {
        readonly Logger Logger = LogManager.GetLogger("PIReplicationToolLogger");

        //TextFieldParser 
        public void ParseInputFileToTagsList(ref List<string> p_TagsList)
        {
            try
            {
                int counter = 0;
                foreach (string line in File.ReadLines(@"E:\test.txt"))
                {
                    p_TagsList.Add(line.ToString());
                    counter++;
                    Logger.Debug($"Tag {line} ({counter}) was taken into account by PI Replication Tool");
                }
            }
            catch(Exception e)
            {
                Logger.Error($"Error parsing input file to a tag list. {e.Message}");
            }
        }

        public void CreateTagsOutputFile(List<string> p_AttributesValueList)
        {
            try
            {
                //TODO : différencier si source ou target file
                string outputFileFullName = Constantes.OutputPath + Constantes.OutputFileName_SourceTags + DateTime.Now.ToString() + "csv";
                File.Create(outputFileFullName);

                // Prepare to write in the output file
                foreach(string line in p_AttributesValueList)
                {
                    string[] currentTagAttributes = line.Split(';');
                    File.WriteAllLines(outputFileFullName, currentTagAttributes);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error writing tag attributes list to output file. {e.Message}");
            }
        }
    }
}

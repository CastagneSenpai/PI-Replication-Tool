using NLog;
using System;
using System.Collections.Generic;
using System.IO;

namespace Models
{
    public static class FilesManager
    {
        static readonly Logger Logger = LogManager.GetLogger("PIReplicationToolLogger");

        //TextFieldParser 
        public static void ParseInputFileToTagsList(ref List<string> p_TagsList)
        {
            try
            {
                int counter = 0;
                foreach (string line in File.ReadLines(@"C:\Users\WL1000621\Desktop\input.txt.txt"))
                //foreach (string line in File.ReadLines(Constants.InputPath + Constants.InputFileName))
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

        //public static void CreateTagsOutputFile(List<string> p_AttributesValueList)
        public static void CreateTagsOutputFile(List<IDictionary<string, object>> p_AttributesValueList)
        {
            try
            {
                //TODO : différencier si source ou target file (pour le moment,, source uniquement)
                string outputFileFullName = Constants.OutputPath + Constants.OutputFileName_SourceTags + DateTime.Now.ToString() + "csv";
                File.Create(outputFileFullName);

                // Prepare to write in the output file
                //foreach(string line in p_AttributesValueList)
                //{
                 //   string[] currentTagAttributes = line.Split(';');
                //    File.WriteAllLines(outputFileFullName, currentTagAttributes);
                //}
            }
            catch (Exception e)
            {
                Logger.Error($"Error writing tag attributes list to output file. {e.Message}");
            }
        }
    }
}

using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

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
                foreach (string line in File.ReadLines(@"C:\Users\WL1000621\Desktop\input.txt"))
                //foreach (string line in File.ReadLines(Constants.InputPath + Constants.InputFileName))
                {
                    p_TagsList.Add(line.ToString());
                    counter++;
                    Logger.Debug($"Tag {line} ({counter}) was taken into account by PI Replication Tool");
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error parsing input file to a tag list. {e.Message}");
            }
        }

        public static void CreateTagsOutputFile(List<IDictionary<string, object>> p_AttributesValueList)
        {
            try
            {
                // Create list<string> object to store the content of the file
                List<string> FileLines = new List<string>();
                FileLines.Clear();

                // Add header of the file using key names
                string header = "";
                foreach (string attributesName in p_AttributesValueList[0].Keys)
                {
                    header += attributesName + Constants.fieldSeparator;
                }
                FileLines.Add(header);

                // Format each tags attributes in one line - string format with separator ";"
                foreach (IDictionary<string, object> currentTag in p_AttributesValueList)
                {
                    string currentLine = "";
                    foreach (object attribute in currentTag.Values)
                    {
                        currentLine += attribute.ToString() + Constants.fieldSeparator;
                    }
                    FileLines.Add(currentLine);
                }

                // Write lines in output file
                //TODO : différencier si source ou target file (pour le moment, source uniquement)
                string outputFileFullName = Constants.OutputPath + Constants.OutputFileName_SourceTags + "_" + DateTime.Now.ToString("yyyy-MM-ddThh-mm-ss") + ".csv";
                //File.Create(outputFileFullName);
                File.WriteAllLines(outputFileFullName, FileLines);
                FileLines.Clear();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + e.StackTrace);
                Logger.Error($"Error writing tag attributes list to output file. {e.Message}");
            }
        }
    }
}

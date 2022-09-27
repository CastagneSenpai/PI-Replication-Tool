using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Windows;

namespace Models
{
    public static class FilesManager
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //TextFieldParser 
        public static void ParseInputFileToTagsList(ref List<string> p_TagsList)
        {
            Logger.Info($"Call method FileManager.ParseInputFileToTagsList.");
            try
            {
                int counter = 0;
                //foreach (string line in File.ReadLines(@"C:\Users\WL1000621\Desktop\input.txt"))
                foreach (string line in File.ReadLines(ConfigurationManager.AppSettings["InputPath"] + ConfigurationManager.AppSettings["InputFileName"]))
                {
                    p_TagsList.Add(line.ToString());
                    counter++;
                    Logger.Debug($"Tag {line} ({counter}) read from input file.");
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error parsing input file to a tag list. {e.Message}");
            }
            Logger.Info($"End method FileManager.ParseInputFileToTagsList.");
        }


        public static void CreateTagsOutputFile(List<IDictionary<string, object>> p_AttributesValueList, BackupType p_BackupType)
        {
            Logger.Info($"Call method FileManager.CreateTagsOutputFile.");
            try
            {
                if(p_AttributesValueList.Count>0)
                {
                    // Create list<string> object to store the content of the file
                    List<string> FileLines = new List<string>();
                    FileLines.Clear();

                    // Add header of the file using key names
                    string header = "";
                    foreach (string attributesName in p_AttributesValueList[0].Keys)
                    {
                        header += attributesName + ConfigurationManager.AppSettings["fieldSeparator"];
                    }
                    FileLines.Add(header);

                    // Format each tags attributes in one line - string format with separator ";"
                    foreach (IDictionary<string, object> currentTag in p_AttributesValueList)
                    {
                        string currentLine = "";
                        foreach (object attribute in currentTag.Values)
                        {
                            currentLine += attribute.ToString() + ConfigurationManager.AppSettings["fieldSeparator"];
                        }
                        FileLines.Add(currentLine);
                    }

                    // Write lines in output file
                    string OutputFileName;
                    if (p_BackupType == BackupType.SourceServerBackup)
                        OutputFileName = ConfigurationManager.AppSettings["OutputFileName_SourceTags"];
                    else
                        OutputFileName = ConfigurationManager.AppSettings["OutputFileName_TargetTags"]; ;

                    string outputFileFullName = ConfigurationManager.AppSettings["OutputPath"] + OutputFileName + "_" + DateTime.Now.ToString("yyyy-MM-ddThh-mm-ss") + ".csv";

                    // To remove because it lock the file : File.Create(outputFileFullName);
                    File.WriteAllLines(outputFileFullName, FileLines);
                    FileLines.Clear();
                }
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + e.StackTrace);
                Logger.Error($"Error writing tag attributes list to output file. {e.Message}");
            }
            Logger.Info($"End method FileManager.CreateTagsOutputFile.");
        }
    }

    // Define if the backup file which contains tags configuration is before or after the replication
    public enum BackupType
    {
        SourceServerBackup = 0,
        TargetServerBackup = 1
    }
}

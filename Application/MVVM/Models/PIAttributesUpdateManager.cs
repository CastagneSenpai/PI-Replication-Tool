using NLog;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using OSIsoft.AF.PI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows;

namespace Models
{
    public sealed class PIAttributesUpdateManager
    {
        #region Properties
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public List<IDictionary<string, object>> AttributesTagsList { get; set; } = new List<IDictionary<string, object>>();
        public HashSet<AFEnumerationSet> DigitalSetList { get; set; } = new HashSet<AFEnumerationSet>();
        public ICollection<PIPointSource> PointSources_Digital { get; set; } = new Collection<PIPointSource>();
        public ICollection<PIPointSource> PointSources_Numerical { get; set; } = new Collection<PIPointSource>();
        Dictionary<string, long> NumericalPSAndRemainingSpace { get; set; } = new Dictionary<string, long>();
        Dictionary<string, long> DigitalPSAndRemainingSpace { get; set; } = new Dictionary<string, long>();
        public string Trigram { get; set; } = "";
        public bool IsLoadingTimeOver { get; set; } = false;
        #endregion

        #region Constructor
        public PIAttributesUpdateManager() { }
        #endregion

        #region Methods
        public void LoadTagsAttributes(PIServer p_PIServer, List<string> p_PITagNames, ref IProgress<double> p_progress)
        {
            Logger.Info("Call method PIAttributeUpdateManager.LoadTagsAttributes");
            this.Clear();
            List<PIPoint> v_PIPointList = new List<PIPoint>();

            int v_TagProgress = 0;

            // Load tags attributes from tag input list (string)
            foreach (string piTagNames in p_PITagNames)
            {
                
                v_TagProgress++;

                // TODO: Changer FindPIPoint par FindPIPoints pour améliorer les perf 
                // https://docs.osisoft.com/bundle/af-sdk/page/html/T_OSIsoft_AF_PI_PIPoint.htm
                // TODO: TryFindPIPoint pour éviter de lever une exception si le tag n'existe pas

                PIPoint v_PIPoint = null;
                bool v_DoPIPointExist = PIPoint.TryFindPIPoint(p_PIServer, piTagNames, out v_PIPoint);
                Logger.Debug($"Start processing {piTagNames}");

                if (v_DoPIPointExist)
                {
                    Logger.Debug($"{v_PIPoint.Name} - Point exist on {p_PIServer.Name}.");
                    //v_PIPointList.Add(v_PIPoint);
                    if (v_PIPoint.PointType.Equals(PIPointType.Digital))
                    {
                        Logger.Debug($"{v_PIPoint.Name} - Point is digital.");
                        DigitalSetList.Add(v_PIPoint.GetStateSet());
                    }

                    Logger.Debug($"{v_PIPoint.Name} - Getting Point Attributes.");
                    var v_CurrentTagAttributes = v_PIPoint.GetAttributes();
                    AttributesTagsList.Add(v_CurrentTagAttributes);
                    p_progress.Report(v_TagProgress);

                    // Display tags attributes in the data grid
                    Logger.Debug($"{v_PIPoint.Name} - Populate Grid with this tag attributes.");
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        PIReplicationManager.ReplicationManager.DataGridCollection.PopulateGridLineByLine(v_CurrentTagAttributes);
                    });

                    Logger.Debug($"{v_PIPoint.Name} - Tag well displayed in the data grid");
                }
                else
                {
                    Logger.Warn($"The PI Point {piTagNames} does not exist in PI server {p_PIServer}. It will be skipped from the replication.");
                }

                Logger.Debug($"Stop processing {v_PIPoint.Name}");
            }
            Logger.Info("End method PIAttributeUpdateManager.LoadTagsAttributes");
        }
        public void UpdateTagsAttributes(PIServer p_PISourceServer, PIServer p_PITargetServer)
        {
            Logger.Info("Call method PIAttributeUpdateManager.UpdateTagsAttributes");
            this.GetTrigrammeFromPIServer(p_PISourceServer);
            if (this.IsThereEnoughtPointSourcesAvailable(p_PITargetServer))
            {
                // Apply update for each tag using List<T>.ForEach() method (most efficient way to proceed)
                AttributesTagsList.ForEach(TagAttributes => UpdateTagAttributes(p_PISourceServer, TagAttributes));
            }
            else
            {
                string v_Message = "Not enought PointSources available for this replication, please create more PItoPI interfaces before continue.";
                Console.WriteLine(v_Message);
                throw new Exception(v_Message);
            }
            Logger.Info("Call method PIAttributeUpdateManager.UpdateTagsAttributes");
        }
        private void UpdateTagAttributes(PIServer p_PISourceServer, IDictionary<string, object> p_TagAttributes)
        {
            Logger.Debug("Call method PIAttributeUpdateManager.UpdateTagAttributes for tag " + p_TagAttributes["tag"]);
            try
            {
                this.UpdatePointSourceAttributes(ref p_TagAttributes);
                this.UpdateSecurityAttributes(ref p_TagAttributes);
                this.UpdateTagNameAndInstrumentTag(ref p_TagAttributes, p_PISourceServer);

                // Actions on Numerical tags only
                if (!(p_TagAttributes["pointtype"].ToString() == "Digital" || p_TagAttributes["pointtype"].ToString() == "String"))
                {
                    this.UpdateCompressionExceptionAttributes(ref p_TagAttributes);
                    this.VerifyTypicalValues(ref p_TagAttributes);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error updating tag {p_TagAttributes["tag"]} attributes. {e.Message}");
            }
            Logger.Debug("End method PIAttributeUpdateManager.UpdateTagAttributes for tag " + p_TagAttributes["tag"]);
        }
        public void UpdatePointSourceAttributes(ref IDictionary<string, object> p_TagAttributes)
        {
            Logger.Debug("Call method PIAttributeUpdateManager.UpdatePointSourceAttributes for tag " + p_TagAttributes["tag"]);
            try
            {
                p_TagAttributes["location1"] = 0;
                if (p_TagAttributes[PICommonPointAttributes.PointType].Equals(PIPointType.Digital) || p_TagAttributes[PICommonPointAttributes.PointType].Equals(PIPointType.String))
                {
                    // Get the PointSource and available space pair
                    var v_PointSource = GetPointSourceForCurrentTag(this.DigitalPSAndRemainingSpace);

                    // Set the PointSource
                    p_TagAttributes["pointsource"] = v_PointSource.Key;

                    // -1 free space for this PointSource
                    this.DigitalPSAndRemainingSpace[v_PointSource.Key] -= 1;
                }
                else
                {
                    var v_PointSource = GetPointSourceForCurrentTag(this.NumericalPSAndRemainingSpace);

                    // Set the PointSource              
                    p_TagAttributes["pointsource"] = v_PointSource.Key;

                    // -1 free space for this PointSource
                    this.NumericalPSAndRemainingSpace[v_PointSource.Key] -= 1;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error updating PointSource attribute for {p_TagAttributes["tag"]}. {e.Message}");
            }

            Logger.Debug("End method PIAttributeUpdateManager.UpdatePointSourceAttributes for tag " + p_TagAttributes["tag"]);
        }
        private void UpdateCompressionExceptionAttributes(ref IDictionary<string, object> p_TagAttributes)
        {
            Logger.Debug("Call method PIAttributeUpdateManager.UpdateCompressionExceptionAttributes for tag " + p_TagAttributes["tag"]);
            try
            {
                // Put compression & exception parameter to 0; except "Compression" which is only updated if tag is numerical
                p_TagAttributes["compressing"] = 0;
                p_TagAttributes["compdev"] = 0;
                p_TagAttributes["compmin"] = 0;
                p_TagAttributes["compmax"] = 0;
                p_TagAttributes["compdevpercent"] = 0;
                p_TagAttributes["excdev"] = 0;
                p_TagAttributes["excmin"] = 0;
                p_TagAttributes["excmax"] = 0;
                p_TagAttributes["excdevpercent"] = 0;
            }
            catch (Exception e)
            {
                Logger.Error($"Error updating compression and exception attributes for tag {p_TagAttributes["tag"]}. {e.Message}");
            }

            Logger.Debug("End method PIAttributeUpdateManager.UpdateCompressionExceptionAttributes for tag " + p_TagAttributes["tag"]);
        }
        private void UpdateSecurityAttributes(ref IDictionary<string, object> p_TagAttributes)
        {
            Logger.Debug("Call method PIAttributeUpdateManager.UpdateSecurityAttributes for tag " + p_TagAttributes["tag"]);
            try
            {
                p_TagAttributes["datasecurity"] = ConfigurationManager.AppSettings["PISecurityConfiguration"];
                p_TagAttributes["ptsecurity"] = ConfigurationManager.AppSettings["PISecurityConfiguration"];
            }
            catch (Exception e)
            {
                Logger.Error($"Error updating security attributes. {e.Message}");
            }
            Logger.Debug("End method PIAttributeUpdateManager.UpdateSecurityAttributes for tag " + p_TagAttributes["tag"]);
        }
        private void VerifyTypicalValues(ref IDictionary<string, object> p_TagAttributes)
        {
            Logger.Debug("Call method PIAttributeUpdateManager.VerifyTypicalValues for tag " + p_TagAttributes["tag"]);
            try
            {
                float v_Zero, v_Typicalvalue, v_Span;

                v_Zero = (float)p_TagAttributes["zero"];
                v_Typicalvalue = (float)p_TagAttributes["typicalvalue"];
                v_Span = (float)p_TagAttributes["span"];

                // If typicalvalue do not respect PI rules in source server, remove typical value
                if (v_Typicalvalue < v_Zero || v_Typicalvalue > v_Zero + v_Span)
                {
                    p_TagAttributes["typicalvalue"] = null;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error verifying typicalValue, zero or span attributes for tag { p_TagAttributes["tag"]}. {e.Message}");
            }

            Logger.Debug("End method PIAttributeUpdateManager.VerifyTypicalValues for tag " + p_TagAttributes["tag"]);
        }
        private void UpdateTagNameAndInstrumentTag(ref IDictionary<string, object> p_TagAttributes, PIServer p_PISourceServer)
        {
            Logger.Debug("Call method PIAttributeUpdateManager.UpdateTagNameAndInstrumentTag for tag " + p_TagAttributes["tag"]);
            try
            {
                // TODO : externaliser dans le fichier config
                // Update TagName if source server is TEPNL, TEPUK or TEPGB
                if (this.Trigram == "NLD")
                {
                    p_TagAttributes["instrumenttag"] = p_TagAttributes["tag"];
                    p_TagAttributes["tag"] = "NLD_" + p_TagAttributes["tag"]; // NL : Prefixe NLD_
                }
                else if (this.Trigram == "ABZ")
                {
                    p_TagAttributes["instrumenttag"] = p_TagAttributes["tag"];
                    p_TagAttributes["tag"] = "UK_" + p_TagAttributes["tag"]; // UK : Prefixe UK_
                }
                else if (this.Trigram == "POG")
                {
                    p_TagAttributes["instrumenttag"] = p_TagAttributes["tag"];
                    p_TagAttributes["tag"] = "POG_" + p_TagAttributes["tag"]; // GB : Prefixe POG_
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error updating TagName and Instrumenttag attributes. {e.Message}");
            }

            Logger.Debug("End method PIAttributeUpdateManager.UpdateTagNameAndInstrumentTag for tag " + p_TagAttributes["tag"]);
        }
        private bool IsThereEnoughtPointSourcesAvailable(PIServer p_PITargetServer)
        {
            Logger.Debug("Call method PIAttributeUpdateManager.IsThereEnoughtPointSourcesAvailable.");

            // Clear PIAttributesUpdateManager lists in case of multiple click on <Update> button (keep AttributesTagsList only)
            this.PointSources_Digital.Clear();
            this.PointSources_Numerical.Clear();
            this.NumericalPSAndRemainingSpace.Clear();
            this.DigitalPSAndRemainingSpace.Clear();

            int v_NbNumericalTagsToReplicate = 0, v_NbDigitalTagsToReplicate = 0;

            // Count how many digital & numerical tags have to be replicated
            foreach (IDictionary<string, object> v_AttributesTag in AttributesTagsList)
            {
                if (v_AttributesTag["pointtype"].ToString() == "digital" || v_AttributesTag["pointtype"].ToString() == "string")
                    v_NbDigitalTagsToReplicate++;
                else
                    v_NbNumericalTagsToReplicate++;
            }

            // Update collection PointSources_Numerical & PointSources_Digital using the Trigram and the PIPointSources list
            foreach (PIPointSource v_PS in p_PITargetServer.PointSources)
            {
                if (v_PS.Name.Contains(this.Trigram) && v_PS.Name.Contains("N0"))
                {
                    this.PointSources_Numerical.Add(v_PS);
                }

                else if (v_PS.Name.Contains(this.Trigram) && v_PS.Name.Contains("D0"))
                {
                    this.PointSources_Digital.Add(v_PS);
                }
            }

            // Count how many space is available for numerical and digital PIPointSources list
            // >> Update NumericalPSAndRemainingSpace/DigitalPSAndRemainingSpace attributes
            long v_NumericalMaxPointCountAllowed = int.Parse(ConfigurationManager.AppSettings["NumericalMaxPointCountAllowed"]);
            long v_AvailableNumericalPointSpace = this.PointSources_Numerical.Sum(v_PS =>
            {
                long v_RemainingSpace = v_NumericalMaxPointCountAllowed - v_PS.PointCount;
                this.NumericalPSAndRemainingSpace.Add(v_PS.Name, v_RemainingSpace);
                return v_RemainingSpace;
            });


            long v_DigitalMaxPointCountAllowed = int.Parse(ConfigurationManager.AppSettings["DigitalMaxPointCountAllowed"]);
            long v_AvailableDigitalPointSpace = this.PointSources_Digital.Sum(v_PS =>
            {
                long v_RemainingSpace = v_DigitalMaxPointCountAllowed - v_PS.PointCount;
                this.DigitalPSAndRemainingSpace.Add(v_PS.Name, v_RemainingSpace);
                return v_RemainingSpace;
            });

            Logger.Debug("End method PIAttributeUpdateManager.IsThereEnoughtPointSourcesAvailable.");
            return (v_AvailableNumericalPointSpace >= v_NbNumericalTagsToReplicate && v_AvailableDigitalPointSpace >= v_NbDigitalTagsToReplicate);
        }
        private KeyValuePair<string, long> GetPointSourceForCurrentTag(Dictionary<string, long> p_PSAndRemainingSpace)
        {
            KeyValuePair<string, long> v_SelectedPointSource = p_PSAndRemainingSpace.First();
            foreach (KeyValuePair<string, long> v_CurrentPointSource in p_PSAndRemainingSpace)
            {
                if (v_CurrentPointSource.Value > v_SelectedPointSource.Value) v_SelectedPointSource = v_CurrentPointSource;
            }
            return v_SelectedPointSource;
        }
        public void GetTrigrammeFromPIServer(PIServer p_PIServer)
        {
            Logger.Debug("Call method PIAttributeUpdateManager.GetTrigrammeFromPIServer.");
            this.Trigram = ConfigurationManager.AppSettings["Trigram_" + p_PIServer.Name];
            if (this.Trigram == "Trigram_")
            {
                throw new Exception("PI server name does not exist is configuration file.");
            }
            Logger.Debug("End method PIAttributeUpdateManager.GetTrigrammeFromPIServer.");
        }
        public void Clear()
        {
            Logger.Debug("Call method PIAttributeUpdateManager.Clear.");
            this.AttributesTagsList.Clear();
            this.PointSources_Digital.Clear();
            this.PointSources_Numerical.Clear();
            this.NumericalPSAndRemainingSpace.Clear();
            this.DigitalPSAndRemainingSpace.Clear();
            this.Trigram = "";
            Logger.Debug("End method PIAttributeUpdateManager.Clear.");
        }

        void PrepareDigitalSetOnTargetServer(PIServer p_TargetPIServer)
        {
            var v_ListDigitalSetTarget = p_TargetPIServer.StateSets;

            foreach (var v_DigitalSet in PIReplicationManager.ReplicationManager.PIAttributesUpdateManager.DigitalSetList)
            {
                if (v_ListDigitalSetTarget.Contains(v_DigitalSet.Name))
                {
                    Logger.Debug($"Digital Set ({v_DigitalSet.Name}) already exists on target server {p_TargetPIServer.Name}.");
                }
                else
                {
                    Logger.Info($"Creating Digital Set ({v_DigitalSet.Name}) on target server {p_TargetPIServer.Name}.");
                    p_TargetPIServer.StateSets.Add(v_DigitalSet);
                    v_DigitalSet.CheckIn();
                    v_DigitalSet.ApplyChanges();
                }
            }
        }

        public void CreateAndPushTags(PIServer p_targetServer)
        {
            Logger.Info($"Call method PIAttributeUpdateManager.CreateAndPushTags for {p_targetServer.Name}.");
            IDictionary<string, IDictionary<string, object>> v_tagsListToBeCreated = new Dictionary<string, IDictionary<string, object>>();

            PrepareDigitalSetOnTargetServer(p_targetServer);
            AttributesTagsList.ForEach(p_tag =>
            {
                if (p_tag[PICommonPointAttributes.PointType].Equals(PIPointType.Digital))
                {
                    try
                    {
                        v_tagsListToBeCreated.Add(GetTagname(p_tag), GetCustomAttributes(p_tag, true));
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Cannot add {GetTagname(p_tag)} to the replication list. Please check that the tag isn't duplicated in your input tag list. {ex.Message}.");
                    }
                }
                else
                {
                    try
                    {
                        v_tagsListToBeCreated.Add(GetTagname(p_tag), GetCustomAttributes(p_tag));
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Cannot add {GetTagname(p_tag)} to the replication list. Please check that the tag isn't duplicated in your input tag list. {ex.Message}.");
                    }
                }
            });

            try
            {
                AFListResults<string, PIPoint> v_ReturnListTagsCreated = p_targetServer.CreatePIPoints(v_tagsListToBeCreated);
                if (v_ReturnListTagsCreated.HasErrors)
                {
                    foreach (var v_Error in v_ReturnListTagsCreated.Errors)
                    {
                        Logger.Warn($"Error while creating tag {v_Error.Key} - {v_Error.Value}");
                    }
                }
            }
            catch (AggregateException e)
            {
                Logger.Error($"Error in method PIAttributeUpdateManager.CreateAndPushTags for {p_targetServer.Name}. {e.Message}");
                throw new Exception();
            }
            catch (PIException e)
            {
                Logger.Error($"Error in method PIAttributeUpdateManager.CreateAndPushTags for {p_targetServer.Name}. {e.Message}");
                throw new Exception();
            }

            Logger.Info($"End method PIAttributeUpdateManager.CreateAndPushTags for {p_targetServer.Name}.");
        }
        public void UpdateAndPushTags(PIServer p_targetServer)
        {
            Logger.Info($"Call method PIAttributeUpdateManager.UpdateAndPushTags for {p_targetServer.Name}.");
            AttributesTagsList.ForEach(p_tag =>
            {
                try
                {
                    PIPoint v_CurrentPIPoint = PIPoint.FindPIPoint(p_targetServer, GetTagname(p_tag));
                    v_CurrentPIPoint.SetAttribute(GetTagname(p_tag), GetCustomAttributes(p_tag));
                    v_CurrentPIPoint.SaveAttributes();
                }
                catch (AggregateException e)
                {
                    Logger.Error($"Error in method PIAttributeUpdateManager.UpdateAndPushTags for {p_targetServer.Name}. {e.Message}");
                    throw new Exception();
                }
                catch (PIException e)
                {
                    Logger.Warn($"Tag {GetTagname(p_tag)} not found in {p_targetServer.Name} : It cannot be updated because UpdateOnly Mode was check. {e.Message}");
                }
            });
            Logger.Info($"End method PIAttributeUpdateManager.UpdateAndPushTags for {p_targetServer.Name}.");
        }
        public void CreateOrUpdateAndPushTags(PIServer targetServer)
        {
            Logger.Info($"Call method PIAttributeUpdateManager.CreateOrUpdateAndPushTags for {targetServer.Name}.");
            IDictionary<string, IDictionary<string, object>> v_tagsListToBeCreated = new Dictionary<string, IDictionary<string, object>>();
            AttributesTagsList.ForEach(p_tag =>
            {

                if (PIPoint.TryFindPIPoint(targetServer, GetTagname(p_tag), out PIPoint v_CurrentPIPoint))
                {
                    // Tag exist on target server : Update it with new configuration
                    Logger.Debug($"Update tag {GetTagname(p_tag)} in {targetServer}");
                    v_CurrentPIPoint.SetAttribute(GetTagname(p_tag), GetCustomAttributes(p_tag));
                    v_CurrentPIPoint.SaveAttributes();
                }
                else
                {
                    // Tag does not exist : Add it to the list of creation tags.
                    if (p_tag[PICommonPointAttributes.PointType].Equals(PIPointType.Digital))
                    {
                        try
                        {
                            v_tagsListToBeCreated.Add(GetTagname(p_tag), GetCustomAttributes(p_tag, true));
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"Cannot add {GetTagname(p_tag)} to the replication list. Please check that the tag isn't duplicated in your input tag list. {ex.Message}.");
                        }
                    }
                    else
                    {
                        try
                        {
                            v_tagsListToBeCreated.Add(GetTagname(p_tag), GetCustomAttributes(p_tag));
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"Cannot add {GetTagname(p_tag)} to the replication list. Please check that the tag isn't duplicated in your input tag list. {ex.Message}.");
                        }
                    }
                }
            });

            try
            {
                Logger.Debug($"Creating tags which are not updated et in {targetServer.Name}");
                AFListResults<string, PIPoint> v_ReturnListTagsCreated = targetServer.CreatePIPoints(v_tagsListToBeCreated);
                if (v_ReturnListTagsCreated.HasErrors)
                {
                    foreach (var v_Error in v_ReturnListTagsCreated.Errors)
                    {
                        Logger.Warn($"Error while creating tag {v_Error.Key} - {v_Error.Value}");
                    }
                }
            }
            catch (AggregateException e)
            {
                Logger.Error($"Error in method PIAttributeUpdateManager.CreateOrUpdateAndPushTags for {targetServer.Name}. {e.Message}");
                throw new Exception();
            }
            catch (PIException e)
            {
                Logger.Error($"Error in method PIAttributeUpdateManager.CreateOrUpdateAndPushTags for {targetServer.Name}. {e.Message}");
                throw new Exception();
            }
            Logger.Info($"End method PIAttributeUpdateManager.CreateOrUpdateAndPushTags for {targetServer.Name}.");
        }
        public string GetTagname(IDictionary<string, object> listeAttributs)
        {
            return listeAttributs[PICommonPointAttributes.Tag].ToString();
        }
        public IDictionary<string, object> GetCustomAttributes(IDictionary<string, object> p_Attributes, bool p_IsDigital = false)
        {
            Dictionary<string, object> p_CommonAttributes = p_Attributes
                .Where(v_Attributes => Constants.CommonAttribute.Any(v_CommonAttributes => v_CommonAttributes == v_Attributes.Key))
                .ToDictionary(k => k.Key, v => v.Value);

            if (p_IsDigital)
            {
                p_CommonAttributes.Add(PICommonPointAttributes.DigitalSetName, p_Attributes[PICommonPointAttributes.DigitalSetName]);
                p_CommonAttributes.Remove(PICommonPointAttributes.Zero);
                p_CommonAttributes.Remove(PICommonPointAttributes.Span);
                p_CommonAttributes.Remove(PICommonPointAttributes.TypicalValue);
            }

            return p_CommonAttributes;
        }
        public void GetCurrentValues(PIServer p_targetServer, IDictionary<string, object> p_TagAttributes)
        {
            Logger.Debug($"Call method PIAttributeUpdateManager.GetCurrentValues for {p_targetServer.Name}.");
            string v_Tagname = GetTagname(p_TagAttributes);
            var v_TagFound = PIPoint.TryFindPIPoint(p_targetServer, v_Tagname, out PIPoint v_Tag);

            if (v_TagFound)
            {
                AFValue v_PIvalue = v_Tag.CurrentValue();
                if (v_PIvalue.IsGood)
                {
                    PIReplicationManager.ReplicationManager.DataGridCollection.UpdateGridStatus(p_TagAttributes, Constants.TagStatus.Replicated, v_PIvalue.Value, v_PIvalue.Timestamp);
                }
                else
                {
                    PIReplicationManager.ReplicationManager.DataGridCollection.UpdateGridStatus(p_TagAttributes, Constants.TagStatus.PtCreated, v_PIvalue.Value, v_PIvalue.Timestamp);
                }
            }
            else
            {
                PIReplicationManager.ReplicationManager.DataGridCollection.UpdateGridStatus(p_TagAttributes, Constants.TagStatus.Error, "No tag found", OSIsoft.AF.Time.AFTime.MinValue);
            }
            Logger.Debug($"End method PIAttributeUpdateManager.GetCurrentValues for {p_targetServer.Name}.");
        }
    }
    #endregion Methods
}
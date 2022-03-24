using NLog;
using OSIsoft.AF;
using OSIsoft.AF.PI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;

namespace Models
{
    public sealed class PIAttributesUpdateManager
    {
        #region Properties
        static readonly Logger Logger = LogManager.GetLogger("PIReplicationToolLogger");
        public List<IDictionary<string, object>> AttributesTagsList { get; set; } = new List<IDictionary<string, object>>();
        public ICollection<PIPointSource> PointSources_Digital { get; set; } = new Collection<PIPointSource>();
        public ICollection<PIPointSource> PointSources_Numerical { get; set; } = new Collection<PIPointSource>();
        Dictionary<string, long> NumericalPSAndRemainingSpace { get; set; } = new Dictionary<string, long>();
        Dictionary<string, long> DigitalPSAndRemainingSpace { get; set; } = new Dictionary<string, long>();
        public string Trigram { get; set; } = "";
        #endregion

        #region Constructor
        public PIAttributesUpdateManager() { }
        #endregion

        #region Methods
        public void LoadTagsAttributes(PIServer p_PIServer, List<string> p_PITagNames)
        {
            this.Clear();
            List<PIPoint> v_PIPointList = new List<PIPoint>();

            foreach (string piTagNames in p_PITagNames)
            {
                try
                {
                    // TODO: Changer FindPIPoint par FindPIPoints pour améliorer les perf 
                    // https://docs.osisoft.com/bundle/af-sdk/page/html/T_OSIsoft_AF_PI_PIPoint.htm
                    var v_PIPoint = PIPoint.FindPIPoint(p_PIServer, piTagNames);
                    v_PIPointList.Add(v_PIPoint);
                }
                catch
                {
                    Logger.Warn($"The PI Point {piTagNames} does not exist in PI server {p_PIServer}. It will be skipped from the replication.");
                }
            }

            foreach (var v_PIPoint in v_PIPointList)
            {
                AttributesTagsList.Add(v_PIPoint.GetAttributes());
            }
        }
        public void UpdateTagsAttributes(PIServer p_PISourceServer, PIServer p_PITargetServer)
        {
            this.GetTrigrammeFromPIServer(p_PISourceServer);
            if (this.IsThereEnoughtPointSourcesAvailable(p_PITargetServer))
            {
                // Apply update for each tag using List<T>.ForEach() method (most efficient way to proceed)
                AttributesTagsList.ForEach(TagAttributes => UpdateTagAttributes(p_PISourceServer, TagAttributes));
            }
            else
            {
                throw new Exception("Not enought PointSources available for this replication, please create more PItoPI interfaces before continue");
            }
        }
        private void UpdateTagAttributes(PIServer p_PISourceServer, IDictionary<string, object> p_TagAttributes)
        {
            try
            {
                this.UpdatePointSourceAttributes(ref p_TagAttributes);
                this.UpdateCompressionExceptionAttributes(ref p_TagAttributes);
                this.UpdateSecurityAttributes(ref p_TagAttributes);
                this.UpdateTagNameAndInstrumentTag(ref p_TagAttributes, p_PISourceServer);

                // Actions on Numerical tags only
                if (!(p_TagAttributes["pointtype"].ToString() == "digital" || p_TagAttributes["pointtype"].ToString() == "string"))
                {
                    this.VerifyTypicalValues(ref p_TagAttributes);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error updating tag {p_TagAttributes["tag"]} attributes. {e.Message}");
            }
        }
        private void UpdatePointSourceAttributes(ref IDictionary<string, object> p_TagAttributes)
        {
            try
            {
                if (p_TagAttributes["pointtype"].ToString() == "digital" || p_TagAttributes["pointtype"].ToString() == "string")
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
                Logger.Error($"Error updating PointSource attribute. {e.Message}");
            }
        }
        private void UpdateCompressionExceptionAttributes(ref IDictionary<string, object> p_TagAttributes)
        {
            try
            {
                // Put compression & exception parameter to 0
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
                Logger.Error($"Error updating compression and exception attributes. {e.Message}");
            }
        }
        private void UpdateSecurityAttributes(ref IDictionary<string, object> p_TagAttributes)
        {
            try
            {
                p_TagAttributes["datasecurity"] = ConfigurationManager.AppSettings["PISecurityConfiguration"];
                p_TagAttributes["ptsecurity"] = ConfigurationManager.AppSettings["PISecurityConfiguration"];
            }
            catch (Exception e)
            {
                Logger.Error($"Error updating security attributes. {e.Message}");
            }
        }
        private void VerifyTypicalValues(ref IDictionary<string, object> p_TagAttributes)
        {
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
                Logger.Error($"Error verifying typicalValue, zero or span attributes. {e.Message}");
            }
        }
        private void UpdateTagNameAndInstrumentTag(ref IDictionary<string, object> p_TagAttributes, PIServer p_PISourceServer)
        {
            try
            {
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
        }
        private bool IsThereEnoughtPointSourcesAvailable(PIServer p_PITargetServer)
        {
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
                // TODO: Clear la liste dans le cas où on clique 2 fois sur le bouton update
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

            if (v_AvailableNumericalPointSpace >= v_NbNumericalTagsToReplicate && v_AvailableDigitalPointSpace >= v_NbDigitalTagsToReplicate)
            {
                return true;
            }
            else
                return false;
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
        public void GetTrigrammeFromPIServer(PIServer p_PIServer) // TODO : Modifier pour aller chercher le trigramme dans le fichier de config
        {
            this.Trigram = ConfigurationManager.AppSettings["Trigram_" + p_PIServer.Name];
            if (this.Trigram == "Trigram_")
            {
                throw new Exception("PI server name does not exist is configuration file.");
            }
        }
        public void Clear()
        {
            this.AttributesTagsList.Clear();
            this.PointSources_Digital.Clear();
            this.PointSources_Numerical.Clear();
            this.NumericalPSAndRemainingSpace.Clear();
            this.DigitalPSAndRemainingSpace.Clear();
            this.Trigram = "";
        }
        public void CreateAndPushTags(PIServer targetServer)
        {
            IDictionary<string, IDictionary<string, object>> listeDeTags = new Dictionary<string, IDictionary<string, object>>();
            AttributesTagsList.ForEach(p_tag =>
            {
                listeDeTags.Add(GetTagname(p_tag), GetCustomAttributes(p_tag));
            });

            try
            {
                AFListResults<string, PIPoint> retour = targetServer.CreatePIPoints(listeDeTags);
            }
            catch (AggregateException)
            {
                // NLOG
                throw new Exception();
            }
            catch (PIException)
            {
                // NLOG
                throw new Exception();
            }
        }

        public string GetTagname(IDictionary<string, object> listeAttributs)
        {
            return listeAttributs[PICommonPointAttributes.Tag].ToString();
        }

        public IDictionary<string, object> GetCustomAttributes(IDictionary<string, object> p_Attributes)
        {
            //IDictionary<string, object> v_tempList = new Dictionary<string, object>(p_Attributes);
            Dictionary<string, object> p_Common_Attributes = p_Attributes
                .Where(attributs => Constants.CommonAttribute.Any(commonAttributes => commonAttributes == attributs.Key))
                .ToDictionary(k => k.Key, v => v.Value);

            return p_Common_Attributes;
        }
        #endregion
    }
}


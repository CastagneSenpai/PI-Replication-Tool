namespace Models
{
    public class PIPointGridFormat
    {
        #region Properties
        public bool Selected { get; set; }
        public string Name { get; set; }
        public string InstrumentTag { get; set; }
        public string PointSource { get; set; }
        public int Location1 { get; set; }
        public string PointType { get; set; }
        public string DigitalSet { get; set; }
        public float Zero { get; set; }
        public float TypicalValue { get; set; }
        public float Span { get; set; }
        public int Compressing { get; set; }
        public float CompDev { get; set; }
        public float CompDevPercent { get; set; }
        public float CompMin { get; set; }
        public float ExcDev { get; set; }
        public float ExcMin { get; set; }
        public float ExcMax { get; set; }
        public float ExcDevPercent { get; set; }
        public string DataSecurity { get; set; }
        public string PtSecurity { get; set; }
        public Constants.TagStatus? Status { get; set; }
        public string CurrentTimestamp { get; set; }
        public string CurrentValue { get; set; }
        #endregion Properties

        #region Constructors
        //Constructor for load mode
        public PIPointGridFormat(bool selected, string name, string instrumentTag, string pointSource, int location1, string pointType, string digitalSet, float zero, float typicalValue, float span, int compressing, float compDev, float compDevPercent, float compMin, float excDev, float excMin, float excMax, float excDevPercent, float v, string dataSecurity, string ptSecurity)
        {

            Selected = selected;
            Name = name;
            InstrumentTag = instrumentTag;
            PointSource = pointSource;
            Location1 = location1;
            PointType = pointType;
            DigitalSet = digitalSet;
            Zero = zero;
            TypicalValue = typicalValue;
            Span = span;
            Compressing = compressing;
            CompDev = compDev;
            CompDevPercent = compDevPercent;
            CompMin = compMin;
            ExcDev = excDev;
            ExcMin = excMin;
            ExcMax = excMax;
            ExcDevPercent = excDevPercent;
            DataSecurity = dataSecurity;
            PtSecurity = ptSecurity;
            Status = null;
        }

        //Constructor for push mode
        public PIPointGridFormat(bool selected, string name, string currentTimestamp, string currentValue, string instrumentTag, string pointSource, int location1, string pointType, string digitalSet, float zero, float typicalValue, float span, int compressing, float compDev, float compDevPercent, float compMin, float excDev, float excMin, float excMax, float excDevPercent, float v, string dataSecurity, string ptSecurity, Constants.TagStatus status)
        {
            Selected = selected;
            Name = name;
            CurrentTimestamp = currentTimestamp;
            CurrentValue = currentValue;
            InstrumentTag = instrumentTag;
            PointSource = pointSource;
            Location1 = location1;
            PointType = pointType;
            DigitalSet = digitalSet;
            Zero = zero;
            TypicalValue = typicalValue;
            Span = span;
            Compressing = compressing;
            CompDev = compDev;
            CompDevPercent = compDevPercent;
            CompMin = compMin;
            ExcDev = excDev;
            ExcMin = excMin;
            ExcMax = excMax;
            ExcDevPercent = excDevPercent;
            DataSecurity = dataSecurity;
            PtSecurity = ptSecurity;
            Status = status;
        }
        #endregion Constructors
    }
}
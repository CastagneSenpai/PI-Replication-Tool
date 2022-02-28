namespace Models
{
    public class PIPointGridFormat
    {
        public PIPointGridFormat(string name, string instrumentTag, string pointType, string pointSource, int location1, float zero, float typicalValue, float span, int compressing, float compDev, float compDevPercent, float compMin, float excDev, float excMin, float excMax, float excDevPercent, float v, string dataSecurity, string ptSecurity)
        {
            Name = name;
            InstrumentTag = instrumentTag;
            PointType = pointType;
            PointSource = pointSource;
            Location1 = location1;
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
        }

        public string Name { get; set; }
        public string InstrumentTag { get; set; }
        public string PointType { get; set; }
        public string PointSource { get; set; }
        public int Location1 { get; set; }
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
    }
}
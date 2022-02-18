namespace Models
{
    public class PIPoint
    {
        public PIPoint(string name, string instrumentTag, string pointType, string pointSource, string location1, string zero, string typicalValue, string span, string compressing, string compDev, string compDevPercent, string compMin, string excDev, string excMin, string excMax, string excDevPercent, string dataSecurity, string ptSecurity)
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

        public PIPoint(string tag)
        {
            Name = tag;
        }

        public string Name { get; set; }
        public string InstrumentTag { get; set; }
        public string PointType { get; set; }
        public string PointSource { get; set; }
        public string Location1 { get; set; }
        public string Zero { get; set; }
        public string TypicalValue { get; set; }
        public string Span { get; set; }
        public string Compressing { get; set; }
        public string CompDev { get; set; }
        public string CompDevPercent { get; set; }
        public string CompMin { get; set; }
        public string ExcDev { get; set; }
        public string ExcMin { get; set; }
        public string ExcMax { get; set; }
        public string ExcDevPercent { get; set; }
        public string DataSecurity { get; set; }
        public string PtSecurity { get; set; }
    }
}
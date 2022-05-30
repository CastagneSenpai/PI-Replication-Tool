using OSIsoft.AF.PI;

namespace Models
{
    public class Constants
    {
        internal static readonly string[] CommonAttribute = {
            PICommonPointAttributes.Archiving,
            PICommonPointAttributes.InstrumentTag,
            PICommonPointAttributes.PointType,
            PICommonPointAttributes.PointSource,
            PICommonPointAttributes.Location1,
            PICommonPointAttributes.Zero,
            PICommonPointAttributes.TypicalValue,
            PICommonPointAttributes.Span,
            PICommonPointAttributes.Compressing,
            PICommonPointAttributes.CompressionDeviation,
            PICommonPointAttributes.CompressionMinimum,
            PICommonPointAttributes.CompressionMaximum,
            PICommonPointAttributes.CompressionPercentage,
            PICommonPointAttributes.ExceptionDeviation,
            PICommonPointAttributes.ExceptionMinimum,
            PICommonPointAttributes.ExceptionMaximum,
            PICommonPointAttributes.ExceptionPercentage,
            PICommonPointAttributes.DataSecurity,
            PICommonPointAttributes.PointSecurity
        };

        public enum TagStatus
        {
            PtCreated,
            Error,
            Replicated
        }

        private Constants() { }
    }
}

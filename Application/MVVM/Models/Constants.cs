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
            PICommonPointAttributes.PointSecurity,
            PICommonPointAttributes.Descriptor,
            PICommonPointAttributes.DisplayDigits,
            PICommonPointAttributes.EngineeringUnits,
            PICommonPointAttributes.ExtendedDescriptor,
            PICommonPointAttributes.Location2,
            PICommonPointAttributes.Location3,
            PICommonPointAttributes.Location4,
            PICommonPointAttributes.Location5,
            PICommonPointAttributes.Scan,
            PICommonPointAttributes.Shutdown,
            PICommonPointAttributes.Step
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

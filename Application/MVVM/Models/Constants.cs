using System.Collections.Generic;

namespace Models
{
    public class Constants
    {
        public const string OutputPath = @"C:\PIReplicationTool\Output\";
        public const string InputPath = @"C:\PIReplicationTool\Input\";

        public const string InputFileName = "input.txt";
        public const string OutputFileName_SourceTags = "SourceTags";
        public const string OutputFileName_TargetTags = "TargetTags";

        public const char   fieldSeparator = ';';
        
        public const string PISecurityConfiguration = "piadmin: A(r,w) | piadmins: A(r,w) | PI_Users_Read_Only: A(r) | PI_Users_Read_Write: A(r,w) | PI_Administrators: A(r,w) | PI_Analytics: A(r,w) | PI_Applications: A(r,w) | PI_Assetframework: A(r) | PI_Buffer: A(r,w) | PI_Interfaces: A(r) | PI_Notifications: A(r) | PI_Perfmon: A() | PI_Vision: A(r) | PIWorld: A()";

        private Constants() { }
    }
}

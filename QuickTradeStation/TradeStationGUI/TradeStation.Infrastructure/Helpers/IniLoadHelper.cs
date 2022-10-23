using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace TradeStation.Infrastructure.Helpers
{
    public static class IniLoadHelper
    {
        public const string SubscribeSection = "Subscribe1";
        public const string MulticastSection = "Multicast";

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,string key,string val,string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,string key,string def,StringBuilder retVal,int size,string filePath);

        /// <summary>
        /// Writes Data to the ini File
        /// </summary>
        /// <param name="filePath">Ini file path</param>
        /// <param name="section">Section name</param>
        /// <param name="key">Key Name</param>
        /// <param name="value">Value</param>
        public static void WriteValue(string filePath, string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, filePath);
        }
        
        /// <summary>
        /// Reads Data Value From the Ini File
        /// </summary>
        /// <param name="filePath">Ini file path</param>
        /// <param name="section">Section name</param>
        /// <param name="key">Key Name</param>
        /// <returns>Value</returns>
        public static string ReadValue(string filePath, string section, string key)
        {
            StringBuilder stringBuilder = new StringBuilder(255);
            int i = GetPrivateProfileString(section, key, "", stringBuilder, 255, filePath);
            return stringBuilder.ToString();
        }
    }
}

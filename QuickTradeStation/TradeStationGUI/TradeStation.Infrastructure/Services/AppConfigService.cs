using System;
using System.Configuration;

using TradeStation.Infrastructure.Helpers;

namespace TradeStation.Infrastructure.Services
{
    public static class AppConfigService
    {
        #region App config related string

        private const string KeyOperatorName = "operatorName";
        private const string KeyOperatorPassword = "operatorPassword";
        private const string KeyTradeServerIp = "tradeServerIp";
        private const string KeyTradeServerPort = "tradeServerPort";

        private const string KeyRefHttpServiceIp = "refHttpServiceIp";
        private const string KeyRefHttpServicePort = "refHttpServicePort";
        private const string KeyKLineHttpServiceIp = "kLineHttpServiceIp";
        private const string KeyKLineHttpServicePort = "kLineHttpServicePort";
        private const string KeySubscribeServerIp = "subscribeServerIp";
        private const string KeySubscribeServerPort = "subscribeServerPort";

        private const string KeyIsMulticastPrice = "isMulticastPrice";
        private const string KeyLicenseFile = "licenceFile";
        private const string KeyMulticastConfigFile = "multicastConfigFile";

        #endregion

        #region Ini config related string

        private const string KeyMulticastStockAddress = "StockAddress";
        private const string KeyMulticastStockPort = "StockPort";
        private const string KeyMulticastIndexAddress = "IndexAddress";
        private const string KeyMulticastIndexPort = "IndexPort";
        private const string KeyMulticastOptionAddress = "OptionAddress";
        private const string KeyMulticastOptionPort = "OptionPort";
        private const string KeyMulticastFutureAddress = "FutureAddress";
        private const string KeyMulticastFuturePort = "FuturePort";

        private const string KeySubscribeStockAddress = "UnicastAddress";
        private const string KeySubscribeStockPort = "UnicastPort";

        #endregion

        #region Properties

        private static string _operatorName;
        public static string OperatorName
        {
            get { return _operatorName; }
            set
            {
                if (_operatorName != value)
                {
                    _operatorName = value;
                    UpdateAppSettings(KeyOperatorName, value);
                }
            }
        }

        private static string _operatorPassword;
        public static string OperatorPassword
        {
            get { return _operatorPassword; }
            set
            {
                if (_operatorPassword != value)
                {
                    _operatorPassword = value;
                    UpdateAppSettings(KeyOperatorPassword, value);
                }
            }
        }

        private static bool _isMulticastPrice;
        public static bool IsMulticastPrice
        {
            get { return _isMulticastPrice; }
            set
            {
                if (_isMulticastPrice != value)
                {
                    _isMulticastPrice = value;
                    UpdateAppSettings(KeyIsMulticastPrice, value);
                }
            }
        }

        private static string _licenseFile = @"license.dat";
        public static string LicenseFile
        {
            get
            {
                return _licenseFile;
            }
            set
            {
                if (_licenseFile != value)
                {
                    _licenseFile = value;
                    UpdateAppSettings(KeyLicenseFile, value);
                }
            }
        }

        private static string _multicastConfigFile;
        public static string MulticastConfigFile
        {
            get { return _multicastConfigFile; }
            set
            {
                if (_multicastConfigFile == value) return;
                _multicastConfigFile = value;
                UpdateAppSettings(KeyMulticastConfigFile, value);
            }
        }

        #region O32 related

        private static string _tradeServerIp;
        public static string TradeServerIp
        {
            get
            {
                return _tradeServerIp;
            }
            set
            {
                if (_tradeServerIp != value)
                {
                    _tradeServerIp = value;
                    UpdateAppSettings(KeyTradeServerIp, value);
                }
            }
        }

        private static int _tradeServerPort;
        public static int TradeServerPort
        {
            get
            {
                return _tradeServerPort;
            }
            set
            {
                if (_tradeServerPort != value)
                {
                    _tradeServerPort = value;
                    UpdateAppSettings(KeyTradeServerPort, value);
                }
            }
        }

        private static string _subscribeServerIp;
        public static string SubscribeServerIp
        {
            get
            {
                return _subscribeServerIp;
            }
            set
            {
                if (_subscribeServerIp != value)
                {
                    _subscribeServerIp = value;
                    UpdateAppSettings(KeySubscribeServerIp, value);
                }
            }
        }

        private static int _subscribeServerPort;
        public static int SubscribeServerPort
        {
            get
            {
                return _subscribeServerPort;
            }
            set
            {
                if (_subscribeServerPort != value)
                {
                    _subscribeServerPort = value;
                    UpdateAppSettings(KeySubscribeServerPort, value);
                }
            }
        }

        #endregion

        #region Json service related

        private static string _refHttpServiceIp;
        public static string RefHttpServiceIp
        {
            get
            {
                return _refHttpServiceIp;
            }
            set
            {
                if (_refHttpServiceIp != value)
                {
                    _refHttpServiceIp = value;
                    UpdateAppSettings(KeyRefHttpServiceIp, value);
                }
            }
        }

        private static int _refHttpServicePort;
        public static int RefHttpServicePort
        {
            get
            {
                return _refHttpServicePort;
            }
            set
            {
                if (_refHttpServicePort != value)
                {
                    _refHttpServicePort = value;
                    UpdateAppSettings(KeyRefHttpServicePort, value);
                }
            }
        }

        private static string _kLineHttpServiceIp;
        public static string KLineHttpServiceIp
        {
            get
            {
                return _kLineHttpServiceIp;
            }
            set
            {
                if (_kLineHttpServiceIp == value) return;
                _kLineHttpServiceIp = value;
                UpdateAppSettings(KeyKLineHttpServiceIp, value);
            }
        }

        private static int _kLineHttpServicePort;
        public static int KLineHttpServicePort
        {
            get
            {
                return _kLineHttpServicePort;
            }
            set
            {
                if (_kLineHttpServicePort == value) return;
                _kLineHttpServicePort = value;
                UpdateAppSettings(KeyKLineHttpServicePort, value);
            }
        }

        #endregion

        #region Price Multicast Related

        private static string _stockMulticastIp;
        public static string StockMulticastIp
        {
            get
            {
                return _stockMulticastIp;
            }
            set
            {
                if (_stockMulticastIp != value)
                {
                    _stockMulticastIp = value;
                    UpdateIniSetting(IniLoadHelper.MulticastSection, KeyMulticastStockAddress, value);
                }
            }
        }

        private static int _stockMulticastPort;
        public static int StockMulticastPort
        {
            get
            {
                return _stockMulticastPort;
            }
            set
            {
                if (_stockMulticastPort != value)
                {
                    _stockMulticastPort = value;
                    UpdateIniSetting(IniLoadHelper.MulticastSection, KeyMulticastStockPort, value.ToString());
                }
            }
        }

        private static string _futureMulticastIp;
        public static string FutureMulticastIp
        {
            get
            {
                return _futureMulticastIp;
            }
            set
            {
                if (_futureMulticastIp != value)
                {
                    _futureMulticastIp = value;
                    UpdateIniSetting(IniLoadHelper.MulticastSection, KeyMulticastFutureAddress, value);
                }
            }
        }

        private static int _futureMulticastPort;
        public static int FutureMulticastPort
        {
            get
            {
                return _futureMulticastPort;
            }
            set
            {
                if (_futureMulticastPort != value)
                {
                    _futureMulticastPort = value;
                    UpdateIniSetting(IniLoadHelper.MulticastSection, KeyMulticastFuturePort, value.ToString());
                }
            }
        }

        private static string _indexMulticastIp;
        public static string IndexMulticastIp
        {
            get
            {
                return _indexMulticastIp;
            }
            set
            {
                if (_indexMulticastIp != value)
                {
                    _indexMulticastIp = value;
                    UpdateIniSetting(IniLoadHelper.MulticastSection, KeyMulticastIndexAddress, value);
                }
            }
        }

        private static int _indexMulticastPort;
        public static int IndexMulticastPort
        {
            get
            {
                return _indexMulticastPort;
            }
            set
            {
                if (_indexMulticastPort != value)
                {
                    _indexMulticastPort = value;
                    UpdateIniSetting(IniLoadHelper.MulticastSection, KeyMulticastIndexPort, value.ToString());
                }
            }
        }

        private static string _optionMulticastIp;
        public static string OptionMulticastIp
        {
            get
            {
                return _optionMulticastIp;
            }
            set
            {
                if (_optionMulticastIp != value)
                {
                    _optionMulticastIp = value;
                    UpdateIniSetting(IniLoadHelper.MulticastSection, KeyMulticastOptionAddress, value);
                }
            }
        }

        private static int _optionMulticastPort;
        public static int OptionMulticastPort
        {
            get
            {
                return _optionMulticastPort;
            }
            set
            {
                if (_optionMulticastPort != value)
                {
                    _optionMulticastPort = value;
                    UpdateIniSetting(IniLoadHelper.MulticastSection, KeyMulticastOptionPort, value.ToString());
                }
            }
        }

        #endregion

        #region Price Subscribe Related

        private static string _stockSubscribeIp;
        public static string StockSubscribeIp
        {
            get { return _stockSubscribeIp; }
            set
            {
                if (_stockSubscribeIp != value)
                {
                    _stockSubscribeIp = value;
                    UpdateIniSetting(IniLoadHelper.SubscribeSection, KeySubscribeStockAddress, value);
                }
            }
        }

        private static int _stockSubscribePort;
        public static int StockSubscribePort
        {
            get { return _stockSubscribePort; }
            set
            {
                if (_stockSubscribePort != value)
                {
                    _stockSubscribePort = value;
                    UpdateIniSetting(IniLoadHelper.SubscribeSection, KeySubscribeStockPort, value.ToString());
                }
            }
        }

        #endregion

        #endregion

        static AppConfigService()
        {
            ReadAllSettings();
        }

        static void ReadAllSettings()
        {
            string result = ReadAppSetting(KeyOperatorName);
            if (!string.IsNullOrEmpty(result))
                _operatorName = result;

            //result = ReadAppSetting(KeyOperatorPassword);
            //if (!string.IsNullOrEmpty(result))
            //    operatorPassword = result;

            result = ReadAppSetting(KeyTradeServerIp);
            if (!string.IsNullOrEmpty(result))
                _tradeServerIp = result;

            result = ReadAppSetting(KeyTradeServerPort);
            try
            {
                _tradeServerPort = int.Parse(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Failed to parse config value:" + result + @",error:" + ex.Message);
            }

            result = ReadAppSetting(KeySubscribeServerIp);
            if (!string.IsNullOrEmpty(result))
                _subscribeServerIp = result;

            result = ReadAppSetting(KeySubscribeServerPort);
            try
            {
                _subscribeServerPort = int.Parse(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Failed to parse config value:" + result + @",error:" + ex.Message);
            }

            result = ReadAppSetting(KeyLicenseFile);
            if (!string.IsNullOrEmpty(result))
            {
                _licenseFile = CommonUtils.CommonUtil.AssemblyPath + result;
            }


            result = ReadAppSetting(KeyRefHttpServiceIp);
            if (!string.IsNullOrEmpty(result))
                _refHttpServiceIp = result;

            result = ReadAppSetting(KeyRefHttpServicePort);
            try
            {
                _refHttpServicePort = int.Parse(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Failed to parse config value:" + result + @",error:" + ex.Message);
            }

            result = ReadAppSetting(KeyKLineHttpServiceIp);
            if (!string.IsNullOrEmpty(result))
                _kLineHttpServiceIp = result;

            result = ReadAppSetting(KeyKLineHttpServicePort);
            try
            {
                _kLineHttpServicePort = int.Parse(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Failed to parse config value:" + result + @",error:" + ex.Message);
            }

            result = ReadAppSetting(KeyMulticastConfigFile);
            if (!string.IsNullOrEmpty(result))
            {
                _multicastConfigFile = result;
            }

            result = ReadAppSetting(KeyIsMulticastPrice);
            try
            {
                _isMulticastPrice = bool.Parse(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Failed to parse config value:" + result + @",error:" + ex.Message);
            }

            // Read ini settings
            StockMulticastIp = ReadIniSetting(IniLoadHelper.MulticastSection, KeyMulticastStockAddress);
            StockMulticastPort = ReadIntIniSetting(IniLoadHelper.MulticastSection, KeyMulticastStockPort);
            FutureMulticastIp = ReadIniSetting(IniLoadHelper.MulticastSection, KeyMulticastFutureAddress);
            FutureMulticastPort = ReadIntIniSetting(IniLoadHelper.MulticastSection, KeyMulticastFuturePort);
            IndexMulticastIp = ReadIniSetting(IniLoadHelper.MulticastSection, KeyMulticastIndexAddress);
            IndexMulticastPort = ReadIntIniSetting(IniLoadHelper.MulticastSection, KeyMulticastIndexPort);
            OptionMulticastIp = ReadIniSetting(IniLoadHelper.MulticastSection, KeyMulticastOptionAddress);
            OptionMulticastPort = ReadIntIniSetting(IniLoadHelper.MulticastSection, KeyMulticastOptionPort);

            StockSubscribeIp = ReadIniSetting(IniLoadHelper.SubscribeSection, KeySubscribeStockAddress);
            StockSubscribePort = ReadIntIniSetting(IniLoadHelper.SubscribeSection, KeySubscribeStockPort);
        }

        private static string ReadAppSetting(string key)
        {
            string result = "";
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                result = appSettings[key] ?? "";
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine(@"Error reading app settings");
            }
            return result;
        }

        private static void UpdateAppSettings<T>(string key, T value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                string valueStr = value.ToString();
                if (settings[key] == null)
                {
                    settings.Add(key, valueStr);
                }
                else
                {
                    settings[key].Value = valueStr;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine(@"Error writing app settings");
            }
        }

        private static string ReadIniSetting(string section, string key)
        {
            string result = string.Empty;

            try
            {
                result = IniLoadHelper.ReadValue(MulticastConfigFile, section, key);

                result = result ?? string.Empty;
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine(@"Error reading ini settings");
            }

            return result;
        }

        static int ReadIntIniSetting(string section, string key)
        {
            var stringValue = ReadIniSetting(section, key);
            var intValue = 0;

            // If the string is empty or parse to int failed, log the error message.
            if (string.IsNullOrEmpty(stringValue)
                || int.TryParse(stringValue, out intValue))
            {
                Console.WriteLine(@"Failed to parse config value:" + stringValue );
            }

            return intValue;
        }

        private static void UpdateIniSetting(string section, string key, string value)
        {
            try
            {
                IniLoadHelper.WriteValue(MulticastConfigFile, section, key, value);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine(@"Error writing ini settings");
            }
        }
    }
}

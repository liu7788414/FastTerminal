using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Windows;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Services;

namespace TradeStation.Infrastructure.Helpers
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SaveLoadSecurityListHelper
    {
        private const string SAVEED_FOLDER = "SelectedSecurityList";
        private MarketDataService _marketDataService;

        public LogUtils Logger { get; set; }

        [ImportingConstructor]
        public SaveLoadSecurityListHelper(
            MarketDataService marketDataService,
            LogUtils logger)
        {
            _marketDataService = marketDataService;

            Logger = logger;
        }

        public void SaveSecurityListToCSV(string fileName, IList<SecurityInfo> securityInfoList)
        {
            var sbuilder = new StringBuilder();
            foreach (var securityInfo in securityInfoList)
            {
                sbuilder.AppendLine(String.Format("{0},{1}", securityInfo.ExID, securityInfo.SecurityID));
            }

            SaveStringContentToCSV(fileName, sbuilder.ToString(), "保存自选证券列表");
        }

        public void SaveSecurityPanelLocationListToCSV(string fileName, IList<SecurityPanelLocation> panelLocationList)
        {
            var sbuilder = new StringBuilder();
            foreach (var panelLocation in panelLocationList)
            {
                sbuilder.AppendLine(String.Format("{0},{1},{2},{3}",
                    panelLocation.ExSecID.ExID,
                    panelLocation.ExSecID.SecurityID,
                    panelLocation.Location.X,
                    panelLocation.Location.Y));
            }

            SaveStringContentToCSV(fileName, sbuilder.ToString(), "保存证券面板坐标列表");
        }

        public IList<SecurityInfo> LoadSecurityListFromCSV(string fileName)
        {
            var resultList = new List<SecurityInfo>();
            var filePath = SAVEED_FOLDER + "\\" + AppConfigService.OperatorName + "\\" + fileName;

            try
            {
                if (File.Exists(filePath))
                {
                    StreamReader sr = new StreamReader(filePath, Encoding.UTF8);

                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] s = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        if (s.Length == 2)
                        {
                            string exID = s[0];
                            string securityID = s[1];

                            if (!string.IsNullOrEmpty(exID) && !string.IsNullOrEmpty(securityID))
                            {
                                var securityInfo = _marketDataService.GetSecurityInfo(exID, securityID);

                                if (null != securityInfo)
                                {
                                    resultList.Add(securityInfo);
                                }
                            }
                        }
                    }

                    sr.Close();
                }
            }
            catch (System.Exception e)
            {
                Logger.Error(String.Format("读取证券列表{0}失败!{1},{2},{3}", filePath, e.Message, e.Source, e.StackTrace));
            }

            return resultList;
        }

        public IList<SecurityPanelLocation> LoadSecurityPanelLocationList(string fileName)
        {
            var resultList = new List<SecurityPanelLocation>();
            var filePath = SAVEED_FOLDER + "\\" + AppConfigService.OperatorName + "\\" + fileName;

            try
            {
                if (File.Exists(filePath))
                {
                    StreamReader sr = new StreamReader(filePath, Encoding.UTF8);

                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] s = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        if (s.Length == 4)
                        {
                            string exID = s[0];
                            string securityID = s[1];
                            string locationXString = s[2];
                            string locationYString = s[3];
                            double locationX, locationY;

                            if (!string.IsNullOrEmpty(exID) && !string.IsNullOrEmpty(securityID)
                                 && !string.IsNullOrEmpty(locationXString) && double.TryParse(locationXString, out locationX)
                                 && !string.IsNullOrEmpty(locationYString) && double.TryParse(locationYString, out locationY))
                            {
                                var panelLocation = new SecurityPanelLocation()
                                {
                                    ExSecID = new ExSecID(exID, securityID),
                                    Location = new Point(locationX, locationY),
                                };

                                resultList.Add(panelLocation);
                            }
                        }
                    }

                    sr.Close();
                }
            }
            catch (System.Exception e)
            {
                Logger.Error(String.Format("证券面板坐标列表{0}失败!{1},{2},{3}", filePath, e.Message, e.Source, e.StackTrace));
            }

            return resultList;
        }

        private void SaveStringContentToCSV(string fileName, string stringContent, string metaInfo)
        {
            try
            {
                var savedPath = SAVEED_FOLDER + "\\" + AppConfigService.OperatorName;
                var directoryInfo = new DirectoryInfo(savedPath);

                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }

                StreamWriter sw = new StreamWriter(savedPath + "\\" + fileName, false, Encoding.UTF8);
                sw.WriteLine(stringContent);

                sw.Close();
            }
            catch (System.Exception e)
            {
                Logger.Error(String.Format("{0}{1}失败!{2},{3},{4}", metaInfo, fileName, e.Message, e.Source, e.StackTrace));
            }
        }
    }
}

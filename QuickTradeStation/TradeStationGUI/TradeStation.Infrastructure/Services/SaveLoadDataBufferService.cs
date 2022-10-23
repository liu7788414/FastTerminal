using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Extensions;
using TradeStation.Infrastructure.Helpers;
using TradeStation.Infrastructure.Models;

namespace TradeStation.Infrastructure.Services
{
    // 数据缓存文件的文件名以："缓存数据类型名-下一次缓存更新时间"的格式命名
    // 程序启动时会扫描缓存文件夹下的文件，以构建各缓存数据对应其下一次更新时间的字典
    // 每次程序发请求会先检查其对应的下一次更新时间是否大于当前时间，若大于则读缓存，不然直接发请求、更新到缓存并设置下一次更新时间
    [Export]
    public class SaveLoadDataBufferService
    {
        private const string BUFFER_DATA_SAVED_FOLDER = "RefDataBuffer";
        private const string BUFFER_DATA_FILE_SEPERATOR = "-";
        private const long DEFAULT_BUFFER_REFRESH_TIME_TICKS = 330000000000; // 9:10
        private const int DEFAULT_BUFFER_REMAIN_DAYS = 5;

        private Dictionary<string, DateTime> _bufferDataNextUpdateTimeMap;

        public LogUtils Logger { get; set; }

        [ImportingConstructor]
        public SaveLoadDataBufferService(
            LogUtils logger)
        {
            Logger = logger;

            _bufferDataNextUpdateTimeMap = new Dictionary<string, DateTime>();
            InitializeRefDataBufferStatusMap();
        }

        // 获取Ref数据缓存文件夹的所有文件信息，以初始化各缓存文件的下一次更新时间字典。
        public void InitializeRefDataBufferStatusMap()
        {
            var newBufferDataNextTimeMap = new Dictionary<string, DateTime>();
            var bufferDataDirectory = new DirectoryInfo(BUFFER_DATA_SAVED_FOLDER);

            if (!bufferDataDirectory.Exists)
            {
                return;
            }

            // Get all buffer files information in the buffer directory.
            var allBufferFiles = bufferDataDirectory.GetFiles();
            foreach (var bufferFile in allBufferFiles)
            {
                // Get each file name.
                var bufferFileName = bufferFile.Name;
                string[] bufferDataInfo = bufferFileName.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                if (bufferDataInfo.Length == 2)
                {
                    // Using file name, get the buffer data name and next update time.
                    string bufferDataName = bufferDataInfo[0];
                    string bufferDataNextTimeString = bufferDataInfo[1];
                    long bufferDataNextTimeLong = 0;

                    // Validate the buffer data name and the update time format.
                    if (!string.IsNullOrEmpty(bufferDataName)
                        && !string.IsNullOrEmpty(bufferDataNextTimeString)
                        && long.TryParse(bufferDataNextTimeString, out bufferDataNextTimeLong)
                        && bufferDataNextTimeLong != 0)
                    {
                        var bufferDataNextTime = DateTimeHelper.ConvertToDateTime(bufferDataNextTimeLong * 100000);
                        DateTime oldBufferNextTime = DateTime.MinValue;

                        // If there exists 2 files with the same name, then use the later one.
                        if (newBufferDataNextTimeMap.TryGetValue(bufferDataName, out oldBufferNextTime))
                        {
                            if (oldBufferNextTime >= bufferDataNextTime)
                            {
                                newBufferDataNextTimeMap[bufferDataName] = oldBufferNextTime;
                            }
                            else
                            {
                                newBufferDataNextTimeMap[bufferDataName] = bufferDataNextTime;
                            }
                        }
                        else
                        {
                            newBufferDataNextTimeMap.Add(bufferDataName, bufferDataNextTime);
                        }
                    }
                }
            }

            // Everytime, the buffer data map should be replaced by the new one.
            _bufferDataNextUpdateTimeMap = newBufferDataNextTimeMap;
        }

        // 通过缓存数据类型名来取得其对应的下一次缓存更新时间。
        public DateTime GetRefDataBufferNextUpdateTime(string refDataBufferFileName, out bool hasBuffer)
        {
            DateTime nextUpdateTime = DateTime.MinValue;

            if (_bufferDataNextUpdateTimeMap.TryGetValue(refDataBufferFileName, out nextUpdateTime))
            {
                hasBuffer = true;
                return nextUpdateTime;
            }
            else
            {
                hasBuffer = false;
                return DateTime.Now.Date.AddTicks(TimeKeeper.DailyInitializationTime.Ticks);
            }
        }

        // 刷新下一次更新时间字典，若不存在缓存数据类型，则新加此数据。
        public void UpdateRefDataBufferNextUpdateTime(string refDataBufferFileName, DateTime nextUpdateTime)
        {
            if (_bufferDataNextUpdateTimeMap.ContainsKey(refDataBufferFileName))
            {
                _bufferDataNextUpdateTimeMap[refDataBufferFileName] = nextUpdateTime;
            }
            else
            {
                _bufferDataNextUpdateTimeMap.Add(refDataBufferFileName, nextUpdateTime);
            }
        }

        public void SaveBufferToFile(string fileName, string bufferData)
        {
            var savedFileName = fileName;

            try
            {
                var savedPath = BUFFER_DATA_SAVED_FOLDER;
                var directoryInfo = new DirectoryInfo(savedPath);

                var currentTime = DateTime.Now;
                var nextUpdateTime = currentTime.Date.AddDays(1).AddTicks(TimeKeeper.DailyInitializationTime.Ticks);
                if (DateTime.Now.TimeOfDay < TimeKeeper.DailyInitializationTime)
                {
                    nextUpdateTime = currentTime.Date.AddTicks(TimeKeeper.DailyInitializationTime.Ticks);
                }
                var nextUpdateTimeStamp = DateTimeHelper.ConvertToDateTimeInt(nextUpdateTime);

                savedFileName = fileName + "-" + nextUpdateTimeStamp;

                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }

                StreamWriter sw = new StreamWriter(savedPath + "\\" + savedFileName, false, Encoding.UTF8);

                sw.WriteLine(bufferData);

                sw.Close();

                UpdateRefDataBufferNextUpdateTime(fileName, nextUpdateTime);
            }
            catch (System.Exception e)
            {
                Logger.Error(String.Format("缓存{0}失败!{1},{2},{3}", fileName, e.Message, e.Source, e.StackTrace));
            }
        }

        public string LoadBufferFromFile(string fileName)
        {
            var hasBuffer = false;
            var nextUpdateTime = GetRefDataBufferNextUpdateTime(fileName, out hasBuffer);

            if (!hasBuffer)
            {
                return string.Empty;
            }

            var filePath = BUFFER_DATA_SAVED_FOLDER + "\\" + fileName + "-" + DateTimeHelper.ConvertToDateTimeInt(nextUpdateTime);
            string buffer = string.Empty;

            try
            {
                if (File.Exists(filePath))
                {
                    StreamReader sr = new StreamReader(filePath, Encoding.UTF8);

                    buffer = sr.ReadToEnd();

                    sr.Close();
                }
            }
            catch (System.Exception e)
            {
                Logger.Error(String.Format("读取{0}失败!{1},{2},{3}", filePath, e.Message, e.Source, e.StackTrace));
            }

            return buffer;
        }

        public void ClearAllBufferFile()
        {
            var directoryInfo = new DirectoryInfo(BUFFER_DATA_SAVED_FOLDER);

            if (directoryInfo.Exists)
            {
                var allTargetFiles = directoryInfo.GetFiles();

                foreach (var targetFile in allTargetFiles)
                {
                    targetFile.Delete();
                }
            }
        }

        public void ClearBufferFileByDefaultRemainDays(string fileName)
        {
            ClearBufferFile(fileName, DEFAULT_BUFFER_REMAIN_DAYS);
        }

        public void ClearBufferFile(string fileName, int daysBufferSaved)
        {
            var directoryInfo = new DirectoryInfo(BUFFER_DATA_SAVED_FOLDER);

            if (directoryInfo.Exists)
            {
                var allTargetFiles = directoryInfo.GetFiles().Where(x => x.Name.StartsWith(fileName, StringComparison.InvariantCultureIgnoreCase));

                var bufferSavedStartDate = DateTime.MaxValue;
                if (daysBufferSaved > 0)
                {
                    bufferSavedStartDate = DateTime.Now.Date.AddDays(daysBufferSaved * -1);
                }

                foreach (var targetFile in allTargetFiles)
                {
                    var bufferInfo = targetFile.Name.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    long nextUpdateTimeLong = 0;

                    if (bufferInfo.Length == 2
                        && !string.IsNullOrEmpty(bufferInfo[1]) && long.TryParse(bufferInfo[1], out nextUpdateTimeLong))
                    {
                        var nextUpdateTime = DateTimeHelper.ConvertToDateTime(nextUpdateTimeLong * 100000);

                        if (nextUpdateTime <= bufferSavedStartDate)
                        {
                            targetFile.Delete();
                        }
                    }
                }
            }
        }
    }
}

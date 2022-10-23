using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Microsoft.Practices.Prism.PubSubEvents;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.Extensions;
using TradeStation.Infrastructure.Helpers;
using TradeStation.Infrastructure.Managers;
using TradeStation.Infrastructure.Metadata;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Models.Local;
using TradeStation.Infrastructure.Payloads.RefDataPayloads;

namespace TradeStation.Infrastructure.Services
{
    [Export]
    [Export(typeof(IDailyInformationGetter))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SecurityRefHttpService : IDailyInformationGetter
    {
        [Import]
        private LogUtils Logger { get; set; }

        [Import]
        private IEventAggregator eventAggregator = null;

        [Import]
        private SecurityInfoMetadata securityInfoMetadata = null;

        [Import]
        private DialogService _dialogService = null;

        [Import]
        private SaveLoadDataBufferService _saveLoadDataBufferHelper = null;

        private HttpClient httpClient = null;

        private const long MAX_RESPONSE_CONTENT_BUFFER_SIZE = 10 * 1024 * 1024;
        private const int REQUEST_TIMEOUT_SECONDS = 30;
        private const string URL_STRING_PATTERN = "http://{0}:{1}";

        #region Service call related Enum

        private enum eRequestTypes
        {
            GetSecurityCodeTable,
            GetExchangeTradePeriod,
            GetBenchMarkRate,
            IsTradingDate,
            LastTradingDate,
            NextTradingDate,
            GetExrightRatioByDate,
            GetOptionInformation,
            GetKLineRecordsWithStartEnd,
            GetKLineRecordsWithCount,
            GetSuspensionInfo,
        }

        public enum eExchangeType
        {
            SH, // 上海证券交易所
            SZ, // 深圳证券交易所
            SHFE, // 上海期货交易所
            DCE, // 大连商品交易所
            CZCE, // 郑州商品交易所
            CFFEX, // CFFEX
        }

        public enum eServiceResultEnum
        {
            success = 0, // 成功
            fail = 1, // 失败
            unAuthorised = 2, // 访问无权限
            dbError = 3, // 数据库异常
            paramError = 4, // 参数错误
        }

        #endregion

        #region Service call related dictionary

        private Dictionary<eRequestTypes, string> requestUrl = new Dictionary<eRequestTypes, string>();

        private Dictionary<eRequestTypes, string> requestDeployPathDic = new Dictionary<eRequestTypes, string>
        {
            {eRequestTypes.GetSecurityCodeTable, "/refDataQuery"},
            {eRequestTypes.GetExchangeTradePeriod, "/refDataQuery"},
            {eRequestTypes.GetBenchMarkRate, "/refDataQuery"},
            {eRequestTypes.IsTradingDate, "/refDataQuery"},
            {eRequestTypes.LastTradingDate, "/refDataQuery"},
            {eRequestTypes.NextTradingDate, "/refDataQuery"},
            {eRequestTypes.GetExrightRatioByDate, "/indicatorQueryService"},
            {eRequestTypes.GetOptionInformation, "/refDataQuery"},
            {eRequestTypes.GetKLineRecordsWithStartEnd, "/indicatorQueryService"},
            {eRequestTypes.GetKLineRecordsWithCount, "/indicatorQueryService"},
            {eRequestTypes.GetSuspensionInfo, "/refDataQuery"},
        };

        private Dictionary<eRequestTypes, string> requestUrlDic = new Dictionary<eRequestTypes, string>
        {
            {eRequestTypes.GetSecurityCodeTable, "/security/querybytype"},
            {eRequestTypes.GetExchangeTradePeriod, "/exchtradetime"},
            {eRequestTypes.GetBenchMarkRate, "/txn/benchmarkrate"},
            {eRequestTypes.IsTradingDate, "/txn/istxndate"},
            {eRequestTypes.LastTradingDate, "/txn/lasttxndate"},
            {eRequestTypes.NextTradingDate, "/txn/nexttxndate"},
            {eRequestTypes.GetExrightRatioByDate, "/getexrightratiobyday"},
            {eRequestTypes.GetOptionInformation, "/option/querybyunderlying"},
            {eRequestTypes.GetKLineRecordsWithStartEnd, "/querybyrange"},
            {eRequestTypes.GetKLineRecordsWithCount, "/querybyendandcount"},
            {eRequestTypes.GetSuspensionInfo, "/suspension/getbydate"},
        };

        private Dictionary<eRequestTypes, string> requestTypeStr = new Dictionary<eRequestTypes, string>
        {
            {eRequestTypes.GetSecurityCodeTable, "获取证券代码表"},
            {eRequestTypes.GetExchangeTradePeriod, "获取证券交易时段列表"},
            {eRequestTypes.GetBenchMarkRate, "获取基准利率"},
            {eRequestTypes.IsTradingDate, "获取证券交易时间信息 - 当前是不是交易日"},
            {eRequestTypes.LastTradingDate, "获取证券交易时段信息 - 上一个交易日"},
            {eRequestTypes.NextTradingDate, "获取证券交易时段信息 - 下一个交易日"},
            {eRequestTypes.GetExrightRatioByDate, "获取复权因子列表"},
            {eRequestTypes.GetOptionInformation, "获取期权信息"},
            {eRequestTypes.GetKLineRecordsWithStartEnd, "获取股票K线"},
            {eRequestTypes.GetKLineRecordsWithCount, "获取股票K线"},
            {eRequestTypes.GetSuspensionInfo, "获取证券停牌信息"},
        };

        private Dictionary<eRequestTypes, bool> requestPendingBoolDic = new Dictionary<eRequestTypes, bool>
        {
            {eRequestTypes.GetSecurityCodeTable, false},
            {eRequestTypes.GetExchangeTradePeriod, false},
            {eRequestTypes.GetBenchMarkRate, false},
            {eRequestTypes.IsTradingDate, false},
            {eRequestTypes.LastTradingDate, false},
            {eRequestTypes.NextTradingDate, false},
            {eRequestTypes.GetExrightRatioByDate, false},
            {eRequestTypes.GetOptionInformation, false},
            {eRequestTypes.GetKLineRecordsWithStartEnd, false},
            {eRequestTypes.GetKLineRecordsWithCount, false},
            {eRequestTypes.GetSuspensionInfo, false},
        };

        #endregion

        public SecurityRefHttpService()
        {
            // Apply auto decompression configuration.
            HttpClientHandler clientHandler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            httpClient = new HttpClient(clientHandler);
            httpClient.MaxResponseContentBufferSize = MAX_RESPONSE_CONTENT_BUFFER_SIZE;
            httpClient.Timeout = System.TimeSpan.FromSeconds(REQUEST_TIMEOUT_SECONDS);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            requestUrl.Add(eRequestTypes.GetSecurityCodeTable, string.Format(URL_STRING_PATTERN, AppConfigService.RefHttpServiceIp, AppConfigService.RefHttpServicePort));
            requestUrl.Add(eRequestTypes.GetExchangeTradePeriod, string.Format(URL_STRING_PATTERN, AppConfigService.RefHttpServiceIp, AppConfigService.RefHttpServicePort));
            requestUrl.Add(eRequestTypes.GetBenchMarkRate, string.Format(URL_STRING_PATTERN, AppConfigService.RefHttpServiceIp, AppConfigService.RefHttpServicePort));
            requestUrl.Add(eRequestTypes.IsTradingDate, string.Format(URL_STRING_PATTERN, AppConfigService.RefHttpServiceIp, AppConfigService.RefHttpServicePort));
            requestUrl.Add(eRequestTypes.LastTradingDate, string.Format(URL_STRING_PATTERN, AppConfigService.RefHttpServiceIp, AppConfigService.RefHttpServicePort));
            requestUrl.Add(eRequestTypes.NextTradingDate, string.Format(URL_STRING_PATTERN, AppConfigService.RefHttpServiceIp, AppConfigService.RefHttpServicePort));
            requestUrl.Add(eRequestTypes.GetExrightRatioByDate, string.Format(URL_STRING_PATTERN, AppConfigService.KLineHttpServiceIp, AppConfigService.KLineHttpServicePort));
            requestUrl.Add(eRequestTypes.GetOptionInformation, string.Format(URL_STRING_PATTERN, AppConfigService.RefHttpServiceIp, AppConfigService.RefHttpServicePort));
            requestUrl.Add(eRequestTypes.GetKLineRecordsWithStartEnd, string.Format(URL_STRING_PATTERN, AppConfigService.KLineHttpServiceIp, AppConfigService.KLineHttpServicePort));
            requestUrl.Add(eRequestTypes.GetKLineRecordsWithCount, string.Format(URL_STRING_PATTERN, AppConfigService.KLineHttpServiceIp, AppConfigService.KLineHttpServicePort));
            requestUrl.Add(eRequestTypes.GetSuspensionInfo, string.Format(URL_STRING_PATTERN, AppConfigService.RefHttpServiceIp, AppConfigService.RefHttpServicePort));
        }

        public void GetInitData()
        {
            TFSplashScreenManager.Instance.Message = "正在读取证券代码表...";
            GetSecurityCodeTable();

            TFSplashScreenManager.Instance.Message = "正在读取证券交易时段列表...";
            GetTradingDateInformation();
            GetExchangeTradePeriod();

            TFSplashScreenManager.Instance.Message = "正在读取证券其他基础信息...";
            GetCurrentSuspensionInfo();
            GetBenchMarkRate("01010204");
            GetExrightRatioByDate();
            GetOptionInformation();
        }

        private string CheckBufferDataFileAndRequest(string bufferDataFileName, eRequestTypes reqType, IList<KeyValuePair<string, string>> parameters)
        {
            var hasBuffer = false;
            var responseString = string.Empty;
            var nextUpdateTime = _saveLoadDataBufferHelper.GetRefDataBufferNextUpdateTime(bufferDataFileName, out hasBuffer);
            var isRequsetSuccess = false;

            if (nextUpdateTime <= DateTime.Now || !hasBuffer)
            {
                JsonResponse response = new JsonResponse();
                try
                {
                    responseString = SendGetRequestToServerSync(reqType, parameters);
                    response = CommonUtil.GetObjectFromJsonString(responseString, typeof(JsonResponse)) as JsonResponse;
                    isRequsetSuccess = true;
                }
                catch (Exception)
                {
                    isRequsetSuccess = false;
                    Logger.Error(string.Format("获取Ref服务器数据失败: {0}", reqType.ToString()));
                }

                if (response.Success && isRequsetSuccess)
                {
                    _saveLoadDataBufferHelper.ClearBufferFileByDefaultRemainDays(bufferDataFileName);
                    _saveLoadDataBufferHelper.SaveBufferToFile(bufferDataFileName, responseString);
                }
            }
            else
            {
                responseString = _saveLoadDataBufferHelper.LoadBufferFromFile(bufferDataFileName);
            }

            return responseString;
        }

        private string SendGetRequestToServerSync(eRequestTypes reqType, IList<KeyValuePair<string, string>> parameters)
        {
            return Task.Run(() => SendGetRequestToServer(reqType, parameters)).Result;
        }

        // Format security id for K-line request.
        private string FormatSecurityIdForKLineRequest(ExSecID exSecId)
        {
            var result = exSecId.SecurityID;

            var securityIdCharArray = exSecId.SecurityID.ToArray();

            if (exSecId.ExID == CommonUtil.郑商所
                && !securityIdCharArray.All(x => char.IsDigit(x)))
            {
                try
                {
                    var insertDigit = DateTime.Now.Year % 100 / 10;
                    var insertPoint = exSecId.SecurityID.IndexOf(securityIdCharArray.First(x => char.IsDigit(x)));

                    result = exSecId.SecurityID.Insert(insertPoint, insertDigit.ToString());
                }
                // Catch异常，以避免之后郑商所交易品种变化带来的问题
                catch (Exception)
                {
                    Logger.Error("Format SecurityID failed when send GetKLineRecords request.");
                }
            }

            return result;
        }

        private async Task<string> SendGetRequestToServer(eRequestTypes reqType, IList<KeyValuePair<string, string>> parameters)
        {
            string responseStr = String.Empty;
            String errMsg = String.Empty;

            if (requestPendingBoolDic[reqType] == true)
                return responseStr;

            // Generate the url parameters.
            int ix = 0;
            var urlParameters = string.Empty;
            if (null != parameters)
            {
                foreach (var parameterPair in parameters)
                {
                    if (ix++ == 0)
                    {
                        urlParameters += "?";
                    }
                    else
                    {
                        urlParameters += "&";
                    }

                    urlParameters += parameterPair.Key + "=" + parameterPair.Value;
                }
            }

            // Generate the whole url string.
            string url = requestUrl[reqType] + requestDeployPathDic[reqType] + requestUrlDic[reqType] + urlParameters;

            Logger.Debug("http request GET:" + url);

            try
            {
                requestPendingBoolDic[reqType] = true;
                var response = await httpClient.GetAsync(new Uri(url));
                responseStr = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException != null)
                    {
                        errMsg = "HTTP错误:" + ex.InnerException.InnerException.Message;
                    }
                    else
                    {
                        errMsg = "HTTP错误:" + ex.InnerException.Message;
                    }
                }
                else
                {
                    errMsg = "HTTP错误:" + ex.Message;
                }
            }
            if (errMsg != String.Empty)
            {
                Logger.Error(errMsg);
                eventAggregator.GetEvent<LogMessageNotifyEvent>().Publish(new LogMessageEntity
                {
                    LogLevel = LogMessageLevel.ERROR,
                    Message = requestTypeStr[reqType] + "失败!"
                });
            }

            requestPendingBoolDic[reqType] = false;

            return responseStr;
        }

        private async Task<string> SendPostRequestToServer(eRequestTypes reqType, string reqStr)
        {
            string responseStr = String.Empty;
            String errMsg = String.Empty;
            if (requestPendingBoolDic[reqType] == true)
                return responseStr;

            string url = requestUrl[reqType] + requestDeployPathDic[reqType] + requestUrlDic[reqType];

            var posContent = new StringContent(reqStr)
            {
                Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
            };
            Logger.Debug("http request post:" + reqStr);

            try
            {
                requestPendingBoolDic[reqType] = true;
                var response = await httpClient.PostAsync(new Uri(url), posContent);
                responseStr = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException != null)
                    {
                        errMsg = "HTTP错误:" + ex.InnerException.InnerException.Message;
                    }
                    else
                    {
                        errMsg = "HTTP错误:" + ex.InnerException.Message;
                    }
                }
                else
                {
                    errMsg = "HTTP错误:" + ex.Message;
                }
            }
            if (errMsg != String.Empty)
            {
                Logger.Error(errMsg);
                eventAggregator.GetEvent<LogMessageNotifyEvent>().Publish(new LogMessageEntity
                {
                    LogLevel = LogMessageLevel.ERROR,
                    Message = requestTypeStr[reqType] + "失败!"
                });
            }

            requestPendingBoolDic[reqType] = false;

            return responseStr;
        }

        public void GetSecurityCodeTable()
        {
            var securityTypes = new List<eCategory>();
            securityTypes.Add(eCategory.股票);
            securityTypes.Add(eCategory.期货);
            securityTypes.Add(eCategory.期权);
            securityTypes.Add(eCategory.基金);
            securityTypes.Add(eCategory.债券);
            securityTypes.Add(eCategory.债券回购);

            GetSecurityCodeTable(securityTypes, true, true);
        }

        public void GetSecurityCodeTable(IList<eCategory> securityTypes, bool isIncludeExpired, bool isCompressResponse)
        {
            eRequestTypes reqType = eRequestTypes.GetSecurityCodeTable;

            var parameters = new List<KeyValuePair<string, string>>();
            if (securityTypes.Any())
            {
                foreach (var securityType in securityTypes)
                {
                    parameters.Add(new KeyValuePair<string, string>("type", ((int)securityType).ToString()));
                }
            }
            if (isIncludeExpired)
            {
                parameters.Add(new KeyValuePair<string, string>("all", "true"));
            }
            if (isCompressResponse)
            {
                parameters.Add(new KeyValuePair<string, string>("gzip", "true"));
            }

            // Get security code table sync.
            string responseStr = CheckBufferDataFileAndRequest(reqType.ToString() ,reqType, parameters);
            if (string.IsNullOrEmpty(responseStr))
            {
                Logger.Error("股票代码表消息为空!");

                _dialogService.ShowMessage("错误", "股票代码表获取失败");

                return;
            }

            Logger.Debug("Get stock code table http response");
            try
            {
                SecurityCodeTableQueryResponse secListResponse = CommonUtil.GetObjectFromJsonString(responseStr, typeof(SecurityCodeTableQueryResponse)) as SecurityCodeTableQueryResponse;
                if (secListResponse != null && secListResponse.SecurityList.Count > 0)
                {
                    securityInfoMetadata.InitSecurityCodeMap(secListResponse.SecurityList);
                    Logger.Debug("Total count of stocks:" + secListResponse.SecurityList.Count);
                }
                else
                {
                    Logger.Error("股票代码表为空!");

                    _dialogService.ShowMessage("错误", "股票代码表获取失败");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("股票代码表获取失败:" + ex.Message);

                _dialogService.ShowMessage("错误", "股票代码表获取失败");
            }
        }

        public void GetExchangeTradePeriod()
        {
            eRequestTypes reqType = eRequestTypes.GetExchangeTradePeriod;

            // Get exchange trade period sync.
            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("effDate", (DateTimeHelper.ConvertToDateTimeInt(DateTime.Now.Date) / 10000).ToString()));

            string responseStr = CheckBufferDataFileAndRequest(reqType.ToString(), reqType, parameters);
            if (string.IsNullOrEmpty(responseStr))
            {
                Logger.Error("获取证券交易时段列表消息为空!");

                _dialogService.ShowMessage("错误", "证券交易时段列表获取失败");

                return;
            }

            Logger.Debug("Get exchange trade period table http response");
            try
            {
                ExchangeTradePeriodsResponse periodsResponse = CommonUtil.GetObjectFromJsonString(responseStr, typeof(ExchangeTradePeriodsResponse)) as ExchangeTradePeriodsResponse;
                if (periodsResponse != null && periodsResponse.ExchangeTradePeriods.Count > 0)
                {
                    securityInfoMetadata.InitTradePeriodDictionary(periodsResponse.ExchangeTradePeriods.Select(x => x.ToExchangeTradePeriodModel()).ToList());
                    Logger.Debug("Total count of periods:" + periodsResponse.ExchangeTradePeriods.Count);
                }
                else
                {
                    Logger.Error("证券交易时段列表为空!");

                    _dialogService.ShowMessage("错误", "证券交易时段列表获取失败");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("证券交易时段列表获取失败:" + ex.Message + "\r\n" + ex.StackTrace);

                _dialogService.ShowMessage("错误", "证券交易时段列表获取失败");
            }
        }

        public void GetExrightRatioByDate()
        {
            eRequestTypes reqType = eRequestTypes.GetExrightRatioByDate;

            // Get exchange trade period sync.
            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("day", (DateTimeHelper.ConvertToDateTimeInt(DateTime.Now.Date) / 10000).ToString()));

            string responseStr = CheckBufferDataFileAndRequest(reqType.ToString(), reqType, parameters);
            if (string.IsNullOrEmpty(responseStr))
            {
                Logger.Error("获取复权因子列表为空!");

                _dialogService.ShowMessage("错误", "复权因子列表获取失败");

                return;
            }

            Logger.Debug("Get ex-right ratio table http response");
            try
            {
                ExrightRatioQueryResponse exrightRatiosResponse = CommonUtil.GetObjectFromJsonString(responseStr, typeof(ExrightRatioQueryResponse)) as ExrightRatioQueryResponse;
                if (exrightRatiosResponse != null && exrightRatiosResponse.ExrightRatioList.Count > 0)
                {
                    securityInfoMetadata.InitExrightRatio(exrightRatiosResponse.ExrightRatioList.Select(x => x.ToExrightRatioModel()).ToList());
                    Logger.Debug("Total count of periods:" + exrightRatiosResponse.ExrightRatioList.Count);
                }
                else
                {
                    Logger.Error("复权因子列表为空!");

                    _dialogService.ShowMessage("错误", "复权因子列表获取失败");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("复权因子列表获取失败:" + ex.Message);

                _dialogService.ShowMessage("错误", "复权因子列表获取失败");
            }
        }

        public void GetTradingDateInformation()
        {
            eRequestTypes isTradingDateReqType = eRequestTypes.IsTradingDate;
            eRequestTypes lastTradingDateReqType = eRequestTypes.LastTradingDate;
            eRequestTypes nextTradingDateReqType = eRequestTypes.NextTradingDate;

            // Get trade date information sync.
            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("date", (DateTimeHelper.ConvertToDateTimeInt(DateTime.Now.Date) / 10000).ToString()));
            parameters.Add(new KeyValuePair<string, string>("marketcode", CommonUtil.上交所));

            string isDateResponseStr = CheckBufferDataFileAndRequest(isTradingDateReqType.ToString(), isTradingDateReqType, parameters);
            string lastDateResponseStr = CheckBufferDataFileAndRequest(lastTradingDateReqType.ToString(), lastTradingDateReqType, parameters);
            string nextDateResponseStr = CheckBufferDataFileAndRequest(nextTradingDateReqType.ToString(), nextTradingDateReqType, parameters);

            if (string.IsNullOrEmpty(isDateResponseStr)
                || string.IsNullOrEmpty(lastDateResponseStr)
                || string.IsNullOrEmpty(nextDateResponseStr))
            {
                Logger.Error("获取证券交易时间信息为空!");

                _dialogService.ShowMessage("错误", "证券交易时间信息获取失败");

                return;
            }

            Logger.Debug("Get trade date information http response");
            try
            {
                IsTradeDateResponse isDateResponse = CommonUtil.GetObjectFromJsonString(isDateResponseStr, typeof(IsTradeDateResponse)) as IsTradeDateResponse;
                LastTradeDateResponse lastDateResponse = CommonUtil.GetObjectFromJsonString(lastDateResponseStr, typeof(LastTradeDateResponse)) as LastTradeDateResponse;
                NextTradeDateResponse nextDateResponse = CommonUtil.GetObjectFromJsonString(nextDateResponseStr, typeof(NextTradeDateResponse)) as NextTradeDateResponse;

                if (null != isDateResponse
                    && null != lastDateResponse
                    && null != nextDateResponse)
                {
                    securityInfoMetadata.InitTradeDate(isDateResponse.IsTradeDate, lastDateResponse.LastTradeDate, nextDateResponse.NextTradeDate);
                }
                else
                {
                    Logger.Error("证券交易时间信息为空!");

                    _dialogService.ShowMessage("错误", "证券交易时间信息获取失败");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("证券交易时间信息获取失败:" + ex.Message);

                _dialogService.ShowMessage("错误", "证券交易时间信息获取失败");
            }
        }

        public void GetBenchMarkRate(string rateID)
        {
            eRequestTypes reqType = eRequestTypes.GetBenchMarkRate;

            // Get exchange trade period sync.
            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("date", (DateTimeHelper.ConvertToDateTimeInt(DateTime.Now.Date) / 10000).ToString()));
            parameters.Add(new KeyValuePair<string, string>("code", rateID));

            string responseStr = CheckBufferDataFileAndRequest(reqType.ToString(), reqType, parameters);
            if (string.IsNullOrEmpty(responseStr))
            {
                Logger.Error("获取基准利率消息为空!");

                _dialogService.ShowMessage("错误", "基准利率获取失败");

                return;
            }

            Logger.Debug("Get bench mark rate http response");
            try
            {
                BenchMarkRateResponse benckMarkRateResponse = CommonUtil.GetObjectFromJsonString(responseStr, typeof(BenchMarkRateResponse)) as BenchMarkRateResponse;
                if (null != benckMarkRateResponse)
                {
                    securityInfoMetadata.InitRFRate(benckMarkRateResponse.RateValue);
                }
                else
                {
                    Logger.Error("获取基准利率为空!");

                    _dialogService.ShowMessage("错误", "基准利率获取失败");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("基准利率获取失败:" + ex.Message);

                _dialogService.ShowMessage("错误", "基准利率获取失败");
            }
        }

        public void GetCurrentSuspensionInfo()
        {
            GetSuspensionInfo(DateTime.Now);
        }

        public void GetSuspensionInfo(DateTime checkDate)
        {
            eRequestTypes reqType = eRequestTypes.GetSuspensionInfo;

            // Get suspension info sync.
            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("date", (DateTimeHelper.ConvertToDateTimeInt(checkDate) / 10000).ToString()));

            // Suspension info should not use buffer to store information.
            string responseStr = SendGetRequestToServerSync(reqType, parameters);
            if (string.IsNullOrEmpty(responseStr))
            {
                Logger.Error("获取停牌信息列表为空!");

                _dialogService.ShowMessage("错误", "停牌信息列表获取失败");

                return;
            }

            Logger.Debug("Get suspension info http response");
            try
            {
                SuspensionInfoQueryResponse suspensionInfoResponse = CommonUtil.GetObjectFromJsonString(responseStr, typeof(SuspensionInfoQueryResponse)) as SuspensionInfoQueryResponse;
                if (suspensionInfoResponse != null && suspensionInfoResponse.SuspensionInfoList.Count > 0)
                {
                    securityInfoMetadata.InitSuspensionInfo(suspensionInfoResponse.SuspensionInfoList.Select(x => x.ToSuspensionInfoModel()).ToList());
                    Logger.Debug("Total count of suspension infos:" + suspensionInfoResponse.SuspensionInfoList.Count);
                }
                else
                {
                    Logger.Error("停牌信息列表为空!");

                    _dialogService.ShowMessage("错误", "停牌信息列表获取失败");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("停牌信息列表获取失败:" + ex.Message);

                _dialogService.ShowMessage("错误", "停牌信息列表获取失败");
            }
        }

        public async void GetOptionInformation()
        {
            eRequestTypes reqType = eRequestTypes.GetOptionInformation;

            string responseStr = await SendGetRequestToServer(reqType, null);
            if (responseStr != String.Empty)
            {
                Logger.Debug("Get option information http response");
                try
                {
                    OptionInformationTableQueryResponse optionInfoListResponse = CommonUtil.GetObjectFromJsonString(responseStr, typeof(OptionInformationTableQueryResponse)) as OptionInformationTableQueryResponse;
                    if (optionInfoListResponse != null && optionInfoListResponse.OptionList.Count > 0)
                    {
                        securityInfoMetadata.InitOptionInfoCollection(optionInfoListResponse.OptionList);
                        Logger.Debug("Total count of options:" + optionInfoListResponse.OptionList.Count);

                        eventAggregator.GetEvent<FinishedGetOptionInformationEvent>().Publish(null);
                    }
                    else
                    {
                        Logger.Error("期权信息表为空!");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("期权信息表获取失败:" + ex.Message);
                }

            }
        }

        public async Task<IList<KLineRecordModel>> GetKLineRecordsWithStartEnd(ExSecID exSecID, eKLinePeriodType kLineType, DateTime startDateTime, DateTime endDateTime, eExRightType exRightType)
        {
            var parameterDic = new List<KeyValuePair<string, string>>();

            parameterDic.Add(new KeyValuePair<string, string>("exch", exSecID.ExID));
            parameterDic.Add(new KeyValuePair<string, string>("secId", FormatSecurityIdForKLineRequest(exSecID)));
            parameterDic.Add(new KeyValuePair<string, string>("periodType", kLineType.ToString()));
            parameterDic.Add(new KeyValuePair<string, string>("start", DateTimeHelper.ConvertToDateTimeInt(startDateTime).ToString()));
            parameterDic.Add(new KeyValuePair<string, string>("end", DateTimeHelper.ConvertToDateTimeInt(endDateTime).ToString()));
            parameterDic.Add(new KeyValuePair<string, string>("exrightType", exRightType.ToString().ToLower()));

            return await GetKLineRecords(exSecID, eRequestTypes.GetKLineRecordsWithStartEnd, parameterDic);
        }

        public async Task<IList<KLineRecordModel>> GetKLineRecordsWithCount(ExSecID exSecID, eKLinePeriodType kLineType, int count, DateTime endDateTime, eExRightType exRightType)
        {
            var parameterDic = new List<KeyValuePair<string, string>>();

            parameterDic.Add(new KeyValuePair<string, string>("exch", exSecID.ExID));
            parameterDic.Add(new KeyValuePair<string, string>("secId", FormatSecurityIdForKLineRequest(exSecID)));
            parameterDic.Add(new KeyValuePair<string, string>("periodType", kLineType.ToString()));
            parameterDic.Add(new KeyValuePair<string, string>("num", count.ToString()));
            parameterDic.Add(new KeyValuePair<string, string>("end", DateTimeHelper.ConvertToDateTimeInt(endDateTime).ToString()));
            parameterDic.Add(new KeyValuePair<string, string>("exrightType", exRightType.ToString().ToLower()));

            return await GetKLineRecords(exSecID, eRequestTypes.GetKLineRecordsWithCount, parameterDic);
        }

        private async Task<IList<KLineRecordModel>> GetKLineRecords(ExSecID exSecID, eRequestTypes requestType, IList<KeyValuePair<string, string>> parameters)
        {
            string responseStr = await SendGetRequestToServer(requestType, parameters);
            IList<KLineRecordModel> list = null;
            if (responseStr != String.Empty)
            {
                Logger.Debug("Get K Line records http response");
                try
                {
                    var kLineQueryResponse = CommonUtil.GetObjectFromJsonString(responseStr, typeof(KLineRecordsQueryResponse)) as KLineRecordsQueryResponse;
                    if (null != kLineQueryResponse && kLineQueryResponse.KLineRecords.Count > 0)
                    {
                        Logger.Debug("Total count of K Line records:" + kLineQueryResponse.KLineRecords.Count);

                        list = kLineQueryResponse.KLineRecords.Select(x => x.ToStockTickDataModel()).ToList();
                    }
                    else
                    {
                        Logger.Error("返回的K线列表为空!");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("K线列表获取失败:" + ex.Message);
                }

            }

            return list;
        }

        public void DailyReInitialize()
        {
            GetInitData();
        }
    }
}

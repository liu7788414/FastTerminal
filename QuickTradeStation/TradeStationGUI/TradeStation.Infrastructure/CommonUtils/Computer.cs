// 注意：首先要在项目bin目录中添加引用 System.Management  

using System;
using System.Management;


namespace TradeStation.Infrastructure.CommonUtils
{
    /// <summary>  
    ///Computer 的摘要说明  
    /// </summary>  

    public class Computer
    {
        private readonly string _macAddress; //计算机的MAC地址  

        public string MacAddress
        {
            get { return _macAddress; }
        }

        private readonly string _diskId; //硬盘的ID  

        public string DiskId
        {
            get { return _diskId; }
        }

        private readonly string _ipAddress; //计算机的IP地址  

        public string IpAddress
        {
            get { return _ipAddress; }
        }

        private static Computer _instance;

        public static Computer Instance()
        {
            return _instance ?? (_instance = new Computer());
        }

        private Computer()
        {
            _macAddress = GetMacAddress();
            _diskId = GetDiskId();
            _ipAddress = GetIpAddress();
        }

        private string GetMacAddress()
        {
            try
            {
                //获取网卡硬件地址   
                var mac = " ";
                var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                var moc = mc.GetInstances();
                foreach (var mo in moc)
                {
                    if ((bool) mo["IPEnabled"])
                    {
                        mac = mo["MacAddress"].ToString();
                        break;
                    }
                }
                return mac;
            }
            catch
            {
                return "unknow";
            }
        }

        private string GetIpAddress()
        {
            try
            {
                //获取IP地址   
                var st = " ";
                var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                var moc = mc.GetInstances();
                foreach (var o in moc)
                {
                    var mo = (ManagementObject) o;
                    if ((bool) mo["IPEnabled"])
                    {
                        Array ar;
                        ar = (Array) mo.Properties["IpAddress"].Value;
                        st = ar.GetValue(0).ToString();
                        break;
                    }
                }
                return st;
            }
            catch
            {
                return "unknow";
            }
        }

        private string GetDiskId()
        {
            try
            {
                //获取硬盘ID   
                var hDid = " ";
                var mc = new ManagementClass("Win32_DiskDrive");
                var moc = mc.GetInstances();
                foreach (var o in moc)
                {
                    var mo = (ManagementObject) o;
                    hDid = (string) mo.Properties["Model"].Value;
                }
                return hDid;
            }
            catch
            {
                return "unknow";
            }
        }
    }

}


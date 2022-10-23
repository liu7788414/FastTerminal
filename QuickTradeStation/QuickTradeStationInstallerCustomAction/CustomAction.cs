using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace QuickTradeStationInstallerCustomAction
{
    [RunInstaller(true)]
    public partial class CustomAction : Installer
    {
        public static readonly string AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
        public static readonly string UpdaterConfigFile = AssemblyPath + "UpdaterConfig.txt";

        public CustomAction()
        {
            InitializeComponent();
        }

        public CustomAction(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            try
            {
                if (
                    TopMostMessageBox.Show("本客户端将运行于[测试环境]吗？ \n [是]-[测试环境]\n [否]-[生产环境]", "请选择客户端的运行环境",
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    SetUpdaterConfigFile("服务器IP和端口=192.168.41.50:8080");
                }
                else
                {
                    SetUpdaterConfigFile("服务器IP和端口=10.41.8.19:8080");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.Source + ex.StackTrace);
            }
        }


        public void SetUpdaterConfigFile(string config)
        {
            if (File.Exists(UpdaterConfigFile))
            {
                var sw = new StreamWriter(UpdaterConfigFile, false, Encoding.UTF8);
                sw.WriteLine(config);
                sw.Close();
            }
            else
            {
                MessageBox.Show(string.Format("未找到文件{0}", UpdaterConfigFile), "Error");
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
        }
    }
}

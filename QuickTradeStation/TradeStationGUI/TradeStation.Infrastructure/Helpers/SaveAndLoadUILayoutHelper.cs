using Infragistics.Windows.DockManager;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TradeStation.Infrastructure.CommonUtils;

namespace TradeStation.Infrastructure.Helpers
{
    public static class SaveAndLoadUILayoutHelper
    {
        public static void LoadLayout(string layoutFile, XamDockManager dockManager)
        {
            try
            {
                if (File.Exists(CommonUtil.AssemblyPath + layoutFile))
                {
                    LoadLayoutFromFile(CommonUtil.AssemblyPath + layoutFile, dockManager);
                }
                else
                {
                    if (File.Exists(CommonUtil.DefaultLayoutPath + layoutFile))
                    {
                        LoadLayoutFromFile(CommonUtil.DefaultLayoutPath + layoutFile, dockManager);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception during Save:" + Environment.NewLine + ex.Message, "SaveLayout Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void SaveLayout(string layoutFile, XamDockManager dockManager)
        {
            // Create a memory stream
            var stream = new MemoryStream();

            try
            {
                if (File.Exists(layoutFile))
                {
                    File.Delete(layoutFile);
                }

                dockManager.SaveLayout(stream);
                var fs = new FileStream(layoutFile, FileMode.OpenOrCreate);
                var w = new BinaryWriter(fs);
                w.Write(stream.ToArray());
                fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception during Save:" + Environment.NewLine + ex.Message, "SaveLayout Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                stream.Close();
            }
        }

        private static void LoadLayoutFromFile(string layoutFile, XamDockManager dockManager)
        {
            var fs = new FileStream(layoutFile, FileMode.Open);
            var sr = new StreamReader(fs, Encoding.UTF8);
            var stream = sr.ReadToEnd();
            dockManager.LoadLayout(stream);
            sr.Close();
            fs.Close();
        }
    }
}

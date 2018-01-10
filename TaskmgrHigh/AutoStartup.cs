using System;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;

namespace TaskmgrHigh
{
    class AutoStartup
    {
        static string Key = "TaskmgrHigh_" + Application.StartupPath.GetHashCode();
        static string RegistryRunPath = (IntPtr.Size == 4 ? @"Software\Microsoft\Windows\CurrentVersion\Run" : @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Run");

        private static string GetExecutablePath()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().Location;
        }
        
        private static int RunAsAdmin(string Arguments)
        {
            Process process = null;
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.Verb = "runas";
            processInfo.FileName = Application.ExecutablePath;
            processInfo.Arguments = Arguments;
            try
            {
                process = Process.Start(processInfo);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                return -1;
            }
            if (process != null)
            {
                process.WaitForExit();
            }
            int ret = process.ExitCode;
            process.Close();
            return ret;
        }

        public static bool Set(bool enabled)
        {
            RegistryKey runKey = null;
            try
            {
                string path = GetExecutablePath();
                runKey = Registry.LocalMachine.OpenSubKey(RegistryRunPath, true);
                if (enabled)
                {
                    runKey.SetValue(Key, path);
                }
                else
                {
                    runKey.DeleteValue(Key);
                }
                runKey.Close();
                return true;
            }
            catch //(Exception e)
            {
                //Logging.LogUsefulException(e);
                return RunAsAdmin("--setautorun") == 0;
            }
            finally
            {
                if (runKey != null)
                {
                    try
                    {
                        runKey.Close();
                    }
                    catch (Exception e)
                    {
                        // Logging.LogUsefulException(e);
                        MessageBox.Show(e.Message);
                    }
                }
            }
        }

        public static bool Switch()
        {
            bool enabled = !Check();
            RegistryKey runKey = null;
            try
            {
                string path = GetExecutablePath();
                runKey = Registry.LocalMachine.OpenSubKey(RegistryRunPath, true);
                if (enabled)
                {
                    runKey.SetValue(Key, path);
                }
                else
                {
                    runKey.DeleteValue(Key);
                }
                runKey.Close();
                return true;
            }
            catch (Exception e)
            {
                // Logging.LogUsefulException(e);
                MessageBox.Show(e.Message);
                return false;
            }
            finally
            {
                if (runKey != null)
                {
                    try
                    {
                        runKey.Close();
                    }
                    catch (Exception e)
                    {
                        // Logging.LogUsefulException(e);
                        MessageBox.Show(e.Message);
                    }
                }
            }
        }

        public static bool Check()
        {
            RegistryKey runKey = null;
            try
            {
                string path = GetExecutablePath();
                runKey = Registry.LocalMachine.OpenSubKey(RegistryRunPath, false);
                string[] runList = runKey.GetValueNames();
                runKey.Close();
                foreach (string item in runList)
                {
                    if (item.Equals(Key))
                        return true;
                }
                return false;
            }
            catch (Exception e)
            {
                // Logging.LogUsefulException(e);
                MessageBox.Show(e.Message);
                return false;
            }
            finally
            {
                if (runKey != null)
                {
                    try
                    {
                        runKey.Close();
                    }
                    catch (Exception e)
                    {
                        // Logging.LogUsefulException(e);
                        MessageBox.Show(e.Message);
                    }
                }
            }
        }
    }
}

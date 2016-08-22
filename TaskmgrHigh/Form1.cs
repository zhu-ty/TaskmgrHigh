﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.NetworkInformation;
using Microsoft.Win32;
using System.Reflection;
using System.Management;

namespace TaskmgrHigh
{



    public partial class Form1 : Form
    {
        Icon ico = new Icon("ICON.ico");
        ContextMenu notifyContextMenu = new ContextMenu();
        KeyboardHook kh = new KeyboardHook();
        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;

            switch (m.Msg)
            {
                case WM_HOTKEY:
                    switch (m.WParam.ToInt32())
                    {
                        case 1:    //按下的是Win+A
                            StartTMG();
                            break;
                    }
                    break;
            }
            base.WndProc(ref m);
        }//HotKey

        ///<summary>
        /// 该函数设置由不同线程产生的窗口的显示状态
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="cmdShow">指定窗口如何显示。查看允许值列表，请查阅ShowWlndow函数的说明部分</param>
        /// <returns>如果函数原来可见，返回值为非零；如果函数原来被隐藏，返回值为零</returns>
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);


        /// <summary>
        ///  该函数将创建指定窗口的线程设置到前台，并且激活该窗口。键盘输入转向该窗口，并为用户改各种可视的记号。
        ///  系统给创建前台窗口的线程分配的权限稍高于其他线程。 
        /// </summary>
        /// <param name="hWnd">将被激活并被调入前台的窗口句柄</param>
        /// <returns>如果窗口设入了前台，返回值为非零；如果窗口未被设入前台，返回值为零</returns>
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                notifyIcon1.Text = "TaskmgrHigh";
                notifyIcon1.Icon = ico;
                notifyIcon1.Text = "Right click me for more info.";
                notifyIcon1.ContextMenuStrip = contextMenuStrip1;
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
                kh.SetHook();
                kh.OnKeyDownEvent += kh_OnKeyDownEvent;
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
                notifyIcon1.Visible = true;
                Hide();
                //MessageBox.Show("启动成功！");
            }
            catch
            {
                MessageBox.Show("启动失败！");
                Close();
            }
        }

        void kh_OnKeyDownEvent(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.T | Keys.Control | Keys.Alt)) { StartTMG(); }
        }

        private void StartTMG()
        {
            Process myprocess = new Process();
            //myprocess.PriorityClass = ProcessPriorityClass.High;
            myprocess = Process.Start("Taskmgr.exe");
            myprocess.PriorityClass = ProcessPriorityClass.High;
            ShowWindowAsync(myprocess.MainWindowHandle, 1);
            SetForegroundWindow(myprocess.MainWindowHandle);
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            //已经取消
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
            else
                contextMenuStrip1.Hide();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            kh.UnHook();
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                //IPAddress ip0,ip1;
                //ip0 = Dns.GetHostAddresses(Dns.GetHostName())[0];
                ////ip1 = Dns.GetHostAddresses(Dns.GetHostName())[1];
                //foreach (IPAddress ip in Dns.GetHostAddresses(Dns.GetHostName()))
                //{
                //    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                //    {
                //        ip0 = ip;
                //        break;
                //    }
                //    else if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                //    {
                //        ip1 = ip;
                //    }
                //}
                //string s;
                //s = "IPv4: " + ip0.ToString() +"\nActive Mac:" +GetActivatedAdaptorMacAddress();
                ////notifyIcon1.Text = s;


                //启用新版字串生成
                string s = GetActivatedAdaptorText();
                Fixes.SetNotifyIconText(notifyIcon1, s);
                string add = await get_outside_v4();
                if (add != "")
                    s += "\n[out] Ipv4:" + add;
                if (s != "")
                    s += "\n";
                s += "Press Ctrl+Alt+T to start taskmgr.";
                Fixes.SetNotifyIconText(notifyIcon1, s);
            }
            catch (Exception)
            {
            }
        }

        public string GetMacAddressByNetworkInformation()
        {
            //string key = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\";
            string macAddress = string.Empty;
            string ans = string.Empty;
            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in nics)
                {
                    //if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                    //    && adapter.GetPhysicalAddress().ToString().Length != 0)
                    //{
                    //    string fRegistryKey = key + adapter.Id + "\\Connection";
                    //    RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                    //    if (rk != null)
                    //    {
                    //        string fPnpInstanceID = rk.GetValue("PnpInstanceID", "").ToString();
                    //        int fMediaSubType = Convert.ToInt32(rk.GetValue("MediaSubType", 0));
                    //        if (fPnpInstanceID.Length > 3 &&
                    //            fPnpInstanceID.Substring(0, 3) == "PCI")
                    //        {
                    //            if (ans != "")
                    //                ans += "\n";
                    //            macAddress = adapter.GetPhysicalAddress().ToString();
                    //            for (int i = 1; i < 6; i++)
                    //            {
                    //                macAddress = macAddress.Insert(3 * i - 1, ":");
                    //            }
                    //            ans += "Mac:" + macAddress;
                    //            //break;
                    //        }
                    //    }

                    //}
                    //if (ans != "")
                    //    ans += "\n";
                    macAddress = adapter.GetPhysicalAddress().ToString();
                    for (int i = 1; i < 6; i++)
                    {
                        macAddress = macAddress.Insert(3 * i - 1, ":");
                    }
                    ans += "\nMac:" + macAddress;
                }
            }
            catch (Exception ex)
            {
                //这里写异常的处理
            }
            return ans;
        }

        /// <summary>
        /// 获得当前机器的活动中Mac地址，若无联网则返回空""
        /// </summary>
        /// <returns>mac地址，例如：18:03:73:AE:38:0D</returns>
        public static string GetActivatedAdaptorText()
        {
            try
            {
                string ans = "";
                //string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if (mo["IPEnabled"].ToString() == "True")
                    {
                        //ans = "Active Network:";
                        ans = "";
                        ans = ans + "Mac:" + mo["MacAddress"].ToString();
                        string[] a = (string[])mo["IPAddress"];
                        bool already_v4 = false, already_v6 = false;
                        foreach (string c in a)
                        {
                            IPAddress abc = IPAddress.Parse(c);
                            if (abc.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && (!already_v4))
                            {
                                ans = ans + "\nIpv4:" + c;
                                already_v4 = true;
                            }
                            else if (abc.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 && (!already_v6))
                            {
                                ans = ans + "\nIpv6:" + c;
                                already_v6 = true;
                            }
                            if (already_v4 && already_v6)
                                break;
                        }
                        //string ov4 = get_outside_v4();
                        //if (ov4 != "")
                        //{
                        //    ans = ans + "\n[Out] Ipv4:" + ov4;
                        //}
                    }
                }
                return ans;
            }
            catch (Exception)
            {
                return "";
            }

        }

        private void 更新IP与MACToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1_Tick(this, new EventArgs());
        }

        async private static Task<string> get_outside_v4()
        {
            string ans = "";
            await Task.Run(() =>
                {
                    try
                    {
                        string direction = "";
                        WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
                        request.Timeout = 10000;
                        using (WebResponse response = request.GetResponse())
                        using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                        {
                            direction = stream.ReadToEnd();
                        }
                        int first = direction.IndexOf("Address:") + 9;
                        int last = direction.LastIndexOf("</body>");
                        ans = direction.Substring(first, last - first);
                    }
                    catch (Exception ex)
                    {
                        ans = "";
                    }
                });
            return ans;
        }

    }
    public class Fixes
    {
        public static void SetNotifyIconText(NotifyIcon ni, string text)
        {
            if (text.Length >= 65535) throw new ArgumentOutOfRangeException("Text limited to 127 characters");
            Type t = typeof(NotifyIcon);
            BindingFlags hidden = BindingFlags.NonPublic | BindingFlags.Instance;
            t.GetField("text", hidden).SetValue(ni, text);
            if ((bool)t.GetField("added", hidden).GetValue(ni))
                t.GetMethod("UpdateIcon", hidden).Invoke(ni, new object[] { true });
        }
    }
}

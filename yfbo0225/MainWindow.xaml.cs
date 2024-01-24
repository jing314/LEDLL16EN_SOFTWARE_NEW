using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using yfbo0225.Pages;

namespace yfbo0225
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // 串口控件声明
        public SerialPort Com_SerialPort = new SerialPort();
        //热插拔实现定时器
        private DispatcherTimer Serial_GetPortConfig_Updata_Timer = new DispatcherTimer();
        // 供子窗体使用
        public static MainWindow MainWindow_Win;
        // 创建子窗体
        public static Config_Page Config_Page_Win;
        public MainWindow()
        {
            InitializeComponent();
            MainWindow_Win = this;
            InitialSetting();
        }
        //初始化设置
        private void InitialSetting()
        {
            //定时500ms串口设备监控
            Serial_GetPortConfig_Updata_Timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            Serial_GetPortConfig_Updata_Timer.Tick += new EventHandler(Serial_GetPortConfig_Timer_Tick);
            Serial_GetPortConfig_Updata_Timer.Start();
        }
        //获取串口完整名字（包括驱动名字)
        Dictionary<String, String> Com_Device_Config = new Dictionary<String, String>();
        private void getPortDeviceName()
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher
            ("select * from Win32_PnPEntity where Name like '%(COM%'"))
            {
                var hardInfos = searcher.Get();
                Com_Device_Config.Clear();
                foreach (var hardInfo in hardInfos)
                {

                    if (hardInfo.Properties["Name"].Value != null)
                    {
                        string deviceName = hardInfo.Properties["Name"].Value.ToString();
                        int startIndex = deviceName.IndexOf("(");
                        int endIndex = deviceName.IndexOf(")");
                        string key = deviceName.Substring(startIndex + 1, deviceName.Length - startIndex - 2);
                        string name = deviceName.Substring(0, startIndex - 1);
                        Com_Device_Config.Add(key, name);
                    }
                    Serial_PortNumber.Items.Clear();
                    foreach (KeyValuePair<string, string> kvp in Com_Device_Config)
                    {
                        Serial_PortNumber.Items.Add(kvp.Key);//更新下拉列表中的串口
                    }
                }
            }
            Serial_PortNumber.SelectedIndex = 0;
            //获取刷新后的第一个设备名称
            // Serial_PortConfig.Content = Serial_PortNumber.Text.ToString() + ":" + Com_Device_Config[Serial_PortNumber.Text.ToString()];
        }
        //串口热插拔
        private void Serial_GetPortConfig_Timer_Tick(object sender, EventArgs e)
        {
            string[] serial_portname = SerialPort.GetPortNames();

            //设备数发生变换
            if (Serial_PortNumber.Items.Count != serial_portname.Length)
            {
                getPortDeviceName();
            }
        }
        // 进入配置页面
        private void In_Config_Page_Click(object sender, RoutedEventArgs e)
        {
            // 判断子窗口为空
            if (Config_Page_Win == null)
            {
                // 隐藏当前窗口
                this.Hide();
                // 创建CAN升级窗口
                Config_Page_Win = new Config_Page();
                // 显示CAN升级窗口
                Config_Page_Win.Show();
            }
        }
    }
}

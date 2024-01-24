using HandyControl.Controls;
using HandyControl.Tools.Extension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace yfbo0225.Pages
{
    /// <summary>
    /// Config_Page.xaml 的交互逻辑
    /// </summary>
    public partial class Config_Page : System.Windows.Window
    {
        private bool First_Run_Flag = true;

        public Config_Page()
        {
            InitializeComponent();
            InitialSetting();
        }
        //初始化设置
        private void InitialSetting()
        {
            // 打开串口
            //获得串口端口信息为:  COM1:通讯端口
            MainWindow.MainWindow_Win.Com_SerialPort.PortName = MainWindow.MainWindow_Win.Serial_PortNumber.Text.Trim();
            MainWindow.MainWindow_Win.Com_SerialPort.BaudRate = 9600;
            MainWindow.MainWindow_Win.Com_SerialPort.DataBits = 8;
            MainWindow.MainWindow_Win.Com_SerialPort.Parity = Parity.None;
            MainWindow.MainWindow_Win.Com_SerialPort.StopBits = StopBits.One;
            MainWindow.MainWindow_Win.Com_SerialPort.ReadTimeout = 1000;
            MainWindow.MainWindow_Win.Com_SerialPort.WriteBufferSize = 1024;
            MainWindow.MainWindow_Win.Com_SerialPort.ReadBufferSize = 4096;
            MainWindow.MainWindow_Win.Com_SerialPort.RtsEnable = false;
            MainWindow.MainWindow_Win.Com_SerialPort.DiscardNull = false;
            MainWindow.MainWindow_Win.Com_SerialPort.ReceivedBytesThreshold = 1;
            MainWindow.MainWindow_Win.Com_SerialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceivedEventHandler);
            try
            {
                MainWindow.MainWindow_Win.Com_SerialPort.Open();
                MainWindow.MainWindow_Win.Com_SerialPort.DiscardInBuffer();
                MainWindow.MainWindow_Win.Com_SerialPort.DiscardOutBuffer();
                Console.WriteLine("串口打开成功！！！");
            }
            catch
            {
                Console.WriteLine("串口打开失败！！！");
            }
            // 绑定按键回调
            for(int count = 1;count < 11;count++)
            {
                for(int ch = 0;ch < 16;ch++)
                {
                    var findName = (Button)FindName(string.Format("Button_ID" + count.ToString() + "_CH" + ch.ToString()));
                    // 防止界面无此名称控件导致报错
                    if (findName != null)
                    {
                        findName.Click += new RoutedEventHandler(Button_Click);
                    }
                }
            }
            led_pixel_set(false);
            // 设置按键关闭
            Max_Light_Button.IsEnabled = false;
            Mix_Light_Button.IsEnabled = false;
            Bre_Speed_Button.IsEnabled = false;
            Flow_Speed_Button.IsEnabled = false;
            Bre_Count_Button.IsEnabled = false;
            
            Select_Mode.SelectedIndex = 0;

        }
        private void led_pixel_set(bool state)
        {
            if(!state)
            {
                // 绑定按键回调
                for (int count = 1; count < 11; count++)
                {
                    for (int ch = 0; ch < 16; ch++)
                    {
                        var findName = (Button)FindName(string.Format("Button_ID" + count.ToString() + "_CH" + ch.ToString()));
                        // 防止界面无此名称控件导致报错
                        if (findName != null)
                        {
                            findName.IsEnabled = false;
                        }
                    }
                }
            }
            else
            {
                // 绑定按键回调
                for (int count = 1; count < 11; count++)
                {
                    for (int ch = 0; ch < 16; ch++)
                    {
                        var findName = (Button)FindName(string.Format("Button_ID" + count.ToString() + "_CH" + ch.ToString()));
                        // 防止界面无此名称控件导致报错
                        if (findName != null)
                        {
                            findName.IsEnabled = true;
                        }
                    }
                }
            }
        }

        private void serialPort_DataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e)
        {
            //读取串口缓冲区接收数据的字节数
            int buff_num = MainWindow.MainWindow_Win.Com_SerialPort.BytesToRead;
            //判断数据为空是跳过
            if (buff_num == 0)
            {
                return;
            }
            
            //创建一个与缓冲区数据数相同的数组
            byte[] buf = new byte[buff_num];
            MainWindow.MainWindow_Win.Com_SerialPort.Read(buf, 0, buff_num);

            Console.WriteLine(buff_num);
            
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            // 显示主窗口
            MainWindow.MainWindow_Win.Com_SerialPort.Close();
            MainWindow.MainWindow_Win.Show();
            MainWindow.Config_Page_Win = null;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 添加蒙版
            Set_Page.Opacity = 0.65;
            Button btn = sender as Button;

            string[] Id_Ch_String = btn.Name.Substring(7).Split('_');

            Input_MessageBox.Show(Convert.ToInt32(Id_Ch_String[0].Substring(2)), Convert.ToInt32(Id_Ch_String[1].Substring(2)));

            Console.WriteLine(Input_MessageBox.Light_Pwm_Value);

            byte[] send_cmd = new byte[6];
            send_cmd[0] = (byte)'<';
            send_cmd[1] = (byte)0x03;
            send_cmd[2] = (byte)Convert.ToInt32(Id_Ch_String[0].Substring(2));
            send_cmd[3] = (byte)Convert.ToInt32(Id_Ch_String[1].Substring(2));
            send_cmd[4] = (byte)((byte)Input_MessageBox.Light_Pwm_Value);
            send_cmd[5] = (byte)'>';
            // 发送同步
            MainWindow.MainWindow_Win.Com_SerialPort.Write(send_cmd, 0, 6);

            // 取消蒙版
            Set_Page.Opacity = 1.0;
        }

        private void Max_Light_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Max_Light_Lable.Content = "最高亮度：" + Convert.ToInt16(Max_Light_Slider.Value).ToString();
        }

        private void Mix_Light_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Mix_Light_Lable.Content = "最低亮度：" + Convert.ToInt16(Mix_Light_Slider.Value).ToString();
        }

        private void Bre_Speed_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Bre_Speed_Lable.Content = "呼吸速度：" + Convert.ToInt16(Bre_Speed_Slider.Value).ToString();
        }

        private void Flow_Speed_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Flow_Speed_Lable.Content = "流水速度：" + Convert.ToInt16(Flow_Speed_Slider.Value).ToString();
        }


        private void Bre_Count_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Bre_Count_Lable.Content = "呼吸灯数：" + Convert.ToInt16(Bre_Count_Slider.Value).ToString();
        }

        private void Select_Mode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(Select_Mode.SelectedIndex)
            {
                case 0:
                    {
                        Max_Light_Button.IsEnabled = true;
                        Mix_Light_Button.IsEnabled = false;
                        Bre_Speed_Button.IsEnabled = false;
                        Flow_Speed_Button.IsEnabled = false;
                        Bre_Count_Button.IsEnabled = false;
                        led_pixel_set(false);
                    } break;
                case 1:
                    {
                        Max_Light_Button.IsEnabled = true;
                        Mix_Light_Button.IsEnabled = true;
                        Bre_Speed_Button.IsEnabled = true;
                        Flow_Speed_Button.IsEnabled = false;
                        Bre_Count_Button.IsEnabled = false;
                        led_pixel_set(false);
                    } break;
                case 2:
                    {
                        Max_Light_Button.IsEnabled = true;
                        Mix_Light_Button.IsEnabled = false;
                        Bre_Speed_Button.IsEnabled = false;
                        Flow_Speed_Button.IsEnabled = true;
                        Bre_Count_Button.IsEnabled = false;
                        led_pixel_set(false);
                    } break;
                case 3:
                    {
                        Max_Light_Button.IsEnabled = true;
                        Mix_Light_Button.IsEnabled = false;
                        Bre_Speed_Button.IsEnabled = false;
                        Flow_Speed_Button.IsEnabled = true;
                        Bre_Count_Button.IsEnabled = false;
                        led_pixel_set(false);
                    } break;
                case 4:
                    {
                        Max_Light_Button.IsEnabled = true;
                        Mix_Light_Button.IsEnabled = false;
                        Bre_Speed_Button.IsEnabled = false;
                        Flow_Speed_Button.IsEnabled = true;
                        Bre_Count_Button.IsEnabled = false;
                        led_pixel_set(false);
                    } break;
                case 5:
                    {
                        Max_Light_Button.IsEnabled = false;
                        Mix_Light_Button.IsEnabled = false;
                        Bre_Speed_Button.IsEnabled = false;
                        Flow_Speed_Button.IsEnabled = true;
                        Bre_Count_Button.IsEnabled = false;
                        led_pixel_set(false);
                    }
                    break;
                case 6:
                    {
                        Max_Light_Button.IsEnabled = false;
                        Mix_Light_Button.IsEnabled = false;
                        Bre_Speed_Button.IsEnabled = false;
                        Flow_Speed_Button.IsEnabled = true;
                        Bre_Count_Button.IsEnabled = false;
                        led_pixel_set(false);
                    }
                    break;
                case 7:
                    {
                        Max_Light_Button.IsEnabled = false;
                        Mix_Light_Button.IsEnabled = false;
                        Bre_Speed_Button.IsEnabled = false;
                        Flow_Speed_Button.IsEnabled = false;
                        Bre_Count_Button.IsEnabled = true;
                        led_pixel_set(false);
                    }
                    break;
                case 8:
                    {
                        Max_Light_Button.IsEnabled = false;
                        Mix_Light_Button.IsEnabled = false;
                        Bre_Speed_Button.IsEnabled = false;
                        Flow_Speed_Button.IsEnabled = false;
                        Bre_Count_Button.IsEnabled = false;
                        led_pixel_set(true);
                    }
                    break;
                case 9:
                    {
                        Max_Light_Button.IsEnabled = false;
                        Mix_Light_Button.IsEnabled = false;
                        Bre_Speed_Button.IsEnabled = false;
                        Flow_Speed_Button.IsEnabled = false;
                        Bre_Count_Button.IsEnabled = false;
                        led_pixel_set(false);
                    }
                    break;
            }
            byte[] send_cmd = new byte[6];
            send_cmd[0] = (byte)'<';
            send_cmd[1] = (byte)0x01;
            send_cmd[2] = (byte)0x00;
            send_cmd[3] = (byte)0x00;
            send_cmd[4] = (byte)((byte)Select_Mode.SelectedIndex + 1);
            send_cmd[5] = (byte)'>';
            // 发送同步
            MainWindow.MainWindow_Win.Com_SerialPort.Write(send_cmd,0,6);

            if(First_Run_Flag)
            {
                Thread.Sleep(50);
                send_cmd[1] = (byte)0x02;
                send_cmd[3] = (byte)0x01;
                send_cmd[4] = (byte)((byte)Max_Light_Slider.Value);
                MainWindow.MainWindow_Win.Com_SerialPort.Write(send_cmd, 0, 6);
                Thread.Sleep(50);
                send_cmd[1] = (byte)0x02;
                send_cmd[3] = (byte)0x02;
                send_cmd[4] = (byte)((byte)Mix_Light_Slider.Value);
                MainWindow.MainWindow_Win.Com_SerialPort.Write(send_cmd, 0, 6);
                Thread.Sleep(50);
                send_cmd[1] = (byte)0x02;
                send_cmd[3] = (byte)0x03;
                send_cmd[4] = (byte)((byte)Bre_Speed_Slider.Maximum - ((byte)Bre_Speed_Slider.Value));
                MainWindow.MainWindow_Win.Com_SerialPort.Write(send_cmd, 0, 6);
                Thread.Sleep(50);
                send_cmd[1] = (byte)0x02;
                send_cmd[3] = (byte)0x04;
                send_cmd[4] = (byte)((byte)Flow_Speed_Slider.Maximum - ((byte)Flow_Speed_Slider.Value));
                MainWindow.MainWindow_Win.Com_SerialPort.Write(send_cmd, 0, 6);
                Thread.Sleep(50);
                send_cmd[1] = (byte)0x02;
                send_cmd[3] = (byte)0x06;
                send_cmd[4] = (byte)((byte)Bre_Count_Slider.Value);
                MainWindow.MainWindow_Win.Com_SerialPort.Write(send_cmd, 0, 6);

                Console.WriteLine("First_Run_Flag");
                First_Run_Flag = false;
            }
        }

        private void Max_Light_Button_Click(object sender, RoutedEventArgs e)
        {
            byte[] send_cmd = new byte[6];
            send_cmd[0] = (byte)'<';
            send_cmd[1] = (byte)0x02;
            send_cmd[2] = (byte)0x00;
            send_cmd[3] = (byte)0x01;
            send_cmd[4] = (byte)((byte)Max_Light_Slider.Value);
            send_cmd[5] = (byte)'>';
            // 发送同步
            MainWindow.MainWindow_Win.Com_SerialPort.Write(send_cmd, 0, 6);
        }

        private void Mix_Light_Button_Click(object sender, RoutedEventArgs e)
        {
            byte[] send_cmd = new byte[6];
            send_cmd[0] = (byte)'<';
            send_cmd[1] = (byte)0x02;
            send_cmd[2] = (byte)0x00;
            send_cmd[3] = (byte)0x02;
            send_cmd[4] = (byte)((byte)Mix_Light_Slider.Value);
            send_cmd[5] = (byte)'>';
            // 发送同步
            MainWindow.MainWindow_Win.Com_SerialPort.Write(send_cmd, 0, 6);
        }

        private void Bre_Speed_Button_Click(object sender, RoutedEventArgs e)
        {
            byte[] send_cmd = new byte[6];
            send_cmd[0] = (byte)'<';
            send_cmd[1] = (byte)0x02;
            send_cmd[2] = (byte)0x00;
            send_cmd[3] = (byte)0x03;
            send_cmd[4] = (byte)((byte)Bre_Speed_Slider.Maximum - ((byte)Bre_Speed_Slider.Value));
            send_cmd[5] = (byte)'>';
            // 发送同步
            MainWindow.MainWindow_Win.Com_SerialPort.Write(send_cmd, 0, 6);
        }

        private void Flow_Speed_Button_Click(object sender, RoutedEventArgs e)
        {
            byte[] send_cmd = new byte[6];
            send_cmd[0] = (byte)'<';
            send_cmd[1] = (byte)0x02;
            send_cmd[2] = (byte)0x00;
            send_cmd[3] = (byte)0x04;
            send_cmd[4] = (byte)((byte)Flow_Speed_Slider.Maximum - ((byte)Flow_Speed_Slider.Value));
            send_cmd[5] = (byte)'>';
            // 发送同步
            MainWindow.MainWindow_Win.Com_SerialPort.Write(send_cmd, 0, 6);
        }

        private void Bre_Count_Button_Click(object sender, RoutedEventArgs e)
        {
            byte[] send_cmd = new byte[6];
            send_cmd[0] = (byte)'<';
            send_cmd[1] = (byte)0x02;
            send_cmd[2] = (byte)0x00;
            send_cmd[3] = (byte)0x06;
            send_cmd[4] = (byte)((byte)Bre_Count_Slider.Value);
            send_cmd[5] = (byte)'>';
            // 发送同步
            MainWindow.MainWindow_Win.Com_SerialPort.Write(send_cmd, 0, 6);
        }
    }
}

using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
    /// Input_MessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class Input_MessageBox : System.Windows.Window
    {
        public static Int32 Light_Pwm_Value = 0;

        public const string PATTERN = "\\b(1?[0-9]{1,2}|2[0-4][0-9]|25[0-5])\\b";
        public Input_MessageBox()
        {
            InitializeComponent();
        }
        public static bool? Show(int id,int ch)
        {
            var InputBox = new Input_MessageBox();
            InputBox.Tilte_Lable.Content = "ID：" + id.ToString().PadLeft(2, '0') + " CH：" + ch.ToString().PadLeft(2, '0') + "设置亮度";
            return InputBox.ShowDialog();
        }

        private void Light_Value_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Light_Value.Text == "")
                return;
            // 判断ID是否合法
            if (System.Text.RegularExpressions.Regex.IsMatch(Light_Value.Text, PATTERN))
            {
                if ((Convert.ToInt32(Light_Value.Text) <= 255) && (Convert.ToInt32(Light_Value.Text) >= 0))
                {
                    Light_Pwm_Value = Convert.ToInt32(Light_Value.Text);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    Light_Value.Text = "";
                }
            }
            else
            {
                Light_Value.Text = "";
            }
        }

        private void Exti_Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}

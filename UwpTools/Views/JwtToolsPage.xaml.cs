using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Newtonsoft.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Text;
using Windows.ApplicationModel.DataTransfer;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了"用户控件"项模板

namespace UwpTools.Views
{
    public sealed partial class JwtToolsPage : Page
    {
        public JwtToolsPage()
        {
            this.InitializeComponent();
        }

        private void ValidateButton_Click(object sender, RoutedEventArgs e)
        {
            string jwt = InputTextBox.Text;
            if (string.IsNullOrEmpty(jwt))
            {
                OutputTextBox.Text = "请输入JWT令牌";
                return;
            }

            try
            {
                string[] parts = jwt.Split('.');
                if (parts.Length < 2)
                {
                    OutputTextBox.Text = "无效的JWT格式";
                    return;
                }

                string header = DecodeBase64(parts[0]);
                string payload = DecodeBase64(parts[1]);

                var headerObj = JsonConvert.DeserializeObject(header);
                var payloadObj = JsonConvert.DeserializeObject(payload);

                string result = $"Header:\n{JsonConvert.SerializeObject(headerObj, Formatting.Indented)}\n\nPayload:\n{JsonConvert.SerializeObject(payloadObj, Formatting.Indented)}";
                
                OutputTextBox.Text = result;
            }
            catch (Exception ex)
            {
                OutputTextBox.Text = $"解码错误: {ex.Message}";
            }
        }

        private string DecodeBase64(string base64String)
        {
            // 修复Base64字符串，添加缺少的填充字符
            string paddedBase64 = base64String;
            switch (base64String.Length % 4)
            {
                case 2:
                    paddedBase64 += "==";
                    break;
                case 3:
                    paddedBase64 += "=";
                    break;
            }

            byte[] data = Convert.FromBase64String(paddedBase64);
            return Encoding.UTF8.GetString(data);
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(OutputTextBox.Text))
            {
                DataPackage dataPackage = new DataPackage();
                dataPackage.SetText(OutputTextBox.Text);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Text = "";
            OutputTextBox.Text = "";
        }
    }
}
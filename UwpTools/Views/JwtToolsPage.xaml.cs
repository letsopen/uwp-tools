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

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了"用户控件"项模板

namespace UwpTools.Views
{
    public sealed partial class JwtToolsPage : Page
    {
        public JwtToolsPage()
        {
            this.InitializeComponent();
        }

        private void DecodeJwt_Click(object sender, RoutedEventArgs e)
        {
            string jwt = JwtTextBox.Text;
            if (string.IsNullOrEmpty(jwt))
            {
                ResultTextBox.Text = "请输入JWT令牌";
                return;
            }

            try
            {
                string[] parts = jwt.Split('.');
                if (parts.Length < 2)
                {
                    ResultTextBox.Text = "无效的JWT格式";
                    return;
                }

                string header = DecodeBase64(parts[0]);
                string payload = DecodeBase64(parts[1]);

                var headerObj = JsonConvert.DeserializeObject(header);
                var payloadObj = JsonConvert.DeserializeObject(payload);

                string result = $"Header:\n{JsonConvert.SerializeObject(headerObj, Formatting.Indented)}\n\nPayload:\n{JsonConvert.SerializeObject(payloadObj, Formatting.Indented)}";
                
                ResultTextBox.Text = result;
            }
            catch (Exception ex)
            {
                ResultTextBox.Text = $"解码错误: {ex.Message}";
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

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            JwtTextBox.Text = "";
            ResultTextBox.Text = "";
        }
    }
}
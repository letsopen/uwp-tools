using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Web;

namespace UwpTools.Views
{
    public sealed partial class DevToolsPage : Page
    {
        public DevToolsPage()
        {
            this.InitializeComponent();
        }

        private void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(InputTextBox.Text))
            {
                ShowError("请输入要处理的文本");
                return;
            }

            if (ToolTypeComboBox.SelectedItem == null)
            {
                ShowError("请选择工具类型");
                return;
            }

            try
            {
                var toolType = (ToolTypeComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Base64Encode";
                string result = ProcessText(InputTextBox.Text, toolType);
                OutputTextBox.Text = result;
            }
            catch (Exception ex)
            {
                ShowError($"处理失败: {ex.Message}");
            }
        }

        private string ProcessText(string input, string toolType)
        {
            return toolType switch
            {
                "Base64Encode" => Convert.ToBase64String(Encoding.UTF8.GetBytes(input)),
                "Base64Decode" => Encoding.UTF8.GetString(Convert.FromBase64String(input)),
                "UrlEncode" => HttpUtility.UrlEncode(input),
                "UrlDecode" => HttpUtility.UrlDecode(input),
                "HtmlEncode" => HttpUtility.HtmlEncode(input),
                "HtmlDecode" => HttpUtility.HtmlDecode(input),
                "Md5Hash" => BitConverter.ToString(MD5.HashData(Encoding.UTF8.GetBytes(input))).Replace("-", "").ToLower(),
                "Sha1Hash" => BitConverter.ToString(SHA1.HashData(Encoding.UTF8.GetBytes(input))).Replace("-", "").ToLower(),
                "Sha256Hash" => BitConverter.ToString(SHA256.HashData(Encoding.UTF8.GetBytes(input))).Replace("-", "").ToLower(),
                _ => throw new ArgumentException("不支持的工具类型")
            };
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(OutputTextBox.Text))
            {
                var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
                dataPackage.SetText(OutputTextBox.Text);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
                ShowMessage("已复制到剪贴板");
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Text = string.Empty;
            OutputTextBox.Text = string.Empty;
            ToolTypeComboBox.SelectedItem = null;
        }

        private void ShowError(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "错误",
                Content = message,
                CloseButtonText = "确定",
                XamlRoot = this.XamlRoot
            };
            _ = dialog.ShowAsync();
        }

        private void ShowMessage(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "提示",
                Content = message,
                CloseButtonText = "确定",
                XamlRoot = this.XamlRoot
            };
            _ = dialog.ShowAsync();
        }
    }
} 
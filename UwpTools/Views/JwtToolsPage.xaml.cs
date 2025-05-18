using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.IdentityModel.Tokens.Jwt;
using Windows.ApplicationModel.DataTransfer;

namespace UwpTools.Views
{
    public sealed partial class JwtToolsPage : Page
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true
        };

        public JwtToolsPage()
        {
            this.InitializeComponent();
        }

        private void ValidateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(InputTextBox.Text))
                {
                    return;
                }

                var token = InputTextBox.Text.Trim();
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var header = JsonSerializer.Serialize(jwtToken.Header, _options);
                var payload = JsonSerializer.Serialize(jwtToken.Payload, _options);

                OutputTextBox.Text = $"Header:\n{header}\n\nPayload:\n{payload}";
            }
            catch (Exception ex)
            {
                OutputTextBox.Text = $"验证错误：{ex.Message}";
            }
        }

        private async void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(OutputTextBox.Text))
            {
                var dataPackage = new DataPackage();
                dataPackage.SetText(OutputTextBox.Text);
                Clipboard.SetContent(dataPackage);

                var dialog = new ContentDialog
                {
                    Title = "提示",
                    Content = "已复制到剪贴板",
                    CloseButtonText = "确定",
                    XamlRoot = this.XamlRoot
                };
                await dialog.ShowAsync();
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Text = string.Empty;
            OutputTextBox.Text = string.Empty;
        }
    }
} 
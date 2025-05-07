using System;
using System.Text;
using System.Text.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace UwpTools.Views
{
    public sealed partial class JwtToolsPage : Page
    {
        public JwtToolsPage()
        {
            this.InitializeComponent();
        }

        private void JwtTokenTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ParseJwtToken();
        }

        private void ParseJwtToken()
        {
            if (string.IsNullOrEmpty(JwtTokenTextBox.Text))
            {
                ResultTextBox.Text = string.Empty;
                return;
            }

            try
            {
                var token = JwtTokenTextBox.Text.Trim();
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);

                var result = new StringBuilder();
                result.AppendLine("Header:");
                result.AppendLine(JsonSerializer.Serialize(jsonToken.Header, new JsonSerializerOptions { WriteIndented = true }));
                result.AppendLine("\nPayload:");
                result.AppendLine(JsonSerializer.Serialize(jsonToken.Payload, new JsonSerializerOptions { WriteIndented = true }));

                if (!string.IsNullOrEmpty(SecretKeyTextBox.Text))
                {
                    try
                    {
                        var key = Encoding.UTF8.GetBytes(SecretKeyTextBox.Text);
                        var validationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(key),
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ClockSkew = TimeSpan.Zero
                        };

                        var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);
                        result.AppendLine("\n验证结果: 签名有效");
                    }
                    catch (Exception ex)
                    {
                        result.AppendLine($"\n验证结果: 签名无效 - {ex.Message}");
                    }
                }

                ResultTextBox.Text = result.ToString();
            }
            catch (Exception ex)
            {
                ResultTextBox.Text = $"解析JWT Token时出错：{ex.Message}";
            }
        }

        private async void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ResultTextBox.Text))
            {
                var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
                dataPackage.SetText(ResultTextBox.Text);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);

                var dialog = new ContentDialog
                {
                    Title = "成功",
                    Content = "解析结果已复制到剪贴板",
                    CloseButtonText = "确定"
                };
                await dialog.ShowAsync();
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            JwtTokenTextBox.Text = string.Empty;
            SecretKeyTextBox.Text = string.Empty;
            ResultTextBox.Text = string.Empty;
        }
    }
} 
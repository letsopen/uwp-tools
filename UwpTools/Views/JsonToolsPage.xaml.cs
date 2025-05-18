using System;
using System.Text.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;
using System.Text.RegularExpressions;

namespace UwpTools.Views
{
    public sealed partial class JsonToolsPage : Page
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true
        };

        public JsonToolsPage()
        {
            this.InitializeComponent();
        }

        private void FormatButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(InputTextBox.Text))
                {
                    return;
                }

                var obj = JsonSerializer.Deserialize<object>(InputTextBox.Text, _options);
                OutputTextBox.Text = JsonSerializer.Serialize(obj, _options);
            }
            catch (Exception ex)
            {
                OutputTextBox.Text = $"格式化错误：{ex.Message}";
            }
        }

        private void CompressButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var jsonObj = JsonSerializer.Deserialize<object>(InputTextBox.Text);
                var options = new JsonSerializerOptions { WriteIndented = false };
                OutputTextBox.Text = JsonSerializer.Serialize(jsonObj, options);
            }
            catch (JsonException ex)
            {
                OutputTextBox.Text = $"错误：{ex.Message}";
            }
        }

        private void CompressEscapeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var jsonObj = JsonSerializer.Deserialize<object>(InputTextBox.Text);
                var options = new JsonSerializerOptions { WriteIndented = false };
                var json = JsonSerializer.Serialize(jsonObj, options);
                OutputTextBox.Text = Regex.Escape(json);
            }
            catch (JsonException ex)
            {
                OutputTextBox.Text = $"错误：{ex.Message}";
            }
        }

        private void ValidateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(InputTextBox.Text))
                {
                    return;
                }

                var obj = JsonSerializer.Deserialize<object>(InputTextBox.Text, _options);
                OutputTextBox.Text = "JSON格式有效";
            }
            catch (Exception ex)
            {
                OutputTextBox.Text = $"JSON格式无效：{ex.Message}";
            }
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(InputTextBox.Text))
                {
                    return;
                }

                var obj = JsonSerializer.Deserialize<object>(InputTextBox.Text, _options);
                OutputTextBox.Text = JsonSerializer.Serialize(obj, _options);
            }
            catch (Exception ex)
            {
                OutputTextBox.Text = $"转换错误：{ex.Message}";
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
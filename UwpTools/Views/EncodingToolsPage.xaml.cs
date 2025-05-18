using System;
using System.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;

namespace UwpTools.Views
{
    public sealed partial class EncodingToolsPage : Page
    {
        public EncodingToolsPage()
        {
            this.InitializeComponent();
            SourceEncodingComboBox.SelectedIndex = 0;
            TargetEncodingComboBox.SelectedIndex = 0;
        }

        private void Encoding_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConvertText();
        }

        private void SourceText_TextChanged(object sender, TextChangedEventArgs e)
        {
            ConvertText();
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            ConvertText();
        }

        private void ConvertText()
        {
            if (string.IsNullOrEmpty(SourceTextBox.Text))
            {
                TargetTextBox.Text = string.Empty;
                return;
            }

            try
            {
                var sourceEncoding = GetEncoding(SourceEncodingComboBox);
                var targetEncoding = GetEncoding(TargetEncodingComboBox);

                if (sourceEncoding != null && targetEncoding != null)
                {
                    byte[] bytes = sourceEncoding.GetBytes(SourceTextBox.Text);
                    string result = targetEncoding.GetString(bytes);
                    TargetTextBox.Text = result;
                }
                else
                {
                    TargetTextBox.Text = "请选择有效的编码格式";
                }
            }
            catch (Exception ex)
            {
                TargetTextBox.Text = $"转换出错：{ex.Message}";
            }
        }

        private Encoding? GetEncoding(ComboBox comboBox)
        {
            if (comboBox.SelectedItem is ComboBoxItem selectedItem && 
                selectedItem.Tag is string encodingName)
            {
                return encodingName.ToLower() switch
                {
                    "utf-8" => Encoding.UTF8,
                    "utf-16" => Encoding.Unicode,
                    "utf-32" => Encoding.UTF32,
                    "gb2312" => Encoding.GetEncoding("gb2312"),
                    "gbk" => Encoding.GetEncoding("gbk"),
                    "gb18030" => Encoding.GetEncoding("gb18030"),
                    "ascii" => Encoding.ASCII,
                    "unicode" => Encoding.Unicode,
                    _ => null
                };
            }
            return null;
        }

        private async void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TargetTextBox.Text))
            {
                var dataPackage = new DataPackage();
                dataPackage.SetText(TargetTextBox.Text);
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
            SourceTextBox.Text = string.Empty;
            TargetTextBox.Text = string.Empty;
        }
    }
} 
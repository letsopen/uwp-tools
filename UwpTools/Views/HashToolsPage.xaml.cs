using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.IO;

namespace UwpTools.Views
{
    public sealed partial class HashToolsPage : Page
    {
        public HashToolsPage()
        {
            this.InitializeComponent();
            HashTypeComboBox.SelectedIndex = 0;
        }

        private void HashTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateHash();
        }

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateHash();
        }

        private void CalculateHash()
        {
            if (HashTypeComboBox.SelectedItem is ComboBoxItem selectedItem && 
                selectedItem.Tag is string hashType && 
                !string.IsNullOrEmpty(InputTextBox.Text))
            {
                try
                {
                    string result = hashType switch
                    {
                        "MD5" => CalculateMD5(InputTextBox.Text),
                        "SHA1" => CalculateSHA1(InputTextBox.Text),
                        "SHA256" => CalculateSHA256(InputTextBox.Text),
                        "SHA384" => CalculateSHA384(InputTextBox.Text),
                        "SHA512" => CalculateSHA512(InputTextBox.Text),
                        "CRC32" => CalculateCRC32(InputTextBox.Text),
                        _ => string.Empty
                    };

                    ResultTextBox.Text = result;
                }
                catch (Exception ex)
                {
                    ResultTextBox.Text = $"计算哈希值时出错：{ex.Message}";
                }
            }
            else
            {
                ResultTextBox.Text = string.Empty;
            }
        }

        private string CalculateMD5(string input)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private string CalculateSHA1(string input)
        {
            using var sha1 = SHA1.Create();
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = sha1.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private string CalculateSHA256(string input)
        {
            using var sha256 = SHA256.Create();
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = sha256.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private string CalculateSHA384(string input)
        {
            using var sha384 = SHA384.Create();
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = sha384.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private string CalculateSHA512(string input)
        {
            using var sha512 = SHA512.Create();
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = sha512.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private string CalculateCRC32(string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            uint crc = 0xFFFFFFFF;
            for (int i = 0; i < inputBytes.Length; i++)
            {
                crc ^= inputBytes[i];
                for (int j = 0; j < 8; j++)
                {
                    crc = (crc & 1) == 1 ? (crc >> 1) ^ 0xEDB88320 : crc >> 1;
                }
            }
            return (~crc).ToString("X8");
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
                    Content = "哈希值已复制到剪贴板",
                    CloseButtonText = "确定"
                };
                await dialog.ShowAsync();
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Text = string.Empty;
            ResultTextBox.Text = string.Empty;
        }
    }
} 
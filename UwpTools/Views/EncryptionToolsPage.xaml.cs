using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;

namespace UwpTools.Views
{
    public sealed partial class EncryptionToolsPage : Page
    {
        private string currentAlgorithm = "aes";

        public EncryptionToolsPage()
        {
            this.InitializeComponent();
            AlgorithmComboBox.SelectedIndex = 0;
        }

        private void AlgorithmComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AlgorithmComboBox.SelectedItem is ComboBoxItem selectedItem && 
                selectedItem.Tag is string algorithm)
            {
                currentAlgorithm = algorithm;
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            switch (currentAlgorithm)
            {
                case "base64":
                    KeyTextBox.Visibility = Visibility.Collapsed;
                    KeyPasswordBox.Visibility = Visibility.Collapsed;
                    break;
                case "rsa":
                    KeyTextBox.Visibility = Visibility.Visible;
                    KeyPasswordBox.Visibility = Visibility.Collapsed;
                    break;
                default:
                    KeyTextBox.Visibility = Visibility.Collapsed;
                    KeyPasswordBox.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string input = InputTextBox.Text;
                if (string.IsNullOrEmpty(input))
                {
                    return;
                }

                string result = currentAlgorithm switch
                {
                    "aes" => EncryptAes(input, KeyPasswordBox.Password),
                    "des" => EncryptDes(input, KeyPasswordBox.Password),
                    "rsa" => EncryptRsa(input, KeyTextBox.Text),
                    "base64" => Convert.ToBase64String(Encoding.UTF8.GetBytes(input)),
                    _ => throw new NotSupportedException("不支持的加密算法")
                };

                OutputTextBox.Text = result;
            }
            catch (Exception ex)
            {
                OutputTextBox.Text = $"加密错误：{ex.Message}";
            }
        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string input = InputTextBox.Text;
                if (string.IsNullOrEmpty(input))
                {
                    return;
                }

                string result = currentAlgorithm switch
                {
                    "aes" => DecryptAes(input, KeyPasswordBox.Password),
                    "des" => DecryptDes(input, KeyPasswordBox.Password),
                    "rsa" => DecryptRsa(input, KeyTextBox.Text),
                    "base64" => Encoding.UTF8.GetString(Convert.FromBase64String(input)),
                    _ => throw new NotSupportedException("不支持的加密算法")
                };

                OutputTextBox.Text = result;
            }
            catch (Exception ex)
            {
                OutputTextBox.Text = $"解密错误：{ex.Message}";
            }
        }

        private string EncryptAes(string plainText, string password)
        {
            using var aes = Aes.Create();
            var key = new Rfc2898DeriveBytes(password, new byte[8], 100000, HashAlgorithmName.SHA256);
            aes.Key = key.GetBytes(32);
            aes.IV = key.GetBytes(16);

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        private string DecryptAes(string cipherText, string password)
        {
            using var aes = Aes.Create();
            var key = new Rfc2898DeriveBytes(password, new byte[8], 100000, HashAlgorithmName.SHA256);
            aes.Key = key.GetBytes(32);
            aes.IV = key.GetBytes(16);

            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }

        private string EncryptDes(string plainText, string password)
        {
            using var des = DES.Create();
            var key = new Rfc2898DeriveBytes(password, new byte[8], 100000, HashAlgorithmName.SHA256);
            des.Key = key.GetBytes(8);
            des.IV = key.GetBytes(8);

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        private string DecryptDes(string cipherText, string password)
        {
            using var des = DES.Create();
            var key = new Rfc2898DeriveBytes(password, new byte[8], 100000, HashAlgorithmName.SHA256);
            des.Key = key.GetBytes(8);
            des.IV = key.GetBytes(8);

            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }

        private string EncryptRsa(string plainText, string publicKey)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicKey);

            byte[] data = Encoding.UTF8.GetBytes(plainText);
            byte[] encrypted = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
            return Convert.ToBase64String(encrypted);
        }

        private string DecryptRsa(string cipherText, string privateKey)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKey);

            byte[] data = Convert.FromBase64String(cipherText);
            byte[] decrypted = rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);
            return Encoding.UTF8.GetString(decrypted);
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
            KeyTextBox.Text = string.Empty;
            KeyPasswordBox.Password = string.Empty;
        }
    }
} 
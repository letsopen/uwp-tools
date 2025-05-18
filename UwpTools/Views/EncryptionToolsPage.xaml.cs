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
                    break;
                default:
                    KeyTextBox.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(InputTextBox.Text))
            {
                ShowError("请输入要加密的文本");
                return;
            }

            if (string.IsNullOrEmpty(KeyTextBox.Text))
            {
                ShowError("请输入密钥");
                return;
            }

            if (AlgorithmComboBox.SelectedItem == null)
            {
                ShowError("请选择加密算法");
                return;
            }

            try
            {
                var algorithm = (AlgorithmComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "AES";
                string result = EncryptText(InputTextBox.Text, KeyTextBox.Text, algorithm);
                OutputTextBox.Text = result;
            }
            catch (Exception ex)
            {
                ShowError($"加密失败: {ex.Message}");
            }
        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(InputTextBox.Text))
            {
                ShowError("请输入要解密的文本");
                return;
            }

            if (string.IsNullOrEmpty(KeyTextBox.Text))
            {
                ShowError("请输入密钥");
                return;
            }

            if (AlgorithmComboBox.SelectedItem == null)
            {
                ShowError("请选择加密算法");
                return;
            }

            try
            {
                var algorithm = (AlgorithmComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "AES";
                string result = DecryptText(InputTextBox.Text, KeyTextBox.Text, algorithm);
                OutputTextBox.Text = result;
            }
            catch (Exception ex)
            {
                ShowError($"解密失败: {ex.Message}");
            }
        }

        private string EncryptText(string text, string key, string algorithm)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] textBytes = Encoding.UTF8.GetBytes(text);

            return algorithm switch
            {
                "AES" => EncryptAes(textBytes, keyBytes),
                "DES" => EncryptDes(textBytes, keyBytes),
                "RSA" => EncryptRsa(textBytes),
                _ => throw new ArgumentException("不支持的加密算法")
            };
        }

        private string DecryptText(string text, string key, string algorithm)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] textBytes = Convert.FromBase64String(text);

            return algorithm switch
            {
                "AES" => DecryptAes(textBytes, keyBytes),
                "DES" => DecryptDes(textBytes, keyBytes),
                "RSA" => DecryptRsa(textBytes),
                _ => throw new ArgumentException("不支持的加密算法")
            };
        }

        private string EncryptAes(byte[] data, byte[] key)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            byte[] encrypted = encryptor.TransformFinalBlock(data, 0, data.Length);

            byte[] result = new byte[aes.IV.Length + encrypted.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(encrypted, 0, result, aes.IV.Length, encrypted.Length);

            return Convert.ToBase64String(result);
        }

        private string DecryptAes(byte[] data, byte[] key)
        {
            using var aes = Aes.Create();
            aes.Key = key;

            byte[] iv = new byte[aes.BlockSize / 8];
            byte[] encrypted = new byte[data.Length - iv.Length];

            Buffer.BlockCopy(data, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(data, iv.Length, encrypted, 0, encrypted.Length);

            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor();
            byte[] decrypted = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);

            return Encoding.UTF8.GetString(decrypted);
        }

        private string EncryptDes(byte[] data, byte[] key)
        {
            using var des = DES.Create();
            des.Key = key;
            des.GenerateIV();

            using var encryptor = des.CreateEncryptor();
            byte[] encrypted = encryptor.TransformFinalBlock(data, 0, data.Length);

            byte[] result = new byte[des.IV.Length + encrypted.Length];
            Buffer.BlockCopy(des.IV, 0, result, 0, des.IV.Length);
            Buffer.BlockCopy(encrypted, 0, result, des.IV.Length, encrypted.Length);

            return Convert.ToBase64String(result);
        }

        private string DecryptDes(byte[] data, byte[] key)
        {
            using var des = DES.Create();
            des.Key = key;

            byte[] iv = new byte[des.BlockSize / 8];
            byte[] encrypted = new byte[data.Length - iv.Length];

            Buffer.BlockCopy(data, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(data, iv.Length, encrypted, 0, encrypted.Length);

            des.IV = iv;
            using var decryptor = des.CreateDecryptor();
            byte[] decrypted = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);

            return Encoding.UTF8.GetString(decrypted);
        }

        private string EncryptRsa(byte[] data)
        {
            using var rsa = RSA.Create();
            byte[] encrypted = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
            return Convert.ToBase64String(encrypted);
        }

        private string DecryptRsa(byte[] data)
        {
            using var rsa = RSA.Create();
            byte[] decrypted = rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);
            return Encoding.UTF8.GetString(decrypted);
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
            KeyTextBox.Text = string.Empty;
            AlgorithmComboBox.SelectedItem = null;
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
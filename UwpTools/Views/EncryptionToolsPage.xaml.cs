using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了"用户控件"项模板

namespace UwpTools.Views
{
    public sealed partial class EncryptionToolsPage : Page
    {
        public EncryptionToolsPage()
        {
            this.InitializeComponent();
        }

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            string input = InputTextBox.Text;
            string key = KeyTextBox.Text;
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(key)) return;

            string algorithm = (AlgorithmComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString();

            string result = algorithm switch
            {
                "AES" => EncryptAes(input, key),
                "DES" => EncryptDes(input, key),
                "RSA" => EncryptRsa(input),
                _ => EncryptAes(input, key)
            };

            OutputTextBox.Text = result;
        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            string input = InputTextBox.Text;
            string key = KeyTextBox.Text;
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(key)) return;

            string algorithm = (AlgorithmComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString();

            string result = algorithm switch
            {
                "AES" => DecryptAes(input, key),
                "DES" => DecryptDes(input, key),
                "RSA" => DecryptRsa(input),
                _ => DecryptAes(input, key)
            };

            OutputTextBox.Text = result;
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(OutputTextBox.Text))
            {
                var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
                dataPackage.SetText(OutputTextBox.Text);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            }
        }

        private string EncryptAes(string plainText, string key)
        {
            try
            {
                byte[] iv = new byte[16];
                byte[] array;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                    aes.IV = iv;

                    ICryptoTransform encryptor = aes.CreateEncryptor();
                    byte[] utf8Bytes = Encoding.UTF8.GetBytes(plainText);
                    array = encryptor.TransformFinalBlock(utf8Bytes, 0, utf8Bytes.Length);
                }

                return Convert.ToBase64String(array);
            }
            catch
            {
                return "加密失败";
            }
        }

        private string DecryptAes(string cipherText, string key)
        {
            try
            {
                byte[] iv = new byte[16];
                byte[] buffer = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                    aes.IV = iv;

                    ICryptoTransform decryptor = aes.CreateDecryptor();
                    byte[] result = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);

                    return Encoding.UTF8.GetString(result);
                }
            }
            catch
            {
                return "解密失败";
            }
        }

        private string EncryptDes(string plainText, string key)
        {
            try
            {
                byte[] iv = new byte[8];
                byte[] array;

                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    des.Key = ASCIIEncoding.ASCII.GetBytes(key.PadRight(8).Substring(0, 8));
                    des.IV = iv;

                    ICryptoTransform encryptor = des.CreateEncryptor();
                    byte[] utf8Bytes = Encoding.UTF8.GetBytes(plainText);
                    array = encryptor.TransformFinalBlock(utf8Bytes, 0, utf8Bytes.Length);
                }

                return Convert.ToBase64String(array);
            }
            catch
            {
                return "加密失败";
            }
        }

        private string DecryptDes(string cipherText, string key)
        {
            try
            {
                byte[] iv = new byte[8];
                byte[] buffer = Convert.FromBase64String(cipherText);

                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    des.Key = ASCIIEncoding.ASCII.GetBytes(key.PadRight(8).Substring(0, 8));
                    des.IV = iv;

                    ICryptoTransform decryptor = des.CreateDecryptor();
                    byte[] result = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);

                    return Encoding.UTF8.GetString(result);
                }
            }
            catch
            {
                return "解密失败";
            }
        }

        private string EncryptRsa(string plainText)
        {
            // 简单模拟，实际的RSA加密更复杂
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }

        private string DecryptRsa(string cipherText)
        {
            // 简单模拟，实际的RSA解密更复杂
            return Encoding.UTF8.GetString(Convert.FromBase64String(cipherText));
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Text = "";
            OutputTextBox.Text = "";
            KeyTextBox.Text = "";
        }
    }
} 
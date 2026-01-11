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
    public sealed partial class HashToolsPage : Page
    {
        public HashToolsPage()
        {
            this.InitializeComponent();
        }

        private void ComputeHash_Click(object sender, RoutedEventArgs e)
        {
            string input = InputTextBox.Text;
            if (string.IsNullOrEmpty(input)) return;

            string algorithm = AlgorithmComboBox.SelectedItem?.ToString();

            string result = ComputeHash(input, algorithm ?? "MD5");

            ResultTextBox.Text = result;
        }

        private string ComputeHash(string input, string algorithm)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes;

            switch (algorithm)
            {
                case "MD5":
                    using (var md5 = MD5.Create())
                    {
                        hashBytes = md5.ComputeHash(inputBytes);
                    }
                    break;
                case "SHA1":
                    using (var sha1 = SHA1.Create())
                    {
                        hashBytes = sha1.ComputeHash(inputBytes);
                    }
                    break;
                case "SHA256":
                    using (var sha256 = SHA256.Create())
                    {
                        hashBytes = sha256.ComputeHash(inputBytes);
                    }
                    break;
                case "SHA384":
                    using (var sha384 = SHA384.Create())
                    {
                        hashBytes = sha384.ComputeHash(inputBytes);
                    }
                    break;
                case "SHA512":
                    using (var sha512 = SHA512.Create())
                    {
                        hashBytes = sha512.ComputeHash(inputBytes);
                    }
                    break;
                default:
                    using (var md5 = MD5.Create())
                    {
                        hashBytes = md5.ComputeHash(inputBytes);
                    }
                    break;
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Text = "";
            ResultTextBox.Text = "";
        }
    }
} 
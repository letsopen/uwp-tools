using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public sealed partial class JsonToolsPage : Page
    {
        public JsonToolsPage()
        {
            this.InitializeComponent();
        }

        private void FormatButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string input = InputTextBox.Text;
                if (string.IsNullOrWhiteSpace(input))
                {
                    OutputTextBox.Text = "输入不能为空";
                    return;
                }

                JToken parsedJson = JToken.Parse(input);
                string formatted = parsedJson.ToString(Formatting.Indented);
                OutputTextBox.Text = formatted;
            }
            catch (Exception ex)
            {
                OutputTextBox.Text = $"解析错误: {ex.Message}";
            }
        }

        private void ValidateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string input = InputTextBox.Text;
                JToken.Parse(input);
                OutputTextBox.Text = "✓ 有效的JSON";
            }
            catch (Exception ex)
            {
                OutputTextBox.Text = $"✗ 无效的JSON: {ex.Message}";
            }
        }

        private void CompressButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string input = InputTextBox.Text;
                JToken parsedJson = JToken.Parse(input);
                string compressed = parsedJson.ToString(Formatting.None);
                OutputTextBox.Text = compressed;
            }
            catch (Exception ex)
            {
                OutputTextBox.Text = $"压缩错误: {ex.Message}";
            }
        }

        private void CompressEscapeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string input = InputTextBox.Text;
                JToken parsedJson = JToken.Parse(input);
                string compressed = parsedJson.ToString(Formatting.None);
                string escaped = System.Uri.EscapeDataString(compressed);
                OutputTextBox.Text = escaped;
            }
            catch (Exception ex)
            {
                OutputTextBox.Text = $"转义错误: {ex.Message}";
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Text = "";
            OutputTextBox.Text = "";
        }
    }
} 
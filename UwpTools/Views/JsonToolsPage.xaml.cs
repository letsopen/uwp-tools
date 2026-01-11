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

        private void FormatJson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string input = JsonInputTextBox.Text;
                if (string.IsNullOrWhiteSpace(input))
                {
                    ResultTextBox.Text = "输入不能为空";
                    return;
                }

                JToken parsedJson = JToken.Parse(input);
                string formatted = parsedJson.ToString(Formatting.Indented);
                ResultTextBox.Text = formatted;
            }
            catch (Exception ex)
            {
                ResultTextBox.Text = $"解析错误: {ex.Message}";
            }
        }

        private void ValidateJson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string input = JsonInputTextBox.Text;
                JToken.Parse(input);
                ValidationResultTextBlock.Text = "✓ 有效的JSON";
                ValidationResultTextBlock.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Green);
            }
            catch (Exception ex)
            {
                ValidationResultTextBlock.Text = $"✗ 无效的JSON: {ex.Message}";
                ValidationResultTextBlock.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Red);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            JsonInputTextBox.Text = "";
            ResultTextBox.Text = "";
            ValidationResultTextBlock.Text = "";
        }
    }
} 
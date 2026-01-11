using System;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UwpTools.Views
{
    public sealed partial class ChineseNumberPage : Page
    {
        private readonly string[] chineseUpper = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
        private readonly string[] chineseLower = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
        private readonly string[] units = { "", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿", "拾", "佰", "仟", "万" };

        public ChineseNumberPage()
        {
            this.InitializeComponent();
            ConversionTypeComboBox.SelectedIndex = 0;
        }

        private void NumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ConvertNumber();
        }

        private void ConversionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConvertNumber();
        }

        private void ConvertNumber()
        {
            if (string.IsNullOrWhiteSpace(NumberTextBox.Text))
            {
                ResultTextBox.Text = string.Empty;
                return;
            }

            try
            {
                if (decimal.TryParse(NumberTextBox.Text, out decimal number))
                {
                    string result = ConvertToChinese(number);
                    ResultTextBox.Text = result;
                }
                else
                {
                    ResultTextBox.Text = "请输入有效的数字";
                }
            }
            catch (Exception ex)
            {
                ResultTextBox.Text = $"转换出错：{ex.Message}";
            }
        }

        private string ConvertToChinese(decimal number)
        {
            bool isUpper = ConversionTypeComboBox.SelectedIndex == 0;
            string[] chineseNumbers = isUpper ? chineseUpper : chineseLower;

            StringBuilder result = new StringBuilder();
            string numStr = Math.Abs(number).ToString("0.00");
            string[] parts = numStr.Split('.');

            // 处理整数部分
            string integerPart = parts[0];
            if (integerPart == "0")
            {
                result.Append(chineseNumbers[0]);
            }
            else
            {
                for (int i = 0; i < integerPart.Length; i++)
                {
                    int digit = integerPart[i] - '0';
                    if (digit != 0)
                    {
                        result.Append(chineseNumbers[digit]);
                        if (i < integerPart.Length - 1)
                        {
                            result.Append(units[integerPart.Length - 1 - i]);
                        }
                    }
                    else
                    {
                        if (i < integerPart.Length - 1 && integerPart[i + 1] != '0')
                        {
                            result.Append(chineseNumbers[0]);
                        }
                    }
                }
            }

            // 处理小数部分
            if (parts.Length > 1)
            {
                result.Append("点");
                string decimalPart = parts[1];
                foreach (char c in decimalPart)
                {
                    result.Append(chineseNumbers[c - '0']);
                }
            }

            // 添加负号
            if (number < 0)
            {
                result.Insert(0, "负");
            }

            return result.ToString();
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
                    Content = "结果已复制到剪贴板",
                    CloseButtonText = "确定"
                };
                await dialog.ShowAsync();
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            NumberTextBox.Text = string.Empty;
            ResultTextBox.Text = string.Empty;
        }
    }
} 
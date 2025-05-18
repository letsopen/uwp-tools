using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;

namespace UwpTools.Views
{
    public sealed partial class RegexToolsPage : Page
    {
        public RegexToolsPage()
        {
            this.InitializeComponent();
        }

        private void MatchButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PatternTextBox.Text))
            {
                ShowError("请输入正则表达式");
                return;
            }

            if (string.IsNullOrEmpty(InputTextBox.Text))
            {
                ShowError("请输入要匹配的文本");
                return;
            }

            try
            {
                var options = GetRegexOptions();
                var regex = new Regex(PatternTextBox.Text, options);
                var matches = regex.Matches(InputTextBox.Text);

                if (matches.Count == 0)
                {
                    ResultTextBox.Text = "未找到匹配项";
                    return;
                }

                var result = new System.Text.StringBuilder();
                for (int i = 0; i < matches.Count; i++)
                {
                    var match = matches[i];
                    result.AppendLine($"匹配 {i + 1}:");
                    result.AppendLine($"位置: {match.Index}");
                    result.AppendLine($"长度: {match.Length}");
                    result.AppendLine($"值: {match.Value}");
                    if (match.Groups.Count > 1)
                    {
                        result.AppendLine("捕获组:");
                        for (int j = 1; j < match.Groups.Count; j++)
                        {
                            result.AppendLine($"  {j}: {match.Groups[j].Value}");
                        }
                    }
                    result.AppendLine();
                }

                ResultTextBox.Text = result.ToString();
            }
            catch (Exception ex)
            {
                ShowError($"匹配失败: {ex.Message}");
            }
        }

        private void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PatternTextBox.Text))
            {
                ShowError("请输入正则表达式");
                return;
            }

            if (string.IsNullOrEmpty(InputTextBox.Text))
            {
                ShowError("请输入要替换的文本");
                return;
            }

            try
            {
                var options = GetRegexOptions();
                var regex = new Regex(PatternTextBox.Text, options);
                var result = regex.Replace(InputTextBox.Text, "$&");
                ResultTextBox.Text = result;
            }
            catch (Exception ex)
            {
                ShowError($"替换失败: {ex.Message}");
            }
        }

        private RegexOptions GetRegexOptions()
        {
            var options = RegexOptions.None;
            if (OptionsComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                options = selectedItem.Tag?.ToString() switch
                {
                    "IgnoreCase" => RegexOptions.IgnoreCase,
                    "Multiline" => RegexOptions.Multiline,
                    "Singleline" => RegexOptions.Singleline,
                    _ => RegexOptions.None
                };
            }
            return options;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            PatternTextBox.Text = string.Empty;
            InputTextBox.Text = string.Empty;
            ResultTextBox.Text = string.Empty;
            OptionsComboBox.SelectedItem = null;
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
    }

    public class RegexMatchResult
    {
        public required string Index { get; set; }
        public required string Value { get; set; }
    }
} 
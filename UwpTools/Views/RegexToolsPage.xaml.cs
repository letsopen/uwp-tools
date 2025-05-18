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

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            TestRegex();
        }

        private void TestRegex()
        {
            if (string.IsNullOrEmpty(PatternTextBox.Text) || string.IsNullOrEmpty(InputTextBox.Text))
            {
                return;
            }

            try
            {
                var regex = new Regex(PatternTextBox.Text);
                var matches = regex.Matches(InputTextBox.Text);
                var results = new List<RegexMatchResult>();

                for (int i = 0; i < matches.Count; i++)
                {
                    results.Add(new RegexMatchResult
                    {
                        Index = $"匹配 {i + 1}:",
                        Value = matches[i].Value
                    });
                }

                MatchesListView.ItemsSource = results;
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "错误",
                    Content = $"正则表达式错误：{ex.Message}",
                    CloseButtonText = "确定",
                    XamlRoot = this.XamlRoot
                };
                _ = dialog.ShowAsync();
            }
        }

        private async void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (MatchesListView.ItemsSource is List<RegexMatchResult> results && results.Count > 0)
            {
                var text = string.Join("\n", results.ConvertAll(r => $"{r.Index} {r.Value}"));
                var dataPackage = new DataPackage();
                dataPackage.SetText(text);
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
            PatternTextBox.Text = string.Empty;
            InputTextBox.Text = string.Empty;
            MatchesListView.ItemsSource = null;
        }
    }

    public class RegexMatchResult
    {
        public required string Index { get; set; }
        public required string Value { get; set; }
    }
} 
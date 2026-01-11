using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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
    public sealed partial class RegexToolsPage : Page
    {
        public RegexToolsPage()
        {
            this.InitializeComponent();
        }

        private void TestRegex_Click(object sender, RoutedEventArgs e)
        {
            string pattern = PatternTextBox.Text;
            string input = InputTextBox.Text;

            try
            {
                RegexOptions options = RegexOptions.None;
                if (IgnoreCaseCheckBox.IsChecked == true)
                    options |= RegexOptions.IgnoreCase;
                if (MultilineCheckBox.IsChecked == true)
                    options |= RegexOptions.Multiline;
                if (SinglelineCheckBox.IsChecked == true)
                    options |= RegexOptions.Singleline;

                Regex regex = new Regex(pattern, options);
                MatchCollection matches = regex.Matches(input);

                if (matches.Count > 0)
                {
                    List<string> matchResults = new List<string>();
                    foreach (Match match in matches)
                    {
                        matchResults.Add($"Match: '{match.Value}' at position {match.Index}");
                        if (match.Groups.Count > 1)
                        {
                            for (int i = 1; i < match.Groups.Count; i++)
                            {
                                matchResults.Add($"  Group {i}: '{match.Groups[i].Value}'");
                            }
                        }
                    }
                    ResultTextBox.Text = string.Join("\n", matchResults);
                }
                else
                {
                    ResultTextBox.Text = "无匹配结果";
                }
            }
            catch (ArgumentException ex)
            {
                ResultTextBox.Text = $"正则表达式错误: {ex.Message}";
            }
        }

        private void ReplaceText_Click(object sender, RoutedEventArgs e)
        {
            string pattern = PatternTextBox.Text;
            string input = InputTextBox.Text;
            string replacement = ReplacementTextBox.Text;

            try
            {
                RegexOptions options = RegexOptions.None;
                if (IgnoreCaseCheckBox.IsChecked == true)
                    options |= RegexOptions.IgnoreCase;
                if (MultilineCheckBox.IsChecked == true)
                    options |= RegexOptions.Multiline;
                if (SinglelineCheckBox.IsChecked == true)
                    options |= RegexOptions.Singleline;

                Regex regex = new Regex(pattern, options);
                string result = regex.Replace(input, replacement);

                ResultTextBox.Text = result;
            }
            catch (ArgumentException ex)
            {
                ResultTextBox.Text = $"正则表达式错误: {ex.Message}";
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            PatternTextBox.Text = "";
            InputTextBox.Text = "";
            ReplacementTextBox.Text = "";
            ResultTextBox.Text = "";
        }
    }
}

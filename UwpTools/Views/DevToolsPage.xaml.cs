using System;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;
using Markdig;

namespace UwpTools.Views
{
    public sealed partial class DevToolsPage : Page
    {
        private string currentToolType = "sql";

        public DevToolsPage()
        {
            this.InitializeComponent();
            ToolTypeComboBox.SelectedIndex = 0;
        }

        private void ToolTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ToolTypeComboBox.SelectedItem is ComboBoxItem selectedItem && 
                selectedItem.Tag is string toolType)
            {
                currentToolType = toolType;
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            switch (currentToolType)
            {
                case "markdown":
                    PreviewWebView.Visibility = Visibility.Visible;
                    OutputTextBox.Visibility = Visibility.Collapsed;
                    FormatButton.Content = "预览";
                    break;
                default:
                    PreviewWebView.Visibility = Visibility.Collapsed;
                    OutputTextBox.Visibility = Visibility.Visible;
                    FormatButton.Content = "格式化";
                    break;
            }
        }

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (currentToolType == "markdown")
            {
                UpdateMarkdownPreview();
            }
        }

        private void FormatButton_Click(object sender, RoutedEventArgs e)
        {
            switch (currentToolType)
            {
                case "sql":
                    FormatSql();
                    break;
                case "xml":
                    FormatXml();
                    break;
                case "markdown":
                    UpdateMarkdownPreview();
                    break;
            }
        }

        private void FormatSql()
        {
            try
            {
                string sql = InputTextBox.Text;
                if (string.IsNullOrWhiteSpace(sql))
                {
                    return;
                }

                var formatted = new StringBuilder();
                int indent = 0;
                bool newLine = true;

                foreach (char c in sql)
                {
                    if (newLine)
                    {
                        formatted.Append(new string(' ', indent * 4));
                        newLine = false;
                    }

                    switch (c)
                    {
                        case '(':
                            formatted.Append(c);
                            indent++;
                            formatted.AppendLine();
                            newLine = true;
                            break;
                        case ')':
                            indent--;
                            formatted.AppendLine();
                            formatted.Append(new string(' ', indent * 4));
                            formatted.Append(c);
                            break;
                        case ',':
                            formatted.Append(c);
                            formatted.AppendLine();
                            newLine = true;
                            break;
                        case ' ':
                            if (!newLine)
                            {
                                formatted.Append(c);
                            }
                            break;
                        default:
                            formatted.Append(c);
                            break;
                    }
                }

                OutputTextBox.Text = formatted.ToString();
            }
            catch (Exception ex)
            {
                OutputTextBox.Text = $"格式化错误：{ex.Message}";
            }
        }

        private void FormatXml()
        {
            try
            {
                string xml = InputTextBox.Text;
                if (string.IsNullOrWhiteSpace(xml))
                {
                    return;
                }

                var doc = XDocument.Parse(xml);
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "    ",
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Replace
                };

                var builder = new StringBuilder();
                using (var writer = XmlWriter.Create(builder, settings))
                {
                    doc.Save(writer);
                }

                OutputTextBox.Text = builder.ToString();
            }
            catch (Exception ex)
            {
                OutputTextBox.Text = $"格式化错误：{ex.Message}";
            }
        }

        private async void UpdateMarkdownPreview()
        {
            try
            {
                string markdown = InputTextBox.Text;
                if (string.IsNullOrWhiteSpace(markdown))
                {
                    return;
                }

                string html = Markdown.ToHtml(markdown);
                string fullHtml = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='utf-8'>
                        <style>
                            body {{ font-family: 'Segoe UI', sans-serif; padding: 20px; }}
                            pre {{ background-color: #f5f5f5; padding: 10px; border-radius: 5px; }}
                            code {{ background-color: #f5f5f5; padding: 2px 4px; border-radius: 3px; }}
                            table {{ border-collapse: collapse; width: 100%; }}
                            th, td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
                            th {{ background-color: #f5f5f5; }}
                        </style>
                    </head>
                    <body>
                        {html}
                    </body>
                    </html>";

                await PreviewWebView.EnsureCoreWebView2Async();
                PreviewWebView.CoreWebView2.NavigateToString(fullHtml);
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "错误",
                    Content = $"预览错误：{ex.Message}",
                    CloseButtonText = "确定",
                    XamlRoot = this.XamlRoot
                };
                await dialog.ShowAsync();
            }
        }

        private async void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            string textToCopy = currentToolType == "markdown" ? InputTextBox.Text : OutputTextBox.Text;
            if (!string.IsNullOrEmpty(textToCopy))
            {
                var dataPackage = new DataPackage();
                dataPackage.SetText(textToCopy);
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
            if (currentToolType == "markdown")
            {
                PreviewWebView.CoreWebView2?.NavigateToString(string.Empty);
            }
        }
    }
} 
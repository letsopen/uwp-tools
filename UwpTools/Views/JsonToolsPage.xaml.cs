using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text.Json;
using System.Text.RegularExpressions;

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
                var jsonObj = JsonSerializer.Deserialize<object>(InputTextBox.Text);
                var options = new JsonSerializerOptions { WriteIndented = true };
                OutputTextBox.Text = JsonSerializer.Serialize(jsonObj, options);
            }
            catch (JsonException ex)
            {
                OutputTextBox.Text = $"错误：{ex.Message}";
            }
        }

        private void CompressButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var jsonObj = JsonSerializer.Deserialize<object>(InputTextBox.Text);
                var options = new JsonSerializerOptions { WriteIndented = false };
                OutputTextBox.Text = JsonSerializer.Serialize(jsonObj, options);
            }
            catch (JsonException ex)
            {
                OutputTextBox.Text = $"错误：{ex.Message}";
            }
        }

        private void CompressEscapeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var jsonObj = JsonSerializer.Deserialize<object>(InputTextBox.Text);
                var options = new JsonSerializerOptions { WriteIndented = false };
                var json = JsonSerializer.Serialize(jsonObj, options);
                OutputTextBox.Text = Regex.Escape(json);
            }
            catch (JsonException ex)
            {
                OutputTextBox.Text = $"错误：{ex.Message}";
            }
        }

        private void ValidateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var jsonObj = JsonSerializer.Deserialize<object>(InputTextBox.Text);
                OutputTextBox.Text = "JSON格式正确！";
            }
            catch (JsonException ex)
            {
                OutputTextBox.Text = $"JSON格式错误：{ex.Message}";
            }
        }
    }
} 
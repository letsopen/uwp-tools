using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;
using System.Text.RegularExpressions;

namespace UwpTools.Views
{
    public sealed partial class ColorToolsPage : Page
    {
        public ColorToolsPage()
        {
            this.InitializeComponent();
        }

        private void ColorInputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(ColorInputTextBox.Text))
            {
                ClearResults();
                return;
            }

            try
            {
                var format = (ColorFormatComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString();
                if (string.IsNullOrEmpty(format))
                {
                    return;
                }

                switch (format)
                {
                    case "HEX":
                        ProcessHexColor(ColorInputTextBox.Text);
                        break;
                    case "RGB":
                        ProcessRgbColor(ColorInputTextBox.Text);
                        break;
                    case "HSL":
                        ProcessHslColor(ColorInputTextBox.Text);
                        break;
                }
            }
            catch (Exception)
            {
                ClearResults();
            }
        }

        private void ProcessHexColor(string hexColor)
        {
            hexColor = hexColor.TrimStart('#');
            if (hexColor.Length != 6)
            {
                throw new FormatException("HEX颜色格式无效");
            }

            int r = Convert.ToInt32(hexColor.Substring(0, 2), 16);
            int g = Convert.ToInt32(hexColor.Substring(2, 2), 16);
            int b = Convert.ToInt32(hexColor.Substring(4, 2), 16);

            UpdateColorPreview(r, g, b);
            UpdateResults(hexColor, $"{r},{g},{b}", RgbToHsl(r, g, b));
        }

        private void ProcessRgbColor(string rgbColor)
        {
            var match = Regex.Match(rgbColor, @"(\d+)\s*,\s*(\d+)\s*,\s*(\d+)");
            if (!match.Success)
            {
                throw new FormatException("RGB颜色格式无效");
            }

            int r = int.Parse(match.Groups[1].Value);
            int g = int.Parse(match.Groups[2].Value);
            int b = int.Parse(match.Groups[3].Value);

            if (r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255)
            {
                throw new FormatException("RGB值必须在0-255之间");
            }

            UpdateColorPreview(r, g, b);
            UpdateResults(
                $"{r:X2}{g:X2}{b:X2}",
                $"{r},{g},{b}",
                RgbToHsl(r, g, b)
            );
        }

        private void ProcessHslColor(string hslColor)
        {
            var match = Regex.Match(hslColor, @"(\d+)\s*,\s*(\d+)%\s*,\s*(\d+)%");
            if (!match.Success)
            {
                throw new FormatException("HSL颜色格式无效");
            }

            int h = int.Parse(match.Groups[1].Value);
            int s = int.Parse(match.Groups[2].Value);
            int l = int.Parse(match.Groups[3].Value);

            if (h < 0 || h > 360 || s < 0 || s > 100 || l < 0 || l > 100)
            {
                throw new FormatException("HSL值无效");
            }

            var (r, g, b) = HslToRgb(h, s, l);
            UpdateColorPreview(r, g, b);
            UpdateResults(
                $"{r:X2}{g:X2}{b:X2}",
                $"{r},{g},{b}",
                $"{h},{s}%,{l}%"
            );
        }

        private void UpdateColorPreview(int r, int g, int b)
        {
            ColorPreviewBorder.Background = new Windows.UI.Xaml.Media.SolidColorBrush(
                Color.FromArgb(255, (byte)r, (byte)g, (byte)b)
            );
        }

        private void UpdateResults(string hex, string rgb, string hsl)
        {
            HexResultTextBox.Text = hex;
            RgbResultTextBox.Text = rgb;
            HslResultTextBox.Text = hsl;
        }

        private void ClearResults()
        {
            HexResultTextBox.Text = string.Empty;
            RgbResultTextBox.Text = string.Empty;
            HslResultTextBox.Text = string.Empty;
            ColorPreviewBorder.Background = null;
        }

        private string RgbToHsl(int r, int g, int b)
        {
            float rf = r / 255f;
            float gf = g / 255f;
            float bf = b / 255f;

            float max = Math.Max(rf, Math.Max(gf, bf));
            float min = Math.Min(rf, Math.Min(gf, bf));
            float delta = max - min;

            float h = 0;
            float s = 0;
            float l = (max + min) / 2;

            if (delta != 0)
            {
                s = l > 0.5f ? delta / (2f - max - min) : delta / (max + min);

                if (max == rf)
                {
                    h = (gf - bf) / delta + (gf < bf ? 6 : 0);
                }
                else if (max == gf)
                {
                    h = (bf - rf) / delta + 2;
                }
                else
                {
                    h = (rf - gf) / delta + 4;
                }

                h *= 60;
            }

            return $"{Math.Round(h)},{Math.Round(s * 100)}%,{Math.Round(l * 100)}%";
        }

        private (int r, int g, int b) HslToRgb(int h, int s, int l)
        {
            float hf = h / 360f;
            float sf = s / 100f;
            float lf = l / 100f;

            float q = lf < 0.5f ? lf * (1 + sf) : lf + sf - lf * sf;
            float p = 2 * lf - q;

            float r = HueToRgb(p, q, hf + 1/3f);
            float g = HueToRgb(p, q, hf);
            float b = HueToRgb(p, q, hf - 1/3f);

            return (
                (int)Math.Round(r * 255),
                (int)Math.Round(g * 255),
                (int)Math.Round(b * 255)
            );
        }

        private float HueToRgb(float p, float q, float t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1/6f) return p + (q - p) * 6 * t;
            if (t < 1/2f) return q;
            if (t < 2/3f) return p + (q - p) * (2/3f - t) * 6;
            return p;
        }

        private async void CopyHexButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(HexResultTextBox.Text))
            {
                var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
                dataPackage.SetText(HexResultTextBox.Text);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);

                var dialog = new ContentDialog
                {
                    Title = "提示",
                    Content = "已复制到剪贴板",
                    CloseButtonText = "确定",

                };
                await dialog.ShowAsync();
            }
        }

        private async void CopyRgbButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(RgbResultTextBox.Text))
            {
                var text = $"RGB({RgbResultTextBox.Text})";
                var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
                dataPackage.SetText(text);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);

                var dialog = new ContentDialog
                {
                    Title = "提示",
                    Content = "已复制到剪贴板",
                    CloseButtonText = "确定",

                };
                await dialog.ShowAsync();
            }
        }

        private async void CopyHslButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(HslResultTextBox.Text))
            {
                var text = $"HSL({HslResultTextBox.Text})";
                var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
                dataPackage.SetText(text);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);

                var dialog = new ContentDialog
                {
                    Title = "提示",
                    Content = "已复制到剪贴板",
                    CloseButtonText = "确定",

                };
                await dialog.ShowAsync();
            }
        }
    }
} 
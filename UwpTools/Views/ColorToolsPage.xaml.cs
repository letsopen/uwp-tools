using System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;

namespace UwpTools.Views
{
    public sealed partial class ColorToolsPage : Page
    {
        private bool isUpdating = false;

        public ColorToolsPage()
        {
            this.InitializeComponent();
        }

        private void HexTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdating) return;
            isUpdating = true;

            try
            {
                string hex = HexTextBox.Text.TrimStart('#');
                if (hex.Length == 6)
                {
                    byte r = Convert.ToByte(hex.Substring(0, 2), 16);
                    byte g = Convert.ToByte(hex.Substring(2, 2), 16);
                    byte b = Convert.ToByte(hex.Substring(4, 2), 16);

                    RedTextBox.Text = r.ToString();
                    GreenTextBox.Text = g.ToString();
                    BlueTextBox.Text = b.ToString();

                    UpdateHslFromRgb(r, g, b);
                    UpdateColorPreview(r, g, b);
                }
            }
            catch { }

            isUpdating = false;
        }

        private void RgbTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdating) return;
            isUpdating = true;

            try
            {
                if (byte.TryParse(RedTextBox.Text, out byte r) &&
                    byte.TryParse(GreenTextBox.Text, out byte g) &&
                    byte.TryParse(BlueTextBox.Text, out byte b))
                {
                    HexTextBox.Text = $"#{r:X2}{g:X2}{b:X2}";
                    UpdateHslFromRgb(r, g, b);
                    UpdateColorPreview(r, g, b);
                }
            }
            catch { }

            isUpdating = false;
        }

        private void HslTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdating) return;
            isUpdating = true;

            try
            {
                if (double.TryParse(HueTextBox.Text, out double h) &&
                    double.TryParse(SaturationTextBox.Text, out double s) &&
                    double.TryParse(LightnessTextBox.Text, out double l))
                {
                    var (r, g, b) = HslToRgb(h, s, l);
                    RedTextBox.Text = r.ToString();
                    GreenTextBox.Text = g.ToString();
                    BlueTextBox.Text = b.ToString();
                    HexTextBox.Text = $"#{r:X2}{g:X2}{b:X2}";
                    UpdateColorPreview(r, g, b);
                }
            }
            catch { }

            isUpdating = false;
        }

        private void UpdateHslFromRgb(byte r, byte g, byte b)
        {
            var (h, s, l) = RgbToHsl(r, g, b);
            HueTextBox.Text = Math.Round(h, 1).ToString();
            SaturationTextBox.Text = Math.Round(s, 1).ToString();
            LightnessTextBox.Text = Math.Round(l, 1).ToString();
        }

        private void UpdateColorPreview(byte r, byte g, byte b)
        {
            ColorPreview.Background = new SolidColorBrush(Color.FromArgb(255, r, g, b));
        }

        private (double h, double s, double l) RgbToHsl(byte r, byte g, byte b)
        {
            double rf = r / 255.0;
            double gf = g / 255.0;
            double bf = b / 255.0;

            double max = Math.Max(Math.Max(rf, gf), bf);
            double min = Math.Min(Math.Min(rf, gf), bf);
            double delta = max - min;

            double h = 0;
            double s = 0;
            double l = (max + min) / 2;

            if (delta != 0)
            {
                s = l > 0.5 ? delta / (2 - max - min) : delta / (max + min);

                if (max == rf)
                    h = (gf - bf) / delta + (gf < bf ? 6 : 0);
                else if (max == gf)
                    h = (bf - rf) / delta + 2;
                else
                    h = (rf - gf) / delta + 4;

                h *= 60;
            }

            return (h, s * 100, l * 100);
        }

        private (byte r, byte g, byte b) HslToRgb(double h, double s, double l)
        {
            s /= 100;
            l /= 100;

            double c = (1 - Math.Abs(2 * l - 1)) * s;
            double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = l - c / 2;

            double rf = 0, gf = 0, bf = 0;

            if (h >= 0 && h < 60)
            {
                rf = c; gf = x; bf = 0;
            }
            else if (h >= 60 && h < 120)
            {
                rf = x; gf = c; bf = 0;
            }
            else if (h >= 120 && h < 180)
            {
                rf = 0; gf = c; bf = x;
            }
            else if (h >= 180 && h < 240)
            {
                rf = 0; gf = x; bf = c;
            }
            else if (h >= 240 && h < 300)
            {
                rf = x; gf = 0; bf = c;
            }
            else if (h >= 300 && h < 360)
            {
                rf = c; gf = 0; bf = x;
            }

            return (
                (byte)Math.Round((rf + m) * 255),
                (byte)Math.Round((gf + m) * 255),
                (byte)Math.Round((bf + m) * 255)
            );
        }

        private async void CopyHexButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(HexTextBox.Text))
            {
                var dataPackage = new DataPackage();
                dataPackage.SetText(HexTextBox.Text);
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

        private async void CopyRgbButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(RedTextBox.Text) &&
                !string.IsNullOrEmpty(GreenTextBox.Text) &&
                !string.IsNullOrEmpty(BlueTextBox.Text))
            {
                var text = $"RGB({RedTextBox.Text}, {GreenTextBox.Text}, {BlueTextBox.Text})";
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

        private async void CopyHslButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(HueTextBox.Text) &&
                !string.IsNullOrEmpty(SaturationTextBox.Text) &&
                !string.IsNullOrEmpty(LightnessTextBox.Text))
            {
                var text = $"HSL({HueTextBox.Text}, {SaturationTextBox.Text}%, {LightnessTextBox.Text}%)";
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
    }
} 
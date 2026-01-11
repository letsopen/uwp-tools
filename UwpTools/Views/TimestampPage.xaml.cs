using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UwpTools.Views
{
    public sealed partial class TimestampPage : Page
    {
        private bool isUpdating = false;

        public TimestampPage()
        {
            this.InitializeComponent();
            TimestampTypeComboBox.SelectedIndex = 0;
        }

        private void TimestampTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTimestampFromDateTime();
        }

        private void TimestampTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdating) return;

            if (long.TryParse(TimestampTextBox.Text, out long timestamp))
            {
                isUpdating = true;
                try
                {
                    DateTime dateTime;
                    if (TimestampTypeComboBox.SelectedIndex == 0) // 秒级
                    {
                        dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
                    }
                    else // 毫秒级
                    {
                        dateTime = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
                    }

                    DatePicker.Date = dateTime;
                    TimePicker.Time = dateTime.TimeOfDay;
                    UpdateResultText(dateTime);
                }
                finally
                {
                    isUpdating = false;
                }
            }
        }

        private void DatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            UpdateTimestampFromDateTime();
        }

        private void TimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            UpdateTimestampFromDateTime();
        }

        private void UpdateTimestampFromDateTime()
        {
            if (isUpdating) return;

            isUpdating = true;
            try
            {
                var dateTime = DatePicker.Date.DateTime + TimePicker.Time;
                if (TimestampTypeComboBox.SelectedIndex == 0) // 秒级
                {
                    TimestampTextBox.Text = new DateTimeOffset(dateTime).ToUnixTimeSeconds().ToString();
                }
                else // 毫秒级
                {
                    TimestampTextBox.Text = new DateTimeOffset(dateTime).ToUnixTimeMilliseconds().ToString();
                }
                UpdateResultText(dateTime);
            }
            finally
            {
                isUpdating = false;
            }
        }

        private void UpdateResultText(DateTime dateTime)
        {
            ResultTextBlock.Text = $"当前时间：{dateTime:yyyy-MM-dd HH:mm:ss}\n" +
                                 $"ISO 8601格式：{dateTime:yyyy-MM-ddTHH:mm:ss}\n" +
                                 $"RFC 1123格式：{dateTime:R}";
        }

        private async void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ResultTextBlock.Text))
            {
                var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
                dataPackage.SetText(ResultTextBlock.Text);
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
            TimestampTextBox.Text = string.Empty;
            ResultTextBlock.Text = string.Empty;
            DatePicker.Date = DateTime.Now;
            TimePicker.Time = DateTime.Now.TimeOfDay;
        }
    }
} 
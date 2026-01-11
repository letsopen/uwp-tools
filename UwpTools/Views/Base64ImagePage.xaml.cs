using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace UwpTools.Views
{
    public sealed partial class Base64ImagePage : Page
    {
        public Base64ImagePage()
        {
            this.InitializeComponent();
        }

        private async void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".gif");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                try
                {
                    // 显示预览
                    var stream = await file.OpenAsync(FileAccessMode.Read);
                    var bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(stream);

                    // 转换为Base64
                    stream.Seek(0);
                    var buffer = new byte[stream.Size];
                    var dataReader = DataReader.FromBuffer(await stream.ReadAsync(buffer.AsBuffer(), (uint)stream.Size, InputStreamOptions.None));
                    dataReader.ReadBytes(buffer);
                    Base64TextBox.Text = Convert.ToBase64String(buffer);

                    PreviewImage.Source = bitmapImage;
                }
                catch (Exception ex)
                {
                    var dialog = new ContentDialog
                    {
                        Title = "错误",
                        Content = $"处理图片时出错：{ex.Message}",
                        CloseButtonText = "确定"
                    };
                    await dialog.ShowAsync();
                }
            }
        }

        private async void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Base64TextBox.Text))
            {
                var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
                dataPackage.SetText(Base64TextBox.Text);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);

                var dialog = new ContentDialog
                {
                    Title = "成功",
                    Content = "Base64编码已复制到剪贴板",
                    CloseButtonText = "确定"
                };
                await dialog.ShowAsync();
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Base64TextBox.Text = string.Empty;
            PreviewImage.Source = null;
        }
    }
} 
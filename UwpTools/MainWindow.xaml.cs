using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using UwpTools.Models;
using UwpTools.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UwpTools
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private List<ToolItem> tools = new();

        public MainWindow()
        {
            this.InitializeComponent();
            InitializeTools();
        }

        private void InitializeTools()
        {
            tools = new List<ToolItem>
            {
                new ToolItem
                {
                    Title = "JSON工具",
                    Description = "JSON格式化、验证和转换工具",
                    Icon = Symbol.Document,
                    PageType = "JsonToolsPage"
                },
                new ToolItem
                {
                    Title = "Base64图片转换",
                    Description = "图片与Base64编码互转工具",
                    Icon = Symbol.Pictures,
                    PageType = "Base64ImagePage"
                },
                new ToolItem
                {
                    Title = "时间戳转换",
                    Description = "时间戳与日期时间互转工具",
                    Icon = Symbol.Clock,
                    PageType = "TimestampPage"
                },
                new ToolItem
                {
                    Title = "中文数字转换",
                    Description = "阿拉伯数字与中文大小写互转工具",
                    Icon = Symbol.Character,
                    PageType = "ChineseNumberPage"
                },
                new ToolItem
                {
                    Title = "HASH工具",
                    Description = "支持MD5、CRC32、SHA1、SHA256、SHA512等哈希算法",
                    Icon = Symbol.Keyboard,
                    PageType = "HashToolsPage"
                },
                new ToolItem
                {
                    Title = "JWT验证工具",
                    Description = "JWT Token解析和验证工具",
                    Icon = Symbol.Permissions,
                    PageType = "JwtToolsPage"
                },
                new ToolItem
                {
                    Title = "子网掩码计算",
                    Description = "IP地址和子网掩码计算工具",
                    Icon = Symbol.Globe,
                    PageType = "SubnetCalculatorPage"
                }
            };

            ToolsListView.ItemsSource = tools;
        }

        private void ToolsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ToolsListView.SelectedItem is ToolItem selectedTool)
            {
                NavigateToTool(selectedTool);
            }
        }

        private void NavigateToTool(ToolItem tool)
        {
            Page? page = null;
            switch (tool.PageType)
            {
                case "JsonToolsPage":
                    page = new JsonToolsPage();
                    break;
                case "Base64ImagePage":
                    page = new Base64ImagePage();
                    break;
                case "TimestampPage":
                    page = new TimestampPage();
                    break;
                case "ChineseNumberPage":
                    page = new ChineseNumberPage();
                    break;
                case "HashToolsPage":
                    page = new HashToolsPage();
                    break;
                case "JwtToolsPage":
                    page = new JwtToolsPage();
                    break;
                case "SubnetCalculatorPage":
                    page = new SubnetCalculatorPage();
                    break;
            }

            if (page != null)
            {
                ContentFrame.Content = page;
                BackButton.Visibility = Visibility.Visible;
                ToolsListView.Visibility = Visibility.Collapsed;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = ToolsListView;
            BackButton.Visibility = Visibility.Collapsed;
            ToolsListView.Visibility = Visibility.Visible;
            ToolsListView.SelectedItem = null;
        }
    }
}

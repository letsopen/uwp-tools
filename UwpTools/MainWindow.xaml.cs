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
                    Description = "JSON格式化、验证和转换",
                    Icon = "Json",
                    PageType = typeof(JsonToolsPage)
                },
                new ToolItem
                {
                    Title = "Base64图片",
                    Description = "Base64图片转换",
                    Icon = "Image",
                    PageType = typeof(Base64ImagePage)
                },
                new ToolItem
                {
                    Title = "时间戳",
                    Description = "时间戳转换工具",
                    Icon = "Clock",
                    PageType = typeof(TimestampPage)
                },
                new ToolItem
                {
                    Title = "中文数字",
                    Description = "中文数字转换工具",
                    Icon = "Number",
                    PageType = typeof(ChineseNumberPage)
                },
                new ToolItem
                {
                    Title = "哈希工具",
                    Description = "多种哈希算法计算",
                    Icon = "Hash",
                    PageType = typeof(HashToolsPage)
                },
                new ToolItem
                {
                    Title = "JWT工具",
                    Description = "JWT验证和解码",
                    Icon = "Jwt",
                    PageType = typeof(JwtToolsPage)
                },
                new ToolItem
                {
                    Title = "子网掩码",
                    Description = "子网掩码计算工具",
                    Icon = "Network",
                    PageType = typeof(SubnetMaskPage)
                },
                new ToolItem
                {
                    Title = "编码转换",
                    Description = "文本编码转换工具",
                    Icon = "Text",
                    PageType = typeof(EncodingToolsPage)
                },
                new ToolItem
                {
                    Title = "正则表达式",
                    Description = "正则表达式测试工具",
                    Icon = "Regex",
                    PageType = typeof(RegexToolsPage)
                },
                new ToolItem
                {
                    Title = "颜色工具",
                    Description = "颜色格式转换工具",
                    Icon = "Color",
                    PageType = typeof(ColorToolsPage)
                },
                new ToolItem
                {
                    Title = "开发辅助",
                    Description = "SQL、XML、Markdown工具",
                    Icon = "Code",
                    PageType = typeof(DevToolsPage)
                },
                new ToolItem
                {
                    Title = "加密工具",
                    Description = "多种加密算法工具",
                    Icon = "Lock",
                    PageType = typeof(EncryptionToolsPage)
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
            if (tool.PageType != null)
            {
                var page = Activator.CreateInstance(tool.PageType) as Page;
                if (page != null)
                {
                    ContentFrame.Content = page;
                    BackButton.Visibility = Visibility.Visible;
                    ToolsListView.Visibility = Visibility.Collapsed;
                }
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

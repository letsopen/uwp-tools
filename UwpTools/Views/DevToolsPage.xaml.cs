using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using UwpTools.Models;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了"用户控件"项模板

namespace UwpTools.Views
{
    public sealed partial class DevToolsPage : Page
    {
        public DevToolsPage()
        {
            this.InitializeComponent();
            LoadTools();
        }

        private void LoadTools()
        {
            var tools = new List<ToolItem>
            {
                new ToolItem { Title = "JSON 格式化", Description = "格式化 JSON 字符串", TargetPage = typeof(JsonToolsPage) },
                new ToolItem { Title = "时间戳转换", Description = "时间戳与日期转换", TargetPage = typeof(TimestampPage) },
                new ToolItem { Title = "加密工具", Description = "MD5、SHA等加密算法", TargetPage = typeof(EncryptionToolsPage) },
                new ToolItem { Title = "哈希工具", Description = "各种哈希算法", TargetPage = typeof(HashToolsPage) },
                new ToolItem { Title = "JWT 解析", Description = "JWT Token 解析", TargetPage = typeof(JwtToolsPage) },
                new ToolItem { Title = "正则表达式测试", Description = "正则表达式测试", TargetPage = typeof(RegexToolsPage) },
                new ToolItem { Title = "子网计算器", Description = "IP 子网计算", TargetPage = typeof(SubnetCalculatorPage) },
                new ToolItem { Title = "子网掩码计算", Description = "子网掩码计算", TargetPage = typeof(SubnetMaskPage) },
                new ToolItem { Title = "中文数字转换", Description = "阿拉伯数字转中文", TargetPage = typeof(ChineseNumberPage) },
                new ToolItem { Title = "Base64 图片编码", Description = "图片转 Base64", TargetPage = typeof(Base64ImagePage) },
                new ToolItem { Title = "颜色工具", Description = "颜色转换工具", TargetPage = typeof(ColorToolsPage) }
            };

            ToolsList.ItemsSource = tools;
        }

        private void ToolsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is ToolItem toolItem)
            {
                Frame.Navigate(toolItem.TargetPage);
            }
        }
    }
} 
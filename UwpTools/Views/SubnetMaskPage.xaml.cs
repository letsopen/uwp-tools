using System;
using System.Net;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace UwpTools.Views
{
    public sealed partial class SubnetMaskPage : Page
    {
        public SubnetMaskPage()
        {
            this.InitializeComponent();
            SubnetMaskComboBox.SelectedIndex = 0;
        }

        private void IpAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateSubnet();
        }

        private void SubnetMask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateSubnet();
        }

        private void CalculateSubnet()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(IpAddressTextBox.Text) || 
                    SubnetMaskComboBox.SelectedItem is not ComboBoxItem selectedItem)
                {
                    return;
                }

                if (!IPAddress.TryParse(IpAddressTextBox.Text, out IPAddress? ipAddress))
                {
                    ShowError("无效的IP地址格式");
                    return;
                }

                if (!int.TryParse(selectedItem.Tag.ToString(), out int prefixLength))
                {
                    ShowError("无效的子网掩码");
                    return;
                }

                var ipBytes = ipAddress.GetAddressBytes();
                var maskBytes = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    if (prefixLength >= 8)
                    {
                        maskBytes[i] = 255;
                        prefixLength -= 8;
                    }
                    else if (prefixLength > 0)
                    {
                        maskBytes[i] = (byte)(255 << (8 - prefixLength));
                        prefixLength = 0;
                    }
                }

                var networkAddress = new byte[4];
                var broadcastAddress = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    networkAddress[i] = (byte)(ipBytes[i] & maskBytes[i]);
                    broadcastAddress[i] = (byte)(networkAddress[i] | ~maskBytes[i]);
                }

                var network = new IPAddress(networkAddress);
                var broadcast = new IPAddress(broadcastAddress);
                var usableHosts = (long)Math.Pow(2, 32 - prefixLength) - 2;

                NetworkAddressText.Text = $"网络地址：{network}";
                BroadcastAddressText.Text = $"广播地址：{broadcast}";
                UsableIpRangeText.Text = $"可用IP范围：{network} - {broadcast}";
                TotalHostsText.Text = $"可用主机数：{usableHosts}";
            }
            catch (Exception ex)
            {
                ShowError($"计算错误：{ex.Message}");
            }
        }

        private async void ShowError(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "错误",
                Content = message,
                CloseButtonText = "确定",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }
} 
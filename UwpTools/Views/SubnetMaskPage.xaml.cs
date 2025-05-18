using System;
using System.Net;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace UwpTools.Views
{
    public sealed partial class SubnetMaskPage : Page
    {
        public SubnetMaskPage()
        {
            this.InitializeComponent();
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(IpAddressTextBox.Text))
            {
                ShowError("请输入IP地址");
                return;
            }

            if (SubnetMaskComboBox.SelectedItem == null)
            {
                ShowError("请选择子网掩码");
                return;
            }

            try
            {
                var ipAddress = IPAddress.Parse(IpAddressTextBox.Text);
                var cidr = int.Parse((SubnetMaskComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "24");
                var subnetMask = CalculateSubnetMask(cidr);
                var networkAddress = CalculateNetworkAddress(ipAddress, subnetMask);
                var broadcastAddress = CalculateBroadcastAddress(networkAddress, cidr);
                var usableIps = CalculateUsableIps(cidr);

                NetworkAddressTextBox.Text = networkAddress.ToString();
                BroadcastAddressTextBox.Text = broadcastAddress.ToString();
                AvailableIpsTextBox.Text = usableIps.ToString();
                IpRangeTextBox.Text = $"可用IP范围：\n{networkAddress} - {broadcastAddress}";
            }
            catch (Exception ex)
            {
                ShowError($"计算失败: {ex.Message}");
            }
        }

        private IPAddress CalculateSubnetMask(int cidr)
        {
            uint mask = ~((1u << (32 - cidr)) - 1);
            var bytes = BitConverter.GetBytes(mask);
            Array.Reverse(bytes);
            return new IPAddress(bytes);
        }

        private IPAddress CalculateNetworkAddress(IPAddress ip, IPAddress subnetMask)
        {
            byte[] ipBytes = ip.GetAddressBytes();
            byte[] maskBytes = subnetMask.GetAddressBytes();
            byte[] networkBytes = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
            }

            return new IPAddress(networkBytes);
        }

        private IPAddress CalculateBroadcastAddress(IPAddress networkAddress, int cidr)
        {
            byte[] networkBytes = networkAddress.GetAddressBytes();
            uint mask = ~((1u << (32 - cidr)) - 1);
            var bytes = BitConverter.GetBytes(~mask);
            Array.Reverse(bytes);

            for (int i = 0; i < 4; i++)
            {
                bytes[i] |= networkBytes[i];
            }

            return new IPAddress(bytes);
        }

        private long CalculateUsableIps(int cidr)
        {
            return (long)Math.Pow(2, 32 - cidr) - 2;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            IpAddressTextBox.Text = string.Empty;
            SubnetMaskComboBox.SelectedItem = null;
            NetworkAddressTextBox.Text = string.Empty;
            BroadcastAddressTextBox.Text = string.Empty;
            AvailableIpsTextBox.Text = string.Empty;
            IpRangeTextBox.Text = string.Empty;
        }

        private void ShowError(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "错误",
                Content = message,
                CloseButtonText = "确定",
                XamlRoot = this.XamlRoot
            };
            _ = dialog.ShowAsync();
        }
    }
} 
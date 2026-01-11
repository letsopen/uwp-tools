using System;
using System.Net;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;

namespace UwpTools.Views
{
    public sealed partial class SubnetCalculatorPage : Page
    {
        public SubnetCalculatorPage()
        {
            this.InitializeComponent();
            SubnetMaskComboBox.SelectedIndex = 8; // 默认选择 /24
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(IpAddressTextBox.Text) || SubnetMaskComboBox.SelectedItem == null)
            {
                ResultTextBox.Text = string.Empty;
                return;
            }

            try
            {
                var ipAddress = IPAddress.Parse(IpAddressTextBox.Text.Trim());
                var subnetBits = int.Parse((SubnetMaskComboBox.SelectedItem as ComboBoxItem)?.Tag.ToString() ?? "24");
                var subnetMask = CalculateSubnetMask(subnetBits);
                var networkAddress = CalculateNetworkAddress(ipAddress, subnetMask);
                var broadcastAddress = CalculateBroadcastAddress(networkAddress, subnetMask);
                var firstUsableAddress = CalculateFirstUsableAddress(networkAddress);
                var lastUsableAddress = CalculateLastUsableAddress(broadcastAddress);
                var totalHosts = CalculateTotalHosts(subnetBits);

                var result = new StringBuilder();
                result.AppendLine($"IP地址: {ipAddress}");
                result.AppendLine($"子网掩码: {subnetMask} (/{subnetBits})");
                result.AppendLine($"网络地址: {networkAddress}");
                result.AppendLine($"广播地址: {broadcastAddress}");
                result.AppendLine($"可用IP范围: {firstUsableAddress} - {lastUsableAddress}");
                result.AppendLine($"可用IP数量: {totalHosts}");

                ResultTextBox.Text = result.ToString();
            }
            catch (Exception ex)
            {
                ResultTextBox.Text = $"计算出错：{ex.Message}";
            }
        }

        private IPAddress CalculateSubnetMask(int bits)
        {
            uint mask = 0xFFFFFFFF;
            mask <<= (32 - bits);
            var bytes = BitConverter.GetBytes(mask);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return new IPAddress(bytes);
        }

        private IPAddress CalculateNetworkAddress(IPAddress ip, IPAddress subnetMask)
        {
            var ipBytes = ip.GetAddressBytes();
            var maskBytes = subnetMask.GetAddressBytes();
            var networkBytes = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
            }

            return new IPAddress(networkBytes);
        }

        private IPAddress CalculateBroadcastAddress(IPAddress networkAddress, IPAddress subnetMask)
        {
            var networkBytes = networkAddress.GetAddressBytes();
            var maskBytes = subnetMask.GetAddressBytes();
            var broadcastBytes = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                broadcastBytes[i] = (byte)(networkBytes[i] | ~maskBytes[i]);
            }

            return new IPAddress(broadcastBytes);
        }

        private IPAddress CalculateFirstUsableAddress(IPAddress networkAddress)
        {
            var bytes = networkAddress.GetAddressBytes();
            bytes[3]++;
            return new IPAddress(bytes);
        }

        private IPAddress CalculateLastUsableAddress(IPAddress broadcastAddress)
        {
            var bytes = broadcastAddress.GetAddressBytes();
            bytes[3]--;
            return new IPAddress(bytes);
        }

        private long CalculateTotalHosts(int subnetBits)
        {
            return (long)Math.Pow(2, 32 - subnetBits) - 2;
        }

        private async void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ResultTextBox.Text))
            {
                var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
                dataPackage.SetText(ResultTextBox.Text);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);

                var dialog = new ContentDialog
                {
                    Title = "成功",
                    Content = "计算结果已复制到剪贴板",
                    CloseButtonText = "确定"
                };
                await dialog.ShowAsync();
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            IpAddressTextBox.Text = string.Empty;
            ResultTextBox.Text = string.Empty;
        }
    }
} 
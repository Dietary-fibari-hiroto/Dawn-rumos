using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using rumos_client.Apis;
using rumos_client.Models;
using System;  
using System.Threading.Tasks;


namespace rumos_client.Components
{
    public sealed partial class DeviceStateBorad : UserControl
    {
        private readonly ApiClient _apiClient = new ApiClient();
        private int _deviceId;
        private int _devicePlatformId;
        private LedColor ledColor = new LedColor(); //Ledの発行情報
        public DeviceStateBorad()
        {
            InitializeComponent();
        }

        //モードの選択のたびに呼び出される関数
        private void ModeComboBox_SelectionChanged(object sender,SelectionChangedEventArgs e)
        {
            if(ModeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                ledColor.Mode = selectedItem.Content?.ToString();
            }
        }

        public int SelectedDeviceId
        {
            get => (int)GetValue(SelectedDeviceIdProperty);
            set => SetValue(SelectedDeviceIdProperty, value);
        }

        public static readonly DependencyProperty SelectedDeviceIdProperty =
            DependencyProperty.Register(
                nameof(SelectedDeviceId),
                typeof(int),
                typeof(DeviceStateBorad),
                new PropertyMetadata(0, OnSelectedDeviceChangedStatic));

        // static コールバックはインスタンスメソッドに橋渡しするだけ
        private static void OnSelectedDeviceChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DeviceStateBorad)d;
            control.OnSelectedDeviceChanged((int)e.NewValue);
        }

        // インスタンスメソッドで async にできる
        private async void OnSelectedDeviceChanged(int newId)
        {
            try
            {
                _deviceId = newId;
                DeviceIdLabel.Text = newId.ToString();

                var device = await _apiClient.GetByIdAsync<Device>("/device", newId);
                if (device == null) return;

                DeviceNameLabel.Text = device.Name;
                DeviceIpLabel.Text = device.Ip_v4;

                //platform_idが前回のものと違ったときの処理
                if(device.Platform_id != _devicePlatformId)
                {
                    _devicePlatformId = device.Platform_id;
                    if (_devicePlatformId != 1) colorPicker.Visibility = Visibility.Collapsed;
                    else colorPicker.Visibility = Visibility.Visible;
                }



                var res = await _apiClient.GetTpState(newId);

  
                if (res.IsConnect)
                {
                    ConnectionBar.Background = new SolidColorBrush(res.IsOn ? Colors.Green : Colors.Red);
                }
                else
                {
                    ConnectionBar.Background = new SolidColorBrush(Colors.Gray);
                }
            }
            catch (Exception ex)
            {
                // 必要に応じてログ出力
                System.Diagnostics.Debug.WriteLine($"Error updating device: {ex.Message}");
                ConnectionBar.Background = new SolidColorBrush(Colors.Gray);
            }
        }

        private async void PowerSupply(object sebder,RoutedEventArgs e)
        {
            if (_devicePlatformId == 2)
            {
                var res = await _apiClient.PostPowerSupply(_deviceId);
                ConnectionBar.Background = new SolidColorBrush(res.IsOn ? Colors.Green : Colors.Red);
            }
            else
            {
                await _apiClient.LuminasLedColorAsync(ledColor, _deviceId);
            }

        }

        private void ColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            var color = args.NewColor;

            // Color → LedColor へ変換

            ledColor.R = color.R;
            ledColor.G = color.G;
            ledColor.B = color.B;
        }




    }
}

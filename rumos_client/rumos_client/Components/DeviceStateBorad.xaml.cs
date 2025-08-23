using rumos_client.Apis;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using rumos_client.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace rumos_client.Components;

public sealed partial class DeviceStateBorad : UserControl
{
    public DeviceStateBorad()
    {
        InitializeComponent();
    }

    public int SelectedDeviceId
    {
        get => (int)GetValue(SelectedDeviceIdProperty);
        set => SetValue(SelectedDeviceIdProperty, value);
     }
    public static readonly DependencyProperty SelectedDeviceIdProperty = 
        DependencyProperty.Register(nameof(SelectedDeviceId),typeof(int),typeof(DeviceStateBorad),
            new PropertyMetadata(0,OnSelectedDeviceChanged));

    private static async void OnSelectedDeviceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (DeviceStateBorad)d;
        int newId = (int)e.NewValue;

        control.DeviceIdLabel.Text = newId.ToString();

        ApiClient _apiClient = new ApiClient();
        var device = await _apiClient.GetByIdAsync<Device>("/device",newId);

        if (device == null) {
            return;
         }

        control.DeviceNameLabel.Text = device?.Name;
        control.DeviceIpLabel.Text = device?.Ip_v4;


    }


}

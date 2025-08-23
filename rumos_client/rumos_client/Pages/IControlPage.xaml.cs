using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using rumos_client.Apis;
using rumos_client.Components;
using rumos_client.Models;
using System.Collections.Generic;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace rumos_client.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IControlPage : Page
    {
        private readonly ApiClient api = new ApiClient();

        public IControlPage()
        {
            InitializeComponent();
            GetDeviceLIst();
            Loaded += IControlPage_Loaded;
            ToHome.NavigateRequest += (s, e) =>
            {
                Frame.Navigate(typeof(FunctionSelectionPage));
            };
        }

        public async void GetDeviceLIst()
        {
            var devices = await api.GetAsync<List<Device>>("/device");

            if(devices == null)
            {
                return;
            }
            DeviceRepeater.ItemsSource = devices;


        }

        public void IControlPage_Loaded(object sender,RoutedEventArgs e)
        {
            DeviceRepeater.ElementPrepared += DeviceRepeater_ElementPrepared;
        }

        private void DeviceRepeater_ElementPrepared(ItemsRepeater sender,ItemsRepeaterElementPreparedEventArgs args)
        {
            if(args.Element is DeviceLabel deviceLabel)
            {
                deviceLabel.DeviceClicked += (s, deviceId) =>
                {
                    DeviceStateBorad.SelectedDeviceId = deviceId;
                };
            }
        }

    }
}

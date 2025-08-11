using System;
using System.IO;
using Microsoft.UI.Xaml;
using rumos.AppSetting;
using rumos.Api;
using Microsoft.UI.Windowing;



// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace roumos_test
{ 
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly RumosApi _rumosApi;

        public MainWindow()
        {
            InitializeComponent();
            _rumosApi = new RumosApi();

        }

        private async void PostSupply(object sender, RoutedEventArgs e) { 
            Test.Text = "ボタン反応";
            try
            {
                string result = await _rumosApi.PostPowerSupply(ApiEndpoints.baseUrl+"/power");
                OutputTextBlock.Text = result;
            }catch(Exception ex)
            {
                OutputTextBlock.Text = $"エラー: {ex.Message}";
            }
        }
    }
}

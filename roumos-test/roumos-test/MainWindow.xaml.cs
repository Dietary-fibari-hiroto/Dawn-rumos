using Grpc.IDevice;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using rumos.Api;
using rumos.AppSetting;
using System;
using System.IO;
using System.Text.Json;



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
            Test.Text = "psボタン反応";
            try
            {
                string res = await _rumosApi.PostPowerSupply(ApiEndpoints.baseUrl + "/sp");
                OutputTextBlock.Text = res;
            }
            catch(Exception ex)
            {
                OutputTextBlock.Text = $"エラー: {ex.Message}";
            }
        }
        private async void PostSupply2(object sender, RoutedEventArgs e)
        {
            Test.Text = "ps2ボタン反応";
            try
            {
                string resJson = await _rumosApi.PostPowerSupply(ApiEndpoints.baseUrl + "/sp2");
                //JSON => RPowerResponse に変換
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                var res = JsonSerializer.Deserialize<RPowerResponse>(resJson, options);

                if(res != null)
                {
                    OutputTextBlock.Text = $"Success: {res.Success}, Message: {res.Message}, IsOn: {res.IsOn}";
                    if (!res.IsOn)
                    {
                        Panel.Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                    }
                    else
                    {
                        Panel.Background = new SolidColorBrush(Color.FromArgb(255, 70, 255, 0));
                    }
                }
                else
                {
                    OutputTextBlock.Text = "レスポンスの変換に失敗しました。";
                }

            }
            catch (Exception ex)
            {
                OutputTextBlock.Text = $"エラー: {ex.Message}";
            }
        }

        private async void PostSupply3(object sender, RoutedEventArgs e)
        {
            Test.Text = "ps3ボタン反応";
            try
            {
                string res = await _rumosApi.PostPowerSupply(ApiEndpoints.baseUrl + "/sp3");

                OutputTextBlock.Text = res;
            }
            catch (Exception ex)
            {
                OutputTextBlock.Text = $"エラー: {ex.Message}";
            }
        }
    }
}

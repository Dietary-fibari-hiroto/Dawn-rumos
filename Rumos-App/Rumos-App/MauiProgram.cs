using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
using Rumos_App.Services;
using System.Reflection;

#if WINDOWS
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;
using Microsoft.UI;  
using Windows.Graphics;
#endif


namespace Rumos_App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // Configuration読み込み
            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream("Rumos_App.appsettings.json");
            var config = new ConfigurationBuilder()
                .AddJsonStream(stream!)
                .Build();
            builder.Configuration.AddConfiguration(config);
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");

                })
                .ConfigureLifecycleEvents(events => {
                    #if WINDOWS
                        events.AddWindows(windows =>
                        {
                            windows.OnWindowCreated(window =>
                            {
                                // MAUI の Window から WinUI3 のネイティブウィンドウを取得
                                var hwnd = WindowNative.GetWindowHandle(window);
                                var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
                                var appWindow = AppWindow.GetFromWindowId(windowId);

                                // WinUI3 の OverlappedPresenter で最大化
                                if (appWindow.Presenter is OverlappedPresenter presenter)
                                {
                                    presenter.Maximize();
                                }
                            });
                        });
                    #endif
                });

            var baseUrl = builder.Configuration["ApiSettings:BaseApiUrl"];

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton(new HttpClient());
            builder.Services.AddScoped<ApiService>();
            builder.Services.AddScoped<MagicRoutineApiService>();
            builder.Services.AddSingleton<SignalRService>();            
            return builder.Build();
        }
    }
}

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using rumos_client.Pages;
using Microsoft.UI.Xaml.Media.Animation;
using System;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace rumos_client
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetFullScreen();

            RootFrame.Navigated += (s, e) =>
            {
                if (RootFrame.Content is UIElement element)
                {
                    element.Opacity = 0;

                    var fadeIn = new DoubleAnimation
                    {
                        To = 1,
                        Duration = new Duration(TimeSpan.FromMilliseconds(300))
                    };

                    var sb = new Storyboard();
                    sb.Children.Add(fadeIn);
                    Storyboard.SetTarget(fadeIn, element);
                    Storyboard.SetTargetProperty(fadeIn, "Opacity");
                    sb.Begin();
                }
            };

            RootFrame.Navigate(typeof(Opening));
        }

        private void SetFullScreen()
        {
            // タイトルバーを非表示にしてコンテンツを画面全体に拡張
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(null);

            // ウィンドウサイズをスクリーンに合わせる
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            // ウィンドウ状態を最大化
            appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
        }


    }
}

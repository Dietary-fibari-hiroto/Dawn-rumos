using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace rumos_client.Tests
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestPage : Page
    {
        private int _pageCount;
        private string[] _backgrounds = new[]
        {
            "ms-appx:///Assets/Images/IMG_0086.jpg",
            "ms-appx:///Assets/Images/IMG_7028.jpg",
            "ms-appx:///Assets/Images/IMG_7053.jpg"
        };
        private int _currentIndex = 0;
        public TestPage()
        {
            InitializeComponent();
            _pageCount = PanelStack.Children.Count;
            BackgroundImage.Source = new BitmapImage(new Uri(_backgrounds[0]));

        }
        // スクロール終了時に「2割ルール」でスナップ
        private void Scroller_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (!e.IsIntermediate) // スクロール完了時のみ
            {
                double viewportHeight = Scroller.ViewportHeight;
                double offset = Scroller.VerticalOffset;

                int currentPage = (int)(offset / viewportHeight);

                int pageIndex = (int)Math.Round(offset / viewportHeight);

                if (pageIndex != _currentIndex && pageIndex < _backgrounds.Length)
                {
                    ChangeBackground(pageIndex);
                    _currentIndex = pageIndex;
                }

                // 次のページに進むしきい値（20%）
                double delta = offset - currentPage * viewportHeight;
                if (delta > viewportHeight * 0.4 && currentPage < _pageCount - 1)
                {
                    currentPage++;
                }
                else if (delta < viewportHeight * 0.8 && delta < 0 && currentPage > 0)
                {
                    currentPage--;
                }

                // スナップ
                Scroller.ChangeView(null, currentPage * viewportHeight, null, false);
            }
        }

        // 画面クリックで上下移動
        private void RootGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var pos = e.GetPosition(RootGrid);
            double viewportHeight = Scroller.ViewportHeight;
            double offset = Scroller.VerticalOffset;

            int currentPage = (int)Math.Round(offset / viewportHeight);

            if (pos.Y < viewportHeight * 0.2 && currentPage > 0)
            {
                // 上 20% クリック → 前ページ
                currentPage--;
            }
            else if (pos.Y > viewportHeight * 0.8 && currentPage < _pageCount - 1)
            {
                // 下 20% クリック → 次ページ
                currentPage++;
            }

            Scroller.ChangeView(null, currentPage * viewportHeight, null, true);
        }

        // ウィンドウサイズ変更時に要素高さを追従
        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in PanelStack.Children)
            {
                if (child is Border border)
                {
                    border.Height = e.NewSize.Height;
                }
            }
        }

        private void ChangeBackground(int index)
        {
            // フェードアウト開始
            FadeOutStoryboard.Completed += (s, e) =>
            {
                // 画像を切り替え
                BackgroundImage.Source = new BitmapImage(new Uri(_backgrounds[index]));

                // フェードイン開始
                FadeInStoryboard.Begin();
                FadeOutStoryboard.Completed -= null; // ハンドラ解除（多重実行防止）
            };

            FadeOutStoryboard.Begin();
        }


    }


}


using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace rumos_client.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FunctionSelectionPage : Page
    {
        //PanelStack内にあるセクションの数
        private int _pageCount;
        private string[] _backgrounds = new[]
        {
            "ms-appx:///Assets/Images/IMG_0086.jpg",
            "ms-appx:///Assets/Images/IMG_7028.jpg",
            "ms-appx:///Assets/Images/IMG_7053.jpg"
        };

        private int _currentIndex = 0;

        public FunctionSelectionPage()
        {
            InitializeComponent();
            //セクションの数を取得
            _pageCount = PanelStack.Children.Count;
            //バックグラウンドイメージをbackgroundsの0番目で初期化
            BackgroundImage.Source = new BitmapImage(new Uri(_backgrounds[0]));
        }

        //スクロール時のルールを定義
        private void Scroller_ViewChanged(object sener,ScrollViewerViewChangedEventArgs e)
        {
            if (!e.IsIntermediate)
            {
                double viewportHeight = Scroller.ViewportHeight;

                double offset = Scroller.VerticalOffset;

                int currentPage = (int)(offset / viewportHeight);

                int pageIndex = (int)Math.Round(offset / viewportHeight);

                if(pageIndex != _currentIndex && pageIndex < _backgrounds.Length)
                {
                    ChangeBackground(pageIndex);
                    _currentIndex = pageIndex;
                }

                //次のぺーじに進むしきい値(20%)
                double delta = offset - currentPage * viewportHeight;
                if(delta > viewportHeight * 0.4 && currentPage < _pageCount - 1)
                {
                    currentPage++;
                } else if(delta < viewportHeight *0.8 && delta < 0 &&currentPage > 0)
                {
                    currentPage--;
                }

                //スナップ
                Scroller.ChangeView(null, currentPage * viewportHeight, null, false);
            }
        }

        //画面クリックで上下移動
        private void RootGrid_Tapped(object sender,TappedRoutedEventArgs e)
        {
            var pos = e.GetPosition(RootGrid);
            double viewportHeight = Scroller.ViewportHeight;
            double offset = Scroller.VerticalOffset;

            int currentPage = (int)Math.Round(offset / viewportHeight);
            
            if(pos.Y <  viewportHeight *0.2 && currentPage > 0)
            {
                currentPage--;
            } else if(pos.Y > viewportHeight * 0.8 && currentPage < _pageCount - 1)
            {
                currentPage--;
            }
            Scroller.ChangeView(null, currentPage * viewportHeight, null, true);
        }

        private  void RootGrid_SizeChanged(object sender,SizeChangedEventArgs e)
        {
            foreach(var child in PanelStack.Children)
            {
                if(child is Border border)
                {
                    border.Height = e.NewSize.Height;
                    border.Width = e.NewSize.Width;
                }
            }
        }

        //バックグラウンドの変更処理
        private void ChangeBackground(int index) {
            FadeOutStoryboard.Completed += (s, e) =>
            {
                //画像を切り替え
                BackgroundImage.Source = new BitmapImage(new Uri(_backgrounds[index]));
                //フェードイン開始
                FadeInStoryboard.Begin();
                FadeOutStoryboard.Completed -= null;
            };
            FadeOutStoryboard.Begin();
        }
    }
}

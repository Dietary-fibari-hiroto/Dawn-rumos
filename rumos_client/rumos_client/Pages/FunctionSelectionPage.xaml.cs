using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace rumos_client.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FunctionSelectionPage : Page
    {
        public FunctionSelectionPage()
        {
            InitializeComponent();
        }
        private async void Next_Click(object sender, RoutedEventArgs e)
        {
            // 文字Currentを上にスライドしながらフェードアウト
            var textOut = new Storyboard();
            var moveOut = new DoubleAnimation { From = 0, To = -50, Duration = new Duration(TimeSpan.FromSeconds(0.5)) };
            Storyboard.SetTarget(moveOut, TextCurrent.RenderTransform);
            Storyboard.SetTargetProperty(moveOut, "Y");

            var fadeOut = new DoubleAnimation { From = 1, To = 0, Duration = new Duration(TimeSpan.FromSeconds(0.5)) };
            Storyboard.SetTarget(fadeOut, TextCurrent);
            Storyboard.SetTargetProperty(fadeOut, "Opacity");

            textOut.Children.Add(moveOut);
            textOut.Children.Add(fadeOut);
            textOut.Begin();

            // 背景画像フェード
            var bgFade = new Storyboard();
            var bgOut = new DoubleAnimation { From = 1, To = 0, Duration = new Duration(TimeSpan.FromSeconds(0.5)) };
            Storyboard.SetTarget(bgOut, Background1);
            Storyboard.SetTargetProperty(bgOut, "Opacity");
            var bgIn = new DoubleAnimation { From = 0, To = 1, Duration = new Duration(TimeSpan.FromSeconds(0.5)) };
            Storyboard.SetTarget(bgIn, Background2);
            Storyboard.SetTargetProperty(bgIn, "Opacity");

            bgFade.Children.Add(bgOut);
            bgFade.Children.Add(bgIn);
            bgFade.Begin();

            await Task.Delay(500);

            // 次の文字を上にスライドしながらフェードイン
            var textIn = new Storyboard();
            var moveIn = new DoubleAnimation { From = 50, To = 0, Duration = new Duration(TimeSpan.FromSeconds(0.5)) };
            Storyboard.SetTarget(moveIn, TextNext.RenderTransform);
            Storyboard.SetTargetProperty(moveIn, "Y");

            var fadeIn = new DoubleAnimation { From = 0, To = 1, Duration = new Duration(TimeSpan.FromSeconds(0.5)) };
            Storyboard.SetTarget(fadeIn, TextNext);
            Storyboard.SetTargetProperty(fadeIn, "Opacity");

            textIn.Children.Add(moveIn);
            textIn.Children.Add(fadeIn);
            textIn.Begin();

            await Task.Delay(500);

            // 文字差し替え用に入れ替え
            TextCurrent.Text = TextNext.Text;
            TextCurrent.Opacity = 1;
            ((TranslateTransform)TextCurrent.RenderTransform).Y = 0;

            TextNext.Opacity = 0;
            ((TranslateTransform)TextNext.RenderTransform).Y = 50;

            // 背景差し替えも入れ替え可能
            var temp = Background1.Source;
            Background1.Source = Background2.Source;
            Background2.Source = temp;
        }

    }
}

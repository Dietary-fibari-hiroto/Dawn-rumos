using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;

using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace rumos_client.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Opening : Page
    {
        public Opening()
        {
            InitializeComponent();
            StartAnimation();
        }

        public async void StartAnimation()
        {
            var easing = new CubicEase { EasingMode = EasingMode.EaseOut };
            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                EasingFunction = easing
            };
            var scaleX = new DoubleAnimation
            {
                From = 0.8,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                EasingFunction = easing
            };
            var scaleY = new DoubleAnimation
            {
                From = 0.8,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                EasingFunction = easing
            };

            var storyboardIn = new Storyboard();
            Storyboard.SetTarget(fadeIn, op_img);
            Storyboard.SetTargetProperty(fadeIn, "Opacity");

            Storyboard.SetTarget(scaleX, op_img.RenderTransform);
            Storyboard.SetTargetProperty(scaleX, "ScaleX");

            Storyboard.SetTarget(scaleY, op_img.RenderTransform);
            Storyboard.SetTargetProperty(scaleY, "ScaleY");

            storyboardIn.Children.Add(fadeIn);
            storyboardIn.Children.Add(scaleY);
            storyboardIn.Children.Add(scaleX);
            storyboardIn.Begin();

            await Task.Delay(2500);

            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(1))
            };
            var storyboardOut = new Storyboard();
            Storyboard.SetTarget(fadeOut, op_img);
            Storyboard.SetTargetProperty(fadeOut, "Opacity");
            storyboardOut.Children.Add(fadeOut);
            storyboardOut.Begin();

            await Task.Delay(1000);

            Frame.Navigate(typeof(FunctionSelectionPage));
        }
    }
}

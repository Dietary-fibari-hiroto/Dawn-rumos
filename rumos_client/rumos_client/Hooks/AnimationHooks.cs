using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml;
using Windows.UI;
using System;
namespace rumos_client.Hooks
{
    class AnimationHooks
    {
        public static void AnimateTextColor(TextBlock textBlock, Color from, Color to)
        {
            var brush = new SolidColorBrush(from);
            textBlock.Foreground = brush;

            var animation = new ColorAnimation
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(0.5)), // 0.5秒で変化
                EnableDependentAnimation = true // Foreground は依存関係アニメなのでこれ必須
            };

            Storyboard.SetTarget(animation, brush);
            Storyboard.SetTargetProperty(animation, "Color");

            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }
    }
}

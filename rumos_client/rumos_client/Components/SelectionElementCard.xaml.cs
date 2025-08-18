using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace rumos_client.Components;

public sealed partial class SelectionElementCard : UserControl
{
    public SelectionElementCard()
    {
        InitializeComponent();
    }

    //画像パス
    public string ImageSource
    {
        get => (string)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public static readonly DependencyProperty ImageSourceProperty =
        DependencyProperty.Register(nameof(ImageSource), typeof(string), typeof(SelectionElementCard),
            new PropertyMetadata(null, OnImageSourceChanged));

    public static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if(d is SelectionElementCard card && e.NewValue is string path) { 
            card.CardImage.Source = new BitmapImage(new Uri(path));
        }
    }


    //画像パス
    public string BackgroundSource
    {
        get => (string)GetValue(BackgroundSourceProperty);
        set => SetValue(BackgroundSourceProperty, value);
    }

    public static readonly DependencyProperty BackgroundSourceProperty =
        DependencyProperty.Register(nameof(BackgroundSource), typeof(string), typeof(SelectionElementCard),
            new PropertyMetadata(null, OnBackgroundSourceChanged));

    public static void OnBackgroundSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SelectionElementCard card && e.NewValue is string path)
        {
            card.CardBackground.Source = new BitmapImage(new Uri(path));
        }
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(SelectionElementCard),
            new PropertyMetadata("", (d, e) =>
            {
                if (d is SelectionElementCard card)
                {
                    card.CardTitle.Text = e.NewValue?.ToString();
                }
            }));
    
}

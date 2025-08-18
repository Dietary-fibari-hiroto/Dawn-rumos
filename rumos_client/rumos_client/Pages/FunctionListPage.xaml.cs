using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using rumos_client.Components;
using rumos_client.Constants;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace rumos_client.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class FunctionListPage : Page
{
    public FunctionListPage()
    {
        InitializeComponent();
        AddSelectionCard();
    }




    private void ToSelector(object sender,RoutedEventArgs e)
    {
        Frame.Navigate(typeof(FunctionSelectionPage));
    }

    private void AddSelectionCard()
    {
        for (int i =0; i < AppUIData.cardData.Length; i++)
        {
            var item = AppUIData.cardData[i];
            var card = new SelectionElementCard
            {
                ImageSource = item.icon,
                BackgroundSource = item.bg,
                Title = item.title,
            };

            int row = i / 3;
            int col = i % 3;

            while (CardGrid.RowDefinitions.Count <= row) CardGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            Grid.SetRow(card, row);
            Grid.SetColumn(card, col);
            CardGrid.Children.Add(card);
            }
    }
}

/*

foreach (var (img, title) in cardData)
{
    var myCard = new MyCard
    {
        ImageSource = img,
        Title = title
    };
stack.Children.Add(myCard);
}
*/
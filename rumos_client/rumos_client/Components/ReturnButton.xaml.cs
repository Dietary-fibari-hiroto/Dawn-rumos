using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using rumos_client.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace rumos_client.Components;

public sealed partial class ReturnButton : UserControl
{
    public event EventHandler? NavigateRequest;
    public ReturnButton()
    {
        InitializeComponent();
    }
    private void ReturnSelection(object sender,TappedRoutedEventArgs e)
    {
        NavigateRequest?.Invoke(this, EventArgs.Empty);
    }
}

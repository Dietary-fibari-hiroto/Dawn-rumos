using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace rumos_client.Components;

public sealed partial class DeviceLabel : UserControl
{
    //コンストラクタ。初期化時の処理
    public DeviceLabel()
    {
        //XAMLのInitializeComponent()を読んでUI要素を構築
        InitializeComponent();
    }

    /*
     * 公開プロパティー DeviceIconSource(string型)
     * これを使ってコントロール外から「画像パス」を設定できるようにする。
     */
    public string DeviceIconSource
    {
        get => (string)GetValue(DeviceIconProperty);// DependencyPropertyから値を取得
        set => SetValue(DeviceIconProperty, value);// DependencyPropertyに値を設定
    }

    // DependencyProperty を定義。XAMLでバインディングやスタイルで設定できるようになる
    public static readonly DependencyProperty DeviceIconProperty =
        DependencyProperty.Register(
            nameof(DeviceIconSource),// プロパティ名
            typeof(string),// プロパティの型 (string)
            typeof(DeviceLabel),// このプロパティを持つ型 (DeviceLabel)
            new PropertyMetadata(// プロパティのメタデータを設定
                null,// 初期値 (null)
                OnDeviceIconSourceChanged// 値が変わったときに呼ばれるコールバック
                )
            );
    // DeviceIconSource の値が変わった時に実行される処理
    private static void OnDeviceIconSourceChanged(DependencyObject d,DependencyPropertyChangedEventArgs e)
    {
        // d が DeviceLabel か確認し、e.NewValue が string (画像のパス) なら処理する
        if (d is DeviceLabel label && e.NewValue is string type)
        {
            string assetPath = type switch
            {
                "0" => "ms-appx:///Assets/Images/Device_icon.png",
                "1" => "ms-appx:///Assets/Images/icontrol_icon.png",
                "2" => "ms-appx:///Assets/Images/Environment_icon.png",
                _ => "ms-appx:///Assets/Images/magicroutin_icon.png" // 未定義の場合
            };
            label.DeviceIcon.Source = new BitmapImage(new Uri(assetPath));
        }
    }


    public string DeviceNameText
    {
        get => (string)GetValue(DeviceNameProperty);
        set => SetValue(DeviceNameProperty, value);
    }

    public static readonly DependencyProperty DeviceNameProperty =
        DependencyProperty.Register(nameof(DeviceNameText), typeof(string), typeof(DeviceLabel),
            new PropertyMetadata("", (d, e) =>
            {
                if (d is DeviceLabel label)
                {
                    label.DeviceName.Text = e.NewValue?.ToString();
                }
            }));

    public int DeviceId
    {
        get => (int)GetValue(DeviceIdProperty);
        set => SetValue(DeviceIdProperty, value);
    }

    public static readonly DependencyProperty DeviceIdProperty =
        DependencyProperty.Register(nameof(DeviceId), typeof(int), typeof(DeviceLabel), new PropertyMetadata(0));

    public event EventHandler<int>? DeviceClicked;

    private void DeviceLabel_Tapped(object sender, TappedRoutedEventArgs e)
    {
        DeviceClicked?.Invoke(this, DeviceId);
    }
}

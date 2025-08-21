using Microsoft.UI.Xaml.Controls;
using rumos_client.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rumos_client.Constants
{
    public class AppUIData
    {
        public static (string icon,string bg, string title)[] cardData = new (string, string,string)[]
        {
        ("ms-appx:///Assets/Images/icontrol_icon.png", "ms-appx:///Assets/Images/IMG_7053.jpg", "I Control"),
        ("ms-appx:///Assets/Images/magicroutin_icon.png", "ms-appx:///Assets/Images/IMG_3681.jpg", "MagicRoutin"),
                ("ms-appx:///Assets/Images/Device_icon.png", "ms-appx:///Assets/Images/IMG_7083.jpg", "Device"),
                        ("ms-appx:///Assets/Images/Environment_icon.png", "ms-appx:///Assets/Images/Environment_bg.png", "Environment"),
                                ("ms-appx:///Assets/Images/setting_icon.png", "ms-appx:///Assets/Images/IMG_7067.jpg", "Setting"),
        };

    }
}

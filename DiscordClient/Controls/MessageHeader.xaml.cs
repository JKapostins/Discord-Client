using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DiscordClient.Controls
{
    public sealed partial class MessageHeader : UserControl
    {
        public MessageHeader()
        {
            this.InitializeComponent();
        }

        public ImageSource UserIcon
        {
            get { return (ImageSource)GetValue(UserIconProperty); }
            set { SetValue(UserIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserIconProperty =
            DependencyProperty.Register("UserIcon", typeof(ImageSource), typeof(MessageHeader), null);



        public string UsernName
        {
            get { return (string)GetValue(UsernNameProperty); }
            set { SetValue(UsernNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UsernName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UsernNameProperty =
            DependencyProperty.Register("UsernName", typeof(string), typeof(MessageHeader), null);



        public SolidColorBrush UserNameColor
        {
            get { return (SolidColorBrush)GetValue(UserNameColorProperty); }
            set { SetValue(UserNameColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserNameColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserNameColorProperty =
            DependencyProperty.Register("UserNameColor", typeof(SolidColorBrush), typeof(MessageHeader), null);





        public string TimeStamp
        {
            get { return (string)GetValue(TimeStampProperty); }
            set { SetValue(TimeStampProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeStamp.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeStampProperty =
            DependencyProperty.Register("TimeStamp", typeof(string), typeof(MessageHeader), null);


    }
}

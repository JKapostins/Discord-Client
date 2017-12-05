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

namespace Discord.Controls
{
    public sealed partial class FriendListItem : UserControl
    {
        public FriendListItem()
        {
            this.InitializeComponent();
        }

        public ImageSource UserIcon
        {
            get { return (ImageSource)GetValue(UserIconProperty); }
            set { SetValue(UserIconProperty, value); }
        }
        // Using a DependencyProperty as the backing store for UserIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserIconProperty =  DependencyProperty.Register("UserIcon", typeof(ImageSource), typeof(FriendListItem), null);

        public Status OnlineStatus
        {
            get { return (Status)GetValue(OnlineStatusProperty); }
            set { SetValue(OnlineStatusProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnlineStatusProperty = DependencyProperty.Register("OnlineStatus", typeof(Status), typeof(FriendListItem), null);

        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }
        // Using a DependencyProperty as the backing store for UserName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserNameProperty = DependencyProperty.Register("UserName", typeof(string), typeof(FriendListItem), null);

        public string GameName
        {
            get { return (string)GetValue(GameNameProperty); }
            set { SetValue(GameNameProperty, value); }
        }
        // Using a DependencyProperty as the backing store for GameName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GameNameProperty = DependencyProperty.Register("GameName", typeof(string), typeof(FriendListItem), null);

        public SolidColorBrush UsernameForeground
        {
            get { return (SolidColorBrush)GetValue(UsernameForegroundProperty); }
            set { SetValue(UsernameForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UsernameForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UsernameForegroundProperty = DependencyProperty.Register("UsernameForeground", typeof(SolidColorBrush), typeof(FriendListItem), null);


    }
}

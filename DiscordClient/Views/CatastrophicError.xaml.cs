using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DiscordClient.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CatastrophicError : Page
    {
        public CatastrophicError()
        {
            this.InitializeComponent();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            CoreApplication.Exit();
        }

        private void launchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame != null)
                {
                    if (GnarlyClient.Instance.Connected)
                    {
                        rootFrame.Navigate(typeof(MainPage));
                    }
                    else
                    {
                        rootFrame.Navigate(typeof(LoginSelection));
                    }
                }
            }
            catch
            {
                CoreApplication.Exit();
            }
        }
    }
}

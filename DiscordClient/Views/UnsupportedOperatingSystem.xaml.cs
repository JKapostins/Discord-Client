using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DiscordClient.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UnsupportedOperatingSystem : Page
    {
        public UnsupportedOperatingSystem()
        {
            this.InitializeComponent();
            string deviceFamilyVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong version = ulong.Parse(deviceFamilyVersion);
            ulong build = (version & 0x00000000FFFF0000L) >> 16;
            string message = "You are using Windows 10 build " + build.ToString() + " which is not currently supported. Please upgrade to 14393 (Anniversary Edition) or later to use this app.";
            string messageSupport = "If upgrading is not an option, request support for build " + build.ToString() + " on the app ";
            Paragraph paragraph = new Paragraph();
            

            paragraph.Inlines.Add(new Run
            {
                Text = message
            });
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(new Run
            {
                Text = messageSupport
            });

            var hyperLink = new Hyperlink()
            {
                NavigateUri = new Uri("https://trello.com/b/HgHREOjb"),
            };
            hyperLink.Inlines.Add(new Run
            {
                Text = "Trello board"
            });
            paragraph.Inlines.Add(hyperLink);
            paragraph.Inlines.Add(new Run
            {
                Text = "."
            });

            messageBlock.Blocks.Add(paragraph);
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            CoreApplication.Exit();
        }
    }
}

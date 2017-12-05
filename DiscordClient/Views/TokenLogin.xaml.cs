using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
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
    public sealed partial class TokenLogin : Page
    {
        public TokenLogin()
        {
            this.InitializeComponent();
        }

        private void DiscordLogin()
        {
            try
            {
                if (tokenBox.Text == string.Empty)
                {
                    errorMessageTextBlock.Text = "You must enter your account token to log in.";
                    errorMessageTextBlock.Visibility = Visibility.Visible;
                    return;
                }

                string additionalErrorDetails = string.Empty;
                if (GnarlyClient.Instance.Connect(tokenBox.Text, out additionalErrorDetails))
                {
                    errorMessageTextBlock.Visibility = Visibility.Collapsed;
                    serverStatus.Visibility = Visibility.Collapsed;
                    errorMessageTextBlock.Text = string.Empty;

                    var vault = new PasswordVault();

                    //If there are any stored credentials remove them.
                    foreach (var c in vault.RetrieveAll())
                    {
                        vault.Remove(c);
                    }

                    //Store the login credentials so we don't have to pompt to login every time they launch the app.
                    vault.Add(new PasswordCredential(App.AppName, tokenBox.Text, App.TokenPassword));
                    Frame.Navigate(typeof(MainPage));
                }
                else
                {
                    errorMessageTextBlock.Text = "Failed to login. Ensure token is correct and you have an internet connection. It's possible that Discord doesn't recognize your IP address which prevents you from logging in. If this is the case they should send you an email to verify it. Additional Details: " + additionalErrorDetails;
                    errorMessageTextBlock.Visibility = Visibility.Visible;
                    serverStatus.Visibility = Visibility.Visible;
                }
            }
            catch (TaskCanceledException)
            {
                App.TryGracefulRelaunch();
            }
            catch (OperationCanceledException)
            {
                App.TryGracefulRelaunch();
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            DiscordLogin();
        }

        private void tokenBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {

            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                loginButton.Focus(FocusState.Keyboard);
                DiscordLogin();
            }

        }

        private async void Hyperlink_Click(Windows.UI.Xaml.Documents.Hyperlink sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://status.discordapp.com"));
        }
    }
}

using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Security.Credentials;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : Page
    {
        public Login()
        {
            this.InitializeComponent();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            DiscordLogin();
        }

        private void emailBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                passwordBox.Focus(FocusState.Keyboard);
            }
        }

        private void passwordBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                loginButton.Focus(FocusState.Keyboard);
                DiscordLogin();
            }
        }

        private void DiscordLogin()
        {
            try
            { 
            if (emailBox.Text == string.Empty && passwordBox.Password == string.Empty)
            {
                errorMessageTextBlock.Text = "Email and password are required to login.";
                errorMessageTextBlock.Visibility = Visibility.Visible;
                return;
            }

            if (emailBox.Text == string.Empty)
            {
                errorMessageTextBlock.Text = "Email is required to login.";
                errorMessageTextBlock.Visibility = Visibility.Visible;
                return;
            }

            if (passwordBox.Password == string.Empty)
            {
                errorMessageTextBlock.Text = "Password is required to login.";
                errorMessageTextBlock.Visibility = Visibility.Visible;
                return;
            }

            string additionalDetails = string.Empty;
            if (GnarlyClient.Instance.Connect(emailBox.Text, passwordBox.Password, out additionalDetails))
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
                vault.Add(new PasswordCredential(App.AppName, emailBox.Text, passwordBox.Password));
                Frame.Navigate(typeof(MainPage));
            }
            else
            {
                errorMessageTextBlock.Text = "Failed to login. Ensure credentials are correct and you have an internet connection. It's possible that Discord doesn't recognize your IP address which prevents you from logging in. If this is the case they should send you an email to verify it. Additional Details: " + additionalDetails; ;
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

        private async void Hyperlink_Click(Windows.UI.Xaml.Documents.Hyperlink sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://status.discordapp.com"));
        }

    }
}

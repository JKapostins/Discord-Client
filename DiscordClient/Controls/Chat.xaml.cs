using Discord;
using DiscordClient.EventArgs;
using DiscordCommon;
using System;
using System.Collections.Generic;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DiscordClient.Controls
{
    public sealed partial class Chat : UserControl
    {
        public Chat()
        {
            try
            {
                this.InitializeComponent();

                UserIconCache = new Dictionary<string, ImageSource>();
                var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
                var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
                var size = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);
                chatTextbox.MaxHeight = size.Height * 0.15; // make sure the text box can only take up 15% of the screen..
                _keyboardCapabilities = new KeyboardCapabilities();

                if (_keyboardCapabilities.KeyboardPresent == 1)
                {
                    chatTextbox.AcceptsReturn = false;
                    chatTextbox.KeyDown += chatTextbox_KeyDown;
                }

                _hasLeftTip = PurchaseManager.Instance.HasLeftTip;
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        public void ScrollToBottom()
        {
            try
            {
                chatWindowScrollView.ChangeView(0.0f, double.MaxValue, 1.0f);
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        public void CloseKeyboard()
        {
            try
            {
                chatWindow.Focus(FocusState.Programmatic);
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        public void ClearChatWindow()
        {
            try
            {
                chatWindow.Blocks.Clear();
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        public void PrintErrorMessage(string errorMessage)
        {
            try
            {
                if (errorMessage != null && errorMessage != string.Empty)
                {
                    Paragraph textParagraph = new Paragraph();
                    textParagraph.Margin = new Thickness(42, 0, 0, 0);
                    textParagraph.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 164, 142, 0));
                    textParagraph.Inlines.Add(new Run
                    {
                        Text = errorMessage
                    });

                    chatWindow.Blocks.Add(textParagraph);
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        public void ProcessMessage(Message message)
        {
            try
            {
                if (message == null || chatWindow == null)
                {
                    return;
                }

                Paragraph headerParagraph = null;
                string timeStamp = string.Empty;
                if (message.Timestamp != null)
                {
                    var dateTime = message.Timestamp.ToLocalTime();
                    if (message.Timestamp.Date.Year == 1)
                    {
                        timeStamp = "Today at " + DateTime.Now.ToString("h:mm tt");
                    }
                    else if (DateTime.Now.Date == dateTime.Date)
                    {
                        timeStamp = "Today at " + dateTime.ToString("h:mm tt");
                    }
                    else
                    {
                        timeStamp = dateTime.Date.ToString("MM/dd/yyyy");
                    }
                }

                if (message.User != null && message.User.Id != _previousId)
                {
                    headerParagraph = new Paragraph();
                    string name = string.Empty;
                    if (message.User.Name != null)
                    {
                        name = message.User.Name;
                    }

                    if (message.User.Nickname != null)
                    {
                        name = message.User.Nickname;
                    }

                    headerParagraph.Inlines.Add(new LineBreak());
                    headerParagraph.Inlines.Add(new InlineUIContainer
                    {
                        Child = new MessageHeader
                        {
                            UserIcon = GetImage(message.User.AvatarUrl),
                            UsernName = name,
                            UserNameColor = GetRoleColor(message.User),
                            TimeStamp = timeStamp
                        }
                    });

                    _previousId = message.User.Id;
                }

                Paragraph textParagraph = new Paragraph();
                textParagraph.Margin = new Thickness(42, 0, 0, 0);
                string[] hyperLinks = null;
                var inlines = ResolveLinks(ResolveMentions(message.Channel, message.RawText), out hyperLinks);
               
                foreach (var inline in inlines)
                {
                    textParagraph.Inlines.Add(inline);
                }

                //Images begin
                if(hyperLinks != null && _hasLeftTip)
                {
                    foreach (var link in hyperLinks)
                    {
                        string resolvedLink = link.Replace(".gifv", ".gif");
                        var bitmap = new BitmapImage(new Uri(resolvedLink, UriKind.Absolute));
                        bitmap.AutoPlay = false;
                        Image image = new Image();
                        image.Source = bitmap;
                        image.VerticalAlignment = VerticalAlignment.Center;
                        image.HorizontalAlignment = HorizontalAlignment.Center;
                        InlineUIContainer container = new InlineUIContainer();
                        container.Child = image;
                        textParagraph.Inlines.Add(container);
                    }
                }
                //Images end

                if (headerParagraph != null)
                {
                    chatWindow.Blocks.Add(headerParagraph);
                }

                chatWindow.Blocks.Add(textParagraph);
               

                //Use this to prevent memory overflows. 
                //GNARLY_TODO: Load older messages if user scrolls to top.
                if(chatWindow.Blocks.Count > MainPage.MaxMessageDownloadCount)
                {
                    chatWindow.Blocks.RemoveAt(0);
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private List<ImageBrush> ResolveImages(List<Inline> links)
        {
            foreach (var link in links)
            {
            }

            return null;
        }

        private SolidColorBrush GetRoleColor(Discord.User user)
        {
            SolidColorBrush brush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
            int previousRole = 0;
            foreach (var role in user.Roles)
            {
                if (role.Position > 0 && role.Position > previousRole)
                {
                    brush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, role.Color.R, role.Color.G, role.Color.B));
                    previousRole = role.Position;
                }
            }

            return brush;
        }

        List<Inline> ResolveLinks(string rawMessage, out string[] hyperLinks)
        {
            List<string> links = new List<string>();
            List<Inline> elements = new List<Inline>();
            if (rawMessage == null || rawMessage == string.Empty)
            {
                hyperLinks = null;
                return elements;
            }

            try
            {
                
                var words = rawMessage.Split(' ');
                foreach (var word in words)
                {
                    var newLines = word.Split('\n');
                    for (int i = 0; i < newLines.Length; ++i)
                    {
                        var line = newLines[i];
                        Uri uriResult = null;
                        bool result = false;
                        bool isUri = false;
                        try
                        {
                            result = Uri.TryCreate(line, UriKind.Absolute, out uriResult);
                            if (result && uriResult != null && uriResult.IsAbsoluteUri)
                            {
                                links.Add(line);
                                var link = new Hyperlink
                                {
                                    NavigateUri = uriResult
                                };
                                link.Inlines.Add(new Run { Text = line + " " });

                                elements.Add(link);
                                isUri = true;
                            }

                        }
                        catch
                        {
                            isUri = false;
                        }

                        if (isUri == false)
                        {
                            elements.Add(new Run { Text = line + " " });
                        }

                        if (i < newLines.Length - 1)
                        {
                            elements.Add(new LineBreak());
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }

            hyperLinks = links.ToArray();
            return elements;
        }


        private string ResolveMentions(Channel channel, string rawMessage)
        {
            if (channel == null && rawMessage != null)
            {
                return rawMessage;
            }
            else if (rawMessage == null)
            {
                return string.Empty;
            }

            string resolvedMessage = rawMessage;

            try
            {
                var users = channel.Users;
                if (users != null)
                {
                    foreach (var user in channel.Users)
                    {
                        if (user != null)
                        {
                            if (user.Nickname != null)
                            {
                                resolvedMessage = resolvedMessage.Replace(string.Format("<@!{0}>", user.Id), "@" + user.Nickname);
                            }
                            else if (user.Name != null)
                            {
                                resolvedMessage = resolvedMessage.Replace(string.Format("<@{0}>", user.Id), "@" + user.Name);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }

            return resolvedMessage;
        }

        private void sendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }
        private void chatTextbox_TextChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                sendMessageButton.IsEnabled = StringFromRichTextBox(chatTextbox) != string.Empty;
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private string StringFromRichTextBox(RichEditBox richText)
        {
            string textValue = string.Empty;
            try
            {
                richText.Document.GetText(Windows.UI.Text.TextGetOptions.AdjustCrlf, out textValue);
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
            return textValue;
        }

        private ImageSource GetImage(string url)
        {
            try
            {
                //Use the default image specified in the CircularImage control.
                if (url == null)
                {
                    return null;
                }

                if (UserIconCache.ContainsKey(url) == false)
                {
                    UserIconCache[url] = new BitmapImage(new Uri(url, UriKind.Absolute));
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }

            return UserIconCache[url];
        }

        private void SendMessage()
        {
            try
            {
                if (StringFromRichTextBox(chatTextbox) != string.Empty)
                {
                    OnMessageSent?.Invoke(new RawMessageEventArgs
                    {
                        Message = StringFromRichTextBox(chatTextbox)
                    });
                    chatTextbox.Document.SetText(Windows.UI.Text.TextSetOptions.None, string.Empty);
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private void chatTextbox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            var shift = Window.Current.CoreWindow.GetKeyState(VirtualKey.Shift);
            if (shift.HasFlag(CoreVirtualKeyStates.Down) && e.Key == VirtualKey.Enter)
            {
                string message = StringFromRichTextBox(chatTextbox);

                var selection = chatTextbox.Document.Selection;
                int cursorPosition = selection.StartPosition;
                message = message.Insert(cursorPosition, Environment.NewLine);
                chatTextbox.Document.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, message);
                chatTextbox.Document.Selection.StartPosition = cursorPosition + 1;
                e.Handled = true;

            }
            else if (e.Key == VirtualKey.Enter)
            {
                SendMessage();
                e.Handled = true;
            }

        }


        private Dictionary<string, ImageSource> UserIconCache;
        private bool _hasLeftTip;
        ulong _previousId = 0;
        public event ChatMessage OnMessageSent;
        private KeyboardCapabilities _keyboardCapabilities;
    }
}

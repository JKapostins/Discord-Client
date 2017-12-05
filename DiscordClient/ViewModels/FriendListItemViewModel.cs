using Discord.Controls;
using Discord.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Discord.ViewModels
{
    public class FriendListItemViewModel : NotificationBase
    {
        public FriendListItemViewModel(User user)
        {
            _user = user;
        }

        public string UserName
        {
            get
            {
                string name = _user.Name;
                try
                {
                    if (_user.Nickname != null)
                    {
                        name = _user.Nickname;
                    }
                }
                catch (Exception exception)
                {
                    App.LogUnhandledError(exception);
                }

                return name;
            }
        }

        public string GameName
        {
            get
            {
                try
                {
                    if (_user.CurrentGame != null && ((Game)_user.CurrentGame).Name != null && ((Game)_user.CurrentGame).Name.ToLower() != "null")
                    {
                        return "Playing " + ((Game)_user.CurrentGame).Name;
                    }
                }
                catch (Exception exception)
                {
                    App.LogUnhandledError(exception);
                }

                return null;
            }
        }

        public ImageSource UserIcon
        {
            get
            {
                var url = _user.AvatarUrl;
                if (url == null)
                {
                    return null;
                }

                return new BitmapImage(new Uri(url, UriKind.Absolute));
            }
        }

        public SolidColorBrush UsernameForeground
        {
            get { return GetRoleColor(_user); }
        }

        public Status UserStatus
        {
            get { return UserStatusConverter.Convert(_user.Status); }
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

        private User _user;
    }
}

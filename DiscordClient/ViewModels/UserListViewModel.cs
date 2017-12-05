using Discord;
using Discord.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordClient.ViewModels
{
    public class UserListViewModel : NotificationBase
    {

        public UserListViewModel(Channel channel)
        {
            try
            {
                OfflineUsers = new ObservableCollection<FriendListItemViewModel>();
                OnlineUsers = new ObservableCollection<FriendListItemViewModel>();

                foreach (var user in channel.Users)
                {
                    if (user.Status == UserStatus.Offline)
                    {
                        OfflineUsers.Add(new FriendListItemViewModel(user));
                    }
                    else
                    {
                        OnlineUsers.Add(new FriendListItemViewModel(user));
                    }
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        public ObservableCollection<FriendListItemViewModel> OfflineUsers { get; set; }
        public ObservableCollection<FriendListItemViewModel> OnlineUsers { get; set; }
    }
}

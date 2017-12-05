using Discord.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Discord.Converters
{
    //This is only used for code behind so doesn't need to be IValueConverter
    class UserStatusConverter 
    {
        public static Status Convert(UserStatus value)
        {
            if (value == null)
            {
                return Status.Offline;
            }

            switch (((UserStatus)value).Value)
            {
                case null:
                    return Status.Offline;
                case "online":
                    return Status.Active;
                case "idle":
                    return Status.Away;
                case "offline":
                    return Status.Offline;
            }
            return Status.Offline;
        }
    }
}

using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace DiscordCommon
{
    public class ToastAssistant
    {
        public static ToastAssistant Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ToastAssistant();
                }
                return _instance;
            }
        }

        public void SendGenericToast(string icon, string title, string content, QueryString launchQuery = null, double experationInHours = 24.0)
        {
            if (icon == null)
            {
                icon = "Assets/DiscordLogoSquare.png";
            }

            // Construct the visuals of the toast
            ToastVisual visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = title
                        },

                        new AdaptiveText()
                        {
                            Text = content
                        },
                    },

                    AppLogoOverride = new ToastGenericAppLogo()
                    {
                        Source = icon,
                        HintCrop = ToastGenericAppLogoCrop.Circle
                    }
                }
            };

            ToastContent toastContent = new ToastContent()
            {
                Visual = visual,
            };
            
            if(launchQuery != null)
            {
                toastContent.Launch = launchQuery.ToString();
            }

            var toast = new ToastNotification(toastContent.GetXml());
            toast.ExpirationTime = DateTime.Now.AddHours(experationInHours);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        public const string LaunchAction = "Action";
        public const string GoToTrello = "GoToTrello";
        public const string GoToDonationPage = "GoToDonationPage";
        private ToastAssistant()
        {
        }

        private static ToastAssistant _instance;
    }
}

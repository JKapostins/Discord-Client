using Microsoft.QueryStringDotNET;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Store;
using Windows.UI.Core;
using Windows.UI.Popups;

namespace DiscordCommon
{
    public class PurchaseManager
    {
        public static PurchaseManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PurchaseManager();
                }
                return _instance;
            }
        }

        public async Task LeaveOneDollarTip()
        {
            await PurchaseHelper(OneDollarPurchase);
            return;
        }

        public async Task LeaveFiveDollarTip()
        {
            await PurchaseHelper(FiveDollarPurchase);
            return;
        }

        public async Task LeaveTenDollarTip()
        {
            await PurchaseHelper(TenDollarPurchase);
            return;
        }

        public bool HasLeftTip
        {
            get
            {
#if FAKE_DONATION
                return true;
#else
                if(_licenseInformation == null)
                {
                    return true;
                }
                else
                {
                    return _licenseInformation.ProductLicenses[OneDollarPurchase].IsActive
                    || _licenseInformation.ProductLicenses[FiveDollarPurchase].IsActive
                    || _licenseInformation.ProductLicenses[TenDollarPurchase].IsActive;
                }
                
#endif
            }
        }

        private PurchaseManager()
        {
            try
            {
                _licenseInformation = CurrentApp.LicenseInformation;
            }
            catch
            {
                //GNARLY_TODO: we need to switch to the new purchase system.
            }
        }

        private async Task PurchaseHelper(string productId)
        {
            if (_licenseInformation != null)
            {
                if (!_licenseInformation.ProductLicenses[productId].IsActive)
                {
                    try
                    {
                        var results = await CurrentApp.RequestProductPurchaseAsync(productId);

                        //Check the license state to determine if the in-app purchase was successful.
                        if (results.Status == ProductPurchaseStatus.Succeeded)
                        {
                            var launchQuery = new QueryString()
                        {
                            { ToastAssistant.LaunchAction, ToastAssistant.GoToTrello }
                        };
                            ToastAssistant.Instance.SendGenericToast("Assets/Developer.png", "Jake (Developer)", "Thank you for your donation! You have been granted \"Early Access\" to new features. Track development progress on trello.", launchQuery, 168.0);
                        }

                    }
                    catch (Exception)
                    {
                        var launchQuery = new QueryString()
                        {
                            { ToastAssistant.LaunchAction, ToastAssistant.GoToDonationPage }
                        };
                        ToastAssistant.Instance.SendGenericToast("Assets/DiscordLogoSquare.png", "Donation Failed", "An unexpected error occured during donation. Please try again.", launchQuery, 24.0);
                    }
                }
            }
            else
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                            async () =>
                            {
                                var popup = new MessageDialog(
                                    "Something went wrong on our end. The information has been sent to the developer and you have been granted early access until this issue is resolved.",
                                    "Donation Failed");
                                await popup.ShowAsync();

                                //GNARLY_TODO: Log this to the server.
                            });
            }

            return;
        }

        private LicenseInformation _licenseInformation;

        private static PurchaseManager _instance;
        private static readonly string OneDollarPurchase = "TipOneDollar";
        private static readonly string FiveDollarPurchase = "TipFiveDollars";
        private static readonly string TenDollarPurchase = "TipTenDollars";
    }
}

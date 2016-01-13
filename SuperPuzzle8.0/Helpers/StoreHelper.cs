using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Store = Windows.ApplicationModel.Store;

namespace PhotoPuzzle.Helpers
{
    public class StoreHelper
    {
        public static bool IsProductActive(string productId)
        {
            try
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    return Store.CurrentApp.LicenseInformation.ProductLicenses[productId].IsActive ? true : false;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static async void Donate(string productId, bool isConsurable = false)
        {
            try
            {
                PopupMessage popupMessage = new PopupMessage();
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    var listing = await CurrentApp.LoadListingInformationAsync();
                    var myProduct = listing.ProductListings.FirstOrDefault(p => p.Value.ProductId == productId);
                    if (!CurrentApp.LicenseInformation.ProductLicenses[myProduct.Value.ProductId].IsActive)
                    {
                        await CurrentApp.RequestProductPurchaseAsync(productId, false);
                        var productLicenses = CurrentApp.LicenseInformation.ProductLicenses;
                        ProductLicense tokenLicense = productLicenses[productId];
                        if (tokenLicense.IsActive)
                        {
                            if (isConsurable)//если продукт расходуемый
                            {
                                CurrentApp.ReportProductFulfillment(productId);
                            }
                            var settings = IsolatedStorageSettings.ApplicationSettings;
                            string product = "product" + productId;
                            if (!settings.Contains(product))
                            {
                                settings.Add(product, true);
                                settings.Save();
                            }
                            //popupMessage.Show("Thank you for your purchase!");//en
                            //popupMessage.Show(AppResources.ThankYouForPurchase);//it
                            popupMessage.Show("Thank you for your purchase!");
                        }
                    }
                    else
                    {
                        //popupMessage.Show("You have already bought this product!");//en
                        //popupMessage.Show(AppResources.ProductExitsts);//it
                        popupMessage.Show("You have already bought this product!");
                    }
                }
                else
                {
                    //popupMessage.Show("Пожалуйста, проверьте подключение к сети Интернет!");//ru
                    //popupMessage.Show("Please check the connection to the Internet!");//en
                    //popupMessage.Show(AppResources.NoInternet);//it
                    popupMessage.Show("Please check the connection to the Internet!");
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}

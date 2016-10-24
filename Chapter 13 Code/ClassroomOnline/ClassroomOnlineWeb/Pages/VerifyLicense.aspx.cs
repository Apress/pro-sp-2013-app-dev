using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;
using ClassroomOnlineWeb.OfficeStoreVerificationService;

namespace ClassroomOnlineWeb.Pages
{
    public partial class VerifyLicense : System.Web.UI.Page
    {
        private Guid _productID = Guid.Parse("{9447ab8b-ae57-471b-a158-34c334b283e5}");

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var contextToken = TokenHelper.GetContextTokenFromRequest(Page.Request);
                var hostWeb = Page.Request["SPHostUrl"];
                var appWeb = Page.Request["SPAppWebUrl"];

                using (var clientContext = TokenHelper.GetClientContextWithContextToken(hostWeb, contextToken, Request.Url.Authority))
                {
                    // Retrieve any app licenses stored within SharePoint, most RELEVANT first.
                    ClientResult<AppLicenseCollection> licenses =
                        Microsoft.SharePoint.Client.Utilities.Utility.GetAppLicenseInformation(clientContext, _productID);
                    clientContext.ExecuteQuery();

                    if (licenses.Value.Count == 0)
                        throw new InvalidOperationException("No license");

                    string licenseString = licenses.Value[0].RawXMLLicenseToken;

                    // Validate the license against the Office Store license service.
                    VerificationServiceClient service = new VerificationServiceClient();

                    VerifyEntitlementTokenRequest request = new VerifyEntitlementTokenRequest();
                    request.EntitlementToken = licenseString;

                    VerifyEntitlementTokenResponse storeLicense = service.VerifyEntitlementToken(request);

                    // Perform licensing checks.
                    //
                    // Use this version of the check in production:
                    //     if (!storeLicense.IsValid || storeLicense.IsExpired)
                    //
                    // Use this version of the check in test/debugging:
                    if ((!storeLicense.IsValid && !storeLicense.IsTest) || storeLicense.IsExpired)
                        throw new InvalidOperationException("Invalid license");

                    // At this point, we have a valid license. We can retrieve the details
                    // of the license (type of license, acquisition date, etc.) from the
                    // license provided by the Office Store and take appropriate actions.

                    // <--TBD-->

                    Response.Write("Licensing Succeeded: " + storeLicense.EntitlementType.ToString());
                }
            }
            catch (Exception ex)
            {
                Response.Write("Licensing failed: " + ex.Message);
            }

        }
    }
}
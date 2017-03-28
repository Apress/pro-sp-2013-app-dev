using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CreateTestAppLicenseWeb.Pages
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var contextToken = TokenHelper.GetContextTokenFromRequest(Page.Request);
            var hostWeb = Page.Request["SPHostUrl"];

            using (var clientContext = TokenHelper.GetClientContextWithContextToken(hostWeb, contextToken, Request.Url.Authority))
            {
                string tokenXml =
                    "<r v=\"0\">"
                    + "<t aid=\"WA103524926\""
                    + " did=\"{3F47392A-2308-4FC6-BF24-740626612B26}\""
                    + " ad=\"2012-06-19T21:48:56Z\""
                    + " te=\"2112-07-15T23:47:42Z\""
                    + " sd=\"2012-02-01T23:47:42Z\""
                    + " test=\"true\""
                    + " pid=\"{9447ab8b-ae57-471b-a158-34c334b283e5}\""
                    + " et=\"Paid\""
                    + " cid=\"739835AE59FDE73E\""
                    + " ts=\"0\""
                    + " sl=\"true\" />"
                    + "<d>449JFz+my0wNoCm0/h+Ci9DsF/W0Q8rqEBqjpe44KkY=</d>"
                    + "</r>";

                Microsoft.SharePoint.Client.Utilities.Utility.ImportAppLicense(
                    clientContext,
                    tokenXml,
                    "en-US",
                    "US",
                    "Classroom Online",
                    Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.LastIndexOf("/")) + "/AppIcon.png",
                    "Pro SharePoint 2013 Apps",
                    5); // SharePoint app with no Office integration

                clientContext.ExecuteQuery();

                Response.Write("License Import succeeded!");
            }
        }
    }
}
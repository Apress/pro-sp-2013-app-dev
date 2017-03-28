using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;

namespace Exercise7_3Web.Pages
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // This copies the context token that is originally posted to the page into a 
            // hidden field on the form so that it is available during event handler
            // routines executed during a postback.
            if (!IsPostBack)
                SPAppToken.Value = TokenHelper.GetContextTokenFromRequest(Request);
        }

        protected void cmdRunRequest_Click(object sender, EventArgs e)
        {
            // For testing only!
            // Remove this line before releasing to production.
            TokenHelper.TrustAllCertificates();
            
            Uri appWebUrl = new Uri(Request.QueryString["SPAppWebUrl"]);

            // The context token is retreived from the SPAppToken field 
            // in TokenHelper.GetContextTokenFromRequest().
            SharePointContextToken contextToken =
                    TokenHelper.ReadAndValidateContextToken(
                                    TokenHelper.GetContextTokenFromRequest(Request),
                                    Request.Url.Authority);

            string accessToken =
                    TokenHelper.GetAccessToken(contextToken, appWebUrl.Authority).AccessToken;

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(Request.QueryString["SPAppWebUrl"] + txtREST.Text);
            req.Method = "GET";
            req.Accept = ddlFormat.SelectedValue;
            req.Headers.Add("Authorization", "Bearer " + accessToken);

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            lblStatus.Text = resp.StatusDescription;

            StreamReader st = new StreamReader(resp.GetResponseStream());
            lblOutput.Text = WebUtility.HtmlEncode(st.ReadToEnd());
        }
    }
}
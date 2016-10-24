using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Data.Services.Client;

namespace UnderhillIntegration
{
    public partial class TestPage : System.Web.UI.Page
    {
        protected void cmdGo_Click(object sender, EventArgs e)
        {
            HttpWebRequest req =
                (HttpWebRequest)HttpWebRequest.Create("http://localhost:26386/GradeService.svc/Grades");

            req.Method = "GET";  // "POST";
            req.Accept = "application/json;odata=verbose";

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            lblStatus.Text = resp.StatusDescription;

            StreamReader st = new StreamReader(resp.GetResponseStream());
            lblResponse.Text = WebUtility.HtmlEncode(st.ReadToEnd());
        }
    }
}
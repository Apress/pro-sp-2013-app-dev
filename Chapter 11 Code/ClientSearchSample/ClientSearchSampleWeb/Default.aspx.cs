using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Search;
using Microsoft.SharePoint.Client.Search.Query;

namespace ClientSearchSampleWeb
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
                SPAppToken.Value = TokenHelper.GetContextTokenFromRequest(Page.Request);
        }

        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            string contextToken = SPAppToken.Value;
            string hostWeb = Page.Request["SPHostUrl"];

            ClientContext ctx = TokenHelper.GetClientContextWithContextToken(hostWeb, contextToken, Request.Url.Authority);
            KeywordQuery keywordQuery = new KeywordQuery(ctx);
            keywordQuery.QueryText = txtSearchText.Text;

            SearchExecutor searchExecutor = new SearchExecutor(ctx);
            ClientResult<ResultTableCollection> results = searchExecutor.ExecuteQuery(keywordQuery);
            ctx.ExecuteQuery();

            DataTable dt = new DataTable();
            dt.Columns.Add("Title");
            dt.Columns.Add("Author");
            dt.Columns.Add("Last Update");
            dt.Columns.Add("URL");

            if (results.Value.Count > 0)
                foreach (Dictionary<String, Object> resultRow in results.Value[0].ResultRows)
                    dt.Rows.Add(resultRow["Title"], resultRow["Author"], resultRow["Write"], resultRow["Path"]);

            grd.DataSource = dt;
            grd.DataBind();
        }
    }
}
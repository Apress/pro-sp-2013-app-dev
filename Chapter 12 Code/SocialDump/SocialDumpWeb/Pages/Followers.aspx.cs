using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Social;

namespace SocialDumpWeb.Pages
{
    public partial class Followers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var contextToken = TokenHelper.GetContextTokenFromRequest(Page.Request);
            var hostWeb = Page.Request["SPHostUrl"];

            using (var clientContext = TokenHelper.GetClientContextWithContextToken(hostWeb, contextToken, Request.Url.Authority))
            {
                SocialFollowingManager m = new SocialFollowingManager(clientContext);
                ClientResult<SocialActor[]> f1 = m.GetFollowed(SocialActorTypes.All);
                ClientResult<SocialActor[]> f2 = m.GetFollowers();
                ClientResult<SocialActor[]> f3 = m.GetSuggestions();

                clientContext.ExecuteQuery();

                DumpList("Followed List", f1.Value);
                DumpList("Follower List", f2.Value);
                DumpList("Suggestion List", f3.Value);
            }
        }

        private void DumpList(string hdr, SocialActor[] lst)
        {
            Response.Write("<h2>" + hdr + "</h2>");
            Response.Write("<ul>");
            foreach (SocialActor actor in lst)
                Response.Write(string.Format("<li>{0}</li>", FormatActor(actor)));
            Response.Write("</ul>");
        }

        private string FormatActor(SocialActor actor)
        {
            return string.Format("{0}: {1}", actor.ActorType, actor.Name);
        }
    }
}
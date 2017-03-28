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
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var contextToken = TokenHelper.GetContextTokenFromRequest(Page.Request);
            var hostWeb = Page.Request["SPHostUrl"];

            using (var clientContext = TokenHelper.GetClientContextWithContextToken(hostWeb, contextToken, Request.Url.Authority))
            {
                SocialFeedManager feedManager = new SocialFeedManager(clientContext);
                clientContext.Load(feedManager);

                SocialFeedOptions feedOptions = new SocialFeedOptions();
                ClientResult<SocialFeed> personalFeed = feedManager.GetFeed(SocialFeedType.Personal, feedOptions);
                ClientResult<SocialFeed> siteFeed = feedManager.GetFeedFor("https://swrightjet.sharepoint.com/ex2", feedOptions);
                ClientResult<SocialFeed> userFeed = feedManager.GetFeedFor("swrightjet@swrightjet.onmicrosoft.com", feedOptions);
                clientContext.ExecuteQuery();

                DumpFeed("Personal Feed", personalFeed.Value);
                DumpFeed("Site Feed", siteFeed.Value);
                DumpFeed("User Feed", userFeed.Value);
            }
        }

        private void DumpFeed(string hdr, SocialFeed feed)
        {
            Response.Write("<h2>" + hdr + "</h2>");
            foreach (SocialThread thread in feed.Threads)
            {
                Response.Write(
                    string.Format("{0} (Owner = {1})<ul>",
                                  thread.ThreadType,
                                  FormatActor(thread.Actors[thread.OwnerIndex])));

                Response.Write(
                    string.Format("<li>{0}: {1} (Author = {2})</li><ul>",
                                  thread.RootPost.PostType,
                                  thread.RootPost.Text,
                                  FormatActor(thread.Actors[thread.RootPost.AuthorIndex])));

                foreach (SocialPost reply in thread.Replies)
                    Response.Write(
                        string.Format("<li>{0}: {1} (Author = {2})</li>",
                                      reply.PostType,
                                      reply.Text,
                                      FormatActor(thread.Actors[reply.AuthorIndex])));
                Response.Write("</ul></ul>");
            }
        }

        private string FormatActor(SocialActor actor)
        {
            return string.Format("{0}: {1}", actor.ActorType, actor.Name);
        }
    }
}
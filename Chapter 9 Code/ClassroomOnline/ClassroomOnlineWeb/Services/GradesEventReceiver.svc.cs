using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;

namespace ClassroomOnlineWeb.Services
{
    public class GradesEventReceiver : IRemoteEventService
    {
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            LogAuditRecord(properties);
            return new SPRemoteEventResult();
        }

        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            LogAuditRecord(properties);
        }

        private void LogAuditRecord(SPRemoteEventProperties properties)
        {
            using (ClientContext clientContext = TokenHelper.CreateRemoteEventReceiverClientContext(properties))
            {
                if (clientContext != null)
                {
                    // Retrieve the user that made the change
                    User user = clientContext.Web.SiteUsers.GetById(properties.ItemEventProperties.CurrentUserId);
                    clientContext.Load(user);
                    clientContext.ExecuteQuery();
                    string userName = user.Title; // User's Name

                    // Format ID and attributes for auditing
                    StringBuilder sb =
                        new StringBuilder("'ListItemID':'" + properties.ItemEventProperties.ListItemId.ToString() + "',");
                    if (properties.ItemEventProperties.AfterProperties != null)
                    {
                        Dictionary<string, object> attrs = properties.ItemEventProperties.AfterProperties;
                        foreach (string key in attrs.Keys)
                            sb.AppendFormat("'{0}':'{1}',", key, (attrs[key] != null) ? attrs[key] : "null");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    string auditData = "{" + sb.ToString() + "}";

                    // Create a new entry in the audit trail
                    List gradeHistory = clientContext.Web.Lists.GetByTitle("GradeHistory");
                    ListItemCreationInformation ici = new ListItemCreationInformation();
                    ListItem newItem = gradeHistory.AddItem(ici);
                    newItem["Title"] = properties.EventType.ToString();
                    newItem["EnteredDate"] = DateTime.Now;
                    newItem["EnteredBy"] = userName;
                    newItem["AuditData"] = auditData;
                    newItem.Update();

                    // is there a lag in deploying the service?
                    clientContext.ExecuteQuery();
                }
            }
        }
    }
}

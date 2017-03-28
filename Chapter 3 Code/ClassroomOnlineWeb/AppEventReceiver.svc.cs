using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;

namespace ClassroomOnlineWeb
{
    public class AppEventReceiver : IRemoteEventService
    {
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            switch (properties.EventType)
            {
                case SPRemoteEventType.AppInstalled:
                    return ProcessInstallation(properties);
                case SPRemoteEventType.AppUpgraded:
                    return ProcessUpgraded(properties);
                case SPRemoteEventType.AppUninstalling:
                    return ProcessUninstalling(properties);
            }

            return new SPRemoteEventResult();
        }

        public SPRemoteEventResult ProcessInstallation(SPRemoteEventProperties properties)
        {
            SPRemoteEventResult result = new SPRemoteEventResult();

            using (ClientContext clientContext = TokenHelper.CreateAppEventClientContext(properties, true))
            {
                if (clientContext != null)
                {
                    WebInformation parentWeb = clientContext.Web.ParentWeb;
                    clientContext.Load(parentWeb);
                    clientContext.ExecuteQuery();

                    List lst = clientContext.Web.Lists.GetByTitle("Configuration Values");
                    ListItem item = lst.AddItem(new ListItemCreationInformation());
                    item["Title"] = "AppInstalled";
                    item["Value"] = (parentWeb.WebTemplate == "APPCATALOG") ? "Tenant Installation" : "Web Installation";
                    item.Update();
                    clientContext.ExecuteQuery();
                }
            }

            return result;
        }

        private SPRemoteEventResult ProcessUpgraded(SPRemoteEventProperties properties)
        {
            SPRemoteEventResult result = new SPRemoteEventResult();

            using (ClientContext clientContext = TokenHelper.CreateAppEventClientContext(properties, true))
            {
                if (clientContext != null)
                {
                    List lst = clientContext.Web.Lists.GetByTitle("Configuration Values");
                    ListItem item = lst.AddItem(new ListItemCreationInformation());
                    item["Title"] = "AppUpgraded";
                    item["Value"] = "Upgraded " + properties.AppEventProperties.PreviousVersion.ToString()
                                    + " to " + properties.AppEventProperties.Version.ToString();
                    item.Update();
                    clientContext.ExecuteQuery();
                }
            }

            return result;
        }

        private SPRemoteEventResult ProcessUninstalling(SPRemoteEventProperties properties)
        {
            SPRemoteEventResult result = new SPRemoteEventResult();

            using (ClientContext clientContext = TokenHelper.CreateAppEventClientContext(properties, true))
            {
                if (clientContext != null)
                {
                    // This is pretty pointless since the list we are updating is about to be destroyed!
                    // In reality, we would want to record this somewhere else.
                    List lst = clientContext.Web.Lists.GetByTitle("Configuration Values");
                    ListItem item = lst.AddItem(new ListItemCreationInformation());
                    item["Title"] = "AppUninstalling";
                    item["Value"] = "going away";
                    item.Update();
                    clientContext.ExecuteQuery();
                }
            }

            return result;
        }

        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            // This method is not used by app events
        }
    }
}

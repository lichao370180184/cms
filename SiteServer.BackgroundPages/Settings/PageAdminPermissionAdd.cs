using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageAdminPermissionAdd : BasePageCms
    {
        public PlaceHolder PhWebsiteSysPermissions;
        public CheckBoxList CblWebsiteSysPermissions;
        public PlaceHolder PhWebsitePluginPermissions;
        public CheckBoxList CblWebsitePluginPermissions;

        public PlaceHolder PhChannelPermissions;
        public CheckBoxList CblChannelPermissions;
        public Literal LtlNodeTree;

        public static string GetRedirectUrl(int siteId, string roleName)
        {
            var queryString = new NameValueCollection { { "SiteId", siteId.ToString() } };
            if (!string.IsNullOrEmpty(roleName))
            {
                queryString.Add("RoleName", roleName);
            }

            return PageUtils.GetSettingsUrl(nameof(PageAdminPermissionAdd), queryString);
        }

        private string GetNodeTreeHtml()
        {
            var htmlBuilder = new StringBuilder();
            var systemPermissionsInfoList = Session[PageAdminRoleAdd.SystemPermissionsInfoListKey] as List<SitePermissionsInfo>;
            if (systemPermissionsInfoList == null)
            {
                PageUtils.RedirectToErrorPage("超出时间范围，请重新进入！");
                return string.Empty;
            }
            var channelIdList = new List<int>();
            foreach (var systemPermissionsInfo in systemPermissionsInfoList)
            {
                channelIdList.AddRange(TranslateUtils.StringCollectionToIntList(systemPermissionsInfo.ChannelIdCollection));
            }

            var treeDirectoryUrl = SiteServerAssets.GetIconUrl("tree");

            htmlBuilder.Append("<span id='ChannelSelectControl'>");
            var theChannelIdList = ChannelManager.GetChannelIdList(SiteId);
            var isLastNodeArray = new bool[theChannelIdList.Count];
            foreach (var theChannelId in theChannelIdList)
            {
                var nodeInfo = ChannelManager.GetChannelInfo(SiteId, theChannelId);
                htmlBuilder.Append(GetTitle(nodeInfo, treeDirectoryUrl, isLastNodeArray, channelIdList));
                htmlBuilder.Append("<br/>");
            }
            htmlBuilder.Append("</span>");
            return htmlBuilder.ToString();
        }

        private string GetTitle(ChannelInfo nodeInfo, string treeDirectoryUrl, IList<bool> isLastNodeArray, ICollection<int> channelIdList)
        {
            var itemBuilder = new StringBuilder();
            if (nodeInfo.Id == SiteId)
            {
                nodeInfo.IsLastNode = true;
            }
            if (nodeInfo.IsLastNode == false)
            {
                isLastNodeArray[nodeInfo.ParentsCount] = false;
            }
            else
            {
                isLastNodeArray[nodeInfo.ParentsCount] = true;
            }
            for (var i = 0; i < nodeInfo.ParentsCount; i++)
            {
                itemBuilder.Append($"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_empty.gif\"/>");
            }
            if (nodeInfo.IsLastNode)
            {
                itemBuilder.Append(nodeInfo.ChildrenCount > 0
                    ? $"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/minus.png\"/>"
                    : $"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_empty.gif\"/>");
            }
            else
            {
                itemBuilder.Append(nodeInfo.ChildrenCount > 0
                    ? $"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/minus.png\"/>"
                    : $"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_empty.gif\"/>");
            }

            var check = "";
            if (channelIdList.Contains(nodeInfo.Id))
            {
                check = "checked";
            }

            var disabled = "";
            if (!IsOwningChannelId(nodeInfo.Id))
            {
                disabled = "disabled";
                check = "";
            }

            itemBuilder.Append($@"
<span class=""checkbox checkbox-primary"" style=""padding-left: 0px;"">
    <input type=""checkbox"" id=""ChannelIdCollection_{nodeInfo.Id}"" name=""ChannelIdCollection"" value=""{nodeInfo.Id}"" {check} {disabled}/>
    <label for=""ChannelIdCollection_{nodeInfo.Id}""> {nodeInfo.ChannelName} &nbsp;<span style=""font-size:8pt;font-family:arial"" class=""gray"">({nodeInfo.ContentNum})</span></label>
</span>
");

            return itemBuilder.ToString();
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            LtlNodeTree.Text = GetNodeTreeHtml();

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Admin);

            if (AuthRequest.AdminPermissions.IsSystemAdministrator)
            {
                var channelPermissions = PermissionConfigManager.Instance.ChannelPermissions;
                foreach (var permission in channelPermissions)
                {
                    if (permission.Name == ConfigManager.ChannelPermissions.ContentCheckLevel1)
                    {
                        if (SiteInfo.Additional.IsCheckContentLevel)
                        {
                            if (SiteInfo.Additional.CheckContentLevel < 1)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (permission.Name == ConfigManager.ChannelPermissions.ContentCheckLevel2)
                    {
                        if (SiteInfo.Additional.IsCheckContentLevel)
                        {
                            if (SiteInfo.Additional.CheckContentLevel < 2)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (permission.Name == ConfigManager.ChannelPermissions.ContentCheckLevel3)
                    {
                        if (SiteInfo.Additional.IsCheckContentLevel)
                        {
                            if (SiteInfo.Additional.CheckContentLevel < 3)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (permission.Name == ConfigManager.ChannelPermissions.ContentCheckLevel4)
                    {
                        if (SiteInfo.Additional.IsCheckContentLevel)
                        {
                            if (SiteInfo.Additional.CheckContentLevel < 4)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (permission.Name == ConfigManager.ChannelPermissions.ContentCheckLevel5)
                    {
                        if (SiteInfo.Additional.IsCheckContentLevel)
                        {
                            if (SiteInfo.Additional.CheckContentLevel < 5)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    var listItem = new ListItem(permission.Text, permission.Name);
                    CblChannelPermissions.Items.Add(listItem);
                }
            }
            else
            {
                PhChannelPermissions.Visible = false;
                var channelPermissions = AuthRequest.AdminPermissions.GetChannelPermissions(SiteId);
                foreach (var channelPermission in channelPermissions)
                {
                    foreach (var permission in PermissionConfigManager.Instance.ChannelPermissions)
                    {
                        if (permission.Name == channelPermission)
                        {
                            if (channelPermission == ConfigManager.ChannelPermissions.ContentCheck)
                            {
                                if (SiteInfo.Additional.IsCheckContentLevel) continue;
                            }
                            else if (channelPermission == ConfigManager.ChannelPermissions.ContentCheckLevel1)
                            {
                                if (SiteInfo.Additional.IsCheckContentLevel == false || SiteInfo.Additional.CheckContentLevel < 1) continue;
                            }
                            else if (channelPermission == ConfigManager.ChannelPermissions.ContentCheckLevel2)
                            {
                                if (SiteInfo.Additional.IsCheckContentLevel == false || SiteInfo.Additional.CheckContentLevel < 2) continue;
                            }
                            else if (channelPermission == ConfigManager.ChannelPermissions.ContentCheckLevel3)
                            {
                                if (SiteInfo.Additional.IsCheckContentLevel == false || SiteInfo.Additional.CheckContentLevel < 3) continue;
                            }
                            else if (channelPermission == ConfigManager.ChannelPermissions.ContentCheckLevel4)
                            {
                                if (SiteInfo.Additional.IsCheckContentLevel == false || SiteInfo.Additional.CheckContentLevel < 4) continue;
                            }
                            else if (channelPermission == ConfigManager.ChannelPermissions.ContentCheckLevel5)
                            {
                                if (SiteInfo.Additional.IsCheckContentLevel == false || SiteInfo.Additional.CheckContentLevel < 5) continue;
                            }

                            PhChannelPermissions.Visible = true;
                            var listItem = new ListItem(permission.Text, permission.Name);
                            CblChannelPermissions.Items.Add(listItem);
                        }
                    }
                }
            }

            if (AuthRequest.AdminPermissions.IsSystemAdministrator)
            {
                foreach (var permission in PermissionConfigManager.Instance.WebsiteSysPermissions)
                {
                    var listItem = new ListItem(permission.Text, permission.Name);
                    CblWebsiteSysPermissions.Items.Add(listItem);
                }

                foreach (var permission in PermissionConfigManager.Instance.WebsitePluginPermissions)
                {
                    var listItem = new ListItem(permission.Text, permission.Name);
                    CblWebsitePluginPermissions.Items.Add(listItem);
                }
            }
            else
            {
                PhWebsiteSysPermissions.Visible = PhWebsitePluginPermissions.Visible = false;
                if (AuthRequest.AdminPermissions.HasSitePermissions(SiteId))
                {
                    var websitePermissionList = AuthRequest.AdminPermissions.GetSitePermissions(SiteId);
                    foreach (var websitePermission in websitePermissionList)
                    {
                        foreach (var permission in PermissionConfigManager.Instance.WebsiteSysPermissions)
                        {
                            if (permission.Name == websitePermission)
                            {
                                PhWebsiteSysPermissions.Visible = true;
                                var listItem = new ListItem(permission.Text, permission.Name);
                                CblWebsiteSysPermissions.Items.Add(listItem);
                            }
                        }
                        foreach (var permission in PermissionConfigManager.Instance.WebsitePluginPermissions)
                        {
                            if (permission.Name == websitePermission)
                            {
                                PhWebsitePluginPermissions.Visible = true;
                                var listItem = new ListItem(permission.Text, permission.Name);
                                CblWebsitePluginPermissions.Items.Add(listItem);
                            }
                        }
                    }
                }
            }

            var systemPermissionsInfoList = Session[PageAdminRoleAdd.SystemPermissionsInfoListKey] as List<SitePermissionsInfo>;
            if (systemPermissionsInfoList != null)
            {
                SitePermissionsInfo systemPermissionsInfo = null;
                foreach (var sitePermissionsInfo in systemPermissionsInfoList)
                {
                    if (sitePermissionsInfo.SiteId == SiteId)
                    {
                        systemPermissionsInfo = sitePermissionsInfo;
                        break;
                    }
                }
                if (systemPermissionsInfo == null) return;

                foreach (ListItem item in CblChannelPermissions.Items)
                {
                    item.Selected = StringUtils.In(systemPermissionsInfo.ChannelPermissions, item.Value);
                }
                foreach (ListItem item in CblWebsiteSysPermissions.Items)
                {
                    item.Selected = StringUtils.In(systemPermissionsInfo.WebsitePermissions, item.Value);
                }
                foreach (ListItem item in CblWebsitePluginPermissions.Items)
                {
                    item.Selected = StringUtils.In(systemPermissionsInfo.WebsitePermissions, item.Value);
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var systemPermissionsInfoList = Session[PageAdminRoleAdd.SystemPermissionsInfoListKey] as List<SitePermissionsInfo>;
            if (systemPermissionsInfoList != null)
            {
                var systemPermissionlist = new List<SitePermissionsInfo>();
                foreach (var systemPermissionsInfo in systemPermissionsInfoList)
                {
                    if (systemPermissionsInfo.SiteId == SiteId)
                    {
                        continue;
                    }
                    systemPermissionlist.Add(systemPermissionsInfo);
                }

                var channelIdList = TranslateUtils.StringCollectionToStringList(Request.Form["ChannelIdCollection"]);
                if (channelIdList.Count > 0 && CblChannelPermissions.SelectedItem != null ||
                    CblWebsiteSysPermissions.SelectedItem != null || CblWebsitePluginPermissions.SelectedItem != null)
                {
                    var websiteSysPermissions =
                        ControlUtils.SelectedItemsValueToStringCollection(CblWebsiteSysPermissions.Items);
                    var websitePluginPermissions =
                        ControlUtils.SelectedItemsValueToStringCollection(CblWebsitePluginPermissions.Items);
                    var systemPermissionsInfo = new SitePermissionsInfo
                    {
                        SiteId = SiteId,
                        ChannelIdCollection = TranslateUtils.ObjectCollectionToString(channelIdList),
                        ChannelPermissions =
                            ControlUtils.SelectedItemsValueToStringCollection(CblChannelPermissions.Items),
                        WebsitePermissions =
                            websiteSysPermissions + (string.IsNullOrEmpty(websitePluginPermissions) ? string.Empty : "," + websitePluginPermissions)
                    };

                    systemPermissionlist.Add(systemPermissionsInfo);
                }

                Session[PageAdminRoleAdd.SystemPermissionsInfoListKey] = systemPermissionlist;
            }

            PageUtils.Redirect(PageAdminRoleAdd.GetReturnRedirectUrl(AuthRequest.GetQueryString("RoleName")));
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageAdminRoleAdd.GetReturnRedirectUrl(AuthRequest.GetQueryString("RoleName")));
        }
    }
}

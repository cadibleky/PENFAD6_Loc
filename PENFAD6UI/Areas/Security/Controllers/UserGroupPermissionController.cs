
using PENFAD6DAL.Repository.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.GlobalObject;
using System.Data;

namespace PENFAD6UI.Areas.Security.Controllers
{
    public class UserGroupPermissionController : Controller
    {
        // GET: Security/UserGroupPermission

        private sec_UserRoleRepo repo_role = new sec_UserRoleRepo();
        private sec_UserGroupRepo repo_groupc = new sec_UserGroupRepo();
        private cSecurityRepo sec = new cSecurityRepo();
        private sec_UserGroupPermissionRepo sec_grantpermission = new sec_UserGroupPermissionRepo();


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddUserGroupPermissionTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "UserGroupPermissionPartial",
                //Model = repo_groupc.GetUserGroupList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }

        public ActionResult GetRoles()
        {
            try
            {
                return this.Store(repo_role.GetUserRoleList());
            }
            catch (System.Exception)
            {

                throw;
            }

        }


        public ActionResult GetGroupsByRoleId(string UserRoleId)
        {
            try
            {
                return this.Store(repo_groupc.GetUserGroupForRoleIdList_Dataset(UserRoleId));
            }
            catch (System.Exception)
            {

                throw;
            }

        }


        public ActionResult Load_MajorModuleMenuOnly()
        {
            try
            {
                MenuPanel mp = new MenuPanel();
                DataSet PrSet = sec.GetMajorModules();
                foreach (DataRow dr in PrSet.Tables[0].Rows)
                {
                    MenuItem me_nu = new MenuItem();
                    me_nu.ID = "N" + dr["MODULE_CODE"].ToString();
                    me_nu.Text = dr["MODULE_NAME"].ToString();

                    string mod_id = dr["MODULE_CODE"].ToString();
                    me_nu.DirectClickAction = Url.Action("Get_SubModuleForMajorModule", "UserGroupPermission");
                    me_nu.DirectEvents.Click.ExtraParams.Add(new { moduleid = mod_id });
                    me_nu.DirectEvents.Click.ExtraParams.Add(new StoreParameter { Name = "txt_UserGroupId", Value = "App.UserGroupId.getValue()", Mode = ParameterMode.Raw });
                    //me_nu.DirectEvents.Click.ExtraParams.Add(new Parameter("UserGroup_Idx", "UserGroupId.getValue()",ParameterMode.Value ));
                    //me_nu.DirectEvents.Click.ExtraParams.Add(new { UserGroup_Id = "App.UserGroupId.getValue()", ParameterMode.Value });

                    

                    mp.Menu.Items.Add(me_nu);
                    MenuSeparator m_sp = new MenuSeparator();
                    mp.Menu.Items.Add(m_sp);
                }

                /////----just for test-----
                ////////for (int i = 100; i <= 130; i++)
                ////////{
                ////////    MenuItem menuitem = new MenuItem();
                ////////    menuitem.ID = "xxs" + i.ToString();
                ////////    menuitem.Text = "Menu " + i.ToString().ToUpper();
                ////////    menuitem.Width = 290;
                ////////    menuitem.MinWidth = 290;
                ////////    //menuitem.Cls = "right-nav-menu";

                ////////    mp.Menu.Items.Add(menuitem);
                ////////    MenuSeparator m_sp = new MenuSeparator();
                ////////    mp.Menu.Items.Add(m_sp);

                ////////}
                 

                return this.Content(ComponentLoader.ToConfig(mp));
            }
            catch (Exception)
            {

                throw;
            }
        }

       

        public ActionResult Get_SubModuleForMajorModule(string moduleid,string txt_UserGroupId)
        {
            try
            {
                //string UserRo_leName = this.GetCmp<TextField>("UserRoleName").Text;
                //string UserGroup_Id = this.GetCmp<TextField>("UserGroupId").Text;
                //string UserGrou_pName = this.GetCmp<TextField>("UserGroupName").Text;

                NodeCollection nodes = new Ext.Net.NodeCollection();

                decimal modulecode = 0;
                if (Microsoft.VisualBasic.Information.IsNumeric(moduleid))
                {
                    modulecode = Convert.ToDecimal(moduleid);
                }
                ////supply modulecode
                this.GetCmp<Hidden>("txtmodulecode").SetValue(modulecode.ToString());
                /////get permissions for selected group
                DataTable dt  = sec_grantpermission.GetPermissionsForAUserGroup_ForAccessRights(txt_UserGroupId, modulecode);
                int suces = sec.AddGroupPermissionDataTable(dt, modulecode);

                DataSet data = sec_grantpermission.GetSubModules_ForMajorModuleForPermissions(modulecode);// (101);s
                DataRelation dr = new DataRelation("RelationT", data.Tables[0].Columns["MODULE_ID"], data.Tables[0].Columns["PARENT_ID"], false);
                data.Relations.Add(dr);

                foreach (DataRow r in data.Tables[0].Rows)
                {
                    string pa_id = r["PARENT_ID"].ToString();

                    if (r["PARENT_ID"].ToString() == "0")
                    {
                        Node asyncNode = new Node();

                        asyncNode.Text = r["MODULE_NAME"].ToString();
                        asyncNode.NodeID = r["MODULE_ID"].ToString();
                        //asyncNode.Checked = false;

                        RecursivelyLoadTree(r, ref asyncNode);
                        nodes.Add(asyncNode);
                    }


                }

                //now iterate through the nodes and check permission gratnted this user group
                //load group persmission and check those that apply
                //DataTable dtpermission = sec.GetGroupPermissions(ModuleCode, this.LblGrpId.Text);
                //foreach (var node in Collect(treeViewPermissions.Nodes))
                //{
                //    string nodeid = node.ClientId;
                //    DataRow foundrow = dtpermission.Rows.Find(nodeid);
                //    if (foundrow != null)
                //    {
                //        node.Checked = true;
                //    }
                //}



                this.GetCmp<TreePanel>("treepanel_submodules").GetRootNode().Reload();
                this.GetCmp<TreePanel>("treepanel_submodules").GetRootNode().AppendChild(nodes);

                return this.Direct();

                // MessageBox.Show(nnnn);
            }
            catch (Exception ex)
            {
                string sss = ex.ToString();
                throw;
            }
        }

        private void RecursivelyLoadTree(DataRow row, ref Node node)
        {
            try
            {
                // string act = rooturl.Substring(rooturl.IndexOf('/') + 1);
                //string con = rooturl.Substring(0, rooturl.IndexOf('/'));

                foreach (DataRow cr in row.GetChildRows("RelationT"))
                {
                    Node n = new Node();

                    n.Text = cr["MODULE_NAME"].ToString();
                    n.NodeID = cr["MODULE_ID"].ToString();

                    if (cr["NODE_LEAF"].ToString() == "L")
                    {
                        string rooturl = cr["URL"].ToString();
                        n.Leaf = true;

                        if (rooturl != "url")
                        {
                            string act = rooturl.Substring(rooturl.IndexOf('/') + 1);
                            string con = rooturl.Substring(0, rooturl.IndexOf('/'));
                            
                            n.CustomAttributes.Add(new ConfigItem("url", Url.Action(actionName: act, controllerName: con)));
                            n.Checked = false;
                            string has_right = cr["HAS_RIGHT"].ToString();
                            if (has_right == "Y")
                            {
                                n.Checked = true;
                            }
                        }


                    }

                    node.Children.Add(n);
                    RecursivelyLoadTree(cr, ref n);
                }
            }
            catch (Exception ex)
            {
                string exxx = ex.ToString();
                throw;
            }

        }




        public ActionResult Save_Permission_For_UserGroup()
        {
            try
            {
                string User_group_id = this.GetCmp<Hidden>("UserGroupId").Text;
                string access_rights = this.GetCmp<Hidden>("txtaccessrights").Text;
                string major_modulecode = this.GetCmp<Hidden>("txtmodulecode").Text;

                if (string.IsNullOrEmpty(User_group_id))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Select user group to assign permissions.Process aborted.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350


                    });
                    return this.Direct();
                }
                else if (string.IsNullOrEmpty(major_modulecode))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Select major module.Process aborted.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350


                    });
                    return this.Direct();
                }

                decimal modulecode = 0;
                if (Microsoft.VisualBasic.Information.IsNumeric(major_modulecode))
                {
                    modulecode = Convert.ToDecimal(major_modulecode);
                }
                if( string.IsNullOrEmpty(User_group_id) == false)
                {
                    if (this.sec_grantpermission.SaveRights_ForUseGroup(User_group_id, access_rights, modulecode))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "Saved successfully.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });

                        this.GetCmp<TreePanel>("treepanel_submodules").GetRootNode().Reload();
                        this.GetCmp<Hidden>("txtaccessrights").SetValue("");
                        this.GetCmp<Hidden>("txtmodulecode").SetValue("");
                    }
                }
                
                return this.Direct();

            }
            catch (Exception ex)
            {
                string sss = ex.ToString();
                throw;
            }
        }

        public ActionResult Clear_Submodules_TreeNodes()
        {
            try
            {

                this.GetCmp<TreePanel>("treepanel_submodules").GetRootNode().Reload();
                return this.Direct();
            }
            catch (Exception ex)
            {
                string sss = ex.ToString();
                throw;
            }
        }




























    }
}
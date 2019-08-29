
using PENFAD6DAL.Repository.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.GlobalObject;

namespace PENFAD6UI.Areas.Security.Controllers
{
    // Louis Amenyo
    public class UserGroupController : Controller
    {
        // GET: Security/UserGroup


        private sec_UserGroupRepo repo_groupc = new sec_UserGroupRepo();
        private sec_UserRoleRepo repo_rolec = new sec_UserRoleRepo();
        // GET: Security/UserRole


        public ActionResult Index()
        {
            return View();
        }


        public ActionResult AddUserGroupTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "UserGroupPartial",
                Model = repo_groupc.GetUserGroupList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }

        public ActionResult SaveUserGroup(sec_UserGroupRepo repo_group)
        {
            try
            {
                repo_group.MakerId = GlobalValue.User_ID;
                repo_group.MakeDate = GlobalValue.Scheme_Today_Date;
                repo_group.UserGroupStatus = "ACTIVE";

                if (this.ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(repo_group.UserGroupId))
                    {
                        //validate gorpuname
                        if (repo_rolec.Validate_UseId_GroupName_RoleName("name", "group", "add", repo_group.UserGroupName, "x") > 0)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = repo_group.UserGroupName + " already exist. Operation Aborted",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Width = 350
                            });
                            return this.Direct();
                        }
                    }
                    if ((repo_group.UserGroupId == "G10") || (repo_group.UserGroupId == "G100"))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Cannot edit system group.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350


                        });
                        return this.Direct();
                    }
                    if (this.repo_groupc.Add_Update_UserGroup(repo_group))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "Saved successfully.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });

                        Store store = X.GetCmp<Store>("groupstore");
                        store.Reload();

                        ClearControls();
                    }
                    return this.Direct();
                }
                else
                {
                    //var errors1 = ModelState .Where(x => x.Value.Errors.Count > 0) .Select(x => new { x.Key, x.Value.Errors }).ToArray();
                    //var errors = ModelState.Values.SelectMany(v => v.Errors);

                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = messages, // " Insufficient data. Operation Aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //return this.Direct();


            }
            catch (Exception ex)
            {
                string sss = ex.ToString();
                return this.Direct();
                throw;
            }
        }


        public ActionResult Read()
        {
            try
            {
                return this.Store(repo_groupc.GetUserGroupList());
            }
            catch (System.Exception)
            {

                throw;
            }

        }

        public ActionResult DeleteUserGroup(string UserGroupId)
        {
            try
            {
                if (string.IsNullOrEmpty(UserGroupId))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Select group to delete.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350


                    });
                    return this.Direct();
                }
                if ((UserGroupId == "G10") || (UserGroupId == "G100"))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Cannot delete system group.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350


                    });
                    return this.Direct();
                }
                //delete Record
                if (repo_groupc.DeleteUserGroup(UserGroupId))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Deleted Successfully.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    //clear controls
                    ClearControls();
                }

                Store store = X.GetCmp<Store>("groupstore");
                store.Reload();

                return this.Direct();
            }
            catch (System.Exception ex)
            {
                string sss = ex.ToString();
                throw;
            }

        }

        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmusergroup");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }





    }
}
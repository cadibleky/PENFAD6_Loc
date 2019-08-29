
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
    public class UserRoleController : Controller
    {
        private sec_UserRoleRepo repo_role = new sec_UserRoleRepo();
        // GET: Security/UserRole
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult AddRoleTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "UserRolePartial",
                Model = repo_role.GetUserRoleList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,

            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }

        public ActionResult SaveRole(sec_UserRoleRepo repo_role)
        {
            try
            {
                repo_role.MakerId = GlobalValue.User_ID;
                repo_role.MakeDate = GlobalValue.Scheme_Today_Date;
                repo_role.UserRoleStatus = "ACTIVE";

                if (this.ModelState.IsValid)
                {
                    if(string.IsNullOrEmpty(repo_role.UserRoleId))
                    {
                        //validate rolename
                        if (repo_role.Validate_UseId_GroupName_RoleName("name", "role", "add", repo_role.UserRoleName, "x") > 0)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = repo_role.UserRoleName + " already exist. Operation Aborted",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Width = 350
                            });
                            return this.Direct();
                        }
                    }
                    if ((repo_role.UserRoleId == "R10") || (repo_role.UserRoleId == "R100"))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Cannot edit system role.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350


                        });
                        return this.Direct();
                    }
                    if (this.repo_role.Add_Update_UserRole(repo_role))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "Saved successfully.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });

                        Store store = X.GetCmp<Store>("rolestore");
                        store.Reload();

                        ClearControls();
                    }
                    return this.Direct();
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = " Insufficient data. Operation Aborted",
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
                //throw;
            }
        }


        public ActionResult Read()
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



        public ActionResult DeleteRole(string UserRoleId)
        {
            try
            {
                if (string.IsNullOrEmpty(UserRoleId))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Select role to delete.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350


                    });
                    return this.Direct();
                }
                if ((UserRoleId== "R10") ||(UserRoleId== "R100"))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Cannot delete system role.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350


                    });
                    return this.Direct();
                }
                //delete Record
                if (repo_role.DeleteRecord(UserRoleId))
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

                Store store = X.GetCmp<Store>("rolestore");
                store.Reload();

                return this.Direct();
            }
            catch (System.Exception ex)
            {
                //string sss = ex.ToString();
                throw ex;
            }

        }

        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmuserrole");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }









    }
}
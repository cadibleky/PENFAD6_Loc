using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.Security;
using PENFAD6DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PENFAD6UI.Controllers.StartUp
{
    public class StartUpController : Controller
    {
        private readonly cSecurityRepo sec = new cSecurityRepo();
        private readonly GlobalValue gb_value = new GlobalValue();

        [Authorize]
        public ActionResult Index()
        {
            try
            {
                ViewBag.CompanyName = "";
                GlobalValue.GlobalValue_Ini();
                return View();
            }
            catch
            {
                return View();
            }
            finally
            {

            }

        }

        public ActionResult Rooturl(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "Index",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData = this.ViewData
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetNextTabAsActive();

            return (pvr);
        }


        public ActionResult LoadModulesMainMenu()
        {
            try
            {
                // get group id
                string userid = User.Identity.Name;
                sec.Get_GroupId(userid);
                //GlobalValue.User_GroupID = "G100";

                DataTable dt = sec.GetMajorModuleAcessForAUserGroup(GlobalValue.User_GroupID, 0);
                List<MenuItem> menuitems = new List<MenuItem>();

                foreach (DataRow module in dt.Rows)
                {
                    MenuItem menuitem = new MenuItem();
                    menuitem.ID = module["ModuleId"].ToString();
                    menuitem.Text = module["Module_Name"].ToString().ToUpper();
                    menuitem.Width = 290;
                    menuitem.MinWidth = 290;
                    //menuitem.Cls = "right-nav-menu";
                    string mod_id = module["ModuleId"].ToString();
                    switch (mod_id)
                    {
                        case "100":
                            menuitem.Icon = Icon.ServerKey;
                            break;
                        case "101":
                            menuitem.Icon = Icon.FlagGh;
                            break;
                    }


                    ////////menuitem.StyleSpec = "border-color:white;";
                    ////menuitem.DirectClickAction = "StartUp/GetSubModule";
                    menuitem.DirectClickAction = Url.Action("GetChildren", "StartUp");
                    menuitem.DirectEvents.Click.ExtraParams.Add(new { moduleid = module["ModuleId"].ToString() });
                    //menuitem.DirectEvents.Click.ExtraParams.Add(new Parameter("containerId", "pnlNav"));
                    //menuitem.DirectEvents.Click.ExtraParams.Add(new Parameter("containerId", "App.Viewp"));

                    ////menuitem.DirectEvents.Click.Before = "App.leftnav.removeAll()";
                    //// menuitem.Icon = Icon.ApplicationOsxDouble;
                    ////menuitem.DirectEvents.Click.Confirmation.ConfirmRequest = true;
                    ////menuitem.DirectEvents.Click.Confirmation.Message = module.ModuleId;


                    menuitems.Add(menuitem);

                }

                X.Mask.Hide();
                return this.Content(ComponentLoader.ToConfig(menuitems));

            }
            catch (Exception ex)
            {
                X.Mask.Hide();
                string sss = ex.ToString();
                return null;
            }
            finally
            {

            }

        }


        public ActionResult GetChildren(string moduleid)
        {
            try
            {
                NodeCollection nodes = new Ext.Net.NodeCollection();
                decimal modulecode = 0;
                if (Microsoft.VisualBasic.Information.IsNumeric(moduleid))
                {
                    modulecode = Convert.ToDecimal(moduleid);
                }

                //get module ids
                DataTable dt = sec.GetGroupPermissions(modulecode, GlobalValue.User_GroupID);
                //insert dt into database
                if (dt.Rows.Count > 0)
                {
                    int suces = sec.AddGroupPermissionDataTable(dt, modulecode);
                    if (suces > 0)
                    {
                        //get permission details 
                        DataSet ds = sec.GetNodesForAUserAFromTemp(0, modulecode);
                        DataRelation data_realtion = new DataRelation("RelationT", ds.Tables[0].Columns["MODULEID"], ds.Tables[0].Columns["Parentid"], false);
                        ds.Relations.Add(data_realtion);

                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            int Parentid = Convert.ToInt32(r["Parentid"].ToString());
                            if (Parentid == 0)
                            {
                                if (r["Parentid"].ToString() == "0")
                                {
                                    Node asyncNode = new Node();

                                    asyncNode.Text = r["MODULE_NAME"].ToString();
                                    asyncNode.NodeID = r["MODULEID"].ToString();
                                    
                                    //asyncNode.Checked = false;

                                    RecursivelyLoadTree(r, ref asyncNode);
                                    nodes.Add(asyncNode);
                                }
                            }
                        }
                    }
                }
                X.Mask.Hide();
                this.GetCmp<TreePanel>("leftnav").GetRootNode().Reload();
                this.GetCmp<TreePanel>("leftnav").GetRootNode().AppendChild(nodes);

                return this.Direct();

                // MessageBox.Show(nnnn);
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
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
                    n.NodeID = cr["MODULEID"].ToString();

                    if (cr["Node_Leaf"].ToString() == "L")
                    {
                        string rooturl = cr["URL"].ToString();
                        n.Leaf = true;

                        if (rooturl != "url")
                        {
                            //n.Href = cr["Url"].ToString();
                            //n.HrefTarget = "MainArea";

                            // n.Call.ControllerContext = "";
                            string act = rooturl.Substring(rooturl.IndexOf('/') + 1);
                            string con = rooturl.Substring(0, rooturl.IndexOf('/'));
                            //act = "/" + act;
                            //act = "AddUserGroupPermissionTab";
                            //con = "Security/UserGroupPermission/AddUserGroupPermissionTab";
                            n.CustomAttributes.Add(new ConfigItem("url", Url.Action(actionName: act, controllerName: con)));
                            // n.CustomAttributes.Add(new ConfigItem("url", Url.Action(con)));
                        }

                        //n.Checked = false;
                    }
                    X.Mask.Hide();
                    node.Children.Add(n);
                    RecursivelyLoadTree(cr, ref n);
                }
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
                string exxx = ex.ToString();
                throw;
            }

        }

        ///Newly Added Login
        ///
        readonly LoginModelRepo loginmodel = new LoginModelRepo();
        readonly App_Parameter_SettingsRepo bankparamsettings = new App_Parameter_SettingsRepo();


        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (returnUrl == "")
            {
                return RedirectToAction("Index");
            }
            return View();
        }

      
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(sec_UserRepo user_login, string ReturnUrl = "")
        {
            //check if bank parameter settings is setup
            var banksettingsFailurecount = bankparamsettings.GetBankParameterSettings("12");
            var banksettingsFailurecountSub1 = bankparamsettings.GetBankParameterSettings("13");
            if (banksettingsFailurecount.Count > 0)
            {
                //check if falure count is enabled
                if (banksettingsFailurecount[0].App_Value == "YES")
                {
                    //check if falure count is set

                    if (banksettingsFailurecountSub1.Count > 0)
                    {

                    }
                    else
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "User password falure count is not set",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350
                        });
                        return View();
                    }
                }
                else
                {

                }
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "User password falure option not set",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return View();
            }



            //check if password expiration is enabled
            var banksettingsEnforceUserPassword = bankparamsettings.GetBankParameterSettings("10");
            var banksettingsEnforceUserPasswordSub1 = bankparamsettings.GetBankParameterSettings("11");
            if (banksettingsEnforceUserPassword.Count > 0)
            {
                //check if password experation is enabled
                if (banksettingsEnforceUserPassword[0].App_Value == "YES")
                {
                    //check if password expiration period is set

                    if (banksettingsEnforceUserPasswordSub1.Count > 0)
                    {

                    }
                    else
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "User password expiration period is not set",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350
                        });
                        return View();
                    }
                }
                else
                {

                }
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "User password expiration option not set",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return View();
            }


            //Check if User Exists
            var userdetails = loginmodel.GetUser(user_login);
            if (userdetails.Count > 0)
            {

                //Check if User Is Logged In
                bool IsloggedIn = loginmodel.IsLoggedIn(user_login);
                if (IsloggedIn == false)
                {
                    //Authenticate User
                    bool userlogin = loginmodel.GetLoginUser(user_login);
                    if (userlogin == true)
                    {
                        var userdata = loginmodel.GetUserData(user_login);
                        //check if userdata count is > 1
                        if (userdata.Count >= 1)
                        {
                            //Check if User Is Active And Approved
                            if (userdata[0].User_Status == "ACTIVE" && userdata[0].Auth_Status == "AUTHORIZED")
                            {
                                //check if password failur option is yes
                                if (banksettingsFailurecount[0].App_Value == "YES")
                                {
                                    //Check Password Failure Count
                                    if (userdata[0].Password_Failure_Count < Convert.ToInt32(banksettingsFailurecountSub1[0].App_Value))
                                    {
                                        //Check if Company Password Expiration Is Yes
                                        if (banksettingsEnforceUserPassword[0].App_Value == "YES")
                                        {
                                            //Check If Password Is Expired Against Number Of Days After Last Password Update
                                            DateTime Currentdate = DateTime.Now;

                                            TimeSpan Totaldays = Currentdate - userdata[0].Password_Change_Date;
                                            if (Totaldays.TotalDays >= Convert.ToInt32(banksettingsEnforceUserPasswordSub1[0].App_Value))
                                            {
                                                //Navigate to Password Change Area

                                                //Create User Session
                                                loginmodel.CleanLoginFailureCount(user_login);
                                                //loginmodel.AddUserLoginSession(user_login);
                                                FormsAuthentication.SetAuthCookie(user_login.User_Id, false);
                                                if (Url.IsLocalUrl(ReturnUrl))
                                                {
                                                    return Redirect(ReturnUrl);
                                                }
                                                else
                                                {
                                                    GlobalValue.User_ID = User.Identity.Name;
                                                    return RedirectToAction("Index");
                                                }
                                            }
                                            else
                                            {
                                                //Create User Session
                                                loginmodel.CleanLoginFailureCount(user_login);
                                                //loginmodel.AddUserLoginSession(user_login);
                                                FormsAuthentication.SetAuthCookie(user_login.User_Id, false);
                                                if (Url.IsLocalUrl(ReturnUrl))
                                                {
                                                    return Redirect(ReturnUrl);
                                                }
                                                else
                                                {
                                                    GlobalValue.User_ID = User.Identity.Name;
                                                    return RedirectToAction("Index");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //Create User Session
                                            loginmodel.CleanLoginFailureCount(user_login);
                                            //loginmodel.AddUserLoginSession(user_login);
                                            FormsAuthentication.SetAuthCookie(user_login.User_Id, false);
                                            if (Url.IsLocalUrl(ReturnUrl))
                                            {
                                                return Redirect(ReturnUrl);
                                            }
                                            else
                                            {
                                                GlobalValue.User_ID = User.Identity.Name;
                                                return RedirectToAction("Index");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        X.Msg.Show(new MessageBoxConfig
                                        {
                                            Title = "Info",
                                            Message = "User account has been locked. Contact administrator..",
                                            Buttons = MessageBox.Button.OK,
                                            Icon = MessageBox.Icon.INFO,
                                            Width = 350
                                        });
                                    }
                                }
                                else
                                {
                                    //Check if Company Password Expiration Is Yes
                                    if (banksettingsEnforceUserPassword[0].App_Value == "YES")
                                    {
                                        //Check If Password Is Expired Against Number Of Days After Last Password Update
                                        DateTime Currentdate = DateTime.Now;

                                        TimeSpan Totaldays = Currentdate - userdata[0].Password_Change_Date;
                                        if (Totaldays.TotalDays >= Convert.ToInt32(banksettingsEnforceUserPasswordSub1[0].App_Value))
                                        {
                                            //Navigate to Password Change Area

                                            //Create User Session
                                            loginmodel.CleanLoginFailureCount(user_login);
                                            //loginmodel.AddUserLoginSession(user_login);
                                            FormsAuthentication.SetAuthCookie(user_login.User_Id, false);
                                            if (Url.IsLocalUrl(ReturnUrl))
                                            {
                                                return Redirect(ReturnUrl);
                                            }
                                            else
                                            {
                                                GlobalValue.User_ID = User.Identity.Name;
                                                return RedirectToAction("Index");
                                            }
                                        }
                                        else
                                        {
                                            //Create User Session
                                            loginmodel.CleanLoginFailureCount(user_login);
                                            //loginmodel.AddUserLoginSession(user_login);
                                            FormsAuthentication.SetAuthCookie(user_login.User_Id, false);
                                            if (Url.IsLocalUrl(ReturnUrl))
                                            {
                                                return Redirect(ReturnUrl);
                                            }
                                            else
                                            {
                                                GlobalValue.User_ID = User.Identity.Name;
                                                return RedirectToAction("Index");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //Create User Session
                                        loginmodel.CleanLoginFailureCount(user_login);
                                        //loginmodel.AddUserLoginSession(user_login);
                                        FormsAuthentication.SetAuthCookie(user_login.User_Id, false);
                                        if (Url.IsLocalUrl(ReturnUrl))
                                        {
                                            return Redirect(ReturnUrl);
                                        }
                                        else
                                        {
                                            GlobalValue.User_ID = User.Identity.Name;
                                            return RedirectToAction("Index");
                                        }

                                    }
                                }

                            }
                            else
                            {
                                X.Msg.Show(new MessageBoxConfig
                                {
                                    Title = "Info",
                                    Message = "User is not authorized to login. Contact administrator..",
                                    Buttons = MessageBox.Button.OK,
                                    Icon = MessageBox.Icon.INFO,
                                    Width = 350
                                });
                            }


                        }
                        else
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Info",
                                Message = "User details not valid. Contact administrator..",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.INFO,
                                Width = 350
                            });
                        }
                    }
                    else
                    {
                        loginmodel.AddLoginFailureCount(user_login);
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "User login failed..",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350
                        });
                    }
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Warning",
                        Message = "User session is active..",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.WARNING,
                        Width = 350
                    });
                }
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Warning",
                    Message = "User does not exist..",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.WARNING,
                    Width = 350
                });
            }

            ModelState.Remove("User_Id");
            ModelState.Remove("Password");
            return View();

        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            //loginmodel.RemoveUserLoginSession(User.Identity.Name);

            Session.Abandon();
            Session.Clear();
            Session.RemoveAll();
            return RedirectToAction("Login");
        }


        // Reports Partials load

        public ActionResult Load_Report(string containerId = "z_mainform_center_panel_tab_main_1")
        {
            return PartialView("Report_ActivePartial");
        }


        
        [Authorize]
        public ActionResult AddUserChangePasswordTab(string containerId = "MainArea")
        {
            cLogger logger = new cLogger();
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "UserChangePasswordPartial",
                    Model = new List<sec_User_Change_Password>(),
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo,
                };
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
                X.GetCmp<TextField>("LayoutChangePasswordUserId").Value = GlobalValue.User_ID;
                X.GetCmp<Window>("UserChangePasswordWindow").Show();
                return pvr;
            }
            catch (Exception ex)
            {

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = ex.Message,
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350


                });
                logger.WriteLog(ex.ToString());
                return this.Direct();
            }
            finally
            {

            }
        }


    } //END CLASS

}
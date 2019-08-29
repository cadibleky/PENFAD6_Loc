
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
    public class UserController : Controller
    {
        // GET: Security/User


        private sec_UserRepo repo_userc = new sec_UserRepo();
        readonly sec_UserRepo repo_user = new sec_UserRepo();
        private sec_UserGroupRepo repo_groupc = new sec_UserGroupRepo();
        private sec_UserRoleRepo repo_rolec = new sec_UserRoleRepo();
        // GET: Security/UserRole
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult AddUserTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "UserPartial",
                Model = repo_userc.GetEmployeeToCreateUserList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }


        public ActionResult AddUserPendingTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "UserApprovePartial",
                Model = repo_userc.GetUserPendingList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddUserResetTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "UserResetPartial",
                Model = repo_userc.GetUserActiveList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }

        public ActionResult AddUserReassignTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "UserReassignPartial",
                Model = repo_userc.GetUserActiveList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }


        public ActionResult AddUserSuspendTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "UserSuspendPartial",
                Model = repo_userc.GetUserActiveList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }

        public ActionResult AddUserActivateTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "UserActivatePartial",
                Model = repo_userc.GetUserSuspendList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }

        public ActionResult AddUserViewTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "UserViewPartial",
                Model = repo_userc.GetUserActiveList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }
        public ActionResult SaveUser(sec_UserRepo repo_user)
        {
            try
            {
                 
                repo_user.Maker_Id = GlobalValue.User_ID;
                repo_user.Make_Date = GlobalValue.Scheme_Today_Date;
                repo_user.Password = repo_user.Employee_Id + "@" + GetRandomvalue().ToString();
                if (this.ModelState.IsValid)
                {
                       //validate gorpuname
                    if (repo_rolec.Validate_UseId_GroupName_RoleName("name", "group", "add", repo_userc.User_Id, "x") > 0)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = repo_user.Employee_Name + " already exist. Operation Aborted",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350
                        });
                        return this.Direct();
                    }
                    //}
                    if ((repo_user.Employee_Id == "SYSTEM") || (repo_user.Employee_Id == "SYSTEM"))
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

             

                    if (this.repo_userc.CreateNewUser(repo_user))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "User account created successfully.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });

                        Store store = X.GetCmp<Store>("userstore");
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
            }
            catch (Exception ex)
            {
                string sss = ex.ToString();
                return this.Direct();
                throw ex;
            }
        }

        public ActionResult ApproveUser(sec_UserRepo repo_user)
        {
            try
            {
                repo_user.Update_Id = GlobalValue.User_ID;
                repo_user.Update_Date = DateTime.Now;
                if (this.ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(repo_user.User_Id))
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message =  "Sorry! No user has been selected for approval",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350
                        });
                        return this.Direct();
                    }

                    repo_user.ApproveNewUser(repo_user);
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "User account approved successfully.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350 
                        });

                        Store store = X.GetCmp<Store>("userapprovestore");
                        store.Reload();
                     }
                    return this.Direct();
                }
                else
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                    X.Mask.Hide();
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
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Could not approve user, check internet connection",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }
        }

        public ActionResult ResetUser(sec_UserRepo repo_user)
        {
            try
            {
                repo_user.Maker_Id = GlobalValue.User_ID;
                repo_user.Make_Date = DateTime.Now;
                repo_user.Password = repo_user.User_Id + "@" + GetRandomvalue().ToString();
                if (this.ModelState.IsValid)
                {
                    if ((repo_user.User_Id == "teksol.admin") || (repo_user.User_Id == "sys.admin"))
                    {
                        X.Mask.Hide();
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


                    repo_user.ResetNewUser(repo_user);
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "User account reset successful.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });

                        Store store = X.GetCmp<Store>("userresetstore");
                        store.Reload();

                    }
                    return this.Direct();
                }
                else
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                    X.Mask.Hide();
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
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Could not reset user password, check internet connection", 
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }
        }

        public ActionResult ReassignUser(sec_UserRepo repo_user)
        {
            try
            {
                repo_user.Maker_Id = GlobalValue.User_ID;
                repo_user.Make_Date = DateTime.Now;
                //repo_user.Password = repo_user.User_Id + "@" + GetRandomvalue().ToString();
                if (this.ModelState.IsValid)
                {
                    if ((repo_user.User_Id == "teksol.admin") || (repo_user.User_Id == "sys.admin"))
                    {
                        X.Mask.Hide();
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

                    if (string.IsNullOrEmpty(repo_user.User_Id))
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "No user selected. Process aborted",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350
                        });
                        return this.Direct();
                    }

                    if (string.IsNullOrEmpty(repo_user.User_Role_Id))
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Select new user group. Process aborted",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350
                        });
                        return this.Direct();
                    }

                    repo_user.ReassignNewUser(repo_user);
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "User group changed successful.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });

                        Store store = X.GetCmp<Store>("userresetstore");
                        store.Reload();

                    }
                    return this.Direct();
                }
                else
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                    X.Mask.Hide();
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
            }
            catch (Exception)
            {
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Could not reset user password, check internet connection",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }
        }

        public ActionResult SuspendUser(sec_UserRepo repo_user)
        {
            try
            {
                repo_user.Maker_Id = GlobalValue.User_ID;
                if (this.ModelState.IsValid)
                {
                    if ((repo_user.User_Id == "teksol.admin") || (repo_user.User_Id == "sys.admin"))
                    {
                        X.Mask.Hide();
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



                    if (repo_user.SuspendNewUser(repo_user))
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "User account successfully suspended.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });

                        Store store = X.GetCmp<Store>("usersuspendstore");
                        store.Reload();
                    }
                    return this.Direct();
                }
                else
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                    X.Mask.Hide();
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
            }
            catch (Exception)
            {
                X.Mask.Hide();
                return this.Direct();
            }
        }

        public ActionResult ActivateUser(sec_UserRepo repo_user)
        {
            try
            {
                repo_user.Maker_Id = GlobalValue.User_ID;
                if (this.ModelState.IsValid)
                {
                    if ((repo_user.User_Id == "teksol.admin") || (repo_user.User_Id == "sys.admin"))
                    {
                        X.Mask.Hide();
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



                    if (repo_user.ActivateNewUser(repo_user))
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "User account successfully activated.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });

                        Store store = X.GetCmp<Store>("useractivatestore");
                        store.Reload();
                    }
                    return this.Direct();
                }
                else
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                    X.Mask.Hide();
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
            }
            catch (Exception)
            {
                X.Mask.Hide();
                return this.Direct();
            }
        }


        public ActionResult ReadPending()
        {
            try
            {
                return this.Store(repo_user.GetUserPendingList());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }

        public ActionResult ReadActive()
        {
            try
            {
                return this.Store(repo_user.GetUserActiveList());
            }
            catch (System.Exception)
            {
                throw;
            }

        }


        public ActionResult ReadSuspended()
        {
            try
            {
                return this.Store(repo_user.GetUserSuspendList());
            }
            catch (System.Exception)
            {
                throw;
            }

        }

        public ActionResult Read()
        {
            try
            {
                return this.Store(repo_userc.GetEmployeeToCreateUserList()); 
            }
            catch (System.Exception)
            {

                throw;
            }

        }

        public ActionResult GetUserGroupForRoleIdList(string roleid)
        {
           try
            {
                return this.Store(repo_groupc.GetUserGroupForRoleIdList(roleid));
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }


     
        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmuser");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }


        private string GetRandomvalue()
        {
            try
            {
                Random random = new Random();
                int randomNumber = random.Next(10000, 90000);

                return randomNumber.ToString();
            }
            catch (Exception)
            {
                return "19074"; 
               // throw;
            }
        }

        readonly LoginModelRepo loginmodel = new LoginModelRepo();
        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="change_password"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult ChangePassword(sec_User_Change_Password change_password)
        {
            cLogger logger = new cLogger();
            try
            {

                if (ModelState.IsValid)
                {
                    //Authenticate User
                    bool userlogin = loginmodel.AuthenticateUserPass(User.Identity.Name, change_password.Old_Password);
                    if (userlogin == true)
                    {
                        repo_user.ChangePassword(User.Identity.Name, change_password.Confirm_Password);

                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "User password change successfully.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });

                        X.GetCmp<FormPanel>("UserChangePasswordFormPanel").Reset();
                        X.GetCmp<Window>("UserChangePasswordWindow").Hide();
                        return this.Direct();
                    }
                    else
                    {

                        //loginmodel.AddLoginFailureCount(user_repo);

                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Authentication failed. Please try again..",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });
                        return this.Direct();
                    }

////////////////
                }

                else
                {
                    string messages = string.Join(Environment.NewLine, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).FirstOrDefault());
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
            }
            catch (Exception ex)
            {
                string ora_code = ex.Message.Substring(0, 9);
                if (ora_code == "ORA-20000")
                {
                    ora_code = "Record already exist. Process aborted..";
                }
                else if (ora_code == "ORA-20100")
                {
                    ora_code = "Record is uniquely defined in the system. Process aborted..";
                }
                else
                {
                    ora_code = ex.ToString();
                }
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = ora_code,
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350


                });
               // logger.WriteLog(ex.ToString());
                return this.Direct();
            }
            finally
            {

            }
        }

    }
}
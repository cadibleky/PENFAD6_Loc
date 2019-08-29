using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.Repository.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Security.Controllers
{
    public class UserAuthorizerController : Controller
    {
        private sec_AuthorizerRepo repo_Authorizer = new sec_AuthorizerRepo();
        // GET: Security/UserAuthorizer
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddUserAuthorizerTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "AddAuthorizerPartial",
              ///  Model = repo_Authorizer,
                Model = repo_Authorizer,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData = this.ViewData
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetNextTabAsActive();

            return (pvr);
        }
        public ActionResult CreateAuthorizer(sec_AuthorizerRepo AuthorizerRepo)
        {
            try
            {

             
                if (this.ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(AuthorizerRepo.User_Id))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Select  User.Process aborted.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });
                        return this.Direct();
                    }

                    if (string.IsNullOrEmpty(AuthorizerRepo.With_Drawal_Authority ))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Select Authorizer Withdrawal  .Process aborted.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });
                        return this.Direct();
                    }
                    if (Microsoft.VisualBasic.Information.IsNumeric(AuthorizerRepo.With_Drawal_Limit))
                    {
                        AuthorizerRepo.With_Drawal_Limit = Convert.ToDecimal(AuthorizerRepo.With_Drawal_Limit);
                        if (AuthorizerRepo.With_Drawal_Limit < 0)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Invalid Authorizer  Withdrawal Limit for " + AuthorizerRepo.Full_Name ,
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Width = 350

                            });
                            return this.Direct();
                        }
                    }
                    else
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Invalid Authorizer  Withdrawal Limit for " + AuthorizerRepo.Full_Name,
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350

                        });
                        return this.Direct();

                    }
                    if (string.IsNullOrEmpty(AuthorizerRepo.Authority_Level ))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Select Authorizer Type.Process aborted.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });
                        return this.Direct();
                    }
                    if (string.IsNullOrEmpty(AuthorizerRepo.Approve_Fixed_Income))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Select Approve Fixed Income.Process aborted.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });
                        return this.Direct();
                    }
                    if (Microsoft.VisualBasic.Information.IsNumeric(AuthorizerRepo.Fixed_Income_Limit ))
                    {
                        AuthorizerRepo.Fixed_Income_Limit = Convert.ToDecimal(AuthorizerRepo.Fixed_Income_Limit);
                        if (AuthorizerRepo.Fixed_Income_Limit < 0)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Invalid Authorizer  Fixed Income Limit for " + AuthorizerRepo.Full_Name,
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Width = 350

                            });
                            return this.Direct();
                        }
                    }
                    else
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Invalid Authorizer  Fixed Income Limit for " + AuthorizerRepo.Full_Name,
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350

                        });
                        return this.Direct();

                    }

                    if (string.IsNullOrEmpty(AuthorizerRepo.Approve_Equity ))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Select Aprove Equity .Process aborted.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });
                        return this.Direct();
                    }

                    if (Microsoft.VisualBasic.Information.IsNumeric(AuthorizerRepo.Equity_Limit))
                    {
                        AuthorizerRepo.Equity_Limit = Convert.ToDecimal(AuthorizerRepo.Equity_Limit);
                        if (AuthorizerRepo.Equity_Limit < 0)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Invalid Authorizer Equity Limit for " + AuthorizerRepo.Full_Name,
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Width = 350

                            });
                            return this.Direct();
                        }
                    }
                    else
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Invalid Authorizer Equity Limit for " + AuthorizerRepo.Full_Name,
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350

                        });
                        return this.Direct();

                    }



                    if (repo_Authorizer.CreateAuthorizer (AuthorizerRepo))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Success",
                            Message = "Authorizer created successfully.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });

                        Store store = X.GetCmp<Store>("authorizerstore");
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
        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frm_AddAuthorizerPartial");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public ActionResult DeleteRecord(string user_id)
        {
            if (user_id == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Please select a Authorizer to delete.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350


                });
                return this.Direct();
            }

            repo_Authorizer.DeleteRecord(user_id);
          

            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Deleted Successfully.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350


            });

            Store store = X.GetCmp<Store>("authorizerstore");
            store.Reload();
            var reset = X.GetCmp<FormPanel>("authorizerstore");
            reset.Reset();

            return this.Direct();
        }

        public ActionResult ReadAuthorizer()
        {
            return this.Store(repo_Authorizer.GetAuthorizer_List()); 
        }
    }
}
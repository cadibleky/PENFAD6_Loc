
using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.Repository.Setup.InvestSetup;
using PENFAD6DAL.Repository.Setup.pfmSetup;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Setup.Controllers.InvestSetup

{
    public class Invest_AuthorizersController : Controller
    {
        readonly Invest_AuthorizersRepo AuthorizersRepo = new Invest_AuthorizersRepo();       
        public ActionResult Index()
        {
            return View();
        }
        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmAuthorizers");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public ActionResult AddAuthorizersTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "AuthorizerPartial",
                Model = AuthorizersRepo.GetAuthList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }
        public ActionResult SaveRecord(Invest_AuthorizersRepo AuthorizersRepo)
        { 
         try
        
        {
            if (ModelState.IsValid)
            {
                    if (Microsoft.VisualBasic.Information.IsNumeric(AuthorizersRepo.Fixed_Income_Limit))
                    {
                        AuthorizersRepo.Fixed_Income_Limit = Convert.ToDecimal(AuthorizersRepo.Fixed_Income_Limit);
                        if (AuthorizersRepo.Fixed_Income_Limit < 0)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Invalid  Fixed Income Limit " + AuthorizersRepo.User_Id ,
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
                            Message = "Invalid  Fixed Income Limit for " + AuthorizersRepo.User_Id,
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350

                        });
                        return this.Direct();

                    }

                    if (Microsoft.VisualBasic.Information.IsNumeric(AuthorizersRepo.Equity_Limit))
                    {
                        AuthorizersRepo.Equity_Limit = Convert.ToDecimal(AuthorizersRepo.Equity_Limit);
                        if (AuthorizersRepo.Equity_Limit < 0)
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Invalid  Equity Limit " + AuthorizersRepo.User_Id,
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
                            Message = "Invalid Equity Limit for " + AuthorizersRepo.User_Id,
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350

                        });
                        return this.Direct();

                    }


                    this.AuthorizersRepo.SaveRecord(AuthorizersRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Authorizer Successfully Saved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                ClearControls();
                Store store = X.GetCmp<Store>("authorizersStore");
                store.Reload();

                return this.Direct();
            }
            else
            {

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
                return this.Direct();
            }
            finally
            {

            }
        }
        
    public ActionResult DeleteRecord(string User_Id)
        {
            if (User_Id == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! No Authorizer has been selected.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

            AuthorizersRepo.DeleteRecord(User_Id);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Authorizer Successfully deleted.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350


            });
            ClearControls();
            Store store = X.GetCmp<Store>("authorizersStore");
            store.Reload();

            return this.Direct();
        }
        public ActionResult Read()
        {
            return this.Store(AuthorizersRepo.GetAuthList());
        }

    
    }
}


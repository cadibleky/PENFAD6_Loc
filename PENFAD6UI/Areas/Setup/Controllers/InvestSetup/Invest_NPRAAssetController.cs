using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.Repository.Setup.InvestSetup;
using PENFAD6DAL.Repository.Setup.pfmSetup;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Setup.Controllers.InvestSetup

{
    public class Invest_NPRAAssetController : Controller
    {
        readonly invest_NPRAAssetRepo NPRAAssetRepo = new invest_NPRAAssetRepo();       
        public ActionResult Index()
        {
            return View();
        }
        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmNPRAAsset");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public ActionResult AddNPRAAssetTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "NPRAAssetPartial",
                Model = NPRAAssetRepo.GetNPRAAssetList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }
        public ActionResult SaveRecord(invest_NPRAAssetRepo NPRAAssetRepo)
        { 
         try      
        {
            if (ModelState.IsValid)
            {
                    if (NPRAAssetRepo.Asset_Allocation_Limit < 0 || NPRAAssetRepo.Asset_Allocation_Limit > 100)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Invalid Asset Allocation Limit for " + NPRAAssetRepo.NPRA_Asset_Class_Name,
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350

                        });

                      
                        return this.Direct();
                    }

                    if (NPRAAssetRepo.Issuer_Limit < 0 || NPRAAssetRepo.Issuer_Limit > 100)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Invalid Asset Allocation Limit for " + NPRAAssetRepo.Issuer_Limit,
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350

                        });

                        return this.Direct();
                    }

                    this.NPRAAssetRepo.SaveRecord(NPRAAssetRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "NPRA Asset Class Successfully Saved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                ClearControls();
                Store store = X.GetCmp<Store>("npraassetStore");
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

        public ActionResult DeleteRecord(string NPRA_Asset_Class_Id)
        {
            if (NPRA_Asset_Class_Id == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! No Asset has been selected.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

            NPRAAssetRepo.DeleteRecord(NPRA_Asset_Class_Id);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "NPRA Asset Successfully deleted.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350


            });
            ClearControls();
            Store store = X.GetCmp<Store>("npraassetStore");
            store.Reload();

            return this.Direct();
        }
        public ActionResult Read()
        {
            return this.Store(NPRAAssetRepo.GetNPRAAssetList());
        }

      
    }
}


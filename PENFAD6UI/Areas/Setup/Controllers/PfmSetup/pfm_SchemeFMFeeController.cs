
using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Setup.Controllers.pfmSetup
{
    public class pfm_SchemeFMFeeController : Controller
    {
        readonly pfm_SchemeFeeRepo SchemeFeeRepo = new pfm_SchemeFeeRepo();       
        public ActionResult Index()
        {
            return View();
        }
        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmsetupSFFM");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public ActionResult AddSchemeFMFeeTab(string containerId = "MainArea")
        {
            try
            { 
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "SchemeFMFeePartial",
                Model = SchemeFeeRepo.GetSchemeFeeList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
                X.Mask.Hide();
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
            }
            catch (System.Exception)
            {
                X.Mask.Hide();
                throw;
            }
        }

        public ActionResult AddSchemeFeeApproveTab(string containerId = "MainArea")
        {
            try
            { 
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "SchemeFeeApprovePartial",
                Model = SchemeFeeRepo.GetSchemeFeeList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
                X.Mask.Hide();
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
            }
            catch (System.Exception)
            {
                X.Mask.Hide();

                throw;
            }
        }
        public ActionResult SaveRecord(pfm_SchemeFeeRepo SchemeFeeRepo) 
        {
            try
            {
                if (Microsoft.VisualBasic.Information.IsNumeric(SchemeFeeRepo.Flat_Or_Rate_Value))
                {
                    SchemeFeeRepo.Flat_Or_Rate_Value = Convert.ToDecimal(SchemeFeeRepo.Flat_Or_Rate_Value);
                    if (SchemeFeeRepo.Flat_Or_Rate_Value < 0)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Invalid   Value Rate for " + SchemeFeeRepo.Scheme_Name + SchemeFeeRepo.Fee_Description,
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
                        Message = "Invalid  Value Rate for " + SchemeFeeRepo.Scheme_Name + SchemeFeeRepo.Fee_Description ,
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350

                    });
                    return this.Direct();

                }

               

            //    if (SchemeFeeRepo.isSchemeFeeUnique(SchemeFeeRepo.Scheme_Id + SchemeFeeRepo.Fee_Id) == true)
            //{
            //    X.Msg.Show(new MessageBoxConfig
            //    {
            //        Title = "Duplicate",
            //        Message = "Scheme Fee already exist.",
            //        Buttons = MessageBox.Button.OK,
            //        Icon = MessageBox.Icon.INFO,
            //        Width = 350
            //    });
            //    return this.Direct();
            //}

            if (SchemeFeeRepo.Flat_Or_Rate == "RATE" && SchemeFeeRepo.Flat_Or_Rate_Value > 100)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "ERROR",
                    Message = "Value can not be more than 100% .",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                return this.Direct();
            }

                //determine next accrual and apply date

                //if (SchemeFeeRepo.Accrual_Frequency == "DAILY")
                //{
                //    SchemeFeeRepo.Next_Accrual_Date = SchemeFeeRepo.First_Accrual_Date.AddDays(+1);
                //}

                //if (SchemeFeeRepo.Accrual_Frequency == "MONTHLY")
                //{
                //    SchemeFeeRepo.Next_Accrual_Date = SchemeFeeRepo.First_Accrual_Date.AddMonths(+1);
                //}

                //if (SchemeFeeRepo.Accrual_Frequency == "QUARTERLY")
                //{
                //    SchemeFeeRepo.Next_Accrual_Date = SchemeFeeRepo.First_Accrual_Date.AddMonths(+3);
                //}


                //if (SchemeFeeRepo.Apply_Frequency == "MONTHLY")
                //{
                //    SchemeFeeRepo.Next_Apply_Date = SchemeFeeRepo.First_Apply_Date.AddMonths(+1);
                //}

                //if (SchemeFeeRepo.Apply_Frequency == "QUARTERLY")
                //{
                //    SchemeFeeRepo.Next_Apply_Date = SchemeFeeRepo.First_Apply_Date.AddMonths(+3);
                //}


                if (ModelState.IsValid)


                    // GET PENSION FUND MANAGER ....ID
                    
            {
                    //check if fee is not more than maximum fee
                    SchemeFeeRepo.checkmaxfee(SchemeFeeRepo);
                    if (SchemeFeeRepo.Fee < SchemeFeeRepo.Flat_Or_Rate_Value)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Scheme Fee can not be more than maximum fee rate.",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });
                        return this.Direct();
                    }


                    this.SchemeFeeRepo.SaveRecordFM(SchemeFeeRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Scheme Fee Successfully Added.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                ClearControls();
                Store store = X.GetCmp<Store>("schemeFeeFMStore");
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
            catch (System.Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult ApproveRecord(string SCHEME_FEE_ID)
        {
            try
            { 
            if (SCHEME_FEE_ID == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! No 'scheme Fee' has been selected for approval.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

            SchemeFeeRepo.ApproveSchemeFeeRecord(SCHEME_FEE_ID);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Scheme Fee Successfully Approved.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350


            });
            ClearControls();
            Store store = X.GetCmp<Store>("schemeFeeStore");
            store.Reload();

            return this.Direct();
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public ActionResult DeleteRecord(pfm_SchemeFeeRepo SchemeFeeRepo)
        {
            try
            { 
            if (SchemeFeeRepo.Scheme_Fee_Id == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! No 'scheme Fee' has been selected.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

            //if (SchemeFeeRepo.Auth_Status != "PENDING")
            //{
            //    X.Msg.Show(new MessageBoxConfig
            //    {
            //        Title = "Error",
            //        Message = "Sorry! Scheme Fee can not be deleted.",
            //        Buttons = MessageBox.Button.OK,
            //        Icon = MessageBox.Icon.INFO,
            //        Width = 350
            //    });
            //    return this.Direct();
            //}

            SchemeFeeRepo.DeleteRecordFM( SchemeFeeRepo);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Scheme Fee Successfully Deleted.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350
            });
            ClearControls();
            Store store = X.GetCmp<Store>("schemeFeeStore");
            store.Reload();

            return this.Direct();
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
            return this.Store(SchemeFeeRepo.GetSchemeFMFeeList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public ActionResult ReadPending()
        {
            try
            { 
            return this.Store(SchemeFeeRepo.GetSchemeFeePendingList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}


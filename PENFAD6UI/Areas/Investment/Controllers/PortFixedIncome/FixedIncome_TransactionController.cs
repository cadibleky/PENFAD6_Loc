
using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Linq;
using System.Web.Mvc;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using PENFAD6DAL.Repository.Investment.Equity_CIS;
using System.Collections.Generic;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.Investment.FixedIncome_Transaction;

namespace PENFAD6UI.Areas.Investment.Controllers.PortFixedIncome
{
    public class FixedIncome_TransactionController : Controller  
    {
        readonly pfm_Scheme_FundRepo SFRepo = new pfm_Scheme_FundRepo();
        readonly pfm_FundManagerRepo FundManagerRepo = new pfm_FundManagerRepo();
        readonly Invest_Trans_Fix_Repo MM_TBill = new Invest_Trans_Fix_Repo();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddFixedIncomeTransTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "FixedDepositTransPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult GetMDate(Invest_Trans_Fix_Repo MM_TBill)
        {
            try
            {
                this.GetCmp<DateField>("MM_TBillMaturity_Date").SetValue(MM_TBill.Start_Date.AddDays(Convert.ToDouble(MM_TBill.Duration_In_Days)));
                return this.Direct();
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
            finally
            {

            }
        }
        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frm_FixedDepositTransPartial_main");
                x.Reset();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        //public void ClearControls_Approve()
        //{
        //    try
        //    {
        //        var x = X.GetCmp<FormPanel>("frm_ApproveFixedDepositTransPartial_main");
        //        x.Reset();
        //    }
        //    catch (System.Exception)
        //    {
        //        throw;
        //    }
        //}

        public ActionResult SaveRecord(Invest_Trans_Fix_Repo MM_TBill)

        {
            try
            {
                if (ModelState.IsValid)
                { 
                    if (MM_TBill.isOrderUnique(MM_TBill.Invest_No) == true)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Duplicate",
                        Message = "Investment already exist.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    return this.Direct();
                }

                    //check if issuer exist
                    MM_TBill.ck_issuer(MM_TBill);

                if ((MM_TBill.CH_NUMBER <= 0))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! Unregistered Issuer",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }
                //get GL balance from GL_Account Table 
                MM_TBill.Get_GL_Balance(MM_TBill);

                if ((MM_TBill.GL_Balance * -1) < MM_TBill.Amount_Invested)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! Cash balance is not enough for this investment",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }


                    //check for investment complaince - Asset
                    MM_TBill.check_compliance_inv(MM_TBill);
                    if (MM_TBill.CHECK_COM == 1)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! Asset allocation limit compliance issue. Process aborted",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });

                        return this.Direct();
                    }



                    if (!string.IsNullOrEmpty(MM_TBill.Scheme_Id))
                {
                    GlobalValue.Get_Scheme_Today_Date(MM_TBill.Scheme_Id);
                    if (MM_TBill.Settlement_Date != GlobalValue.Scheme_Today_Date)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! settlement date must be equal to scheme working date of " + GlobalValue.Scheme_Today_Date.Date.ToString(),
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
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
                        Message = "Scheme cannot be verified.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                }

                    this.MM_TBill.Add_Submit_Trans(MM_TBill);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Investment Successfully Processed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                    //ClearControls();
                    //Store store = X.GetCmp<Store>("MM_TBillSFStore");
                    //store.Reload();
                    //return this.Direct();
					var pvr = new Ext.Net.MVC.PartialViewResult
					{
						ViewName = "FixedDepositTransPartial",
						ContainerId = "MainArea",
						RenderMode = RenderMode.AddTo,
					};

					this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
					return pvr;
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
                    ora_code = "Not all records are supplied. Process aborted..";
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
                //log.Write(level: Serilog.Events.LogEventLevel.Information, messageTemplate: ex.Message + " " + DateTime.Now);
                return this.Direct();
            }
        }

       
        // filter Fund Manager for scheme
        public ActionResult GetFM(string Scheme_Id)
        {
            var misdepartmentrepo = new Invest_Trans_Fix_Repo();
            var mydata = misdepartmentrepo.GetFMList(Scheme_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.Fund_Manager_Id;
                string name = ddd.Fund_Manager;

                data.Add(new { Id = id, Name = name });
            }

            return this.Store(data);

        }        

        // filter GL Account for scheme
        public ActionResult GetpID(string Class_Id)
        {
            var misdepartmentrepo = new Invest_Trans_Fix_Repo();
            var mydata = misdepartmentrepo.GetECISPList(Class_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.Product_Id;
                string name = ddd.Product_Name;

                data.Add(new { pId = id, pName = name });
            }

            return this.Store(data);
        }


        // filter GL Account for scheme
        public ActionResult GetGLAB(string Scheme_Fund_Id)
        {
            var misdepartmentrepo = new Invest_Trans_Fix_Repo();
            var mydata = misdepartmentrepo.GetGLASFList(Scheme_Fund_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.GL_Account_No;
                string name = ddd.GL_Account_Name;

                data.Add(new { FDNEWId = id, gName = name });
            }

            return this.Store(data);
        }

      
        public ActionResult ReadSchemeFund()
        {
            try
            {
                return this.Store(SFRepo.GetSchemeFundList());
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
        }

        // filter GL Account for scheme
        public ActionResult GetGLacc(string GL_Account_No)
        {
            var misdepartmentrepo = new Invest_Equity_CISRepo();
            var mydata = misdepartmentrepo.GetGLAccFList(GL_Account_No);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                decimal id = ddd.GL_Balance * -1;
                decimal name = ddd.GL_Balance * -1;

                data.Add(new { FDNEWId_B = id, mName = name });
            }

            return this.Store(data);
        }


        public ActionResult calValues(Invest_Trans_Fix_Repo MM_TBill)
        {
            try
            {
                this.GetCmp<DateField>("MM_TBillMaturity_Date").SetValue(MM_TBill.Start_Date.AddDays(Convert.ToDouble(MM_TBill.Duration_In_Days)));

                MM_TBill.Daily_Int_Rate = ((MM_TBill.Annual_Int_Rate / 100) / MM_TBill.Interest_Day_Basic);
               MM_TBill.Interest_On_Maturity = (MM_TBill.Daily_Int_Rate * MM_TBill.Amount_Invested * MM_TBill.Duration_In_Days);
               MM_TBill.Amount_on_Maturity = (MM_TBill.Interest_On_Maturity + MM_TBill.Amount_Invested);

                this.GetCmp<Hidden>("txt_MM_TBill_Daily_Int_Rate").SetValue((MM_TBill.Annual_Int_Rate /100)/MM_TBill.Interest_Day_Basic );
                this.GetCmp<TextField>("frmInterest_On_Maturity").SetValue(MM_TBill.Daily_Int_Rate * MM_TBill.Amount_Invested * MM_TBill.Duration_In_Days );
                this.GetCmp<TextField>("frmAmount_on_txtMaturity").SetValue(MM_TBill.Interest_On_Maturity + MM_TBill.Amount_Invested);
                return this.Direct();
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
            finally
            {

            }
        }

    }
}




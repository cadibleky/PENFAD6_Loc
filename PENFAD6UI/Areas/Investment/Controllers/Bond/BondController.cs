
using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Linq;
using System.Web.Mvc;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using PENFAD6DAL.Repository.Investment.Equity_CIS;
using System.Collections.Generic;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.Investment.Bond;
using PENFAD6DAL.Repository.Investment.FixedIncome_Transaction;

namespace PENFAD6UI.Areas.Investment.Controllers.Bond
{
    public class BondController : Controller  
    {
        readonly pfm_Scheme_FundRepo SFRepo = new pfm_Scheme_FundRepo();
        readonly pfm_FundManagerRepo FundManagerRepo = new pfm_FundManagerRepo();
        readonly BondRepo MM_TBill = new BondRepo();
        readonly Invest_Trans_Fix_Repo TransFixRepo = new Invest_Trans_Fix_Repo();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddBondTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "BondPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddSecBondTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "SecBondPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddSellSecBondTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "SellSecBondPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddSellSecBondReverseTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "SellReversePartial_Bonds",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }
        public ActionResult GetMDate(BondRepo MM_TBill)
        {
            try
            {
                this.GetCmp<DateField>("MM_TBillMaturity_Date_Bond").SetValue(MM_TBill.Start_Date.AddDays(Convert.ToDouble(MM_TBill.Duration_In_Days)));
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

        //get maturity date for secondary bonds      
        public ActionResult Sec_GetMDate(BondRepo MM_TBill)
        {
            try
            {
                this.GetCmp<DateField>("MM_TBillMaturity_Date_SecBond").SetValue(MM_TBill.Start_Date.AddDays(Convert.ToDouble(MM_TBill.Duration_In_Days)));
                //this.GetCmp<NumberField>("ff").SetValue(MM_TBill.Last_Coupon_Payment_Date.AddDays(182));

                ////// get duration bond will run
                //this.GetCmp<NumberField>("ttd").SetValue(MM_TBill.Amount_Invested);
                //MM_TBill.Trans_Duration = (MM_TBill.Maturity_Date.Date - MM_TBill.Last_Coupon_Payment_Date.Date).Days;
                //this.GetCmp<NumberField>("ttss").SetValue(MM_TBill.Trans_Duration);
                //this.GetCmp<NumberField>("frmInterest_On_Maturity_SecBond").SetValue(MM_TBill.Daily_Int_Rate * MM_TBill.Base_Norminal_Value * MM_TBill.Trans_Duration);
                //MM_TBill.Interest_On_Maturity = MM_TBill.Daily_Int_Rate * MM_TBill.Base_Norminal_Value * MM_TBill.Trans_Duration;
                //this.GetCmp<NumberField>(
                //    "frmAmount_on_txtMaturity_SecBond").SetValue(MM_TBill.Base_Norminal_Value + MM_TBill.Interest_On_Maturity);

               

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

        //get trans duration and future values
        public ActionResult Sec_GetDuration(BondRepo MM_TBill)
        {
            try
            {

                this.GetCmp<NumberField>("ttd").SetValue(MM_TBill.Amount_Invested);
                MM_TBill.Trans_Duration = (MM_TBill.Maturity_Date.Date - MM_TBill.Last_Coupon_Payment_Date.Date).Days;
                this.GetCmp<NumberField>("ttss").SetValue(MM_TBill.Trans_Duration);
                this.GetCmp<NumberField>("frmInterest_On_Maturity_SecBond").SetValue(MM_TBill.Daily_Int_Rate * MM_TBill.Base_Norminal_Value * MM_TBill.Trans_Duration);
                MM_TBill.Interest_On_Maturity = MM_TBill.Daily_Int_Rate * MM_TBill.Base_Norminal_Value * MM_TBill.Trans_Duration;
                this.GetCmp<NumberField>(
                    "frmAmount_on_txtMaturity_SecBond").SetValue(MM_TBill.Base_Norminal_Value + MM_TBill.Interest_On_Maturity);

                this.GetCmp<NumberField>("ff").SetValue(MM_TBill.Last_Coupon_Payment_Date.AddDays(182));              
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

        public ActionResult Sec_GetInitialLastDate(BondRepo MM_TBill)
        {
            try
            {
             
                this.GetCmp<NumberField>("dd").SetValue(MM_TBill.Start_Date);
                MM_TBill.Last_Coupon_Payment_Date = MM_TBill.Start_Date.Date;

                //get duration bond will run
                this.GetCmp<NumberField>("ttd").SetValue(MM_TBill.Amount_Invested);
                MM_TBill.Trans_Duration = (MM_TBill.Maturity_Date.Date - MM_TBill.Last_Coupon_Payment_Date.Date).Days;
                this.GetCmp<NumberField>("ttss").SetValue(MM_TBill.Trans_Duration);
                this.GetCmp<NumberField>("frmInterest_On_Maturity_SecBond").SetValue(MM_TBill.Daily_Int_Rate * MM_TBill.Base_Norminal_Value * MM_TBill.Trans_Duration);
                MM_TBill.Interest_On_Maturity = MM_TBill.Daily_Int_Rate * MM_TBill.Base_Norminal_Value * MM_TBill.Trans_Duration;
                this.GetCmp<NumberField>(
                    "frmAmount_on_txtMaturity_SecBond").SetValue(MM_TBill.Base_Norminal_Value + MM_TBill.Interest_On_Maturity);

                this.GetCmp<NumberField>("ff").SetValue(MM_TBill.Last_Coupon_Payment_Date.AddDays(182));


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


        public void ClearControls_Sec()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frm_SecBondPartial_main");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frm_BondPartial_main");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }
       
        public ActionResult SaveRecord(BondRepo MM_TBill)
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

                    MM_TBill.GL_Balance = Math.Round(MM_TBill.GL_Balance, 2);
                    MM_TBill.Amount_Invested = Math.Round(MM_TBill.Amount_Invested, 2);

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
                if (!string.IsNullOrEmpty(MM_TBill.Scheme_Id))
                {
                    GlobalValue.Get_Scheme_Today_Date(MM_TBill.Scheme_Id);
                    if (MM_TBill.Settlement_Date != GlobalValue.Scheme_Today_Date)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! Settlement date must be equal to scheme working date of " + GlobalValue.Scheme_Today_Date.Date.ToString(),
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
					//Store store = X.GetCmp<Store>("BondSFStore");
					//store.Reload();
					//return this.Direct();
					var pvr = new Ext.Net.MVC.PartialViewResult
					{
						ViewName = "BondPartial",
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

        public ActionResult Sec_SaveRecord(BondRepo MM_TBill)
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

                    if ((MM_TBill.Cost) <= 0)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! Invalid Cost",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });

                        return this.Direct();
                    }
            

                    //get GL balance from GL_Account Table 
                    MM_TBill.Get_GL_Balance(MM_TBill);
                    MM_TBill.GL_Balance = Math.Round(MM_TBill.GL_Balance, 2);
                    MM_TBill.Cost = Math.Round(MM_TBill.Cost, 2);
                    MM_TBill.Brokerage_Fee = Math.Round(MM_TBill.Brokerage_Fee, 2);

                    if ((MM_TBill.GL_Balance * -1) < MM_TBill.Cost + MM_TBill.Brokerage_Fee)
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

                    if (MM_TBill.Last_Coupon_Payment_Date < MM_TBill.Start_Date)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! Last Coupon Payment Date can not be before Start Date",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350
                        });

                        return this.Direct();
                    }

                    if (MM_TBill.Amount_Invested != MM_TBill.Cost && MM_TBill.Current_Yield <=0)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Invalid Current Yield. Please enter a valid Yield.",
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
                                Message = "Sorry! Settlement date must be equal to scheme working date of " + GlobalValue.Scheme_Today_Date.Date.ToString(),
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

                    this.MM_TBill.Add_Submit_Trans_Sec(MM_TBill);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Investment Successfully Processed.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
					//ClearControls_Sec();
					//Store store = X.GetCmp<Store>("SecBondSFStore");
					//store.Reload();
					//return this.Direct();
					var pvr = new Ext.Net.MVC.PartialViewResult
					{
						ViewName = "SecBondPartial",
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
            var misdepartmentrepo = new BondRepo();
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
            var misdepartmentrepo = new BondRepo();
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
            var misdepartmentrepo = new BondRepo();
            var mydata = misdepartmentrepo.GetGLASFList(Scheme_Fund_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.GL_Account_No;
                string name = ddd.GL_Account_Name;

                data.Add(new { bondnewId = id, gName = name });
            }

            return this.Store(data);
        }

        // filter GL Account for scheme-sec bond purchase
        public ActionResult Sec_GetGLAB(string Scheme_Fund_Id)
        {
            var misdepartmentrepo = new BondRepo();
            var mydata = misdepartmentrepo.GetGLASFList(Scheme_Fund_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.GL_Account_No;
                string name = ddd.GL_Account_Name;

                data.Add(new { secbondnewId = id, gName = name });
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

                data.Add(new { bondnewId_B = id, mName = name });
            }

            return this.Store(data);
        }

        // filter GL Account for scheme - sec bond purchase
        public ActionResult Sec_GetGLacc(string GL_Account_No)
        {
            var misdepartmentrepo = new Invest_Equity_CISRepo();
            var mydata = misdepartmentrepo.GetGLAccFList(GL_Account_No);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                decimal id = ddd.GL_Balance * -1;
                decimal name = ddd.GL_Balance * -1;

                data.Add(new { secbondnewId_B = id, mName = name });
            }

            return this.Store(data);
        }
        public ActionResult ReadBondClass(BondRepo MM_TBill)
        {
            MM_TBill.GetBondClassList(MM_TBill);
            X.GetCmp<Hidden>("txt_Bond_Class").SetValue(MM_TBill.Bond_Class);
           // X.GetCmp<TextField>("txt_SecBond_Class").SetValue(MM_TBill.Bond_Class);
            return this.Direct();
        }

        public ActionResult ReadSecBondClass(BondRepo MM_TBill)
        {
            MM_TBill.GetBondClassList(MM_TBill);
            // X.GetCmp<Hidden>("txt_Bond_Class").SetValue(MM_TBill.Bond_Class);
            X.GetCmp<Hidden>("txt_SecBond_Class").SetValue(MM_TBill.Bond_Class);
            return this.Direct();
        }


        public ActionResult Sec_ReadPending_Transaction()
        {
            try
            {
                return this.Store(MM_TBill.GetSchemeFundProductListbond());
            }
            catch (Exception ex)
            {
                return this.Direct();
            }
        }

          public ActionResult calValues(BondRepo MM_TBill)
        {
            try
            {
                this.GetCmp<DateField>("MM_TBillMaturity_Date_Bond").SetValue(MM_TBill.Start_Date.AddDays(Convert.ToDouble(MM_TBill.Duration_In_Days)));

                MM_TBill.Daily_Int_Rate = ((MM_TBill.Annual_Int_Rate / 100) / MM_TBill.Interest_Day_Basic);
                MM_TBill.Interest_On_Maturity = (MM_TBill.Daily_Int_Rate * MM_TBill.Amount_Invested * MM_TBill.Trans_Duration);
                MM_TBill.Amount_on_Maturity = (MM_TBill.Interest_On_Maturity + MM_TBill.Amount_Invested);

                this.GetCmp<Hidden>("txt_MM_TBill_Daily_Int_Rate_Bond").SetValue((MM_TBill.Annual_Int_Rate / 100) / MM_TBill.Interest_Day_Basic);
                this.GetCmp<TextField>("frmInterest_On_Maturity_Bond").SetValue(MM_TBill.Daily_Int_Rate * MM_TBill.Amount_Invested * MM_TBill.Duration_In_Days);
                this.GetCmp<TextField>("frmAmount_on_txtMaturity_Bond").SetValue(MM_TBill.Interest_On_Maturity + MM_TBill.Amount_Invested);
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

        public ActionResult calValuessec(BondRepo MM_TBill)
        {
            try
            {
                this.GetCmp<DateField>("MM_TBillMaturity_Date_SecBond").SetValue(MM_TBill.Start_Date.AddDays(Convert.ToDouble(MM_TBill.Duration_In_Days)));
                MM_TBill.Daily_Int_Rate = ((MM_TBill.Annual_Int_Rate / 100) / MM_TBill.Interest_Day_Basic);
                //this.GetCmp<DateField>("dd").SetValue(MM_TBill.Start_Date);
                //MM_TBill.Last_Coupon_Payment_Date = MM_TBill.Start_Date;
                //MM_TBill.Amount_on_Maturity = (MM_TBill.Interest_On_Maturity + MM_TBill.Amount_Invested);

                this.GetCmp<Hidden>("txt_MM_TBill_Daily_Int_Rate_SecBond").SetValue((MM_TBill.Annual_Int_Rate / 100) / MM_TBill.Interest_Day_Basic);
                //this.GetCmp<TextField>("frmInterest_On_Maturity_Bond").SetValue(MM_TBill.Daily_Int_Rate * MM_TBill.Amount_Invested * MM_TBill.Duration_In_Days);
                //this.GetCmp<TextField>("frmAmount_on_txtMaturity_Bond").SetValue(MM_TBill.Interest_On_Maturity + MM_TBill.Amount_Invested);


                this.GetCmp<NumberField>("ttd").SetValue(MM_TBill.Amount_Invested);
                MM_TBill.Trans_Duration = (MM_TBill.Maturity_Date.Date - MM_TBill.Last_Coupon_Payment_Date.Date).Days;
                this.GetCmp<NumberField>("ttss").SetValue(MM_TBill.Trans_Duration);
                this.GetCmp<NumberField>("frmInterest_On_Maturity_SecBond").SetValue(MM_TBill.Daily_Int_Rate * MM_TBill.Base_Norminal_Value * MM_TBill.Trans_Duration);
                MM_TBill.Interest_On_Maturity = MM_TBill.Daily_Int_Rate * MM_TBill.Base_Norminal_Value * MM_TBill.Trans_Duration;
                this.GetCmp<NumberField>(
                    "frmAmount_on_txtMaturity_SecBond").SetValue(MM_TBill.Base_Norminal_Value + MM_TBill.Interest_On_Maturity);

                this.GetCmp<DateField>("ff").SetValue(MM_TBill.Last_Coupon_Payment_Date.AddDays(182));

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

        public ActionResult calValuessec1(BondRepo MM_TBill)
        {
            try
            {
                this.GetCmp<DateField>("MM_TBillMaturity_Date_SecBond").SetValue(MM_TBill.Start_Date.AddDays(Convert.ToDouble(MM_TBill.Duration_In_Days)));
                MM_TBill.Daily_Int_Rate = ((MM_TBill.Annual_Int_Rate / 100) / MM_TBill.Interest_Day_Basic);
                this.GetCmp<DateField>("dd").SetValue(MM_TBill.Start_Date);
                MM_TBill.Last_Coupon_Payment_Date = MM_TBill.Start_Date;
                //MM_TBill.Amount_on_Maturity = (MM_TBill.Interest_On_Maturity + MM_TBill.Amount_Invested);

                this.GetCmp<Hidden>("txt_MM_TBill_Daily_Int_Rate_SecBond").SetValue((MM_TBill.Annual_Int_Rate / 100) / MM_TBill.Interest_Day_Basic);
                //this.GetCmp<TextField>("frmInterest_On_Maturity_Bond").SetValue(MM_TBill.Daily_Int_Rate * MM_TBill.Amount_Invested * MM_TBill.Duration_In_Days);
                //this.GetCmp<TextField>("frmAmount_on_txtMaturity_Bond").SetValue(MM_TBill.Interest_On_Maturity + MM_TBill.Amount_Invested);


                this.GetCmp<NumberField>("ttd").SetValue(MM_TBill.Amount_Invested);
                MM_TBill.Trans_Duration = (MM_TBill.Maturity_Date.Date - MM_TBill.Last_Coupon_Payment_Date.Date).Days;
                this.GetCmp<NumberField>("ttss").SetValue(MM_TBill.Trans_Duration);
                this.GetCmp<NumberField>("frmInterest_On_Maturity_SecBond").SetValue(MM_TBill.Daily_Int_Rate * MM_TBill.Base_Norminal_Value * MM_TBill.Trans_Duration);
                MM_TBill.Interest_On_Maturity = MM_TBill.Daily_Int_Rate * MM_TBill.Base_Norminal_Value * MM_TBill.Trans_Duration;
                this.GetCmp<NumberField>(
                    "frmAmount_on_txtMaturity_SecBond").SetValue(MM_TBill.Base_Norminal_Value + MM_TBill.Interest_On_Maturity);

                this.GetCmp<DateField>("ff").SetValue(MM_TBill.Last_Coupon_Payment_Date.AddDays(182));

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


        public ActionResult AddSellRecordSec(BondRepo MM_TBill)
        {
            try
            {
              
                   
                   
                    if (string.IsNullOrEmpty(MM_TBill.Invest_No))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Please select investment. Process aborted",
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
                                Message = "Sorry! date must be equal to scheme working date of " + GlobalValue.Scheme_Today_Date.Date.ToString(),
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

                    this.MM_TBill.Add_Submit_Trans_Sec_Sell(MM_TBill);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Transaction Successfully Processed.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    //ClearControls_Sec();
                    //Store store = X.GetCmp<Store>("SecBondSFStore");
                    //store.Reload();
                    //return this.Direct();
                    var pvr = new Ext.Net.MVC.PartialViewResult
                    {
                        ViewName = "SellSecBondPartial",
                        ContainerId = "MainArea",
                        RenderMode = RenderMode.AddTo,
                    };
                    this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
                    return pvr;
               
            }
            catch (Exception ex)
            {
                string ora_code = ex.Message.Substring(0, 9);
                if (ora_code == "ORA-20000")
                {
                    ora_code = "Error. Process aborted..";
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


        public ActionResult ReadMB_Transaction()
        {
            return this.Store(MM_TBill.GetInvest_FixedIncome_List_Pay21());
        }


        // SELL BONDS
        public ActionResult ReceiptMBondRecord(BondRepo BondRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(BondRepo.Invest_No))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Investment has been selected for approval.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //get record
                BondRepo.GetFMRecord(BondRepo);

                if (string.IsNullOrEmpty(BondRepo.Receipt_Date.ToString()))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Date is required.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                if (!string.IsNullOrEmpty(BondRepo.Scheme_Id))
                {
                    GlobalValue.Get_Scheme_Today_Date(BondRepo.Scheme_Id);
                    if (BondRepo.Receipt_Date > GlobalValue.Scheme_Today_Date)
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! Date must be equal to scheme working date of " + GlobalValue.Scheme_Today_Date.Date.ToString(),
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350

                        });

                        return this.Direct();
                    }
                }
                else
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Scheme cannot be verified.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                }

                //if (BondRepo.Principal_Amount > 0 && BondRepo.Invest_Status == "ACTIVE")
                //{
                //    X.Mask.Hide();
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Please Principal can not be received. Process aborted",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });

                //    return this.Direct();
                //}

                if (BondRepo.Principal_Amount <= 0 && BondRepo.Interest_Amount <= 0)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please enter 'Principal Amount' or 'Interest Amount' to confirm investment payment",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                if (BondRepo.Principal_Amount > BondRepo.Principal_Bal)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Principal Amount can not be more than Principal Balance. Process aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                if (BondRepo.Interest_Amount > BondRepo.Interest_Accrued)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Interest Amount can not be more than Accrued Interest. Process aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }


                if (string.IsNullOrEmpty(BondRepo.Credit_Account_No))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Credit Bank Account is required.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                BondRepo.Interest_Accrued = BondRepo.Interest_Accrued - BondRepo.Interest_Amount;
                BondRepo.Interest_Paid = BondRepo.Interest_Paid + BondRepo.Interest_Amount;
                BondRepo.Interest_Bal = BondRepo.Interest_Bal - BondRepo.Interest_Amount;
                BondRepo.Principal_Paid = BondRepo.Principal_Paid + BondRepo.Principal_Amount;
                BondRepo.Principal_Bal = BondRepo.Principal_Bal - BondRepo.Principal_Amount;

                BondRepo.Receipt_Bond_Sell(BondRepo);
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Investment Payment Successfully Confirmed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                //ClearControlsMB();
                //Store store = X.GetCmp<Store>("ReceiptMBfixedincomestore");
                //store.RemoveAll();

                //return this.Direct();
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "SellSecBondPartial",
                    ContainerId = "MainArea",
                    RenderMode = RenderMode.AddTo,
                };
                this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
                return pvr;
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
                return this.Direct();
            }
        }

        public ActionResult rp_GLBMB_GetGLAB(string Scheme_Fund_Id)
        {
            var misdepartmentrepo = new Invest_Trans_Fix_Repo();
            var mydata = misdepartmentrepo.GetGLASFList(Scheme_Fund_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.GL_Account_No;
                string name = ddd.GL_Account_Name;

                data.Add(new { rp_GLBMB_gId = id, gName = name });
            }

            return this.Store(data);
        }

        public ActionResult ReverseReadPendingBonds_Transaction(Invest_Trans_Fix_Repo MM_TBILL)
        {
            Store store = X.GetCmp<Store>("ReverseSaleBondsstore");
            store.Reload();
            store.DataBind();
            return this.Store(TransFixRepo.ReverseReadPendingBonds_TransactionSell(MM_TBILL));
        }

        // REVERSE BOND SALE
        public ActionResult ReverseReceiptRecord_Bonds(Invest_Trans_Fix_Repo TransFixRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(TransFixRepo.TID))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Investment Sale has been selected for reversal.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //get record
                TransFixRepo.GetFMRecord(TransFixRepo);

                if (!string.IsNullOrEmpty(TransFixRepo.Scheme_Id))
                {
                    GlobalValue.Get_Scheme_Today_Date(TransFixRepo.Scheme_Id);
                    if (TransFixRepo.Receipt_Date > GlobalValue.Scheme_Today_Date)
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! This transaction can not be reversed. Process aborted",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Width = 350

                        });

                        return this.Direct();
                    }
                }
                else
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Scheme cannot be verified.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                }

                //TransFixRepo.Interest_Accrued = TransFixRepo.Interest_Accrued + TransFixRepo.Interest_Amount;
                //TransFixRepo.Interest_Paid = TransFixRepo.Interest_Paid - TransFixRepo.Interest_Amount;
                //TransFixRepo.Interest_Bal = TransFixRepo.Interest_Bal + TransFixRepo.Interest_Amount;
                //TransFixRepo.Principal_Paid = TransFixRepo.Principal_Paid - TransFixRepo.Principal_Amount;
                //TransFixRepo.Principal_Bal = TransFixRepo.Principal_Bal + TransFixRepo.Principal_Amount;

                TransFixRepo.Sale_MM_TBill_Reverse(TransFixRepo);
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Investment Sale Successfully Reversed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                //ClearControls_ReverseR_Bonds();
                //Store store = X.GetCmp<Store>("ReverseReceiptBondsstore");
                //store.RemoveAll();

                //return this.Direct();
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "SellReversePartial_Bonds",
                    ContainerId = "MainArea",
                    RenderMode = RenderMode.AddTo,
                };
                this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
                return pvr;
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
                return this.Direct();
            }
        }


    }
}




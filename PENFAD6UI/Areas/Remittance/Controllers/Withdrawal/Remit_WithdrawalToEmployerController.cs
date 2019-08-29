using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employee;
using PENFAD6DAL.Repository.CRM.Employer;
using PENFAD6DAL.Repository.Investment.Bond;
using PENFAD6DAL.Repository.Investment.Equity_CIS;
using PENFAD6DAL.Repository.Remittance.Contribution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Remittance.Controllers.Withdrawal
{
    public class Remit_WithdrawalToEmployerController : Controller
    {
        readonly crm_EmployeeSchemeFundRepo ESFRepo = new crm_EmployeeSchemeFundRepo();
        Remit_WithdrawalRepo WithdrawalRepo = new Remit_WithdrawalRepo();
        readonly crm_EmployeeRepo employee = new crm_EmployeeRepo();
        readonly Remit_ReceiptRepo ReceiptRepo = new Remit_ReceiptRepo();

        public ActionResult AddWithdrawalTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "WithdrawalPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }
        public ActionResult AddWithdrawalApproveTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "WithdrawalApprovePartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        //public ActionResult AddWithdrawalPayTab(string containerId = "MainArea")
        //{
        //    var pvr = new Ext.Net.MVC.PartialViewResult
        //    {
        //        ViewName = "WithdrawalPayPartial",
        //        ContainerId = containerId,
        //        RenderMode = RenderMode.AddTo,
        //    };
        //    this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
        //    return pvr;
        //}

        //public ActionResult AddWithdrawalPayTaxTab(string containerId = "MainArea")
        //{
        //    var pvr = new Ext.Net.MVC.PartialViewResult
        //    {
        //        ViewName = "WithdrawalPayTaxPartial",
        //        ContainerId = containerId,
        //        RenderMode = RenderMode.AddTo,
        //    };
        //    this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
        //    return pvr;
        //}

        public ActionResult AddWithdrawalReverseTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "WithdrawalReversePartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public void ClearControls_Approve()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmWithdrawalApproveP_Main");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        public void ClearControls_Pay()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmWithdrawalPayP_Main");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void ClearControls_Reverse()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmWithdrawalReverseP_Main");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void ClearControls_PayTax()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmWithdrawalPayTaxP_Main");
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
                var x = X.GetCmp<FormPanel>("frm_WithdrawalP");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        //public ActionResult AddReversePurchaseTab(string containerId = "MainArea")
        //{
        //    var pvr = new Ext.Net.MVC.PartialViewResult
        //    {
        //        ViewName = "ReversePurchasePartial",
        //        ContainerId = containerId,
        //        RenderMode = RenderMode.AddTo,
        //    };
        //    this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
        //    return pvr;
        //}


        public ActionResult SaveRecord(Remit_WithdrawalRepo WithdrawalRepo, Remit_ReceiptRepo ReceiptRepo)
        {
            try
            {
                
                if (string.IsNullOrEmpty(ReceiptRepo.ES_Id))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select Employer Account",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                if (string.IsNullOrEmpty(WithdrawalRepo.GL_Account_No))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select Bank Account",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                if (!WithdrawalRepo.Trans_Request_Date.HasValue)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Actual Request Date is a required field",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }



                if (string.IsNullOrEmpty(WithdrawalRepo.Withdrawal_Reason))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Withdrawal Reason is a required field",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }


                // check if there is a pending request for the employee scheme fund

                if (WithdrawalRepo.GetcheckESFListEmp(WithdrawalRepo) == true)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! There is already a pending request for the Employee Account",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                // validate scheme date             
                if (!string.IsNullOrEmpty(WithdrawalRepo.Scheme_Id))
                {
                    GlobalValue.Get_Scheme_Today_Date(WithdrawalRepo.Scheme_Id);
                    if (WithdrawalRepo.Trans_Request_Date != GlobalValue.Scheme_Today_Date)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! Request date must be equal to scheme working date " ,
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
                        Message = "Employee account cannot be verified.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                }

                ////get GL balance from GL_Account Table 
                //WithdrawalRepo.Get_GL_Balance(WithdrawalRepo);

                //if (WithdrawalRepo.Total_Withdrawal_Amount > WithdrawalRepo.GL_Balance * -1)
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Sorry! Insufficient Account Balance.Process aborted.",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.ERROR,
                //        Width = 350
                //    });
                //    return this.Direct();
                //}

                WithdrawalRepo.GetESFList(WithdrawalRepo.ESF_Id);

                WithdrawalRepo.Total_Withdrawal_Unit = Math.Round(WithdrawalRepo.Total_Withdrawal_Unit, 4);
                WithdrawalRepo.Total_Unit_Balance = Math.Round(WithdrawalRepo.Total_Unit_Balance, 4);

                if (WithdrawalRepo.Total_Withdrawal_Unit > WithdrawalRepo.Total_Unit_Balance)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry!Total Withdrawal(Unit) can not be more than Total Unit",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }


                //if (ModelState.IsValid)
                //{
                    this.WithdrawalRepo.SaveRecord_TE(WithdrawalRepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Benefit Transfer Successfully Processed.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
					// ClearControls();
					//Store store = X.GetCmp<Store>("WithdrawalESFStore");
					//store.Reload();
					//return this.Direct();
					var pvr = new Ext.Net.MVC.PartialViewResult
					{
						ViewName = "WithdrawalPartial",
						ContainerId = "MainArea",
						RenderMode = RenderMode.AddTo,
					};
					this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
					return pvr;
				//}
                //else
                //{
                //    string messages = string.Join(Environment.NewLine, ModelState.Values
                //                       .SelectMany(x => x.Errors)
                //                       .Select(x => x.ErrorMessage));
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = messages, // " Insufficient data. Operation Aborted",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.ERROR,
                //        Width = 350
                //    });
                //    return this.Direct();
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult ApproveRecord(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                if (WithdrawalRepo.Withdrawal_No == string.Empty)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Request has been selected for approval.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }


                WithdrawalRepo.GetESFList(WithdrawalRepo.ESF_Id);

                WithdrawalRepo.Total_Withdrawal_Unit = Math.Round(WithdrawalRepo.Total_Withdrawal_Unit, 4);
                WithdrawalRepo.Total_Unit_Balance = Math.Round(WithdrawalRepo.Total_Unit_Balance, 4);

                if (WithdrawalRepo.Total_Withdrawal_Unit > WithdrawalRepo.Total_Unit_Balance)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry!Total Withdrawal(Unit) can not be more than Total Unit.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                GlobalValue.Get_Scheme_Today_Date(WithdrawalRepo.Scheme_Id);
                ///approve pending withdrawal

                WithdrawalRepo.Approve_Unit_WithdrawalTE(WithdrawalRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Benefit Request Successfully Approved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
				//ClearControls_Approve();
				//Store store = X.GetCmp<Store>("ApproveRequestWithStore");
				//store.Reload();

				//return this.Direct();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "WithdrawalApprovePartial",
					ContainerId = "MainArea",
					RenderMode = RenderMode.AddTo,
				};
				this.GetCmp<TabPanel>("MianArea").SetLastTabAsActive();
				return pvr;
			}
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult PayRecord(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                if (string.IsNullOrEmpty( WithdrawalRepo.Withdrawal_No))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Benefit has been selected for Payment.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                if (string.IsNullOrEmpty(WithdrawalRepo.GL_Account_No))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select Bank Account.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }


              //  WithdrawalRepo.GetESFList(WithdrawalRepo.ESF_Id);

                //get GL balance from GL_Account Table 
                WithdrawalRepo.Get_GL_Balance(WithdrawalRepo);

                if (WithdrawalRepo.Total_Withdrawal_Amount > WithdrawalRepo.GL_Balance * -1)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! Insufficient Account Balance.Process aborted.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }



                if (string.IsNullOrEmpty(WithdrawalRepo.Payment_Mode))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Payment Mode is required",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //if (WithdrawalRepo.Payment_Mode != "CASH" && string.IsNullOrEmpty(WithdrawalRepo.Instrument_No) )
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Instrument Numebr is required.",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });
                //    return this.Direct();
                //}

                if (!WithdrawalRepo.Pay_Date_Benefit.HasValue)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Payment Date is required",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }
                ///pay pending withdrawal

                WithdrawalRepo.Pay_Unit_Withdrawal(WithdrawalRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Benefit Request Successfully Paid.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
				//ClearControls_Pay();
				//Store store = X.GetCmp<Store>("PayRequestWithStore");
				//store.Reload();
				//return this.Direct();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "WithdrawalPayPartial",
					ContainerId = "MainArea",
					RenderMode = RenderMode.AddTo,
				};
				this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
				return pvr;
			}
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Reverse benefit payment
        public ActionResult ReversePayRecord(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(WithdrawalRepo.Withdrawal_No))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Transfer has been selected for reversal.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                if (string.IsNullOrEmpty(WithdrawalRepo.Reverse_Reason))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please enter Reason for Reversal. Process aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //// validate scheme date             
                //if (!string.IsNullOrEmpty(WithdrawalRepo.Scheme_Id))
                //{
                    GlobalValue.Get_Scheme_Today_Date(WithdrawalRepo.Scheme_Id);
                //    if (WithdrawalRepo.Trans_Request_Date != GlobalValue.Scheme_Today_Date)
                //    {
                //        X.Msg.Show(new MessageBoxConfig
                //        {
                //            Title = "Error",
                //            Message = "Sorry! Request date must be equal to scheme working date ",
                //            Buttons = MessageBox.Button.OK,
                //            Icon = MessageBox.Icon.INFO,
                //            Width = 350

                //        });

                //        return this.Direct();
                //    }
                //}
                //else
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Employee account cannot be verified.",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350

                //    });
                //}

                WithdrawalRepo.Reverse_Unit_WithdrawalTE(WithdrawalRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Benefit Trabsfer Successfully Reversed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
				//ClearControls_Reverse();
				//Store store = X.GetCmp<Store>("ReverseRequestWithStore");
				//store.Reload();

				//return this.Direct();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "WithdrawalReversePartial",
					ContainerId = "MainArea",
					RenderMode = RenderMode.AddTo,
				};
				this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
				return pvr;
			}
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult PayTaxRecord(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(WithdrawalRepo.Withdrawal_No))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Benefit has been selected for Payment.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                if (string.IsNullOrEmpty(WithdrawalRepo.GL_Account_No))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select Bank Account.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }


                WithdrawalRepo.GetESFList(WithdrawalRepo.ESF_Id);

                //get GL balance from GL_Account Table 
                WithdrawalRepo.Get_GL_Balance(WithdrawalRepo);

                if (WithdrawalRepo.Tax > WithdrawalRepo.GL_Balance * -1)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! Insufficient Account Balance.Process aborted.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }



                if (string.IsNullOrEmpty(WithdrawalRepo.Payment_Mode))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Payment Mode is required",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //if (WithdrawalRepo.Payment_Mode != "CASH" && string.IsNullOrEmpty(WithdrawalRepo.Instrument_No))
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Instrument Numebr is required.",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });

                //    return this.Direct();
                //}

                if (!WithdrawalRepo.Pay_Date_Benefit.HasValue)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Payment Date is required",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }


                ///pay pending tax

                WithdrawalRepo.PayTax_Unit_Withdrawal(WithdrawalRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Benefit Tax Successfully Paid.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
				//ClearControls_PayTax();
				//Store store = X.GetCmp<Store>("PayTaxRequestWithStore");
				//store.Reload();

				//return this.Direct();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "WithdrawalPayTaxPartial",
					ContainerId = "MainArea",
					RenderMode = RenderMode.AddTo,
				};
				this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
				return pvr;
			}
            catch (Exception ex)
            {
                throw ex;
            }
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

                data.Add(new { wTEPay_GLB_gId = id, gName = name });
            }

            return this.Store(data);
        }

        // filter GL Account for scheme 
        public ActionResult GetGLASFListBTE(string Scheme_Fund_Id)
        {
            var misdepartmentrepo = new BondRepo();
            var mydata = misdepartmentrepo.GetGLASFListBTE(Scheme_Fund_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.GL_Account_No;
                string name = ddd.GL_Account_Name;

                data.Add(new { wTEPay_GLB_gId = id, gName = name });
            }

            return this.Store(data);
        }

        // filter GL Account for scheme
        public ActionResult GetGLAB2(string Scheme_Fund_Id)
        {
            var misdepartmentrepo = new BondRepo();
            var mydata = misdepartmentrepo.GetGLASFList(Scheme_Fund_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.GL_Account_No;
                string name = ddd.GL_Account_Name;

                data.Add(new { TAXdId = id, dName = name });
            }

            return this.Store(data);
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

                data.Add(new { TAXmId = id, mName = name });
            }

            return this.Store(data);
        }

        // filter GL Account for scheme
        public ActionResult GetGLaccPay(string GL_Account_No)
        {
            var misdepartmentrepo = new Invest_Equity_CISRepo();
            var mydata = misdepartmentrepo.GetGLAccFList(GL_Account_No);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                decimal id = ddd.GL_Balance * -1;
                decimal name = ddd.GL_Balance * -1;

                data.Add(new { WTEPaymId = id, mName = name });
            }

            return this.Store(data);
        }

        public ActionResult DisapproveRecord(string Withdrawal_No)
        {
            try
            {
                if (Withdrawal_No == string.Empty)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Request has been selected for disapproval.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                WithdrawalRepo.DisapproveWithdrawalRecordTE(Withdrawal_No);
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Transfer Successfully Disapproved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
				//ClearControls_Approve();
				//Store store = X.GetCmp<Store>("ApproveRequestWithStore");
				//store.Reload();

				//return this.Direct();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "WithdrawalApprovePartial",
					ContainerId = "MainArea",
					RenderMode = RenderMode.AddTo,
				};
				this.GetCmp<TabPanel>("MianArea").SetLastTabAsActive();
				return pvr;
			}
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ReadEmployeeSchemeFund(string ESF_Id)
        {
            try
            {
                return this.Store(WithdrawalRepo.GetESFList(ESF_Id));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult ReadESchemeFund(string Employer_Id, string Employer_Name)
        {
            try
            {
                return this.Store(WithdrawalRepo.GetSFList(Employer_Id, Employer_Name));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ReadWithdrawalApprove_Tax()
        {
            try
            {
                return this.Store(WithdrawalRepo.GetWithdrawalApprovedListTax());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ReadWithdrawalApprove()
        {
            try
            {
                return this.Store(WithdrawalRepo.GetWithdrawalApprovedList());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ReadWithdrawalReverse()
        {
            try
            {
                return this.Store(WithdrawalRepo.GetWithdrawalReverseListTE());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ReadWithdrawalPending()
        {
            try
            {
              return this.Store( WithdrawalRepo.GetWithdrawalPendingListTE());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // filter employer scheme for employeeFilterEmpolyeeSFGrid
        public ActionResult FilterESGrid(string Employer_Id)
        {
            var misdepartmentrepo = new crm_EmployeeRepo();
            var mydata = misdepartmentrepo.GetEmployerSFList(Employer_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.Scheme_Fund_Id;
                string name = ddd.Scheme_Name + " / " + ddd.Fund_Name;

                data.Add(new { Id = id, Name = name });
            }

            return this.Store(data);
        }

        // Get unit price
        public ActionResult GetUPrice(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                WithdrawalRepo.Get_UnitPrice(WithdrawalRepo);
                this.GetCmp<TextField>("WithdrawalTEP_UnitP").SetValue(WithdrawalRepo.Unit_Price);
                this.GetCmp<TextField>("WithdrawalTE_TotalB").SetValue(WithdrawalRepo.Unit_Price * WithdrawalRepo.Total_Unit_Balance);
                return this.Direct();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }

        // Get Accrued Benefit
        public ActionResult GetUPricePay(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                //WithdrawalRepo.Get_UnitPrice(WithdrawalRepo);
                //this.GetCmp<TextField>("WithdrawalP_UnitP").SetValue(WithdrawalRepo.Unit_Price);
                this.GetCmp<TextField>("WithdrawalTE_TotalB").SetValue(WithdrawalRepo.Unit_Price * WithdrawalRepo.Total_Unit_Balance);
                this.GetCmp<TextField>("WithdrawalTEPartial_TotalWithdrawal_Display").SetValue(WithdrawalRepo.Total_Withdrawal_Temp * WithdrawalRepo.Unit_Price);

                return this.Direct();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }

        public ActionResult GetUnit_Amount(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                //if (WithdrawalRepo.Type == "AMOUNT")
                //{
                //    this.GetCmp<TextField>("WithdrawalPartial_TotalWithdrawal_Display").SetValue(WithdrawalRepo.Total_Withdrawal_Temp / WithdrawalRepo.Unit_Price);
                   
                //}
                //else
                {
                    this.GetCmp<TextField>("WithdrawalTEPartial_TotalWithdrawal_Display").SetValue(WithdrawalRepo.Total_Withdrawal_Temp * WithdrawalRepo.Unit_Price);

                }
                return this.Direct();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }

        public ActionResult GetUPriceApprove(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                WithdrawalRepo.Get_UnitPrice(WithdrawalRepo);
                this.GetCmp<TextField>("WithdrawalApprove_TotalBTE").SetValue(WithdrawalRepo.Unit_Price * (WithdrawalRepo.Employee_Unit_Balance + WithdrawalRepo.Employer_Unit_Balance));
                return this.Direct();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }


        public ActionResult ReadEmployerScheme(Remit_ReceiptRepo rr)
        {
            try
            {

                {
                    return this.Store(ReceiptRepo.GetReceiptESListBTE(rr));
                }

            }
            catch (Exception ex)
            {
                return this.Direct();
            }
        }

    }
}




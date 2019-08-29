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
    public class Remit_PortOutController : Controller
    {
        readonly crm_EmployeeSchemeFundRepo ESFRepo = new crm_EmployeeSchemeFundRepo();
        Remit_WithdrawalRepo WithdrawalRepo = new Remit_WithdrawalRepo();
        readonly crm_EmployeeRepo employee = new crm_EmployeeRepo();
       
        public ActionResult AddPortOutTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "PortOutPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }
        public ActionResult AddPortOutApproveTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "PortOutApprovePartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddPortOutReverseTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "PortOutReversePartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddPortOutPayTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "PortOutPayPartial",
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
                var x = X.GetCmp<FormPanel>("frmPortOutApproveP_Main");
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
                var x = X.GetCmp<FormPanel>("frmPortOutPayP_Main");
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
                var x = X.GetCmp<FormPanel>("frmPortOutReverseP_Main");
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
                var x = X.GetCmp<FormPanel>("frm_PortOutP");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }



        public ActionResult SaveRecord(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
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

                if (string.IsNullOrEmpty(WithdrawalRepo.New_Trustee))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "New Trustee is a required field",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                

                // check if there is a pending request for the employee scheme fund

                //if (WithdrawalRepo.GetcheckESFList(WithdrawalRepo) == true)
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Sorry! There is already a pending request for the Employee Account",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });

                //    return this.Direct();
                //}

                // validate scheme date             
                if (!string.IsNullOrEmpty(WithdrawalRepo.Scheme_Id))
                {
                    GlobalValue.Get_Scheme_Today_Date(WithdrawalRepo.Scheme_Id);
                    if (WithdrawalRepo.Trans_Request_Date > GlobalValue.Scheme_Today_Date)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! Request date can not be above the scheme working date " ,
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

              //  WithdrawalRepo.GetESFList(WithdrawalRepo.ESF_Id);

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
				
				if (ModelState.IsValid)
                {
                    this.WithdrawalRepo.SaveRecord_PortOut(WithdrawalRepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Porting Out Successfully Processed.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
					// ClearControls();
					//Store store = X.GetCmp<Store>("PortOutESFStore");
					//store.Reload();
					//return this.Direct();
					var pvr = new Ext.Net.MVC.PartialViewResult
					{
						ViewName = "PortOutPartial",
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
                throw ex;
            }
        }


        public ActionResult ApproveRecord(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                if (WithdrawalRepo.PortOut_No == string.Empty)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No record has been selected for approval.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                GlobalValue.Get_Scheme_Today_Date(WithdrawalRepo.Scheme_Id);

                ///approve pending withdrawal

                WithdrawalRepo.Approve_Unit_PortOut(WithdrawalRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Porting Out Successfully Approved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
				//ClearControls_Approve();
				//Store store = X.GetCmp<Store>("ApproveRequestPortOutStore");
				//store.Reload();

				//return this.Direct();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "PortOutApprovePartial",
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


        public ActionResult PayRecord(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                if (string.IsNullOrEmpty( WithdrawalRepo.PortOut_No))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No record has been selected for Payment.",
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


               WithdrawalRepo.GetESFPortOutList(WithdrawalRepo.PortOut_No);

                //get GL balance from GL_Account Table 
                WithdrawalRepo.Get_GL_Balance(WithdrawalRepo);   
                          
                    if (WithdrawalRepo.Total_Benefit > WithdrawalRepo.GL_Balance * -1)
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

				if (!string.IsNullOrEmpty(WithdrawalRepo.Scheme_Id))
				{
					GlobalValue.Get_Scheme_Today_Date(WithdrawalRepo.Scheme_Id);
					if (WithdrawalRepo.Pay_Date_Benefit != GlobalValue.Scheme_Today_Date)
					{
						X.Mask.Hide();
						X.Msg.Show(new MessageBoxConfig
						{
							Title = "Error",
							Message = "Sorry! Transaction date must be equal to scheme working date of " + GlobalValue.Scheme_Today_Date.Date.ToString(),
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
				///pay pending withdrawal

				WithdrawalRepo.Pay_Unit_PortOut(WithdrawalRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Port-Out Successfully Paid.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
				//ClearControls_Pay();
				//Store store = X.GetCmp<Store>("PayRequestPortOutStore");
				//store.Reload();

				//return this.Direct();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "PortOutPayPartial",
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

        // reverse port out
        public ActionResult ReverseRecord(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(WithdrawalRepo.PortOut_No))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No record has been selected for Payment.",
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
                        Message = "Problem with Bank Account.",
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
                        Message = "Reason for Reversal is required",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //  WithdrawalRepo.GetESFList(WithdrawalRepo.ESF_Id);

                //get GL balance from GL_Account Table 
                WithdrawalRepo.Get_GL_Balance(WithdrawalRepo);

                //if (WithdrawalRepo.Total_Benefit > WithdrawalRepo.GL_Balance * -1)
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


                //if (string.IsNullOrEmpty(WithdrawalRepo.Payment_Mode))
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Payment Mode is required",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.ERROR,
                //        Width = 350
                //    });
                //    return this.Direct();
                //}

                //if (!WithdrawalRepo.Pay_Date_Benefit.HasValue)
                //{
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Payment Date is required",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350
                //    });

                //    return this.Direct();
                //}

                ///pay pending withdrawal
                ///
                //if (!string.IsNullOrEmpty(WithdrawalRepo.Scheme_Id))
                //{
                    GlobalValue.Get_Scheme_Today_Date(WithdrawalRepo.Scheme_Id);
                //    if (WithdrawalRepo.Pay_Date_Benefit != GlobalValue.Scheme_Today_Date)
                //    {
                //        X.Mask.Hide();
                //        X.Msg.Show(new MessageBoxConfig
                //        {
                //            Title = "Error",
                //            Message = "Sorry! This transaction can not be reversed. Process aborted",
                //            Buttons = MessageBox.Button.OK,
                //            Icon = MessageBox.Icon.INFO,
                //            Width = 350

                //        });

                //        return this.Direct();
                //    }
                //}
                //else
                //{
                //    X.Mask.Hide();
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Scheme cannot be verified.",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350

                //    });
                //}

                WithdrawalRepo.Reverse_Unit_PortOut(WithdrawalRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Port-Out Successfully Reversed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
				//ClearControls_Reverse();
				//Store store = X.GetCmp<Store>("ReverseRequestPortOutStore");
				//store.Reload();

				//return this.Direct();
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "PortOutReversePartial",
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

                data.Add(new { PortOutPay_GLB_gId = id, gName = name });
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

                data.Add(new { dId = id, dName = name });
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

                data.Add(new { mId = id, mName = name });
            }

            return this.Store(data);
        }

        public ActionResult DisapproveRecord(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                if (WithdrawalRepo.PortOut_No == string.Empty)
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

                WithdrawalRepo.DisapprovePortOutRecord(WithdrawalRepo);
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Withdrawal Successfully Disapproved.",
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
					ViewName = "PortOutApprovePartial",
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

        public ActionResult DisapproveRecord_Pay(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                if (WithdrawalRepo.PortOut_No == string.Empty)
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

                WithdrawalRepo.DisapprovePortOutRecord_Pay(WithdrawalRepo);
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Withdrawal Successfully Disapproved.",
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
                    ViewName = "PortOutApprovePartial",
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


        public ActionResult ReadESchemeFund(string Employer_Id)
        {
            try
            {
                return this.Store(WithdrawalRepo.GetSFList(Employer_Id));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       

        public ActionResult ReadPortOutApprove()
        {
            try
            {
                return this.Store(WithdrawalRepo.GetPortOutApprovedList());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult ReadPortOutPending()
        {
            try
            {
              return this.Store( WithdrawalRepo.GetPortOutPendingList());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ReadPortOutReverse()
        {
            try
            {
                return this.Store(WithdrawalRepo.GetPortOutReverseList());
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
                this.GetCmp<TextField>("PortOutP_UnitP").SetValue(WithdrawalRepo.Unit_Price);
                this.GetCmp<TextField>("PortOut_TotalB").SetValue(WithdrawalRepo.Unit_Price * WithdrawalRepo.Total_Unit_Balance);
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
                this.GetCmp<TextField>("PortOut_TotalB").SetValue(WithdrawalRepo.Unit_Price * WithdrawalRepo.Total_Unit_Balance);
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

        // Get Accrued Benefit for reversal
        public ActionResult GetUPriceReverse(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                this.GetCmp<TextField>("PortOutReverse_TotalB").SetValue(WithdrawalRepo.Unit_Price * WithdrawalRepo.Total_Unit_Balance);
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
               // WithdrawalRepo.Get_UnitPrice(WithdrawalRepo);
                this.GetCmp<TextField>("PortOutApprove_TotalB").SetValue(WithdrawalRepo.Unit_Price * (WithdrawalRepo.Employee_Unit_Balance + WithdrawalRepo.Employer_Unit_Balance));
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

    }
}




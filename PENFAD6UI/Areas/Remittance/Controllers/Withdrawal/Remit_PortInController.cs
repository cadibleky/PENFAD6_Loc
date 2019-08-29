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
    public class Remit_PortInController : Controller
    {
        readonly crm_EmployeeSchemeFundRepo ESFRepo = new crm_EmployeeSchemeFundRepo();
        Remit_WithdrawalRepo WithdrawalRepo = new Remit_WithdrawalRepo();
        readonly crm_EmployeeRepo employee = new crm_EmployeeRepo();
       
        public ActionResult AddPortInTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "PortInPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }
        public ActionResult AddPortInApproveTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "PortInApprovePartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddPortInReverseTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "PortInReversePartial",
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
                var x = X.GetCmp<FormPanel>("frmPortInApproveP_Main");
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
                var x = X.GetCmp<FormPanel>("frm_PortInP");
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


        public ActionResult SaveRecord_Port(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                if ((WithdrawalRepo.Employee_Amount + WithdrawalRepo.Employee_Amount) <= 0)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Total port-in amount must be more than 0",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                }

                if (!WithdrawalRepo.Trans_Date.HasValue)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry!Invalid Date",
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
                    if (WithdrawalRepo.Trans_Date != GlobalValue.Scheme_Today_Date)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry!Date must be equal to scheme working date " ,
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


                if (string.IsNullOrEmpty(WithdrawalRepo.Previous_Trustee))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Previous Trustee is a required field",
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
                        Message = "Receiving Account is a required field",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                    return this.Direct();
                }


                WithdrawalRepo.GetESFList(WithdrawalRepo.ESF_Id);

               
                if (WithdrawalRepo.ESF_Id != null)
                {
                    this.WithdrawalRepo.SaveRecord_Port(WithdrawalRepo);

                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Port In Successfully Processed.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    var pvr = new Ext.Net.MVC.PartialViewResult
                    {
                        ViewName = "PortInPartial",
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

                // validate scheme date             
                //if (!string.IsNullOrEmpty(WithdrawalRepo.Scheme_Id))
                //{
                    GlobalValue.Get_Scheme_Today_Date(WithdrawalRepo.Scheme_Id);
                //    if (WithdrawalRepo.Trans_Date != GlobalValue.Scheme_Today_Date)
                //    {
                //        X.Msg.Show(new MessageBoxConfig
                //        {
                //            Title = "Error",
                //            Message = "Sorry!Date must be equal to scheme working date ",
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

                ///approve pending withdrawal

                WithdrawalRepo.Approve_Unit_PortIn(WithdrawalRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Porting In Successfully Approved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "PortInApprovePartial",
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


        public ActionResult ReverseRecord(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                if (WithdrawalRepo.Withdrawal_No == string.Empty)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Port In has been selected for reversal.",
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
                        Message = "Reversal Reason is required.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                WithdrawalRepo.GetESFList(WithdrawalRepo.ESF_Id);

                GlobalValue.Get_Scheme_Today_Date(WithdrawalRepo.Scheme_Id);

                //// validate scheme date             
                //if (!string.IsNullOrEmpty(WithdrawalRepo.Scheme_Id))
                //{
                //    GlobalValue.Get_Scheme_Today_Date(WithdrawalRepo.Scheme_Id);
                //    if (WithdrawalRepo.Trans_Date != GlobalValue.Scheme_Today_Date)
                //    {
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
                //    X.Msg.Show(new MessageBoxConfig
                //    {
                //        Title = "Error",
                //        Message = "Employee account cannot be verified.",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350

                //    });
                //}
                ///reverse port in

                WithdrawalRepo.Reverse_Unit_PortIn(WithdrawalRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Porting In Successfully Reversed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "PortInReversePartial",
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

                data.Add(new { gId = id, gName = name });
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

        public ActionResult DisapproveRecord(string PortIn_No)
        {
            try
            {
                if (PortIn_No == string.Empty)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Record has been selected for disapproval.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                WithdrawalRepo.DisapprovePortInRecord(PortIn_No);
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Porting In Successfully Disapproved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "PortInApprovePartial",
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
        public ActionResult ReadPortInPending()
        {
            try
            {
              return this.Store( WithdrawalRepo.GetPortInPendingList());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ReadPortInReverse()
        {
            try
            {
                return this.Store(WithdrawalRepo.GetPortInReverseList());
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
                this.GetCmp<TextField>("PortInP_UnitP").SetValue(WithdrawalRepo.Unit_Price);
                this.GetCmp<TextField>("PortIn_TotalB").SetValue(WithdrawalRepo.Unit_Price * WithdrawalRepo.Total_Unit_Balance);
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

        public ActionResult Employee_GetUnit_Amount(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                if (WithdrawalRepo.Unit_Price == 0)
                {
                    WithdrawalRepo.Unit_Price = 1;
                    this.GetCmp<TextField>("Employee_PortInPartial_TotalPortIn_Display").SetValue(WithdrawalRepo.Employee_Amount / WithdrawalRepo.Unit_Price);
                }
                else
                {
                    this.GetCmp<TextField>("Employee_PortInPartial_TotalPortIn_Display").SetValue(WithdrawalRepo.Employee_Amount / WithdrawalRepo.Unit_Price);
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

        public ActionResult Employer_GetUnit_Amount(Remit_WithdrawalRepo WithdrawalRepo)
        {
            try
            {
                if (WithdrawalRepo.Unit_Price == 0)
                {
                    WithdrawalRepo.Unit_Price = 1;
                    this.GetCmp<TextField>("Employer_PortInPartial_TotalPortIn_Display").SetValue(WithdrawalRepo.Employer_Amount / WithdrawalRepo.Unit_Price);
                }
                else
                {
                    this.GetCmp<TextField>("Employer_PortInPartial_TotalPortIn_Display").SetValue(WithdrawalRepo.Employer_Amount / WithdrawalRepo.Unit_Price);
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

		public ActionResult All_GetUnit_Amount(Remit_WithdrawalRepo WithdrawalRepo)
		{
			try
			{
				if (WithdrawalRepo.Unit_Price == 0)
				{
					WithdrawalRepo.Unit_Price = 1;
					this.GetCmp<TextField>("Employee_PortInPartial_TotalPortIn_Display").SetValue(WithdrawalRepo.Employee_Amount / WithdrawalRepo.Unit_Price);
					this.GetCmp<TextField>("Employer_PortInPartial_TotalPortIn_Display").SetValue(WithdrawalRepo.Employer_Amount / WithdrawalRepo.Unit_Price);
				}
				else 
				{
					this.GetCmp<TextField>("Employee_PortInPartial_TotalPortIn_Display").SetValue(WithdrawalRepo.Employee_Amount / WithdrawalRepo.Unit_Price);
					this.GetCmp<TextField>("Employer_PortInPartial_TotalPortIn_Display").SetValue(WithdrawalRepo.Employer_Amount / WithdrawalRepo.Unit_Price);
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
                this.GetCmp<TextField>("WithdrawalApprove_TotalB").SetValue(WithdrawalRepo.Unit_Price * (WithdrawalRepo.Employee_Unit_Balance + WithdrawalRepo.Employer_Unit_Balance));
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




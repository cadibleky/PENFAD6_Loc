
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using System.Text;
using PENFAD6DAL.Repository.GL;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.Investment.Bond;
using PENFAD6DAL.Repository.Investment.Equity_CIS;
using Dapper;
using PENFAD6DAL.DbContext;
using System.Data;

namespace PENFAD6UI.Areas.GL.Controllers
{
    public class GLFeeController : Controller
    {
        readonly GLFeeRepo GLFee = new GLFeeRepo();
        readonly GLAccountRepo GLARepo = new GLAccountRepo();
        readonly pfm_Scheme_FundRepo SFRepo = new pfm_Scheme_FundRepo();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddGLFeeTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "GLFeeCreatePartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddGLFeeApproveTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "GLFeeApprovePartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddGLFeeReversalTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "GLFeeReversalPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmGLFeeCreate");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void ClearControls_Approve()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmGLFeeApprove");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void ClearControls_Reversal()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmGLFeeReversal");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

    
        public ActionResult ReadCreditGL(string scheme_fund_id)
        {
            try
            {
                return this.Store(GLFee.CreditList(scheme_fund_id));
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        public ActionResult ReadDebitGL(string scheme_fund_id)
        {
            try
            {
                return this.Store(GLFee.debitList(scheme_fund_id));
              
            }

            catch (System.Exception)
            {
                throw;
            }
        }

        public ActionResult ReadGLApprove(GLFeeRepo GLFee)
        {
            try
            {
  
                {          
                    return this.Store(GLFee.GLApproveList2());
                }
            }
            catch (System.Exception EX)
            {
                throw EX;
            }
        }

        public ActionResult ReadGLApprove1(GLFeeRepo GLFee)
        {
            try
            {
                if (GLFee.Fee_Id == "F0002")
                {
                    GLFee.GLApproveList(GLFee);
                    this.GetCmp<TextField>("GLFeeApp_FMID").SetValue(GLFee.Fund_Manager);
                }
                else
                {
                    GLFee.Fund_Manager_Id = "NA";
                }
                return this.Direct();
            }
            catch (System.Exception EX)
            {
                throw EX;
            }
        }

        public ActionResult ReadGLApprove2(GLFeeRepo GLFee)
        {
            try
            {
                if (GLFee.Fee_Id == "F0002")
                {
                    GLFee.GLApproveList(GLFee);
                    this.GetCmp<TextField>("GLFeeRev_FMID").SetValue(GLFee.Fund_Manager);
                }
                else
                {
                    GLFee.Fund_Manager_Id = "NA";
                }
                return this.Direct();
            }
            catch (System.Exception EX)
            {
                throw EX;
            }
        }

        public ActionResult ReadGLReversal(GLFeeRepo GLFee, DateTime? From_Date, DateTime? To_Date)
        {
            try
            {
                //if (GLFee.Fee_Id == "F0002")
                //{
                //    return this.Store(GLFee.GLReversalList(From_Date, To_Date));
                //}
                //else
                {
                    return this.Store(GLFee.GLReversalList(From_Date, To_Date));
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult SaveRecord(GLFeeRepo GLFee)
        {
            try
            {
                if (string.IsNullOrEmpty(GLFee.Scheme_Fund_Id))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select Scheme Account.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350

                    });
                    return this.Direct();
                }


                if (string.IsNullOrEmpty(GLFee.Fee_Id))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select Fee Type.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();
                }

                if (GLFee.Apply_Bal < GLFee.Paid_Amount || GLFee.Paid_Amount <= 0)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please Amount can not be more than 'Amount Payable' or less than 0.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();
                }

                if (string.IsNullOrEmpty(GLFee.Trans_Type))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please 'Transaction Type' is required",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();
                }

                if (!GLFee.Trans_Date.HasValue)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please 'Transaction Date' is required",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();
                }

                if (string.IsNullOrEmpty(GLFee.Narration))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please 'Narration' is required",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();
                }

                if (GLFee.Paid_Amount <= 0)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please enter a valid Amount",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    return this.Direct();
                }

                GLFee.Get_Balance(GLFee);
                //GET GL BALANCES
                if ((GLFee.GL_Balance * -1) < GLFee.Paid_Amount)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry Bank Account Balance not sufficient for transaction",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    return this.Direct();
                }

                if (!string.IsNullOrEmpty(GLFee.Scheme_Fund_Id.Substring(0, 2)))
                {
                    GlobalValue.Get_Scheme_Today_Date((GLFee.Scheme_Fund_Id.Substring(0, 2)));
                    if (GLFee.Trans_Date != GlobalValue.Scheme_Today_Date)
                    {
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
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Scheme cannot be verified.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                }

                //if (ModelState.IsValid)
                //{

                GLFee.SaveRecord(GLFee);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Fee Successfully Submitted.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "GLFeeCreatePartial",
                    ContainerId = "MainArea",
                    RenderMode = RenderMode.AddTo,
                };
                this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
                return pvr;
            }
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
       // }
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

        public ActionResult ApproveRecord(GLFeeRepo GLFee)
        {
            try
            {
                if (GLFee.TID < 1)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Transaction has been selected for approval.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                //GET GL BALANCES
                GLFee.Get_Balance(GLFee);

                if ((GLFee.GL_Balance * -1) < GLFee.Paid_Amount)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! Account Balance not sufficient for transaction",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();
                }

                GlobalValue.Get_Scheme_Today_Date((GLFee.Scheme_Fund_Id.Substring(0, 2)));

                ///approve pending fee payment 
                GLFee.Approve_GLFee(GLFee);
             
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Fee Payment Successfully Approved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "GLFeeApprovePartial",
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

        public ActionResult DisapproveRecord(GLFeeRepo GLFee)
        {
            try
            {
                if (GLFee.TID < 1)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Transaction has been selected for approval.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                ///disapprove pending fee payment 
                GLFee.Disapprove_GLFee(GLFee);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Fee Payment Disapproved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "GLFeeApprovePartial",
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

        public ActionResult ReverseRecord(GLFeeRepo GLFee)
        {
            try
            {
               
                if (GLFee.TID < 1)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No Transaction has been selected for reversal.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                GlobalValue.Get_Scheme_Today_Date((GLFee.Scheme_Fund_Id.Substring(0, 2)));

                //if (!string.IsNullOrEmpty(GLFee.Scheme_Fund_Id.Substring(0, 2)))
                //{
                //    GlobalValue.Get_Scheme_Today_Date((GLFee.Scheme_Fund_Id.Substring(0, 2)));
                //    if (GLFee.Trans_Date != GlobalValue.Scheme_Today_Date)
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
                //        Message = "Scheme cannot be verified.",
                //        Buttons = MessageBox.Button.OK,
                //        Icon = MessageBox.Icon.INFO,
                //        Width = 350

                //    });
                //}

                ///reverse fee payment
                GLFee.Reverse_Fee(GLFee);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Reversal",
                    Message = "Fee Payment Successfully Reversed.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "GLFeeReversalPartial",
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

        public ActionResult ReadSF()
        {
            try
            {
                return this.Store(SFRepo.GetSchemeFundList());
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        // filter GL Account for scheme
        public ActionResult GetGLFeeID(GLFeeRepo GLFee)
        {
            var misdepartmentrepo = new GLFeeRepo();
            var mydata = misdepartmentrepo.GetFeesList2(GLFee);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.Fund_Manager_Id;
                string name = ddd.Fund_Manager;

                data.Add(new { GLFeeId = id, GLFeeName = name });
            }
            return this.Store(data);
        }

        //get values for fee payment
        public ActionResult getValues(GLFeeRepo GLFee)
        {
            GLFee.GetFeesList(GLFee);
            this.GetCmp<TextField>("GLFee_Apply_Bal").SetValue(GLFee.Apply_Bal);
            this.GetCmp<DateField>("GLFee_Last_Apply_Date").SetValue(GLFee.Last_Apply_Date);
            this.GetCmp<DateField>("GLFee_Last_Paid_Date").SetValue(GLFee.Last_Paid_Date);
            return this.Direct();
        }


        // filter GL Account for scheme
        public ActionResult GetGLFeeAB(string Scheme_Fund_Id)
        {
            var misdepartmentrepo = new BondRepo();
            var mydata = misdepartmentrepo.GetGLASFList(Scheme_Fund_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.GL_Account_No;
                string name = ddd.GL_Account_Name;

                data.Add(new { GLFeeBankId = id, GLFeeBankName = name });
            }

            return this.Store(data);
        }

        // filter GL Account for scheme
        public ActionResult GetGLFeeaccPay(string GL_Account_No)
        {
            var misdepartmentrepo = new Invest_Equity_CISRepo();
            var mydata = misdepartmentrepo.GetGLAccFList(GL_Account_No);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                decimal id = ddd.GL_Balance * -1;
                decimal name = ddd.GL_Balance * -1;

                data.Add(new { GLFeemId = id, GLFeemName = name });
            }

            return this.Store(data);
        }

        public ActionResult GetGLFeeApply(GLFeeRepo GLFee)
        {
            try
            {
                if (string.IsNullOrEmpty (GLFee.Scheme_Fund_Id))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select scheme account.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                    return this.Direct();
                }

                if (string.IsNullOrEmpty(GLFee.Fee_Id))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select fee.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });

                    return this.Direct();
                }

                // GlobalValue.Get_Scheme_Today_Date(GLFee.Scheme_Id);

                //APPLY FEES EOD_ACCRUE_SF_FEES

                if (string.IsNullOrEmpty(GLFee.Fund_Manager_Id))
                {
                    GLFee.Fund_Manager_Id = "NA";
                }

                var con = new AppSettings();      
                var paramACCAPP = new DynamicParameters();
                paramACCAPP.Add("P_SCHEME_ID", GLFee.Scheme_Fund_Id, DbType.String, ParameterDirection.Input);
                paramACCAPP.Add("P_FEE_ID", GLFee.Fee_Id, DbType.String, ParameterDirection.Input);
                paramACCAPP.Add("P_FM_ID", GLFee.Fund_Manager_Id, DbType.String, ParameterDirection.Input);
                paramACCAPP.Add("P_VALUE_DATE", GlobalValue.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                con.GetConnection().Execute("APPLY_FEES_FORCE", paramACCAPP, commandType: CommandType.StoredProcedure);

                GLFee.GetFeesList2(GLFee);
                this.GetCmp<TextField>("GLFeeCreate_FeeID").SetValue(""); 
                this.GetCmp<TextField>("GLFee_Apply_Bal").SetValue("");
                this.GetCmp<DateField>("GLFee_Last_Apply_Date").SetValue("");
                this.GetCmp<DateField>("GLFee_Last_Paid_Date").SetValue("");
                
                //PREVIEW


                return this.Direct();
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }
    }//end class


}
using Ext.Net;
using Ext.Net.MVC;
using System.Web.Mvc;
using System;

using PENFAD6DAL.Repository.Setup.SystemSetup;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.Setup.Utilities;
using System.Linq;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using PENFAD6DAL.DbContext;
using System.Transactions;
using Oracle.ManagedDataAccess.Client;
using Dapper;
using System.Data;

namespace PENFAD6UI.Areas.Setup.Controllers.Utilities
{
    public class EndOfYearController : Controller
    {
        readonly EndOfDayRepo EODRepo = new EndOfDayRepo();
        cLogger logger = new cLogger();
        public ActionResult Index()
        {
            return View();
        }
        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmEOY");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public ActionResult AddEOYTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "EOYPartial",
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo,
                };
                X.Mask.Hide();
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

                return pvr;
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
                throw ex;
            }
        }

        public ActionResult AddEOYAllTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "EOYAllPartial",
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo,
                };
                X.Mask.Hide();
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

                return pvr;
            }
            catch (Exception ex)
            {
                X.Mask.Hide();
                throw ex;
            }
        }

        public ActionResult GetSchemeDate(EndOfDayRepo EODRepo)
        {
            try
            {
                if (string.IsNullOrEmpty(EODRepo.Scheme_Id))
                { }

                else
                {
                    GlobalValue.Get_Scheme_Today_Date(EODRepo.Scheme_Id);
                    this.GetCmp<DateField>("EOY_Scheme_Today_Date").SetValue(GlobalValue.Scheme_Today_Date);
                    EODRepo.GetSchemeList(EODRepo);
                    this.GetCmp<TextField>("txtschemeEOY").SetValue(EODRepo.Scheme_Name);
                }
                return this.Direct();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult RunEOY(EndOfDayRepo EODRepo)
        {



            // if (ModelState.IsValid)

            {

                if (string.IsNullOrEmpty(EODRepo.Scheme_Id))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! Scheme must be selected",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    return this.Direct();
                }


                EODRepo.GetSystemDate(EODRepo);

                if (EODRepo.Scheme_Today_Date > EODRepo.System_Date)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! End of Year can not be ran for.  " + EODRepo.Scheme_Name + ". System Date is invalid",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    return this.Direct();
                }

                if (!string.IsNullOrEmpty(EODRepo.Scheme_Id))
                {
                    GlobalValue.Get_Scheme_Today_Date(EODRepo.Scheme_Id);
                    EODRepo.Scheme_Today_Date = GlobalValue.Scheme_Today_Date;
                    if (EODRepo.Scheme_Today_Date != EODRepo.Confirm_Date)
                    {
                        X.Mask.Hide();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Sorry! Wrong Date " + EODRepo.Scheme_Name,
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


                if (EODRepo.Scheme_Today_Date != EODRepo.Confirm_Date)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! Wrong Date " + EODRepo.Scheme_Name,
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    return this.Direct();
                }

                //if (EODRepo.RunEOD(EODRepo) == true)
                var app = new AppSettings();

                TransactionOptions tsOp = new TransactionOptions();
                tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
                TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);
                tsOp.Timeout = TimeSpan.FromMinutes(120000);

                using (OracleConnection conn = new OracleConnection(app.conString()))  //
                {
                    try
                    {
                        conn.Open();


                        //Check if it's end of year.

                        var paramRed = new DynamicParameters();
                        // paramRed.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        paramRed.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        paramRed.Add("VDATA", "", DbType.Int32, ParameterDirection.Output);
                        conn.Execute("EOD_IS_EOY", paramRed, commandType: CommandType.StoredProcedure);
                        int redem = paramRed.Get<int>("VDATA");
                        if (redem <= 0)
                        {
                            X.Mask.Hide();
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Sorry! End of Year is not valid for this Date." + Environment.NewLine + "Proces aborted",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Width = 350
                            });
                            return this.Direct();
                        }


                        //Payment
                        var paramRe = new DynamicParameters();
                        paramRe.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        paramRe.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        paramRe.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                        conn.Execute("EOD_PENDING_PAYMENT", paramRe, commandType: CommandType.StoredProcedure);
                        int receipt = paramRe.Get<int>("VDATA");
                        if (receipt > 0)
                        {
                            X.Mask.Hide();
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Pending Payment exist for this Scheme." + Environment.NewLine + "Process aborted",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Width = 350

                            });
                            return this.Direct();
                        }

                        //Purchases
                        var paramPu = new DynamicParameters();
                        paramPu.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        paramPu.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        paramPu.Add("VDATA", "", DbType.Int32, ParameterDirection.Output);
                        conn.Execute("EOD_PENDING_PURCHASE", paramPu, commandType: CommandType.StoredProcedure);
                        int purchase = paramPu.Get<int>("VDATA");
                        if (purchase > 0)
                        {
                            X.Mask.Hide();
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Pending Unit Purchase exist for this Scheme." + Environment.NewLine + "Process aborted",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Width = 350
                            });
                            return this.Direct();
                        }

                        ////Redemption
                        //var paramRed = new DynamicParameters();
                        //paramRed.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        //paramRed.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        //paramRed.Add("VDATA", "", DbType.Int32, ParameterDirection.Output);
                        //conn.Execute("EOD_PENDING_REDEMPTION", paramRed, commandType: CommandType.StoredProcedure);
                        //int redem = paramRed.Get<int>("VDATA");
                        //if (redem > 0)
                        //{
                        //    X.Mask.Hide();
                        //    X.Msg.Show(new MessageBoxConfig
                        //    {
                        //        Title = "Error",
                        //        Message = "Pending Redemption exists for this Scheme." + Environment.NewLine + "Proces aborted",
                        //        Buttons = MessageBox.Button.OK,
                        //        Icon = MessageBox.Icon.ERROR,
                        //        Width = 350
                        //    });
                        //    return this.Direct();
                        //}
                        // GL(Journal)

                        var paramJo = new DynamicParameters();
                        paramJo.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        paramJo.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        paramJo.Add("VDATA", "", DbType.Int32, ParameterDirection.Output);
                        conn.Execute("EOD_PENDING_JOURNAL", paramJo, commandType: CommandType.StoredProcedure);
                        int journal = paramJo.Get<int>("VDATA");
                        if (journal > 0)
                        {
                            X.Mask.Hide();
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Pending Journal exist for this Scheme." + Environment.NewLine + "Process aborted",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Width = 350
                            });
                            return this.Direct();
                        }

                        //check for pending investments - TBILL / MM
                        var paramIn = new DynamicParameters();
                        paramIn.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        paramIn.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        paramIn.Add("VDATA", "", DbType.Int32, ParameterDirection.Output);
                        conn.Execute("EOD_PENDING_MMTB", paramIn, commandType: CommandType.StoredProcedure);
                        int MM = paramIn.Get<int>("VDATA");
                        if (MM > 0)
                        {
                            X.Mask.Hide();
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Pending Money Market / T-Bill / Bond exist for this Scheme." + Environment.NewLine + "Process aborted",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Width = 350
                            });
                            return this.Direct();
                        }

                        //check for pending investments - EQUITY / CIS
                        var paramInE = new DynamicParameters();
                        paramInE.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        paramInE.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        paramInE.Add("VDATA", "", DbType.Int32, ParameterDirection.Output);
                        conn.Execute("EOD_PENDING_EQCIS", paramInE, commandType: CommandType.StoredProcedure);
                        int MME = paramInE.Get<int>("VDATA");
                        if (MME > 0)
                        {
                            X.Mask.Hide();
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Pending Equity / CIS exist for this Scheme." + Environment.NewLine + "Process aborted",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Width = 350
                            });
                            return this.Direct();
                        }

                        // check uquity prices AND CIS prices
                        var paramInEp = new DynamicParameters();
                        paramInEp.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        paramInEp.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        paramInEp.Add("VDATA", "", DbType.Int32, ParameterDirection.Output);
                        conn.Execute("EOD_PENDING_EQPRICES", paramInEp, commandType: CommandType.StoredProcedure);
                        int MMEpp = paramInEp.Get<int>("VDATA");
                        if (MMEpp > 0)
                        {
                            X.Mask.Hide();
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Sorry, Equity/CIS prices are required for " + EODRepo.Scheme_Today_Date + Environment.NewLine + "Process aborted",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Width = 350
                            });
                            return this.Direct();
                        }


                        // check pending PortIns
                        var paramInEpPortin = new DynamicParameters();
                        paramInEpPortin.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        paramInEpPortin.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        paramInEpPortin.Add("VDATA", "", DbType.Int32, ParameterDirection.Output);
                        conn.Execute("EOD_PENDING_PORTINGIN", paramInEpPortin, commandType: CommandType.StoredProcedure);
                        int MMEppPortin = paramInEpPortin.Get<int>("VDATA");
                        if (MMEppPortin > 0)
                        {
                            X.Mask.Hide();
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Sorry, Pending Port-In exist for this scheme. Process aborted",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Width = 350
                            });
                            return this.Direct();
                        }

                        // check pending benefit tranfer to employer
                        var paramInEpPortinEmp = new DynamicParameters();
                        paramInEpPortinEmp.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        paramInEpPortinEmp.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        paramInEpPortinEmp.Add("VDATA", "", DbType.Int32, ParameterDirection.Output);
                        conn.Execute("EOD_PENDING_BTOE", paramInEpPortinEmp, commandType: CommandType.StoredProcedure);
                        int MMEppPortinEmp = paramInEpPortinEmp.Get<int>("VDATA");
                        if (MMEppPortinEmp > 0)
                        {
                            X.Mask.Hide();
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Sorry, Pending Benfit Transfer to Emplpyer exist for this scheme. Process aborted",
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Width = 350
                            });
                            return this.Direct();
                        }


                        //Accrue investment interest -- HIT RECEIVABLE AND INVESTMENT INC0ME WITH ACCRUALS
                        var paramMMA = new DynamicParameters();
                        paramMMA.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        paramMMA.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        paramMMA.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
                        conn.Execute("EOD_ACCRUE_MMTBIL", paramMMA, commandType: CommandType.StoredProcedure);

                        //check for bonds interest payment
                        var paramMMAMb = new DynamicParameters();
                        paramMMAMb.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        paramMMAMb.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        paramMMAMb.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
                        conn.Execute("EOD_BOND_INT_PAY", paramMMAMb, commandType: CommandType.StoredProcedure);


                        //check for securities maturity and ?rollovers
                        var paramMMAM = new DynamicParameters();
                        paramMMAM.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        paramMMAM.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        paramMMAM.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
                        conn.Execute("EOD_ACCRUE_MMTBILMATURITY", paramMMAM, commandType: CommandType.StoredProcedure);



                        //VALUE EQUITY /CIS
                        //update gl_account_table
                        var paramVEQUI = new DynamicParameters();
                        paramVEQUI.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        paramVEQUI.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        paramVEQUI.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
                        conn.Execute("EOD_VALUE_EQUITY", paramVEQUI, commandType: CommandType.StoredProcedure);

                        //update portfolio report tables
                        var paramVEQUIr = new DynamicParameters();
                        paramVEQUIr.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        paramVEQUIr.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        paramVEQUIr.Add("P_MAKER_ID", GlobalValue.User_ID, DbType.String, ParameterDirection.Input);
                        conn.Execute("EOD_PORT_REPORTS", paramVEQUIr, commandType: CommandType.StoredProcedure);


                        //ACCRUE AND APPLY FEES EOD_ACCRUE_SF_FEES
                        var paramACCAPP = new DynamicParameters();
                        paramACCAPP.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        paramACCAPP.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        conn.Execute("EOD_ACCRUE_SF_FEES", paramACCAPP, commandType: CommandType.StoredProcedure);


                        //ACCRUE AND APPLY FUND MANAGER FEES
                        var paramFMACCAPP = new DynamicParameters();
                        paramFMACCAPP.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        paramFMACCAPP.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        conn.Execute("EOD_ACCRUE_FM_FEES", paramFMACCAPP, commandType: CommandType.StoredProcedure);

                        //update total unit
                        var paramUPPoo = new DynamicParameters();
                        paramUPPoo.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        //paramUPPoo.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        conn.Execute("EOD_UPD_SF", paramUPPoo, commandType: CommandType.StoredProcedure);


                        //calculate unit price
                        var paramUPP = new DynamicParameters();
                        paramUPP.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        paramUPP.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        conn.Execute("EOD_CAL_U_PRICE", paramUPP, commandType: CommandType.StoredProcedure);

                        //log unit price / NAV / AUM PER SCHEME FUND for next day
                        var paramMMAUMUP = new DynamicParameters();
                        paramMMAUMUP.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        paramMMAUMUP.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        conn.Execute("EOD_LOG_UNITP", paramMMAUMUP, commandType: CommandType.StoredProcedure);


                        // Apply surchage on contributions due for surchage
                        DynamicParameters param = new DynamicParameters();
                        param.Add(name: "P_SCHEME_ID", value: EODRepo.Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                        param.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        conn.Execute("EOD_SURCHARGE_CON_LOG", param, commandType: CommandType.StoredProcedure);


                        // move today date to next date
                        DynamicParameters parama = new DynamicParameters();
                        parama.Add(name: "P_SCHEME_ID", value: EODRepo.Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                        parama.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        conn.Execute("EOD_NEXT_DATE", parama, commandType: CommandType.StoredProcedure);

                        // flag retired
                        var paramTBU = new DynamicParameters();
                        paramTBU.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        paramTBU.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        conn.Execute("Z_UNCLAIMED", paramTBU, commandType: CommandType.StoredProcedure);

                        // RENEW EMAIL SENDING -STATEMENT
                        var paramEMAIL = new DynamicParameters();
                        paramEMAIL.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        paramEMAIL.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        conn.Execute("EOD_EMAIL_SEND_RENEW", paramEMAIL, commandType: CommandType.StoredProcedure);

                        // RENEW EMAIL SENDING - LOGIN
                        var paramEMAILLOGIN = new DynamicParameters();
                        paramEMAILLOGIN.Add("P_SCHEME_ID", EODRepo.Scheme_Id, DbType.String, ParameterDirection.Input);
                        paramEMAILLOGIN.Add("P_VALUE_DATE", EODRepo.Scheme_Today_Date, DbType.Date, ParameterDirection.Input);
                        conn.Execute("EOD_EMAIL_SEND_RENEW_LOGIN", paramEMAILLOGIN, commandType: CommandType.StoredProcedure);



                        ts.Complete();
                        {
                            X.Mask.Hide();
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Success",
                                Message = "END OF YEAR SUCCESSFULLY EXECUTED FOR  " + EODRepo.Scheme_Name,
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.INFO,
                                Width = 350
                            });
                            var x = X.GetCmp<FormPanel>("frmEOD");
                            x.Reset();
                            return this.Direct();
                        }
                    }

                    catch (Exception ex)
                    {
                        X.Mask.Hide();
                        throw ex;
                    }
                    finally
                    {
                        X.Mask.Hide();
                        ts.Dispose();
                        if (conn.State == ConnectionState.Open)
                        {
                            X.Mask.Hide();
                            conn.Close();
                        }
                    }

                }
            }

        }
    }
}









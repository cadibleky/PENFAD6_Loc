
using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employee;
using PENFAD6DAL.Repository.Remittance.Contribution;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Remittance.Controllers.Unit_Transfer
{
    public class Remit_Unit_TransferController: Controller
    {
        Remit_Unit_TransferRepo Unit_TransferRepo = new Remit_Unit_TransferRepo();
        crm_EmployeeRepo EmployeeRepo = new crm_EmployeeRepo();
        pfm_Scheme_FundRepo SFRepo = new pfm_Scheme_FundRepo();
        public ActionResult AddUnit_TransferTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "Unit_TransferPartial",
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }
        public ActionResult AddUnit_TransferApproveTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "Unit_TransferApprovePartial",
                // Model = PurchaseRepo.GetPurchasePendingList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }

        public ActionResult AddUnit_MergeTransferTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "Unit_MergePartial",
                // Model = PurchaseRepo.GetPurchasePendingList(),
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
                var x = X.GetCmp<FormPanel>("frmUnit_TransferApprovePartial");
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
                var x = X.GetCmp<FormPanel>("frmUnit_TransferPartial");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        public ActionResult SaveRecord(Remit_Unit_TransferRepo Unit_TransferRepo)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    this.Unit_TransferRepo.SaveRecord(Unit_TransferRepo);
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Units Transfer successfully sent for approval.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    var pvr = new Ext.Net.MVC.PartialViewResult
                    {
                        ViewName = "Unit_TransferPartial",
                        ContainerId = "MainArea",
                        RenderMode = RenderMode.AddTo,
                    };
                    this.GetCmp<TabPanel>("MainArea").SetLastTabAsActive();
                    return pvr;
                }
                else
                {
                    X.Mask.Hide();
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
                X.Mask.Hide();
                return this.Direct();
            }
        
        }

        public ActionResult SaveRecordMerge(Remit_Unit_TransferRepo Unit_TransferRepo)
        {
            try
            {


                if   (string.IsNullOrEmpty (Unit_TransferRepo.Scheme_Fund_Id))
                {
                    X.Mask.Hide();
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

                if (string.IsNullOrEmpty(Unit_TransferRepo.From_ESF_Id))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select source employee account",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                if (string.IsNullOrEmpty(Unit_TransferRepo.To_ESF_Id))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select destination employee account",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                if (string.IsNullOrEmpty(Unit_TransferRepo.Reason_Transfer))
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please enter reason.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }


                this.Unit_TransferRepo.Approve_Unit_Merge(Unit_TransferRepo);
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Employee Accounts successfully merged",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "Unit_MergePartial",
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


        public ActionResult ApproveRecord(Remit_Unit_TransferRepo Unit_TransferRepo)
        {
            try
            {
                if (Unit_TransferRepo.TID < 1)
                {
                    X.Mask.Hide();
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

                // get the pending unit transfer record
                Unit_TransferRepo.GetPendingUTList(Unit_TransferRepo);

                //approve pending unit transfer
                Unit_TransferRepo.Approve_Unit_Transfer(Unit_TransferRepo);
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Unit Transfer Successfully Approved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "Unit_TransferApprovePartial",
                    // Model = PurchaseRepo.GetPurchasePendingList(),
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

        public ActionResult DisapproveRecord(string TID)
        {
            try
            {
                if (TID == string.Empty)
                {
                    X.Mask.Hide();
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Sorry! No record has been selected for disapproval.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }

                Unit_TransferRepo.DisapproveUTRecord(TID);
                X.Mask.Hide();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Unit Transfer Successfully Disapproved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "Unit_TransferApprovePartial",
                    // Model = PurchaseRepo.GetPurchasePendingList(),
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

      
        public ActionResult ReadPurchases(Remit_Unit_TransferRepo Unit_TransferRepo)
        {
            try
            {
                return this.Store(Unit_TransferRepo.GetPurchasesList(Unit_TransferRepo));
            }
            catch (System.Exception)
            {

                throw;
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

        public ActionResult ReadPendingUT(Remit_Unit_TransferRepo Unit_TransferRepo)
        {
            try
            {
                return this.Store(Unit_TransferRepo.GetPendingUTList(Unit_TransferRepo));
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        
        public ActionResult FilterEmpolyeeSFGrid(string Scheme_Fund_Id)
        {
            try
            {
                return this.Store(Unit_TransferRepo.GetESFList(Scheme_Fund_Id));
            }
            catch (System.Exception)
            {

                throw;
            }
        }

    }
}





using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.Repository.CRM.Employee;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using PENFAD6DAL.Repository.Setup.RemitSetup;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;

namespace PENFAD6UI.Areas.Setup.Controllers.RemitSetup
{
    public class Bank_Scheme_FundsController : Controller
    {
        readonly remit_BankSchemeFundRepo BankSFRepo = new remit_BankSchemeFundRepo();
        readonly pfm_Scheme_FundRepo SFRepo = new pfm_Scheme_FundRepo();
        readonly crm_EmployeeRepo EmployeeRepo = new crm_EmployeeRepo();
        public ActionResult Index()
        {
            return View();
        }
        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmBankSF");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public ActionResult AddBankSFTab(string containerId = "MainArea")
        {
            try
            {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "BankSchemeFundPartial",
                    //Model = BankSFRepo.GetESFList(),
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


        public ActionResult SaveRecord(remit_BankSchemeFundRepo BankSFRepo)
        {
            try
            {
                string BankRef = BankSFRepo.GL_Account_No + BankSFRepo.Scheme_Fund_Id;
            //if (BankSFRepo.isESUnique(BankRef) == true)
            //{
            //    X.Msg.Show(new MessageBoxConfig
            //    {
            //        Title = "Duplicate",
            //        Message = "Bank Account already exist for this Scheme-Fund.",
            //        Buttons = MessageBox.Button.OK,
            //        Icon = MessageBox.Icon.INFO,
            //        Width = 350
            //    });
            //    return this.Direct();
            //}
     

            if (ModelState.IsValid)
            {         
               this.BankSFRepo.SaveRecord(BankSFRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Bank Account Successfully Allocated to Scheme-Fund.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350

                });
                ClearControls();
                Store store = X.GetCmp<Store>("BankSFStore");
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

        }

        // filter GL account for scheme-fund
        public ActionResult GetGLAB(string Scheme_Fund_Id)
        {
            var misdepartmentrepo = new remit_BankSchemeFundRepo();
            var mydata = misdepartmentrepo.GetGLASFList(Scheme_Fund_Id);

            List<object> data = new List<object>();
            foreach (var ddd in mydata)
            {
                string id = ddd.GL_Account_No;
                string name = ddd.GL_Account_Name;

                data.Add(new { Id = id, Name = name });
            }

            return this.Store(data);

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

        public ActionResult DeleteRecord(string Bank_RefNo)
        {
            if (Bank_RefNo == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! No 'Bank-SchemeFund' has been selected.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

            BankSFRepo.DeleteRecord(Bank_RefNo);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Bank-SchemeFund Successfully Deleted.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350


            });
            ClearControls();
            Store store = X.GetCmp<Store>("BankSFStore");
            store.Reload();

            return this.Direct();
        }
        public ActionResult Read()
        {
            try
            {
                return this.Store(BankSFRepo.GetBankSFListGrid());
            }
            catch (System.Exception)
            {

                throw;
            }
        }


    }
}


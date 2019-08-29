
using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.Repository.Setup.InvestSetup;
using PENFAD6DAL.Repository.Setup.pfmSetup;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Setup.Controllers.InvestSetup

{
    public class Invest_ProductController : Controller
    {
        readonly Invest_ProductRepo ProductRepo = new Invest_ProductRepo();       
        public ActionResult Index()
        {
            return View();
        }
        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("frmProduct");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public ActionResult AddProductTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "ProductPartial",
                Model = ProductRepo.GetProductList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }
        public ActionResult SaveRecord(Invest_ProductRepo ProductRepo)
        { 
         try
        
        {
            if (ModelState.IsValid)
            {
                    if (Microsoft.VisualBasic.Information.IsNumeric(ProductRepo.Equity_CIS_Unit_Price))
                    {
                        ProductRepo.Equity_CIS_Unit_Price = Convert.ToDecimal(ProductRepo.Equity_CIS_Unit_Price);
                        if (ProductRepo.Equity_CIS_Unit_Price < 0 && ProductRepo.Class_Id == "04")
                        {
                            X.Msg.Show(new MessageBoxConfig
                            {
                                Title = "Error",
                                Message = "Invalid   Unit Price for " + ProductRepo.Product_Name ,
                                Buttons = MessageBox.Button.OK,
                                Icon = MessageBox.Icon.ERROR,
                                Width = 350

                            });

                            var xxx = X.GetCmp<ComboBox>("txtClass_Id");
                            xxx.Enable();
                            return this.Direct();
                        }
                    }
                    else
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Invalid   Unit Price for " + ProductRepo.Product_Name,
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350

                        });
                        return this.Direct();

                    }
                    if (ProductRepo.Bond_Class == "NA" && ProductRepo.Class_Id == "03")
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Bond Type can not be 'NA'",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350

                        });

                       
                        return this.Direct();
                    }
                    if (ProductRepo.Fix_Floating == "NA" && ProductRepo.Class_Id == "03")
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Error",
                            Message = "Fix/Floating can not be 'NA'",
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Width = 350

                        });


                        return this.Direct();
                    }
                    this.ProductRepo.SaveRecord(ProductRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Product Successfully Saved.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                ClearControls();
                Store store = X.GetCmp<Store>("productStore");
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
            catch (Exception ex)
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
            finally
            {

            }
        }
        
    public ActionResult DeleteRecord(string Product_Id)
        {
            if (Product_Id == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! No Product has been selected.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

            ProductRepo.DeleteRecord(Product_Id);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Product Successfully deleted.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350


            });
            ClearControls();
            Store store = X.GetCmp<Store>("productStore");
            store.Reload();

            return this.Direct();
        }
        public ActionResult Read()
        {
            return this.Store(ProductRepo.GetProductList());
        }

       
        public ActionResult gclasscode(Invest_ProductRepo ProductRepo)
        {
            ProductRepo.getclasscode(ProductRepo);


            if (ProductRepo.Class_Id == "04" || ProductRepo.Class_Id == "05")
            {
                this.GetCmp<NumberField>("ProductP_UnitPrice").SetHidden(false);
                this.GetCmp<ComboBox>("ProductP_Listed").SetHidden(false);
               
            }
            else
            {
                this.GetCmp<NumberField>("ProductP_UnitPrice").SetValue("0");
                this.GetCmp<ComboBox>("ProductP_Listed").SetValue("NO");
                this.GetCmp<NumberField>("ProductP_UnitPrice").SetHidden(true);
                this.GetCmp<ComboBox>("ProductP_Listed").SetHidden(true);

            }

            if (ProductRepo.Class_Id == "03")
            {             
                this.GetCmp<ComboBox>("txt_Bond_Type").SetHidden(false);
                this.GetCmp<ComboBox>("txt_FIX_float").SetHidden(false);
            }
            else
            {             
                this.GetCmp<ComboBox>("txt_Bond_Type").SetHidden(true);
                this.GetCmp<ComboBox>("txt_Bond_Type").SetValue("NA");
                this.GetCmp<ComboBox>("txt_FIX_float").SetHidden(true);
                this.GetCmp<ComboBox>("txt_FIX_float").SetValue("NA");
            }

            return this.Direct();
        }

     

    }
}


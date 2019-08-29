using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.GlobalObject;
using PENFAD6DAL.Repository.CRM.Employer;
using System;
using System.Linq;
using System.Web.Mvc;
using PENFAD6DAL.Services;
using PENFAD6UI.Services;


namespace PENFAD6UI.Areas.CRM.Controllers.Employer
{
    public class EmployerController : Controller
    {
        readonly crm_EmployerRepo EmployerRepo = new crm_EmployerRepo();
        
        string error = "";
        // GET: Employer

        // Written by Fredrick T. Lutterodt
        public ActionResult AddNewEmployerTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "AddNewEmployerPartial",
                Model = EmployerRepo.GetEmployerList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }

        public ActionResult AddEditEmployerTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "AddEditEmployerPartial",
                Model = EmployerRepo.GetEmployerList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }

		public ActionResult AddEditEmployerStatusTab(string containerId = "MainArea")
		{
			var pvr = new Ext.Net.MVC.PartialViewResult
			{
				ViewName = "AddEditEmployerStatusPartial",
				Model = EmployerRepo.GetEmployerList(),
				ContainerId = containerId,
				RenderMode = RenderMode.AddTo,
			};
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

			return pvr;
		}

		public ActionResult AddDeleteEmployerTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "AddDeleteEmployerPartial",
                Model = EmployerRepo.GetEmployerList(),
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
                var x = X.GetCmp<FormPanel>("employerEdit");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        public void ClearControls_Delete()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("employerDelete");
                x.Reset();
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        public ActionResult AddEmployerEnquiryTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "AddViewEmployerPartial",
                Model = EmployerRepo.GetEmployerList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            return pvr;
        }
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult SaveRecord(crm_EmployerRepo EmployerRepo)
        { 

            EmployerRepo.Employer_Status = "PENDING";
            EmployerRepo.Maker_Id = GlobalValue.User_ID;
            EmployerRepo.Auth_Status = "PENDING";

            //checking if employer already exist in editing queue.

            //if ( String.IsNullOrEmpty(EmployerRepo.Contact_Person))
            //{
            //    X.Msg.Show(new MessageBoxConfig
            //    {
            //        Title = "Error",
            //        Message = "Contact Person is required.",
            //        Buttons = MessageBox.Button.OK,
            //        Icon = MessageBox.Icon.INFO,
            //        Width = 350
            //    });
            //    return this.Direct();
            //}

            if (String.IsNullOrEmpty( EmployerRepo.Region_Id))
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Region is required.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                return this.Direct();
            }

            if (String.IsNullOrEmpty(EmployerRepo.District_Id))
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "District is required.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                return this.Direct();
            }

            if (String.IsNullOrEmpty(EmployerRepo.Sector_Id))
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sector is required.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                return this.Direct();
            }

            if (String.IsNullOrEmpty(EmployerRepo.Scheme_Id))
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Select Scheme.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                return this.Direct();
            }

            if (ModelState.IsValid)

            {
                if (this.EmployerRepo.EmployerExist(EmployerRepo.Employer_Name, out error))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = $"Employer name - {EmployerRepo.Employer_Name.ToUpper()} already exist.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Width = 350
                    });
                    return this.Direct();
                }
                
                this.EmployerRepo.SaveRecord(EmployerRepo);

                var x = X.GetCmp<FormPanel>("employerNew");
                x.Reset();

                // display success message to user 
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Saved Successfully.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                
                return this.Direct();
            }

            // Build error messages for invalid model state
            var messages = string.Join(Environment.NewLine, ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage));

            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Error",
                Message = messages,
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.ERROR,
                Width = 350
            });
            return this.Direct();
        }

        public ActionResult UpdateRecord(crm_EmployerRepo EmployerRepo)
        {
            try
            {

            EmployerRepo.Maker_Id = GlobalValue.User_ID;
            EmployerRepo.Auth_Status = "EDITING";

            //if (String.IsNullOrEmpty(EmployerRepo.Contact_Person))
            //{
            //    X.Msg.Show(new MessageBoxConfig
            //    {
            //        Title = "Error",
            //        Message = "Contact Person is required.",
            //        Buttons = MessageBox.Button.OK,
            //        Icon = MessageBox.Icon.INFO,
            //        Width = 350
            //    });
            //    return this.Direct();
            //}

            if (String.IsNullOrEmpty(EmployerRepo.Region_Id))
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Region is required.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                return this.Direct();
            }

            if (String.IsNullOrEmpty(EmployerRepo.District_Id))
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "District is required.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                return this.Direct();
            }

            if (String.IsNullOrEmpty(EmployerRepo.Sector_Id))
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sector is required.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                return this.Direct();
            }


            if (ModelState.IsValid)
            {
                            
                this.EmployerRepo.UpdateRecord(EmployerRepo);

                ClearControls();
                Store store = X.GetCmp<Store>("employerStore");
                store.Reload();

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Updated Successfully.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });

                return this.Direct();
            }
            var messages = string.Join(Environment.NewLine, ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage));

            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Error",
                Message = messages,
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.ERROR,
                Width = 350
            });
            return this.Direct();
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

        public ActionResult DeleteEmployerRecord(crm_EmployerRepo EmployerRepo)
        {
            try
            {

                if (String.IsNullOrEmpty(EmployerRepo.Employer_Id))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select Employer. Process aborted",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    return this.Direct();
                }


               
                    this.EmployerRepo.DeleteEmployerRecord(EmployerRepo);

                    ClearControls_Delete();
                    Store store = X.GetCmp<Store>("employerdeleteStore");
                    store.Reload();
                
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Employer Deleted Successfully.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });

                    return this.Direct();
                
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
                    ora_code = "Record can not be deleted. Process aborted..";
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


		public ActionResult UpdateRecord_Status(crm_EmployerRepo EmployerRepo)
		{
			try
			{

				EmployerRepo.Maker_Id = GlobalValue.User_ID;

				if (string.IsNullOrEmpty(EmployerRepo.Employer_Id))
				{
					X.Msg.Show(new MessageBoxConfig
					{
						Title = "Error",
						Message = "Please select Employer. Process aborted",
						Buttons = MessageBox.Button.OK,
						Icon = MessageBox.Icon.INFO,
						Width = 350
					});
					return this.Direct();
				}

				if (string.IsNullOrEmpty(EmployerRepo.Employer_Status))
				{
					X.Msg.Show(new MessageBoxConfig
					{
						Title = "Error",
						Message = "Please select New Status. Process aborted",
						Buttons = MessageBox.Button.OK,
						Icon = MessageBox.Icon.INFO,
						Width = 350
					});
					return this.Direct();
				}

				this.EmployerRepo.UpdateRecord_Status(EmployerRepo);

				X.Msg.Show(new MessageBoxConfig
				{
					Title = "Success",
					Message = "Employer Status Successfully Changed.",
					Buttons = MessageBox.Button.OK,
					Icon = MessageBox.Icon.INFO,
					Width = 350
				});
				var pvr = new Ext.Net.MVC.PartialViewResult
				{
					ViewName = "AddEditEmployerStatusPartial",
					Model = EmployerRepo.GetEmployerList(),
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
		public ActionResult Read()
        {
            return this.Store(EmployerRepo.GetEmployerList());
        }

        
    }
}
using Ext.Net;
using Ext.Net.MVC;
using System.Web.Mvc;
using System;
using PENFAD6DAL.Repository.Setup.SystemSetup;
using PENFAD6DAL.GlobalObject;
using System.Web;
using System.Collections.Generic;

namespace PENFAD6UI.Areas.Setup.Controllers.SystemSetup
{
    public class setup_CompanyController : Controller
    {
         List<setup_CompanyRepo> empList = new List<setup_CompanyRepo>();
        readonly setup_CompanyRepo CompanyRepo = new setup_CompanyRepo();

        cLogger logger = new cLogger();
        public static byte[] mImage;
        public static byte[] mImage_ID;
        public ActionResult Index()
        {
            return View();
        }
       
        public ActionResult AddCompanyTab(string containerId= "MainArea")
        {
            try {
                var pvr = new Ext.Net.MVC.PartialViewResult
                {
                    ViewName = "CompanyPartial",
                    Model = CompanyRepo.GetCompanyList(),
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo,
                };
                X.Mask.Hide();
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
                return pvr;
            }
            catch(Exception ex)
            {
                X.Mask.Hide();
                throw ex;
            }
        }

        public ActionResult DisplayPhoto(string COMPANY_NAME)
        {
            try
            {
                if (string.IsNullOrEmpty(COMPANY_NAME))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Company record is required",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350


                    });
                    return this.Direct();
                }

                empList = CompanyRepo.GetEmpList(COMPANY_NAME);

                if (empList.Count > 0)
                {

                    byte[] pic = null;


                    pic = empList[0].LOGO;

                    //if (kycList[0].SIGNATURE.Length > 0)
                    pic = empList[0].LOGO;


                    if ((pic != null && pic.Length > 0))
                    {
                        this.GetCmp<Image>("empEdit_pic1_company").ImageUrl = "../ImageProcessorLogo/ViewImageProcessorLogo.ashx?COMPANY_NAME=" + COMPANY_NAME; // + ", " + Pict_Image; 
                    }  


                }

                return this.Direct();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult SavePhoto()
        {
            try
            {
                if (X.GetCmp<FileUploadField>("empEdit_btnloadfile_company").HasFile)
                {
                    HttpPostedFile file_posted = this.GetCmp<FileUploadField>("empEdit_btnloadfile_company").PostedFile;

                    ImageWork.Current_Image_Path = ImageWork.ImagePhysicalPath(file_posted);

                    mImage = ImageWork.ImageToByte(ImageWork.Current_Image_Path);

                    //clear image field
                    this.GetCmp<Image>("empEdit_pic1_company").ImageUrl = "";
                    this.GetCmp<Image>("empEdit_pic1_company").ImageUrl = "../ImageProcessorLogo/ImageProcessorLogo.ashx?Filedata=" + ImageWork.Current_Image_Path;

                }

               
                return this.Direct();
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult Read()
        {
            try {
                return this.Store(CompanyRepo.GetCompanyList());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult UpdateRecord(setup_CompanyRepo CompanyRepo)
        {

            try
            {
           
                CompanyRepo.LOGO = mImage;

                if (string.IsNullOrEmpty(CompanyRepo.COMPANY_NAME))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Please select the Company to proceed.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350

                    });
                    return this.Direct();
                }
               
                //update employee record
                if (this.CompanyRepo.AddEmployeeRecordToTmp(CompanyRepo))
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Company Updated Successfully.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
      
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Process aborted.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO,
                        Width = 350
                    });
                    return this.Direct();
                }
                //Store store = X.GetCmp<Store>("employeeStore");
                //store.Reload();


                return this.Direct();
            }
            catch (Exception ex)
            {
                logger.WriteLog(ex.Message);
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Process failed -" + ex.Message,
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350


                });
                return this.Direct();
            }

        }


    }
}


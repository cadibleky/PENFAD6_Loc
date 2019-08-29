using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;


namespace PENFAD6UI.Areas.Security.Controllers
{
    public class SecurityController : Controller
    {
        // GET: Security/Security
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddUserRolesPartial(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "UserRolePartial",
                //Model = modulerepo.GetAllModules(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData = this.ViewData
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetNextTabAsActive();

            return (pvr);
        }










    }
}
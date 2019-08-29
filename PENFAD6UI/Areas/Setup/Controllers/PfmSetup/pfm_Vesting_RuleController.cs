
using Ext.Net;
using Ext.Net.MVC;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using System.Web.Mvc;

namespace PENFAD6UI.Areas.Setup.Controllers.PfmSetup
{
    public class pfm_Vesting_RuleController : Controller
    {
        readonly pfm_VestingRuleRepo VestingRuleRepo = new pfm_VestingRuleRepo();       
        public ActionResult Index()
        {
            return View();
        }
        public void ClearControls()
        {
            try
            {
                var x = X.GetCmp<FormPanel>("vestingRule");
                x.Reset();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public ActionResult AddVestingRuleTab(string containerId = "MainArea")
        {
            var pvr = new Ext.Net.MVC.PartialViewResult
            {
                ViewName = "VestingRulePartial",
                Model = VestingRuleRepo.GetVestingRuleList(),
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
            };
            X.Mask.Hide();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return pvr;
        }
        public ActionResult SaveRecord(pfm_VestingRuleRepo VestingRuleRepo)
 
        {
            if (VestingRuleRepo.isVestingRuleUnique(VestingRuleRepo.Vesting_Rule) == true)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Duplicate",
                    Message = "Vesting Rule '"+ VestingRuleRepo.Vesting_Rule + "' already exist.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                return this.Direct();
            }
            if (ModelState.IsValid)
            {
                this.VestingRuleRepo.SaveRecord(VestingRuleRepo);

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Success",
                    Message = "Vesting Rule Successfully added.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                ClearControls();
                Store store = X.GetCmp<Store>("vestingRuleStore");
                store.Reload();

                return this.Direct();
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "ERROR",
                    Message = "Incomplete Entry.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Width = 350
                });
                Store store = X.GetCmp<Store>("vestingRuleStore");
                store.Reload();

                return this.Direct();
            }
                // return this.Direct();                    

    }
        public ActionResult DeleteRecord(string VESTING_RULE_ID)
        {
            if (VESTING_RULE_ID == string.Empty)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Sorry! No 'Vesting Rule' has been selected.",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Width = 350
                });
                return this.Direct();
            }

            VestingRuleRepo.DeleteRecord(VESTING_RULE_ID);
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Success",
                Message = "Vesting Rule Successfully deleted.",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO,
                Width = 350


            });
            ClearControls();
            Store store = X.GetCmp<Store>("vestingRuleStore");
            store.Reload();

            return this.Direct();
        }
        public ActionResult Read()
        {
            return this.Store(VestingRuleRepo.GetVestingRuleList());
        }  
}
}


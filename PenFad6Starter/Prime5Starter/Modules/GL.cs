using PenFad6Starter.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PenFad6Starter.Modules
{
    public class GL
    {
        addController adcon = new addController();
        public void add_GL_Modules()
        {
            try
            {
                string tree = "T";
                string view = "V";
                int parentid = 0;
                int modulecode = 14;
                int aa = 0;
                //foldername/controllername/action


                //############################# GL Operations ####################################
                ////GL  -Parent level : starts with = 11 -----0-----11
                parentid = 0;
                aa = 1401;
                adcon.add_Module_oracle(1401, "GL Journal Operations", parentid, modulecode, "url", aa, "N", "description", tree);//N

              
                parentid = 1401;
                aa = 110201;
                adcon.add_Module_oracle(140101, "New Journal", parentid, modulecode, "GL/GLJournal/AddGLJournalTab", aa += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(140102, "Approve Journal", parentid, modulecode, "GL/GLJournal/AddGLJournalApproveTab", aa += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(140103, "Reverse Journal", parentid, modulecode, "GL/GLJournal/AddGLJournalReversalTab", aa += 1, "L", "description", tree);//L

                //############################# GL CHART ####################################
                //GL setup-Parent level : starts with = 1101 -----0-----11
                parentid = 0;
                aa = 1402;
                adcon.add_Module_oracle(1402, "GL Chart of Accounts", parentid, modulecode, "url", aa, "N", "description", tree);//N

                parentid = 1402;
                aa = 110201;
                adcon.add_Module_oracle(140201, "GL Chart of Account", parentid, modulecode, "GL/GLChart/AddGLChartTab", aa += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(140202, "GL Account For Scheme-Fund", parentid, modulecode, "GL/GLChart/AddGLAccountTab", aa += 1, "L", "description", tree);//L
                                                                                                                                                                     //adcon.add_Module_oracle(140203, "Create GL Bank Account", parentid, modulecode, "GL/GLChart/AddGLAccountBankTab", aa += 1, "L", "description", tree);//L
                parentid = 0;
                aa = 1403;
                adcon.add_Module_oracle(1403, "Fee Payment", parentid, modulecode, "url", aa, "N", "description", tree);//N

                parentid = 1403;
                aa = 110201;
                adcon.add_Module_oracle(140301, "New Fee Payment", parentid, modulecode, "GL/GLFee/AddGLFeeTab", aa += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(140302, "Approve Fee Payment", parentid, modulecode, "GL/GLFee/AddGLFeeApproveTab", aa += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(140303, "Reverse Fee Payment", parentid, modulecode, "GL/GLFee/AddGLFeeReversalTab", aa += 1, "L", "description", tree);//L

                //adcon.add_Module_oracle(140204, "Configure Default Bank Account", parentid, modulecode, "GL/GLChart/AddGLAccountBankDefaultTab", aa += 1, "L", "description", tree);//L

                //############################# GL CHART ####################################
                //GL setup-Parent level : starts with = 1101 -----0-----11
                parentid = 0;
                aa = 1406;
                adcon.add_Module_oracle(1406, "Financial Reports", parentid, modulecode, "url", aa, "N", "description", tree);//N

                parentid = 1406;
                aa = 110201;
                adcon.add_Module_oracle(140601, "View GL", parentid, modulecode, "GL/Report_Ledger_Account/Report_Ledger_Account_Tab", aa += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(140602, "Financial Reports-Fund Level", parentid, modulecode, "GL/Report_YearlyTB/Report_TB_Tab", aa += 1, "L", "description", tree);//L
				adcon.add_Module_oracle(140603, "Financial Reports-Scheme Level", parentid, modulecode, "GL/Report_YearlyTB_scheme/Report_TB_Tab", aa += 1, "L", "description", tree);//L
                parentid = 1406;
                aa = 140604;
                adcon.add_Module_oracle(140604, "Consolidated GL", parentid, modulecode, "url", aa, "N", "description", tree);//N
                parentid = 140604;
                aa = 140604;
                adcon.add_Module_oracle(14060401, "Merge GL Accounts", parentid, modulecode, "GL/GLConsolidated/AddSchemeab", aa += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(14060402, "View Consolidated GL", parentid, modulecode, "GL/Report_Ledger_AccountConsolidated/Report_Ledger_Account_Tab", aa += 1, "L", "description", tree);//L


                parentid = 0;
                aa = 1407;
                adcon.add_Module_oracle(1407, "Other Reports", parentid, modulecode, "url", aa, "N", "description", tree);//N

                parentid = 1407;
                aa = 110201;
                adcon.add_Module_oracle(140701, "Daily Net Asset Fees", parentid, modulecode, "CRM/Report_DailyFees/Report_DailyFees_Tab", aa += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(140702, "Daily Unit Prices/NAVs", parentid, modulecode, "CRM/Report_DailyUnitPrice/Report_DailyUnitPrice_Tab", aa += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(140703, "Benefit Summary", parentid, modulecode, "CRM/Report_EmployeeBen/Report_EmployeeBen_Tab", aa += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(140704, "Scheme Summary", parentid, modulecode, "CRM/Report_SchemeSummary/Report_Scheme_Tab", aa += 1, "L", "description", tree);//L



            }
            catch (Exception ex)
            {
                string sss = ex.ToString();
                throw ex;
            }


        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PenFad6Starter.Modules
{
   public class Investment_13
    {
        addController adcon = new addController();

        public void add_Investment_Modules()
        {
            try
            {
                string tree = "T";
                string view = "V";
                int parentid = 0;
                int modulecode = 13;
                int posi = 0;
                string Node = "N";
                string Leaf = "L";
                //foldername/controllername/action

                //################################### Contribution Upload Manager   ####################################
                //Scheme Setup -Parent level : starts with = 1201 -----0-----12
                parentid = 0;
                posi = 1301;
                adcon.add_Module_oracle(1301, "Money Market/T-Bill", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                adcon.add_Module_oracle(1302, "Bonds", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                adcon.add_Module_oracle(1303, "Equity/CIS", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                adcon.add_Module_oracle(1304, "Scheme-Fund Level Reports", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                adcon.add_Module_oracle(1305, "Scheme Level Reports", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                adcon.add_Module_oracle(1306, "Fund Manager Level Reports", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N

                //################################### Money Market/T-Bill  ####################################
                parentid = 1301;  //----level 2
                posi = 1301;
                adcon.add_Module_oracle(130101, "New Investment", parentid, modulecode, "Investment/FixedIncome_Transaction/AddFixedIncomeTransTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(130102, "Approve Investment", parentid, modulecode, "Investment/ApproveFixed_InCome_Transaction/ApproveFixedIncomeTransTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(130103, "Reverse Investment", parentid, modulecode, "Investment/DisinvestFixed_InCome_Transaction/DisinvestTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(130104, "Disinvest Security", parentid, modulecode, "Investment/Receipt/ReceiptTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(130105, "Confirm interest Receipt ", parentid, modulecode, "Investment/Receipt/ReceiptIntTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(130106, "Confirm Matured Investment Receipt", parentid, modulecode, "Investment/Receipt/ReceiptMTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(130107, "Reverse Investment Receipt", parentid, modulecode, "Investment/Receipt/ReceiptReverseTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(130108, "Adjust Accrued Interest", parentid, modulecode, "Investment/Receipt/ReceiptAccruedTab", posi += 1, Leaf, "description", tree);//L



                //################################### Bonds  ####################################

                parentid = 1302;  //----level 2
                posi = 1302;
                adcon.add_Module_oracle(130201, "Primary", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                adcon.add_Module_oracle(130202, "Secondary", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                adcon.add_Module_oracle(130203, "Others", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N

                //################################### Primary  ####################################
                parentid = 130201;  //----level 2
                posi = 130201;
                adcon.add_Module_oracle(13020101, "New Bond", parentid, modulecode, "Investment/Bond/AddBondTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(13020102, "Approve Bond", parentid, modulecode, "Investment/ApproveBond/AddApproveBondTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(13020103, "Reverse Bond", parentid, modulecode, "Investment/ReverseBond/AddReverseBondTab", posi += 1, Leaf, "description", tree);//L
               
                parentid = 130203;  //----level 2
                posi = 130203;
                adcon.add_Module_oracle(13020301, "Confirm Interest Receipt", parentid, modulecode, "Investment/Receipt/ReceiptBTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(13020302, "Confirm Matured Bond Receipt", parentid, modulecode, "Investment/Receipt/ReceiptMBTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(13020303, "Reverse Bond Receipt", parentid, modulecode, "Investment/Receipt/ReceiptReverseBondsTab", posi += 1, Leaf, "description", tree);//L
                //adcon.add_Module_oracle(13020304, "Disinvest Bond", parentid, modulecode, "Investment/Receipt/ReceiptDisinvestBondsTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(13020304, "Adjust Accrued Interest", parentid, modulecode, "Investment/Receipt/AdjustAccruedBondsTab", posi += 1, Leaf, "description", tree);//L



                parentid = 130202;  //----level 2
                posi = 130202;
                adcon.add_Module_oracle(13020201, "Purchase", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                adcon.add_Module_oracle(13020202, "Sale", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
               
                //################################### Secondary purchase  ####################################
                parentid = 13020201;  //----level 2
                posi = 13020201;
                adcon.add_Module_oracle(1302020101, "New Purchase", parentid, modulecode, "Investment/Bond/AddSecBondTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(1302020102, "Approve Purchase", parentid, modulecode, "Investment/ApproveBond/AddApproveSecBondTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(1302020103, "Reverse Purchase", parentid, modulecode, "Investment/ReverseBond/AddReverseSecBondTab", posi += 1, Leaf, "description", tree);//L
               
                //################################### Secondary Sell  ####################################
                parentid = 13020202;  //----level 2
                posi = 13020202;
                adcon.add_Module_oracle(1302020201, "New Sale", parentid, modulecode, "Investment/Bond/AddSellSecBondTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(1302020202, "Reverse Sale", parentid, modulecode, "Investment/Bond/AddSellSecBondReverseTab", posi += 1, Leaf, "description", tree);//L
              
                //################################### Equity/CIS ####################################
                parentid = 1303;  //----level 2
                posi = 1303;
                adcon.add_Module_oracle(130301, "Purchase", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                adcon.add_Module_oracle(130302, "Sale", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N


                parentid = 130301;  //----level 2
                posi = 130301;
                adcon.add_Module_oracle(13030101, "New Purchase", parentid, modulecode, "Investment/Equity_CIS/AddEquity_CISTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(13030102, "Approve Purchase", parentid, modulecode, "Investment/Equity_CIS/AddEquity_CISApproveTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(13030103, "Reverse Purchase", parentid, modulecode, "Investment/Equity_CIS/AddEquity_CISReverseTab", posi += 1, Leaf, "description", tree);//L

                parentid = 130302;  //----level 2
                posi = 130302;
                adcon.add_Module_oracle(13030201, "New Sale", parentid, modulecode, "Investment/Equity_CIS_Sell/AddEquity_CISTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(13030202, "Approve Sale", parentid, modulecode, "Investment/Equity_CIS_Sell/AddEquity_CISApproveTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(13030203, "Reverse Sale", parentid, modulecode, "Investment/Equity_CIS_Sell/AddEquity_CISReverseTab", posi += 1, Leaf, "description", tree);//L


                //################################### reports  ####################################
                parentid = 1304;  //----level 2
                posi = 1304;
                adcon.add_Module_oracle(130401, "Portfolio Valuation", parentid, modulecode, "Investment/Reports/AddPVTab", posi += 1, Leaf, "description", tree);//L
               // adcon.add_Module_oracle(130402, "Investment Receipts", parentid, modulecode, "CRM/Report_SchemeInvestmentReceipt/Report_SchemeInvestmentReceipt_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(130403, "Investment Maturity", parentid, modulecode, "Investment/IM_Reports/AddIMTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(130404, "Coupon Maturity", parentid, modulecode, "Investment/CM_Reports/AddCMTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(130405, "Security Purchases", parentid, modulecode, "Investment/SP_Reports/AddSPTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(130406, "Dashboard", parentid, modulecode, "Investment/Dashboard/AddDBTab", posi += 1, Leaf, "description", tree);//L

                parentid = 1305;  //----level 2
                posi = 1305;
                adcon.add_Module_oracle(130501, "Portfolio Valuation", parentid, modulecode, "Investment/SchemeReports/AddPVTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(130402, "Investment Receipts", parentid, modulecode, "CRM/Report_SchemeInvestmentReceipt/Report_SchemeInvestmentReceipt_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(130503, "Investment Maturity", parentid, modulecode, "Investment/SchemeIM_Reports/AddIMTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(130504, "Coupon Maturity", parentid, modulecode, "Investment/SchemeCM_Reports/AddCMTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(130505, "Security Purchases", parentid, modulecode, "Investment/SchemeSP_Reports/AddSPTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(130506, "Dashboard", parentid, modulecode, "Investment/SchemeDashboard/AddDBTab", posi += 1, Leaf, "description", tree);//L

                parentid = 1306;  //----level 2
                posi = 1306;
                adcon.add_Module_oracle(130601, "Portfolio Valuation", parentid, modulecode, "Investment/FMReports/AddPVTab", posi += 1, Leaf, "description", tree);//L
               
            }
            catch (Exception ex)
            {
                string sss = ex.ToString();
                throw ex;
            }

        }













    }
}

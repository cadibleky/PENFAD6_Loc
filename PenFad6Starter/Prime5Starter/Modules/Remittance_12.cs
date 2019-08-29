using PenFad6Starter.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PenFad6Starter.Modules
{
    public class Remittance_12
    {

        addController adcon = new addController();

        public void add_Remiitance_Modules()
        {
            try
            {
                string tree = "T";
                string view = "V";
                int parentid = 0;
                int modulecode = 12;
                int posi = 0;
                string Node = "N";
                string Leaf = "L";
                //foldername/controllername/action

                //################################### Contribution Upload Manager   ####################################
                //Scheme Setup -Parent level : starts with = 1201 -----0-----12
                parentid = 0;
                posi = 1201;
                adcon.add_Module_oracle(1201, "Contribution Upload Manager", parentid, modulecode, "url", posi, Node, "description", tree);//N

                parentid = 1201;  //----level 2
                posi = 1201;
                adcon.add_Module_oracle(120101, "Upload Employee Remittance", parentid, modulecode, "Remittance/Remit_ContributionInitialUpLoad_Log/AddContributionRemitTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120102, "Upload Supplementary Remittance", parentid, modulecode, "Remittance/Remit_ContributionSupUpLoad_Log/AddContributionRemitTab_Sup", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120103, "Upload Back Pay Remittance", parentid, modulecode, "Remittance/Remit_ContributionBackUpLoad_Log/AddContributionRemitTab_Back", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120104, "Upload TPFA Remittance", parentid, modulecode, "Remittance/Remit_ContributionTPFAUpLoad_Log/AddContributionRemitTab_TPFA", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120105, "Upload Surcharge", parentid, modulecode, "Remittance/Remit_ContributionSURCHARGEUpLoad_Log/AddContributionRemitTab_SURCHARGE", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120106, "Approve Uploaded Remittance", parentid, modulecode, "Remittance/ApprovedRemit_Contribution/AddApproveContributionTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120107, "Send/Download Payment Advice", parentid, modulecode, "Remittance/PayAdviceRemit_Contribution/AddPayAdviceTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120108, "Delete Uploaded Remittance", parentid, modulecode, "Remittance/DeleteRemit_Contribution/AddDeleteContributionTab", posi += 1, Leaf, "description", tree);//L

                //################################### Receipts/Payments  ####################################
                //Scheme Setup -Parent level : starts with = 1202 -----0-----12
                parentid = 0;
                posi = 1202;
                adcon.add_Module_oracle(1202, "Confirm Employer Payment", parentid, modulecode, "url", posi, Node, "description", tree);//N

                parentid = 1202;  //----level 2
                posi = 1202;
                adcon.add_Module_oracle(120201, "Capture Payment", parentid, modulecode, "Remittance/Remit_Receipt/AddReceiptTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120202, "Approve Payment", parentid, modulecode, "Remittance/Remit_Receipt/AddReceiptApproveTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120203, "Reverse Payment", parentid, modulecode, "Remittance/Remit_Receipt/AddReverseReceiptTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120204, "Send/Download Payment Confirmation", parentid, modulecode, "Remittance/Remit_Receipt/AddSendReceiptTab", posi += 1, Leaf, "description", tree);//L
                //adcon.add_Module_oracle(120204, "Capture Batch Payment", parentid, modulecode, "Remittance/Remit_Receipt/AddReverseReceiptTab", posi += 1, Leaf, "description", tree);//L


                //################################### PURCHASE  ####################################
                //Scheme Setup -Parent level : starts with = 1203 -----0-----12
                parentid = 0;
                posi = 1203;
                adcon.add_Module_oracle(1203, "Unit Purchase Operation", parentid, modulecode, "url", posi, Node, "description", tree);//N

                parentid = 1203;  //----level 2
                posi = 1203;
                adcon.add_Module_oracle(120301, "Purchase Units", parentid, modulecode, "Remittance/Remit_Purchase/AddPurchaseTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120302, "Approve Purchases", parentid, modulecode, "Remittance/Remit_Purchase/AddPurchaseApproveTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120303, "Reversal-Purchases", parentid, modulecode, "Remittance/Remit_Purchase/AddPurchaseReverseTab", posi += 1, Leaf, "description", tree);//L

                //################################### REPORTS  ####################################
                //Scheme Setup -Parent level : starts with = 1204 -----0-----12
                parentid = 0;
                posi = 1204;
                adcon.add_Module_oracle(1204, "Amend Remittance", parentid, modulecode, "url", posi, Node, "description", tree);//N

                parentid = 1204;  //----level 2
                posi = 1204;

                adcon.add_Module_oracle(120401, "Unit Transfer", parentid, modulecode, "Remittance/Remit_Unit_Transfer/AddUnit_TransferTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120402, "Approve Unit Transfer", parentid, modulecode, "Remittance/Remit_Unit_Transfer/AddUnit_TransferApproveTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120403, "Merge Employee Accounts", parentid, modulecode, "Remittance/Remit_Unit_Transfer/AddUnit_MergeTransferTab", posi += 1, Leaf, "description", tree);//L
                //adcon.add_Module_oracle(120404, "Delete Contribution", parentid, modulecode, "Remittance/Remit_Unit_Change/AddUnit_ChangeTab", posi += 1, Leaf, "description", tree);//L

                //################################### WITHDRAWAL  ####################################
                //Scheme Setup -Parent level : starts with = 1203 -----0-----12
                parentid = 0;
                posi = 1205;
                adcon.add_Module_oracle(1205, "Benefit Payment", parentid, modulecode, "url", posi, Node, "description", tree);//N

                parentid = 1205;  //----level 2
                posi = 1205;
                adcon.add_Module_oracle(120501, "Benefit Request", parentid, modulecode, "Remittance/Remit_Withdrawal/AddWithdrawalTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120502, "Approve Benefit Request", parentid, modulecode, "Remittance/Remit_Withdrawal/AddWithdrawalApproveTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120503, "Pay Benefit", parentid, modulecode, "Remittance/Remit_Withdrawal/AddWithdrawalPayTab", posi += 1, Leaf, "description", tree);//L           
                adcon.add_Module_oracle(120504, "Pay Benefit Tax", parentid, modulecode, "Remittance/Remit_Withdrawal/AddWithdrawalPayTaxTab", posi += 1, Leaf, "description", tree);//L           
                adcon.add_Module_oracle(120505, "Reverse Benefit Paid", parentid, modulecode, "Remittance/Remit_Withdrawal/AddWithdrawalReverseTab", posi += 1, Leaf, "description", tree);//L           

                parentid = 0;
                posi = 1206;
                adcon.add_Module_oracle(1206, "Porting In", parentid, modulecode, "url", posi, Node, "description", tree);//N

                parentid = 1206;  //----level 2
                posi = 1206;
                adcon.add_Module_oracle(120601, "New Port-In", parentid, modulecode, "Remittance/Remit_PortIn/AddPortInTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120602, "Approve Port-In", parentid, modulecode, "Remittance/Remit_PortIn/AddPortInApproveTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120603, "Reverse Port-In", parentid, modulecode, "Remittance/Remit_PortIn/AddPortInReverseTab", posi += 1, Leaf, "description", tree);//L

                parentid = 0;
                posi = 1207;
                adcon.add_Module_oracle(1207, "Porting Out", parentid, modulecode, "url", posi, Node, "description", tree);//N


                parentid = 1207;  //----level 2
                posi = 1207;
                adcon.add_Module_oracle(120701, "New Port-Out", parentid, modulecode, "Remittance/Remit_PortOut/AddPortOutTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120702, "Approve Port-Out", parentid, modulecode, "Remittance/Remit_PortOut/AddPortOutApproveTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120703, "Pay Port-Out", parentid, modulecode, "Remittance/Remit_PortOut/AddPortOutPayTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120704, "Reverse Port-Out", parentid, modulecode, "Remittance/Remit_PortOut/AddPortOutReverseTab", posi += 1, Leaf, "description", tree);//L
				adcon.add_Module_oracle(120705, "Port-Out Employer", parentid, modulecode, "Remittance/BatchPortOut/AddPortOutEmployerTab", posi += 1, Leaf, "description", tree);//L

                parentid = 0;
                posi = 1208;
                adcon.add_Module_oracle(1208, "Benefit Transfer To Employer", parentid, modulecode, "url", posi, Node, "description", tree);//N
                parentid = 1208;  //----level 2
                posi = 1208;
                adcon.add_Module_oracle(120801, "New Benefit Transfer", parentid, modulecode, "Remittance/Remit_WithdrawalToEmployer/AddWithdrawalTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120802, "Approve Benefit Transfer", parentid, modulecode, "Remittance/Remit_WithdrawalToEmployer/AddWithdrawalApproveTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120804, "Reverse Benefit Transfer", parentid, modulecode, "Remittance/Remit_WithdrawalToEmployer/AddWithdrawalReverseTab", posi += 1, Leaf, "description", tree);//L


                parentid = 0;
                posi = 1209;
                adcon.add_Module_oracle(1209, "Send SMS-Blast", parentid, modulecode, "url", posi, Node, "description", tree);//N
                parentid = 1209;  //----level 2
                posi = 1209;
                adcon.add_Module_oracle(120901, "Send SMS-Blast to Employers", parentid, modulecode, "CRM/EmployeeSMSStatement/AddEmployeeSMSStatementTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(120902, "Send SMS-Blast to Employees", parentid, modulecode, "CRM/EmployeeSMSStatement/AddEmployeeSMSStatementIndiTab", posi += 1, Leaf, "description", tree);//L



                parentid = 0;
                posi = 1210;
                adcon.add_Module_oracle(1210, "Scheme Reports", parentid, modulecode, "url", posi, Node, "description", tree);//N

                parentid = 1210;  //----level 2
                posi = 1210;
                adcon.add_Module_oracle(121001, "Accrued Benefit", parentid, modulecode, "CRM/Report_SchemeBenefit/Report_SchemeBenefit_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(121002, "Current Members", parentid, modulecode, "CRM/Report_SchemeMembership/Report_SchemeMembership_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(121003, "Deferred Members", parentid, modulecode, "CRM/Report_SchemeMembershipDeferred/Report_SchemeMembershipDeferred_Tab", posi += 1, Leaf, "description", tree);//L
				adcon.add_Module_oracle(121004, "Total Active Members (As At)", parentid, modulecode, "CRM/Report_SchemeMembershipAll/Report_SchemeMembership_Tab", posi += 1, Leaf, "description", tree);//L
				adcon.add_Module_oracle(121005, "Total Members Enrolled (Period)", parentid, modulecode, "CRM/Report_SchemeMembershipEnrolled/Report_SchemeMembership_Tab", posi += 1, Leaf, "description", tree);//L
				adcon.add_Module_oracle(121006, "Total Members Exited (Period)", parentid, modulecode, "CRM/Report_SchemeMembershipExitted/Report_SchemeMembership_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(121007, "Total Members (Age)", parentid, modulecode, "CRM/Report_SchemeMembershipAge/Report_SchemeMembership_Tab", posi += 1, Leaf, "description", tree);//L

                adcon.add_Module_oracle(121008, "Active Employers (As At)", parentid, modulecode, "CRM/Report_SchemeEmployer/Report_SchemeEmployer_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(121009, "Total Employers Enrolled (Peroid)", parentid, modulecode, "CRM/Report_SchemeEmployerEnrolled/Report_SchemeEmployerEnrolled_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(121010, "Paid Schedule(Period)", parentid, modulecode, "CRM/Report_Monthly_Schedule_Scheme/Report_MSS_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(121011, "Defaulters Schedule(Monthly)", parentid, modulecode, "CRM/Report_Monthly_Schedule_Defaulters/Report_MSSD_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(121012, "Benefits and Withdrawals (Period)", parentid, modulecode, "CRM/Report_SchemeMembershipWithdrawal/Report_SchemeMembership_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(121013, "Port In (Period)", parentid, modulecode, "CRM/Report_SchemeMembershipPortIn/Report_SchemeMembership_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(121014, "Port Out (Period)", parentid, modulecode, "CRM/Report_SchemeMembershipPortOut/Report_SchemeMembership_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(121015, "Preservation Schedule", parentid, modulecode, "CRM/Report_SchemeMembershipDeferredP/Report_SchemeMembershipDeferredP_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(121016, "Unclaimed Benefits Schedule", parentid, modulecode, "CRM/Report_SchemeMembershipUnclaimed/Report_SchemeMembershipUnclaimed_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(121017, "Scheme Statements", parentid, modulecode, "CRM/Report_SchemeStatement/Report_Employer_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(121018, "Basic Salaries", parentid, modulecode, "CRM/Report_SchemeMembershipSalary/Report_SchemeMembership_Tab", posi += 1, Leaf, "description", tree);//L

                adcon.add_Module_oracle(121019, "Member's Bio Data", parentid, modulecode, "CRM/Report_SchemeMembershipList/Report_SchemeMembership_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(121020, "Memeber's Beneficiaries/Next of Kin", parentid, modulecode, "CRM/Report_SchemeMembershipListBene/Report_SchemeMembership_Tab", posi += 1, Leaf, "description", tree);//L

                adcon.add_Module_oracle(121021, "Monthly Schedule,Paid", parentid, modulecode, "CRM/Report_Monthly_Schedule_SchemePaid/Report_MSS_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(121022, "Monthly Schedule,Arrears", parentid, modulecode, "CRM/Report_Monthly_Schedule_SchemeArrears/Report_MSS_Tab", posi += 1, Leaf, "description", tree);//L



                //parentid = 110207;  //for sms
                //posi = 11020701;
                //adcon.add_Module_oracle(11020701, "Send SMS-Employer", parentid, modulecode, "CRM/EmployeeSMSStatement/AddEmployeeSMSStatementTab", posi += 1, Leaf, "description", tree);//L
                //adcon.add_Module_oracle(11020702, "Send SMS-Scheme", parentid, modulecode, "CRM/EmployeeSMSStatement_SCHEME/AddEmployeeSMSStatement_SCHEMETab", posi += 1, Leaf, "description", tree);//L


            }
            catch (Exception ex)
            {
                string sss = ex.ToString();
                throw ex;
            }


        }


    } //end class
}

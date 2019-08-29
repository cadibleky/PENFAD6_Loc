using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using PenFad6Starter.DbContext;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.SqlClient;

namespace PenFad6Starter.Modules
{
    public class Setup
    {

        // system security code is 100 

        addController adcon = new addController();


        private void add_Module_sql(int p_ModuleId, string p_ModuleName, int p_ParentId, int p_ModuleCode, string p_url, int p_Position, string p_NodeLeaf, string p_Desc, string p_NavType)
        {
            //TransactionOptions tsOp = new TransactionOptions();
            //tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            //TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);

            var app = new AppSettings();
            try
            {
                using (SqlConnection conp = new SqlConnection(app.conStringSQL()))
                {
                    conp.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "ad_controller";
                        cmd.Parameters.Add("@ModuleId", SqlDbType.Decimal).Value = p_ModuleId;
                        cmd.Parameters.Add("@ModuleName", SqlDbType.NVarChar).Value = p_ModuleName;
                        cmd.Parameters.Add("@ParentId", SqlDbType.Decimal).Value = p_ParentId;
                        cmd.Parameters.Add("@Url", SqlDbType.NVarChar).Value = p_url;

                        cmd.Parameters.Add("@ModuleCode", SqlDbType.Decimal).Value = p_ModuleCode;
                        cmd.Parameters.Add("@LeafNode", SqlDbType.VarChar).Value = p_NodeLeaf;
                        //cmd.Parameters.Add("p_Desc", OracleDbType.Varchar2).Value = p_Desc;
                        //cmd.Parameters.Add("p_NavType", OracleDbType.Varchar2).Value = p_NavType;
                        cmd.ExecuteNonQuery();
                        //conp.Close();
                    }
                }

                // ts.Complete();
            }
            catch (Exception ex)
            {
                string sss = ex.ToString();
                throw ex;
            }
            finally
            {
                //if (conp.State==ConnectionState.Open)
                //{
                //    conp.Close();
                //}

                // ts.Dispose();
            }

        }


        public void add_Setup_Modules()
        {
            try
            {
                string tree = "T";
                string view = "V";
                int parentid = 0;
                int modulecode = 101;
                int posi = 0;
                //foldername/controllername/action

                //################################### scheme   ####################################
                //Scheme Setup -Parent level : starts with = 10001 -----0-----101
                parentid = 0;
                posi = 10101;
                adcon.add_Module_oracle(10101, "Scheme Setup", parentid, modulecode, "url", posi, "N", "description", tree);//N
                                                                                                     //Users - node level 1: start with : 1000101----10001----100
                parentid = 10101;
                posi = 10101;
                adcon.add_Module_oracle(1010101, "Create New Schemes", parentid, modulecode, "Setup/pfm_Scheme/AddSchemeTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010102, "Edit Schemes", parentid, modulecode, "Setup/pfm_Scheme/AddEditSchemeTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010103, "Approve Scheme", parentid, modulecode, "Setup/pfm_Scheme/AddApproveSchemeTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010104, "View Schemes", parentid, modulecode, "Setup/pfm_Scheme/AddViewSchemeTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010105, "Scheme Funds", parentid, modulecode, "Setup/pfm_Scheme_Fund/AddSchemeFundTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010106, "Scheme Fund Managers", parentid, modulecode, "Setup/pfm_Scheme_Fund_Manager/AddSchemeFundManagerTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010107, "Scheme Fee", parentid, modulecode, "Setup/pfm_SchemeFee/AddSchemeFeeTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010108, "Fund Manager Fee", parentid, modulecode, "Setup/pfm_SchemeFMFee/AddSchemeFMFeeTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010109, "Reassign Scheme Custodian", parentid, modulecode, "Setup/pfm_Scheme/AddReassignCustodianSchemeTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010110, "Scheme Fund Princing", parentid, modulecode, "Setup/pfm_Scheme_Fund/AddSchemeFundNavUnitPricingTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010111, "Vesting Rule", parentid, modulecode, "Setup/pfm_Vesting_Rule/AddVestingRuleTab", posi += 1, "L", "description", tree);//L

                ////################################### system   ####################################
                ////system Setup -Parent level : starts with = 10001 -----0-----101
                parentid = 0;
                posi = 10102;
                adcon.add_Module_oracle(10102, "System Setup", parentid, modulecode, "url", posi, "N", "description", tree);//N
                                                                                                                        //Users - node level 1: start with : 1000101----10001----100
                parentid = 10102;
                posi = 10102;
                adcon.add_Module_oracle(1010201, "Custodian", parentid, modulecode, "Setup/pfm_Custodian/AddCustodianTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010202, "Fees", parentid, modulecode, "Setup/pfm_Fees/AddFeesTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010203, "Funds", parentid, modulecode, "Setup/pfm_Fund/AddFundTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010204, "Fund Managers", parentid, modulecode, "Setup/pfm_FundManager/AddFundManagerTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010205, "Issuer", parentid, modulecode, "Setup/setup_Issuer/AddIssuerTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010206, "Sector", parentid, modulecode, "Setup/setup_Sector/AddSectorTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010207, "Banks", parentid, modulecode, "Setup/setup_Bank/AddBankTab", posi += 1, "L", "description", tree);//L

                ////################################### Investment   ####################################
                ////system Setup -Parent level : starts with = 10001 -----0-----101
                parentid = 0;
                posi = 10104;
                adcon.add_Module_oracle(10104, "Investment Setup", parentid, modulecode, "url", posi, "N", "description", tree);//N
                                                                                                                                        //Users - node level 1: start with : 1000101----10001----100
                parentid = 10104;
                posi = 10104;
                adcon.add_Module_oracle(1010401, "NPRA Asset Class", parentid, modulecode, "Setup/Invest_NPRAAsset/AddNPRAAssetTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010402, "Products", parentid, modulecode, "Setup/Invest_Product/AddProductTab", posi += 1, "L", "description", tree);//L
                 // adcon.add_Module_oracle(1010403, "Authorizers", parentid, modulecode, "Setup/Invest_Authorizers/AddAuthorizersTab", posi += 1, "L", "description", tree);//L


                //###################################    ####################################
                //Employee setup -Parent level : starts with = 10002 -----0-----101
                parentid = 0;
                posi = 10103;
                adcon.add_Module_oracle(10103, "CRM Setup", parentid, modulecode, "url", posi, "N", "description", tree);//N
                 
                parentid = 10103;
                posi = 10103;
                adcon.add_Module_oracle(1010301, "Gender", parentid, modulecode, "Setup/setup_Gender/AddGenderTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010302, "Identity Type", parentid, modulecode, "Setup/setup_IdentityType/AddIdentityTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010303, "Marital Status", parentid, modulecode, "Setup/setup_MaritalStatus/AddMaritalStatusTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010304, "Region/State", parentid, modulecode, "Setup/setup_Region/AddRegionTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010305, "Relationship", parentid, modulecode, "Setup/setup_Relationship/AddRelationshipTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010306, "Title", parentid, modulecode, "Setup/setup_Title/AddTitleTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010307, "Country", parentid, modulecode, "Setup/setup_Country/AddCountryTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010308, "District", parentid, modulecode, "Setup/setup_District/AddDistrictTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010309, "Employee Type", parentid, modulecode, "Setup/setup_EmployeeType/AddEmployeeTypeTab", posi += 1, "L", "description", tree);//L


                parentid = 0;
                posi = 10105;
                adcon.add_Module_oracle(10105, "GL Account Setup", parentid, modulecode, "url", posi, "N", "description", tree);//N

                parentid = 10105;
                posi = 10105;
                adcon.add_Module_oracle(1010502, "Create GL Bank Account", parentid, modulecode, "GL/GLChart/AddGLAccountBankTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010501, "Configure Collection Bank Account", parentid, modulecode, "GL/GLChart/AddGLAccountBankDefaultTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010503, "Create GL Expense Account", parentid, modulecode, "GL/GLChart/AddGLAccountExpenseTab", posi += 1, "L", "description", tree);//L


                //Migration setup -Parent level : starts with = 10002 -----0-----101
                parentid = 0;
                posi = 10106;
                adcon.add_Module_oracle(10106, "Staff Management", parentid, modulecode, "url", posi, "N", "description", tree);//N

                parentid = 10106;
                posi = 10106;
                adcon.add_Module_oracle(1010601, "Add/Edit Staff", parentid, modulecode, "Setup/setup_Staff/AddStaffTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010602, "Drop Satff", parentid, modulecode, "Setup/setup_Staff/DropStaffTab", posi += 1, "L", "description", tree);//L

                parentid = 0;
                posi = 101066;
                adcon.add_Module_oracle(101066, " Setup Company", parentid, modulecode, "url", posi, "N", "description", tree);//N

                parentid = 101066;
                posi = 101066;
                adcon.add_Module_oracle(10106601, "Update Company", parentid, modulecode, "Setup/setup_Company/AddCompanyTab", posi += 1, "L", "description", tree);//L
               

                //###################################    ####################################
                //Migration setup -Parent level : starts with = 10002 -----0-----101
                parentid = 0;
                posi = 10107;
                adcon.add_Module_oracle(10107, "Migration", parentid, modulecode, "url", posi, "N", "description", tree);//N

                parentid = 10107;
                posi = 10107;
                adcon.add_Module_oracle(1010701, "Migrate Employer/Accounts", parentid, modulecode, "CRM/EmployerBatchUpload/AddEmployerBatchTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010702, "Migrate Employee Contributions", parentid, modulecode, "Remittance/Remit_Con_Migrate/AddConMigrationTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010703, "Migrate Employer Contributions", parentid, modulecode, "Remittance/Remit_Con_Employer_Migrate/AddConEmployerMigrationTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010704, "Migrate Transfer In Contributions", parentid, modulecode, "Remittance/Remit_Con_Sup_Migrate/AddConSupMigrationTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010705, "Migrate Back Pay Employee", parentid, modulecode, "Remittance/Remit_Con_Sup_Migrate/AddConBPayMigrationTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010706, "Migrate Back Pay Employer", parentid, modulecode, "Remittance/Remit_Con_Employer_Migrate/AddBPEmployerMigrationTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010707, "Migrate Units", parentid, modulecode, "Remittance/Remit_Unit_Migrate/AddUnitMigrationTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010708, "Delete Migrated Remittance", parentid, modulecode, "Remittance/DeleteRemit_Contribution/AddDeleteContributionBatchTab", posi += 1, "L","description", tree);//L
                adcon.add_Module_oracle(1010711, "Migrated Raw Employee", parentid, modulecode, "Remittance/Remit_Con_MigrateRaw/AddEmpMigrationRawTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(1010712, "Migrated Raw Remittance", parentid, modulecode, "Remittance/Remit_Con_MigrateRaw/AddConMigrationRawTab", posi += 1, "L", "description", tree);//L

                parentid = 10107;
                posi = 10107;
                adcon.add_Module_oracle(1010709, "GL Initial Operations", parentid, modulecode, "url", posi, "N", "description", tree);//N

                parentid = 1010709;
                posi = 1010709;
                adcon.add_Module_oracle(101070901, "New GL Balance", parentid, modulecode, "GL/GLInitial/AddGLInitialTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(101070902, "Approve GL Balance", parentid, modulecode, "GL/GLInitial/AddGLInitialApproveTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(101070903, "Reverse GL Balance", parentid, modulecode, "GL/GLInitial/AddGLInitialReversalTab", posi += 1, "L", "description", tree);//L

                parentid = 10107;
                posi = 10107;
                adcon.add_Module_oracle(1010710, "Portfolio", parentid, modulecode, "url", posi, "N", "description", tree);//N

                parentid = 1010710;
                posi = 1010710;
                adcon.add_Module_oracle(101070101, "Money Market/T-Bill/Bonds", parentid, modulecode, "Investment/MigratePortfolio/AddMigrateMTBTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(101070102, "Equity/CIS", parentid, modulecode, "Investment/MigratePortfolio/AddMigrateECTab", posi += 1, "L", "description", tree);//L


               
            }
            catch (Exception ex)
            {
                string sss = ex.ToString();
                throw ex;
            }


        }

         public void add_Utilities_Modules()
        {
            try
            {
                string tree = "T";
                string view = "V";
                int parentid = 0;
                int modulecode = 15;
                int posi = 0;
                //foldername/controllername/action

                //################################### scheme   ####################################
                //Scheme Setup -Parent level : starts with = 10001 -----0-----101
                parentid = 0;
                posi = 1501;
                adcon.add_Module_oracle(1501, "End Of Day", parentid, modulecode, "url", posi, "N", "description", tree);//N
                                                                                                     //Users - node level 1: start with : 1000101----10001----100
                parentid = 1501;
                posi = 1501;
                adcon.add_Module_oracle(150101, "Run By Scheme", parentid, modulecode, "Setup/EndOfDay/AddEODTab", posi += 1, "L", "description", tree);//L
                //adcon.add_Module_oracle(150102, "Reverse EOD", parentid, modulecode, "Setup/EndOfDayReversal/AddEOD_ReversalTab", posi += 1, "L", "description", tree);//L
                                                                                                                                                        // adcon.add_Module_oracle(150102, "Run All Schemes", parentid, modulecode, "Setup/EndOfDay/AddEODAllTab", posi += 1, "L", "description", tree);//L

                //parentid = 0;
                //posi = 1502;
                //adcon.add_Module_oracle(1502, "End Of Month", parentid, modulecode, "url", posi, "N", "description", tree);//N
                                                                                                                         //Users - node level 1: start with : 1000101----10001----100
                //parentid = 1502;
                //posi = 1502;
                //adcon.add_Module_oracle(150201, "Run By Scheme", parentid, modulecode, "Setup/EndOfMonth/AddEOMTab", posi += 1, "L", "description", tree);//L
                //                                                                                                                                        // adcon.add_Module_oracle(150102, "Run All Schemes", parentid, modulecode, "Setup/EndOfDay/AddEODAllTab", posi += 1, "L", "description", tree);//L

                parentid = 0;
                posi = 1503;
                adcon.add_Module_oracle(1503, "End Of Year", parentid, modulecode, "url", posi, "N", "description", tree);//N
                                                                                                                           //Users - node level 1: start with : 1000101----10001----100
                parentid = 1503;
                posi = 1503;
                adcon.add_Module_oracle(150301, "Run By Scheme", parentid, modulecode, "Setup/EndOfYear/AddEOYTab", posi += 1, "L", "description", tree);//L

                parentid = 0;
                posi = 1504;
                adcon.add_Module_oracle(1504, "Others", parentid, modulecode, "url", posi, "N", "description", tree);//N
                                                                                                                          //Users - node level 1: start with : 1000101----10001----100
                parentid = 1504;
                posi = 1504;
                adcon.add_Module_oracle(150401, "Upload Current Equities Unit Prices", parentid, modulecode, "Investment/LoadUnitPrice/AddLoadUnitPriceTab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(150402, "Back Date Scheme Date", parentid, modulecode, "Setup/EndOfDayBack/AddEODBackTab", posi += 1, "L", "description", tree);//L

            }
            catch (Exception ex)
            {
                string sss = ex.ToString();
                throw ex;
            }


        }

    }
}

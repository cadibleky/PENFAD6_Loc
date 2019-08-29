using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PenFad6Starter.DbContext;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace PenFad6Starter.Modules
{
   public class CRM_11
    {
        addController adcon = new addController();

        private void add_Module(int p_ModuleId, string p_ModuleName, int p_ParentId, string p_ModuleCode, string p_url, int p_Position, string p_NodeLeaf, string p_Desc, string p_NavType)
        {
            //TransactionOptions tsOp = new TransactionOptions();
            //tsOp.IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
            //TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, tsOp);

            var app = new AppSettings();
            try
            {
                using (OracleConnection conp = new OracleConnection(app.conPrime()))
                {
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "add_sec_ControllerMajor";
                        cmd.Parameters.Add("p_ModuleId", OracleDbType.Varchar2).Value = p_ModuleId;
                        cmd.Parameters.Add("p_ModuleName", OracleDbType.Varchar2).Value = p_ModuleName;
                        cmd.Parameters.Add("p_ParentId", OracleDbType.Varchar2).Value = p_ParentId;
                        cmd.Parameters.Add("p_ModuleCode", OracleDbType.Varchar2).Value = p_ModuleCode;
                        cmd.Parameters.Add("p_url", OracleDbType.Varchar2).Value = p_url;
                        cmd.Parameters.Add("p_Position", OracleDbType.Varchar2).Value = p_Position;
                        cmd.Parameters.Add("p_NodeLeaf", OracleDbType.Varchar2).Value = p_NodeLeaf;
                        cmd.Parameters.Add("p_Desc", OracleDbType.Varchar2).Value = p_Desc;
                        cmd.Parameters.Add("p_NavType", OracleDbType.Varchar2).Value = p_NavType;
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
              
            }

        }

        public void add_CRM_Modules()
        {
            try
            {
                string tree = "T";
                string view = "V";
                int parentid = 0;
                int modulecode = 11;
                int posi = 0;
                string Node = "N";
                string Leaf = "L";
                //foldername/controllername/action

                //################################### Employee   ####################################
                //Scheme Setup -Parent level : starts with = 1101 -----0-----11
                parentid = 0;
                posi = 1101;
                adcon.add_Module_oracle(1101, "Employee Management", parentid, modulecode, "url", posi, Node, "description", tree);//N
                 
                parentid = 1101;  //----level 2
                posi = 1101;
                adcon.add_Module_oracle(110101, "New Employee", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                adcon.add_Module_oracle(110102, "Edit Employee", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N               
                adcon.add_Module_oracle(110103, "Approve Employee", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                adcon.add_Module_oracle(110104, "Employee Scheme-Fund", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                adcon.add_Module_oracle(110105, "Employee Enquiry", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                adcon.add_Module_oracle(110106, "Send Information", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                adcon.add_Module_oracle(110107, "Delete Employee", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                adcon.add_Module_oracle(110150, "Employee Reports/Certificate", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N


                parentid = 110101; // for new employee
                posi = 110101;
                adcon.add_Module_oracle(11010101, "Individual Employee", parentid, modulecode, "CRM/AddNewEmployee/AddEmployeeTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11010102, "Add Beneficiary/Next-Of-Kin", parentid, modulecode, "CRM/crm_BeneNext/AddBeneNextTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11010103, "Batch Upload-Employee", parentid, modulecode, "CRM/EmployeeBatchUpload/AddEmployeeBatchTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11010104, "Batch Upload-Beneficiary/NoK", parentid, modulecode, "CRM/BeneficiaryBatchUpload/AddBeneBatchTab", posi += 1, Leaf, "description", tree);//L

                parentid = 110102; // for edit employee
                posi = 110102;
                adcon.add_Module_oracle(11010201, "Employee Info", parentid, modulecode, "CRM/UpdateEmployee/AddUpdateEmployeeTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11010202, "Beneficiary/Next-Of-Kin", parentid, modulecode, "CRM/crm_BeneNext/AddBeneNextEditTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11010203, "Batch Employee Edit", parentid, modulecode, "CRM/EmployeeBatchEdit/AddEmployeeBatchEditTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11010204, "Change Employee Status", parentid, modulecode, "CRM/UpdateEmployee/AddTransferEmployeeTab", posi += 1, Leaf, "description", tree);//L
                //adcon.add_Module_oracle(11010205, "Activate Employee", parentid, modulecode, "CRM/UpdateEmployee/AddActivateEmployeeTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11010206, "Change Employer", parentid, modulecode, "CRM/Employee_Scheme_Funds/AddChangeESFTab_Employer", posi += 1, Leaf, "description", tree);//L

                parentid = 110103; // for approve employee
                posi = 110103;
                adcon.add_Module_oracle(11010301, "New Individual Employee", parentid, modulecode, "CRM/ApprovePendingEmployee/ApprovePendingEmployeeTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11010302, "New Batch Employee", parentid, modulecode, "CRM/ApproveNewEmployeeBatchUpload/AddEmployeeBatchUploadApprovalTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11010303, "Edited Employee Info.", parentid, modulecode, "CRM/ApproveEditingEmployee/ApproveEditingEmployeeTab", posi += 1, Leaf, "description", tree);//L

                parentid = 110104; // for  employee sheme
                posi = 110104;
                adcon.add_Module_oracle(11010401, "New Employee Scheme-Fund", parentid, modulecode, "CRM/Employee_Scheme_Funds/AddESFTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11010402, "Approve Employee Scheme-Fund", parentid, modulecode, "CRM/Employee_Scheme_Funds/AddESFApproveTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11010403, "View Employee Scheme-Fund.", parentid, modulecode, "CRM/Employee_Scheme_Funds/AddESFViewTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11010404, "Close Employee Scheme Fund.", parentid, modulecode, "CRM/Employee_Scheme_Funds/AddESFCloseTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11010405, "Change Employee Scheme-Fund", parentid, modulecode, "CRM/Employee_Scheme_Funds/AddChangeESFTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11010406, "Activate Employee Scheme-Fund", parentid, modulecode, "CRM/Employee_Scheme_Funds/AddESFActivateTab", posi += 1, Leaf, "description", tree);//L


                parentid = 110105; // for  employee enquiry
                posi = 110105;
                adcon.add_Module_oracle(11010501, "View Employee", parentid, modulecode, "CRM/crm_ViewEmployees/AddViewEmployeeTab", posi += 1, Leaf, "description", tree);//L


                parentid = 110150; // for  employee reports
                posi = 110150;
                adcon.add_Module_oracle(11015001, "Employee Certificate", parentid, modulecode, "CRM/Report_EmployeeCertificate/Report_EmployeeCertificate_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11015002, "Employee Statement", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                adcon.add_Module_oracle(11015004, "Active Employee Accounts", parentid, modulecode, "CRM/Report_EmployeeActive/Report_EmployeeActive_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11015005, "Retirement List", parentid, modulecode, "CRM/Report_EmployeeRetirement/Report_EmployeeRetirement_Tab", posi += 1, Leaf, "description", tree);//L

                parentid = 11015002; // for  employee reports
                posi = 11015002;
                adcon.add_Module_oracle(1101500201, "General", parentid, modulecode, "CRM/Report_EmployeeStatement/Report_Employee_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(1101500202, "Specific", parentid, modulecode, "CRM/Report_EmployeeStatementSpecific/Report_Employee_Tab", posi += 1, Leaf, "description", tree);//L


                parentid = 110106;  // for employee----level 3
                posi = 11010601;
                adcon.add_Module_oracle(11010601, "Send Login Credentials", parentid, modulecode, "CRM/EmployeePassword/AddEmployeePasswordIndiTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11010602, "Send Statements.", parentid, modulecode, "CRM/EmployeeEmailStatement/AddEmployeeEmailStatementIndiTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11010603, "Download Statements.", parentid, modulecode, "CRM/EmployeeDownloadStatement/AddEmployeeDownloadStatementIndiTab", posi += 1, Leaf, "description", tree);//L

                parentid = 110107;  // for employee----level 3
                posi = 11010701;
                adcon.add_Module_oracle(11010701, "Individual Employee", parentid, modulecode, "CRM/UpdateEmployee/AddDeleteEmployeeTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11010702, "Batch Employees.", parentid, modulecode, "CRM/UpdateEmployee/AddDeleteBatchEmployeeTab", posi += 1, Leaf, "description", tree);//L


                //################################### Employer   ####################################
                //Scheme Setup -Parent level : starts with = 1101 -----0-----11
                parentid = 0;
                posi = 1102;
                adcon.add_Module_oracle(1102, "Employer Management", parentid, modulecode, "url", posi, Node, "description", tree);//N

                parentid = 1102;  //----level 2
                posi = 110201;
                adcon.add_Module_oracle(110201, "Create/Edit Employer", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                adcon.add_Module_oracle(110202, "Approve Employer", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                adcon.add_Module_oracle(110203, "Employer Enquiry", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                adcon.add_Module_oracle(110204, "Employer Scheme Account", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                adcon.add_Module_oracle(110205, "Send Information", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                adcon.add_Module_oracle(110206, "Delete Employer", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                //adcon.add_Module_oracle(110207, "Send SMS", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N
                adcon.add_Module_oracle(110250, "Employer Reports", parentid, modulecode, "url", posi += 1, Node, "description", tree);//N


                parentid = 110201;  //for Create/Edit Employer----level 3
                posi = 11020101;
                adcon.add_Module_oracle(11020101, "New Employer", parentid, modulecode, "CRM/Employer/AddNewEmployerTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11020102, "Edit Employer", parentid, modulecode, "CRM/Employer/AddEditEmployerTab", posi += 1, Leaf, "description", tree);//L
				adcon.add_Module_oracle(11020103, "Edit Employer Status", parentid, modulecode, "CRM/Employer/AddEditEmployerStatusTab", posi += 1, Leaf, "description", tree);//L


				parentid = 110202;  // for Approve Employer----level 3
                posi = 11020201;
                adcon.add_Module_oracle(11020201, "Approve New Employer", parentid, modulecode, "CRM/ApprovedEmployer/AddApproveNewEmployerTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11020202, "Approve Edited Employer Info.", parentid, modulecode, "CRM/ApprovedEmployer/AddApproveEditedEmployerTab", posi += 1, Leaf, "description", tree);//L


                parentid = 110203;  // for Employer Enquiry----level 3
                posi = 11020301;
                adcon.add_Module_oracle(11020301, "View Employer Info.", parentid, modulecode, "CRM/Employer/AddEmployerEnquiryTab", posi += 1, Leaf, "description", tree);//L
                //adcon.add_Module_oracle(11020302, "Employer Scheme", parentid, modulecode, "CRM/Employer/AddEmployerEnquiryTab", posi += 1, Leaf, "description", tree);//L
                                                                                                                                                                                //adcon.add_Module_oracle(11020303, "New Employer Statement", parentid, modulecode, "CRM/Employer_Schemes/AddESTab", posi += 1, Leaf, "description", tree);//L

                parentid = 110204;  // for employer scheme accounts----level 3
                posi = 11020401;
                adcon.add_Module_oracle(11020401, "Create Scheme Account", parentid, modulecode, "CRM/Employer_Schemes/AddESTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11020402, "Approve Scheme Account.", parentid, modulecode, "CRM/Employer_Schemes/AddESApproveTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11020403, "View Scheme Account", parentid, modulecode, "CRM/Employer_Schemes/AddESViewTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11020404, "Close Scheme Account", parentid, modulecode, "CRM/Employer_Schemes/AddESCloseTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11020405, "Reactivate Scheme Account", parentid, modulecode, "CRM/Employer_Schemes/AddESActivateTab", posi += 1, Leaf, "description", tree);//L

                parentid = 110205;  // for employer scheme accounts----level 3
                posi = 11020501;
                adcon.add_Module_oracle(11020501, "Send Batch Login Credentials", parentid, modulecode, "CRM/EmployeePassword/AddEmployeePasswordTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11020502, "Send Batch Statements.", parentid, modulecode, "CRM/EmployeeEmailStatement/AddEmployeeEmailStatementTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11020503, "Download Batch Statements(All).", parentid, modulecode, "CRM/EmployeeDownloadStatement/AddEmployeeDownloadStatementTab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11020504, "Download Batch Statements(Active).", parentid, modulecode, "CRM/EmployeeDownloadStatement/AddEmployeeDownloadStatementActiveTab", posi += 1, Leaf, "description", tree);//L

                parentid = 110206;  //for delete Employer----level 3
                posi = 11020601;
                adcon.add_Module_oracle(11020601, "Delete Employer", parentid, modulecode, "CRM/Employer/AddDeleteEmployerTab", posi += 1, Leaf, "description", tree);//L

                
                parentid = 110250;  // for employer reports----level 3
                posi = 110250;
                adcon.add_Module_oracle(11025001, "Employer Statements", parentid, modulecode, "CRM/Report_EmployerStatement/Report_Employer_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11025002, "Active Employer Accounts", parentid, modulecode, "CRM/Report_EmployerActive/Load_Employer_Report_IFrame", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11025003, "Monthly Schedule", parentid, modulecode, "CRM/Report_Monthly_Schedule/Report_MS_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11025004, "Other Reports", parentid, modulecode, "CRM/Report_EmployerBen/Report_EmployerBen_Tab", posi += 1, "L", "description", tree);//L
                adcon.add_Module_oracle(11025005, "Employees", parentid, modulecode, "CRM/Report_EmployerMembers/Report_Employer_Tab", posi += 1, Leaf, "description", tree);//L
                adcon.add_Module_oracle(11025006, "Employee's Beneficiaries/Next of Kins", parentid, modulecode, "CRM/Report_EmployerMembersBen/Report_Employer_Tab", posi += 1, Leaf, "description", tree);//L



            }
            catch (Exception ex)
            {
                string sss = ex.ToString();
                throw ex;
            }


        }





  


    }
}

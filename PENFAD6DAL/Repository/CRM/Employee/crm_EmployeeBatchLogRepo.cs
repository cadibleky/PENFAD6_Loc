using Dapper;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PENFAD6DAL.Repository.CRM.Employee
{
  public  class crm_EmployeeBatchLogRepo
    {
        public string Batch_No { get; set; }
        public string Batch_Status { get; set; }
        public string Batch_Description { get; set; }
        public string Employer_name { get; set; }
        public string Employer_Id { get; set; }
        public string MakerId { get; set; }
        public DateTime  MakeDate { get; set; }
        public string Scheme_Id { get; set; }
		public string Scheme_Fund_Id { get; set; }
		public string Scheme_Name { get; set; }
        public string Fund_Id { get; set; }
        public string Fund_Name { get; set; }
        public string Mobile_Number { get; set; }
        public string First_Name { get; set; }
        public string SEND_SMS { get; set; }

		public string ES_Id { get; set; }
		public string New_Trustee { get; set; }
		public DateTime? Pay_Date_Benefit { get; set; }

		public string GL_Account_No { get; set; }
		public decimal Unit_Price { get; set; }



		IDbConnection con;
        public List<crm_EmployeeBatchLogRepo> GetBatch_EmployeeList_ByStatus(string batch_status)
        {
            var batch_list = new List<crm_EmployeeBatchLogRepo>();
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sel_crm_EmployeeBatchLog";
                        cmd.Parameters.Add("p_status", OracleDbType.Varchar2, ParameterDirection.Input).Value = batch_status;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            batch_list = dt.AsEnumerable().Select(row => new crm_EmployeeBatchLogRepo
                            {
                                 Employer_name = row.Field<string>("EMPLOYER_NAME"),
                                 Employer_Id = row.Field<string>("EMPLOYER_ID"),
                                 Scheme_Name = row.Field<string>("SCHEME_NAME"),
                                 Batch_Status = row.Field<string>("BATCHSTATUS"),
                                 Batch_No = row.Field<string>("BATCHNO"),
                                 MakerId = row.Field<string>("MAKERID"),
                                 MakeDate = row.Field<DateTime>("MAKEDATE")
                            }).ToList();

                        }
                    }

                    return batch_list;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conp.State == ConnectionState.Open)
                    {
                        conp.Close();
                        conp.Dispose();
                    }
                }
            }

        }

        public List<crm_EmployeeBatchLogRepo> GetBatch_EmployeeList_ByStatus_Delete(string Employer_Id)
        {
            var batch_list = new List<crm_EmployeeBatchLogRepo>();
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SEL_CRM_EMPLOYEEBATCHLOG_DEL";
                        cmd.Parameters.Add("p_employerid", OracleDbType.Varchar2, ParameterDirection.Input).Value = Employer_Id;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            batch_list = dt.AsEnumerable().Select(row => new crm_EmployeeBatchLogRepo
                            {
                                Employer_name = row.Field<string>("EMPLOYER_NAME"),
                                Employer_Id = row.Field<string>("EMPLOYER_ID"),
                                Scheme_Name = row.Field<string>("SCHEME_NAME"),
                                Batch_Status = row.Field<string>("BATCHSTATUS"),
                                Batch_No = row.Field<string>("BATCHNO"),
                                MakerId = row.Field<string>("MAKERID"),
                                MakeDate = row.Field<DateTime>("MAKEDATE")
                            }).ToList();

                        }
                    }

                    return batch_list;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conp.State == ConnectionState.Open)
                    {
                        conp.Close();
                        conp.Dispose();
                    }
                }
            }

        }


        public IEnumerable GetEmployerSchemeByEmployerId(string employer_id)
        {
            //var role_list = new List<sec_UserGroupRepo>();
            List<object> data = new List<object>();
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    if (string.IsNullOrEmpty(employer_id))
                        employer_id = "0";
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SEL_REMIT_EMPLOYER_SCHEME_BYID";
                        cmd.Parameters.Add("p_employer_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = employer_id;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            foreach (DataRow roww in dt.Rows)
                            {
                                string id = roww["SCHEME_ID"].ToString(); // cityNode.SelectSingleNode("id").InnerText;
                                string name = roww["SCHEME_NAME"].ToString();  //cityNode.SelectSingleNode("name").InnerText;
                                data.Add(new { Id = id, Name = name });
                            }

                        }
                    }

                    return data;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conp.State == ConnectionState.Open)
                    {
                        conp.Close();
                        conp.Dispose();
                    }
                }
            }

        }

        //Get Employer Scheme Account By Employer Id
        public List<crm_EmployeeBatchLogRepo> Get_Crm_Employer_SchemeByStatus(string Es_status)
        {
            string status = "ACTIVE";
            Es_status = status;

            var batch_list = new List<crm_EmployeeBatchLogRepo>();
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SEL_CRM_EMPLOYER_SCHEMEBYEMPID";
                        cmd.Parameters.Add("p_Status", OracleDbType.Varchar2, ParameterDirection.Input).Value = Es_status;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            batch_list = dt.AsEnumerable().Select(row => new crm_EmployeeBatchLogRepo
                            {
                                Employer_name = row.Field<string>("Employer_Name"),
                                Employer_Id = row.Field<string>("EMPLOYER_ID"),
                                Scheme_Name = row.Field<string>("SCHEME_NAME"),
                                Scheme_Id = row.Field<string>("SCHEME_ID"),
                               //// ES_Id = row.Field<string>("EMPLOYER_ACCOUNT_NO"),
                                ////=Convert.ToDateTime(row.Field<DateTime>("")),
                               ///DeadLine_Date = Convert.ToDateTime(row["NEXT_DEADLINE_DATE"]),


                            }).ToList();

                        }
                    }

                    return batch_list;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conp.State == ConnectionState.Open)
                    {
                        conp.Close();
                        conp.Dispose();
                    }
                }
            }

        }
        ///Delete Batch
        public bool Delete_BatchRecord(string batch_no)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                //Input Param
                param.Add(name: "p_batchNo", value: batch_no, dbType: DbType.String, direction: ParameterDirection.Input);

                int result = con.Execute(sql: "DEL_CRM_EMPLOYEE_BATCH", param: param, commandType: CommandType.StoredProcedure);
                if (result > 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    if (con != null) { con = null; }
                }
            }

        }

        public void Delete_BatchRecord_All(string batch_no)
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                var param = new DynamicParameters();

                //Input Param
                param.Add(name: "p_batchNo", value: batch_no, dbType: DbType.String, direction: ParameterDirection.Input);

               con.Execute(sql: "DEL_CRM_EMPLOYEE_BATCH_DEL", param: param, commandType: CommandType.StoredProcedure);
                //if (result > 0)
                //    return false;
                //else
                //    return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    if (con != null) { con = null; }
                }
            }

        }

		public List<crm_EmployeeBatchLogRepo> GetBatch_EmployeeList_ByStatus_portout(string Employer_Id)
		{
			var batch_list = new List<crm_EmployeeBatchLogRepo>();
			var app = new AppSettings();
			using (OracleConnection conp = new OracleConnection(app.conString()))
			{
				try
				{
					//Get connection
					conp.Open();
					using (OracleCommand cmd = new OracleCommand())
					{
						cmd.Connection = conp;
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.CommandText = "SEL_CRM_PORTOUTBATCHLOG";
						cmd.Parameters.Add("p_employerid", OracleDbType.Varchar2, ParameterDirection.Input).Value = Employer_Id;
						cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

						using (OracleDataAdapter da = new OracleDataAdapter(cmd))
						{
							DataTable dt = new DataTable();
							da.Fill(dt);
							batch_list = dt.AsEnumerable().Select(row => new crm_EmployeeBatchLogRepo
							{
								Fund_Name = row.Field<string>("Fund_Name"),
								Employer_Id = row.Field<string>("EMPLOYER_ID"),
								Scheme_Name = row.Field<string>("SCHEME_NAME"),
								Scheme_Id = row.Field<string>("SCHEME_ID"),
								Fund_Id = row.Field<string>("Fund_Id"),
								Scheme_Fund_Id = row.Field<string>("Scheme_Fund_Id"),
								Unit_Price = row.Field<decimal>("Unit_Price"),
								ES_Id = row.Field<string>("ES_ID"),
							}).ToList();

						}
					}

					return batch_list;
				}
				catch (Exception ex)
				{
					throw ex;
				}
				finally
				{
					if (conp.State == ConnectionState.Open)
					{
						conp.Close();
						conp.Dispose();
					}
				}
			}

		}

		public void Portout(crm_EmployeeBatchLogRepo repo_emplog)
		{
			
			try
			{
				//Get Connection
				AppSettings app = new AppSettings();
				con = app.GetConnection();
				DynamicParameters param = new DynamicParameters();

				param.Add(name: "P_EMPLOYERID", value: repo_emplog.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
				param.Add(name: "P_SCHEMEFUNDID", value: repo_emplog.Scheme_Fund_Id, dbType: DbType.String, direction: ParameterDirection.Input);
				param.Add(name: "P_TDATE", value: repo_emplog.Pay_Date_Benefit, dbType: DbType.DateTime, direction: ParameterDirection.Input);
				param.Add(name: "P_NEWTRUSTEE", value: repo_emplog.New_Trustee, dbType: DbType.String, direction: ParameterDirection.Input);
				param.Add(name: "P_GL_NO", value: repo_emplog.GL_Account_No, dbType: DbType.String, direction: ParameterDirection.Input);
				param.Add(name: "P_UNITPRICE", value: repo_emplog.Unit_Price, dbType: DbType.Decimal, direction: ParameterDirection.Input);
				param.Add(name: "P_MAKERID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
				param.Add(name: "P_ESID", value: repo_emplog.ES_Id, dbType: DbType.String, direction: ParameterDirection.Input);
				con.Execute("REMIT_BATCH_PORTOUT", param, commandType: CommandType.StoredProcedure);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (con.State == ConnectionState.Open)
				{
					con.Close();
					if (con != null) { con = null; }
				}
			}
		}


	}// end class


}

using Dapper;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.Repository.Setup.PfmSetup;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PENFAD6DAL.Repository.Remittance.Contribution
{
    public class Remit_BatchLogRepo
    {
        public string Con_Log_Id { get; set; }
        [Required(ErrorMessage = "Select  Employer ID is a required data item.")]
        [Display(Name = "Employer ID")]
        public string Employer_Name { get; set; }
        [Required(ErrorMessage = "Select  Employer ID is a required data item.")]
        [Display(Name = "Employer Name")]
        public string Employer_Id { get; set; }

        //public string Empoyer_Id { get; set; }
        [Required(ErrorMessage = "Select  Scheme Name is a required data item.")]
        [Display(Name = "Scheme Name")]

        public string Scheme_Name { get; set; }
        public string Scheme_id { get; set; }
        public string ES_Id { get; set; }
        [Required(ErrorMessage = "Select  Month Name is a required data item.")]
        [Display(Name = "Month Name")]
        public string For_Month { get; set; }
        [Required(ErrorMessage = "Select  Year is a required data item.")]
        [Display(Name = "Year")]
        public string For_Year { get; set; }
        public DateTime DeadLine_Date { get; set; }
        public string ISINARREAS_YesNo { get; set; }
        public string Unit_Purchased_YesNo { get; set; }
        public double Total_Employee { get; set; }
        public decimal Total_Contribution { get; set; }
        public string Log_Status { get; set; }

        public string Total_Contribute { get; set; }

        public DateTime NEXT_SURCHARGE_DATE { get; set; }

        IDbConnection conn;

        public List<Remit_BatchLogRepo> GetBatch_RemitList_ByStatus(string batch_status)
        {
            var batch_list = new List<Remit_BatchLogRepo>();
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
                        cmd.CommandText = "SEL_REMIT_CON_PENDINGAPPROVAL";
                        cmd.Parameters.Add("p_Log_status", OracleDbType.Varchar2, ParameterDirection.Input).Value = batch_status;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            batch_list = dt.AsEnumerable().Select(row => new Remit_BatchLogRepo
                            {
                                Employer_Name = row.Field<string>("EMPLOYER_NAME"),
                                Employer_Id = row.Field<string>("EMPLOYER_ID"),
                                Scheme_Name = row.Field<string>("SCHEME_NAME"),

                                For_Month = row["FOR_MONTH"].ToString(),
                                For_Year = row["FOR_YEAR"].ToString(),
                                Total_Contribution = Convert.ToDecimal(row["TOTAL_CONTRIBUTION"].ToString()),
                                Total_Contribute = row["TOTALCONTRIBUTE"].ToString(),
                                Log_Status = row.Field<string>("LOG_STATUS"),
                                ES_Id = row.Field<string>("ES_ID"),
                                Con_Log_Id = row.Field<string>("CON_LOG_ID"),
                                Scheme_id = row.Field<string>("SCHEME_ID"),


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

        public List<Remit_BatchLogRepo> GetBatch_RemitList_ForDelete(string batch_status)
        {
            var batch_list = new List<Remit_BatchLogRepo>();
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
                        cmd.CommandText = "SEL_REMIT_CON_PENDINGDELETE";
                        cmd.Parameters.Add("p_Log_status", OracleDbType.Varchar2, ParameterDirection.Input).Value = batch_status;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            batch_list = dt.AsEnumerable().Select(row => new Remit_BatchLogRepo
                            {
                                Employer_Name = row.Field<string>("EMPLOYER_NAME"),
                                Employer_Id = row.Field<string>("EMPLOYER_ID"),
                                Scheme_Name = row.Field<string>("SCHEME_NAME"),

                                For_Month = row["FOR_MONTH"].ToString(),
                                For_Year = row["FOR_YEAR"].ToString(),
                                Total_Contribution = Convert.ToDecimal(row["TOTAL_CONTRIBUTION"].ToString()),
                                Total_Contribute = row["TOTALCONTRIBUTE"].ToString(),
                                Log_Status = row.Field<string>("LOG_STATUS"),
                                ES_Id = row.Field<string>("ES_ID"),
                                Con_Log_Id = row.Field<string>("CON_LOG_ID"),
                                Scheme_id = row.Field<string>("SCHEME_ID"),


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
        public IEnumerable GetEmployerTotalConByEmployerId(string employer_id)
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
                        cmd.CommandText = "SEL_REMIT_CONLOG_TOTALPENDING";
                        cmd.Parameters.Add("p_Employer_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = employer_id;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            foreach (DataRow roww in dt.Rows)
                            {
                                string Tot = roww["TOTAL_EMPLOYEE"].ToString(); // cityNode.SelectSingleNode("id").InnerText;
                                // decimal  Con = Convert.ToDecimal(roww["SCHEME_NAME"]).ToString());  //cityNode.SelectSingleNode("name").InnerText;
                                //data.Add(new { Id = id, Name = name });
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



        public void Get_Employer_Scheme_Month_Year_For_Remitance_Upload(string Employer_Schem_Id, out decimal system_month, out decimal system_year, out DateTime N_Deadline)
        {
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {
                    system_month = 0;
                    system_year = 0;
                    N_Deadline = DateTime.Now;
                    if (string.IsNullOrEmpty(Employer_Schem_Id))
                        Employer_Schem_Id = "0";
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SEL_REMIT_CON_NEXT_MON_YEAR";
                        cmd.Parameters.Add("p_es_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = Employer_Schem_Id;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        OracleDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            system_month = Convert.ToDecimal(dr["FOR_MONTH"].ToString()); // p_result.Value.ToString();
                            system_year = Convert.ToDecimal(dr["FOR_YEAR"].ToString());
                            N_Deadline = Convert.ToDateTime(dr["DEADLINE_DATE"]);
                        }

                    }


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


        public IEnumerable<pfm_SchemeRepo> Get_Scheme_Current_Dates(string Schem_Id)
        {
            var data = new List<pfm_SchemeRepo>();
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {

                    if (string.IsNullOrEmpty(Schem_Id))
                        Schem_Id = "0";
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SEL_PFM_SCHEME_DATES";
                        cmd.Parameters.Add("p_scheme_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = Schem_Id;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            data = dt.AsEnumerable().Select(row => new pfm_SchemeRepo
                            {
                                Scheme_Id = row.Field<string>("SCHEME_ID"),
                                Next_Deadline_Date = row.Field<DateTime>("NEXT_DEADLINE_DATE"),
                                Today_Date = row.Field<DateTime>("TODAY_DATE"),
                                DeadLine_Day = row.Field<decimal>("DEADLINE_DAY"),
                                Surcharge_Grace_Period = row.Field<decimal>("SURCHARGE_GRACE_PERIOD")
                            }).ToList();
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





    }// end class


}





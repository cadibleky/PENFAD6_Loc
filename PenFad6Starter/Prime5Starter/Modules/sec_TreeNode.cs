using Dapper;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using PenFad6Starter.DbContext;

namespace PenFad6Starter.Modules
{
   public class sec_TreeNode
    {
        public  decimal ModuleId { get; set; }
        public string ModuleName { get; set; }
        public decimal ParentId { get; set; }
        public decimal ModuleCode { get; set; }
        public string Url { get; set; }
        public decimal Position { get; set; }
        public string NodeLeaf { get; set; }
        public string NavType { get; set; }
        public List<sec_TreeNode> Children { get; set; }

        







        public List<sec_TreeNode> GetModules(decimal ModuleCode)
        {
             
            var data = new List<sec_TreeNode>();
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conPrime()))
            {
                try
                {
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sel_sec_ControllerByModuleCode";
                        cmd.Parameters.Add("p_ModuleCode", OracleDbType.Decimal).Value = ModuleCode;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            data = dt.AsEnumerable().Select(row => new sec_TreeNode
                            {
                                ModuleId = row.Field<decimal>("MODULE_ID"),
                                ModuleName = row.Field<string>("MODULE_NAME"),
                                ParentId = row.Field<decimal>("PARENT_ID"),
                                ModuleCode = row.Field<decimal>("MODULE_CODE"),
                                Url = row.Field<string>("URL"),
                                //Position = row.Field<decimal>("EMPLOYEE_ID"),
                                NodeLeaf = row.Field<string>("NODE_LEAF"),
                                NavType = row.Field<string>("NAV_TYPE")
                                
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


        public List<sec_TreeNode> GetModules_new(decimal ModuleCode)
        {
            var data = new List<sec_TreeNode>();
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conPrime()))
            {
                try
                {
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sel_sec_ControllerByModuleCode";
                        cmd.Parameters.Add("p_ModuleCode", OracleDbType.Decimal).Value = ModuleCode;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            data = dt.AsEnumerable().Select(row => new sec_TreeNode
                            {
                                ModuleId = row.Field<decimal>("MODULE_ID"),
                                ModuleName = row.Field<string>("MODULE_NAME"),
                                ParentId = row.Field<decimal>("PARENT_ID"),
                                ModuleCode = row.Field<decimal>("MODULE_CODE"),
                                Url = row.Field<string>("URL"),
                                //Position = row.Field<decimal>("EMPLOYEE_ID"),
                                NodeLeaf = row.Field<string>("NODE_LEAF"),
                                NavType = row.Field<string>("NAV_TYPE")

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

        public DataTable GetModules_DataTable(decimal ModuleCode)
        {
            DataTable data = new DataTable();
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conPrime()))
            {
                try
                {
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sel_sec_ControllerByTest"; //"sel_sec_ControllerByModuleCode";
                        cmd.Parameters.Add("p_ModuleCode", OracleDbType.Decimal).Value = ModuleCode;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            data = dt;
                            //dt = dt.AsEnumerable().Select(row => new sec_TreeNode
                            //{
                            //    ModuleId = row.Field<decimal>("MODULE_ID"),
                            //    ModuleName = row.Field<string>("MODULE_NAME"),
                            //    ParentId = row.Field<decimal>("PARENT_ID"),
                            //    ModuleCode = row.Field<decimal>("MODULE_CODE"),
                            //    Url = row.Field<string>("URL"),
                            //    //Position = row.Field<decimal>("EMPLOYEE_ID"),
                            //    NodeLeaf = row.Field<string>("NODE_LEAF"),
                            //    NavType = row.Field<string>("NAV_TYPE")

                            //}).ToList();

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

        public DataSet GetModules_DataSetsql()
        {

            DataSet data = new DataSet();
            var app = new AppSettings();
            using (SqlConnection conp = new SqlConnection(app.conStringSQL()))
            {
                try
                {
                    //Get connection
                    conp.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "sel_controllers"; //"sel_sec_ControllerByModuleCode";
                        //cmd.Parameters.Add("p_ModuleCode", OracleDbType.Decimal).Value = ModuleCode;
                        //cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds,"nodeT");
                            data = ds;
                             

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









    }//end class
}

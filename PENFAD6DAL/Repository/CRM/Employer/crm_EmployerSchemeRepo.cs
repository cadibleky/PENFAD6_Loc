﻿using Dapper;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using PENFAD6DAL.GlobalObject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PENFAD6DAL.Repository.CRM.Employer
{
    public class crm_EmployerSchemeRepo
    {

        [Required(ErrorMessage = "Please select Employer")]
        public string Employer_Name { get; set; }
        public string ES_Id { get; set; }
        [Required(ErrorMessage = "Please select Scheme")]
        public string Scheme_Id { get; set; }       
        public string Scheme_Name { get; set; }     
        public string Employer_Id { get; set; }
    
        public string Registration_Number { get; set; }

        public string Office_Location { get; set; }

        public string Phone_Number { get; set; }      
        public string Auth_Status { get; set; }
        public string ES_Status { get; set; }
        public string Auth_Id { get; set; }
        public string Maker_Id { get; set; }


        IDbConnection con;
        public void SaveRecord(crm_EmployerSchemeRepo ESRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();

                param.Add(name: "P_ES_ID", value: ESRepo.Employer_Id + ESRepo.Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SCHEME_ID", value: ESRepo.Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_EMPLOYER_ID", value: ESRepo.Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_MAKER_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);

                con.Execute("MIX_REMIT_EMPLOYER_SCHEME", param, commandType: CommandType.StoredProcedure);
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


        // Approve Employer Scheme
        public void ApproveESRecord(crm_EmployerSchemeRepo ESRepo)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_ES_ID", value: ES_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_EMPLOYER_ID", value: Employer_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_SCHEME_ID", value: Scheme_Id, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_ES_STATUS", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTH_STATUS", value: "AUTHORIZED", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_AUTH_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("APP_REMIT_ES", param, commandType: CommandType.StoredProcedure);
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

        // Approve Employer Scheme
        public void CloseESRecord(string ES_ID)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_ES_ID", value: ES_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_ES_STATUS", value: "CLOSED", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("UPD_REMIT_ES", param, commandType: CommandType.StoredProcedure);
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

        public void ActivateESRecord(string ES_ID)
        {
            try
            {
                //Get Connection
                AppSettings app = new AppSettings();
                con = app.GetConnection();
                DynamicParameters param = new DynamicParameters();
                param.Add(name: "P_ES_ID", value: ES_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_ES_STATUS", value: "ACTIVE", dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add(name: "P_UPDATE_ID", value: GlobalValue.User_ID, dbType: DbType.String, direction: ParameterDirection.Input);
                con.Execute("UPD_REMIT_ES", param, commandType: CommandType.StoredProcedure);
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


        //public bool DeleteRecord(string ES_ID)
        //{
        //    try
        //    {
        //        //Get Connection
        //        AppSettings app = new AppSettings();
        //        con = app.GetConnection();
        //        DynamicParameters param = new DynamicParameters();
        //        //Input Param
        //        param.Add(name: "P_ES_ID", value: ES_ID, dbType: DbType.String, direction: ParameterDirection.Input);
        //        int result = con.Execute(sql: "DEL_REMIT_EMPLOYER_FUND", param: param, commandType: CommandType.StoredProcedure);

        //        if (result > 0)
        //            return true;
        //        else
        //            return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (con.State == ConnectionState.Open)
        //        {
        //            con.Close();
        //            if (con != null) { con = null; }
        //        }
        //    }
        //}

        public bool isESUnique(string ES_ID) 
        {
            try
            {
                //Get connection
                var con = new AppSettings();
                var param = new DynamicParameters();
                param.Add("P_ES_ID", ES_ID, DbType.String, ParameterDirection.Input);
                param.Add("VDATA", null, DbType.Int32, ParameterDirection.Output);
                con.GetConnection().Execute("SEL_REMIT_ES_EXIST", param, commandType: CommandType.StoredProcedure);
                int paramoption = param.Get<int>("VDATA");

                if (paramoption == 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataSet ESData()
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                DataSet ds = new DataSet();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                OracleParameter param = cmd.CreateParameter();

                cmd.CommandText = "SEL_REMIT_EMPLOYER_SCHEME";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "ES");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<crm_EmployerSchemeRepo> GetESList()
        {
            try
            {
                DataSet dt = ESData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new crm_EmployerSchemeRepo
                {
                    ES_Id = row.Field<string>("ES_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                    Employer_Name = row.Field<string>("EMPLOYER_NAME"),
                    Registration_Number = row.Field<string>("REGISTRATION_NUMBER"),
                    Office_Location = row.Field<string>("OFFICE_LOCATION"),                  
                    Phone_Number = row.Field<string>("PHONE_NUMBER"),
                    ES_Status = row.Field<string>("ES_STATUS"),
                    Auth_Status = row.Field<string>("AUTH_STATUS")
                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
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

        // FOR PENDING EMPLOYER SCHEME
        public DataSet ESPendingData()
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                DataSet ds = new DataSet();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                OracleParameter param = cmd.CreateParameter();

                cmd.CommandText = "SEL_REMIT_ES_PENDING";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "es");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<crm_EmployerSchemeRepo> GetESPendingList()
        {
            try
            {
                DataSet dt = ESPendingData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new crm_EmployerSchemeRepo
                {
                    ES_Id = row.Field<string>("ES_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                    Employer_Name = row.Field<string>("EMPLOYER_NAME"),
                    Registration_Number = row.Field<string>("REGISTRATION_NUMBER"),
                    Office_Location = row.Field<string>("OFFICE_LOCATION"),
                    Phone_Number = row.Field<string>("PHONE_NUMBER"),
                    ES_Status = row.Field<string>("ES_STATUS"),
                    Auth_Status = row.Field<string>("AUTH_STATUS")
                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
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

        // FOR NOT CLOSED EMPLOYER SCHEME
        public DataSet ESNotClosedData()
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                DataSet ds = new DataSet();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                OracleParameter param = cmd.CreateParameter();

                cmd.CommandText = "SEL_REMIT_ES_NOTCLOSED";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "es");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<crm_EmployerSchemeRepo> GetESNotClosedList()
        {
            try
            {
                DataSet dt = ESNotClosedData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new crm_EmployerSchemeRepo
                {
                    ES_Id = row.Field<string>("ES_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                    Employer_Name = row.Field<string>("EMPLOYER_NAME"),
                    Registration_Number = row.Field<string>("REGISTRATION_NUMBER"),
                    Office_Location = row.Field<string>("OFFICE_LOCATION"),
                    Phone_Number = row.Field<string>("PHONE_NUMBER"),
                    ES_Status = row.Field<string>("ES_STATUS"),
                    Auth_Status = row.Field<string>("AUTH_STATUS")
                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
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

        // FOR  CLOSED EMPLOYER SCHEME
        public DataSet ESClosedData()
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                DataSet ds = new DataSet();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                OracleParameter param = cmd.CreateParameter();

                cmd.CommandText = "SEL_REMIT_ES_CLOSED";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "es");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<crm_EmployerSchemeRepo> GetESClosedList()
        {
            try
            {
                DataSet dt = ESClosedData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new crm_EmployerSchemeRepo
                {
                    ES_Id = row.Field<string>("ES_ID"),
                    Scheme_Id = row.Field<string>("SCHEME_ID"),
                    Scheme_Name = row.Field<string>("SCHEME_NAME"),
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                    Employer_Name = row.Field<string>("EMPLOYER_NAME"),
                    Registration_Number = row.Field<string>("REGISTRATION_NUMBER"),
                    Office_Location = row.Field<string>("OFFICE_LOCATION"),
                    Phone_Number = row.Field<string>("PHONE_NUMBER"),
                    ES_Status = row.Field<string>("ES_STATUS"),
                    Auth_Status = row.Field<string>("AUTH_STATUS")
                }).ToList();

                return eList;
            }
            catch (Exception)
            {
                throw;
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


        // FOR EMPLOYER GRID
        public DataSet EmployerData()
        {
            try
            {
                //Get connection
                var app = new AppSettings();
                con = app.GetConnection();

                DataSet ds = new DataSet();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                OracleParameter param = cmd.CreateParameter();

                cmd.CommandText = "SEL_CRM_EMPLOYER_ES";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = (OracleConnection)con;

                param = cmd.Parameters.Add("cur", OracleDbType.RefCursor);
                param.Direction = ParameterDirection.Output;

                da = new OracleDataAdapter(cmd);
                da.Fill(ds, "emp");
                return ds;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public IEnumerable<crm_EmployerRepo> GetEmployerList()
        {
            try
            {
                DataSet dt = EmployerData();
                var eList = dt.Tables[0].AsEnumerable().Select(row => new crm_EmployerRepo
                {
                    Employer_Id = row.Field<string>("EMPLOYER_ID"),
                    Employer_Name = row.Field<string>("EMPLOYER_NAME"),
                    Business_Address = row.Field<string>("BUSINESS_ADDRESS"),
                    Employer_Postal_Address = row.Field<string>("POSTAL_ADDRESS"),
                    Phone_Number = row.Field<string>("PHONE_NUMBER"),
                    Employer_Email_Address = row.Field<string>("EMAIL_ADDRESS"),
                    Web_Site = row.Field<string>("WEB_SITE"),
                    Registration_Number = row.Field<string>("REGISTRATION_NUMBER"),
                    Contact_Number = row.Field<string>("CONTACT_NUMBER"),
                    Contact_Person = row.Field<string>("CONTACT_PERSON"),
                    Employer_Other_Number = row.Field<string>("OTHER_NUMBER"),
                    Region_Id = row.Field<string>("REGION_ID"),
                    Office_Location = row.Field<string>("OFFICE_LOCATION"),
                    Sector_Id = row.Field<string>("SECTOR_ID"),


                }).ToList();

                return eList;

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

    }
}
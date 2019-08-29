using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Oracle.ManagedDataAccess.Client;
using PENFAD6DAL.DbContext;
using Dapper;
using PENFAD6DAL.GlobalObject;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace PENFAD6DAL.Repository.GL
{
    public class GLTBtRepo
    {
       
        public string containerId { get; set; }
        public string Scheme_Fund_Id { get; set; }
        //[Required]
        public DateTime? TB_Date { get; set; }

    } //end class gl repo
}

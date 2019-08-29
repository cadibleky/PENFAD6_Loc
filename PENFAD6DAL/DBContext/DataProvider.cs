using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PENFAD6DAL.DbContext
{ 
    enum DataProvider
    {
        SqlServer,
        OleDb,
        Odbc,
        Oracle
    }
}


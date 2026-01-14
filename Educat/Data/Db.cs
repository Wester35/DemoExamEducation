using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace EducAnalys.Data
{
    public static class Db
    {
        private static readonly string _connectString = 
            @"Server=localhost\SQLEXPRESS;
            Database=Educ;
            Trusted_connection=true;
            SqlServerCertificate=true;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connectString);
        }
    }
}

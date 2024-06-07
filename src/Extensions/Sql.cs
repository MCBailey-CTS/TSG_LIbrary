using System;
using System.Data.SqlClient;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
        public const string conn_str =
            "Data Source=tsgapps2.toolingsystemsgroup.com;Initial Catalog=CTSAPP;User ID=CTSAPP;Password=RI4SU9d2JxH8LcrxSDPS";


        public static object ExecuteScalar(string command_text)
        {
            using (var cnn = new SqlConnection(conn_str))
            {
                cnn.Open();

                using (var sql = new SqlCommand
                       {
                           Connection = cnn,

                           CommandText = command_text
                       })
                {
                    return sql.ExecuteScalar();
                }
            }
        }

        public static int ExecuteScalarInt(string command_text)
        {
            return Convert.ToInt32(Convert.ToDecimal(ExecuteScalar(command_text)));
        }

        public static SqlDataReader ExecuteDatReader(string command_text)
        {
            using (var cnn = new SqlConnection(conn_str))
            {
                cnn.Open();

                using (var sql = new SqlCommand
                       {
                           Connection = cnn,

                           CommandText = command_text
                       })
                {
                    return sql.ExecuteReader();
                }
            }
        }
    }
}
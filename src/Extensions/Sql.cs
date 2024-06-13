using System;
using System.Data.SqlClient;

namespace TSG_Library.Extensions
{
    public static partial class __Extensions_
    {
        #region Sql

        public const string conn_str =
            "Data Source=tsgapps2.toolingsystemsgroup.com;Initial Catalog=CTSAPP;User ID=CTSAPP;Password=RI4SU9d2JxH8LcrxSDPS";


        public static object __ExecuteScalar(string command_text)
        {
            using (SqlConnection cnn = new SqlConnection(conn_str))
            {
                cnn.Open();

                using (SqlCommand sql = new SqlCommand
                       {
                           Connection = cnn,

                           CommandText = command_text
                       })
                {
                    return sql.ExecuteScalar();
                }
            }
        }

        public static int __ExecuteScalarInt(string command_text)
        {
            return Convert.ToInt32(Convert.ToDecimal(__ExecuteScalar(command_text)));
        }

        public static SqlDataReader __ExecuteDataReader(string command_text)
        {
            using (SqlConnection cnn = new SqlConnection(conn_str))
            {
                cnn.Open();

                using (SqlCommand sql = new SqlCommand
                       {
                           Connection = cnn,

                           CommandText = command_text
                       })
                {
                    return sql.ExecuteReader();
                }
            }
        }

        #endregion
    }
}
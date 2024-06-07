using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;

namespace TSG_Library.Utilities
{
    public static class UFuncLog
    {
        public static string connection_string_ctsapp =>
            @"Data Source=tsgapps2.toolingsystemsgroup.com;Initial Catalog=CTSAPP;User ID=CTSAPP;Password=RI4SU9d2JxH8LcrxSDPS";

        public static int open(string connection_string, string ufunc_name, int hash)
        {
            using (var cnn = new SqlConnection(connection_string))
            {
                cnn.Open();

                int scope_identity;

                using (var sql = new SqlCommand
                       {
                           Connection = cnn,

                           CommandText =
                               $@"INSERT INTO ufunc_log (user_name, ufunc_name, session_process_id, date_started, hash_id, executing_path) 
                                    values (
                                        '{Environment.UserName}',
                                        '{ufunc_name}', 
                                        {Process.GetCurrentProcess().Id}, 
                                        '{DateTime.Now}', 
                                        {hash}, 
                                        '{Assembly.GetExecutingAssembly().Location}')
                        SELECT SCOPE_IDENTITY()"
                       })
                {
                    scope_identity = decimal.ToInt32((decimal)sql.ExecuteScalar());
                }


                using (var sql = new SqlCommand
                       {
                           Connection = cnn,
                           CommandText = $@"INSERT INTO ufuncs_in_use (ufunc_log_id) values ({scope_identity})"
                       })
                {
                    sql.ExecuteScalar();
                }

                return scope_identity;
            }
        }

        public static void close(string connection_string, int scope_identity)
        {
            using (var cnn = new SqlConnection(connection_string))
            {
                cnn.Open();

                using (var sql = new SqlCommand
                       {
                           Connection = cnn,

                           CommandText = $@" update ufunc_log
                                        set date_ended='{DateTime.Now}'
                                        where {scope_identity} = execute_id"
                       })
                {
                    sql.ExecuteScalar();
                }
            }


            using (var cnn = new SqlConnection(connection_string))
            {
                cnn.Open();

                using (var sql = new SqlCommand
                       {
                           Connection = cnn,

                           CommandText =
                               $@" DELETE FROM ufuncs_in_use where {scope_identity} = ufunc_log_id select @@ROWCOUNT as deleted"
                       })
                {
                    sql.ExecuteScalar();
                }
            }
        }

        public static int method_executed(string connection_string, int scope_identity, string method_signature)
        {
            using (var cnn = new SqlConnection(connection_string))
            {
                cnn.Open();

                using (var sql = new SqlCommand
                       {
                           Connection = cnn,
                           CommandText =
                               $@"INSERT INTO ufunc_method_log (method_signature, result, message, ufunc_log_id) 
                                    values (
                                        '{method_signature}',
                                        0, 
                                        '', 
                                        {scope_identity})
                                        select SCOPE_IDENTITY()"
                       })
                {
                    return decimal.ToInt32((decimal)sql.ExecuteScalar());
                }
            }
        }


        public static void method_terminated(string connection_string, int method_scope_identity, int result,
            string message = "")
        {
            using (var cnn = new SqlConnection(connection_string))
            {
                cnn.Open();

                using (var sql = new SqlCommand
                       {
                           Connection = cnn,

                           CommandText =
                               $@"update ufunc_method_log set result ={result}, message='{message}' where id={method_scope_identity}"
                       })
                {
                    sql.ExecuteScalar();
                }
            }
        }
    }
}
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using NXOpen;
using NXOpen.UF;

namespace TSG_Library.Forms
{
    internal class SessionSql
    {
        public const string conn_str =
            "Data Source=tsgapps2.toolingsystemsgroup.com;Initial Catalog=CTSAPP;User ID=CTSAPP;Password=RI4SU9d2JxH8LcrxSDPS";

        public static int _session__id;

        private static readonly Session _session_ = Session.GetSession();

        public static ListingWindow __lw
        {
            get
            {
                var lw = Session.GetSession().ListingWindow;

                if (lw.IsOpen) return lw;

                lw.Open();

                return lw;
            }
        }

        public static int Startup()
        {
            try
            {
                int? user_id = null;

                user_id = NewMethod(user_id);

                if (user_id is null)
                    user_id = NewMethod1();
                NewMethod2(user_id);
            }
            catch (Exception ex)
            {
                var lw = Session.GetSession().ListingWindow;
                lw.Open();
                lw.WriteLine(ex.Message);
            }

            return 0;
        }

        private static void NewMethod2(int? user_id)
        {
            using (var cnn = new SqlConnection(conn_str))
            {
                cnn.Open();

                UFSession.GetUFSession().UF.AskSystemInfo(out var info);

                var is_batch = _session_.IsBatch ? 1 : 0;

                using (var sql = new SqlCommand
                       {
                           Connection = cnn,

                           CommandText = $@"
				            insert into session_log 
				                (ses_user_id, 
                                 ses_proc_id, ses_date, 
                                 ses_ENV_NX_FULL_VERSION, 
                                 ses_ENV_SPLM_LICENSE_SERVER, 
                                 ses_ENV_USER_STARTUP, 
                                 ses_is_batch,                                 
                                 ses_sys_log,
                                 ses_machine_type) 
                                    values(
					                {user_id}, 
					                {Process.GetCurrentProcess().Id}, 
					                '{DateTime.Now}', 
                                    '{_session_.GetEnvironmentVariableValue("NX_FULL_VERSION")}',
                                    '{_session_.GetEnvironmentVariableValue("SPLM_LICENSE_SERVER")}',
                                    '{_session_.GetEnvironmentVariableValue("USER_STARTUP")}',
                                    {is_batch},
                                    '{_session_.LogFile.FileName}',
                                    '{info.machine_type}') 
                                    SELECT SCOPE_IDENTITY()"
                       })
                {
                    _session__id = Convert.ToInt32(Convert.ToDecimal(sql.ExecuteScalar()));
                }

                using (var sql = new SqlCommand
                       {
                           Connection = cnn,

                           //CommandText = $@" insert into sessions_in_use (siu_ses_id) values ({_session__id})"
                           CommandText =
                               $@" insert into sessions_in_use (siu_ses_id, siu_user_id, siu_proc_id) values ({_session__id}, {user_id}, {Process.GetCurrentProcess().Id})"
                       })
                {
                    sql.ExecuteScalar();
                }
            }
        }

        public static int user_id()
        {
            return 0;
        }

        private static int? NewMethod1()
        {
            int? user_id;
            using (var cnn = new SqlConnection(conn_str))
            {
                cnn.Open();

                UFSession.GetUFSession().UF.AskSystemInfo(out var info);

                using (var sql = new SqlCommand
                       {
                           Connection = cnn,

                           CommandText = $@"insert into ufunc_users (user_name) values('{info.user_name}')"
                       })
                {
                    user_id = (int)sql.ExecuteScalar();
                }
            }

            return user_id;
        }

        private static int? NewMethod(int? user_id)
        {
            using (var cnn = new SqlConnection(conn_str))
            {
                cnn.Open();

                UFSession.GetUFSession().UF.AskSystemInfo(out var info);

                using (var sql = new SqlCommand
                       {
                           Connection = cnn,

                           CommandText =
                               $@"select user_id, user_name from ufunc_users where user_name='{info.user_name}'"
                       })
                {
                    var result = sql.ExecuteScalar();

                    if (result is int _k)
                        user_id = _k;
                }
            }

            return user_id;
        }

        public static int GetUnloadOption()
        {
            return (int)Session.LibraryUnloadOption.AtTermination;
        }

        public static void UnloadLibrary()
        {
            using (var cnn = new SqlConnection(conn_str))
            {
                cnn.Open();
                using (var sql = new SqlCommand
                       {
                           Connection = cnn,

                           CommandText = $@"delete from sessions_in_use where siu_ses_id={_session__id}"
                       })
                {
                    sql.ExecuteNonQuery();
                }
            }
        }
    }
}
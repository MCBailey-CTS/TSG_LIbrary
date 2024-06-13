using System;
using NXOpen;

namespace TSG_Library.Extensions
{
    public static partial class __Extensions_
    {
        #region Exception

        public static void __PrintException(this Exception ex)
        {
            ex.__PrintException(null);
        }

        public static void __PrintException(this Exception ex, string userMessage)
        {
            _ = ex.GetType();
            _ = ex.Message;

            //int __nx_error_code = -1;

            //var error_lines = new List<string>();


            print_("///////////////////////////////////////////");

            if (!string.IsNullOrEmpty(userMessage))
                print_($"UserMessage: {userMessage}");

            print_($"Message: {ex.Message}");

            print_($"Exception: {ex.GetType()}");

            if (ex is NXException nx)
                print_($"Error Code: {nx.ErrorCode}");

            string[] methods = ex.StackTrace.Split('\n');

            //for (int i = 0; i < methods.Length; i++)
            //{
            //    string line = methods[i];

            //    int lastIndex = line.LastIndexOf('\\');

            //    if (lastIndex < 0)
            //        continue;

            //    string substring = line.Substring(lastIndex);

            //    print_($"[{i}]: {substring}");

            //    error_lines.Add($"[{i}]: {substring}");
            //}

            for (int i = 0; i < methods.Length; i++)
                //string line = ;
                //int lastIndex = line.LastIndexOf('\\');
                //if (lastIndex < 0)
                //    continue;
                //string substring = line.Substring(lastIndex);
                print_($"[{i}]: {methods[i]}");
            //error_lines.Add($"[{i}]: {substring}");
            print_("///////////////////////////////////////////");
            //foreach(var)
            //using (var cnn = new SqlConnection(conn_str))
            //{
            //    cnn.Open();

            //    using (var sql = new SqlCommand
            //    {
            //        Connection = cnn,

            //        CommandText = $@"insert into ufunc_exceptions
            //                        (ufunc_exception_type)
            //                        values
            //                        ('{ex.Message}')"


            //    })
            //        sql.ExecuteScalar();
            //}
        }

        #endregion
    }
}
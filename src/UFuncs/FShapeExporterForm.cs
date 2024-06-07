using System;
using System.Windows.Forms;
using TSG_Library.Attributes;
using static TSG_Library.UFuncs._UFunc;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_f_shape_exporter)]
    public partial class FShapeExporterForm : _UFuncForm
    {
        public string Answer;

        public int scope_indentity = -1;

        public FShapeExporterForm()
        {
            InitializeComponent();
        }

        private void BtnAccept_Click(object sender, EventArgs e)
        {
            // int? method_scope_identity = null;

            // int result = 1;

            // string message = "";

            // try
            // {
            //     method_scope_identity = UFuncLog.method_executed(UFuncLog.connection_string_ctsapp, scope_indentity, nameof(BtnAccept_Click));
            // }
            // catch (Exception ex)
            // {
            //     result = -1;
            //     ex._PrintException();
            //     message = ex.Message;
            // }

            // if (method_scope_identity is int scope)
            //     UFuncLog.method_terminated(UFuncLog.connection_string_ctsapp, scope, result, message);
        }

        private void TxtOpNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 13)
                Result();
        }

        private void Result()
        {
            if(txtOpNum.Text != string.Empty)
            {
                Answer = txtOpNum.Text;
                Dispose();
            }
            else
            {
                MessageBox.Show(@"Please type a Sim number");
            }
        }

        private void FShapeExporterForm_Load(object sender, EventArgs e)
        {
            //var connetionString = "Data Source=cqz02f6h9c.database.windows.net;Initial Catalog=TSG-Test-Mark;Persist Security Info=True;User ID=TSGTestdev;Password=CA09876ca; MultipleActiveResultSets=true; Connection Timeout=120";
            //var connetionString = "Data Source=cqz02f6h9c.database.windows.net;Initial Catalog=TSGMaster_Dev;Persist Security Info=True;User ID=TSGTestdev;Password=CA09876ca; MultipleActiveResultSets=true; Connection Timeout=120";


            //scope_indentity = UFuncLog.open(UFuncLog.connection_string_ctsapp, "f-shape-exporter", GetHashCode());
        }

        private void FShapeExporterForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //UFuncLog.close(UFuncLog.connection_string_ctsapp, scope_indentity);
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
        }
    }
}


//INSERT INTO ufunc_log (user_name, ufunc_name, session_process_id, date_started, hash_id) 
//                                    values('{Environment.UserName}', 'f-shape-exporter', { Process.GetCurrentProcess().Id}, '{DateTime.Now}', { GetHashCode()})
//                        SELECT SCOPE_IDENTITY()"
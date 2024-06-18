using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TSG_Library.Attributes;
using TSG_Library.Extensions;
using TSG_Library.Forms;

namespace TSG_Library.UFuncs
{
    public partial class SessionForm : Form
    {
        private readonly ISet<_UFunc> __active = new HashSet<_UFunc>();

        public SessionForm()
        {
            InitializeComponent();

            treeView1.Sort();

            IEnumerable<Type> __pairs = from __type in typeof(SessionSql).Assembly.GetTypes()
                let atts = __type.GetCustomAttributes(typeof(UFuncAttribute), false)
                where atts.Length == 1
                let ufunc_att = (UFuncAttribute)atts[0]
                select __type;

            foreach (Type k in __pairs)
                //switch(k.Name)
                //{
                //    case "LayoutRefSets":
                //    case "ProfileTrimAndForm":
                //    case "CloneStrip":
                //    case "StripRefSetter":
                //        break;
                //    default:
                //        continue;
                //}
                treeView1.Nodes.Add(k.Name).Tag = k;
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
//        UserMessage: TSG_Library.Forms.BlankDataBuilderForm
//Message: Unable to cast object of type 'TSG_Library.Forms.BlankDataBuilderForm' to type 'TSG_Library.Ufuncs._UFunc'.
//Exception: System.InvalidCastException
//[0]: \SessionForm.cs:line 53
            try
            {
                Type __type = (Type)e.Node.Tag;


                _IUFunc __obj = (_IUFunc)Activator.CreateInstance(__type);


                __obj.execute();

                //if (__obj.libary_unload_option == NXOpen.Session.LibraryUnloadOption.Immediately)
                //{
                //    __obj.dispose();
                //    return;
                //}

                //__active.Add(__obj);
            }
            catch (Exception ex)
            {
                ex.__PrintException($"{(Type)e.Node.Tag}");
            }
        }

        //private void SessionForm_Load(object sender, EventArgs e)
        //{
        //    //Reset();
        //}


        //public void Reset()
        //{
        //    //treeView1.Nodes.Clear();

        //    //var node = treeView1.Nodes.Add("Loaded Parts");

        //    //node.Text = NXOpen.Session.GetSession().Parts.Display.Leaf;


        //}

        //private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        print_(session__.nx___id);
        //    }
        //    catch (Exception ex)
        //    {
        //        ex._PrintException();
        //    }

        //    //using (var cnn = new SqlConnection(conn_str))
        //    //{
        //    //    cnn.Open();

        //    //    using (var sql = new SqlCommand
        //    //    {
        //    //        Connection = cnn,

        //    //        CommandText = $@"insert into session_log1 
        //    //                        (user_name, process_id) 
        //    //                        values
        //    //                        ('{Environment.UserName}', {System.Diagnostics.Process.GetCurrentProcess().Id})"
        //    //    })
        //    //        user_id = (int)sql.ExecuteScalar();
        //    //}

        //    //using (var cnn = new SqlConnection(conn_str))
        //    //{
        //    //    cnn.Open();

        //    //    using (var sql = new SqlCommand
        //    //    {
        //    //        Connection = cnn,

        //    //        CommandText = $@"select session_id from session_log1 
        //    //                            where user_name='{Environment.UserName}'
        //    //                            and process_id={System.Diagnostics.Process.GetCurrentProcess().Id}"
        //    //    })
        //    //    {
        //    //        using(var dr = sql.ExecuteReader())
        //    //        {
        //    //            print_(dr.HasRows);
        //    //        }
        //    //    }
        //    //        //user_id = (int)sql.ExecuteScalar();
        //    //}

        //    //return user_id;
        //}

        //public const string conn_str = "Data Source=tsgapps2.toolingsystemsgroup.com;Initial Catalog=CTSAPP;User ID=CTSAPP;Password=RI4SU9d2JxH8LcrxSDPS";

        //private void SessionForm_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    //e.CloseReason = CloseReason.
        //    //using (var cnn = new SqlConnection(conn_str))
        //    //{
        //    //    cnn.Open();

        //    //    using (var sql = new SqlCommand
        //    //    {
        //    //        Connection = cnn,

        //    //        CommandText = $@"select session_id from session_log1 
        //    //                            where user_name='{Environment.UserName}'
        //    //                            and process_id={System.Diagnostics.Process.GetCurrentProcess().Id}"
        //    //    })
        //    //    {
        //    //        using (var dr = sql.ExecuteReader())
        //    //        {
        //    //            print_(dr.HasRows);
        //    //        }
        //    //    }
        //    //    //user_id = (int)sql.ExecuteScalar();
        //    //}

        //    //foreach(var k in __active)
        //    //{
        //    //    //print_(k.ufunc_name);

        //    //    if(!k.is_disposed())
        //    //        k.dispose();


        //    //}
        //}

        //private void SessionForm_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    //try
        //    //{
        //    //    using (var cnn = new SqlConnection(conn_str))
        //    //    {
        //    //        cnn.Open();
        //    //        using (var sql = new SqlCommand
        //    //        {
        //    //            Connection = cnn,


        //    //            CommandText = $@"update ufunc_user_properties1
        //    //                             set uup_value='{{X={Location.X},Y={Location.Y}}}'
        //    //                             where uup_key='window-location' and
        //    //                             user_name='{Environment.UserName}' and  
        //    //                             ufunc_name='session-form'
        //    //                        "
        //    //        })
        //    //            sql.ExecuteNonQuery();
        //    //    }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    ex._PrintException();
        //    //}
        //}

        //public static int /*session__.nx___id*/;
    }
}
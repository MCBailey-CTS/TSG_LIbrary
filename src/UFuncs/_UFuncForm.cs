using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TSG_Library.Attributes;

namespace TSG_Library.UFuncs
{
    public class _UFuncForm : Form, _IUFunc
    {
        public string ufunc_name => GetType().GetCustomAttributes(true).OfType<UFuncAttribute>().Single().ufunc_name;

        public void execute()
        {
            Show();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // _UFuncForm
            // 
            ClientSize = new Size(284, 261);
            Name = "_UFuncForm";
            TopMost = true;
            ResumeLayout(false);
        }
    }
}
using System;
using System.Windows.Forms;

namespace TSG_Library.Disposable
{
    public class HideShowForm : IDisposable
    {
        private readonly Form __form;

        public HideShowForm(Form __form)
        {
            this.__form = __form;

            __form.Hide();
        }

        public void Dispose()
        {
            __form.Show();
        }
    }
}
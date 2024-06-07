using System;
using System.Windows.Forms;

namespace TSG_Library.Disposable
{
    public class FormHideShow : IDisposable
    {
        private readonly Form __form;

        public FormHideShow(Form __form)
        {
            __form.Hide();

            this.__form = __form;
        }

        public void Dispose()
        {
            __form.Show();
        }
    }
}
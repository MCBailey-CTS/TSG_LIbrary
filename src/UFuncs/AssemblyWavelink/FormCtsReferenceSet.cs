using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NXOpen;
using Component = NXOpen.Assemblies.Component;
using Point = System.Drawing.Point;

namespace TSG_Library.Forms
{
    public class FormCtsReferenceSet : Form
    {
        private const int MinimumListBoxWidth = 156;
        private const int MaxRows = 40;
        private const int MinRows = 10;

        /// <summary>Required designer variable.</summary>
        private readonly IContainer components = null;

        protected Button btnExit;
        protected Button btnOk;
        protected ListBox lstBox;

        public FormCtsReferenceSet()
        {
            InitializeComponent();
        }

        public FormCtsReferenceSet(IEnumerable<Component> components)
            : this()
        {
            Component[] array1 = components.ToArray();

            if(!array1.Any())
                throw new ArgumentOutOfRangeException(nameof(components));

            var array2 = MergeStrings(array1).ToArray();
            if(array1.Length == 1)
            {
                var strArray = new string[2]
                {
                    "layout",
                    "blank"
                };
                Array.Sort(array2);
                Text = strArray.Any(array1[0].Name.ToLower().Contains) ? array1[0].Name : array1[0].DisplayName;
            }
            else
            {
                Text = "Multiple Tools";
            }

            lstBox.Items.AddRange(array2.Distinct().Cast<object>().ToArray());
            SetFormToListBox();
        }

        public ListingWindow Lw
        {
            get
            {
                if(!Session.GetSession().ListingWindow.IsOpen)
                    Session.GetSession().ListingWindow.Open();
                return Session.GetSession().ListingWindow;
            }
        }

        public string SelectedReferenceSetName { get; protected set; }

        public bool IsSelected { get; protected set; }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if(lstBox.SelectedIndex < 0)
                return;
            SelectedReferenceSetName = lstBox.Text;
            IsSelected = true;
            Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected IEnumerable<string> MergeStrings(IEnumerable<Component> snapComponents)
        {
            Component[] array = snapComponents.ToArray();
            IEnumerable<string> source = ((Part)array[0].Prototype).GetAllReferenceSets().Select(str => str.Name);
            for (var index = 1; index < array.Length; ++index)
            {
                List<string> list = ((BasePart)array[index].Prototype).GetAllReferenceSets().Select(str => str
                    .Name).ToList();
                source = source.Select(x => x).Intersect(list).ToList();
            }

            return source;
        }

        public void RemoveReferenceSet(string name)
        {
            lstBox.Items.Remove(name);
        }

        protected virtual void SetFormToListBox()
        {
            var array = lstBox.Items.OfType<string>().ToArray();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            lstBox.Location = new Point(12, 12);
            var num = array.Select(str =>
                {
                    var data = new { str, font = lstBox.Font };
                    return data;
                }).Select(_param0 => TextRenderer.MeasureText(_param0.str, _param0.font)).Select(size => size.Width)
                .Concat(new int[1]).Max();
            lstBox.Width = num + 20 < 156 ? 156 : num + 20;
            if(array.Length > 40)
                lstBox.Height = lstBox.ItemHeight * 41;
            else if(array.Length < 10)
                lstBox.Height = lstBox.ItemHeight * 10;
            else
                lstBox.Height = lstBox.ItemHeight * (array.Length + 1);
            btnOk.Location = new Point(lstBox.Location.X, lstBox.Location.Y + lstBox.Height - 3);
            Button btnExit = this.btnExit;
            Point location = lstBox.Location;
            var x = location.X + lstBox.Width - this.btnExit.Size.Width;
            location = btnOk.Location;
            var y = location.Y;
            Point point = new Point(x, y);
            btnExit.Location = point;
            location = lstBox.Location;
            Width = location.X * 2 + lstBox.Size.Width + 20;
        }

        private void lstBox_DoubleClick(object sender, EventArgs e)
        {
            IsSelected = true;
            SelectedReferenceSetName = lstBox.Text;
            btnOk.PerformClick();
        }

        private void FormCtsReferenceSet_Load(object sender, EventArgs e)
        {
        }

        private void FormCtsReferenceSet_MouseClick(object sender, MouseEventArgs e)
        {
        }

        /// <summary>Clean up any resources being used.</summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lstBox = new ListBox();
            btnOk = new Button();
            btnExit = new Button();
            SuspendLayout();
            lstBox.FormattingEnabled = true;
            lstBox.Location = new Point(12, 12);
            lstBox.MinimumSize = new Size(156, 95);
            lstBox.Name = "lstBox";
            lstBox.Size = new Size(156, 95);
            lstBox.TabIndex = 0;
            lstBox.DoubleClick += lstBox_DoubleClick;
            btnOk.Location = new Point(12, 113);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(75, 23);
            btnOk.TabIndex = 1;
            btnOk.Text = "Ok";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            btnExit.Location = new Point(93, 113);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(75, 23);
            btnExit.TabIndex = 2;
            btnExit.Text = "Exit";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            AutoScaleDimensions = new SizeF(6f, 13f);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(284, 262);
            Controls.Add(btnExit);
            Controls.Add(btnOk);
            Controls.Add(lstBox);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = nameof(FormCtsReferenceSet);
            Padding = new Padding(12);
            Text = nameof(FormCtsReferenceSet);
            Load += FormCtsReferenceSet_Load;
            MouseClick += FormCtsReferenceSet_MouseClick;
            ResumeLayout(false);
        }
    }
}
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Windows.Forms;

//namespace TSG_Library.UFuncs
//{
//    public partial class AssemblyWavelink
//    {
//        public class FormCtsReferenceSet : Form
//        {
//            /// <summary>Required designer variable.</summary>
//            private readonly System.ComponentModel.IContainer components = null;

//            private const int MinimumListBoxWidth = 156;
//            private const int MaxRows = 40;
//            private const int MinRows = 10;
//            protected ListBox lstBox;
//            protected Button btnOk;
//            protected Button btnExit;

//            public FormCtsReferenceSet()
//            {
//                InitializeComponent();
//            }

//            private void btnOk_Click(object sender, EventArgs e)
//            {
//                if (lstBox.SelectedIndex < 0)
//                    return;
//                SelectedReferenceSetName = lstBox.Text;
//                IsSelected = true;
//                Close();
//            }

//            private void btnExit_Click(object sender, EventArgs e)
//            {
//                Close();
//            }

//            public NXOpen.ListingWindow Lw
//            {
//                get
//                {
//                    if (!NXOpen.Session.GetSession().ListingWindow.IsOpen)
//                        NXOpen.Session.GetSession().ListingWindow.Open();
//                    return NXOpen.Session.GetSession().ListingWindow;
//                }
//            }

//            protected IEnumerable<string> MergeStrings(IEnumerable<NXOpen.Assemblies.Component> snapComponents)
//            {
//                NXOpen.Assemblies.Component[] array = snapComponents.ToArray();
//                IEnumerable<string> source = array[0]._Prototype().GetAllReferenceSets().Select(str => str.Name);
//                for (int index = 1; index < array.Length; ++index)
//                {
//                    List<string> list = ((NXOpen.BasePart)array[index].Prototype).GetAllReferenceSets().Select(str => str
//                        .Name).ToList();
//                    source = source.Select(x => x).Intersect(list).ToList();
//                }

//                return source;
//            }

//            //public FormCtsReferenceSet(IEnumerable<NXOpen.Assemblies.Component> components)
//            //    : this(components.Select(c => (Component)c))
//            //{
//            //}

//            public FormCtsReferenceSet(IEnumerable<NXOpen.Assemblies.Component> components)
//                : this()
//            {
//                //Snap.NX.   Component[] array1 = components.Select( (component => Snap.NX. Component.Wrap(( component).Tag))).ToArray<Snap.NX. Component>();

//                NXOpen.Assemblies.Component[] array1 = components.ToArray();

//                if (!array1.Any())
//                    throw new ArgumentOutOfRangeException(nameof(components));
//                string[] array2 = MergeStrings(array1).ToArray();
//                if (array1.Length == 1)
//                {
//                    string[] strArray = new string[2]
//                    {
//                    "layout",
//                    "blank"
//                    };
//                    Array.Sort(array2);
//                    Text = strArray.Any(new Func<string, bool>(((NXOpen.NXObject)array1[0]).Name.ToLower().Contains)) ? ((NXOpen.NXObject)array1[0]).Name : array1[0].DisplayName;
//                }
//                else
//                    Text = "Multiple Tools";

//                lstBox.Items.AddRange(array2.Distinct().Cast<object>().ToArray());
//                SetFormToListBox();
//            }

//            public void RemoveReferenceSet(string name)
//            {
//                lstBox.Items.Remove(name);
//            }

//            protected virtual void SetFormToListBox()
//            {
//                string[] array = lstBox.Items.OfType<string>().ToArray();
//                FormBorderStyle = FormBorderStyle.FixedSingle;
//                lstBox.Location = new System.Drawing.Point(12, 12);
//                int num = array.Select(str =>
//                {
//                    var data = new { str = str, font = lstBox.Font };
//                    return data;
//                }).Select(_param0 => TextRenderer.MeasureText(_param0.str, _param0.font)).Select(size => size.Width).Concat(new int[1]).Max();
//                lstBox.Width = num + 20 < 156 ? 156 : num + 20;
//                if (array.Length > 40)
//                    lstBox.Height = lstBox.ItemHeight * 41;
//                else if (array.Length < 10)
//                    lstBox.Height = lstBox.ItemHeight * 10;
//                else
//                    lstBox.Height = lstBox.ItemHeight * (array.Length + 1);
//                btnOk.Location = new System.Drawing.Point(lstBox.Location.X, lstBox.Location.Y + lstBox.Height - 3);
//                Button btnExit = this.btnExit;
//                System.Drawing.Point location = lstBox.Location;
//                int x = location.X + lstBox.Width - this.btnExit.Size.Width;
//                location = btnOk.Location;
//                int y = location.Y;
//                System.Drawing.Point point = new System.Drawing.Point(x, y);
//                btnExit.Location = point;
//                location = lstBox.Location;
//                Width = location.X * 2 + lstBox.Size.Width + 20;
//            }

//            public string SelectedReferenceSetName { get; protected set; }

//            public bool IsSelected { get; protected set; }

//            private void lstBox_DoubleClick(object sender, EventArgs e)
//            {
//                IsSelected = true;
//                SelectedReferenceSetName = lstBox.Text;
//                btnOk.PerformClick();
//            }

//            private void FormCtsReferenceSet_Load(object sender, EventArgs e)
//            {
//            }

//            private void FormCtsReferenceSet_MouseClick(object sender, MouseEventArgs e)
//            {
//            }

//            /// <summary>Clean up any resources being used.</summary>
//            /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
//            protected override void Dispose(bool disposing)
//            {
//                if (disposing && components != null)
//                    components.Dispose();
//                base.Dispose(disposing);
//            }

//            /// <summary>
//            /// Required method for Designer support - do not modify
//            /// the contents of this method with the code editor.
//            /// </summary>
//            private void InitializeComponent()
//            {
//                lstBox = new ListBox();
//                btnOk = new Button();
//                btnExit = new Button();
//                SuspendLayout();
//                lstBox.FormattingEnabled = true;
//                lstBox.Location = new System.Drawing.Point(12, 12);
//                lstBox.MinimumSize = new Size(156, 95);
//                lstBox.Name = "lstBox";
//                lstBox.Size = new Size(156, 95);
//                lstBox.TabIndex = 0;
//                lstBox.DoubleClick += new EventHandler(lstBox_DoubleClick);
//                btnOk.Location = new System.Drawing.Point(12, 113);
//                btnOk.Name = "btnOk";
//                btnOk.Size = new Size(75, 23);
//                btnOk.TabIndex = 1;
//                btnOk.Text = "Ok";
//                btnOk.UseVisualStyleBackColor = true;
//                btnOk.Click += new EventHandler(btnOk_Click);
//                btnExit.Location = new System.Drawing.Point(93, 113);
//                btnExit.Name = "btnExit";
//                btnExit.Size = new Size(75, 23);
//                btnExit.TabIndex = 2;
//                btnExit.Text = "Exit";
//                btnExit.UseVisualStyleBackColor = true;
//                btnExit.Click += new EventHandler(btnExit_Click);
//                AutoScaleDimensions = new SizeF(6f, 13f);
//                AutoScaleMode = AutoScaleMode.Font;
//                AutoSize = true;
//                AutoSizeMode = AutoSizeMode.GrowAndShrink;
//                ClientSize = new Size(284, 262);
//                Controls.Add(btnExit);
//                Controls.Add(btnOk);
//                Controls.Add(lstBox);
//                MaximizeBox = false;
//                MinimizeBox = false;
//                Name = nameof(FormCtsReferenceSet);
//                Padding = new Padding(12);
//                Text = nameof(FormCtsReferenceSet);
//                Load += new EventHandler(FormCtsReferenceSet_Load);
//                MouseClick += new MouseEventHandler(FormCtsReferenceSet_MouseClick);
//                ResumeLayout(false);
//            }
//        }
//    }
//}
//// 1574


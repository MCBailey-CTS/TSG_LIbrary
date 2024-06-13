using System;
using System.Windows.Forms;
using NXOpen;
using TSG_Library.Attributes;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs
{
    [UFunc("identify-and-delet")]
    //[RevisionEntry("1.0", "2021", "09", "27")]
    //[Revision("1.0.1", "Created for NX 11")]
    //[RevisionEntry("11.1", "2023", "01", "09")]
    //[Revision("11.1.1", "Removed validation")]
    public partial class IdentifyAndDeleteForm : _UFuncForm
    {
        public IdentifyAndDeleteForm()
        {
            InitializeComponent();
        }

        private static Part __display_part_ => Session.GetSession().Parts.Display;

        private void SetTreeViewTolayer()
        {
            var layer = (int)numericUpDown1.Value;

            if(layer < 1)
                return;

            if(layer > 256)
            {
                treeView1.Nodes.Add("Layer must be between 1 and 256");

                return;
            }

            NXObject[] objects = __display_part_.Layers.GetAllObjectsOnLayer(layer);

            if(objects.Length == 0)
            {
                treeView1.Nodes.Add($"No objects on layer: {layer}");

                return;
            }

            foreach (NXObject nxObject in objects)
            {
                var key = nxObject.GetType().Name;

                TreeNode objectNode = treeView1.Nodes[key];

                if(objectNode is null)
                    objectNode = treeView1.Nodes.Add(key, key);

                TreeNode node = objectNode.Nodes.Add($"{nxObject.Tag}");

                node.Tag = nxObject;
            }

            foreach (TreeNode objectNode in treeView1.Nodes)
                objectNode.Text = $"{objectNode.Nodes[0].Tag.GetType().Name}: {objectNode.GetNodeCount(false)}";

            treeView1.SelectedNode = null;

            //numericUpDown1.Focus();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode != Keys.Delete)
                return;

            if(!treeView1.Focused)
                return;

            TreeNode selectedNode = treeView1.SelectedNode;

            if(selectedNode is null)
                return;

            if(!(selectedNode.Parent is null))
                return;
            session_.UpdateManager.ClearErrorList();

            Session.UndoMarkId id = session_.SetUndoMark(Session.MarkVisibility.Visible, "Delete");

            foreach (TreeNode node in selectedNode.Nodes)
            {
                if(!(node.Tag is NXObject nxObject))
                    continue;

                session_.UpdateManager.AddObjectsToDeleteList(new TaggedObject[] { nxObject });
            }

            session_.UpdateManager.DoUpdate(id);

            session_.DeleteUndoMark(id, "Delete");

            treeView1.Nodes.Clear();

            SetTreeViewTolayer();
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();

            SetTreeViewTolayer();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            numericUpDown1.Focus();

            numericUpDown1.Select(0, 1);
        }

        //public override void execute()
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}
using System;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Features;
using TSG_Library.Properties;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs
{
    public partial class AssemblyNavigator : Form
    {
        private int _partClosedHandler;
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS0169 // The field 'AssemblyNavigator._partCreatedHandler_' is never used
        private int _partCreatedHandler_;
#pragma warning restore CS0169 // The field 'AssemblyNavigator._partCreatedHandler_' is never used
#pragma warning restore IDE0044 // Add readonly modifier
        private int _partModifiedHandler_;
        private int _partOpenedHandler_;
        private int _partRenamedHandler_;
        private int _partSavedAsHandler_;
        private int _partSavedHandler_;
        private int _workPartChangedHandler_;

        public AssemblyNavigator()
        {
            InitializeComponent();
        }

        private void AssemblyNavigator_Load(object sender, EventArgs e)
        {
            Location = Settings.Default.assembly_navigator_window_location;
            _partClosedHandler = session_.Parts.AddPartClosedHandler(PartClosedHandler_);
            _partModifiedHandler_ = session_.Parts.AddPartCreatedHandler(PartCreatedHandler_);
            _partOpenedHandler_ = session_.Parts.AddPartModifiedHandler(PartModifiedHandler_);
            _partRenamedHandler_ = session_.Parts.AddPartOpenedHandler(PartOpenedHandler_);
            _partSavedAsHandler_ = session_.Parts.AddPartRenamedHandler(PartRenamedHandler_);
            _partSavedAsHandler_ = session_.Parts.AddPartSavedAsHandler(PartSavedAsHandler_);
            _partSavedHandler_ = session_.Parts.AddPartSavedHandler(PartSavedHandler_);
            _workPartChangedHandler_ = session_.Parts.AddWorkPartChangedHandler(WorkPartChangedHandler_);
            Reset();
        }

        private void AssemblyNavigator_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.assembly_navigator_window_location = Location;
            Settings.Default.Save();
            session_.Parts.RemovePartClosedHandler(_partClosedHandler);
            session_.Parts.RemovePartModifiedHandler(_partModifiedHandler_);
            session_.Parts.RemovePartOpenedHandler(_partOpenedHandler_);
            session_.Parts.RemovePartOpenedHandler(_partRenamedHandler_);
            session_.Parts.RemovePartRenamedHandler(_partSavedAsHandler_);
            session_.Parts.RemovePartSavedAsHandler(_partSavedAsHandler_);
            session_.Parts.RemovePartSavedHandler(_partSavedHandler_);
            session_.Parts.RemoveWorkPartChangedHandler(_workPartChangedHandler_);
        }

        private void WorkPartChangedHandler_(BasePart part)
        {
            print_($"__work_part_ changed from {part.Leaf} to {__work_part_.Leaf}");
        }

        private void PartSavedHandler_(BasePart part)
        {
            print_($"Part saved {part.Leaf}");
        }

        private void PartSavedAsHandler_(BasePart part)
        {
            print_($"Part saved as {part.Leaf}");
        }

        private void PartRenamedHandler_(BasePart part)
        {
            print_($"Part renamed {part.Leaf}");
        }

        private void PartOpenedHandler_(BasePart part)
        {
            print_($"Part opened {part.Leaf}");
        }

        private void PartModifiedHandler_(BasePart part)
        {
            print_($"Part modifed {part.Leaf}");
        }

        private void PartCreatedHandler_(BasePart part)
        {
            print_($"Part created {part.Leaf}");
        }

        private void PartClosedHandler_(BasePart part)
        {
            print_($"Part closed {part.Leaf}");
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            print_("AfterCheck");
        }

        private void treeView1_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            print_("AfterCollapse");
        }

        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            print_("AfterExpand");
        }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            print_("AfterLabelEdit");
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            print_("AfterSelect");
        }

        private void treeView1_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            print_("BeforeCheck");
        }

        private void treeView1_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            print_("BeforeCollapse");
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            print_("BeforeExpand");
        }

        private void treeView1_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            print_("BeforeLabelEdit");
        }

        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            print_("BeforeSelect");
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            print_("NodeMouseClick");
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            print_("NodeMouseDoubleClick");
        }

        private void treeView1_NodeMouseHover(object sender, TreeNodeMouseHoverEventArgs e)
        {
            print_("NodeMouseHover");
        }

        public void Reset()
        {
            treeView1.Nodes.Clear();

            if(__display_part_ is null)
            {
                treeView1.Nodes.Add("There is no display part");
                return;
            }

            Feature[] features = __work_part_.Features.ToArray();

            if(features.Length > 0)
            {
                TreeNode feature_node = treeView1.Nodes.Add("Features");

                foreach (Feature feature in features)
                    feature_node.Nodes.Add($"{feature.GetFeatureName()}, {feature.FeatureType}");
            }
        }
    }
}
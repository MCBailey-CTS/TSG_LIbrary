using System.Windows.Forms;

namespace TSG_Library.Extensions
{
    public static partial class __Extensions_
    {
        #region TreeNode

        public static TreeNode __SetText(this TreeNode node, string text)
        {
            node.Text = text;
            return node;
        }

        #endregion
    }
}
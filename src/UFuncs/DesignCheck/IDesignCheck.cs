using System.Windows.Forms;
using NXOpen;

namespace TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities
{
    public interface IDesignCheck
    {
        bool PerformCheck(Part part, out TreeNode result_node);

        bool IsPartValidForCheck(Part part, out string message);

        //NXOpen.TaggedObject[] GetObjectsForCheck(NXOpen.Part part);

        //DCResult Check(NXOpen.TaggedObject obj, out string  message);

        //DCResult Check(NXOpen.TaggedObject obj, out TreeNode object_node);
    }

    public enum DCResult
    {
        pass,
        fail,
        ignore,
        exception
    }

    public interface IDesignCheckLinkedBody : IDesignCheck
    {
    }
}
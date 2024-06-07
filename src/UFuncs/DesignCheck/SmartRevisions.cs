using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using NXOpen;
using TSG_Library.Extensions;

namespace TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities
{
    [Obsolete]
    public class SmartRevisions : IDesignCheck
    {
        private readonly IDictionary<string, string> _dict;

        private readonly ISet<Part> _parts_opened;

        public SmartRevisions()
        {
            _parts_opened = new HashSet<Part>();

            _dict = new Dictionary<string, string>();

            foreach (var dir in Directory.GetDirectories("G:\\0Library", "*", SearchOption.TopDirectoryOnly))
            {
                if (dir == @"G:\0Library\DfsrPrivate")
                    continue;

                foreach (var file in Directory.GetFiles(dir, "Smart*.prt", SearchOption.AllDirectories))
                {
                    var leaf = Path.GetFileNameWithoutExtension(file);

                    _dict.Add(leaf, file);
                }
            }
        }

        public bool IsPartValidForCheck(Part part, out string message)
        {
            message = "";
            return true;
        }

        //public TreeNode PerformCheck(NXOpen.Part part)
        //{
        //    throw new NotImplementedException();
        //}

        public bool PerformCheck(Part part, out TreeNode result_node)
        {
            result_node = part.__TreeNode();
            return false;
        }

        public void Dispose()
        {
            foreach (var part in _parts_opened)
                part.Close(BasePart.CloseWholeTree.True, BasePart.CloseModified.CloseModified, null);
        }

        [Obsolete]
        public IEnumerable<TreeNode> Validate(Part part)
        {
            throw new NotImplementedException();
            //if (!part._IsPartDetail())
            //{
            //    yield return new DesignCheckResult(Result.Ignore, part, this, "Part is not a detail.");
            //    yield break;
            //}

            //if (!part.HasUserAttribute("LIBRARY", NXObject.AttributeType.String, -1))
            //{
            //    yield return new DesignCheckResult(Result.Ignore, part, this, "Part does not have a LIBRARY attribute.");
            //    yield break;
            //}

            //string detail_library_value = part.GetUserAttributeAsString("LIBRARY", NXObject.AttributeType.String, -1);

            //if (!detail_library_value.StartsWith("Smart"))
            //{
            //    yield return new DesignCheckResult(Result.Ignore, part, this, "Parts library attribute does not start with Smart.");
            //    yield break;
            //}

            //if (!part.HasUserAttribute("REVISION", NXObject.AttributeType.String, -1))
            //{
            //    yield return new DesignCheckResult(Result.Fail, part, this, "Part has LIBRARY attribute but does not contain a REVISION attribute.");
            //    yield break;
            //}

            //string detail_revision_value = part.GetUserAttributeAsString("REVISION", NXObject.AttributeType.String, -1);

            //string lib0_part_path = _dict[detail_library_value];

            //Part lib0_part = session_.find_or_open(lib0_part_path);

            //_parts_opened.Add(lib0_part);

            //string lib0_library_value = lib0_part.GetUserAttributeAsString("LIBRARY", NXObject.AttributeType.String, -1);

            //string lib0_revision_value = lib0_part.GetUserAttributeAsString("REVISION", NXObject.AttributeType.String, -1);

            //if (detail_revision_value == lib0_revision_value)
            //{
            //    yield return new DesignCheckResult(Result.Pass, part, this, $"Library: {detail_library_value}");
            //    yield break;
            //}

            //yield return new DesignCheckResult(Result.Fail, part, this, $"Detail: {detail_revision_value}", $"Library: {lib0_revision_value}");
            //yield break;
        }
    }
}
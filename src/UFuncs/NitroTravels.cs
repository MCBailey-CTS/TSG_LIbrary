using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NXOpen;
using TSG_Library.Attributes;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs
{
    [UFunc("nitro-travels")]
    //[RevisionLog("NitroTravels")]
    //[RevisionEntry("1.0", "2020", "02", "17")]
    //[Revision("1.0.1", "Revision Log Created for NX 11.")]
    //[RevisionEntry("11.1", "2023", "01", "09")]
    //[Revision("11.1.1", "Removed validation")]
    public class NitroTravels : _UFunc
    {
        public override void execute()
        {
            try
            {
                print_(ufunc_rev_name);

                IEnumerable<string> partFiles = Directory
                    .GetFiles(@"G:\0Library\NitroCylinders", "*.prt", SearchOption.AllDirectories)
                    .Select(Path.GetFileNameWithoutExtension)
                    .Select(t => t.ToUpper());

                ISet<string> hash = new HashSet<string>(partFiles);

                if (__display_part_.ComponentAssembly.RootComponent is null)
                {
                    print_("Couldn't display part doesn't have a root component");
                    return;
                }

                Part[] parts = __display_part_.ComponentAssembly.RootComponent
                    .__Descendants()
                    .Select(__c => __c.Prototype).OfType<Part>()
                    .OrderBy(p => p.Leaf).ToArray();

                foreach (Part part in parts)
                {
                    if (!part.HasUserAttribute("LIBRARY", NXObject.AttributeType.String, -1))
                        continue;

                    string att = part.GetUserAttributeAsString("LIBRARY", NXObject.AttributeType.String, -1).ToUpper();

                    if (!hash.Contains(att))
                        continue;

                    Expression expression = part.__FindExpressionOrNull("TRAVEL");

                    if (expression is null)
                        continue;

                    print_($"{part.Leaf}, Library: {att}, Travel: \"{expression.Value}\"");
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }
    }
}
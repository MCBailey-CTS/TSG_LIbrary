using NXOpen;
using System;
using System.Linq;
using TSG_Library.Attributes;
using TSG_Library.Extensions;

namespace TSG_Library.UFuncs
{
    [UFunc("clean-body-ref-set")]
    public class CleanBodyRefSet : _UFunc
    {
        public override void execute()
        {
            var selected_comps = Ui.Selection.SelectManyComponents();

            if (selected_comps.Length == 0)
                return;

            var selected_parts = selected_comps.Select(c => c.__Prototype())
                .Distinct()
                .ToArray();

            foreach(var part in selected_parts )
            {
                if (!part.__HasReferenceSet("BODY"))
                    continue;

                var refset = part.__ReferenceSets("BODY");

                var curves = refset.AskAllDirectMembers()
                    .OfType<Curve>()
                    .ToList();

                curves.FindAll(c => c.Layer == 1).ForEach(c => c.__Layer(3));

                refset.RemoveObjectsFromReferenceSet(curves.ToArray<NXObject>());

                var datum_planes = refset.AskAllDirectMembers()
                    .OfType<Curve>()
                    .ToList();

                curves.FindAll(c => c.Layer == 1).ForEach(c => c.__Layer(3));

                refset.RemoveObjectsFromReferenceSet(curves.ToArray<NXObject>());

                // only objects on layer 1.

                // move curves -> 3
                // datum csys -> 11
                // datum plane -> 10
                // sheet bodies -> 10
                // all other objects -. 70
            }
        }
    }
}
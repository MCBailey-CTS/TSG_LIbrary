using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Disposable;
using TSG_Library.Extensions;
using TSG_Library.Geom;
using TSG_Library.UFuncs.Mirror.Rules;
using TSG_Library.UFuncs.MirrorComponents.Features;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs.Mirror.Features
{
    public class MirrorChamfer : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "CHAMFER";


        public override void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict,
            Surface.Plane plane, Component originalComp)
        {
            print_("here1");

            Component component = (Component)dict[originalComp];
            originalFeature.Suppress();
            Feature feature = (Feature)dict[originalFeature];
            feature.Suppress();
            Part part = component.__Prototype();

            using (session_.__UsingDisplayPartReset())
            {
                __display_part_ = originalComp.__Prototype(); ;

                ApexRangeChamferBuilder chamferBuilder = originalFeature.__OwningPart().Features.DetailFeatureCollection.CreateApexRangeChamferBuilder((ApexRangeChamfer)originalFeature);

                using (new Destroyer(chamferBuilder))
                {
                    var collector = chamferBuilder.EdgeManager.EdgeChainSetList.GetContents()[0].Edges;

                    originalFeature.Suppress();

                    collector.GetRules(out var ru);

                    var edgDumb = (EdgeDumbRule)ru[0];

                    edgDumb.GetData(out var data);

                    var j = new List<NXOpen.Curve>();

                    foreach (var l in data)
                    {
                        j.Add(l.__ToCurve());
                    }

                    __display_part_ = originalComp.Parent.__Prototype();

                    print_("here0");


                    throw new System.InvalidOperationException("EXPECTED ERROR");
                    IList<SelectionIntentRule> list = new List<SelectionIntentRule>();
                    collector.GetRules(out SelectionIntentRule[] rules);
                    foreach (SelectionIntentRule originalRule in rules)
                        list.Add(BaseMirrorRule.MirrorRule(originalRule, originalFeature, plane, originalComp, dict));
                }
            }

            //__work_part_ = component.__Prototype();
            //Session.UndoMarkId featureEditMark = session_.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
            //EditWithRollbackManager editWithRollbackManager = __work_part_.Features.StartEditWithRollbackManager(feature, featureEditMark);
            //using (new Destroyer(editWithRollbackManager))
            //{
            //    chamferBuilder = __work_part_.Features.DetailFeatureCollection.CreateApexRangeChamferBuilder((ApexRangeChamfer)feature);

            //    using (new Destroyer(chamferBuilder))
            //    {
            //        var col = __work_part_.ScCollectors.CreateCollector();
            //        col.ReplaceRules(list.ToArray(), false);

            //        chamferBuilder.EdgeManager.EdgeChainSetList.GetContents()[0].Edges = col;

            //        //chamferBuilder..ReverseOffsets = !chamferBuilder.ReverseOffsets;
            //        //chamferBuilder.SmartCollector.ReplaceRules(list.ToArray(), false);
            //        chamferBuilder.CommitFeature();
            //    }
            //}
        }

    }
}
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Disposable;
using TSG_Library.Geom;
using static TSG_Library.Extensions;

namespace TSG_Library.UFuncs.UFuncUtilities.MirrorUtilities
{
    public class MirrorExtrude : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "EXTRUDE";

        public override void Mirror(
            Feature originalFeature,
            IDictionary<TaggedObject, TaggedObject> dict,
            Surface.Plane plane,
            Component originalComp)
        {
            var mirroredComp = (Component)dict[originalComp];

            var mirroredPart = mirroredComp._Prototype();

            var originalPart = originalComp._Prototype();

            originalFeature.Suppress();

            Point3d mirrorOrigin;

            Vector3d mirrorVector;

            var mirroredFeature = (Extrude)dict[originalFeature];

            _WorkPart = originalPart;

            var builder = originalPart.Features.CreateExtrudeBuilder(originalFeature);

            IList<SelectionIntentRule> mirrorRules = new List<SelectionIntentRule>();

            using (new Destroyer(builder))
            {
                mirrorOrigin = builder.Direction.Origin._MirrorMap(plane, originalComp, mirroredComp);

                mirrorVector = builder.Direction.Vector._MirrorMap(plane, originalComp, mirroredComp);

                builder.Section.GetSectionData(out var originalSectionDatas);

                foreach (var originalSectionData in originalSectionDatas)
                {
                    originalSectionData.GetRules(out var originalRules);

                    foreach (var originalRule in originalRules)
                        switch (originalRule)
                        {
                            case EdgeBoundaryRule originalEdgeBoundaryRule:

                                originalEdgeBoundaryRule.GetData(out var originalFaces);

                                IList<Face> mirrorFaces = new List<Face>();

                                foreach (var originalFace in originalFaces)
                                {
                                    if(!dict.ContainsKey(originalFace))
                                        throw new MirrorException("Extrude, dictionary did not contain FACE as a key.");

                                    var mirrorFace = (Face)dict[originalFace];

                                    mirrorFaces.Add(mirrorFace);
                                }

                                if(mirrorFaces.Count == 0)
                                    throw new MirrorException("Extrude: EdgeBoundaryRule did not have enough faces.");

                                mirrorRules.Add(mirroredComp._Prototype().ScRuleFactory
                                    .CreateRuleEdgeBoundary(mirrorFaces.ToArray()));

                                break;
                            default:
                                throw new MirrorException($"Unknown rule type for Extrude {originalRule.Type}");
                        }
                }
            }

            if(mirrorRules.Count == 0)
                throw new MirrorException("Did not have enough rules for a commit.");

            _WorkPart = mirroredComp._Prototype();

            builder = mirroredPart.Features.CreateExtrudeBuilder(mirroredFeature);

            using (new Destroyer(builder))
            {
                var point = mirroredComp._Prototype().Points.CreatePoint(mirrorOrigin);

                builder.Direction = mirroredComp._Prototype().Directions.CreateDirection(point, mirrorVector);

                builder.Section = mirroredComp._Prototype().Sections.CreateSection();

                builder.Section.AddToSection(mirrorRules.ToArray(), null, null, null, _Point3dOrigin,
                    Section.Mode.Create);

                builder.CommitFeature();
            }

            originalFeature.Unsuppress();

            mirroredFeature.Unsuppress();
        }
    }
}
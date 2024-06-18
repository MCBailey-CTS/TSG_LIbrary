using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Disposable;
using TSG_Library.Geom;
using static TSG_Library.Extensions.__Extensions_;

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
            Component mirroredComp = (Component)dict[originalComp];

            Part mirroredPart = mirroredComp.__Prototype();

            Part originalPart = originalComp.__Prototype();

            originalFeature.Suppress();

            Point3d mirrorOrigin;

            Vector3d mirrorVector;

            Extrude mirroredFeature = (Extrude)dict[originalFeature];

            _WorkPart = originalPart;

            ExtrudeBuilder builder = originalPart.Features.CreateExtrudeBuilder(originalFeature);

            IList<SelectionIntentRule> mirrorRules = new List<SelectionIntentRule>();

            using (new Destroyer(builder))
            {
                mirrorOrigin = builder.Direction.Origin.__MirrorMap(plane, originalComp, mirroredComp);

                mirrorVector = builder.Direction.Vector.__MirrorMap(plane, originalComp, mirroredComp);

                builder.Section.GetSectionData(out SectionData[] originalSectionDatas);

                foreach (SectionData originalSectionData in originalSectionDatas)
                {
                    originalSectionData.GetRules(out SelectionIntentRule[] originalRules);

                    foreach (SelectionIntentRule originalRule in originalRules)
                        switch (originalRule)
                        {
                            case EdgeBoundaryRule originalEdgeBoundaryRule:

                                originalEdgeBoundaryRule.GetData(out Face[] originalFaces);

                                IList<Face> mirrorFaces = new List<Face>();

                                foreach (Face originalFace in originalFaces)
                                {
                                    if (!dict.ContainsKey(originalFace))
                                        throw new MirrorException("Extrude, dictionary did not contain FACE as a key.");

                                    Face mirrorFace = (Face)dict[originalFace];

                                    mirrorFaces.Add(mirrorFace);
                                }

                                if (mirrorFaces.Count == 0)
                                    throw new MirrorException("Extrude: EdgeBoundaryRule did not have enough faces.");

                                mirrorRules.Add(mirroredComp.__Prototype().ScRuleFactory
                                    .CreateRuleEdgeBoundary(mirrorFaces.ToArray()));

                                break;
                            default:
                                throw new MirrorException($"Unknown rule type for Extrude {originalRule.Type}");
                        }
                }
            }

            if (mirrorRules.Count == 0)
                throw new MirrorException("Did not have enough rules for a commit.");

            _WorkPart = mirroredComp.__Prototype();

            builder = mirroredPart.Features.CreateExtrudeBuilder(mirroredFeature);

            using (new Destroyer(builder))
            {
                Point point = mirroredComp.__Prototype().Points.CreatePoint(mirrorOrigin);

                builder.Direction = mirroredComp.__Prototype().Directions.CreateDirection(point, mirrorVector);

                builder.Section = mirroredComp.__Prototype().Sections.CreateSection();

                builder.Section.AddToSection(mirrorRules.ToArray(), null, null, null, _Point3dOrigin,
                    Section.Mode.Create);

                builder.CommitFeature();
            }

            originalFeature.Unsuppress();

            mirroredFeature.Unsuppress();
        }
    }
}
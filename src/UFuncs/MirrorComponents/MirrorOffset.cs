using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs.UFuncUtilities.MirrorUtilities
{
    public class MirrorOffset : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "OFFSET";

        public override void Mirror(
            Feature originalFeature,
            IDictionary<TaggedObject, TaggedObject> dict,
            Surface.Plane plane,
            Component originalComp)
        {
            var mirrorFeature = (OffsetFace)dict[originalFeature];
            var mirrorComponent = (Component)dict[originalComp];
            var builder = originalComp.__Prototype().Features.CreateOffsetFaceBuilder(originalFeature);
            DisplayableObject[] originalObjects;

            using (session_.__UsingBuilderDestroyer(builder))
            {
                originalObjects = builder.FaceCollector.GetObjects().Cast<DisplayableObject>().ToArray();
            }

            _WorkPart = mirrorComponent.__Prototype();
            builder = mirrorComponent.__Prototype().Features.CreateOffsetFaceBuilder(mirrorFeature);

            using (session_.__UsingBuilderDestroyer(builder))
            {
                IList<SelectionIntentRule> mirrorRules = new List<SelectionIntentRule>();

                foreach (var originalObj in originalObjects)
                    switch (originalObj)
                    {
                        case Edge _:
                            throw new MirrorException("Can't mirror edge in offset face");
                        case Body _:
                            throw new MirrorException("Can't mirror body in offset face.");
                        case Face originalFace:
                            if(!dict.ContainsKey(originalFace))
                                throw new MirrorException("OffsetFace dictionary did not contain FACE as a key.");

                            var mirrorFace = (Face)dict[originalFace];

                            SelectionIntentRule mirrorRule = mirrorComponent.__Prototype().ScRuleFactory
                                .CreateRuleFaceDumb(new[] { mirrorFace });

                            mirrorRules.Add(mirrorRule);

                            break;
                        default:
                            throw new MirrorException(
                                $"Unknown object type \"{originalObj.GetType().Name}\" in offset face.");
                    }

                if(mirrorRules.Count == 0)
                    throw new MirrorException("Did not have enough SelectionIntentRules for OffsetFace commit.");

                builder.FaceCollector.ReplaceRules(mirrorRules.ToArray(), false);
                builder.Commit();
                originalFeature.Unsuppress();
                mirrorFeature.Unsuppress();
            }
        }
    }
}
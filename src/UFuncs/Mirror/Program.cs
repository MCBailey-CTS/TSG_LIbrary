using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public class Program
    {
        public static void Mirror208_3001(Surface.Plane plane, Component originalComp,
            IDictionary<TaggedObject, TaggedObject> dict)
        {
            Component component = (Component)dict[originalComp];
            try
            {
                IMirrorFeature[] source = new IMirrorFeature[9]
                {
                    new MirrorLinkedBody(),
                    new MirrorEdgeBlend(),
                    new MirrorChamfer(),
                    new MirrorSubtract(),
                    new MirrorIntersect(),
                    new MirrorUnite(),
                    new MirrorBlock(),
                    new MirrorExtractedBody(),
                    new MirrorExtrude()
                };
                Part part = originalComp.__Prototype();
                Part part2 = component.__Prototype();
                originalComp.__Prototype().Features.SuppressFeatures(originalComp.__Prototype().Features.GetFeatures());
                component.__Prototype().Features.SuppressFeatures(component.__Prototype().Features.GetFeatures());
                Body[] array = part.Bodies.ToArray();
                Body[] array2 = part2.Bodies.ToArray();
                for (int i = 0; i < array.Length; i++) dict.Add(array[i], array2[i]);

                foreach (Feature originalFeature in part.Features)
                    try
                    {
                        IMirrorFeature mirrorFeature =
                            source.SingleOrDefault(mirror => mirror.FeatureType == originalFeature.FeatureType);
                        if (mirrorFeature != null)
                        {
                            mirrorFeature.Mirror(originalFeature, dict, plane, originalComp);
                            continue;
                        }

                        print_("///////////////////////////");
                        print_("Unable to mirror:");
                        print_("Type: " + originalFeature.FeatureType);
                        print_("Name: " + originalFeature.GetFeatureName());
                        print_("Part: " + originalFeature.OwningPart.Leaf);
                        print_("///////////////////////////");
                    }
                    catch (MirrorException ex)
                    {
                        print_("////////////////////////////////////////////////////");
                        print_(part.Leaf);
                        print_("Unable to mirror " + originalFeature.GetFeatureName());
                        print_(ex.Message);
                        print_("////////////////////////////////////////////////////");
                    }

                __work_part_ = __display_part_;
                foreach (Part item in __display_part_.__DescendantParts())
                foreach (Expression expression3 in item.Expressions)
                {
                    if (expression3.Status != Expression.StatusOption.Broken) continue;

                    Expression[] array3 = expression3.__OwningPart().Expressions.ToArray();
                    Expression[] array4 = array3;
                    foreach (Expression expression2 in array4)
                        if (expression2.Tag != expression3.Tag && expression3.Name == expression2.RightHandSide)
                            item.Expressions.Delete(expression2);

                    item.Expressions.Delete(expression3);
                    print_("Deleted broken expression \"" + expression3.Name + "\" in part \"" +
                           expression3.__OwningPart().Leaf + "\"");
                }
            }
            catch (Exception ex2)
            {
                ex2.__PrintException();
            }
            finally
            {
                Session.UndoMarkId undoMarkId = session_.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                session_.UpdateManager.DoInterpartUpdate(undoMarkId);
                session_.UpdateManager.DoAssemblyConstraintsUpdate(undoMarkId);
            }
        }
    }
}
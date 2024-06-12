using System;
using System.Collections.Generic;
using System.Linq;
using CTS_Library;
using CTS_Library.Attributes;
using CTS_Library.Extensions;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public class Program 
    {
        [Singleton]
        public static Program StaticInstance { get; } = new Program();


        public static void Mirror208_3001(Surface.Plane plane, Component originalComp, IDictionary<TaggedObject, TaggedObject> dict)
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
                Part part = originalComp._Prototype();
                Part part2 = component._Prototype();
                originalComp._Prototype().Features.SuppressFeatures(originalComp._Prototype().Features.GetFeatures());
                component._Prototype().Features.SuppressFeatures(component._Prototype().Features.GetFeatures());
                Body[] array = part.Bodies.ToArray();
                Body[] array2 = part2.Bodies.ToArray();
                for (int i = 0; i < array.Length; i++)
                {
                    dict.Add(array[i], array2[i]);
                }

                foreach (Feature originalFeature in part.Features)
                {
                    try
                    {
                        IMirrorFeature mirrorFeature = source.SingleOrDefault((IMirrorFeature mirror) => mirror.FeatureType == originalFeature.FeatureType);
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
                }

                Globals._WorkPart = Globals._DisplayPart;
                foreach (Part item in Globals._DisplayPart._DescendantParts())
                {
                    foreach (Expression expression3 in item.Expressions)
                    {
                        if (expression3.Status != Expression.StatusOption.Broken)
                        {
                            continue;
                        }

                        Expression[] array3 = expression3._OwningPart().Expressions.ToArray();
                        Expression[] array4 = array3;
                        foreach (Expression expression2 in array4)
                        {
                            if (expression2.Tag != expression3.Tag && expression3.Name == expression2.RightHandSide)
                            {
                                item.Expressions.Delete(expression2);
                            }
                        }

                        item.Expressions.Delete(expression3);
                        print_("Deleted broken expression \"" + expression3.Name + "\" in part \"" + expression3._OwningPart().Leaf + "\"");
                    }
                }
            }
            catch (Exception ex2)
            {
                ex2._PrintException();
            }
            finally
            {
                Globals.UnSuppressDisplay();
                Session.UndoMarkId undoMarkId = Globals._Session.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                Globals._Session.UpdateManager.DoInterpartUpdate(undoMarkId);
                Globals._Session.UpdateManager.DoAssemblyConstraintsUpdate(undoMarkId);
            }
        }
    }



}
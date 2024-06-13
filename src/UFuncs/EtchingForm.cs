using System;
using System.Linq;
using MoreLinq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Attributes;
using TSG_Library.Utilities;
using static TSG_Library.Extensions.__Extensions_;
using static TSG_Library.UFuncs._UFunc;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_etching)]
    public partial class EtchingForm : _UFuncForm
    {
        public EtchingForm()
        {
            InitializeComponent();
        }

        private void btnSingleSelection_Click(object sender, EventArgs e)
        {
        }

        private void btnSmartSelection_Click(object sender, EventArgs e)
        {
            _ = AssemblyAutoDetailForm.CreateMaterialList();
        }


        public static void Etching1()
        {
            Component[] components = __display_part_.ComponentAssembly.RootComponent
                .__Descendants()
                .Where(__c => __c.__IsLoaded())
                .DistinctBy(__c => __c.DisplayName)
                .ToArray();

            foreach (Component __c in components)
                try
                {
                    Part __prototype = __c.__Prototype();

                    if (!__prototype.__HasDynamicBlock())
                        continue;

                    __work_part_ = __prototype;

                    GFolder __folder = GFolder.create(__work_part_.FullPath);

                    AddFastenersForm1.SetWcsToWorkPart();

                    session_.__SetDisplayToWork();

                    double expected_z = __display_part_.WCS.CoordinateSystem.Origin.Z;

                    foreach (TaggedObject __member in __c.__Members())
                    {
                        if (!(__member is Face __face))
                            continue;

                        if (!__face.__IsPlanar())
                            continue;

                        Point3d[] __edge_positions = __face.GetEdges().SelectMany(__e =>
                        {
                            __e.GetVertices(out Point3d vert0, out Point3d vert1);
                            return new[] { vert0, vert1 };
                        }).ToArray();

                        if (!__edge_positions.All(__pos => System.Math.Abs(expected_z - __pos.Z) < .001))
                            continue;

                        using (session_.__usingDisplayPartReset())
                        {
                            __display_part_ = (Part)__c.Prototype;

                            AddFastenersForm1.SetWcsToWorkPart();

                            Face __proto_face = (Face)__face.Prototype;

                            double z = __proto_face.__EdgePositions().Select(__p => __p.Z).First();

                            Point3d[] edge_pos = __proto_face.__EdgePositions().ToArray();

                            double average_x = edge_pos.Select(__p => __p.X).Average();
                            double average_y = edge_pos.Select(__p => __p.Y).Average();
                            double average_z = edge_pos.Select(__p => __p.Z).Average();

                            Point3d new_origin = new Point3d(average_x, average_y, z);
                            Matrix3x3 new_ori = __display_part_.WCS.__Orientation();

                            Text detail_text_feature = (Text)__display_part_.__CreateTextFeature(
                                $"{__display_part_.Leaf}",
                                new_origin,
                                new_ori,
                                .900d,
                                .400d,
                                "Arial",
                                TextBuilder.ScriptOptions.Oem);

#pragma warning disable CS0219 // Variable is assigned but its value is never used
                            Text materail_text_feature = null;
#pragma warning restore CS0219 // Variable is assigned but its value is never used

                            throw new NotImplementedException();

                            //if (__display_part_.try_user_attribute_as_string("MATERIAL", out string __value))
                            //    materail_text_feature = (NXOpen.Features.Text)__display_part_._CreateTextFeature(
                            //            $"{__value}",
                            //            new_origin - .5 * new_ori.y_vec,
                            //            new_ori,
                            //            .900d,
                            //            .400d,
                            //            "Arial",
                            //            NXOpen.Features.TextBuilder.ScriptOptions.Oem);


                            ////var extrude 


                            //NXOpen.NXObject[] group = detail_text_feature.GetEntities()
                            //    .OfType<NXOpen.Curve>()
                            //    .Concat(materail_text_feature.GetEntities())
                            //    .ToArray();

                            //Direction_ direction = __display_part_.create_direction(Position_.origin(), NXOpen.Vector3d.z_unit());

                            //NXOpen.Features.ExtrudeBuilder extrudeBuilder = TSG_Library.Extensions.__work_part_.Features.CreateExtrudeBuilder(null);
                            //extrudeBuilder.Section = TSG_Library.Extensions.__work_part_.Sections.CreateSection(0d, 0d, 0d);
                            //extrudeBuilder.BooleanOperation.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Create;
                            //extrudeBuilder.Section.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.OnlyCurves);
                            //extrudeBuilder.Section.AllowSelfIntersection(true);
                            //foreach (NXOpen.Curve curve in group)
                            //{
                            //    NXOpen.CurveDumbRule curveDumbRule = TSG_Library.Extensions.__work_part_.ScRuleFactory.CreateRuleBaseCurveDumb(new NXOpen.IBaseCurve[] { curve });
                            //    NXOpen.SelectionIntentRule[] rule = new NXOpen.SelectionIntentRule[] { curveDumbRule };
                            //    extrudeBuilder.Section.AddToSection(rule, curve, null, null, _Point3dOrigin, NXOpen.Section.Mode.Create, false);
                            //}

                            //extrudeBuilder.Limits.StartExtend.Value.RightHandSide = $"{.02}";
                            //extrudeBuilder.Limits.EndExtend.Value.RightHandSide = $"{-.02}";
                            //extrudeBuilder.Direction = direction;
                            //extrudeBuilder.ParentFeatureInternal = false;
                            //NXOpen.Features.Extrude feature = (NXOpen.Features.Extrude)extrudeBuilder.CommitFeature();
                            //extrudeBuilder.Destroy();

                            //session_.create.Subtract(
                            //    __proto_face.GetBody(),
                            //    feature.GetBodies()
                            //    .Select(__b => Snap.NX.Body.Wrap(__b.Tag))
                            //    .ToArray()
                            //    ).ToString();

                            //foreach (NXOpen.NXObject curve in group)
                            //    Snap.NX.Curve.Wrap(curve.Tag).Layer = 12;

                            //print_($"/////////////");
                            //print_("Created text feature...");
                            //print_(__display_part_.Leaf);
                            //print_($"{$"{__display_part_.GetUserAttributeAsString("MATERIAL", NXOpen.NXObject.AttributeType.String, -1)}"}");
                            //print_($"/////////////");
                        }
                    }
                    //break;
                }
                catch (Exception ex)
                {
                    ex.__PrintException(__c.DisplayName);
                }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Features;
using NXOpen.GeometricUtilities;
using NXOpen.UF;
using NXOpen.Utilities;
using TSG_Library.UFuncs;
using static TSG_Library.Extensions;

namespace TSG_Library.Utilities
{
    public static class ProfileBuilder
    {
        public static void Create(
            IEnumerable<Tuple<Vector3d, ISet<Curve>>> source,
            int layer,
            string expression,
            ProfileTrimAndForm.Offset padOffsetOut,
            bool isUpper,
            bool isTrimProfile,
            int color,
            bool createFeatureGroup)
        {
            var slugs = CreateSlugs(source, color).ToArray();
            var uniteTargetLayer = isTrimProfile
                ? layer + 5
                : isUpper
                    ? layer + 5
                    : layer + 4;
            var uniteTargets = CreateUniteTargets(slugs, color, uniteTargetLayer).ToArray();
            CreatePunches(uniteTargets, color, layer);
            foreach (var target in uniteTargets)
                //Body_[] toolBodies = target.Punches.SelectMany(o => new Extrude_(o.Tag).Bodies).ToArray();
                //session_.create.Unite(target.Unite.GetBodies()[0], toolBodies);
                throw new NotImplementedException();

            var pads = CreatePad(uniteTargets, color, layer + 6).ToArray();
            CreateOffsetPads(pads, expression, padOffsetOut);
            if(isTrimProfile)
            {
                var lowers = CreateLower(uniteTargets, color, layer + 4).ToArray();
                CreateOffsetPads(lowers, "b", padOffsetOut);
            }

            if(!createFeatureGroup) return;
            foreach (var target in uniteTargets)
            {
                var objects = new List<TaggedObject>();
                objects.AddRange(target.Slugs.Select(obj => obj.Slug));
                objects.Add(target.Unite);
                objects.Add(target.PadObj);
                objects.AddRange(target.Unite.GetAllChildren().Select(feature => (TaggedObject)feature));
                objects.AddRange(target.Unite.GetAllChildren().SelectMany(feature => feature.GetAllChildren())
                    .Select(feature => (TaggedObject)feature));
                CreateFeatureGroup("PTF", false, objects.Where(o => o != null).ToArray());
            }
        }

        public static FeatureGroup CreateFeatureGroup(string name, bool show, params TaggedObject[] objects)
        {
            var tagObjects = (from obj in objects select obj.Tag).ToArray();
            var hideResult = 1;
            if(show) hideResult = 0;
            UFSession.GetUFSession().Modl
                .CreateSetOfFeature(name, tagObjects, tagObjects.Length, hideResult, out var tag);
            return (FeatureGroup)NXObjectManager.Get(tag);
        }

        private static IEnumerable<Pad> CreateLower(UniteTarget[] uniteTargets, int color, int layer)
        {
            foreach (var unite in uniteTargets)
            {
                var linkedBody = WaveLinkObject(unite.Unite.GetBodies()[0], ExtractFaceBuilder.ParentPartType.WorkPart);
                linkedBody.SetName("Lower");
                linkedBody.GetBodies()[0].__Color(color);
                linkedBody.GetBodies()[0].__Layer(layer);
                yield return new Pad(linkedBody, unite);
            }
        }

        private static void CreateOffsetPads(Pad[] pads, string expression, ProfileTrimAndForm.Offset offsetDirection)
        {
            foreach (var pad in pads)
            {
                var offsetFaceBuilder = __work_part_.Features.CreateOffsetFaceBuilder(null);
                offsetFaceBuilder.Distance.RightHandSide = expression + "";
                offsetFaceBuilder.Direction = offsetDirection == ProfileTrimAndForm.Offset.In;
                var faceBodyRule1 = __work_part_.ScRuleFactory.CreateRuleFaceBody(pad.PadObj.GetBodies()[0]);
                var rules1 = new SelectionIntentRule[1];
                rules1[0] = faceBodyRule1;
                offsetFaceBuilder.FaceCollector.ReplaceRules(rules1, false);
                offsetFaceBuilder.Commit();
                offsetFaceBuilder.Destroy();
            }
        }

        public static ExtractFace WaveLinkObject(
            Body body,
            ExtractFaceBuilder.ParentPartType parentType = ExtractFaceBuilder.ParentPartType.OtherPart)
        {
            var builder = session_.Parts.Work.BaseFeatures.CreateWaveLinkBuilder(null);
            builder.ExtractFaceBuilder.ParentPart = ExtractFaceBuilder.ParentPartType.OtherPart;

            SelectionIntentRule[] rules = { session_.Parts.Work.ScRuleFactory.CreateRuleBodyDumb(new[] { body }) };

            builder.ExtractFaceBuilder.ExtractBodyCollector.ReplaceRules(rules, false);

            builder.Type = WaveLinkBuilder.Types.BodyLink;
            builder.WaveDatumBuilder.Associative = true;
            builder.ExtractFaceBuilder.ParentPart = parentType;

            var nXObject1 = builder.Commit();
            builder.Destroy();

            if(nXObject1 is null)
                throw new Exception("Didnt create anything");

            return (ExtractFace)nXObject1;
        }

        private static IList<Pad> CreatePad(IList<UniteTarget> uniteTargets, int color, int layer)
        {
            IList<Pad> pads = new List<Pad>();

            foreach (var unite in uniteTargets)
            {
                var body = unite.Unite.GetBodies()[0];
                var linkedBody = WaveLinkObject(body, ExtractFaceBuilder.ParentPartType.WorkPart);
                var pad = new Pad(linkedBody, unite);
                linkedBody.SetName("Pad");
                linkedBody.GetBodies()[0].__Layer(layer);
                linkedBody.GetBodies()[0].__Color(color);
                unite.SetPadObj(linkedBody);
                pads.Add(new Pad(pad.PadObj, unite));
            }

            return pads;
        }

        private static void CreatePunches(IList<UniteTarget> uniteTargets, int color, int layer)
        {
            foreach (var unite in uniteTargets)
            foreach (var slug in unite.Slugs)
                try
                {
                    var builder = __work_part_.Features.CreateExtrudeBuilder(null);
                    builder.Section = __work_part_.Sections.CreateSection(0, 0, 0);
                    builder.Limits.StartExtend.Value.RightHandSide = "-100";
                    builder.Section.SetAllowedEntityTypes(Section.AllowTypes.OnlyCurves);
                    var edgeBoundaryRule = __work_part_.ScRuleFactory.CreateRuleEdgeBoundary(new[] { slug.TopFace });
                    SelectionIntentRule[] selectionRule = { edgeBoundaryRule };
                    builder.Section.AddToSection(selectionRule, slug.TopFace, null, null, _Point3dOrigin,
                        Section.Mode.Create, false);
                    builder.Direction =
                        __work_part_.Directions.CreateDirection(slug.Origin, slug.Vector,
                            SmartObject.UpdateOption.AfterModeling);
                    var scCollector = __work_part_.ScCollectors.CreateCollector();
                    var rule = __work_part_.ScRuleFactory.CreateRuleFaceDumb(new[] { slug.TopFace });
                    scCollector.ReplaceRules(new SelectionIntentRule[] { rule }, false);
                    builder.Limits.EndExtend.TrimType = Extend.ExtendType.UntilExtended;
                    builder.Limits.EndExtend.Target = unite.TopMostFace;
                    builder.BooleanOperation.Type = BooleanOperation.BooleanType.Create;
                    builder.BooleanOperation.SetTargetBodies(new[] { slug.TopFace.GetBody() });
                    var feature = builder.CommitFeature();
                    builder.Destroy();
                    scCollector.Destroy();
                    var punch = (Extrude)feature;
                    //punch.Name = "Punch";
                    //punch.layer = layer;
                    //punch._SetDisplayColor(color);
                    //unite.AddPunch(punch);
                    throw new NotImplementedException();
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }
        }


        [Obsolete(nameof(NotImplementedException))]
        public static CartesianCoordinateSystem CreateCoordinateSystem(this Vector3d vector, Point3d origin)
        {
            //NXOpen.NXMatrix nMatrix = __work_part_.NXMatrices.Create(new Orientation_(vector));
            //NXOpen.UF.UFSession.GetUFSession().Csys.CreateTempCsys(origin.Array, nMatrix.Tag, out NXOpen.Tag tempTag);
            //return (NXOpen.CartesianCoordinateSystem)session_._GetTaggedObject(tempTag);
            throw new NotImplementedException();
        }

        private static IEnumerable<UniteTarget> CreateUniteTargets(IEnumerable<SlugObj> slugs, int color, int layer)
        {
            //ILookup<NXOpen.Vector3d, SlugObj> dictionary = slugs.ToLookup(slug => slug.Vector);


            //List<UniteTarget> uniteTargets = new List<UniteTarget>();
            //NXOpen.CoordinateSystem absCoord = NXOpen.Vector3d.AxisZ.CreateCoordinateSystem();
            //foreach (IGrouping<NXOpen.Vector3d, SlugObj> key in dictionary)
            //    try
            //    {
            //        NXOpen.CoordinateSystem tempCsys = dictionary[key.Key].First().Vector.CreateCoordinateSystem();
            //        List<Position_ > mappedPositions = dictionary[key.Key].SelectMany(obj => obj.Polyline)
            //            .SelectMany(curve => new[] { curve._StartPoint(), curve._EndPoint() })
            //            .Select(position => NXOpen.CartesianCoordinateSystem.__MapCsysToCsys(position, absCoord, tempCsys))
            //            .ToList();
            //        mappedPositions.AddRange(
            //            from curve in dictionary[key.Key].SelectMany(slugObj => slugObj.Polyline).OfType<NXOpen.Arc>()
            //            select NXOpen.CartesianCoordinateSystem.__MapCsysToCsys(curve._OriginCenter(), absCoord, tempCsys));

            //        double maxZValue = mappedPositions.Select(position => position.Z).Max() + 500;
            //        double minYValue = mappedPositions.Select(position => position.Y).Min() - 50;
            //        double maxYValue = mappedPositions.Select(position => position.Y).Max() + 50;
            //        double minXValue = mappedPositions.Select(position => position.X).Min() - 50;
            //        double maxXValue = mappedPositions.Select(position => position.X).Max() + 50;
            //        Position_ cornerBottomLeft = NXOpen.CartesianCoordinateSystem.__MapCsysToCsys(new Position_ (minXValue, minYValue, maxZValue), tempCsys, absCoord);
            //        Position_ cornerTopLeft = NXOpen.CartesianCoordinateSystem.__MapCsysToCsys(new Position_ (minXValue, maxYValue, maxZValue), tempCsys, absCoord);
            //        Position_ cornerBottomRight = NXOpen.CartesianCoordinateSystem.__MapCsysToCsys(new Position_ (maxXValue, minYValue, maxZValue), tempCsys, absCoord);
            //        Position_ cornerTopRight = NXOpen.CartesianCoordinateSystem.__MapCsysToCsys(new Position_ (maxXValue, maxYValue, maxZValue), tempCsys, absCoord);
            //        NXOpen.Line[] retainerLines = new NXOpen.Line[]
            //        {
            //            session_.create.Line(cornerBottomLeft, cornerBottomRight),
            //            session_.create.Line(cornerTopLeft, cornerTopRight),
            //            session_.create.Line(cornerTopRight, cornerBottomRight),
            //            session_.create.Line(cornerTopLeft, cornerBottomLeft)
            //        };

            //        NXOpen.Features.ExtrudeBuilder extrudeBuilder = dictionary[key.Key].First().Slug.OwningPart.Features.CreateExtrudeBuilder(dictionary[key.Key].First().Slug);

            //        retainerLines.ToList().ForEach(line1 => Snap.NX.Line.Wrap(line1.Tag).Layer = 15);
            //        NXOpen.Direction direction;
            //        direction = extrudeBuilder.Direction;
            //        extrudeBuilder.Destroy();
            //        Snap.NX.Extrude extrude = MakeExtrude(retainerLines, direction, "0", "50");
            //        extrude.NXOpenDisplayableObject._SetDisplayColor(color);
            //        extrude.Name = "UniteTarget";
            //        extrude.Layer = layer;
            //        uniteTargets.Add(new UniteTarget(extrude, dictionary[key.Key]));
            //    }
            //    catch (Exception ex)
            //    {
            //        ex._PrintException();
            //    }

            //return uniteTargets;

            throw new NotImplementedException();
        }


        private static IEnumerable<SlugObj> CreateSlugs(IEnumerable<Tuple<Vector3d, ISet<Curve>>> source, int color)
        {
            //Tuple<NXOpen.Curve[], NXOpen.Vector3d>[] grouping = source.ToArray();

            //foreach (Tuple<NXOpen.Vector3d, ISet<NXOpen.Curve>> grouping in source)
            //{
            //    NXOpen.Direction direction = __work_part_.Directions.CreateDirection(grouping.Item2.First()._StartPoint(), grouping.Item1, NXOpen.SmartObject.UpdateOption.AfterModeling);
            //    Extrude_ slug = MakeExtrude(grouping.Item2.Cast<NXOpen.Curve>().ToArray(), direction, "-e", "-em");
            //    //slug.NXOpenExtrude.GetBodies()[0]._SetDisplayColor();
            //    slug.Name = "SlugPTF";
            //    slug.NXOpenDisplayableObject._SetDisplayColor(color);
            //    yield return new SlugObj(slug, grouping.Item1, grouping.Item2);
            //}

            throw new NotImplementedException();
        }

        private static Extrude MakeExtrude(IEnumerable<Curve> group, Direction direction, string rightHandSide,
            string leftHandSide)
        {
            var extrudeBuilder = __work_part_.Features.CreateExtrudeBuilder(null);

            using (session_.__UsingBuilderDestroyer(extrudeBuilder))
            {
                extrudeBuilder.Section = __work_part_.Sections.CreateSection(0d, 0d, 0d);
                extrudeBuilder.BooleanOperation.Type = BooleanOperation.BooleanType.Create;
                extrudeBuilder.Section.SetAllowedEntityTypes(Section.AllowTypes.OnlyCurves);
                extrudeBuilder.Section.AllowSelfIntersection(true);
                foreach (var curve in group)
                {
                    var curveDumbRule = __work_part_.ScRuleFactory.CreateRuleBaseCurveDumb(new IBaseCurve[] { curve });
                    SelectionIntentRule[] rule = { curveDumbRule };
                    extrudeBuilder.Section.AddToSection(rule, curve, null, null, _Point3dOrigin, Section.Mode.Create,
                        false);
                }

                extrudeBuilder.Limits.StartExtend.Value.RightHandSide = rightHandSide;
                extrudeBuilder.Limits.EndExtend.Value.RightHandSide = leftHandSide;
                extrudeBuilder.Direction = direction;
                extrudeBuilder.ParentFeatureInternal = false;
                return (Extrude)extrudeBuilder.CommitFeature();
            }
        }
    }
}
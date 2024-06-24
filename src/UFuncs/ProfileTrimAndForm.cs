using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NXOpen;
using NXOpen.Features;
using NXOpen.GeometricUtilities;
using NXOpen.UF;
using NXOpen.Utilities;
using TSG_Library.Attributes;
using TSG_Library.Disposable;
using TSG_Library.Utilities;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs
{
    [UFunc(_UFunc.ufunc_profile_trim_and_form)]
    public partial class ProfileTrimAndForm : _UFuncForm
    {
        public enum Offset
        {
            Out,

            In
        }

        public ProfileTrimAndForm()
        {
            InitializeComponent();
        }

        private int Layer => int.Parse(cmbLayer.Text);

        private void MainForm_Load(object sender, EventArgs e)
        {
            Text = $"{AssemblyFileVersion} - ProfileTrimAndForm";
            grpForm.Enabled = true;
            new[] { rdoForm, rdoUpper, rdoInner, rdoAllCurves, rdoTrim }.ToList()
                .ForEach(button => button.Checked = true);
            rdoTrim.Checked = true;
            chkFeatureGroup.Checked = true;
            for (int i = 11; i < 242; i += 10) cmbLayer.Items.Add(i);
            cmbLayer.SelectedIndex = 0;
            cmbLayer.Text = @"11";
        }

        private void BtnExecute_Click(object sender, EventArgs e)
        {
            try
            {
                Hide();

                SelectCurveGroups selectCurves = new SelectCurveGroups();

                Tuple<Vector3d, ISet<Curve>>[] pairs = selectCurves.SelectCurveGroups1(rdoAllCurves.Checked, Layer)
                    .ToArray();

                if (pairs.Length == 0)
                    return;

                Create(pairs);


                Part display = Session.GetSession().Parts.Display;

                try
                {
                    const string lwr_prof_name = "LWR-PROFILE";
                    ReferenceSet lwrProfRefset = display.GetAllReferenceSets()
                        .SingleOrDefault(refset => refset.Name == lwr_prof_name);
                    if (lwrProfRefset is null)
                    {
                        lwrProfRefset = display.CreateReferenceSet();
                        lwrProfRefset.SetName(lwr_prof_name);
                    }

                    // offset face lower body
                    NXObject[] lowerObjects = display.Features
                        .GetFeatures()
                        .Where(feat => feat.Name.ToUpper() == "LOWER")
                        .OfType<ExtractFace>()
                        .Select(ext => ext.GetAllChildren()[0])
                        .OfType<OffsetFace>()
                        .SelectMany(off => off.GetBodies())
                        .OfType<NXObject>()
                        .ToArray();
                    lwrProfRefset.AddObjectsToReferenceSet(lowerObjects);
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }

                try
                {
                    const string upr_prof_name = "UPR-PROFILE";
                    ReferenceSet uprProfRefset = display.GetAllReferenceSets()
                        .SingleOrDefault(refset => refset.Name == upr_prof_name);
                    if (uprProfRefset is null)
                    {
                        uprProfRefset = display.CreateReferenceSet();
                        uprProfRefset.SetName(upr_prof_name);
                    }

                    // unite retainer body
                    NXObject[] uniteObjects = display.Features
                        .GetFeatures()
                        .Where(feat => feat.Name.ToUpper() == "UNITETARGET")
                        .OfType<Extrude>()
                        .SelectMany(ext => ext.GetBodies())
                        .OfType<NXObject>()
                        .ToArray();
                    uprProfRefset.AddObjectsToReferenceSet(uniteObjects);
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }

                try
                {
                    const string pad_prof_name = "PAD-PROFILE";
                    ReferenceSet padProfRefset = display.GetAllReferenceSets()
                        .SingleOrDefault(refset => refset.Name == pad_prof_name);
                    if (padProfRefset is null)
                    {
                        padProfRefset = display.CreateReferenceSet();
                        padProfRefset.SetName(pad_prof_name);
                    }

                    // offset face pad body
                    NXObject[] padObjects = display.Features
                        .GetFeatures()
                        .Where(feat => feat.Name.ToUpper() == "PAD")
                        .OfType<ExtractFace>()
                        .Select(ext => ext.GetAllChildren()[0])
                        .OfType<OffsetFace>()
                        .SelectMany(off => off.GetBodies())
                        .OfType<NXObject>()
                        .ToArray();
                    padProfRefset.AddObjectsToReferenceSet(padObjects);
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                UFSession.GetUFSession().Ui.SetStatus("Profile Trim and Form exited");
                Show();
            }
        }


        private void Create(IEnumerable<Tuple<Vector3d, ISet<Curve>>> pairs)
        {
            Tuple<Vector3d, ISet<Curve>>[] pairArray = pairs.ToArray();

            if (pairArray.Length == 0)
                return;

            int color = rdoTrim.Checked ? 186 : 42;

            string padOffset = rdoTrim.Checked ? "p" : ".5";

            Offset offsetDirection = rdoTrim.Checked || rdoOuter.Checked
                ? Offset.Out
                : Offset.In;

            ProfileBuilder.Create(pairArray,
                Layer,
                padOffset,
                offsetDirection,
                rdoUpper.Checked,
                rdoTrim.Checked,
                color,
                chkFeatureGroup.Checked);
        }

        private void RdoTrim_CheckedChanged(object sender, EventArgs e)
        {
            grpForm.Enabled = rdoForm.Checked;
        }

        public class SelectCurveGroups
        {
            private IDictionary<DatumAxis, ISet<Curve>> _dict;

            private IDictionary<Curve, int> _dictColors;

            private IDictionary<Curve, DatumAxis> _dictCurveAxis;

            private ISet<Curve> _selectedCurves;

            private int selectedObjects;

            private int Init_Proc(IntPtr select_, IntPtr user_data)
            {
                UFSession.GetUFSession().Ui.SetSelProcs(select_, Filter_Proc, null, user_data);

                return UFConstants.UF_UI_SEL_SUCCESS;
            }

            private void Selected(DatumAxis datumAxis)
            {
                if (!_dict.ContainsKey(datumAxis))
                    throw new ArgumentOutOfRangeException(nameof(datumAxis), "Unknown datum axis selected.");

                ISet<Curve> curves = _dict[datumAxis];

                _dict.Remove(datumAxis);

                DatumAxis newDatumAxis = datumAxis.__OwningPart()
                    .__CreateFixedDatumAxis(datumAxis.Origin, datumAxis.Direction.__Negate());

                _dict.Add(newDatumAxis, curves);

                session_.__DeleteObjects(datumAxis);

                foreach (Curve curve in curves)
                {
                    if (_dictCurveAxis.ContainsKey(curve))
                    {
                        _dictCurveAxis[curve] = newDatumAxis;
                        continue;
                    }

                    _dictCurveAxis.Add(curve, newDatumAxis);
                }

                UFSession.GetUFSession().Modl.Update();
            }


            private void Selected(Curve curve)
            {
                if (!curve.__IsClosed())
                {
                    curve.Unhighlight();
                    return;
                }

                if (_selectedCurves.Contains(curve))
                {
                    Deselected(curve);
                    return;
                }

                if (!(curve is Conic conic))
                    return;

                conic.GetOrientation(out Point3d center, out Vector3d xDirection, out Vector3d yDirection);
                Matrix3x3 orientation = xDirection.__ToMatrix3x3(yDirection);
                DatumAxis datumAxis = conic.__OwningPart().__CreateFixedDatumAxis(center, orientation.__AxisZ());
                _dict.Add(datumAxis, new HashSet<Curve>(new[] { conic }));
                selectedObjects++;
                _dictColors.Add(conic, conic.Color);
                _selectedCurves.Add(conic);
                conic.__Color(7);
                _dictCurveAxis.Add(conic, datumAxis);
            }

            [Obsolete(nameof(NotImplementedException))]
            private void Selected(Curve[] snap_curves)
            {
                if (_selectedCurves.Contains(snap_curves[0]))
                {
                    Deselected(snap_curves);
                    return;
                }

                if (snap_curves.Length < 3)
                    throw new InvalidOperationException("Selected a set of curves that has less than 3 curves.");

                Vector3d a_vec = snap_curves[0].__StartPoint().__Subtract(snap_curves[0].__EndPoint());
                Vector3d b_vec = snap_curves[1].__StartPoint().__Subtract(snap_curves[1].__EndPoint());
                Vector3d cross_vec = b_vec.__Cross(a_vec);

                bool __all_on_plane = true;

                for (int i = 2; i < snap_curves.Length; i++)
                {
                    Vector3d c_vec = snap_curves[i].__StartPoint().__Subtract(snap_curves[i].__EndPoint());

                    UFSession.GetUFSession().Vec3.Dot(cross_vec.__ToArray(), c_vec.__ToArray(), out double dot_product);

                    if (System.Math.Abs(dot_product) < 0.0001)
                        continue;

                    __all_on_plane = false;
                    break;
                }

                if (!__all_on_plane)
                    throw new InvalidOperationException("Selected curves do not all lie on the same plane");


                // NXOpen.Conic conic = curves.OfType<NXOpen.Conic>().FirstOrDefault();

                // this is where you need to determine if curves all lie on the same plane.


                // var all__EndPoint()s = curves.SelectMany(__c => new[] { __c._StartPoint(), __c._EndPoint() }).ToArray();

                // var plane =  session_.create.BoundedPlane(curves.Select(__c => Snap.NX.Curve.Wrap(__c.Tag)).ToArray());


                // NXOpen.UF.UFSession.GetUFSession().Vec3.Dot()

                // Snap.Vector.AxisX.

                // Snap.Vector.Cross()


                // session_.create.DatumPlane()

                // var origin = new double[3];

                // var vector = new double[3];

                // NXOpen.UF.UFSession.GetUFSession().Modl.AskDatumPlane.AskPlane(plane.Tag, origin, vector);

                // if (conic is null)
                //    return;

                // if (_selectedCurves.Contains(conic))
                // {
                //    Deselected(conic);

                //    return;

                // }

                // conic.GetOrientation(out NXOpen.Point3d center, out NXOpen.Vector3d xDirection, out NXOpen.Vector3d yDirection);

                // var orientation = xDirection._ToMatrix3x3(yDirection);

                // NXOpen.DatumAxis datumAxis = __work_part_.__CreateDatumAxis(snap_curves[0]._StartPoint(), cross_vec);

                // _dict.Add(datumAxis, new HashSet<NXOpen.Curve>(curves));

                // foreach (NXOpen.Curve curve in curves)
                // {
                //    selectedObjects++;

                //    _selectedCurves.Add(curve);

                //    _dictColors.Add(curve, curve.Color);

                //    curve._SetDisplayColor(7);

                //    _dictCurveAxis.Add(curve, datumAxis);


                // }
            }

            private void Deselected(Curve deselectedCurve)
            {
                deselectedCurve.__Color(_dictColors[deselectedCurve]);

                DatumAxis connectedDatumAxis = _dictCurveAxis[deselectedCurve];

                ISet<Curve> curveSet = _dict[connectedDatumAxis];

                foreach (Curve curve in curveSet)
                {
                    selectedObjects--;
                    curve.__Color(_dictColors[curve]);
                    _dictColors.Remove(curve);
                    _dictCurveAxis.Remove(curve);
                    _selectedCurves.Remove(curve);
                }

                session_.__DeleteObjects(connectedDatumAxis);

                _dict.Remove(connectedDatumAxis);

                UFSession.GetUFSession().Modl.Update();
            }


            private void Deselected(Curve[] deselectedCurves)
            {
                //deselectedCurve._SetDisplayColor(_dictColors[deselectedCurve]);

                DatumAxis connectedDatumAxis = _dictCurveAxis[deselectedCurves[0]];

                ISet<Curve> curveSet = _dict[connectedDatumAxis];

                foreach (Curve curve in curveSet)
                {
                    selectedObjects--;
                    curve.__Color(_dictColors[curve]);
                    _dictColors.Remove(curve);
                    _dictCurveAxis.Remove(curve);
                    _selectedCurves.Remove(curve);
                }

                session_.__DeleteObjects(connectedDatumAxis);

                _dict.Remove(connectedDatumAxis);
                UFSession.GetUFSession().Modl.Update();
            }

            private void Select(Curve curve)
            {
                ScCollector collector = curve.__OwningPart().ScCollectors.CreateCollector();
                try
                {
                    using (SelectionIntentRule rule = curve.__OwningPart().ScRuleFactory
                               .CreateRuleCurveChain(curve, curve, true, .01))
                    {
                        collector.ReplaceRules(new[] { rule }, false);

                        Curve[] collectedCurves = collector.GetObjects().Cast<Curve>().ToArray();

                        switch (collectedCurves.Length)
                        {
                            case 0:
                                throw new Exception();
                            case 1:
                                Selected(collectedCurves[0]);
                                return;
                            default:
#pragma warning disable CS0618 // Type or member is obsolete
                                Selected(collectedCurves);
#pragma warning restore CS0618 // Type or member is obsolete
                                return;
                        }
                    }
                }
                finally
                {
                    collector.Destroy();
                }
            }

            private int Filter_Proc(Tag _object, int[] type, IntPtr user_data, IntPtr select_)
            {
                switch (Session.GetSession().GetObjectManager().GetTaggedObject(_object))
                {
                    case Curve _:
                        return UFConstants.UF_UI_SEL_ACCEPT;
                    case DatumAxis datumAxis:
                        if (_dict.ContainsKey(datumAxis))
                            return UFConstants.UF_UI_SEL_ACCEPT;
                        return UFConstants.UF_UI_SEL_REJECT;
                    default:
                        return UFConstants.UF_UI_SEL_REJECT;
                }
            }

            public IEnumerable<Tuple<Vector3d, ISet<Curve>>> SelectCurveGroups1(bool preSelect, int layer)
            {
                _dict = new Dictionary<DatumAxis, ISet<Curve>>();

                _dictColors = new Dictionary<Curve, int>();

                _dictCurveAxis = new Dictionary<Curve, DatumAxis>();

                _selectedCurves = new HashSet<Curve>();

                if (preSelect)
                {
                    foreach (Curve curve in Session.GetSession().Parts.Work.Curves)
                    {
                        if (_selectedCurves.Contains(curve))
                            continue;

                        if (curve.Layer != layer)
                            continue;

                        if (curve.IsBlanked)
                            curve.Unblank();

                        if (curve is Spline)
                            continue;

                        Select(curve);
                    }

                    if (_selectedCurves.Count == 0)
                    {
                        print_($"Did not find any curves on layer \"{layer}\".");
                        yield break;
                    }
                }

                bool returnFag = false;

                using (new LockNX())
                {
                    bool continueFlag = true;

                    while (continueFlag)
                    {
                        double[] cursor = new double[3];
                        UFSession.GetUFSession().Ui.SelectWithSingleDialog(
                            $"{selectedObjects}",
                            $"{selectedObjects}",
                            UFConstants.UF_UI_SEL_SCOPE_ANY_IN_ASSEMBLY,
                            Init_Proc,
                            IntPtr.Zero,
                            out int response,
                            out Tag obj,
                            cursor,
                            out Tag view);

                        switch (response)
                        {
                            case UFConstants.UF_UI_OBJECT_SELECTED:
                                switch (Session.GetSession().GetObjectManager().GetTaggedObject(obj))
                                {
                                    case DatumAxis selectedDatumAxis:
                                        Selected(selectedDatumAxis);
                                        continue;
                                    case Curve curve:
                                        Select(curve);
                                        continue;
                                    default:
                                        continue;
                                }
                            case UFConstants.UF_UI_OK:
                                continueFlag = false;
                                returnFag = true;
                                continue;
                            case UFConstants.UF_UI_CANCEL:
                                continueFlag = false;
                                continue;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(response), $"Response: \"{response}\".");
                        }
                    }

                    foreach (KeyValuePair<DatumAxis, ISet<Curve>> pair in _dict)
                    {
                        DatumAxis datumAxis = pair.Key;

                        ISet<Curve> curveSet = pair.Value;

                        if (returnFag)
                            yield return new Tuple<Vector3d, ISet<Curve>>(datumAxis.Direction.__Negate(), curveSet);


                        session_.__DeleteObjects(datumAxis);

                        foreach (Curve curve in curveSet)
                            curve.__Color(_dictColors[curve]);
                    }

                    _dictColors = null;
                    _dict = null;
                    _dictCurveAxis = null;
                    _selectedCurves = null;
                }
            }
        }

        public struct Pad
        {
            public ExtractFace PadObj;

            public UniteTarget UniteTarget;

            public Pad(ExtractFace padObj, UniteTarget uniteTarget)
                : this()
            {
                PadObj = padObj;
                UniteTarget = uniteTarget;
            }
        }

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
                SlugObj[] slugs = CreateSlugs(source, color).ToArray();
                int uniteTargetLayer = isTrimProfile
                    ? layer + 5
                    : isUpper
                        ? layer + 5
                        : layer + 4;
                UniteTarget[] uniteTargets = CreateUniteTargets(slugs, color, uniteTargetLayer).ToArray();
                CreatePunches(uniteTargets, color, layer);
                foreach (UniteTarget target in uniteTargets)
                    //Body_[] toolBodies = target.Punches.SelectMany(o => new Extrude_(o.Tag).Bodies).ToArray();
                    //session_.create.Unite(target.Unite.GetBodies()[0], toolBodies);
                    throw new NotImplementedException();

                Pad[] pads = CreatePad(uniteTargets, color, layer + 6).ToArray();
                CreateOffsetPads(pads, expression, padOffsetOut);
                if (isTrimProfile)
                {
                    Pad[] lowers = CreateLower(uniteTargets, color, layer + 4).ToArray();
                    CreateOffsetPads(lowers, "b", padOffsetOut);
                }

                if (!createFeatureGroup) return;
                foreach (UniteTarget target in uniteTargets)
                {
                    List<TaggedObject> objects = new List<TaggedObject>();
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
                Tag[] tagObjects = (from obj in objects select obj.Tag).ToArray();
                int hideResult = 1;
                if (show) hideResult = 0;
                UFSession.GetUFSession().Modl
                    .CreateSetOfFeature(name, tagObjects, tagObjects.Length, hideResult, out Tag tag);
                return (FeatureGroup)NXObjectManager.Get(tag);
            }

            private static IEnumerable<Pad> CreateLower(UniteTarget[] uniteTargets, int color, int layer)
            {
                foreach (UniteTarget unite in uniteTargets)
                {
                    ExtractFace linkedBody =
                        WaveLinkObject(unite.Unite.GetBodies()[0], ExtractFaceBuilder.ParentPartType.WorkPart);
                    linkedBody.SetName("Lower");
                    linkedBody.GetBodies()[0].__Color(color);
                    linkedBody.GetBodies()[0].__Layer(layer);
                    yield return new Pad(linkedBody, unite);
                }
            }

            private static void CreateOffsetPads(Pad[] pads, string expression, ProfileTrimAndForm.Offset offsetDirection)
            {
                foreach (Pad pad in pads)
                {
                    OffsetFaceBuilder offsetFaceBuilder = __work_part_.Features.CreateOffsetFaceBuilder(null);
                    offsetFaceBuilder.Distance.RightHandSide = expression + "";
                    offsetFaceBuilder.Direction = offsetDirection == ProfileTrimAndForm.Offset.In;
                    FaceBodyRule faceBodyRule1 = __work_part_.ScRuleFactory.CreateRuleFaceBody(pad.PadObj.GetBodies()[0]);
                    SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
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
                WaveLinkBuilder builder = session_.Parts.Work.BaseFeatures.CreateWaveLinkBuilder(null);
                builder.ExtractFaceBuilder.ParentPart = ExtractFaceBuilder.ParentPartType.OtherPart;

                SelectionIntentRule[] rules = { session_.Parts.Work.ScRuleFactory.CreateRuleBodyDumb(new[] { body }) };

                builder.ExtractFaceBuilder.ExtractBodyCollector.ReplaceRules(rules, false);

                builder.Type = WaveLinkBuilder.Types.BodyLink;
                builder.WaveDatumBuilder.Associative = true;
                builder.ExtractFaceBuilder.ParentPart = parentType;

                NXObject nXObject1 = builder.Commit();
                builder.Destroy();

                if (nXObject1 is null)
                    throw new Exception("Didnt create anything");

                return (ExtractFace)nXObject1;
            }

            private static IList<Pad> CreatePad(IList<UniteTarget> uniteTargets, int color, int layer)
            {
                IList<Pad> pads = new List<Pad>();

                foreach (UniteTarget unite in uniteTargets)
                {
                    Body body = unite.Unite.GetBodies()[0];
                    ExtractFace linkedBody = WaveLinkObject(body, ExtractFaceBuilder.ParentPartType.WorkPart);
                    Pad pad = new Pad(linkedBody, unite);
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
                foreach (UniteTarget unite in uniteTargets)
                    foreach (SlugObj slug in unite.Slugs)
                        try
                        {
                            ExtrudeBuilder builder = __work_part_.Features.CreateExtrudeBuilder(null);
                            builder.Section = __work_part_.Sections.CreateSection(0, 0, 0);
                            builder.Limits.StartExtend.Value.RightHandSide = "-100";
                            builder.Section.SetAllowedEntityTypes(Section.AllowTypes.OnlyCurves);
                            EdgeBoundaryRule edgeBoundaryRule =
                                __work_part_.ScRuleFactory.CreateRuleEdgeBoundary(new[] { slug.TopFace });
                            SelectionIntentRule[] selectionRule = { edgeBoundaryRule };
                            builder.Section.AddToSection(selectionRule, slug.TopFace, null, null, _Point3dOrigin,
                                Section.Mode.Create, false);
                            builder.Direction =
                                __work_part_.Directions.CreateDirection(slug.Origin, slug.Vector,
                                    SmartObject.UpdateOption.AfterModeling);
                            ScCollector scCollector = __work_part_.ScCollectors.CreateCollector();
                            FaceDumbRule rule = __work_part_.ScRuleFactory.CreateRuleFaceDumb(new[] { slug.TopFace });
                            scCollector.ReplaceRules(new SelectionIntentRule[] { rule }, false);
                            builder.Limits.EndExtend.TrimType = Extend.ExtendType.UntilExtended;
                            builder.Limits.EndExtend.Target = unite.TopMostFace;
                            builder.BooleanOperation.Type = BooleanOperation.BooleanType.Create;
                            builder.BooleanOperation.SetTargetBodies(new[] { slug.TopFace.GetBody() });
                            Feature feature = builder.CommitFeature();
                            builder.Destroy();
                            scCollector.Destroy();
                            Extrude punch = (Extrude)feature;
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
            public static CartesianCoordinateSystem CreateCoordinateSystem(Vector3d vector, Point3d origin)
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
                ExtrudeBuilder extrudeBuilder = __work_part_.Features.CreateExtrudeBuilder(null);

                using (session_.__UsingBuilderDestroyer(extrudeBuilder))
                {
                    extrudeBuilder.Section = __work_part_.Sections.CreateSection(0d, 0d, 0d);
                    extrudeBuilder.BooleanOperation.Type = BooleanOperation.BooleanType.Create;
                    extrudeBuilder.Section.SetAllowedEntityTypes(Section.AllowTypes.OnlyCurves);
                    extrudeBuilder.Section.AllowSelfIntersection(true);
                    foreach (Curve curve in group)
                    {
                        CurveDumbRule curveDumbRule =
                            __work_part_.ScRuleFactory.CreateRuleBaseCurveDumb(new IBaseCurve[] { curve });
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



        public struct SlugObj
        {
            public Extrude Slug { get; }

            public Vector3d Vector { get; }

            public ISet<Curve> Polyline { get; }

            public Face TopFace { get; }

            public Point3d Origin => Polyline.First().__StartPoint();

            [Obsolete(nameof(NotImplementedException))]
            public SlugObj(Extrude extrudeSp, Vector3d vector, ISet<Curve> polyline) : this()
            {
                extrudeSp.OwningPart.__AssertIsWorkPart();
                Slug = extrudeSp;
                Vector = vector;
                Polyline = polyline;

                Face[] validFaces = extrudeSp.GetFaces()
                    .Where(face => face.__IsPlanar())
                    .Where(face =>
                        vector.__IsEqualTo(face.__NormalVector()) || vector.__IsEqualTo(face.__NormalVector().__Negate()))
                    .ToArray();

                CoordinateSystem coordSystem = __work_part_.__CreateCsys(vector);
                CoordinateSystem abs = __work_part_.__CreateCsys(__Vector3dZ());

                TopFace = (Face)validFaces.MaxBy(face1 =>
                    face1.GetEdges().First().__StartPoint().__MapCsysToCsys(abs, coordSystem).Z);
                Face maxFace = null;
                double maxZ = double.MinValue;

                foreach (Face face1 in validFaces)
                {
                    double other = face1.GetEdges().First().__StartPoint().__MapCsysToCsys(abs, coordSystem).Z;

                    if (other > maxZ)
                        maxFace = face1;
                }

                TopFace = maxFace;
            }
        }

        public struct UniteTarget
        {
            public Extrude Unite { get; }

            public IEnumerable<SlugObj> Slugs { get; }

            private Vector3d Vector => Slugs.First().Vector;

            // ReSharper disable once MemberCanBeMadeStatic.Global
            public Face TopMostFace => throw
                //NXOpen.Vector3d unitizedVector = Vector.Unitize();
                //NXOpen.Face[] validFaces = Unite.GetFaces()
                //    .Where(face => face.SolidFaceType == NXOpen.Face.FaceType.Planar)
                //    .Where(face => unitizedVector._IsParallelTo(face._NormalVector()))
                //    .ToArray();
                //NXOpen.CartesianCoordinateSystem coordSystem =  unitizedVector.create_coordinate_system();
                //NXOpen.CartesianCoordinateSystem abs = NXOpen.Vector3d.z_unit().create_coordinate_system();
                //Face_ maxFace = null;
                //double maxZ = double.MinValue;
                //foreach (Face_ face1 in validFaces)
                //{
                //    double other = NXOpen.CartesianCoordinateSystem.__MapCsysToCsys(face1.edges.First().start_point, abs, coordSystem).z;
                //    if (other > maxZ)
                //        maxFace = face1;
                //}
                //return maxFace;
                new NotImplementedException();

            public TaggedObject PadObj { get; private set; }

            private readonly List<TaggedObject> _punches;

            public IEnumerable<TaggedObject> Punches => _punches;

            public void AddPunch(TaggedObject tagged)
            {
                _punches.Add(tagged);
            }

            public void SetPadObj(TaggedObject tagged)
            {
                PadObj = tagged;
            }

            public UniteTarget(Extrude extrude, IEnumerable<SlugObj> slugs) :
                this()
            {
                Unite = extrude;
                Slugs = slugs;
                _punches = new List<TaggedObject>();
            }
        }
    }
}
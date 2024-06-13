using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Features;
using NXOpen.UF;
using TSG_Library.Attributes;
using TSG_Library.Disposable;
using TSG_Library.Utilities;
using static TSG_Library.Extensions.__Extensions_;

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
    }
}
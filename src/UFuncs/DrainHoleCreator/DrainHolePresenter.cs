//using MoreLinq;

using System;
using System.Linq;
using NXOpen;
using NXOpen.Features;
using NXOpen.UF;

namespace TSG_Library.UFuncs.DrainHoleCreator
{
    public class DrainHolePresenter
    {
        private readonly IDrainHoleModel _model;
        private readonly IDrainHoleView _view;

        public DrainHolePresenter(IDrainHoleView view, IDrainHoleModel model)
        {
            _model = model;

            _view = view;

#pragma warning disable CS0612 // Type or member is obsolete
            _view.MainButtonClicked += RunDrainHoleOperation;
#pragma warning restore CS0612 // Type or member is obsolete

            _view.MidpointsCheckChanged += MidpointsCheckChanged;

            _view.CornersCheckChanged += CornersCheckChanged;

            _view.DiameterChanged += DiameterChanged;

            _view.UnitsChanged += UnitsChanged;

            _view.SubtractCheckChanged += SubtractCheckChanged;

            _view.StartLimitChanged += StartLimitChanged;

            _view.EndLimitChanged += EndLimitChanged;
        }

        private void StartLimitChanged(object sender, double startLimit)
        {
            _model.DrainHoleSettings.ExtrusionStartLimit = startLimit;
        }

        private void EndLimitChanged(object sender, double endLimit)
        {
            _model.DrainHoleSettings.ExtrusionEndLimit = endLimit;
        }

        private void MidpointsCheckChanged(object sender, bool boolean)
        {
            _model.DrainHoleSettings.MidPoints = boolean;
        }

        private void CornersCheckChanged(object sender, bool boolean)
        {
            _model.DrainHoleSettings.Corners = boolean;
        }

        private void SubtractCheckChanged(object sender, bool boolean)
        {
            _model.DrainHoleSettings.SubtractHoles = boolean;
        }

        private void DiameterChanged(object sender, double diameter)
        {
            _model.DrainHoleSettings.Diameter = diameter;
        }

        private void UnitsChanged(object sender, Units units)
        {
            _model.DrainHoleSettings.DiameterUnits = units;
        }

        [Obsolete]
        private void RunDrainHoleOperation(object sender, EventArgs e)
        {
            //try
            //{
            //    // todo: Add {SetPrompt} information.

            //    int counter = 1;

            //    while (true)
            //    {
            //        // Prompts the user to select a face and get a result.
            //        Snap.UI.Selection.Result result = _model.DrainHoleCreator.SelectFaceAndGetResult();

            //        // If the {result.Response} equals {Back} or {Cancel} then we can return.
            //        if (result.Response == Snap.UI.Response.Cancel || result.Response == Snap.UI.Response.Back) return;

            //        // If the {result.Objects} is null or doesn't contain anything, then the user middle clicked or clicked the {Ok} button without selecting anything.
            //        // Therefore we can just return.
            //        if (result.Objects == null || result.Objects.Length == 0) return;

            //        // Creates an undo.
            //        TSG_Library.Extensions.SetUndoMark(TSG_Library.Extensions.MarkVisibility.Visible, $"Drain Hole {counter++}");

            //        // Creates the curves to be used for creating the extrusion.
            //        NXOpen.Curve[] curves = _model.DrainHoleCreator.CreateExtrusionCurves(result, _model.DrainHoleSettings);

            //        // We are expecting there to only be 1 curve created.
            //        if (curves.Length != 1)
            //            throw new Exception("There was more than one curve created.");

            //        // Creates the extrusion from the {curves}.
            //        NXOpen.Features.Extrude extrusion = _model.DrainHoleCreator.CreateExtrusionCore(result, curves, _model.DrainHoleSettings);

            //        // Adds the {extrusion} to the the feature group.
            //        //   AddToFeatureGroup(__work_part_, _model.DrainHoleSettings.FeatureGroupName, extrusion);

            //        NXOpen.Features.BooleanFeature subtractionFeature = null;

            //        if (_model.DrainHoleSettings.SubtractHoles)
            //        {
            //            subtractionFeature = _model.DrainHoleCreator.CreateSubtraction(result, extrusion, _model.DrainHoleSettings);

            //            // Adds the {extrusion} to the the feature group.
            //            // AddToFeatureGroup(__work_part_, _model.DrainHoleSettings.FeatureGroupName, subtractionFeature);
            //        }

            //        _model.DrainHoleCreator.CleanUp(result, curves, extrusion, subtractionFeature, _model.DrainHoleSettings);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ex._PrintException();
            //}
            throw new NotImplementedException();
        }


        // ReSharper disable once UnusedMember.Local
        private static void AddToFeatureGroup(Part owningPart, string featureGroupName, Feature feature)
        {
            // Attempts to get a feature group with the given {featureGroupName} from the {owningPart}.
            var featureGroup = owningPart.Features
                .OfType<FeatureGroup>()
                .SingleOrDefault(group => group.Name == featureGroupName);

            // If {featureGroup is not null, then add the {feature}.
            if(featureGroup != null)
            {
                featureGroup.AddMembersWithRelocation(new[] { feature }, false, false);
                return;
            }


            // If {featureGroup} is null, then we need to create it and then return it.
            UFSession.GetUFSession().Modl
                .CreateSetOfFeature(featureGroupName, new[] { feature.Tag }, 1, UFConstants.TRUE, out _);
        }
    }
}
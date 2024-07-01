using NXOpen;
using NXOpen.Features;
using TSG_Library.Ui;

namespace TSG_Library.UFuncs.DrainHoleCreator
{
    public interface IDrainHoleCreator
    {
        Result SelectFaceAndGetResult();


        Curve[] CreateExtrusionCurves
        (
            Result result,
            IDrainHoleSettings drainHoleSettings
        );


        Extrude CreateExtrusionCore
        (
            Result result,
            Curve[] curves,
            IDrainHoleSettings drainHoleSettings
        );


        BooleanFeature CreateSubtraction
        (
            Result result,
            Extrude extrusionCore,
            IDrainHoleSettings drainHoleSettings
        );


        void CleanUp
        (
            // ReSharper disable once ParameterTypeCanBeEnumerable.Global
            Result result,
            Curve[] curves,
            Extrude extrusionCore,
            BooleanFeature subtraction,
            IDrainHoleSettings drainHoleSettings
        );
    }
}
namespace TSG_Library.UFuncs.DrainHoleCreator
{
    public interface IDrainHoleSettings
    {
        double Diameter { get; set; }
        Units DiameterUnits { get; set; }
        bool SubtractHoles { get; set; }
        bool Corners { get; set; }
        bool MidPoints { get; set; }
        int CurveLayer { get; }
        int CurveColor { get; }
        string ExtrusionName { get; }
        string SubtractionName { get; }
        string CurveName { get; }
        int ExtrusionLayer { get; }
        int SubtractionLayer { get; }
        double ExtrusionStartLimit { get; set; }
        double ExtrusionEndLimit { get; set; }
        string FeatureGroupName { get; }
    }
}
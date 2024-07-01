namespace TSG_Library.UFuncs.DrainHoleCreator
{
    public class DrainHoleSettings : IDrainHoleSettings
    {
        public double Diameter { get; set; } //= 2.0d;
        public Units DiameterUnits { get; set; } //= Units.Inches;
        public bool SubtractHoles { get; set; } //= true;
        public bool Corners { get; set; } //= true;
        public bool MidPoints { get; set; } //= true;
        public int CurveLayer { get; set; } //= -1;
        public int CurveColor { get; set; } //= -1;
        public string ExtrusionName { get; set; } //= string.Empty;
        public string SubtractionName { get; set; } // = string.Empty;
        public string CurveName { get; set; } //= string.Empty;
        public int ExtrusionLayer { get; set; } //= -1;
        public int SubtractionLayer { get; set; } //= -1;
        public double ExtrusionStartLimit { get; set; } // = .25 ;
        public double ExtrusionEndLimit { get; set; } //= -2.0 ;
        public string FeatureGroupName { get; set; }
    }
}
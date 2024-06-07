namespace TSG_Library.Enum
{
    //
    // Summary:
    //     Template types that can be used when creating a new part
    public enum Templates
    {
        //
        // Summary:
        //     A Part Modeling template, with a datum CSYS
        Modeling = 1,

        //
        // Summary:
        //     An Assembly Modeling template
        Assembly = 3,

        //
        // Summary:
        //     A Shape Studio template, with perspective and grid
        ShapeStudio = 2,

        //
        // Summary:
        //     A sheet metal template
        NXSheetMetal = 8,

        //
        // Summary:
        //     An aerospace sheet metal template
        AeroSheetMetal = 12,

        //
        // Summary:
        //     A Logical Routing template
        RoutingLogical = 7,

        //
        // Summary:
        //     A Mechanical Routing template
        RoutingMechanical = 6,

        //
        // Summary:
        //     An Electrical Routing template
        RoutingElectrical = 5,

        //
        // Summary:
        //     A template that creates a blank part file
        Blank = 4
    }
}
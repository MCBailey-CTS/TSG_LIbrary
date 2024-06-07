using System;
using System.Collections.Generic;
using NXOpen;
using NXOpen.Features;
using TSG_Library.Utilities;

namespace TSG_Library.UFuncs
{
    public interface IDrawDieComponent
    {
        GFolder FolderWithCtsNumber { get; }

        BasePart PartToAddTo { get; }

        double BlockLength { get; }

        double BlockWidth { get; }

        double BlockHeight { get; }


        Matrix3x3 BlockOrientation { get; }

        Point3d Origin { get; }

        int MinimumDetailNumber { get; }

        int BlockColor { get; }

        string MaterialValue { get; }

        IEnumerable<Tuple<ReferenceSet, Feature.BooleanType>> ReferenceSetBooleanPairs { get; }
    }
}
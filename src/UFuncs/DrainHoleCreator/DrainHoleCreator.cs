//using MoreLinq;

using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Features;
using NXOpen.UF;
using TSG_Library.Ui;
using static NXOpen.UF.UFConstants;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs.DrainHoleCreator
{
    public class DrainHoleCreator : IDrainHoleCreator
    {
        public Result SelectFaceAndGetResult()
        {
#pragma warning disable CS0612 // Type or member is obsolete
            return SelectSingleFaceWithResult();
#pragma warning restore CS0612 // Type or member is obsolete
        }

        [Obsolete]
        public Curve[] CreateExtrusionCurves(Result result, IDrainHoleSettings drainHoleSettings)
        {
            // Just make sure that at least {Corners} or {MidPoints} is set to true.
            // Throw exception if not.
            if(!drainHoleSettings.Corners && !drainHoleSettings.MidPoints)
                throw new InvalidOperationException(
                    $"Neither {nameof(drainHoleSettings.Corners)} nor {nameof(drainHoleSettings.MidPoints)} are set to true.");

            Face selectedFace = (Face)result.Objects[0];

            // Computes the intersection position between the {selectedFace} and the {cursorRay}.
            Compute.DistanceResult distanceResult = Compute.ClosestPoints(result.CursorRay, selectedFace);

            // Gets the intersection position.
            Point3d selectedPosition = distanceResult.Point1;

            // Gets the edges from the {selectedFace}.
            Edge[] selectedFaceEdges = selectedFace.GetEdges();

            // Linear edges.
            Edge[] linearEdges = selectedFaceEdges.Where(edge => edge.SolidEdgeType == Edge.EdgeType.Linear).ToArray();

            //NXOpen.Point3d minimumCornerPosition = MinBy(linearEdges
            //    .SelectMany(edge => new[] { edge._StartPoint(), edge._EndPoint() })
            //    .Distinct(new EqualityPos()), pos => selectedPosition.__Distance2(pos));

            //double minimumCornerDistance = selectedPosition.__Distance(minimumCornerPosition);

            //NXOpen.Point3d minimumMidPosition = MinBy(linearEdges
            //    .Select(edge => edge._StartPoint().__MidPoint(edge._EndPoint()))
            //    , pos => selectedPosition.__Distance2(pos));

            //double minimumMidpointDistance = selectedPosition.__Distance(minimumMidPosition);

            //if (drainHoleSettings.Corners && drainHoleSettings.MidPoints)
            //    return minimumCornerDistance < minimumMidpointDistance
            //        ? new[] { CornersOnly(selectedFace, minimumCornerPosition, drainHoleSettings.Diameter, drainHoleSettings.DiameterUnits, linearEdges) }
            //        : new[] { MidPointsOnly(selectedFace, selectedPosition, minimumMidPosition, drainHoleSettings.Diameter, drainHoleSettings.DiameterUnits, linearEdges) };

            //if (drainHoleSettings.Corners)
            //    return new[] { CornersOnly(selectedFace, minimumCornerPosition, drainHoleSettings.Diameter, drainHoleSettings.DiameterUnits, linearEdges) };

            //if (drainHoleSettings.MidPoints)
            //    return new[] { MidPointsOnly(selectedFace, selectedPosition, minimumMidPosition, drainHoleSettings.Diameter, drainHoleSettings.DiameterUnits, linearEdges) };

            //throw new InvalidProgramException("Bad corners and mid points.");

            throw new NotImplementedException();
        }

        [Obsolete]
        public Extrude CreateExtrusionCore(Result result, Curve[] curves, IDrainHoleSettings drainHoleSettings)
        {
            // For the drain creator, we are expecting there to only be one curve in {curves}.
            if(curves.Length != 1)
                throw new ArgumentOutOfRangeException(nameof(curves));

            // For the drain creator we are expecting the one curve to be a conic.
            if(!(curves[0] is Conic conic))
                throw new ArgumentException(@"The given curve was not a conic.", nameof(curves));

            // Gets the single face that we are expecting from the {result}.
            Face faceFromResult = (Face)result.Objects[0];

            // Gets the owning part of the {faceFromResult}.
            Part owningPart = faceFromResult.__OwningPart();

            // Creates the extrusion.
            //return owningPart._CreateExtrusionFromClosedConic(conic, drainHoleSettings.ExtrusionStartLimit, drainHoleSettings.ExtrusionEndLimit);
            throw new NotImplementedException();
        }

        public BooleanFeature CreateSubtraction(Result result, Extrude extrusionCore,
            IDrainHoleSettings drainHoleSettings)
        {
            // Gets the single face that we are expecting from the {result}.
            Face faceFromResult = (Face)result.Objects[0];

            // Gets the body that defines the {faceFromResult}.
            Body castingBody = faceFromResult.GetBody();

            // Gets the bodies that make up the {extrusionCore}.
            return castingBody.__Subtract(extrusionCore.GetBodies());
        }

        public void CleanUp(
            Result result,
            Curve[] curves,
            Extrude extrusionCore,
            BooleanFeature subtraction,
            IDrainHoleSettings drainHoleSettings)
        {
            foreach (Curve curve in curves)
            {
                curve.SetName(drainHoleSettings.CurveName);
                curve.Layer = drainHoleSettings.CurveLayer;
                curve.__Color(drainHoleSettings.CurveColor);
            }

            extrusionCore.SetName(drainHoleSettings.ExtrusionName);

            foreach (Body body in extrusionCore.GetBodies())
                body.Layer = drainHoleSettings.ExtrusionLayer;

            if(subtraction is null)
                return;

            foreach (Body body in subtraction.GetBodies())
                body.Layer = drainHoleSettings.SubtractionLayer;

            subtraction.SetName(drainHoleSettings.SubtractionName);
        }

        [Obsolete]
        public static Result SelectSingleFaceWithResult()
        {
            //const string selectFace = "Select Face";
            //Dialog selection = SelectObject(selectFace);
            //selection.AllowMultiple = false;
            //selection.Title = selectFace;
            //selection.IncludeFeatures = false;
            //selection.Scope = Dialog.SelectionScope.AnyInAssembly;
            //selection.KeepHighlighted = false;
            //selection.SetFilter(Snap.NX.ObjectTypes.Type.Face);
            //return selection.Show();
            throw new NotImplementedException();
        }

        public static TSource MinBy<TSource, TKey>(IEnumerable<TSource> source,
            Func<TSource, TKey> selector)
        {
            return MinBy(source, selector, null);
        }

        public static TSource MinBy<TSource, TKey>(IEnumerable<TSource> source,
            Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if(source == null) throw new ArgumentNullException(nameof(source));
            if(selector == null) throw new ArgumentNullException(nameof(selector));
            comparer = comparer ?? Comparer<TKey>.Default;

            using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
            {
                if(!sourceIterator.MoveNext()) throw new InvalidOperationException("Sequence contains no elements");

                TSource min = sourceIterator.Current;
                TKey minKey = selector(min);
                while (sourceIterator.MoveNext())
                {
                    TSource candidate = sourceIterator.Current;
                    TKey candidateProjected = selector(candidate);
                    if(comparer.Compare(candidateProjected, minKey) >= 0) continue;
                    min = candidate;
                    minKey = candidateProjected;
                }

                return min;
            }
        }

        private static Curve MidPointsOnly(
            Face selectedFace,
            Point3d selectedPosition,
            Point3d closestPosition,
            double diameter,
            Units units,
            params Edge[] linearEdges)
        {
            // Gets the owning part of the {selectedFace}.
            Part owningPart = selectedFace.__OwningPart();

            // Gets the edge whose mid point is equal to the {closestPosition}.
            Edge midpointEdge = linearEdges.Single(edge =>
                edge.__StartPoint().__MidPoint(edge.__EndPoint()).__IsEqualTo(closestPosition));

            Vector3d vec_edge = midpointEdge.__NormalVector();

            Matrix3x3 matrix = vec_edge.__ToMatrix3x3(selectedPosition.__Subtract(closestPosition));

            __display_part_.WCS.SetOriginAndMatrix(closestPosition, matrix);

            // Gets the mappedMidpoint.
            var outputCoords = new double[3];

            UFSession.GetUFSession().Csys.MapPoint(UF_CSYS_ROOT_COORDS, closestPosition.__ToArray(),
                UF_CSYS_ROOT_WCS_COORDS, outputCoords);

            Point3d mappedCenterPoint;

            if((Units)owningPart.PartUnits == units)
                // The {DiameterUnits} for this creator matches the units of the {owningPart}.
                mappedCenterPoint = outputCoords.__ToPoint3d().__Add(new Vector3d(0, diameter / 2, 0));

            else if(owningPart.PartUnits == BasePart.Units.Inches)
                mappedCenterPoint = outputCoords.__ToPoint3d().__Add(new Vector3d(0, diameter / 25.4 / 2, 0));
            else
                mappedCenterPoint = outputCoords.__ToPoint3d().__Add(new Vector3d(0, diameter * 25.4 / 2, 0));

            // Maps the {mappedCenterPoint} back to absolute coordinates to get the actual location of the center point.
            outputCoords = new double[3];

            UFSession.GetUFSession().Csys.MapPoint(UF_CSYS_ROOT_WCS_COORDS, mappedCenterPoint.__ToArray(),
                UF_CSYS_ROOT_COORDS, outputCoords);

            // Gets the normal of the {selectedFace}.
            Vector3d normalVector = selectedFace.__NormalVector();

            return CreateCircle(owningPart, outputCoords.__ToPoint3d(), normalVector, units, diameter);
        }

#pragma warning disable IDE0051 // Remove unused private members
        private static Curve CornersOnly(Face selectedFace, Point3d closestPosition, double diameter, Units units,
            params Edge[] linearEdges)
#pragma warning restore IDE0051 // Remove unused private members
        {
            //// Gets the edges that from {linearEdges} that has the {closestPosition} as one of it's end points.
            //NXOpen.Edge[] connectedEdges = linearEdges.Where(edge => edge._StartPoint()._IsEqualTo(closestPosition) || edge._EndPoint()._IsEqualTo(closestPosition)).ToArray();

            //// We are expecting exactly 2 {connectedEdges}.
            //if (connectedEdges.Length != 2)
            //    throw new InvalidOperationException($"Expecting closest corner to be attached to exactly 2 edges. It was actually attached to {connectedEdges.Length} edge(s).");

            //// Gets the owning part of the {selectedFace}.
            //NXOpen.Part owningPart = selectedFace.__OwningPart();

            //NXOpen.Curve[] offsetCurves = OffsetCurves(owningPart, diameter, units, connectedEdges);

            //Compute.IntersectionResult[] result = Compute.IntersectInfo(offsetCurves[0], offsetCurves[1]);

            //session_.delete_objects(offsetCurves);

            //if (result is null || result.Length == 0)
            //    throw new InvalidOperationException("The resulting offset did not have an intersection.");

            //// Gets the first intersection.
            //NXOpen.Point3d? position = result[0].Position;

            //if (position is null)
            //    throw new InvalidProgramException("The result did not have an intersection with the selected face.");

            //NXOpen.Point3d intersectionPosition = (NXOpen.Point3d)position;

            //// Gets the normal of the {selectedFace}.
            //NXOpen.Vector3d normalVector = selectedFace._NormalVector();

            //return CreateCircle(owningPart, intersectionPosition, normalVector, units, diameter);

            throw new NotImplementedException();
        }

        [Obsolete]
        private static Curve[] OffsetCurves(Part owningPart, double diameter, Units units, params Edge[] connectedEdges)
        {
            // Creates an offset curve builder.
            //NXOpen.Features.OffsetCurveBuilder builder = owningPart.Features.CreateOffsetCurveBuilder(null);

            // We need to match the units of the {builder} to be the units of the {owningPart}.
            //builder.OffsetDistance.Units = owningPart.PartUnits == session_.un NXOpen.BasePart.Units.Millimeters
            //    ? NXOpen.BasePart.Units.Millimeters
            //    : NXOpen.BasePart.Units.Inches;

            //using (new Destroyer(builder))
            //{
            //    if ((Units)owningPart.PartUnits == units)
            //        // The {DiameterUnits} for this creator matches the units of the {owningPart}.
            //        builder.OffsetDistance.RightHandSide = $"{diameter / 2}";
            //    else if (owningPart.PartUnits == NXOpen.BasePart.Units.Inches)
            //        // If we get here, then the {owningPart.PartUnits} is equal {Inches} and the {DiameterUnits} for this creator is equal to {Millimeters}.
            //        builder.OffsetDistance.RightHandSide = $"{diameter / 25.4 / 2}in";
            //    else
            //        // If we get here, then the {owningPart.PartUnits} is equal {Millimeters} and the {DiameterUnits} for this creator is equal to {Inches}.
            //        builder.OffsetDistance.RightHandSide = $"{diameter * 25.4 / 2}mm";

            //    builder.InputCurvesOptions.Associative = false;

            //    builder.InputCurvesOptions.InputCurveOption = NXOpen.GeometricUtilities.CurveOptions.InputCurve.Retain;

            //    builder.CurvesToOffset.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.OnlyCurves);

            //    builder.TrimMethod = NXOpen.Features.OffsetCurveBuilder.TrimOption.ExtendTangents;

            //    foreach (NXOpen.Edge edge in connectedEdges)
            //    {
            //        // We want to ignore edges that are arcs and closed.
            //        if (edge.SolidEdgeType == NXOpen.Edge.EdgeType.Circular && Snap.NX.Edge.Wrap(edge.Tag).IsClosed) continue;

            //        NXOpen.EdgeDumbRule edgeDumbRule = owningPart.ScRuleFactory.CreateRuleEdgeDumb(new[] { edge });

            //        NXOpen.SelectionIntentRule[] selectionIntentRules = new NXOpen.SelectionIntentRule[] { edgeDumbRule };

            //        builder.CurvesToOffset.AddToSection(selectionIntentRules, edge, null, null, NXOpen.Point3d.Origin, NXOpen.Section.Mode.Create, false);
            //    }

            //    builder.Commit();

            //    return builder.GetCommittedObjects().Select(nxObject => (NXOpen.Curve)nxObject).ToArray();
            //}

            throw new NotImplementedException();
        }

        private static Arc CreateCircle(Part owningPart, Point3d outputCoords, Vector3d normalVector, Units units,
            double diameter)
        {
            if((Units)owningPart.PartUnits == units)
                // The {DiameterUnits} for this creator matches the units of the {owningPart}.
                return owningPart.__CreateCircle(outputCoords, normalVector, diameter / 2 - .001);

            // If we get here, then the {owningPart.PartUnits} is equal {Inches} and the {DiameterUnits} for this creator is equal to {Millimeters}.
            return owningPart.PartUnits == BasePart.Units.Inches
                ? owningPart.__CreateCircle(outputCoords, normalVector, diameter / 25.4 / 2 - .001)

                // If we get here, then the {owningPart.PartUnits} is equal {Millimeters} and the {DiameterUnits} for this creator is equal to {Inches}.
                : owningPart.__CreateCircle(outputCoords, normalVector, diameter * 25.4 / 2 - .001);
        }
    }
}
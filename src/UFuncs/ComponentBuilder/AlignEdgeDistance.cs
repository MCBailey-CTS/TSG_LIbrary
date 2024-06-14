using System.Collections.Generic;
using NXOpen;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm
    {
        private double AlignEdgeDistanceNegZ(List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
            List<Line> allzAxisLines, double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            double distance = MapAndConvert(inputDist, mappedBase, mappedPoint, 2);



            foreach (Line zAxisLine in allzAxisLines) ZStartPoint(distance, zAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, "Z", false);
            return distance;
        }


        private double AlignEdgeDistancePosZ(List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
             List<Line> allzAxisLines, double inputDist, double[] mappedBase, double[] mappedPoint,int index, )
        {
            double distance = MapAndConvert(inputDist, mappedBase, mappedPoint, 2);


            foreach (Line zAxisLine in allzAxisLines) ZEndPoint(distance, zAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, "Z", false);
            return distance;
        }


        private double AlignEdgeDistanceNegY(List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
            List<Line> allyAxisLines, double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            double distance = MapAndConvert(inputDist, mappedBase, mappedPoint, 1);



            foreach (Line yAxisLine in allyAxisLines) YStartPoint(distance, yAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, "Y", false);
            return distance;
        }


        private double AlignEdgeDistancePosY(
            List<NXObject> movePtsHalf, 
            List<NXObject> movePtsFull,
            List<Line> allyAxisLines, 
            double inputDist, 
            double[] mappedBase, 
            double[] mappedPoint,
            int index,
            bool isPos)
        {
            double distance = MapAndConvert(inputDist, mappedBase, mappedPoint, 1);


            foreach (Line yAxisLine in allyAxisLines) YEndPoint(distance, yAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, "Y", false);
            return distance;
        }


        private double AlignEdgeDistanceNegX(List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
             List<Line> allxAxisLines, double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            double distance = MapAndConvert(inputDist, mappedBase, mappedPoint, 0);


            foreach (Line xAxisLine in allxAxisLines) XStartPoint(distance, xAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, "X", false);
            return distance;
        }


        private double AlignEdgeDistancePosX(List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
             List<Line> allxAxisLines, double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            double distance = MapAndConvert(inputDist, mappedBase, mappedPoint, 0);


            foreach (Line xAxisLine in allxAxisLines) XEndPoint(distance, xAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, "X", false);
            return distance;
        }
    }
}
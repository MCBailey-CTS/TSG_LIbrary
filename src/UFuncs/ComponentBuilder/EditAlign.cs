using System;
using System.Collections.Generic;
using NXOpen;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm
    {
        private double EditAlignNegZ(
            List<NXObject> movePtsHalf,
            List<NXObject> movePtsFull,
            List<Line> allzAxisLines,
            double[] mappedBase,
            double[] mappedPoint,
            int index,
            string dir_xyz)
        {
            double distance = Math.Abs(mappedPoint[index] - mappedBase[index]);

            if (mappedBase[index] < mappedPoint[index]) distance *= -1;

            foreach (Line zAxisLine in allzAxisLines)
                switch (dir_xyz)
                {
                    case "X":
                        XStartPoint(distance, zAxisLine);
                        continue;
                    case "Y":
                        YStartPoint(distance, zAxisLine);
                        continue;
                    case "Z":
                        ZStartPoint(distance, zAxisLine);
                        continue;
                    default:
                        throw new ArgumentException();
                }

            MoveObjects(movePtsHalf, movePtsFull, distance, dir_xyz, false);
            return distance;
        }

        //private double EditAlignPosZ(
        //  List<NXObject> movePtsHalf,
        //  List<NXObject> movePtsFull,
        //  List<Line> allzAxisLines,
        //  double[] mappedBase,
        //  double[] mappedPoint,
        //  int index,
        //  string dir_xyz)
        //{
        //    var distance = Math.Abs(mappedPoint[index] - mappedBase[index]);

        //    if (mappedBase[index] < mappedPoint[index]) distance *= -1;

        //    foreach (var zAxisLine in allzAxisLines)
        //        switch (dir_xyz)
        //        {
        //            case "X":
        //                XEndPoint(distance, zAxisLine);
        //                continue;
        //            case "Y":
        //                YEndPoint(distance, zAxisLine);
        //                continue;
        //            case "Z":
        //                ZEndPoint(distance, zAxisLine);
        //                continue;
        //            default:
        //                throw new ArgumentException();
        //        }

        //    MoveObjects(movePtsHalf, movePtsFull, distance, dir_xyz, false);
        //    return distance;
        //}

        private double EditAlignPosZ(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> allzAxisLines,
            double[] mappedBase, double[] mappedPoint, int index, string dir_xyz)
        {
            double distance = Math.Abs(mappedPoint[index] - mappedBase[index]);

            if (mappedBase[index] < mappedPoint[index])
                distance *= -1;

            foreach (Line zAxisLine in allzAxisLines)
                ZEndPoint(distance, zAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, dir_xyz, false);
            return distance;
        }

        private double EditAlignNegY(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> allyAxisLines,
            double[] mappedBase, double[] mappedPoint, int index, string dir_xyz)
        {
            double distance = Math.Abs(mappedPoint[index] - mappedBase[index]);

            if (mappedBase[index] < mappedPoint[index])
                distance *= -1;

            foreach (Line yAxisLine in allyAxisLines)
                YStartPoint(distance, yAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, dir_xyz, false);
            return distance;
        }

        private double EditAlignPosY(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> allyAxisLines,
            double[] mappedBase, double[] mappedPoint, int index, string dir_xyz)
        {
            double distance = Math.Abs(mappedPoint[index] - mappedBase[index]);

            if (mappedBase[index] < mappedPoint[index])
                distance *= -1;

            foreach (Line yAxisLine in allyAxisLines)
                YEndPoint(distance, yAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, dir_xyz, false);
            return distance;
        }


        private double EditAlignNegX(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> allxAxisLines,
            double[] mappedBase, double[] mappedPoint, int index, string dir_xyz)
        {
            double distance = Math.Abs(mappedPoint[index] - mappedBase[index]);

            if (mappedBase[index] < mappedPoint[index])
                distance *= -1;

            foreach (Line xAxisLine in allxAxisLines)
                XStartPoint(distance, xAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, dir_xyz, false);
            return distance;
        }


        private double EditAlignPosX(
            List<NXObject> movePtsHalf,
            List<NXObject> movePtsFull,
            List<Line> allxAxisLines,
            double[] mappedBase,
            double[] mappedPoint,
            int index,
            string dir_xyz)
        {
            double distance = Math.Abs(mappedPoint[index] - mappedBase[index]);

            if (mappedBase[index] < mappedPoint[index])
                distance *= -1;

            foreach (Line xAxisLine in allxAxisLines)
                XEndPoint(distance, xAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, dir_xyz, false);
            return distance;
        }
    }
}
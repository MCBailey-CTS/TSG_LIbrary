using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm
    {

        private double EditAlignNegZ(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negZObjs, List<Line> allzAxisLines, double[] mappedBase, double[] mappedPoint, int index)
        {
            var distance = Math.Abs(mappedPoint[index] - mappedBase[index]);

            if (mappedBase[index] < mappedPoint[index]) distance *= -1;

            foreach (var addLine in negZObjs) movePtsFull.Add(addLine);

            foreach (var zAxisLine in allzAxisLines)
            {
                ZStartPoint(distance, zAxisLine);
            }

            MoveObjectsZ(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private double EditAlignPosZ(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> allzAxisLines, double[] mappedBase, double[] mappedPoint, int index)
        {
            var distance = Math.Abs(mappedPoint[index] - mappedBase[index]);

            if (mappedBase[index] < mappedPoint[index]) 
                distance *= -1;

            foreach (var zAxisLine in allzAxisLines)
                ZEndPoint(distance, zAxisLine);

            MoveObjectsZ(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private double EditAlignNegY(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> allyAxisLines, double[] mappedBase, double[] mappedPoint, int index)
        {
            var distance = Math.Abs(mappedPoint[index] - mappedBase[index]);

            if (mappedBase[index] < mappedPoint[index]) 
                distance *= -1;

            foreach (var yAxisLine in allyAxisLines)
                YStartPoint(distance, yAxisLine);

            MoveObjectsY(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private double EditAlignPosY(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> allyAxisLines, double[] mappedBase, double[] mappedPoint, int index)
        {
            var distance = Math.Abs(mappedPoint[index] - mappedBase[index]);

            if (mappedBase[index] < mappedPoint[index]) 
                distance *= -1;

            foreach (var yAxisLine in allyAxisLines)
                YEndPoint(distance, yAxisLine);

            MoveObjectsY(movePtsHalf, movePtsFull, distance);
            return distance;
        }


        private double EditAlignNegX(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> allxAxisLines, double[] mappedBase, double[] mappedPoint, int index)
        {
            var distance = Math.Abs(mappedPoint[index] - mappedBase[index]);

            if (mappedBase[index] < mappedPoint[index])
                distance *= -1;

            foreach (var xAxisLine in allxAxisLines)
                XStartPoint(distance, xAxisLine);

            MoveObjectsX(movePtsHalf, movePtsFull, distance);
            return distance;
        }


        private double EditAlignPosX(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> allxAxisLines, double[] mappedBase, double[] mappedPoint, int index)
        {
            var distance = Math.Abs(mappedPoint[index] - mappedBase[index]);

            if (mappedBase[index] < mappedPoint[index])
                distance *= -1;

            foreach (var xAxisLine in allxAxisLines)
                XEndPoint(distance, xAxisLine);

            MoveObjectsX(movePtsHalf, movePtsFull, distance);
            return distance;
        }

    }
}

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

        private double EditAlignNegZ(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negZObjs, List<Line> allzAxisLines, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[2] - mappedBase[2]);
            if (mappedBase[2] < mappedPoint[2]) distance *= -1;

            foreach (var addLine in negZObjs) movePtsFull.Add(addLine);

            foreach (var zAxisLine in allzAxisLines)
            {
                NewMethod51(distance, zAxisLine);
            }

            NewMethod52(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private double EditAlignPosZ(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posZObjs, List<Line> allzAxisLines, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[2] - mappedBase[2]);
            if (mappedBase[2] < mappedPoint[2]) distance *= -1;

            foreach (var addLine in posZObjs) movePtsFull.Add(addLine);

            foreach (var zAxisLine in allzAxisLines)
            {
                NewMethod53(distance, zAxisLine);
            }

            NewMethod54(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private double EditAlignNegY(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negYObjs, List<Line> allyAxisLines, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[1] - mappedBase[1]);
            if (mappedBase[1] < mappedPoint[1]) distance *= -1;

            foreach (var addLine in negYObjs) movePtsFull.Add(addLine);

            foreach (var yAxisLine in allyAxisLines)
            {
                NewMethod55(distance, yAxisLine);
            }

            NewMethod56(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private double EditAlignPosY(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posYObjs, List<Line> allyAxisLines, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[1] - mappedBase[1]);
            if (mappedBase[1] < mappedPoint[1]) distance *= -1;

            foreach (var addLine in posYObjs) movePtsFull.Add(addLine);

            foreach (var yAxisLine in allyAxisLines)
            {
                var mappedEndPoint = MapAbsoluteToWcs(yAxisLine.EndPoint);
                var addY = new Point3d(mappedEndPoint.X,
                    mappedEndPoint.Y + distance, mappedEndPoint.Z);
                var mappedAddY = MapWcsToAbsolute(addY);
                yAxisLine.SetEndPoint(mappedAddY);
            }

            MoveObjects(movePtsFull.ToArray(), distance, "Y");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
            return distance;
        }

        private double EditAlignNegX(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negXObjs, List<Line> allxAxisLines, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[0] - mappedBase[0]);
            if (mappedBase[0] < mappedPoint[0]) distance *= -1;

            foreach (var addLine in negXObjs) movePtsFull.Add(addLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                NewMethod57(distance, xAxisLine);
            }

            NewMethod58(movePtsHalf, movePtsFull, distance);
            return distance;
        }


        private double EditAlignPosX(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posXObjs, List<Line> allxAxisLines, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[0] - mappedBase[0]);
            if (mappedBase[0] < mappedPoint[0]) distance *= -1;

            foreach (var posXLine in posXObjs) movePtsFull.Add(posXLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                NewMethod59(distance, xAxisLine);
            }

            NewMethod60(movePtsHalf, movePtsFull, distance);
            return distance;
        }

    }
}

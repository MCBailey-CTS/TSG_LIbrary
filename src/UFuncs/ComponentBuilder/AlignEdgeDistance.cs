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



        private double AlignEdgeDistanceNegZ(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negZObjs, List<Line> allzAxisLines, double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            var distance = NewMethod38(inputDist, mappedBase, mappedPoint);

            foreach (var addLine in negZObjs) movePtsFull.Add(addLine);

            foreach (var zAxisLine in allzAxisLines)
            {
                NewMethod39(distance, zAxisLine);
            }

            NewMethod40(movePtsHalf, movePtsFull, distance);
            return distance;
        }





        private double AlignEdgeDistancePosZ(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posZObjs, List<Line> allzAxisLines, double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            var distance = NewMethod35(inputDist, mappedBase, mappedPoint);

            foreach (var addLine in posZObjs) movePtsFull.Add(addLine);

            foreach (var zAxisLine in allzAxisLines)
            {
                NewMethod36(distance, zAxisLine);
            }

            NewMethod37(movePtsHalf, movePtsFull, distance);
            return distance;
        }





        private double AlignEdgeDistanceNegY(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negYObjs, List<Line> allyAxisLines, double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            var distance = NewMethod32(inputDist, mappedBase, mappedPoint);

            foreach (var addLine in negYObjs) movePtsFull.Add(addLine);

            foreach (var yAxisLine in allyAxisLines)
            {
                NewMethod33(distance, yAxisLine);
            }

            NewMethod34(movePtsHalf, movePtsFull, distance);
            return distance;
        }




        private double AlignEdgeDistancePosY(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posYObjs, List<Line> allyAxisLines, double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            var distance = NewMethod31(inputDist, mappedBase, mappedPoint);

            foreach (var addLine in posYObjs) movePtsFull.Add(addLine);

            foreach (var yAxisLine in allyAxisLines)
            {
                NewMethod30(distance, yAxisLine);
            }

            NewMethod29(movePtsHalf, movePtsFull, distance);
            return distance;
        }





        private double AlignEdgeDistanceNegX(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negXObjs, List<Line> allxAxisLines, double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            var distance = NewMethod28(inputDist, mappedBase, mappedPoint);

            foreach (var addLine in negXObjs) movePtsFull.Add(addLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                NewMethod26(distance, xAxisLine);
            }

            NewMethod27(movePtsHalf, movePtsFull, distance);
            return distance;
        }



        private double AlignEdgeDistancePosX(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posXObjs, List<Line> allxAxisLines, double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[0] - mappedBase[0]);
            distance = NewMethod75(inputDist, mappedBase, mappedPoint, distance);

            foreach (var posXLine in posXObjs) movePtsFull.Add(posXLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                NewMethod25(distance, xAxisLine);
            }

            NewMethod24(movePtsHalf, movePtsFull, distance);
            return distance;
        }
    }
}

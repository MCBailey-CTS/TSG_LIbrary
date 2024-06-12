using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm
    {

        private static double NewMethod38(double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[2] - mappedBase[2]);
            if (mappedBase[2] < mappedPoint[2])
            {
                distance *= -1;
                distance += inputDist;
            }
            else
            {
                distance -= inputDist;
            }

            return distance;
        }


        private static double NewMethod35(double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[2] - mappedBase[2]);
            if (mappedBase[2] < mappedPoint[2])
            {
                distance *= -1;
                distance += inputDist;
            }
            else
            {
                distance -= inputDist;
            }

            return distance;
        }



        private static double NewMethod32(double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[1] - mappedBase[1]);
            if (mappedBase[1] < mappedPoint[1])
            {
                distance *= -1;
                distance += inputDist;
            }
            else
            {
                distance -= inputDist;
            }

            return distance;
        }

        private static double NewMethod31(double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[1] - mappedBase[1]);
            if (mappedBase[1] < mappedPoint[1])
            {
                distance *= -1;
                distance += inputDist;
            }
            else
            {
                distance -= inputDist;
            }

            return distance;
        }





        private static double NewMethod28(double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[0] - mappedBase[0]);
            if (mappedBase[0] < mappedPoint[0])
            {
                distance *= -1;
                distance += inputDist;
            }
            else
            {
                distance -= inputDist;
            }

            return distance;
        }






        private static double NewMethod75(double inputDist, double[] mappedBase, double[] mappedPoint, double distance)
        {
            if (mappedBase[0] < mappedPoint[0])
            {
                distance *= -1;
                distance += inputDist;
            }
            else
            {
                distance -= inputDist;
            }

            return distance;
        }







    }
}

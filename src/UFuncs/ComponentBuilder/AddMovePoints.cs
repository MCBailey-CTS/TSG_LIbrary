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



        private static void EditSizePointsNeg(Point pointPrototype, List<NXObject> doNotMovePts, List<NXObject> movePtsHalf, List<NXObject> movePtsFull)
        {
            foreach (Point namedPt in _workPart.Points)
            {
                if (namedPt.Name == "")
                    continue;

                if (namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                    doNotMovePts.Add(namedPt);

                else if (namedPt.Name.Contains("Y") && pointPrototype.Name.Contains("Y"))
                    doNotMovePts.Add(namedPt);

                else if (namedPt.Name.Contains("Z") && pointPrototype.Name.Contains("Z"))
                    doNotMovePts.Add(namedPt);
                else if (namedPt.Name.Contains("BLKORIGIN"))
                    movePtsFull.Add(namedPt);
                else
                    movePtsHalf.Add(namedPt);
            }

            movePtsFull.Add(pointPrototype);
        }

        private static void EditSizePointsPos(Point pointPrototype, List<NXObject> doNotMovePts, List<NXObject> movePtsHalf, List<NXObject> movePtsFull)
        {
            foreach (Point namedPt in _workPart.Points)
            {
                if (namedPt.Name == "")
                    continue;

                if (namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                    doNotMovePts.Add(namedPt);

                else if (namedPt.Name.Contains("Y") && pointPrototype.Name.Contains("Y"))
                    doNotMovePts.Add(namedPt);

                else if (namedPt.Name.Contains("Z") && pointPrototype.Name.Contains("Z"))
                    doNotMovePts.Add(namedPt);
                else if (namedPt.Name.Contains("BLKORIGIN"))
                    doNotMovePts.Add(namedPt);
                else
                    movePtsHalf.Add(namedPt);
            }

            movePtsFull.Add(pointPrototype);
        }



        private static void NewMethod8(Point pointPrototype, List<NXObject> doNotMovePts, List<NXObject> movePtsHalf, List<NXObject> movePtsFull)
        {
            foreach (Point namedPt in _workPart.Points)
                if (namedPt.Name != "")
                {
                    if (namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Y") && pointPrototype.Name.Contains("Y"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Z") && pointPrototype.Name.Contains("Z"))
                        doNotMovePts.Add(namedPt);
                    else if (namedPt.Name.Contains("BLKORIGIN"))
                        movePtsFull.Add(namedPt);
                    else
                        movePtsHalf.Add(namedPt);
                }

            movePtsFull.Add(pointPrototype);
        }

        private static void NewMethod5(Point pointPrototype, List<NXObject> doNotMovePts, List<NXObject> movePtsHalf, List<NXObject> movePtsFull)
        {
            foreach (Point namedPt in _workPart.Points)
                if (namedPt.Name != "")
                {
                    if (namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Y") && pointPrototype.Name.Contains("Y"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Z") && pointPrototype.Name.Contains("Z"))
                        doNotMovePts.Add(namedPt);
                    else if (namedPt.Name.Contains("BLKORIGIN"))
                        doNotMovePts.Add(namedPt);
                    else
                        movePtsHalf.Add(namedPt);
                }

            movePtsFull.Add(pointPrototype);
        }


        private static void NewMethod3(Point pointPrototype, List<NXObject> doNotMovePts, List<NXObject> movePtsHalf, List<NXObject> movePtsFull)
        {
            foreach (Point namedPt in _workPart.Points)
                if (namedPt.Name != "")
                {
                    if (namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Y") &&
                            pointPrototype.Name.Contains("Y"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Z") &&
                            pointPrototype.Name.Contains("Z"))
                        doNotMovePts.Add(namedPt);
                    else if (namedPt.Name.Contains("BLKORIGIN"))
                        movePtsFull.Add(namedPt);
                    else
                        movePtsHalf.Add(namedPt);
                }

            movePtsFull.Add(pointPrototype);
        }

        private static void NewMethod1(Point pointPrototype, List<NXObject> doNotMovePts, List<NXObject> movePtsHalf, List<NXObject> movePtsFull)
        {
            foreach (Point namedPt in _workPart.Points)
                if (namedPt.Name != "")
                {
                    if (namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Y") &&
                            pointPrototype.Name.Contains("Y"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Z") &&
                            pointPrototype.Name.Contains("Z"))
                        doNotMovePts.Add(namedPt);
                    else if (namedPt.Name.Contains("BLKORIGIN"))
                        doNotMovePts.Add(namedPt);
                    else
                        movePtsHalf.Add(namedPt);
                }

            movePtsFull.Add(pointPrototype);
        }

        private static void AlignEdgeDistanceNeg(Point pointPrototype, List<NXObject> doNotMovePts, List<NXObject> movePtsHalf, List<NXObject> movePtsFull)
        {
            foreach (Point namedPt in _workPart.Points)
                if (namedPt.Name != "")
                {
                    if (namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Y") &&
                            pointPrototype.Name.Contains("Y"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Z") &&
                            pointPrototype.Name.Contains("Z"))
                        doNotMovePts.Add(namedPt);
                    else if (namedPt.Name.Contains("BLKORIGIN"))
                        movePtsFull.Add(namedPt);
                    else
                        movePtsHalf.Add(namedPt);
                }

            movePtsFull.Add(pointPrototype);
        }

        private static void AlignEdgeDistancePos(Point pointPrototype, List<NXObject> doNotMovePts, List<NXObject> movePtsHalf, List<NXObject> movePtsFull)
        {
            foreach (Point namedPt in _workPart.Points)
                if (namedPt.Name != "")
                {
                    if (namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Y") &&
                            pointPrototype.Name.Contains("Y"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Z") &&
                            pointPrototype.Name.Contains("Z"))
                        doNotMovePts.Add(namedPt);
                    else if (namedPt.Name.Contains("BLKORIGIN"))
                        doNotMovePts.Add(namedPt);
                    else
                        movePtsHalf.Add(namedPt);
                }

            movePtsFull.Add(pointPrototype);
        }



        private static void MotionCallbackDynamicNeg(Point pointPrototype, List<NXObject> doNotMovePts, List<NXObject> movePtsHalf, List<NXObject> movePtsFull)
        {
            foreach (Point namedPt in _workPart.Points)
                if (namedPt.Name != "")
                {
                    if (namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Y") && pointPrototype.Name.Contains("Y"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Z") && pointPrototype.Name.Contains("Z"))
                        doNotMovePts.Add(namedPt);
                    else if (namedPt.Name.Contains("BLKORIGIN"))
                        movePtsFull.Add(namedPt);
                    else
                        movePtsHalf.Add(namedPt);
                }

            movePtsFull.Add(pointPrototype);
        }

        private static void MotionCallbackDynamicPos(Point pointPrototype, List<NXObject> doNotMovePts, List<NXObject> movePtsHalf, List<NXObject> movePtsFull)
        {
            foreach (Point namedPt in _workPart.Points)
                if (namedPt.Name != "")
                {
                    if (namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Y") && pointPrototype.Name.Contains("Y"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Z") && pointPrototype.Name.Contains("Z"))
                        doNotMovePts.Add(namedPt);
                    else if (namedPt.Name.Contains("BLKORIGIN"))
                        doNotMovePts.Add(namedPt);
                    else
                        movePtsHalf.Add(namedPt);
                }

            movePtsFull.Add(pointPrototype);
        }









    }
}

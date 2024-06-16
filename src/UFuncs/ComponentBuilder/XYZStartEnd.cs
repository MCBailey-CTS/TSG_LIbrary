using NXOpen;
using TSG_Library.Extensions;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm
    {
        private void SetLineEndPoints(double distance, Line line, bool isStart, string dir_xyz)
        {
            Point3d point = isStart
                ? line.StartPoint
                : line.EndPoint;

            Point3d mappedPoint = MapAbsoluteToWcs(point);
            Point3d add;

            switch (dir_xyz)
            {
                case "X":
                    add = mappedPoint.__AddX(distance);
                    break;
                case "Y":
                    add = mappedPoint.__AddY(distance);
                    break;
                case "Z":
                    add = mappedPoint.__AddZ(distance);
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }

            Point3d mappedAddX = MapWcsToAbsolute(add);

            if (isStart)

                line.SetStartPoint(mappedAddX);
            else
                line.SetEndPoint(mappedAddX);
        }
    }
}
// 2045
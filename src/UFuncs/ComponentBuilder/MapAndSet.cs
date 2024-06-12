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
        private void NewMethod74(double distance, Line yAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(yAxisLine.StartPoint);
            var addY = new Point3d(mappedStartPoint.X,
                mappedStartPoint.Y + distance, mappedStartPoint.Z);
            var mappedAddY = MapWcsToAbsolute(addY);
            yAxisLine.SetStartPoint(mappedAddY);
        }

        private void NewMethod72(double distance, Line yAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(yAxisLine.EndPoint);
            var addY = new Point3d(mappedEndPoint.X, mappedEndPoint.Y + distance,
                mappedEndPoint.Z);
            var mappedAddY = MapWcsToAbsolute(addY);
            yAxisLine.SetEndPoint(mappedAddY);
        }

        private void NewMethod70(double distance, Line xAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(xAxisLine.EndPoint);
            var addX = new Point3d(mappedEndPoint.X + distance, mappedEndPoint.Y,
                mappedEndPoint.Z);
            var mappedAddX = MapWcsToAbsolute(addX);
            xAxisLine.SetEndPoint(mappedAddX);
        }

        private void NewMethod68(double distance, Line xAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(xAxisLine.StartPoint);
            var addX = new Point3d(mappedStartPoint.X + distance,
                mappedStartPoint.Y, mappedStartPoint.Z);
            var mappedAddX = MapWcsToAbsolute(addX);
            xAxisLine.SetStartPoint(mappedAddX);
        }

        private void NewMethod66(double distance, Line zAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(zAxisLine.StartPoint);
            var addZ = new Point3d(mappedStartPoint.X, mappedStartPoint.Y,
                mappedStartPoint.Z + distance);
            var mappedAddZ = MapWcsToAbsolute(addZ);
            zAxisLine.SetStartPoint(mappedAddZ);
        }

        private void NewMethod19(double xDistance, Line xAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(xAxisLine.StartPoint);
            var addX = new Point3d(mappedStartPoint.X + xDistance, mappedStartPoint.Y,
                mappedStartPoint.Z);
            var mappedAddX = MapWcsToAbsolute(addX);
            xAxisLine.SetStartPoint(mappedAddX);
        }

        private void NewMethod16(double yDistance, Line yAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(yAxisLine.StartPoint);
            var addY = new Point3d(mappedStartPoint.X, mappedStartPoint.Y + yDistance,
                mappedStartPoint.Z);
            var mappedAddY = MapWcsToAbsolute(addY);
            yAxisLine.SetStartPoint(mappedAddY);
        }

        private void NewMethod33(double distance, Line yAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(yAxisLine.StartPoint);
            var addY = new Point3d(mappedStartPoint.X,
                mappedStartPoint.Y + distance, mappedStartPoint.Z);
            var mappedAddY = MapWcsToAbsolute(addY);
            yAxisLine.SetStartPoint(mappedAddY);
        }

        private void NewMethod39(double distance, Line zAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(zAxisLine.StartPoint);
            var addZ = new Point3d(mappedStartPoint.X, mappedStartPoint.Y,
                mappedStartPoint.Z + distance);
            var mappedAddZ = MapWcsToAbsolute(addZ);
            zAxisLine.SetStartPoint(mappedAddZ);
        }

        private void NewMethod36(double distance, Line zAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(zAxisLine.EndPoint);
            var addZ = new Point3d(mappedEndPoint.X, mappedEndPoint.Y,
                mappedEndPoint.Z + distance);
            var mappedAddZ = MapWcsToAbsolute(addZ);
            zAxisLine.SetEndPoint(mappedAddZ);
        }

        private void NewMethod25(double distance, Line xAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(xAxisLine.EndPoint);
            var addX = new Point3d(mappedEndPoint.X + distance,
                mappedEndPoint.Y, mappedEndPoint.Z);
            var mappedAddX = MapWcsToAbsolute(addX);
            xAxisLine.SetEndPoint(mappedAddX);
        }

        private void NewMethod30(double distance, Line yAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(yAxisLine.EndPoint);
            var addY = new Point3d(mappedEndPoint.X,
                mappedEndPoint.Y + distance, mappedEndPoint.Z);
            var mappedAddY = MapWcsToAbsolute(addY);
            yAxisLine.SetEndPoint(mappedAddY);
        }

        private void NewMethod26(double distance, Line xAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(xAxisLine.StartPoint);
            var addX = new Point3d(mappedStartPoint.X + distance,
                mappedStartPoint.Y, mappedStartPoint.Z);
            var mappedAddX = MapWcsToAbsolute(addX);
            xAxisLine.SetStartPoint(mappedAddX);
        }

        private void NewMethod9(double zDistance, Line zAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(zAxisLine.StartPoint);
            var addZ = new Point3d(mappedStartPoint.X, mappedStartPoint.Y,
                mappedStartPoint.Z + zDistance);
            var mappedAddZ = MapWcsToAbsolute(addZ);
            zAxisLine.SetStartPoint(mappedAddZ);
        }

        private void NewMethod51(double distance, Line zAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(zAxisLine.StartPoint);
            var addZ = new Point3d(mappedStartPoint.X, mappedStartPoint.Y,
                mappedStartPoint.Z + distance);
            var mappedAddZ = MapWcsToAbsolute(addZ);
            zAxisLine.SetStartPoint(mappedAddZ);
        }

        private void NewMethod53(double distance, Line zAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(zAxisLine.EndPoint);
            var addZ = new Point3d(mappedEndPoint.X, mappedEndPoint.Y,
                mappedEndPoint.Z + distance);
            var mappedAddZ = MapWcsToAbsolute(addZ);
            zAxisLine.SetEndPoint(mappedAddZ);
        }

        private void NewMethod55(double distance, Line yAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(yAxisLine.StartPoint);
            var addY = new Point3d(mappedStartPoint.X,
                mappedStartPoint.Y + distance, mappedStartPoint.Z);
            var mappedAddY = MapWcsToAbsolute(addY);
            yAxisLine.SetStartPoint(mappedAddY);
        }

        private void NewMethod57(double distance, Line xAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(xAxisLine.StartPoint);
            var addX = new Point3d(mappedStartPoint.X + distance,
                mappedStartPoint.Y, mappedStartPoint.Z);
            var mappedAddX = MapWcsToAbsolute(addX);
            xAxisLine.SetStartPoint(mappedAddX);
        }

        private void NewMethod59(double distance, Line xAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(xAxisLine.EndPoint);
            var addX = new Point3d(mappedEndPoint.X + distance,
                mappedEndPoint.Y, mappedEndPoint.Z);
            var mappedAddX = MapWcsToAbsolute(addX);
            xAxisLine.SetEndPoint(mappedAddX);
        }

        private void NewMethod63(double distance, Line zAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(zAxisLine.EndPoint);
            var addZ = new Point3d(mappedEndPoint.X, mappedEndPoint.Y,
                mappedEndPoint.Z + distance);
            var mappedAddZ = MapWcsToAbsolute(addZ);
            zAxisLine.SetEndPoint(mappedAddZ);
        }

        private void NewMethod49(double distance, Line zAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(zAxisLine.StartPoint);
            var addZ = new Point3d(mappedStartPoint.X, mappedStartPoint.Y,
                mappedStartPoint.Z + distance);
            var mappedAddZ = MapWcsToAbsolute(addZ);
            zAxisLine.SetStartPoint(mappedAddZ);
        }

        private void NewMethod47(double distance, Line zAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(zAxisLine.EndPoint);
            var addZ = new Point3d(mappedEndPoint.X, mappedEndPoint.Y,
                mappedEndPoint.Z + distance);
            var mappedAddZ = MapWcsToAbsolute(addZ);
            zAxisLine.SetEndPoint(mappedAddZ);
        }

        private void NewMethod61(double distance, Line yAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(yAxisLine.StartPoint);
            var addY = new Point3d(mappedStartPoint.X,
                mappedStartPoint.Y + distance, mappedStartPoint.Z);
            var mappedAddY = MapWcsToAbsolute(addY);
            yAxisLine.SetStartPoint(mappedAddY);
        }

        private void NewMethod45(double distance, Line yAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(yAxisLine.EndPoint);
            var addY = new Point3d(mappedEndPoint.X,
                mappedEndPoint.Y + distance, mappedEndPoint.Z);
            var mappedAddY = MapWcsToAbsolute(addY);
            yAxisLine.SetEndPoint(mappedAddY);
        }

        private void NewMethod43(double distance, Line xAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(xAxisLine.StartPoint);
            var addX = new Point3d(mappedStartPoint.X + distance,
                mappedStartPoint.Y, mappedStartPoint.Z);
            var mappedAddX = MapWcsToAbsolute(addX);
            xAxisLine.SetStartPoint(mappedAddX);
        }

        private void NewMethod41(double distance, Line xAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(xAxisLine.EndPoint);
            var addX = new Point3d(mappedEndPoint.X + distance,
                mappedEndPoint.Y, mappedEndPoint.Z);
            var mappedAddX = MapWcsToAbsolute(addX);
            xAxisLine.SetEndPoint(mappedAddX);
        }

        private void NewMethod13(double zDistance, Line zAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(zAxisLine.EndPoint);
            var addZ = new Point3d(mappedEndPoint.X, mappedEndPoint.Y,
                mappedEndPoint.Z + zDistance);
            var mappedAddZ = MapWcsToAbsolute(addZ);
            zAxisLine.SetEndPoint(mappedAddZ);
        }

        private void NewMethod17(double yDistance, Line yAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(yAxisLine.EndPoint);
            var addY = new Point3d(mappedEndPoint.X, mappedEndPoint.Y + yDistance,
                mappedEndPoint.Z);
            var mappedAddY = MapWcsToAbsolute(addY);
            yAxisLine.SetEndPoint(mappedAddY);
        }

        private void NewMethod21(double xDistance, Line xAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(xAxisLine.EndPoint);
            var addX = new Point3d(mappedEndPoint.X + xDistance, mappedEndPoint.Y,
                mappedEndPoint.Z);
            var mappedAddX = MapWcsToAbsolute(addX);
            xAxisLine.SetEndPoint(mappedAddX);
        }
    }
}
using System;
using System.Collections;
using NXOpen;
using NXOpen.UF;

namespace TSG_Library.UFuncs
{
    public struct CtsDimensionData : IComparable
    {
        public CtsDimensionData(string objType, Tag objectTag, double x, double y, ExtremePointId extremeId) : this()
        {
            Type = objType;
            DimEntity = objectTag;
            DimXvalue = x;
            DimYvalue = y;
        }

        public string Type { get; set; }

        public Tag DimEntity { get; set; }

        public double DimXvalue { get; set; }

        public double DimYvalue { get; set; }

        public int ExtPointId { get; set; }


        int IComparable.CompareTo(object obj)
        {
            return string.CompareOrdinal(Type, ((CtsDimensionData)obj).Type);
        }

        private class SortXdescending_ : IComparer
        {
            int IComparer.Compare(object x, object x1)
            {
                if(x is null && x1 is null) return 0;
                if(x is null ^ x1 is null) return 1;
                if(((CtsDimensionData)x).DimXvalue > ((CtsDimensionData)x1).DimXvalue) return -1;
                return ((CtsDimensionData)x).DimXvalue < ((CtsDimensionData)x1).DimXvalue ? 1 : 0;
            }
        }

        private class SortYdescending_ : IComparer
        {
            public int Compare(object y, object y1)
            {
                if(y is null && y1 is null) return 0;
                if(y is null ^ y1 is null) return 1;
                if(((CtsDimensionData)y).DimYvalue > ((CtsDimensionData)y1).DimYvalue) return -1;
                return ((CtsDimensionData)y).DimYvalue < ((CtsDimensionData)y1).DimYvalue ? 1 : 0;
            }
        }

        public static IComparer SortXdescending()
        {
            return new SortXdescending_();
        }

        public static IComparer SortYdescending()
        {
            return new SortYdescending_();
        }

        public enum ExtremePointId
        {
            None,
            MinX,
            MaxX,
            MinY,
            MaxY,
            MinZ,
            MaxZ
        }

        public enum EndPointAssociativity
        {
            None = 0,
            FirstEndPoint = UFConstants.UF_DRF_first_end_point,
            LastEndPoint = UFConstants.UF_DRF_last_end_point
        }
    }
}
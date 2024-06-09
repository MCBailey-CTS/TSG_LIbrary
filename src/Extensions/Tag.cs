using NXOpen;

namespace TSG_Library.Extensions
{
    public static partial class __Extensions_
    {
        #region Tag

        public static TaggedObject __ToTaggedObject(this Tag tag)
        {
            return session_.GetObjectManager().GetTaggedObject(tag);
        }

        public static TSource __To<TSource>(this Tag tag) where TSource : TaggedObject
        {
            return (TSource)tag.__ToTaggedObject();
        }

        #endregion
    }
}
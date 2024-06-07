using NXOpen;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
        //public static NXOpen.TaggedObject _GetTaggedObject(this NXOpen.Session session, NXOpen.Tag tag)
        //{
        //    return session.GetObjectManager().GetTaggedObject(tag);
        //}


        public static TaggedObject _ToTaggedObject(this Tag tag)
        {
            return session_.GetObjectManager().GetTaggedObject(tag);
        }

        public static TSource _To<TSource>(this Tag tag) where TSource : TaggedObject
        {
            return (TSource)tag._ToTaggedObject();
        }
    }
}
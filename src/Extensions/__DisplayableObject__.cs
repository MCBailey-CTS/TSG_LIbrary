using System;
using NXOpen;
using TSG_Library.Attributes;
using TSG_Library.Disposable;
using static TSG_Library.Extensions.Extensions_;

namespace TSG_Library.Extensions
{
    [ExtensionsAspect]
    public static class __DisplayableObject__
    {
        public static IDisposable __UsingRedisplayObject(this DisplayableObject displayableObject)
        {
            return new RedisplayObject(displayableObject);
        }

        public static int __Color(this DisplayableObject obj)
        {
            return obj.Color;
        }

        public static void __Color(this DisplayableObject obj, int color, bool redisplayObj = true)
        {
            obj.Color = color;

            if(redisplayObj)
                obj.__RedisplayObject();
        }

        public static void __Translucency(this DisplayableObject obj, int translucency, bool redisplayObj = true)
        {
            ufsession_.Obj.SetTranslucency(obj.Tag, translucency);

            if(redisplayObj)
                obj.__RedisplayObject();
        }

        public static int __Layer(this DisplayableObject displayableObject)
        {
            return displayableObject.Layer;
        }

        public static void __Layer(this DisplayableObject displayableObject, int layer, bool redisplayObj = true)
        {
            displayableObject.Layer = layer;

            if(redisplayObj)
                displayableObject.__RedisplayObject();
        }

        public static void __RedisplayObject(this DisplayableObject obj)
        {
            obj.RedisplayObject();
        }

        public static void __Blank(this DisplayableObject obj)
        {
            obj.Blank();
        }

        public static void __Unblank(this DisplayableObject obj)
        {
            obj.Unblank();
        }

        public static void __Highlight(this DisplayableObject obj)
        {
            obj.Highlight();
        }


        public static void __Unhighlight(this DisplayableObject obj)
        {
            obj.Unhighlight();
        }

        public static bool __IsBlanked(DisplayableObject obj)
        {
            return obj.IsBlanked;
        }
    }
}
using System;
using NXOpen;

namespace TSG_Library.Disposable
{
    public class RedisplayObject : IDisposable
    {
        private readonly DisplayableObject displayableObject;

        public RedisplayObject(DisplayableObject displayableObject)
        {
            this.displayableObject = displayableObject;
        }

        public void Dispose()
        {
            displayableObject.RedisplayObject();
        }
    }
}
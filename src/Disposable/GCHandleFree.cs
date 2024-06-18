using System;
using System.Runtime.InteropServices;

namespace TSG_Library.Disposable
{
    public class GCHandleFree : IDisposable
    {
        private readonly GCHandle __handle;

        public GCHandleFree(GCHandle __handle)
        {
            this.__handle = __handle;
        }

        public void Dispose()
        {
            if (__handle.IsAllocated)
                __handle.Free();
        }
    }
}
using System;
using NXOpen.Assemblies;
using TSG_Library.Extensions;

namespace TSG_Library.Disposable
{
    public class ResetComponentReferenceSet : IDisposable
    {
        private readonly Component __component;

        private readonly string __reference_set;

        public ResetComponentReferenceSet(Component component)
        {
            __reference_set = component.ReferenceSet;
            __component = component;
        }

        public ResetComponentReferenceSet(Component component, string reference_set)
        {
            __reference_set = component.ReferenceSet;
            __component = component;
            __component._ReferenceSet(reference_set);
        }

        public void Dispose()
        {
            __component._ReferenceSet(__reference_set);
        }
    }
}
using System;
using NXOpen.Assemblies;

namespace TSG_Library.Exceptions
{
    public class ImproperReferenceSetException : Exception
    {
        public ImproperReferenceSetException(Component component, string refSet)
        {
        }
    }
}
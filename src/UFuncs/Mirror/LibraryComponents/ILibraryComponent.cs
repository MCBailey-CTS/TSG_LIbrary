using System.Collections.Generic;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;
using TSG_Library.UFuncs.Mirror.LibraryComponents;

namespace TSG_Library.UFuncs.Mirror.LibraryComponents
{
    public interface ILibraryComponent
    {
        bool IsLibraryComponent(Component component);

        void Mirror(Surface.Plane plane, Component mirroredComp, ExtractFace originalLinkedBody, Component fromComp,
            IDictionary<TaggedObject, TaggedObject> dict);
    }
}
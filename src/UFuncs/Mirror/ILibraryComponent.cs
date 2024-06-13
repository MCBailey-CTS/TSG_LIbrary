using System.Collections.Generic;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public interface ILibraryComponent
    {
        bool IsLibraryComponent(Component component);

        void Mirror(Surface.Plane plane, Component mirroredComp, ExtractFace originalLinkedBody, Component fromComp,
            IDictionary<TaggedObject, TaggedObject> dict);
    }
}
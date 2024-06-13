using NXOpen;
using static NXOpen.Selection;
using Curve = TSG_Library.Geom.Curve;
// ReSharper disable ClassNeverInstantiated.Global

namespace TSG_Library.Ui
{
    //
    // Summary:
    //     The result returned from showing a selection dialog
    public class Result
    {
        internal Result(NXObject[] objects, Response response, Curve.Ray ray)
        {
            Objects = objects;
            Response = response;
            CursorRay = ray;
        }

        //
        // Summary:
        //     The selected objects (possibly zero or one)
        public NXObject[] Objects { get; internal set; }

        //
        // Summary:
        //     The first selected object. Always equal to Objects[0]
        public NXObject Object => Objects[0];

        //
        // Summary:
        //     The user's response
        //
        // Remarks:
        //     The possible values of the response are:
        //
        //     • Back -- the user clicked the "Back" button
        //     • Cancel -- the user clicked the "Cancel" button
        //     • Ok -- the user clicked the "OK" button
        //     • ObjectSelectedByName -- the user selected an object by entering its name
        //     • ObjectSelected -- the user selected an object using the mouse
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Response Response { get; internal set; }

        //
        // Summary:
        //     The cursor ray
        //
        // Remarks:
        //     You can think of selection as a process of shooting an infinite line (the cursor
        //     ray) at your model. The object that gets selected is one that this ray hits,
        //     or the one that's closest to the ray. Sometimes, rather than just knowing which
        //     object was selected, you want to know where on the object the user clicked. You
        //     can figure this out by using the cursor ray. For example, you can find out where
        //     the ray intersects the model, or you can find out which end of a curve is closest
        //     to the ray. An example in chapter 15 of the SNAP "Getting Started" guide gives
        //     the details.
        //
        //     In some situations (like selecting by rectangle, selecting by name, or selecting
        //     multiple objects) the Selection.Result returned will have its CursorRay set to
        //     Nothing.
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public Curve.Ray CursorRay { get; internal set; }

        //
        // Summary:
        //     The cursor position at selected object
        //
        // Remarks:
        //     The "pick point" is a point on the selected object. It is either the place where
        //     the cursor ray pierces the object, or the closest point of the object to the
        //     cursor ray. Please see chapter 15 of the SNAP Getting Started Guide for an explanation
        //     of the cursor ray.
        //
        //     If the user selects multiple objects, the PickPoint property will have the value
        //     Nothing.
        public Point3d PickPoint { get; internal set; }
    }
}
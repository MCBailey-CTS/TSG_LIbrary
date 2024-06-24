using System;
using NXOpen;
using NXOpen.CAM.FBM;
using NXOpen.Features;
using TSG_Library.Geom;

namespace TSG_Library.Extensions
{
    public static partial class Extensions
    {
        #region NXObject

        public static void __DeleteUserAttribute(
            this NXObject nxobject,
            string title,
            Update.Option update_option = Update.Option.Now)
        {
            nxobject.DeleteUserAttribute(NXObject.AttributeType.String, title, true, update_option);
        }

        public static void __RemoveParameters(this NXObject nxobject)
        {
            RemoveParametersBuilder removeBuilder = nxobject.__OwningPart().Features.CreateRemoveParametersBuilder();

            using (session_.__UsingBuilderDestroyer(removeBuilder))
            {
                removeBuilder.Objects.Add(nxobject);
                removeBuilder.Commit();
            }
        }

        public static void __NXOpenNXObject(NXObject nxobject)
        {
            //nxobject.DeleteUserAttribute
            //nxobject.DeleteUserAttributes
            //nxobject.FindObject
            //nxobject.GetStringAttribute
            //nxobject.HasUserAttribute
            //nxobject.IsOccurrence
            //nxobject.JournalIdentifier
            //nxobject.OwningComponent
            //nxobject.OwningPart
            //nxobject.Print
            //nxobject.Prototype
            //nxobject.SetName
            //nxobject.Name
            //nxobject.SetUserAttribute
        }

        public static NXObject __Prototype(this NXObject obj)
        {
            return (NXObject)obj.Prototype;
        }

        public static string __GetAttribute(this NXObject nXObject, string title)
        {
            return nXObject.GetUserAttributeAsString(title, NXObject.AttributeType.String, -1);
        }

        public static bool __HasAttribute(this NXObject nXObject, string title)
        {
            return nXObject.HasUserAttribute(title, NXObject.AttributeType.String, -1);
        }

        public static void __DeleteAttribute(this NXObject nXObject, string title)
        {
            nXObject.DeleteUserAttribute(NXObject.AttributeType.String, title, true, Update.Option.Now);
        }

        public static void __SetAttribute(this NXObject nxobject, string title, string value)
        {
            nxobject.SetUserAttribute(title, -1, value, Update.Option.Now);
        }

        public static Part __OwningPart(this NXObject nXObject)
        {
            return (Part)nXObject.OwningPart;
        }

        public static string __GetStringAttribute(this NXObject part, string title)
        {
            return part.GetUserAttributeAsString(title, NXObject.AttributeType.String, -1);
        }

        public static NXObject.AttributeInformation[] __GetAttributes(this NXObject component)
        {
            return component.GetUserAttributes();
        }

        //
        // Summary:
        //     Copies an NX.NXObject (with no transform)
        //
        // Returns:
        //     A copy of the input object
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The object is an edge. Edges cannot be copied.
        //
        //   T:System.ArgumentException:
        //     The object is a face. Faces cannot be copied.
        //
        //   T:System.ArgumentException:
        //     A feature cannot be copied unless all of its ancestors are copied too.
        //
        // Remarks:
        //     The new object will be on the same layer as the original one.
        public static NXObject Copy(this NXObject nxObject)
        {
            Transform xform = Transform.CreateTranslation(0.0, 0.0, 0.0);
            return nxObject.Copy(xform);
        }

        //
        // Summary:
        //     Transforms/copies an object
        //
        // Parameters:
        //   xform:
        //     Transform to be applied
        //
        // Returns:
        //     Output object
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The object is an edge. Edges cannot be copied.
        //
        //   T:System.ArgumentException:
        //     The object is a face. Faces cannot be copied.
        //
        //   T:System.ArgumentException:
        //     A feature cannot be copied unless all of its ancestors are copied too.
        //
        //   T:System.ArgumentException:
        //     A transform that does not preserve angles cannot be applied to a coordinate system.
        //
        // Remarks:
        //     To create a transformation, use the functions in the Snap.Geom.Transform class.
        public static NXObject Copy(this NXObject nxObject, Transform xform)
        {
            _ = new double[16];
            int n_objects = 1;
            Tag[] objects = new Tag[1] { nxObject.Tag };
            int move_or_copy = 2;
            int dest_layer = 0;
            int trace_curves = 2;
            Tag[] array = new Tag[n_objects];
            int status = 0;

            try
            {
                ufsession_.Trns.TransformObjects(xform.Matrix, objects, ref n_objects, ref move_or_copy, ref dest_layer, ref trace_curves, array, out var _, out status);

                switch (status)
                {
                    case 3:
                        throw NXException.Create(674861);
                    case 4:
                        throw new ArgumentException("The matrix does not preserve angles");
                    case 5:
                        throw NXException.Create(670024);
                    case 6:
                        throw NXException.Create(670008);
                    case 7:
                        throw NXException.Create(670009);
                    case 8:
                        throw NXException.Create(37);
                    case 9:
                        throw NXException.Create(670022);
                    case 10:
                        throw NXException.Create(670023);
                    case 11:
                        throw NXException.Create(660002);
                    case 0:
                        if (array[0] == Tag.Null)
                        {
                            throw NXException.Create(674861);
                        }

                        break;
                }

                NXObject[] array2 = new NXObject[n_objects];

                for (int i = 0; i < n_objects; i++)
                    array2[i] = (NXObject)array[i].__ToTaggedObject();

                return array2[0];
            }
            catch (NXException innerException)
            {
                switch (nxObject)
                {
                    case Edge _:
                        throw new ArgumentException("The object is an edge. Edges cannot be copied", innerException);
                    case Face _:
                        throw new ArgumentException("The object is a face. Faces cannot be copied", innerException);
                    case NXOpen.Features.Feature _:
                    case DatumPlane _:
                    case DatumAxis _:
                        throw new ArgumentException("A feature cannot be copied unless all of its ancestors are copied too", innerException);
                    case CoordinateSystem _:
                    default:
                        throw;
                }
            }

            #endregion
        }
    }
}
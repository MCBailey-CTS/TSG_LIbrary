using NXOpen;

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

        #endregion
    }
}
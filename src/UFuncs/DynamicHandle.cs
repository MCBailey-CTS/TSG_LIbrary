using System;
using NXOpen;
using NXOpen.UserDefinedObjects;
using TSG_Library.Attributes;
using TSG_Library.UFuncs;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.Ufuncs
{
    [UFunc("dynamic-handle")]
    public class DynamicHandle : _UFunc
    {
        private static UserDefinedClass _myUDOClass;

        public override void execute()
        {
        }


        public static int myDisplayCB(UserDefinedDisplayEvent displayEvent)
        {
            try
            {
                var displayFlag = displayEvent.UserDefinedObject.GetIntegers();

                if(displayFlag[0] != 1)
                    return 0;

                var myUdoDoubles = displayEvent.UserDefinedObject.GetDoubles();
                var udoLocation = new Point3d[1];
                udoLocation[0].X = myUdoDoubles[0];
                udoLocation[0].Y = myUdoDoubles[1];
                udoLocation[0].Z = myUdoDoubles[2];
                displayEvent.DisplayContext.DisplayPoints(udoLocation,
                    UserDefinedObjectDisplayContext.PolyMarker.BigAsterisk);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            return 0;
        }

        public static int myDeleteCB(UserDefinedLinkEvent deleteEvent)
        {
            return 0;
        }

        public static int Startup()
        {
            const int retValue = 0;

            try
            {
                initializeUDO(false);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            return retValue;
        }

        public static int initializeUDO(bool alertUser)
        {
            try
            {
                if(!(_myUDOClass is null))
                    return 0;

                if(alertUser)
                    UI.GetUI().NXMessageBox
                        .Show("UDO", NXMessageBox.DialogType.Information, "Registering C# UDO Class");

                // Define your custom UDO class 
                _myUDOClass =
                    session_.UserDefinedClassManager.CreateUserDefinedObjectClass("UdoDynamicHandle", "Resize Handle");
                // Setup properties on the custom UDO class 
                _myUDOClass.AllowQueryClassFromName = UserDefinedClass.AllowQueryClass.On;
                _myUDOClass.SetIsOccurrenceableFlag(true);
                // Register callbacks for the UDO class 
                _myUDOClass.AddDisplayHandler(myDisplayCB);
                _myUDOClass.AddAttentionPointHandler(myDisplayCB);
                _myUDOClass.AddFitHandler(myDisplayCB);
                _myUDOClass.AddDeleteHandler(myDeleteCB);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            return 0;
        }
    }
}
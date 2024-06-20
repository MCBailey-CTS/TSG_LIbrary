using System;
using System.Linq;
using System.Reflection;
using NXOpen;
using NXOpen.UF;
using TSG_Library.Attributes;
using TSG_Library.UFuncs;
using TSG_Library.UFuncs.Mirror;
using static TSG_Library.Extensions.Extensions;

public static class Program
{
    public static void Main(string[] args)
    {
        try
        {

            //foreach (var f in __work_part_.Features.ToArray())
            //    print_(f.FeatureType);

            //return;

            //new MainForm().Show();

            Testing.Mirror();

            return;

            //var plane = new Surface.Plane(_Point3dOrigin, __Vector3dY());
            //var frComp = __display_part_.__RootComponent().GetChildren().Single(c => c.DisplayName.EndsWith("1100"));
            //frComp.__SetWcsToComponent();
            //var toOrigin = frComp.__Origin().__Mirror(plane);
            //var toOrientation = frComp.__Orientation().__Mirror(plane);
            //string toFilePath = "H:\\CTS\\003939 (mirror-001774)\\001774-010\\001774-010-9000.prt";
            //var toPart = session_.__New(toFilePath, frComp.__Prototype().PartUnits);
            //var toComp = __display_part_.__AddComponent(toPart, origin: toOrigin, orientation: toOrientation);


            //var dict = new Dictionary<TaggedObject, TaggedObject>
            //{
            //    { frComp, toComp }
            //};

            //TSG_Library.UFuncs.Mirror.Program.Mirror208_3001(plane, frComp, dict);

            return;
            //////new TSG_Library.UFuncs.Mirror.MainForm().Show();
            ////new AddFastenersForm1().Show();
            //return;

#pragma warning disable CS0162 // Unreachable code detected
            if (args.Length == 0)
            {
                print_("No main arguments");
                return;
            }
#pragma warning restore CS0162 // Unreachable code detected

            execute(args[0]);
        }
        catch (Exception ex)
        {
            ex.__PrintException();
        }
    }

    public static int GetUnloadOption()
    {
        return (int)Session.LibraryUnloadOption.Explicitly;
    }

    public static void UnloadLibrary()
    {
    }

    public static void execute(string ufunc_name)
    {
        _IUFunc __ufunc = null;

        foreach (Type type in Assembly.GetAssembly(typeof(Program)).GetTypes())
        {
            UFuncAttribute ufunc_att = type.GetCustomAttributes().OfType<UFuncAttribute>().SingleOrDefault();

            if (ufunc_att is null)
                continue;

            if (ufunc_att.ufunc_name != ufunc_name)
                continue;

            __ufunc = (_IUFunc)Activator.CreateInstance(type);

            break;
        }

        if (__ufunc is null)
        {
            print_($"Could not find ufunc with name '{ufunc_name}'");

            return;
        }

        UFSession.GetUFSession().UF.PrintSyslog($"Executed ufunc: '{__ufunc.ufunc_name}'", false);

        __ufunc.execute();
    }
}
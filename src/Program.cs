using System;
using System.Linq;
using System.Reflection;
using NXOpen;
using NXOpen.UF;
using TSG_Library.Attributes;
using TSG_Library.UFuncs;
using static TSG_Library.Extensions.__Extensions_;

public static class Program
{
    public static int WorkPartChangedRegister;


    ////[PostSharp.]
    ////[TrackUsage]
    //public static void CheckPostSHarp()
    //{
    //    print_("IN");
    //}

    //[PSerializable]
    //public class TrackUsage : OnMethodBoundaryAspect
    //{
    //    public TrackUsage()
    //    {

    //    }

    //    public override void OnException(MethodExecutionArgs args)
    //    {
    //        args.Exception._PrintException();
    //        base.OnException(args);
    //    }

    //    public override bool CompileTimeValidate(MethodBase method)
    //    {
    //        if (method.Name != "Hello")
    //            Message.Write(method, SeverityType.Error, "Custom01",
    //            "The target type does not implement ISecurable.");
    //        //Message.Write(SeverityType.Error, )
    //        //    throw new InvalidAnnotationException($"Method: {method.Name}");
    //        //new MessageLocation()

    //        //new Message()

    //        //Message.Write("hello");

    //        return base.CompileTimeValidate(method);
    //    }
    //    //{

    //    //    Message.Write()
    //    //    print_("IN INVOKE");
    //    //    base.OnInvoke(args);
    //    //}
    //}

    //[TrackUsage]
    public static void temp()
    {
    }

    public static void Main(string[] args)
    {
        try
        {
            //print_("BEFORE");
            //CheckPostSHarp();
            //print_("AFTER");

            //print_("hello world");

            new TSG_Library.UFuncs.MirrorComponents.Features.MainForm().Show();

            //new ComponentBuilder().Show();

            return;

            //__display_part_.Curves.ToArray()[0]._GetAttributeTitlesByType

#pragma warning disable CS0162 // Unreachable code detected
            new CycleComponentsForm().Show();
#pragma warning restore CS0162 // Unreachable code detected
            //new AssemblyAutoDetailForm().Show();

            //TSG_Library.

            //new TSG_Library.UFuncs.AssemblyWavelink.Form().Show();

            //new AddFastenersForm1().execute();

            //TSG_Library.Ui.Selection.Select


            //IntPtr user_data = IntPtr.Zero;


            ////session_._SelectSingleObject(init_proc, user_data, out int response, out var _object);

            //var component = TSG_Library.Ui.Selection.SelectSingleComponent(__c => __c.DisplayName._IsFastenerExtended_());

            //print_(component.DisplayName);

            return;

#pragma warning disable CS0162 // Unreachable code detected
            if(args.Length == 0)
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

    private static int init_proc(IntPtr user_data, IntPtr other)
    {
        try
        {
            return UFConstants.UF_UI_SEL_SUCCESS;
        }
        catch (Exception ex)
        {
            ex.__PrintException();
            return UFConstants.UF_UI_SEL_FAILURE;
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

        foreach (var type in Assembly.GetAssembly(typeof(Program)).GetTypes())
        {
            var ufunc_att = type.GetCustomAttributes().OfType<UFuncAttribute>().SingleOrDefault();

            if(ufunc_att is null)
                continue;

            if(ufunc_att.ufunc_name != ufunc_name)
                continue;

            __ufunc = (_IUFunc)Activator.CreateInstance(type);

            break;
        }

        if(__ufunc is null)
        {
            print_($"Could not find ufunc with name '{ufunc_name}'");

            return;
        }

        UFSession.GetUFSession().UF.PrintSyslog($"Executed ufunc: '{__ufunc.ufunc_name}'", false);

        __ufunc.execute();
    }


    //private static int FilterProcess(NXOpen.Tag _object, int[] type, IntPtr userData, IntPtr select)
    //{
    //    if (!(NXOpen.Utilities.NXObjectManager.Get(_object) is T obj))
    //        return 0;
    //    try
    //    {
    //        return SelectionPredicate(obj) ? 1 : 0;
    //    }
    //    catch (Exception ex)
    //    {
    //        ex._PrintException();
    //    }

    //    return 0;
    //}

    //private static int InitialProcess(IntPtr select, IntPtr userData)
    //{
    //    if (Masks != null && Masks.Any())
    //        ufsession_.Ui.SetSelMask(select, NXOpen.UF.UFUi.SelMaskAction.SelMaskClearAndEnableSpecific, 1, Masks);
    //    ufsession_.Ui.SetSelProcs(select, FilterProcess, null, userData);
    //    return NXOpen.UF.UFConstants.UF_UI_SEL_SUCCESS;
    //}
}
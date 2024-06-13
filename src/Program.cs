using System;
using System.Linq;
using System.Reflection;
using NXOpen;
using NXOpen.UF;
using TSG_Library.Attributes;
using TSG_Library.UFuncs;
using TSG_Library.UFuncs.MirrorComponents.Features;
using static TSG_Library.Extensions.__Extensions_;

public static class Program
{
    public static int WorkPartChangedRegister;


    public static void Main(string[] args)
    {
        try
        {
            new MainForm().Show();

            return;
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
        int temp = 10;

        switch (temp)
        {
            case 0:
            {
                print_("kdk");
                break;
            }
        }

        if (true) print_("kdkdk");
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
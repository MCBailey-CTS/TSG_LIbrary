using System.IO;
using System.Linq;
using TSG_Library.Utilities;

namespace TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities
{
    public class Model
    {
        /// <summary>The tolerance that should be used for metric comparisons.</summary>
        public static readonly double
            MetricTolerance; // = double.Parse(Ucf.MultipleValues("Metric_Tolerance").Single());

        /// <summary>The tolerance that should be used for english comparisons.</summary>
        public static readonly double
            EnglishTolerance; // = double.Parse(Ucf.MultipleValues("English_Tolerance").Single());

        static Model()
        {
            // ReSharper disable once JoinDeclarationAndInitializer
            string ucfString;
            const string uDrive = "U:\\nxFiles\\UfuncFiles\\DesignCheck.ucf";
            const string oUfunc = "G:\\CTS\\junk\\0Ufunc Testing\\DesignCheck.ucf";
#if DEBUG
            ucfString = File.Exists(oUfunc) ? oUfunc : uDrive;
#else
            ucfString = uDrive;
#endif

            Ucf = new Ucf(ucfString);
            MetricTolerance = double.Parse(Ucf["Metric_Tolerance"].Single());
            EnglishTolerance = double.Parse(Ucf["English_Tolerance"].Single());
        }

        public static Ucf Ucf { get; }
    }
}
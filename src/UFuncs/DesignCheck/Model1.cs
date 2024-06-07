//using TSG_Library.Utilities;

//namespace TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities
//{
//    public class Model
//    {
//        public static Ucf Ucf { get; }

//        static Model()
//        {
//            // ReSharper disable once JoinDeclarationAndInitializer
//            string ucfString;
//            const string uDrive = "U:\\nxFiles\\UfuncFiles\\DesignCheck.ucf";
//            const string oUfunc = "G:\\CTS\\junk\\0Ufunc Testing\\DesignCheck.ucf";
//#if DEBUG
//            ucfString = File.Exists(oUfunc) ? oUfunc : uDrive;
//#else
//            ucfString = uDrive;
//#endif

//            Ucf = new Ucf(ucfString);
//            MetricTolerance = double.Parse(Ucf["Metric_Tolerance"].Single());
//            EnglishTolerance = double.Parse(Ucf["English_Tolerance"].Single());
//        }

//        /// <summary>The tolerance that should be used for metric comparisons.</summary>
//        public static readonly double MetricTolerance; // = double.Parse(Ucf.MultipleValues("Metric_Tolerance").Single());

//        /// <summary>The tolerance that should be used for english comparisons.</summary>
//        public static readonly double EnglishTolerance; // = double.Parse(Ucf.MultipleValues("English_Tolerance").Single());

//        // var failedComponents = (from partOcc in partOccsTags
//        //                         select (NXOpen.Assemblies.Component)NXOpen.Utilities.NXObjectManager.Get(partOcc)
//        //     into component
//        //                         where !component.IsSuppressed
//        //                         where component.GetPositionOverrideType() == NXOpen.Assemblies.PositionOverrideType.Explicit
//        //                         select (component, new[] { $"Position Override is set too {component.GetPositionOverrideType()}" })).Select(dummy => ((NXOpen.Assemblies.Component,
//        //                           IEnumerable<string>))dummy).ToList();

//        //if (failedComponents.Count == 0)
//        //{
//        //    yield return new DesignCheckResult(true, part, this,
//        //        new ObjectNode($"All part occurrences passed."));
//        //    yield break;
//        //}

//        //List<ObjectNode> errorObjectNodes = new List<ObjectNode>();
//        //foreach (Tuple<Component, IEnumerable<string>> tuple in failedComponents)
//        //{
//        //    ObjectNode<Component> linkedBodyNode = new ObjectNode<Component>(tuple.Item1, tuple.Item1.Name);
//        //    linkedBodyNode.AddRange(tuple.Item2.Select(s => new ObjectNode(s)));
//        //    errorObjectNodes.Add(linkedBodyNode);
//        //}

//        //yield return new DesignCheckResult(false, part, this, errorObjectNodes);
//    }
//}


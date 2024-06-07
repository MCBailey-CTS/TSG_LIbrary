using System.Reflection;
using System.Text.RegularExpressions;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;
using TSG_Library.Extensions;

namespace TSG_Library.Attributes
{
    [PSerializable]
    public class ExtensionsAspect : OnMethodBoundaryAspect
    {
        public override void OnException(MethodExecutionArgs args)
        {
            args.Exception._PrintException();
            base.OnException(args);
        }

        public override bool CompileTimeValidate(MethodBase method)
        {
            if (!Regex.IsMatch(method.Name, "^__[A-Za-z0-9]+$"))
                Message.Write(method, SeverityType.Error, "Custom01",
                    $"Didn't pass {method.Name}");

            return base.CompileTimeValidate(method);
        }
    }
}
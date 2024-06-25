using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using TSG_Library.Extensions;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("TSG_Library")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("TSG_Library")]
[assembly: AssemblyCopyright("Copyright ©  2024")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("1efbe2f3-41c1-41bf-84c8-3660330985b7")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.6.0.0")]
[assembly: AssemblyFileVersion("1.6.0.0")]
[assembly: DebuggerDisplay("{Leaf}", Target = typeof(NXOpen.BasePart))]
//[assembly: DebuggerDisplay("{DisplayName} " + "{" +nameof(Extensions.__Origin) + "}", Target = typeof(NXOpen.Assemblies.Component))]
[assembly: DebuggerDisplay("{DisplayName} {TSG_Library.Extensions.Extensions.__Origin(this)}", Target = typeof(NXOpen.Assemblies.Component))]

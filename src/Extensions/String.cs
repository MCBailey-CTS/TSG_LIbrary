using System;
using System.IO;
using System.Text.RegularExpressions;
using NXOpen;
using NXOpen.Assemblies;

namespace TSG_Library.Extensions
{
    public static partial class Extensions
    {
        #region String

        private static bool __FastenerInfo(string file, string regex, out string diameter, out string length)
        {
            if (file.Contains("\\"))
                file = Path.GetFileNameWithoutExtension(file);

            Match match = Regex.Match(file, regex, RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                diameter = string.Empty;

                length = string.Empty;

                return false;
            }

            diameter = match.Groups["diameter"].Value;

            length = match.Groups["length"].Value;

            return true;
        }

        //[IgnoreExtensionAspect]
        public static bool _IsBlhcs_(this string leafOrDisplayName)
        {
            return leafOrDisplayName.ToLower().Contains("0375-bhcs-062")
                   || leafOrDisplayName.ToLower().Contains("10mm-bhcs-016");
        }

        //[IgnoreExtensionAspect]
        public static bool _IsLayout_(this string leafOrDisplayName)
        {
            return leafOrDisplayName.ToLower().Contains("-layout");
        }

        //[IgnoreExtensionAspect]
        public static bool _IsShcs_(this string leaf)
        {
            return leaf.ToLower().Contains("-shcs-");
        }

        //[IgnoreExtensionAspect]
        public static bool _IsShcs_(this Component component)
        {
            return component.DisplayName._IsShcs_();
        }

        //[IgnoreExtensionAspect]
        public static bool _IsShcs_(this Part part)
        {
            return part.Leaf._IsShcs_();
        }

        //[IgnoreExtensionAspect]
        public static bool _IsDwl_(this string leaf)
        {
            return leaf.ToLower().Contains("-dwl-");
        }

        //[IgnoreExtensionAspect]
        public static bool _IsDwl_(this Component component)
        {
            return component.DisplayName._IsDwl_();
        }

        //[IgnoreExtensionAspect]
        public static bool _IsDwl_(this Part part)
        {
            return part.Leaf._IsDwl_();
        }

        //[IgnoreExtensionAspect]
        public static bool _IsJckScrew_(this string leaf)
        {
            return !leaf._IsJckScrewTsg_() && leaf.ToLower().Contains("-jck-screw-");
        }

        //[IgnoreExtensionAspect]
        public static bool _IsJckScrew_(this Component component)
        {
            return component.DisplayName._IsJckScrew_();
        }

        //[IgnoreExtensionAspect]
        public static bool _IsJckScrew_(this Part part)
        {
            return part.Leaf._IsJckScrew_();
        }

        //[IgnoreExtensionAspect]
        public static bool _IsJckScrewTsg_(this string leaf)
        {
            return leaf.ToLower().Contains("-jck-screw-tsg");
        }

        //[IgnoreExtensionAspect]
        public static bool _IsJckScrewTsg_(this Component component)
        {
            return component.DisplayName._IsJckScrewTsg_();
        }

        //[IgnoreExtensionAspect]
        public static bool _IsJckScrewTsg_(this Part part)
        {
            return part.Leaf._IsJckScrewTsg_();
        }

        //[IgnoreExtensionAspect]
        public static bool _IsLhcs_(this string leaf)
        {
            return leaf.ToLower().Contains("-lhcs-");
        }

        //[IgnoreExtensionAspect]
        public static bool _IsSss_(this string leaf)
        {
            return leaf.ToLower().Contains("-sss-");
        }

        //[IgnoreExtensionAspect]
        public static bool _IsBhcs_(this string leaf)
        {
            return leaf.ToLower().Contains("-bhcs-");
        }

        //[IgnoreExtensionAspect]
        public static bool _IsFhcs_(this string leaf)
        {
            return leaf.ToLower().Contains("-fhcs-");
        }

        //[IgnoreExtensionAspect]
        public static bool _IsFastener_(this string leafOrDisplayName)
        {
            return leafOrDisplayName._IsShcs_()
                   || leafOrDisplayName._IsDwl_()
                   || leafOrDisplayName._IsJckScrew_()
                   || leafOrDisplayName._IsJckScrewTsg_();
        }

        //[IgnoreExtensionAspect]
        public static bool _IsFastenerExtended_(this string leafOrDisplayName)
        {
            return leafOrDisplayName.__IsFastener()
                   || leafOrDisplayName._IsLhcs_()
                   || leafOrDisplayName._IsSss_()
                   || leafOrDisplayName._IsBhcs_()
                   || leafOrDisplayName._IsFhcs_()
                   || leafOrDisplayName._IsBlhcs_();
        }

        public static string __AskDetailNumber(this string file)
        {
            string leaf = Path.GetFileNameWithoutExtension(file);
            Match match = Regex.Match(leaf, "^\\d+-\\d+-(?<detail>\\d+)$");

            if (!match.Success)
                throw new FormatException("Could not find detail number.");

            return match.Groups["detail"].Value;
        }

        public static bool __IsShcs(this string file)
        {
            return file.__IsShcs(out _);
        }

        public static bool __IsDwl(this string file)
        {
            return file.__IsDwl(out _);
        }

        public static bool __IsJckScrew(this string file)
        {
            return file.__IsJckScrew(out _);
        }

        public static bool __IsJckScrewTsg(this string file)
        {
            return file.__IsJckScrewTsg(out _);
        }

        public static bool __IsShcs(
            this string file,
            out string diameter,
            out string length)
        {
            return __FastenerInfo(file, RegexShcs, out diameter, out length);
        }

        public static bool __IsDwl(
            this string file,
            out string diameter,
            out string length)
        {
            return __FastenerInfo(file, RegexDwl, out diameter, out length);
        }

        public static bool __IsFastener(this string file)
        {
            return file.__IsFastener(out _);
        }

        public static bool __IsShcs(
            this string file,
            out string diameter)
        {
            return file.__IsShcs(out diameter, out _);
        }

        public static bool __IsDwl(
            this string file,
            out string diameter)
        {
            return file.__IsDwl(out diameter, out _);
        }

        public static bool __IsJckScrew(
            this string file,
            out string diameter)
        {
            return __FastenerInfo(file, RegexJckScrew, out diameter, out _);
        }

        public static bool __IsJckScrewTsg(
            this string file,
            out string diameter)
        {
            return __FastenerInfo(file, RegexJckScrewTsg, out diameter, out _);
        }

        public static bool __IsFastener(this string file, out string diameter)
        {
            if (file.__IsShcs(out diameter))
                return true;

            if (file.__IsDwl(out diameter))
                return true;

            return file.__IsJckScrew(out diameter) || file.__IsJckScrewTsg(out diameter);
        }

        public static bool __IsLoaded(this string partName)
        {
            int status = ufsession_.Part.IsLoaded(partName);

            switch (status)
            {
                case 0: // not loaded
                    return false;
                case 1: // fully loaded
                case 2: // partially loaded
                    return true;
                default:
                    throw NXException.Create(status);
            }
        }

        public static bool __IsDetail(this string str)
        {
            string leaf = Path.GetFileNameWithoutExtension(str);

            if (leaf is null)
                return false;

            return Regex.IsMatch(leaf, "^\\d+-\\d+-\\d+$");
        }


        public static string __AskDetailOp(this string path)
        {
            string leaf = Path.GetFileNameWithoutExtension(path);

            Match match = Regex.Match(leaf, "^\\d+-(?<op>\\d+)-\\d+$");

            if (!match.Success)
                throw new Exception($"could not find an op: '{leaf}'");

            return match.Groups["op"].Value;
        }


        //public static bool __IsAssemblyHolder(string str)
        //{
        //    if (string.IsNullOrEmpty(str))
        //        return false;

        //    str = Path.GetFileNameWithoutExtension(str);
        //    var startIndex = str.LastIndexOf('-');

        //    if (startIndex < 0)
        //        return false;

        //    var str1 = str.Substring(startIndex);

        //    var strArray1 = new string[5]
        //    {
        //        "lwr",
        //        "upr",
        //        "lsh",
        //        "ush",
        //        "000"
        //    };

        //    var strArray2 = new string[2] { "lsp", "usp" };
        //    return strArray1.Any(str1.EndsWith) ||
        //           strArray2.Any(str1.Contains);
        //}

        //public static bool __IsFastener(string path)
        //{
        //    return IsScrew(path) || IsDowel(path) || IsJigJackTsg(path) || IsJigJack(path);
        //}

        //public static bool __IsScrew(string path)
        //{
        //    return path.Contains("shcs");
        //}

        //public static bool IsDowel(string path)
        //{
        //    return path.Contains("dwl");
        //}

        //public static bool IsJigJack(string path)
        //{
        //    return path.Contains("jck-screw") && !path.Contains("tsg");
        //}

        //public static bool IsJigJackTsg(string path)
        //{
        //    return path.Contains("jck-screw-tsg");
        //}

        public static bool __IsPartDetail(this string partLeaf)
        {
            return Regex.IsMatch(partLeaf, DetailNumberRegex);
        }

        //public static bool __IsAssemblyHolder(this string str)
        //{
        //    return str._IsLsh() || str._IsUsh() || str._IsLwr() || str._IsUpr() || str._IsLsp() || str._IsUsp() ||
        //           str._Is000();
        //}

        public static bool __IsLsh(this string str)
        {
            return Regex.IsMatch(str, RegexLsh, RegexOptions.IgnoreCase);
        }

        public static bool __IsUsh(this string str)
        {
            return Regex.IsMatch(str, RegexUsh, RegexOptions.IgnoreCase);
        }

        public static bool __IsLsp(this string str)
        {
            return Regex.IsMatch(str, RegexLsp, RegexOptions.IgnoreCase);
        }

        public static bool __IsUsp(this string str)
        {
            return Regex.IsMatch(str, RegexUsp, RegexOptions.IgnoreCase);
        }

        public static bool __IsLwr(this string str)
        {
            return Regex.IsMatch(str, RegexLwr, RegexOptions.IgnoreCase);
        }

        public static bool __IsUpr(this string str)
        {
            return Regex.IsMatch(str, RegexUpr, RegexOptions.IgnoreCase);
        }

        public static bool __Is000(this string str)
        {
            return Regex.IsMatch(str, RegexOp000Holder, RegexOptions.IgnoreCase);
        }

        #endregion
    }
}
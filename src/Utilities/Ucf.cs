using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TSG_Library.Extensions;

namespace TSG_Library.Utilities
{
    public class Ucf
    {
        // private static readonly Ucf _ucf = null;

        private const string RecStart = "<rec>";

        private const string RecEnd = "</rec>";

        private const string End = ":END:";

        public const string ConceptControlFile = @"U:\nxFiles\UfuncFiles\ConceptControlFile.ucf";

        public readonly Dictionary<string, string[]> Dictionary;

        public Ucf(string ucfFilePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(ucfFilePath)
                    .Where(line => !string.IsNullOrEmpty(line))
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Where(line => !line.StartsWith(@"//"))
                    .ToArray();

                Dictionary = new Dictionary<string, string[]>();

                for (int index = 0; index < lines.Length; index++)
                {
                    string startLine = lines[index];
                    if (!startLine.StartsWith(":") || !startLine.EndsWith(":")) continue;
                    List<string> list = new List<string>();
                    for (int endIndex = index + 1; endIndex < lines.Length; endIndex++)
                    {
                        string line = lines[endIndex];
                        if (line == End)
                        {
                            index = endIndex;
                            break;
                        }

                        list.Add(line);
                    }

                    if (list.Count == 0)
                        throw new Exception("Start and End didn't contain any content.");
                    Dictionary[startLine.Substring(1, startLine.Length - 2)] = list.ToArray();
                }

                string[] keyArray = Dictionary.Keys.ToArray();
                foreach (string key in keyArray)
                {
                    string[] valueArray = Dictionary[key];
                    for (int valueIndex = 0; valueIndex < valueArray.Length; valueIndex++)
                        valueArray[valueIndex] = ConstructString(Dictionary, valueArray[valueIndex]);
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        public string[] this[string titleIndex] => MultipleStrings(titleIndex);

        public static IEnumerable<string> StaticRead(string filePath, string startDelimeter, string endDelimeter,
            StringComparison comparisonType)
        {
            // Checks to make sure that {filePath} exists.
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Could not find file \"{filePath}\".", filePath);

            // Reads in all the lines from the {filePath}.
            List<string> filePathLines = File.ReadAllLines(filePath).ToList();

            // Checks to make sure that {filePathLines} contains at least one line in it.
            if (filePathLines.Count == 0)
                throw new ArgumentException(@"Supplied file path doesn't contain any readable lines.",
                    nameof(filePath));


            int startDelimeterIndex =
                filePathLines.FindIndex(line => string.Equals(line, startDelimeter, comparisonType));

            int endDelimeterIndex = filePathLines.FindIndex(line => string.Equals(line, endDelimeter, comparisonType));

            // Checks to make sure the {startDelimeterIndex} is less than {endDelimeterIndex}.
            if (startDelimeterIndex >= endDelimeterIndex)
                throw new Exception("Start delimeter was greater than end delimeter.");

            if (startDelimeterIndex < 0)
                throw new Exception("Did not find start delimeter.");

            if (endDelimeterIndex < 0)
                throw new Exception("Did not find end delimeter.");

            for (int index = startDelimeterIndex + 1; index < endDelimeterIndex; index++)
                yield return filePathLines[index];
        }

        [DebuggerStepThrough]
        public IEnumerable<TSource> MultipleValues<TSource>(string key)
        {
            if (!Dictionary.ContainsKey(key))
                throw new KeyNotFoundException($"Dictionary doesn't contain key: {key}.");

            return Dictionary[key].Cast<TSource>().ToArray();
        }

        public IEnumerable<int> MultipleIntegers(string key)
        {
            return MultipleStrings(key).Select(int.Parse).ToArray();
        }

        public int SingleInteger(string key)
        {
            return MultipleIntegers(key).Single();
        }

        public IEnumerable<double> MultipleDouble(string key)
        {
            return MultipleStrings(key).Select(double.Parse).ToArray();
        }

        public double SingleDouble(string key)
        {
            return MultipleDouble(key).Single();
        }

        public string[] MultipleStrings(string key)
        {
            if (!Dictionary.ContainsKey(key))
                throw new ArgumentException($"Dictionary doesn't contain key: {key}.");

            return Dictionary[key];
        }

        public string SingleValue(string key)
        {
            string[] strings = MultipleStrings(key);

            if (strings.Length > 1)
                throw new ArgumentException($"Dictionary[{key}] contains more than one element.");

            return strings[0];
        }

        private static string ConstructString(IDictionary<string, string[]> dictionary, string str)
        {
            int startIndex = str.IndexOf(RecStart, 0, StringComparison.Ordinal);
            int endIndex = str.IndexOf(RecEnd, 0, StringComparison.Ordinal);
            if (startIndex < 0 && endIndex < 0)
                // "str" doesn't contain any recursive entities.
                return str;
            IEnumerable<string> strings = ParseString(dictionary, str, true);
            return strings.Aggregate("", (current, next) => current + next);
        }

        private static IEnumerable<string> ParseString(IDictionary<string, string[]> dictionary, string currentString,
            bool startDelimeter)
        {
            if (startDelimeter)
            {
                int startIndex = currentString.IndexOf(RecStart, StringComparison.Ordinal);

                switch (startIndex)
                {
                    case var index when index < 0:
                        yield return currentString;
                        break;
                    case 0:
                    {
                        IEnumerable<string> strings = ParseString(dictionary,
                            currentString.Substring(startIndex + RecStart.Length),
                            false);
                        foreach (string str in strings)
                            yield return str;
                    }
                        break;
                    default:
                    {
                        // The first index of <rec> is not 0, therefore we want to return the string leading up to <rec>.
                        yield return currentString.Substring(0, startIndex);

                        IEnumerable<string> strings = ParseString(dictionary,
                            currentString.Substring(startIndex + RecStart.Length),
                            false);
                        foreach (string str in strings)
                            yield return str;
                    }
                        break;
                }
            }
            else
            {
                int endIndex = currentString.IndexOf(RecEnd, StringComparison.Ordinal);

                switch (endIndex)
                {
                    case var index when index < 0:
                        // We want to throw here because <rec> was unable to be found in the string, and therefore should have never been put into this method.
                        throw new ArgumentOutOfRangeException(nameof(currentString),
                            $@"Could not find an instance of {RecEnd} within {currentString}.");
                    case 0:
                        // Then we have an empty string as a value which is no good.
                        throw new ArgumentException("You cannot have an empty value for a recursive entity.");
                    default:
                    {
                        string recursiveEntity = currentString.Substring(0, endIndex);
                        if (!dictionary.ContainsKey(recursiveEntity))
                            throw new InvalidOperationException($"Could not find a key named {recursiveEntity}");
                        if (dictionary[recursiveEntity].Length != 1)
                            throw new InvalidOperationException(
                                $"Key {recursiveEntity} contains {dictionary[recursiveEntity].Length} elements.");
                        yield return dictionary[recursiveEntity][0];
                    }
                        break;
                }

                string substring = currentString.Substring(endIndex + RecEnd.Length);

                if (substring.Length == 0)
                    yield break;

                IEnumerable<string> strings = ParseString(dictionary, substring, true);

                foreach (string str in strings)
                    yield return str;
            }
        }
    }
}
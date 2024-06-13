using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NXOpen;
using NXOpen.Annotations;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncUtilities.AssemblyAutoDetailUtilities
{
    [Obsolete]
    public static class DrillChart
    {
        private const string holeChartText = @"U:\nxFiles\UfuncFiles\HoleChart.txt";

        private static Part __display_part_ => Session.GetSession().Parts.Display;

        public static string[] Main()
        {
            string[] lines = File.ReadAllLines(holeChartText)
                .Where(s => !string.IsNullOrEmpty(s))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Where(s => !s.StartsWith("//"))
                .ToArray();

            IList<string[]> holeChart = new List<string[]>();

            for (int i = 1; i < lines.Length; i++)
            {
                string[] split = lines[i].Split('\t');

                holeChart.Add(split);
            }

            // Get the solid body on layer 1
            Body solidBody = __display_part_.__SolidBodyLayer1OrNull();

            if (solidBody is null)
                throw new ArgumentException("Display part does not have solid body on layer 1");

            IDictionary<double, Tuple<int[], IList<Face>, string[]>> dict =
                new Dictionary<double, Tuple<int[], IList<Face>, string[]>>();

            foreach (Face face in solidBody.GetFaces())
            {
                if (face.SolidFaceType != Face.FaceType.Cylindrical)
                    continue;

                if (!face.Name.ToUpper().Contains("HOLECHART"))
                    continue;

                double[] point = new double[3];
                double[] dir = new double[3];
                double[] box = new double[6];

                TheUFSession.Modl.AskFaceData(face.Tag, out int _, point, dir, box, out double radius, out _, out _);

                double diameter = radius * 2; // * 25.4;

                string[] actualLine =
                (
                    from line in holeChart
                    let tempRadius = double.Parse(line[1])
                    where System.Math.Abs(tempRadius - diameter) < .000000001
                    select line
                ).FirstOrDefault();

                if (actualLine is null)
                {
                    print_($"Couldn't find hole chart: {diameter}");

                    continue;
                }

                if (!dict.ContainsKey(diameter))
                    dict.Add(diameter,
                        new Tuple<int[], IList<Face>, string[]>(new[] { 0 }, new List<Face>(), actualLine));

                dict[diameter].Item1[0]++;

                dict[diameter].Item2.Add(face);
            }

            session_.__DeleteObjects(__display_part_.Layers.GetAllObjectsOnLayer(230).OfType<Note>().ToArray());

            char letter = 'A';

            IList<IList<string>> actualLines = new List<IList<string>>();

            foreach (double diameter in dict.Keys)
            {
                Tuple<int[], IList<Face>, string[]> tuple = dict[diameter];

                int count = tuple.Item1[0];

                IList<string> list = new List<string>();

                IList<Face> faces = tuple.Item2;

                string[] message = tuple.Item3;

                list.Add($"{letter} ");

                string temp = message.Length == 3 ? $"{message[2]} " : $"{message[0]} ";


                string[] split = Regex.Split(temp, "FOR\\s");

                list.Add($"{split[0]}FOR");
                list.Add(split[1]);


                //list.Add(temp);

                list.Add($"QTY {count}");

                actualLines.Add(list);


                foreach (Face face in faces)
                {
                    double[] point = new double[3];
                    double[] dir = new double[3];
                    double[] box = new double[6];

                    TheUFSession.Modl.AskFaceData(face.Tag, out int _, point, dir, box, out double _, out _, out _);

                    using (session_.__UsingDoUpdate())
                    {
                        using (LetteringPreferences letteringPreferences1 =
                               __work_part_.Annotations.Preferences.GetLetteringPreferences())
                        using (UserSymbolPreferences userSymbolPreferences1 =
                               __work_part_.Annotations.NewUserSymbolPreferences(
                                   UserSymbolPreferences.SizeType.ScaleAspectRatio,
                                   1.0,
                                   1.0))
                        {
                            userSymbolPreferences1.SetLengthAndHeight(.125, .125);

                            Note note1 = __work_part_.Annotations.CreateNote(
                                new[] { $"{letter}" },
                                point.__ToPoint3d(),
                                AxisOrientation.Horizontal,
                                letteringPreferences1,
                                userSymbolPreferences1);

                            note1.Layer = 230;

                            note1.RedisplayObject();

                            note1.SetName("HOLECHARTNOTE");
                            TheUFSession.View.ConvertToModel(__display_part_.ModelingViews.WorkView.Tag, note1.Tag);

                            DraftingNoteBuilder draftingNoteBuilder1 =
                                __work_part_.Annotations.CreateDraftingNoteBuilder(note1);

                            using (session_.__UsingBuilderDestroyer(draftingNoteBuilder1))
                            {
                                draftingNoteBuilder1.Style.LetteringStyle.GeneralTextSize = .125;
                                draftingNoteBuilder1.Commit();
                            }
                        }
                    }
                }

                letter++;
            }

            IList<string> note = new List<string>();

            foreach (IList<string> t in actualLines)
            {
                string _letter = t[0];
                string drill = t[1];
                string fastenr = t[2];
                string quantity = t[3];

                note.Add($"{_letter}{drill}");
                note.Add($"{fastenr}{quantity}".ToUpper());
                note.Add("");


                string s = "";
                foreach (string k in t) s += k;

                note.Add(s);
            }


            //            WriteLine(str);

            //           var session_ = NXOpen.Session.GetSession();
            //            var workPart = session_.Parts.Work;
            //            var displayPart = session_.Parts.Display;
            //            var drawingSheet1 = workPart.DrawingSheets.FindObject("4-VIEW");
            //            drawingSheet1.Open();
            //
            //            session_.ApplicationSwitchImmediate("UG_APP_DRAFTING");
            //
            //            workPart.Drafting.EnterDraftingApplication();
            //            
            //            var  point1 = new NXOpen.Point3d(91.560795454545442, 71.080042613636351, 0.0);
            //
            //
            //
            //            session_.create.Note(point1, Snap.Orientation.Identity, new TextStyle(), note.ToArray());

            return note.ToArray();
            //
        }
    }
}
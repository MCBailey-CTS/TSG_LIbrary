using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NXOpen;
using NXOpen.Annotations;
using TSG_Library.Extensions;
using static TSG_Library.Extensions.Extensions_;

namespace TSG_Library.UFuncUtilities.AssemblyAutoDetailUtilities
{
    [Obsolete]
    public static class DrillChart
    {
        private const string holeChartText = @"U:\nxFiles\UfuncFiles\HoleChart.txt";

        private static Part __display_part_ => Session.GetSession().Parts.Display;

        public static string[] Main()
        {
            var lines = File.ReadAllLines(holeChartText)
                .Where(s => !string.IsNullOrEmpty(s))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Where(s => !s.StartsWith("//"))
                .ToArray();

            IList<string[]> holeChart = new List<string[]>();

            for (var i = 1; i < lines.Length; i++)
            {
                var split = lines[i].Split('\t');

                holeChart.Add(split);
            }

            // Get the solid body on layer 1
            var solidBody = __display_part_.__SolidBodyLayer1OrNull();

            if (solidBody is null)
                throw new ArgumentException("Display part does not have solid body on layer 1");

            IDictionary<double, Tuple<int[], IList<Face>, string[]>> dict =
                new Dictionary<double, Tuple<int[], IList<Face>, string[]>>();

            foreach (var face in solidBody.GetFaces())
            {
                if (face.SolidFaceType != Face.FaceType.Cylindrical)
                    continue;

                if (!face.Name.ToUpper().Contains("HOLECHART"))
                    continue;

                var point = new double[3];
                var dir = new double[3];
                var box = new double[6];

                TheUFSession.Modl.AskFaceData(face.Tag, out var _, point, dir, box, out var radius, out _, out _);

                var diameter = radius * 2; // * 25.4;

                var actualLine =
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

            session_.delete_objects(__display_part_.Layers.GetAllObjectsOnLayer(230).OfType<Note>().ToArray());

            var letter = 'A';

            IList<IList<string>> actualLines = new List<IList<string>>();

            foreach (var diameter in dict.Keys)
            {
                var tuple = dict[diameter];

                var count = tuple.Item1[0];

                IList<string> list = new List<string>();

                var faces = tuple.Item2;

                var message = tuple.Item3;

                list.Add($"{letter} ");

                var temp = message.Length == 3 ? $"{message[2]} " : $"{message[0]} ";


                var split = Regex.Split(temp, "FOR\\s");

                list.Add($"{split[0]}FOR");
                list.Add(split[1]);


                //list.Add(temp);

                list.Add($"QTY {count}");

                actualLines.Add(list);


                foreach (var face in faces)
                {
                    var point = new double[3];
                    var dir = new double[3];
                    var box = new double[6];

                    TheUFSession.Modl.AskFaceData(face.Tag, out var _, point, dir, box, out var _, out _, out _);

                    using (session_.using_do_update())
                    {
                        using (var letteringPreferences1 =
                               __work_part_.Annotations.Preferences.GetLetteringPreferences())
                        using (var userSymbolPreferences1 = __work_part_.Annotations.NewUserSymbolPreferences(
                                   UserSymbolPreferences.SizeType.ScaleAspectRatio,
                                   1.0,
                                   1.0))
                        {
                            userSymbolPreferences1.SetLengthAndHeight(.125, .125);

                            var note1 = __work_part_.Annotations.CreateNote(
                                new[] { $"{letter}" },
                                point._ToPoint3d(),
                                AxisOrientation.Horizontal,
                                letteringPreferences1,
                                userSymbolPreferences1);

                            note1.Layer = 230;

                            note1.RedisplayObject();

                            note1.SetName("HOLECHARTNOTE");
                            TheUFSession.View.ConvertToModel(__display_part_.ModelingViews.WorkView.Tag, note1.Tag);

                            var draftingNoteBuilder1 = __work_part_.Annotations.CreateDraftingNoteBuilder(note1);

                            using (session_.using_builder_destroyer(draftingNoteBuilder1))
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

            foreach (var t in actualLines)
            {
                var _letter = t[0];
                var drill = t[1];
                var fastenr = t[2];
                var quantity = t[3];

                note.Add($"{_letter}{drill}");
                note.Add($"{fastenr}{quantity}".ToUpper());
                note.Add("");


                var s = "";
                foreach (var k in t) s += k;

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
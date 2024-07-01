using NXOpen;
using NXOpen.Assemblies;
using System;
using System.Data.SqlClient;
using System.Linq;
using TSG_Library.Attributes;
using TSG_Library.Extensions;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs
{
    [UFunc("clean-body-ref-set")]
    public class CleanBodyRefSet : _UFunc
    {
        public override void execute()
        {
            //print_(ufunc_name);

            {
                string connection_string_ctsapp =
                @"Data Source=tsgapps2.toolingsystemsgroup.com;Initial Catalog=CTSAPP;User ID=CTSAPP;Password=RI4SU9d2JxH8LcrxSDPS";

                using (var cnn = new SqlConnection(connection_string_ctsapp))
                {
                    cnn.Open();

                    var command = new SqlCommand()
                    {
                        Connection = cnn,
                        CommandText = $"insert into UFuncUsage (ufunc, user_) values ('{ufunc_name}', '{Environment.UserName}')"
                    };

                    command.ExecuteScalar();
                }
            }








            var selected_comps = Ui.Selection.SelectManyComponents(ufunc_rev_name);

            if (selected_comps.Length == 0)
                return;

            var selected_parts = selected_comps.Select(c => c.__Prototype())
                .Distinct()
                .ToArray();

            foreach (var part in selected_parts)
                using (session_.__UsingDisplayPartReset())
                {
                    __work_part_ = part;

                    var layer1Objects = part.Layers.GetAllObjectsOnLayer(1).ToHashSet();

                    var layer1Count = layer1Objects.Count();

                    var solidBodies = layer1Objects.OfType<Body>()
                        .Where(b => b.IsSolidBody)
                        .ToHashSet();

                    layer1Objects.OfType<Curve>().ToList().ForEach(c => c.__Layer(3));
                    layer1Objects.OfType<Point>().ToList().ForEach(p => p.__Layer(9));
                    layer1Objects.OfType<DatumAxis>().ToList().ForEach(p => p.__Layer(255));
                    layer1Objects.OfType<DatumPlane>().ToList().ForEach(p => p.__Layer(255));
                    layer1Objects.OfType<CoordinateSystem>().ToList().ForEach(p => p.__Layer(255));

                    layer1Objects.OfType<Body>()
                        .Where(b => b.IsSheetBody)
                        .ToList()
                        .ForEach(b => b.__Layer(10));

                    part.Layers.GetAllObjectsOnLayer(1).Except(solidBodies)
                        .OfType<DisplayableObject>()
                        .ToList()
                        .ForEach(o => o.__Layer(11));

                    if (!part.__HasReferenceSet("BODY"))
                        continue;

                    var refset = part.__ReferenceSets("BODY");

                    var components = refset.AskAllDirectMembers()
                        .OfType<Component>()
                        .ToArray();

                    var bodies = refset.AskAllDirectMembers()
                        .OfType<Body>()
                        .Where(b => b.IsSolidBody)
                        .ToArray();

                    var edges = bodies.SelectMany(b => b.GetEdges()).ToArray();
                    var faces = bodies.SelectMany(b => b.GetFaces()).ToArray();

                    var objects = refset.AskAllDirectMembers()
                        .Except(components)
                        .Except(bodies)
                        .Except(edges)
                        .Except(faces)
                        .OfType<NXObject>()
                        .ToArray();

                    if (objects.Length == 0)
                        continue;

                    refset.RemoveObjectsFromReferenceSet(objects);

                }
        }
    }
}
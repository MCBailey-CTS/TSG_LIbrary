using System;
using System.Linq;
using System.Windows.Forms;
using NXOpen;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities
{
    [Obsolete]
    public class CastingHalfMoons : IDesignCheck
    {
        public bool IsPartValidForCheck(Part part, out string message)
        {
            //message = "";
            return new CastingChildren().IsPartValidForCheck(part, out message);
        }

        public bool PerformCheck(Part part, out TreeNode result_node)
        {
            result_node = part.__TreeNode();
            return false;
        }


        [Obsolete]
        private static bool CheckMoons(Face face1, Face face2)
        {
            // In order to check to see if these faces are planar, then we need to make a coordinate system from Face1's Normal.
            // Then map one of the Positions on Face2. Then check the mapped positions.X value is 0. If it's 0 then return true.
            //__display_part_.WCS.SetOriginAndMatrix(face1.GetEdges()[0]._StartPoint(), new Snap.Orientation(face1._NormalVector()));
            //return System.Math.Abs(CoordinateSystem.MapAcsToWcs(face2.GetEdges()[0]._StartPoint()).Z) < .0001;
            throw new NotImplementedException();
        }

        [Obsolete]
        public TreeNode PerformCheck(Part part)
        {
            throw new NotImplementedException();
#pragma warning disable CS0162 // Unreachable code detected
            const string halfMoonConst = "HALFMOON";
#pragma warning restore CS0162 // Unreachable code detected

            var allHalfMoonFacesInPart = part.Bodies
                .ToArray()
                .SelectMany(body => body.GetFaces())
                .Where(face => face.Name.StartsWith(halfMoonConst))
                .ToArray();

            //switch (allHalfMoonFacesInPart.Length)
            //{
            //    case 0:
            //        yield return new DesignCheckResult(false, part, this,
            //            new ObjectNode("Could not find any named half moons."));
            //        yield break;
            //    case 1:
            //    case 2:
            //    case 3:
            //        yield return new DesignCheckResult(false, part, this,
            //            new ObjectNode("Only found " + allHalfMoonFacesInPart.Length + " face(s)."));
            //        yield break;
            //    case 4:
            //        // This is valid and we want to get out.
            //        break;
            //    default:
            //        yield return new DesignCheckResult(false, part, this,
            //            new ObjectNode($"Found too many half moon faces: {allHalfMoonFacesInPart.Length}."));
            //        yield break;
            //}

            if(allHalfMoonFacesInPart.Any(face => face.SolidFaceType == Face.FaceType.Planar))
            {
                //yield return new DesignCheckResult(false, part, this,
                //    new ObjectNode("Found at least one face named half moon that is not a Planar Face."));
                //yield break;
            }

            var dictionary = allHalfMoonFacesInPart.ToLookup(face => face.Name);

            var halfMoons1 = dictionary[halfMoonConst + "1"].ToArray();

            var halfMoons2 = dictionary[halfMoonConst + "2"].ToArray();

            // Checks to make sure that the program found 2 and only 2 NXOpen.Face's named HALFMOON1.
            if(halfMoons1.Length != 2)
            {
                //yield return new DesignCheckResult(false, part, this, new ObjectNode(halfMoons1.Length == 1
                //    ? "Only found one face named " + halfMoonConst + "1."
                //    : "Found three faces named " + halfMoonConst + "1."));
                //yield break;
            }

            // Checks to make sure that the program found 2 and only 2 NXOpen.Face's named HALFMOON2.
            if(halfMoons2.Length != 2)
            {
                //yield return new DesignCheckResult(false, part, this, new ObjectNode(halfMoons2.Length == 1
                //    ? "Only found one face named " + halfMoonConst + "2."
                //    : "Found three faces named " + halfMoonConst + "2."));
                //yield break;
            }

            // Checks to make sure that the normal of all the half moon faces is parallel to the absolute XY plane of the displayed snapPart.
            if(allHalfMoonFacesInPart.Any(halfMoonFace =>
                   !halfMoonFace.__NormalVector().__IsPerpendicularTo(__Vector3dZ())))
            {
                //yield
                //    return new DesignCheckResult(false, part, this,
                //        new ObjectNode(
                //            "Found at least one half moon face whose normal isn't parallel to the absolute Z Axis of the displayed snapPart."));
                //yield break;
            }

            if(!CheckMoons(halfMoons1[0], halfMoons1[1]))
            {
                //yield
                //    return new DesignCheckResult(false, part, this,
                //        new ObjectNode("The half moon 1 faces are not aligned properly."));
                //yield break;
            }

            //yield return CheckMoons(halfMoons2[0], halfMoons2[1])
            //    ? new DesignCheckResult(true, part, this)
            //    : new DesignCheckResult(false, part, this,
            //        new ObjectNode("The half moon 2 faces are not aligned properly."));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using CTS_Library;
using CTS_Library.Extensions;
using CTS_Library.Utilities;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Extensions;
using TSG_Library.Geom;
using Vector = NXOpen.Vector3d;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    [Obsolete]
    public class MirrorSmartStockEjector : ILibraryComponent
    {
        public bool IsLibraryComponent(Component component)
        {
            if (!component.HasUserAttribute("LIBRARY", NXObject.AttributeType.String, -1))
            {
                return false;
            }

            return component.GetUserAttributeAsString("LIBRARY", NXObject.AttributeType.String, -1).ToUpper() == "SMART STOCK EJECTORS";
        }

        public void Mirror(Surface.Plane plane, Component mirroredComp, ExtractFace originalLinkedBody, Component fromComp, IDictionary<TaggedObject, TaggedObject> dict)
        {

            ExtractFace extractFace = (ExtractFace)dict[originalLinkedBody];
            Point3d val = fromComp.__Origin().__Mirror(plane);
            Matrix3x3 val2 = fromComp.__Orientation().__Mirror(plane);
            Vector axisY = val2.__AxisY();
            Vector val3 = val2.__AxisX().__Negate();
            val2 = axisY.__ToMatrix3x3(val3);
            PartLoadStatus loadStatus;
            Component newFromComp = fromComp._OwningPart().ComponentAssembly.AddComponent(fromComp._Prototype(), "Entire Part", fromComp.Name, val, val2, fromComp.Layer, out loadStatus);
            ExtractFaceBuilder extractFaceBuilder = originalLinkedBody._OwningPart().Features.CreateExtractFaceBuilder(originalLinkedBody);
            Body[] array = extractFaceBuilder.ExtractBodyCollector.GetObjects().OfType<Body>().ToArray();
            if (array.Length == 0)
            {
                throw new InvalidOperationException("Unable to find bodies for smart key");
            }

            extractFaceBuilder.Destroy();
            loadStatus.Dispose();
            Globals._WorkPart = mirroredComp._Prototype();
            Globals._WorkComponent = mirroredComp;
            Session.UndoMarkId featureEditMark = Globals._Session.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
            EditWithRollbackManager editWithRollbackManager = Globals._WorkPart.Features.StartEditWithRollbackManager(extractFace, featureEditMark);
            using (new Destroyer(editWithRollbackManager))
            {
                extractFaceBuilder = Globals._WorkPart.Features.CreateExtractFaceBuilder(extractFace);
                newFromComp._ReferenceSet("Entire Part");
                using (new Destroyer(extractFaceBuilder))
                {
                    IList<Body> source = array.Select((Body originalBody) => (Body)newFromComp.FindOccurrence(originalBody)).ToList();
                    SelectionIntentRule selectionIntentRule = Globals._WorkPart.ScRuleFactory.CreateRuleBodyDumb(source.ToArray());
                    extractFaceBuilder.ExtractBodyCollector.ReplaceRules(new SelectionIntentRule[1] { selectionIntentRule }, createRulesWoUpdate: false);
                    extractFaceBuilder.Associative = true;
                    extractFaceBuilder.CommitFeature();
                }
            }

            newFromComp._ReferenceSet("BODY");
        }
    }




}
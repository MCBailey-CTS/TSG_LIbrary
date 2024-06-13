using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Extensions;
using TSG_Library.Geom;
using Vector = NXOpen.Vector3d;
using static TSG_Library.Extensions.__Extensions_;
using TSG_Library.Disposable;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public class MirrorSmartKey : ILibraryComponent
    {
        public bool IsLibraryComponent(Component component)
        {
            if (!component.HasUserAttribute("LIBRARY", NXObject.AttributeType.String, -1))
            {
                return false;
            }

            return component.GetUserAttributeAsString("LIBRARY", NXObject.AttributeType.String, -1).ToUpper() == "SMART KEY METRIC";
        }

        public void Mirror(Surface.Plane plane, Component mirroredComp, ExtractFace originalLinkedBody, Component fromComp, IDictionary<TaggedObject, TaggedObject> dict)
        {
            
            ExtractFace extractFace = (ExtractFace)dict[originalLinkedBody];
            var val = fromComp.__Origin()._Mirror(plane);
            Matrix3x3 val2 = fromComp.__Orientation().__Mirror(plane);
            Vector axisY = val2.__AxisY();
            Vector val3 = val2.__AxisX().__Negate();
            val2 = axisY.__ToMatrix3x3(val3);
            PartLoadStatus loadStatus;
            Component newFromComp = fromComp.__OwningPart().ComponentAssembly.AddComponent(fromComp.__Prototype(), "Entire Part", fromComp.Name, val, val2, fromComp.Layer, out loadStatus);
            ExtractFaceBuilder extractFaceBuilder = originalLinkedBody.__OwningPart().Features.CreateExtractFaceBuilder(originalLinkedBody);
            Body[] array = extractFaceBuilder.ExtractBodyCollector.GetObjects().OfType<Body>().ToArray();
            if (array.Length == 0)
            {
                throw new InvalidOperationException("Unable to find bodies for smart key");
            }

            extractFaceBuilder.Destroy();
            loadStatus.Dispose();
            __work_part_ = mirroredComp.__Prototype();
            __work_component_ = mirroredComp;
            Session.UndoMarkId featureEditMark = session_.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
            EditWithRollbackManager editWithRollbackManager = __work_part_.Features.StartEditWithRollbackManager(extractFace, featureEditMark);
            using (new Destroyer(editWithRollbackManager))
            {
                extractFaceBuilder = __work_part_.Features.CreateExtractFaceBuilder(extractFace);
                newFromComp.__ReferenceSet("Entire Part");
                using (new Destroyer(extractFaceBuilder))
                {
                    IList<Body> source = array.Select((Body originalBody) => (Body)newFromComp.FindOccurrence(originalBody)).ToList();
                    SelectionIntentRule selectionIntentRule = __work_part_.ScRuleFactory.CreateRuleBodyDumb(source.ToArray());
                    extractFaceBuilder.ExtractBodyCollector.ReplaceRules(new SelectionIntentRule[1] { selectionIntentRule }, createRulesWoUpdate: false);
                    extractFaceBuilder.Associative = true;
                    extractFaceBuilder.CommitFeature();
                }
            }

            newFromComp.__ReferenceSet("BODY");
        }
    }




}
using System;
using System.Collections.Generic;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    [Obsolete]
    public class MirrorSmartStandardLiftersGuidedKeepersMetric : ILibraryComponent
    {
        public bool IsLibraryComponent(Component component)
        {
            if (!component.HasUserAttribute("LIBRARY", NXObject.AttributeType.String, -1))
                return false;

            return component.GetUserAttributeAsString("LIBRARY", NXObject.AttributeType.String, -1).ToUpper() == "SMART STANDARD LIFTERS GUIDED KEEPERS METRIC";
        }

        [Obsolete]
        public void Mirror(Surface.Plane plane, Component mirroredComp, ExtractFace originalLinkedBody, Component fromComp, IDictionary<TaggedObject, TaggedObject> dict)
        {
            throw new NotImplementedException();
            //ExtractFace extractFace = (ExtractFace)dict[originalLinkedBody];
            //Point3d val = fromComp.__Origin().__Mirror(plane);
            //Orientation val2 = fromComp._Orientation()._Mirror(plane);
            //Vector axisY = val2.AxisY;
            //Vector val3 = -val2.AxisX;
            //val2 = new Orientation(axisY, val3);
            //PartLoadStatus loadStatus;
            //Component newFromComp = fromComp._OwningPart().ComponentAssembly.AddComponent(fromComp._Prototype(), "Entire Part", fromComp.Name, Position.op_Implicit(val), Orientation.op_Implicit(val2), fromComp.Layer, out loadStatus);
            //ExtractFaceBuilder extractFaceBuilder = originalLinkedBody._OwningPart().Features.CreateExtractFaceBuilder(originalLinkedBody);
            //Body[] array = extractFaceBuilder.ExtractBodyCollector.GetObjects().OfType<Body>().ToArray();
            //if (array.Length == 0)
            //{
            //    throw new InvalidOperationException("Unable to find bodies for smart key");
            //}

            //extractFaceBuilder.Destroy();
            //loadStatus.Dispose();
            //Globals._WorkPart = mirroredComp._Prototype();
            //Globals._WorkComponent = mirroredComp;
            //Session.UndoMarkId featureEditMark = Globals._Session.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
            //EditWithRollbackManager editWithRollbackManager = Globals._WorkPart.Features.StartEditWithRollbackManager(extractFace, featureEditMark);
            //using (new Destroyer(editWithRollbackManager))
            //{
            //    extractFaceBuilder = Globals._WorkPart.Features.CreateExtractFaceBuilder(extractFace);
            //    newFromComp._ReferenceSet("Entire Part");
            //    using (new Destroyer(extractFaceBuilder))
            //    {
            //        IList<Body> source = array.Select((Body originalBody) => (Body)newFromComp.FindOccurrence(originalBody)).ToList();
            //        SelectionIntentRule selectionIntentRule = Globals._WorkPart.ScRuleFactory.CreateRuleBodyDumb(source.ToArray());
            //        extractFaceBuilder.ExtractBodyCollector.ReplaceRules(new SelectionIntentRule[1] { selectionIntentRule }, createRulesWoUpdate: false);
            //        extractFaceBuilder.Associative = true;
            //        extractFaceBuilder.CommitFeature();
            //    }
            //}

            //newFromComp._ReferenceSet("BODY");
        }
    }




}
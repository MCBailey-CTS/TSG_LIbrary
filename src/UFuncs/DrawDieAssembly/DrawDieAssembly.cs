//using NXOpen_;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text.RegularExpressions;
//using TSG_Library.Utilities;

//namespace TSG_Library.Ufuncs
//{
//    public class DrawDieAssembly
//    {
//        private readonly Ucf _ucf;

//        private readonly GFolder _folder;

//        private readonly double _blank010SizeInchX;

//        private readonly double _blank010SizeInchY;

//        // ReSharper disable once NotAccessedField.Local
//        private readonly double _blank010SizeInchZ;

//        private readonly double _layout020SizeInchX;

//        private readonly double _layout020SizeInchY;

//        private readonly double _layout020SizeInchZ;

//        private readonly Position_ _blank010BoundingCenter;

//        private readonly Position_ _layout020BoundingCenter;

//        private readonly Part _op020LayoutPart;

//        private readonly NXOpen.Assemblies.Component _op020LayoutComp;

//        private readonly Part _stripControl;

//        private readonly double _materialThickness;

//        private const string UpperCavity = "UPPER_CAVITY";

//        public const string LowerBinder = "LOWER_BINDER";

//        public const string LowerPost = "LOWER_POST";

//        private readonly IDictionary<string, List<KeyValuePair<ReferenceSet, Feature.BooleanType>>> _refsetDictionary;


//        public DrawDieAssembly()
//        {
//            // todo: need to find the dieset control. If one doesn't exist, throw exception.
//            // todo: need to get a_uprDieShoeDeckHeight expression from dieset control. If one doesn't exist, throw exception.
//            // todo: use the value of the a_uprDieShoeDeckHeight expression as the Z height for the origin in the lower binder.

//            // Initializes the Ucf. 
//            //_ucf = new Ucf("U:\\nxFiles\\UfuncFiles\\DrawDie.ucf");

//            _ucf = new Ucf(@"G:\CTS\junk\0Ufunc Testing\DrawDie.ucf");

//            // Gets the regex pattern for finding the op 020 layout.
//            string op020LayoutRegex = _ucf["OP_020_LAYOUT_REGEX"].Single();

//            // Initializes a GFolderWithCtsNumber for the current displayed part.
//            _folder = GFolder.create_or_null(__work_part_) ?? throw new Exception("Must be ran from a valid GFolderWithCtsNumber.");

//            // Gets the regex pattern for finding the op 020 000.
//            // ReSharper disable once InconsistentNaming
//            string op020_000Regex = _ucf["OP_020_000_REGEX"].Single();

//            // Checks to make sure that this program is being run from an OP-020.
//            if (!Regex.IsMatch(Globals.__display_part_.Leaf, op020_000Regex))
//                throw new Exception("Must be ran from xxxx-020-000");

//            // Trys and finds an op-020-layout amongst the DisplayParts immediate children..
//            _op020LayoutComp = Globals.__display_part_._Children().FirstOrDefault(comp => Regex.IsMatch(comp.DisplayName, op020LayoutRegex, RegexOptions.IgnoreCase))
//                               // PreCheck
//                               // If {op020LayoutComp} is null, then that means there isn't an op-020-layout. We need to stop.
//                               ?? throw new Exception("DisplayPart does not have an Op-020-layout.");

//            // Gets the strip control of the current job.
//            _stripControl = Globals.IsStringLoaded(_folder.StripControl)
//                // Find the part that represents the strip control for this given GFolderWithCtsNumber.
//                ? Part_.FindByName(_folder.StripControl)
//                // If the strip control is not loaded, we just go ahead and load it form the GFolderWithCtsNumber.
//                : Part_.OpenPart(_folder.StripControl);

//            // PreCheck
//            // Find the expression labeled "M".
//            string expName = _ucf["STRIP_CONTROL_MATERIAL_TITLE"].Single();
//            _materialThickness = _stripControl.Expressions.ToArray().SingleOrDefault(expression => expression.Name == expName)?.Value
//                                 // If the expression was not able to be found we need to stop.
//                                 ?? throw new Exception($"Strip Control did not contain an expression titled \"{expName}\".");

//            // Checks to see if the op020LayoutComp.Prototype is loaded.
//            if (!Globals.IsStringLoaded(_op020LayoutComp.DisplayName))
//                // Loads it if is not.
//                Part_.OpenPart(_folder.Layout("020"));

//            // Find the part that represents the Layout-Op-020 for this given GFolderWithCtsNumber. 
//            _op020LayoutPart = Part_.FindByName(_op020LayoutComp.DisplayName);

//            // Initializes the Dictionary.
//            // Represents the collections of the reference sets to use for each section to be made.
//            _refsetDictionary = new Dictionary<string, List<KeyValuePair<ReferenceSet, Feature.BooleanType>>>();

//            Dictionary<string, IDictionary<int, List<KeyValuePair<Regex, Feature.BooleanType>>>> tempDictionary = new Dictionary<string, IDictionary<int, List<KeyValuePair<Regex, Feature.BooleanType>>>>();

//            // iterate through the lines and parse out the operations to perform.
//            foreach (string str in _ucf["REFERENCE_BOOLEANS"])
//            {
//                Match match = Regex.Match(str, "^{(?<PartKey>.+)},{(?<ReferenceSetName>.+)},{(?<Boolean>.+)},{(?<Index>.+)}$");
//                if (!match.Success) throw new Exception("failed regex expression");

//                // Gets the part key.
//                string partKey = match.Groups["PartKey"].Value;

//                // If the dictionary doesn't contain the given part key. Need to add it.
//                if (!tempDictionary.ContainsKey(partKey))
//                    tempDictionary[partKey] = new Dictionary<int, List<KeyValuePair<Regex, Feature.BooleanType>>>();

//                // Gets the index.
//                int index = int.Parse(match.Groups["Index"].Value);

//                // If the sub dictionary doesn't contain the given index. Need to add it.
//                if (!tempDictionary[partKey].ContainsKey(index))
//                    tempDictionary[partKey][index] = new List<KeyValuePair<Regex, Feature.BooleanType>>();

//                // Gets the regex expression.
//                Regex regex = new Regex(match.Groups["ReferenceSetName"].Value, RegexOptions.IgnoreCase);

//                // Gets the boolean type to perform.
//                Feature.BooleanType booleanType = (Feature.BooleanType)Enum.Parse(typeof(Feature.BooleanType), match.Groups["Boolean"].Value);

//                // add the regex and boolean to the sub list in the sub dictionary.
//                tempDictionary[partKey][index].Add(new KeyValuePair<Regex, Feature.BooleanType>(regex, booleanType));
//            }

//            print_(tempDictionary.Count);


//            foreach (KeyValuePair<string, IDictionary<int, List<KeyValuePair<Regex, Feature.BooleanType>>>> pair in tempDictionary)
//            {
//                // The key string that represents either the 
//                // Lower_Post, Upper_Cavity, Lower_Binder.
//                string cavityBinderPostKey = pair.Key;

//                // If the dictionary doesn't contain the key, then add the key and create a new list for the value.
//                if (!_refsetDictionary.ContainsKey(cavityBinderPostKey))
//                    _refsetDictionary.Add(cavityBinderPostKey, new List<KeyValuePair<ReferenceSet, Feature.BooleanType>>());

//                // If done correctly, then the Dictionary contained within the {pair.Value} should already be sorted.
//                for (int index = 0; index < pair.Value.Keys.Count; index++)
//                {
//                    // Sets a flag variable to mark whether or not to continue on to use the next index.
//                    bool foundAllRefsets = true;

//                    // The list of regex to match for the current index for the current {cavityBinderPostKey}.
//                    foreach (KeyValuePair<Regex, Feature.BooleanType> regexBoolean in tempDictionary[cavityBinderPostKey][index])
//                    {
//                        // Gets the reference set that matches the given regex pattern.
//                        // Can be null.
//                        ReferenceSet referenceSet = _op020LayoutPart.MatchReferenceSet(regexBoolean.Key);

//                        if (referenceSet != null)
//                        {
//                            // Adds the specified {ReferenceSet,Boolean} to the specified list of reference sets for the given {cavityBinderPostKey}. 
//                            _refsetDictionary[cavityBinderPostKey].Add(new KeyValuePair<ReferenceSet, Feature.BooleanType>(referenceSet, regexBoolean.Value));
//                            continue;
//                        }

//                        // If the reference set is null then a reference set was not able to be found.
//                        // If ({index} == {pair.Value.Keys.Count} - 1) then we want to throw an exception because there is not another 
//                        // a set of regex expressions to look for.
//                        if (index == pair.Value.Keys.Count - 1)
//                        {

//                            //throw new InvalidOperationException($"Unable to match reference set for the {cavityBinderPostKey}, {regexBoolean.Key}.");
//                        }

//                        // Clears the list of reference sets already found.
//                        _refsetDictionary[cavityBinderPostKey].Clear();

//                        // Tells the parent nested loop to move on to the next index.
//                        foundAllRefsets = false;

//                        // If were are not on the last index, then we wan to just continue on to the next one.
//                        break;
//                    }

//                    // If we found all the reference sets for a given set then we want to move onto the next Draw Die Section.
//                    if (foundAllRefsets)
//                        break;
//                }
//            }

//            // Gets all the dimensions that make up the bodies on layer 10.
//            Tuple<Position_ , double[]> tuple1 = _op020LayoutPart.AskBoxDimensionsFromBodiesOnLayer(10);
//            _blank010SizeInchX = tuple1.Item2[0];
//            _blank010SizeInchY = tuple1.Item2[1];
//            _blank010SizeInchZ = tuple1.Item2[2];
//            _blank010BoundingCenter = tuple1.Item1;

//            // Gets all the dimensions that make up the bodies on layer 20.
//            Tuple<Position_ , double[]> tuple2 = _op020LayoutPart.AskBoxDimensionsFromBodiesOnLayer(20);
//            _layout020SizeInchX = tuple2.Item2[0];
//            _layout020SizeInchY = tuple2.Item2[1];
//            _layout020SizeInchZ = tuple2.Item2[2];
//            _layout020BoundingCenter = tuple2.Item1;
//        }


//        public void Execute()
//        {
//            Part upperCavity, lowerPost, lowerBinder;

//            Part usp = Globals.FindPartByRegex($"{_folder.CustomerNumber}-020-usp") ?? throw new Exception("Could not find a part that matched the given expression");

//            Part lsp = Globals.FindPartByRegex($"{_folder.CustomerNumber}-020-lsp") ?? throw new Exception("Could not find a part that matched the given expression");

//            using (new ReferenceSetReset(_op020LayoutComp))
//            using (session_.using_display_reset())
//            {
//                {
//                    // Upper Cavity
//                    double blockLength = _blank010SizeInchX + 1;
//                    double blockWidth = _blank010SizeInchY + 1;
//                    double blockHeight = _layout020SizeInchZ + 3.5;
//                    Snap.Orientation blockOrientation = new Snap.Orientation(-Snap.Orientation.Identity.AxisY, -Snap.Orientation.Identity.AxisX);
//                    const int blockColor = 134;
//                    double newMinX = (_layout020BoundingCenter.X + blockLength) / 2;
//                    double newMinY = (_layout020BoundingCenter.Y + blockWidth) / 2;
//                    double temp = blockLength;
//                    blockLength = blockWidth;
//                    blockWidth = temp;
//                    Snap.Vector vector = new Snap.Vector(_layout020BoundingCenter.X, _layout020BoundingCenter.Y, 0);
//                    Position_ blockOrigin = new Position_ (newMinX, newMinY, 3.5 + _materialThickness / 25.4) + vector / 2;
//                    KeyValuePair<ReferenceSet, Feature.BooleanType>[] referenceSetNames = _refsetDictionary[UpperCavity].ToArray();
//                    upperCavity = Create(usp,
//                        blockOrigin,
//                        blockOrientation,
//                        blockLength,
//                        blockWidth,
//                        blockHeight,
//                        int.Parse(_ucf["CompName_UpperCavity"].Single()),
//                        _ucf["Material_UpperCavity"].Single(),
//                        blockColor,
//                        referenceSetNames);
//                }

//                {
//                    // Lower Binder
//                    double blockLength = _blank010SizeInchX + 1;
//                    double blockWidth = _blank010SizeInchY + 1;
//                    double blockHeight = double.Parse(_ucf["HeightZCLowerBinder"].Single()) + _layout020SizeInchZ;
//                    Snap.Orientation blockOrientation = Snap.Orientation.Identity;
//                    const int blockColor = 144;
//                    double newMinX = (_blank010BoundingCenter.X - blockLength) / 2;
//                    double newMinY = (_blank010BoundingCenter.Y - blockWidth) / 2;
//                    Snap.Vector vector = new Snap.Vector(_blank010BoundingCenter.X, _blank010BoundingCenter.Y, 0);
//                    Position_ blockOrigin = new Position_ (newMinX, newMinY, -blockHeight) + vector / 2;
//                    KeyValuePair<ReferenceSet, Feature.BooleanType>[] referenceSetNames = _refsetDictionary[LowerBinder].ToArray();
//                    lowerBinder = Create(lsp,
//                        blockOrigin,
//                        blockOrientation,
//                        blockLength,
//                        blockWidth,
//                        blockHeight,
//                        int.Parse(_ucf["CompName_LowerBinder"].Single()),
//                        _ucf["CompName_LowerBinder"].Single(),
//                        blockColor,
//                        referenceSetNames);
//                }

//                {
//                    // Lower Post
//                    double blockLength = _layout020SizeInchX + 1;
//                    double blockWidth = _layout020SizeInchY + 1;
//                    double blockHeight = _layout020SizeInchZ + 1.25;
//                    Snap.Orientation blockOrientation = Snap.Orientation.Identity;
//                    const int blockColor = 134;
//                    double newMinX = (_layout020BoundingCenter.X - blockLength) / 2;
//                    double newMinY = (_layout020BoundingCenter.Y - blockWidth) / 2;
//                    Snap.Vector vector = new Snap.Vector(_layout020BoundingCenter.X, _layout020BoundingCenter.Y, 0);
//                    Position_ blockOrigin = new Position_ (newMinX, newMinY, -blockHeight) + vector / 2;
//                    KeyValuePair<ReferenceSet, Feature.BooleanType>[] referenceSetNames = _refsetDictionary[LowerPost].ToArray();
//                    lowerPost = Create(lsp,
//                        blockOrigin,
//                        blockOrientation,
//                        blockLength,
//                        blockWidth,
//                        blockHeight,
//                        int.Parse(_ucf["CompName_LowerPost"].Single()),
//                        _ucf["Material_LowerPost"].Single(),
//                        blockColor,
//                        referenceSetNames);
//                }

//                Part_ displayPartSp = Globals.__display_part_;

//                // Gets the upper cavity component in the assembly.
//                NXOpen.Assemblies.Component upperCavityComp = displayPartSp.RootComponent.Descendants.Single(comp => comp.NXOpenComponent.DisplayName == upperCavity.Leaf);

//                // Gets the lower post component in the assembly.
//                NXOpen.Assemblies.Component lowerPostComp = displayPartSp.RootComponent.Descendants.Single(comp => comp.NXOpenComponent.DisplayName == lowerPost.Leaf);

//                // Gets the lower binder component in the assembly.
//                NXOpen.Assemblies.Component lowerBinderComp = displayPartSp.RootComponent.Descendants.Single(comp => comp.NXOpenComponent.DisplayName == lowerBinder.Leaf);
//                Snap.Geom.Box3d lowerBinderBoundingBox = lowerBinderComp.Box;

//                using (session_.using_display_reset())
//                {
//                    AddKeys(usp, upperCavityComp.Box, Side.Top, int.Parse(_ucf.SingleValue("CompName_UpperCavity")));
//                }

//                using (session_.using_display_reset())
//                {
//                    AddKeys(lsp, lowerPostComp.Box, Side.Bottom, int.Parse(_ucf.SingleValue("CompName_LowerPost")));
//                }

//                using (session_.using_display_reset())
//                {
//                    AddStandOffs(lsp, lowerBinderBoundingBox, Side.Bottom, int.Parse(_ucf.SingleValue("CompName_LowerBinder")));
//                }

//                using (session_.using_display_reset())
//                {
//                    AddStandOffs(lsp, lowerBinderComp.Box, Side.Top, int.Parse(_ucf.SingleValue("CompName_LowerBinder")));
//                }

//                BoundingBox boundingBoxLayer10 = Snap.Geom.Box3d.Combine(_op020LayoutPart.Bodies.ToArray().Where(body => body.IsSolidBody && body.Layer == 10).Select(body => Snap.NX.Body.Wrap(body.Tag).Box).ToArray());

//                using (session_.using_display_reset())
//                {
//                    AddGauges(lsp, boundingBoxLayer10 / 25.4, Side.Bottom, lowerBinderBoundingBox, new Snap.Vector(0, 0, 2.75), int.Parse(_ucf.SingleValue("CompName_LowerBinder")));
//                }
//            }
//        }

//        public Part Create(IDrawDieComponent dieComponent)
//        {
//            return Create(dieComponent.PartToAddTo,
//                dieComponent.Origin,
//                dieComponent.BlockOrientation,
//                dieComponent.BlockLength,
//                dieComponent.BlockWidth,
//                dieComponent.BlockHeight,
//                dieComponent.MinimumDetailNumber,
//                dieComponent.MaterialValue,
//                dieComponent.BlockColor,
//                dieComponent.ReferenceSetBooleanPairs.Select(tuple => new KeyValuePair<ReferenceSet, Feature.BooleanType>(tuple.Item1, tuple.Item2)).ToArray());
//        }

//        /// <summary>
//        /// Assuming that this method is called when the DisplayPart is "XXXX-020-000".
//        /// Creates a part/component, under a given assembly, with a set block size, and set of reference sets to link into. 
//        /// </summary>
//        /// <param name="partToAddTo">The part to add the "to be created" part as an immediate child component.</param>
//        /// <param name="blockOrigin">The origin of the block.</param>
//        /// <param name="blockOrientation">The orientation of the block.</param>
//        /// <param name="blockLength">The length of the block.</param>
//        /// <param name="blockWidth">The width of the block.</param>
//        /// <param name="blockHeight">The height of the block.</param>
//        /// <param name="componentName">The name of the ucf header to get the name of what the component child will be named when the new part is added as an immediate child.</param>
//        /// <param name="materialValue">The name of the ucf header to get the material attribute to apply to the new part.</param>
//        /// <param name="blockColor">The color of the block.</param>
//        /// <param name="referenceSetNames">The name of the references sets of the layout file to use to link bodies from.</param>
//        private Part Create(
//             BasePart partToAddTo,
//            Position_ blockOrigin,
//             Snap.Orientation blockOrientation,
//            double blockLength,
//            double blockWidth,
//            double blockHeight,
//            int componentName,
//             string materialValue,
//            int blockColor,
//             params KeyValuePair<ReferenceSet, Feature.BooleanType>[] referenceSetNames)
//        {
//            // We want to reset this back to the display part every time.
//            using (new ReferenceSetReset(_op020LayoutComp))
//            using (session_.using_display_reset())
//            {
//                // Gets the string that the to be added component's name should be set too.
//                int newComponentName = componentName; //int.Parse(_ucf[componentNameUcf].Single());

//                //Constructs the path of the part that will be the defining prototype for the component that is to be added to {partToAddTo}.
//                string newPartPath = $"{Path.GetDirectoryName(__work_part_.FullPath)}\\{_folder.CustomerNumber}-020-{newComponentName}.prt";

//                // Finds the next detail number that doesn't exist. 
//                while (File.Exists(newPartPath)) newPartPath = $"{Path.GetDirectoryName(__work_part_.FullPath)}\\{_folder.CustomerNumber}-020-{newComponentName++}.prt";

//                // Copies the seed_base to the {newPartPath}.
//                File.Copy(_ucf.SingleValue("SEED_PART"), newPartPath);

//                // Opens the {newPart}.
//                Part_ newPart = Part_.OpenPart(newPartPath);

//                Block dynamicBlock = CreateDynamicBlock(newPart, materialValue, blockOrigin, blockOrientation, blockLength, blockWidth, blockHeight, blockColor);

//                // DisplayPart is reset to 000.

//                // Adds a new part occurence of the type {newPart} to the assembly of {partToAddTo}.
//                NXOpen.Assemblies.Component newComponent = partToAddTo.AddComponent(newPart, newComponentName + "");

//                // Sets the newly created component to be the WorkComponent.
//                //Globals.__work_component_OrNull = Globals.__display_part_.ComponentAssembly.MapComponentsFromSubassembly(newComponent).Single();

//                // At this point we can link in the bodies and perform subtractions on the dynamic block in the newly created part.

//                // Iterates through the reference set names of the {_op020LayoutComp}.
//                foreach (KeyValuePair<ReferenceSet, Feature.BooleanType> refsetName in referenceSetNames)
//                {
//                    // Sets {_op020LayoutComp} to the specified reference set.
//                    // We don't need to check if the part has a reference set named that, because that is taken care of with pre check.
//                    Globals.__display_part_.ComponentAssembly.ReplaceReferenceSet(_op020LayoutComp, refsetName.Key.Name);

//                    // Gets all the solid bodies contained within the {_op020LayoutComp}.
//                    IEnumerable<Body> bodies = _op020LayoutComp._MemberSolidBodies();

//                    // Creates the linked body.
//                    ExtractFace linkedBody = bodies.CreateLinkedBody();

//                    // Sets the name of the linked body to the name of ref set that was used to create it.
//                    linkedBody.SetName(refsetName.Key.Name);

//                    // Gets the body from the newly created linked body.
//                    // This will be the tool body in the upcoming boolean operation.
//                    // There should only be one body because we are using {ExtractFaceBuilder.NXOpen.Features.FeatureOptionType.OneNXOpen.Features.FeatureForAllBodies}
//                    // as our NXOpen.Features.Feature option type.
//                    Body toolBody = linkedBody.GetBodies().Single();

//                    // Gets the body that makes up the newly created dynamic block.
//                    // This will serve as the target body for the upcoming boolean operation.
//                    Body targetBody = dynamicBlock.GetBodies()[0];

//                    //// Checks to make sure that there is an interference between the two bodies.
//                    //if (Globals.CheckInterference(targetBody, toolBody)[0] != Globals.CheckInterferenceEnum.Interference)
//                    //{
//                    //    // If you make it here, then the two bodies have no interference what so ever.
//                    //    // At this point we want to inform the uses, leave the linked body and just continue on.
//                    //    print_("Bodies did not intersect, boolean operation not performed.");
//                    //    continue;
//                    //}

//                    // ReSharper disable once SwitchStatementMissingSomeCases
//                    switch (refsetName.Value)
//                    {
//                        case Feature.BooleanType.Subtract:
//                            // Set the name of the {linkedBody} to the current {referenceSet}.
//                            session_.create.Subtract(dynamicBlock.GetBodies()[0], linkedBody.GetBodies()[0]);
//                            break;
//                        case Feature.BooleanType.Intersect:
//                            // Set the name of the {linkedBody} to the current {referenceSet}.
//                            session_.create.Intersect(dynamicBlock.GetBodies()[0], linkedBody.GetBodies()[0]);
//                            break;
//                        case Feature.BooleanType.Unite:
//                            // Set the name of the {linkedBody} to the current {referenceSet}.
//                            session_.create.Unite(dynamicBlock.GetBodies()[0], linkedBody.GetBodies()[0]);
//                            break;
//                        default:
//                            throw new ArgumentOutOfRangeException();
//                    }
//                }

//                return newPart;
//            }
//        }

//        private static Block CreateDynamicBlock(
//            Part partToAddDynamicBlock,
//            string materialValue,
//            Position_ blockOrigin,
//            Snap.Orientation blockOrientation,
//            double blockLength,
//            double blockWidth,
//            double blockHeight,
//            int blockColor)
//        {
//            // Sets a display reset so that we can go back to the display part assembly so that we can start linking things in.
//            using (session_.using_display_reset())
//            {
//                // Sets the newly created part to the DisplayPart.
//                Globals.__display_part_ = partToAddDynamicBlock;


//                // Creates and sets the value for the MATERIAL attribute.
//                Globals.__display_part_.SetUserAttribute("MATERIAL", -1, materialValue, Update.Option.Later);

//                // At this point we can now construct the block.

//                // Actually creates the block.
//                Snap.NX.Block dynamicBlock = session_.create.Block(blockOrigin, blockOrientation, blockLength, blockWidth, blockHeight);

//                // Sets the name of the block.
//                dynamicBlock.Name = "DYNAMIC BLOCK";

//                // Gets the body reference set that is in the newly created part.
//                ReferenceSet bodyReferenceSet = Globals.__display_part_.GetAllReferenceSets().SingleOrDefault(set => set.Name == "BODY");

//                // Creates the body reference set if one doesn't exist.
//                if (bodyReferenceSet == null)
//                {
//                    bodyReferenceSet = Globals.__display_part_.CreateReferenceSet();
//                    bodyReferenceSet.SetName("BODY");
//                }

//                // Adds the newly created body of the block to the body reference set.
//                bodyReferenceSet.AddObjectsToReferenceSet(new NXOpen.NXObject[] { dynamicBlock.Body.NXOpenBody });

//                // Colors the body of the block.

//                dynamicBlock.NXOpenDisplayableObject._SetDisplayColor(blockColor);

//                // Gets the bottom face of the block.
//           Snap.NX.     Face bottomFaceOfBlock = dynamicBlock.Faces.Single(face =>
//                    face.Edges.SelectMany(edge => new[] { edge.StartPoint, edge.EndPoint }).All(pos => Math.Abs(pos.Z - dynamicBlock.Origin.Z) < .001));

//                // Colors the bottom face of the block the yellow color that you see on all the dynamic block.
//                bottomFaceOfBlock.NXOpenDisplayableObject._SetDisplayColor(6);

//                return dynamicBlock;
//            }
//        }

//        private void AddStandOffs(BasePart partToAddTo, BoundingBox box, Side side, int compName)
//        {
//            // Gets the path to the smart key;
//            string standoffPath = _ucf.SingleValue("STANDOFF_SEED_FILE");
//            // Gets the display name of the smart key.
//            string standoffDisplayName = Path.GetFileNameWithoutExtension(standoffPath) ?? throw new Exception("Bad smart key path.");
//            // Checks to see if the smartKey is loaded.
//            if (!Globals.IsStringLoaded(standoffPath))
//                // Load the key if it is not loaded.
//                Part_.OpenPart(standoffPath);
//            // Gets the actual part that represents the smart key.
//            Part_ standoffPart = Part_.FindByName(standoffPath);

//            // todo: we are assuming the body reference set for now.
//            ReferenceSet standoffBodyRefset = standoffPart.GetAllReferenceSets().SingleOrDefault(set => set.Name == "BODY") ?? throw new Exception("Could not find a body reference set.");

//          Snap.Geom.  Box3d[] boxes = standoffBodyRefset.AskMembersInReferenceSet()
//                .OfType<Body>()
//                .Where(body => body.IsSolidBody)
//                .Select(body => Snap.NX.Body.Wrap(body.Tag).Box)
//                .ToArray();

//            Snap.Geom.Box3d bodyBoundingBox = Snap.Geom.Box3d.Combine(boxes);

//            double zDistanceToAbsolute = bodyBoundingBox.MaxZ;

//            Snap.Vector translationVector = new Snap.Vector(0, 0, 0);

//            Rectangle rectangle = box.FaceRectangle(side);

//            foreach (Position_ corner in rectangle.Corners)
//            {
//                Snap.Orientation orientation = rectangle.CornerOrientation(corner);

//                Snap.Vector vec1 = orientation.AxisZ.Unitize();

//                Snap.Vector vec2 = -Snap.Vector.AxisZ.Unitize();

//                if (vec1._IsEqualTo(vec2))
//                {
//                    translationVector = new Snap.Vector(0, 0, -zDistanceToAbsolute);
//                    orientation = new Snap.Orientation(orientation.AxisY, orientation.AxisX, -orientation.AxisZ);
//                }

//                throw new NotImplementedException();

//                //NXOpen.Assemblies.Component smartKey = partToAddTo.ComponentAssembly.AddComponent(standoffPath, "BODY", standoffDisplayName, corner + translationVector, orientation, Globals.WorkLayer, out _);

//                //// Gets the body reference set from the "partToAddTo", which in this context is the USP.
//                //ReferenceSet bodyReferenceSet = partToAddTo.GetAllReferenceSets().Single(set => set.Name == "BODY");

//                //// Adds the newly added smart key t o the body reference set.
//                //bodyReferenceSet.AddObjectsToReferenceSet(new NXOpen.NXObject[] { smartKey });
//            }

//            // Gets the string that the to be added component's name should be set too.
//            int newComponentName = compName;

//            //Constructs the path of the part that will be the defining prototype for the component that is to be added to {partToAddTo}.
//            string newPartPath = $"{Path.GetDirectoryName(__work_part_.FullPath)}\\{_folder.CustomerNumber}-020-{newComponentName}.prt";

//            // Finds the next detail number that doesn't exist. 
//            while (File.Exists(newPartPath)) newPartPath = $"{Path.GetDirectoryName(__work_part_.FullPath)}\\{_folder.CustomerNumber}-020-{newComponentName++}.prt";

//            standoffPart.SaveAs(newPartPath);

//            standoffPart.NamePartOccurrences(Globals.__display_part_, newComponentName + "");
//        }

//        private void AddKeys(BasePart partToAddTo, BoundingBox box, Side side, int compName)
//        {
//            // Gets the path to the smart key;
//            string smartKeyPath = _ucf.SingleValue("SMART_KEY_SEED_FILE");
//            // Gets the display name of the smart key.
//            string smartKeyDisplayName = Path.GetFileNameWithoutExtension(smartKeyPath) ?? throw new Exception("Bad smart key path.");
//            // Checks to see if the smartKey is loaded.
//            if (!Globals.IsStringLoaded(smartKeyPath))
//                // Load the key if it is not loaded.
//                Part_.OpenPart(smartKeyPath);
//            // Gets the actual part that represents the smart key.
//            Part_ smartKeyPart = Part_.FindByName(smartKeyPath);

//            // Gets the rectangle that represents the given side of the box.
//            Rectangle rectangle = box.FaceRectangle(side);

//            foreach (Position_ midPoint in rectangle.Midpoints)
//            {
//                // So get the unitized vector into the block from the edge that the position lies on.
//                Snap.Vector vectorIn = rectangle.VectorIn(midPoint);

//                // We want to offset the midpoint in .5 inches.
//                // That way when the smart key component is added, it will be flush with the end of the block.
//                Position_ offsetOrigin = midPoint + 1.5 * vectorIn;

//                // The vectorIn just also happens to be the direction we want the x vector of the to-be added smart key component to be pointing.
//                Snap.Vector xVector = vectorIn;

//                // Gets the z vector for the to be added smart key component.
//                // We want it to be the negative vector of actual rectangle because we want the orientation of the smart key component
//                // to be relative to the actual orientation of the block.
//                Snap.Vector zVector = -box.FaceVector(side);

//                throw new NotImplementedException();

//                //// Gets the y vector of the orientation.
//                //Snap.Vector yVector = -xVector._Cross(zVector);

//                //if (Math.Abs(vectorIn.X - 1) < .001)
//                //    offsetOrigin = offsetOrigin + .5 * yVector;
//                //else if (Math.Abs(vectorIn.X - -1) < .001)
//                //    offsetOrigin = offsetOrigin + -.5 * yVector;

//                //// Creates the origin to be used for the to-be added smart key.
//                //Snap.Orientation orientation = new Snap.Orientation(xVector, yVector);

//                //// var smartKey = partToAddTo.ComponentAssembly.AddComponent(smartKeyPath, "BODY", smartKeyDisplayName, Position.Origin, Orientation.Identity, 1, out _);
//                //NXOpen.Assemblies.Component smartKey = partToAddTo.ComponentAssembly.AddComponent(smartKeyPath, "BODY", smartKeyDisplayName, offsetOrigin, orientation, Globals.WorkLayer, out _);

//                //// Gets the body reference set from the "partToAddTo", which in this context is the USP.
//                //ReferenceSet bodyReferenceSet = partToAddTo.GetAllReferenceSets().Single(set => set.Name == "BODY");

//                //// Adds the newly added smart key to the body reference set.
//                //bodyReferenceSet.AddObjectsToReferenceSet(new NXOpen.NXObject[] { smartKey });
//            }

//            // Gets the string that the to be added component's name should be set too.
//            int newComponentName = compName;

//            //Constructs the path of the part that will be the defining prototype for the component that is to be added to {partToAddTo}.
//            string newPartPath = $"{Path.GetDirectoryName(__work_part_.FullPath)}\\{_folder.CustomerNumber}-020-{newComponentName}.prt";

//            // Finds the next detail number that doesn't exist. 
//            while (File.Exists(newPartPath)) newPartPath = $"{Path.GetDirectoryName(__work_part_.FullPath)}\\{_folder.CustomerNumber}-020-{newComponentName++}.prt";

//            smartKeyPart.SaveAs(newPartPath);

//            smartKeyPart.NamePartOccurrences(Globals.__display_part_, newComponentName + "");
//        }

//        // ReSharper disable once UnusedParameter.Local
//        private void AddGauges(BasePart partToAddTo, BoundingBox boundingBox, Side side, BoundingBox lowerBinderBoundingBox, Snap.Vector vectorOffset, int compName)
//        {
//            // Gets the path to the smart key;
//            string smartGaugePath = _ucf.SingleValue("SMART_MISUMI_NEST_GAUGES");
//            // Gets the display name of the smart key.
//            string smartGaugeDisplayName = Path.GetFileNameWithoutExtension(smartGaugePath) ?? throw new Exception("Bad smart key path.");
//            // Checks to see if the smartKey is loaded.
//            if (!Globals.IsStringLoaded(smartGaugePath))
//                // Load the key if it is not loaded.
//                Part_.OpenPart(smartGaugePath);
//            // Gets the actual part that represents the smart key.
//            Part_ smartGaugePart = Part_.FindByName(smartGaugePath);

//            // Gets the rectangle that represents the given side of the box.
//            Rectangle rectangle = boundingBox.FaceRectangle(side);

//            foreach (Position_ midPoint in rectangle.Midpoints)
//            {
//                // So get the unitized vector into the block from the edge that the position lies on.
//                Snap.Vector vectorIn = rectangle.VectorIn(midPoint);

//                // We want to offset the midpoint in .5 inches.
//                // That way when the smart key component is added, it will be flush with the end of the block.
//                Position_ offsetOrigin = new Position_ (midPoint.X, midPoint.Y, lowerBinderBoundingBox.Origin.Z + 2.75);

//                // The vectorIn just also happens to be the direction we want the x vector of the to be added smart key component to be pointing.
//                Snap.Vector xVector = vectorIn;

//                // Gets the z vector for the to be added smart key component.
//                // We want it to be the negative vector of actual rectangle because we want the orientation of the smart key component
//                // to be relative to the actual orientation of the block.
//                Snap.Vector zVector = -boundingBox.FaceVector(side);

//                throw new NotImplementedException();

//                //Snap.Vector yVector = -xVector._Cross(zVector);

//                //Snap.Orientation orientation = new Snap.Orientation(-xVector, -yVector);

//                //// Actually adds the part and gets the component.
//                //NXOpen.Assemblies.Component smartKey = partToAddTo.ComponentAssembly.AddComponent(smartGaugePath, "BODY", smartGaugeDisplayName, offsetOrigin, orientation, Globals.WorkLayer, out _);

//                //// Gets the body reference set from the "partToAddTo", which in this context is the USP.
//                //ReferenceSet bodyReferenceSet = partToAddTo.GetAllReferenceSets().Single(set => set.Name == "BODY");

//                //// Adds the newly added smart key to the body reference set.
//                //bodyReferenceSet.AddObjectsToReferenceSet(new NXOpen.NXObject[] { smartKey });
//            }

//            // Gets the string that the to be added component's name should be set too.
//            int newComponentName = compName;

//            //Constructs the path of the part that will be the defining prototype for the component that is to be added to {partToAddTo}.
//            string newPartPath = $"{Path.GetDirectoryName(__work_part_.FullPath)}\\{_folder.CustomerNumber}-020-{newComponentName}.prt";

//            // Finds the next detail number that doesn't exist. 
//            while (File.Exists(newPartPath)) newPartPath = $"{Path.GetDirectoryName(__work_part_.FullPath)}\\{_folder.CustomerNumber}-020-{newComponentName++}.prt";

//            smartGaugePart.SaveAs(newPartPath);

//            smartGaugePart.NamePartOccurrences(Globals.__display_part_, newComponentName + "");
//        }
//    }
//}


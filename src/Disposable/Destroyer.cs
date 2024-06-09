using System;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;

namespace TSG_Library.Disposable
{
    public class Destroyer : IDisposable
    {
        private readonly Builder _builder;

        private readonly bool _callStop;

        private readonly EditWithRollbackManager _editWithRollbackManager;

        private readonly ObjectSelector _objectSelector;

        private readonly SelectionIntentRule[] _rules;

        private readonly ScCollector _scCollector;

        private readonly Section _section;

        private readonly StepCreator _stepCreator;

        private readonly bool _updateFeatureBeforeStop;

        public Destroyer(EditWithRollbackManager editWithRollbackManager, bool callStop = true,
            bool updateFeatureBeforeStop = false)
        {
            _editWithRollbackManager = editWithRollbackManager;

            _updateFeatureBeforeStop = updateFeatureBeforeStop;

            _callStop = callStop;
        }

        public Destroyer(SelectionIntentRule[] rules)
        {
            _rules = rules;
        }

        public Destroyer(Section section)
        {
            _section = section;
        }

        public Destroyer(ObjectSelector objectSelector)
        {
            _objectSelector = objectSelector;
        }

        public Destroyer(ScCollector collector)
        {
            _scCollector = collector;
        }

        public Destroyer(StepCreator stepCreator)
        {
            _stepCreator = stepCreator;
        }

        public Destroyer(Part part, out CopyPasteBuilder builder, params Feature[] featuresToCopy)
        {
            if(featuresToCopy == null)
                throw new ArgumentNullException(nameof(featuresToCopy));

            if(featuresToCopy.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(featuresToCopy));

            if(part.Tag != Session.GetSession().Parts.Work.Tag)
                throw new ArgumentException($@"Part ""{part.Leaf}"" must be the current work part.", nameof(part));

            // ReSharper disable once CoVariantArrayConversion
            builder = part.Features.CreateCopyPasteBuilder2(featuresToCopy);

            builder.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);

            builder.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;

            _builder = builder;
        }

        // public Destroyer(ExtractFace feature, out ExtractFaceBuilder builder)
        // {
        //     builder = feature.__OwningPart().Features.CreateExtractFaceBuilder(feature);

        //     _builder = builder;
        // }

        // public Destroyer(Block feature, out BlockFeatureBuilder builder)
        // {
        //     builder = feature.__OwningPart().Features.CreateBlockFeatureBuilder(feature);

        //     _builder = builder;
        // }

        // public Destroyer(DeleteFace feature, out DeleteFaceBuilder builder)
        // {
        //     builder = feature.__OwningPart().Features.CreateDeleteFaceBuilder(feature);

        //     _builder = builder;
        // }

        // public Destroyer(Extrude feature, out ExtrudeBuilder builder)
        // {
        //     builder = feature.__OwningPart().Features.CreateExtrudeBuilder(feature);

        //     _builder = builder;
        // }

        // public Destroyer(EdgeBlend feature, out EdgeBlendBuilder builder)
        // {
        //     builder = feature.__OwningPart().Features.CreateEdgeBlendBuilder(feature);

        //     _builder = builder;
        // }

        // public Destroyer(Chamfer feature, out ChamferBuilder builder)
        // {
        //     builder = feature.__OwningPart().Features.CreateChamferBuilder(feature);

        //     _builder = builder;
        // }

        // public Destroyer(OffsetFace feature, out OffsetFaceBuilder builder)
        // {
        //     builder = feature.__OwningPart().Features.CreateOffsetFaceBuilder(feature);

        //     _builder = builder;
        // }


        public Destroyer(Part part, out CreateNewComponentBuilder builder)
        {
            builder = part.AssemblyManager.CreateNewComponentBuilder();

            _builder = builder;
        }

        public Destroyer(out CreateNewComponentBuilder builder) : this(Session.GetSession().Parts.Work, out builder)
        {
        }

        public Destroyer(Builder builder)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        public void Dispose()
        {
            if(!(_editWithRollbackManager is null))
            {
                _editWithRollbackManager.UpdateFeature(_updateFeatureBeforeStop);

                if(_callStop)
                    _editWithRollbackManager.Stop();

                _editWithRollbackManager.Destroy();
            }

            _stepCreator?.Destroy();

            _scCollector?.Destroy();

            _builder?.Destroy();

            _objectSelector?.Destroy();

            _section?.Destroy();

            if(_rules is null)
                return;

            foreach (var rule in _rules)
                if(!(rule is null))
                    using (rule)
                    {
                    }
        }
    }
}
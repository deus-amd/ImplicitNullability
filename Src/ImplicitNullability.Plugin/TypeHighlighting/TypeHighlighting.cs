using System;
using System.Linq;
using ImplicitNullability.Plugin.VsFormatDefinitions;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl.DocumentMarkup;
using JetBrains.Util;

[assembly: RegisterHighlighter(
    INTypeHighlightingAttributeIds.HIGHLIGHTING_ID,
    EffectColor = "Yellow", // TODO: what?
    EffectType = EffectType.DOTTED_UNDERLINE,
    Layer = HighlighterLayer.SYNTAX, VSPriority = VSPriority.IDENTIFIERS)]

//[assembly: RegisterConfigurableSeverity(
//  INTypeHighlighting.SEVERITY_ID, null,
//  HighlightingGroupIds.CodeInfo, "INTypeHighlighting",
//  "INTypeHighlighting",
//  Severity.HINT, false)]

namespace ImplicitNullability.Plugin.TypeHighlighting
{
    [StaticSeverityHighlighting(
        Severity.INFO,
        CSharpLanguage.Name, // TODO: GroupID!
        AttributeId = INTypeHighlightingAttributeIds.HIGHLIGHTING_ID,
        ShowToolTipInStatusBar = false, ToolTipFormatString = MESSAGE)]
    //[ConfigurableSeverityHighlighting(
    //  INTypeHighlighting.SEVERITY_ID,
    //  CSharpLanguage.Name, // TODO: GroupID!
    //  AttributeId = INTypeHighlightingAttributeIds.HIGHLIGHTING_ID,
    //  ShowToolTipInStatusBar = false, ToolTipFormatString = MESSAGE)]
    public class StaticINTypeHighlighting : IHighlighting
    {
        //public const string SEVERITY_ID = "INTypeHighlighting";
        private const string MESSAGE = "'{0}' is (explicitly or implicitly) [NotNull]";

        [NotNull]
        private readonly ITreeNode _element;


        public StaticINTypeHighlighting([NotNull] ITypeUsage typeUsage)
        {
            _element = typeUsage.NotNull("typeUsage"); // REVIEW: remove again? 
            ToolTip = string.Format(MESSAGE, typeUsage.GetText());
        }

        public bool IsValid()
        {
            return _element.IsValid();
        }

        public DocumentRange CalculateRange() => _element.GetDocumentRange();

        [NotNull]
        public string ToolTip { get; }

        [NotNull]
        public string ErrorStripeToolTip => ToolTip;
    }

    //[ShellComponent]
    //public class ConfigurableSeverityHacks
    //{
    //    [NotNull]
    //    private static readonly Severity[] Severities =
    //    {
    //        Severity.HINT
    //    };

    //    [NotNull]
    //    private static readonly string[] HighlightingIds = { INTypeHighlightingAttributeIds.HIGHLIGHTING_ID };

    //    public ConfigurableSeverityHacks()
    //    {
    //        var severityIds = HighlightingAttributeIds.ValidHighlightingsForSeverity;
    //        lock (severityIds)
    //        {
    //            foreach (var severity in Severities)
    //            {
    //                ICollection<string> collection;
    //                if (!severityIds.TryGetValue(severity, out collection)) continue;

    //                foreach (var highlightingId in HighlightingIds)
    //                {
    //                    if (!collection.Contains(highlightingId))
    //                    {
    //                        collection.Add(highlightingId);
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    [ElementProblemAnalyzer(
        new[]
        {
            typeof(IMethodDeclaration),
            typeof(IRegularParameterDeclaration) /*TODO: What about the other IParameterDeclaration-s?*/,
            typeof(IOperatorDeclaration),
            typeof(IDelegateDeclaration)
        },
        HighlightingTypes = new[]
        {
            typeof(StaticINTypeHighlighting)
        })]
    public sealed class INTypeHighlightingAnalyzer : ElementProblemAnalyzer<IDeclaration>
    {
        private readonly NullnessProvider _nullnessProvider;
        private readonly ContainerElementNullnessProvider _containerElementNullnessProvider;
        //private readonly ImplicitNullabilityProvider _implicitNullabilityProvider;

        public INTypeHighlightingAnalyzer(CodeAnnotationsCache codeAnnotationsCache/*, ImplicitNullabilityProvider implicitNullabilityProvider*/)
        {
            _nullnessProvider = codeAnnotationsCache.GetProvider<NullnessProvider>();
            _containerElementNullnessProvider = codeAnnotationsCache.GetProvider<ContainerElementNullnessProvider>();
            //_implicitNullabilityProvider = implicitNullabilityProvider;
        }

        protected override void Run(IDeclaration declaration, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (data.ProcessKind == DaemonProcessKind.VISIBLE_DOCUMENT)
                RunForVisibleDocument(declaration, consumer);
        }

        private void RunForVisibleDocument(IDeclaration declaration, IHighlightingConsumer consumer)
        {
            var parameterDeclaration = declaration as IRegularParameterDeclaration;
            if (parameterDeclaration?.DeclaredElement != null)
            {
                HandleElement(consumer, parameterDeclaration.DeclaredElement, parameterDeclaration.TypeUsage /*TODO: Can get null during typing!*/);
            }

            var methodDeclaration = declaration as IMethodDeclaration;
            if (methodDeclaration?.DeclaredElement != null)
            {
                var method = methodDeclaration.DeclaredElement;

                if (method.ReturnType.IsTask() || method.ReturnType.IsGenericTask())
                {
                    if (method.ReturnType.IsGenericTask())
                    {
                        var userTypeUsage = (IUserTypeUsage)methodDeclaration.TypeUsage;
                        var innerTypeUsage = userTypeUsage.ScalarTypeName.TypeArgumentList.TypeArgumentNodes.FirstOrDefault();

                        HandleContainerElement(consumer, method, innerTypeUsage);
                    }
                }
                else
                {
                    HandleElement(consumer, method, methodDeclaration.TypeUsage /*TODO: Can get null during typing!*/);
                }
            }

            var operatorDeclaration = declaration as IOperatorDeclaration;
            if (operatorDeclaration != null)
            {
                HandleElement(consumer, operatorDeclaration.DeclaredElement, operatorDeclaration.TypeUsage /*TODO: Can get null during typing!*/);
            }

            var delegateDeclaration = declaration as IDelegateDeclaration;
            if (delegateDeclaration != null)
            {
                HandleElement(consumer, delegateDeclaration.DeclaredElement, delegateDeclaration.TypeUsage /*TODO: Can get null during typing!*/);
            }

            // TODO: Handling of iterator methods => do not show the whole IEnumerable<T> as non-nullable although is would be right?
            // TODO: Handling of async methods => same as above and even more important because otherwise the inner type wouldn't be visible. What about two parts?
        }

        private void HandleElement(IHighlightingConsumer consumer, IAttributesOwner element, ITypeUsage typeUsageOfElement)
        {
            if (IsNotNull(element))
            //if (IsNotNullWithoutExplicitAttributes(element))
            {
                consumer.AddHighlighting(new StaticINTypeHighlighting(typeUsageOfElement));
            }
        }

        private bool IsNotNull(IAttributesOwner attributesOwner)
        {
            return _nullnessProvider.GetInfo(attributesOwner) == CodeAnnotationNullableValue.NOT_NULL;
        }

        //// TODO: show again only for implicitly [NotNull]?

        private bool IsNotNullWithoutExplicitAttributes(IAttributesOwner attributesOwner)
        {
            return !attributesOwner.GetAttributeInstances(false).Any() &&
                   _nullnessProvider.GetInfo(attributesOwner) == CodeAnnotationNullableValue.NOT_NULL;
        }

        //// TODO: duplicated!
        //private bool IsImplicitlyNotNull(IAttributesOwner declaredElement, IEnumerable<IAttributeInstance> attributeInstances)
        //{
        //    return !_nullabilityProvider.ContainsAnyExplicitNullabilityAttributes(attributeInstances) &&
        //    _implicitNullabilityProvider.AnalyzeDeclaredElement(declaredElement) == CodeAnnotationNullableValue.NOT_NULL;
        //}

        private void HandleContainerElement(IHighlightingConsumer consumer, IAttributesOwner element, ITypeUsage typeUsageOfElement)
        {
            if (IsItemNotNull(element))
            {
                consumer.AddHighlighting(new StaticINTypeHighlighting(typeUsageOfElement));
            }
        }

        private bool IsItemNotNull(IAttributesOwner attributesOwner)
        {
            return _containerElementNullnessProvider.GetInfo(attributesOwner) == CodeAnnotationNullableValue.NOT_NULL;
        }
    }

    //[DaemonStage(LastStage = true)]
    //public class INTypeHighlightingDaemonStage : CSharpDaemonStageBase
    //{
    //    private readonly NullabilityProvider _nullabilityProvider;
    //    private readonly ImplicitNullabilityProvider _implicitNullabilityProvider;

    //    public INTypeHighlightingDaemonStage(NullabilityProvider nullabilityProvider, ImplicitNullabilityProvider implicitNullabilityProvider)
    //    {
    //        _nullabilityProvider = nullabilityProvider;
    //        _implicitNullabilityProvider = implicitNullabilityProvider;
    //    }

    //    // Methods
    //    protected override IDaemonStageProcess CreateProcess(IDaemonProcess process, IContextBoundSettingsStore settings, DaemonProcessKind processKind, ICSharpFile file)
    //    {
    //        return new INTypeHighlightingDaemonStageProcess(process, file, settings, processKind, _nullabilityProvider, _implicitNullabilityProvider);
    //    }


    //    private class INTypeHighlightingDaemonStageProcess : CSharpDaemonStageProcessBase
    //    {
    //        private readonly IContextBoundSettingsStore _settingsStore;
    //        private readonly DaemonProcessKind _processKind;
    //        private readonly NullabilityProvider _nullabilityProvider;
    //        private readonly ImplicitNullabilityProvider _implicitNullabilityProvider;

    //        public INTypeHighlightingDaemonStageProcess(
    //            [NotNull] IDaemonProcess process,
    //            [NotNull] ICSharpFile file,
    //            [NotNull] IContextBoundSettingsStore settingsStore,
    //            DaemonProcessKind processKind,
    //            NullabilityProvider nullabilityProvider, ImplicitNullabilityProvider implicitNullabilityProvider) : base(process, file)
    //        {
    //            _settingsStore = settingsStore;
    //            _processKind = processKind;
    //            _nullabilityProvider = nullabilityProvider;
    //            _implicitNullabilityProvider = implicitNullabilityProvider;
    //        }

    //        public override void Execute(Action<DaemonStageResult> committer)
    //        {
    //            HighlightInFile(delegate (ICSharpFile file, IHighlightingConsumer consumer)
    //            {
    //                file.ProcessDescendants(this, consumer);
    //            }, committer, _settingsStore);
    //        }

    //        // TODO: What about the other IParameterDeclaration-s?
    //        public override void VisitRegularParameterDeclaration(IRegularParameterDeclaration parameterDeclaration, IHighlightingConsumer consumer)
    //        {
    //            if (parameterDeclaration?.DeclaredElement != null)
    //            {
    //                var parameter = parameterDeclaration.DeclaredElement;
    //                if (IsImplicitlyNotNull(parameter, parameter.GetAttributeInstances(inherit: false)))
    //                {
    //                    var typeUsage = parameterDeclaration.TypeUsage;

    //                    consumer.AddHighlighting(new INTypeHighlighting(typeUsage), typeUsage.GetNavigationRange());
    //                }
    //            }

    //            base.VisitRegularParameterDeclaration(parameterDeclaration, consumer);
    //        }

    //        public override void VisitMethodDeclaration(IMethodDeclaration methodDeclaration, IHighlightingConsumer consumer)
    //        {
    //            var method = methodDeclaration.DeclaredElement;

    //            if (IsImplicitlyNotNull(method, method.GetAttributeInstances(inherit: false)))
    //            {
    //                var typeUsage = methodDeclaration.TypeUsage;

    //                consumer.AddHighlighting(new INTypeHighlighting(typeUsage), typeUsage.GetNavigationRange());
    //            }

    //            base.VisitMethodDeclaration(methodDeclaration, consumer);
    //        }

    //        // TODO: duplicated!
    //        private bool IsImplicitlyNotNull(IDeclaredElement declaredElement, IEnumerable<IAttributeInstance> attributeInstances)
    //        {
    //            return !_nullabilityProvider.ContainsAnyExplicitNullabilityAttributes(attributeInstances) &&
    //                   _implicitNullabilityProvider.AnalyzeDeclaredElement(declaredElement) == CodeAnnotationNullableValue.NOT_NULL;
    //        }
    //    }
    //}
}
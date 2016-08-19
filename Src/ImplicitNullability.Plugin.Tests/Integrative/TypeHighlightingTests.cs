using ImplicitNullability.Plugin.Settings;
using ImplicitNullability.Plugin.Tests.Infrastructure;
using ImplicitNullability.Plugin.TypeHighlighting;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
#if RESHARPER20161
using CSharpHighlightingTestBase = JetBrains.ReSharper.FeaturesTestFramework.Daemon.CSharpHighlightingTestNet45Base;

#else
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;

#endif

namespace ImplicitNullability.Plugin.Tests.Integrative
{
    public class TypeHighlightingTests : CSharpHighlightingTestBase
    {
        // TODO: Try to test this static highlighting also with SampleSolutionTestBase

        protected override string RelativeTestDataPath =>
            TestDataPathUtility.GetPathRelativeToSolution(@"Src\ImplicitNullability.Samples.CodeWithIN\TypeHighlighting");


        [Test]
        public void TestTypeHighlightingSample()
        {
            DoNamedTest2();
        }

        protected override void DoTestSolution(params string[] fileSet)
        {
            ExecuteWithinSettingsTransaction(store =>
            {
                RunGuarded(() => store.SetValue((ImplicitNullabilitySettings s) => s.Enabled, true));

                base.DoTestSolution(fileSet);
            });
        }

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile)
        {
            if (highlighting is StaticINTypeHighlighting)
                return true;

            return base.HighlightingPredicate(highlighting, sourceFile);
        }
    }
}
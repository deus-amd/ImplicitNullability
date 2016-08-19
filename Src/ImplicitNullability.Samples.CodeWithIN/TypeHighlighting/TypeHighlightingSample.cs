using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.TypeHighlighting
{
    public abstract class TypeHighlightingSampleBase
    {
        [NotNull]
        public abstract string VirtualMethodExplicit([NotNull] string a, [CanBeNull] string b);
    }

    public class TypeHighlightingSample : TypeHighlightingSampleBase
    {
        [NotNull]
        public string MethodExplicit([NotNull] string a)
        {
            return a;
        }

        public string MethodImplicit(string a)
        {
            Console.WriteLine(a == null);
            return null;
        }

        public void NonReferenceTypes(int a, DateTime b)
        {
        }

        [CanBeNull]
        public string Nullable([CanBeNull] string a)
        {
            return a;
        }

        // ReSharper disable once ImplicitNotNullConflictInHierarchy
        public override string VirtualMethodExplicit(string a, string b)
        {
            return a;
        }

        public static TypeHighlightingSample operator ++(TypeHighlightingSample value)
        {
            return new TypeHighlightingSample();
        }

        [CanBeNull]
        public static TypeHighlightingSample operator --([CanBeNull] TypeHighlightingSample value)
        {
            return null;
        }

        public delegate string SomeDelegate(string a);

        [CanBeNull]
        public delegate string SomeNullableDelegate([CanBeNull] string a);

        // TODO: Iterator is implicitly NotNull. That's true, still make an exemption (to be symmetric with async methods?)
        public IEnumerable<string> Iterator()
        {
            yield return "";
        }

        // TODO: Highlight inner type based on ItemNotNull
        public async Task<string> AsyncMethod()
        {
            await Task.Delay(0);
            return "";
        }

        public async Task VoidAsyncMethod()
        {
            await Task.Delay(0);
        }

        [ItemCanBeNull]
        public async Task<string> NullableAsyncMethod()
        {
            await Task.Delay(0);
            return null;
        }

        // TODO: What about non-async methods?
        public Task<string> NonAsyncButTaskResult()
        {
            return Task.FromResult("");
        }

        [ItemCanBeNull]
        public Task<string> NonAsyncButNullableTaskResult()
        {
            return Task.FromResult<string>(null);
        }
    }
}
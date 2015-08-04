using System;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.Modules;

namespace ImplicitNullability.Plugin.Infrastructure
{
    public static class PsiExtensions
    {
        /// <summary>
        /// Check if this element is part of the solution code (see http://devnet.jetbrains.com/thread/454638)
        /// </summary>
        public static bool IsPartOfSolutionCode([NotNull] this IClrDeclaredElement declaredElement)
        {
            return declaredElement.Module is IProjectPsiModule;
        }

        public static bool IsUnknown(this CodeAnnotationNullableValue? x)
        {
            return x == null;
        }

        public static bool IsInput([NotNull] this IParameter parameter)
        {
            return parameter.Kind == ParameterKind.VALUE;
        }

        public static bool IsRef([NotNull] this IParameter parameter)
        {
            return parameter.Kind == ParameterKind.REFERENCE;
        }

        public static bool IsInputOrRef([NotNull] this IParameter parameter)
        {
            return parameter.IsInput() || parameter.IsRef();
        }

        public static bool IsOut([NotNull] this IParameter parameter)
        {
            return parameter.Kind == ParameterKind.OUTPUT;
        }
    }
}
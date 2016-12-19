﻿using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.NotNullOnImplicitCanBeNull
{
    public class NotNullOnImplicitCanBeNullSample
    {
        [NotNull]
        public int? Field /*TODO Expect_NotNullOnImplicitCanBeNull[Implicit]*/ = 42;

        public void MethodWithNullableInt([NotNull] int? nullableInt /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/)
        {
            // R# ignores the [NotNull] here, but respects it at the call site.

            ReSharper.TestValueAnalysis(nullableInt /*Expect:AssignNullToNotNullAttribute*/, nullableInt == null);
        }

        public void MethodWithOptionalParameter(
            // ReSharper disable AssignNullToNotNullAttribute - because in R# 9+ this hides NotNullOnImplicitCanBeNull
            [NotNull] string optional = null /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/)
            // ReSharper restore AssignNullToNotNullAttribute
        {
            // R# ignores the [NotNull] here, but respects it at the call site.

            ReSharper.TestValueAnalysis(optional /*Expect:AssignNullToNotNullAttribute*/, optional == null);
        }

        public string this[
            [NotNull] int? nullableInt /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/,
            // ReSharper disable AssignNullToNotNullAttribute - because in R# 9+ this hides NotNullOnImplicitCanBeNull
            [NotNull] string optional = null /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/] => null;

        // ReSharper restore AssignNullToNotNullAttribute

        public void MethodWithNullableIntRefAndOutParameterMethod(
            [NotNull] ref int? refParam /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/,
            [NotNull] out int? outParam /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/)
        {
            // REPORT? Warning, although explicitly NotNull
            ReSharper.TestValueAnalysis(refParam /*Expect:AssignNullToNotNullAttribute*/, refParam == null);

            outParam = null /*Expect:AssignNullToNotNullAttribute*/; // This warning results from the explicit NotNull
        }

        [NotNull]
        public int? FunctionWithNullableInt /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/()
        {
            return null /*Expect:AssignNullToNotNullAttribute*/; // This warning results from the explicit NotNull
        }

        [ItemNotNull]
        public async Task<int?> AsyncFunctionWithNullableInt /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/()
        {
            await Task.Delay(0);
            return null /*Expect:AssignNullToNotNullAttribute*/; // This warning results from the explicit NotNull
        }

        [NotNull]
        public delegate int? Delegate /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/(
            [NotNull] int? a /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/,
            [NotNull] ref int? refParam /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/,
            [NotNull] out int? outParam /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/);

        public class Operator
        {
            public static explicit operator Operator([NotNull] int? value /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/)
            {
                return new Operator();
            }

            [NotNull]
            public static explicit operator /*Expect:NotNullOnImplicitCanBeNull[Implicit]*/ int?(Operator value)
            {
                return null /*Expect:AssignNullToNotNullAttribute*/; // This warning results from the explicit NotNull
            }
        }
    }
}
using ImplicitNullability.Plugin.Settings;
using JetBrains.Util;

namespace ImplicitNullability.Plugin
{
    /// <summary>
    /// Represents the implicit nullability configuration (the chosen options in the settings / in the assembly attributes).
    /// </summary>
    public struct ImplicitNullabilityConfiguration
    {
        public static readonly ImplicitNullabilityConfiguration AllDisabled = new ImplicitNullabilityConfiguration(false, false, false, false);

        public static ImplicitNullabilityConfiguration CreateFromSettings(ImplicitNullabilitySettings implicitNullabilitySettings)
        {
            Assertion.Assert(implicitNullabilitySettings.Enabled, "implicitNullabilitySettings.Enabled");

            return new ImplicitNullabilityConfiguration(
                implicitNullabilitySettings.EnableInputParameters,
                implicitNullabilitySettings.EnableRefParameters,
                implicitNullabilitySettings.EnableOutParametersAndResult,
                implicitNullabilitySettings.EnableFields);
        }

        public ImplicitNullabilityConfiguration(
            bool enableInputParameters,
            bool enableRefParameters,
            bool enableOutParametersAndResult,
            bool enableFields)
        {
            EnableInputParameters = enableInputParameters;
            EnableRefParameters = enableRefParameters;
            EnableOutParametersAndResult = enableOutParametersAndResult;
            EnableFields = enableFields;
        }

        public bool EnableInputParameters { get; }

        public bool EnableRefParameters { get; }

        public bool EnableOutParametersAndResult { get; }

        public bool EnableFields { get; }
    }
}
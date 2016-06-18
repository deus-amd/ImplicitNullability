using JetBrains.Application.Settings;
using JetBrains.ReSharper.Resources.Settings;

namespace ImplicitNullability.Plugin.Settings
{
    [SettingsKey(typeof(CodeInspectionSettings), "Implicit Nullability")]
    public class ImplicitNullabilitySettings
    {
        [SettingsEntry(false, "Enabled")]
        public readonly bool Enabled;

        [SettingsEntry(true, "EnableInputParameters")]
        public readonly bool EnableInputParameters;

        [SettingsEntry(true, "EnableRefParameters")]
        public readonly bool EnableRefParameters;

        [SettingsEntry(true, "EnableOutParametersAndResult")]
        public readonly bool EnableOutParametersAndResult;

        [SettingsEntry(true, "EnableFields")]
        public readonly bool EnableFields;

        /*
         
        TODO for fields:

        # Highlightings !
        # Public vs. Non Public ? readonly?
        # More Test cases?


        Settings variants:
        
         # [AssemblyMetadataAttribute("ImplicitNullability.AppliesTo", "..., ReadonlyFields, NonReadonlyFields")]
         
         # [AssemblyMetadataAttribute("ImplicitNullability.AppliesTo", "..., Fields")] vs. [AssemblyMetadataAttribute("..., ReadonlyFields")]
         
         # [AssemblyMetadataAttribute("ImplicitNullability.AppliesTo", "..., Fields[ReadOnly|NonReadonly]")]
         
         # [AssemblyMetadataAttribute("ImplicitNullability.AppliesTo", "..., Fields[OnlyReadOnly]")]
         
         # [AssemblyMetadataAttribute("ImplicitNullability.AppliesTo", "..., Fields")]
           &
           [AssemblyMetadataAttribute("ImplicitNullability.Fields", "OnlyReadonly")]




        */
    }
}
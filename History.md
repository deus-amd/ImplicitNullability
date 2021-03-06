### vNext ###
- Ignore methods with [ContractAnnotation] attribute

### 3.2.0 ###
- ReSharper 2016.3 support

### 3.1.0 ###
- ReSharper 2016.2 support

### 3.0.0 ###
- Implicit Nullability support for compiled assemblies with [AssemblyMetadata("ImplicitNullability.AppliesTo", "...")] configuration attribute
- New "Implicit NotNull element cannot override CanBeNull in base type, nullability should be explicit" warning for return values and out parameters
- New "Implicit NotNull result or out parameter overrides unknown nullability of external code" hint
- Added suppression of "Base declaration has the same annotation" highlighting on code elements with enabled implicit nullability
- Added exclusion of delegate BeginInvoke() method parameters and Invoke() / EndInvoke() results because their implicit nullability cannot be overridden with explicit annotations
- ReSharper 2016.1 support

### 2.2.0 ###
- ReSharper 10.0 support
- Added configuration option in ReSharper's "Products & Features" settings
- Dropped ReSharper 8.2 support

### 2.1.0 ###
- Support for Task<T> method return types (big thanks to Ivan Serdiuk) [ReSharper 9.2+]
- Fixed extension meta data to enable running Implicit Nullability in InspectCode (ReSharper Command Line Tools)

### 2.0.0 ###
- Added support for method / delegate results and out parameters (can be enabled / disabled via a new option)
- Split option for method / delegate / indexer input and ref parameters into two separate options
- ReSharper 9.2 support

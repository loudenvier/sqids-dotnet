#if !NET7_0_OR_GREATER
namespace Sqids;

[AttributeUsage(AttributeTargets.Class)]
internal class GenerateSqidsLegacyOverloadsAttribute : Attribute { }

#endif

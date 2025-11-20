using System;

namespace HamrahFelez.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class UseProductionDbAttribute : Attribute
    {
    }
}

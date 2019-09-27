using System;

namespace WebHost.ClientApi.CodeGeneration
{
    [
        AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Method) 
    ]
    public sealed class ClientRefIgnoreAttribute : Attribute
    {
    }
}
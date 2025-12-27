using System;

namespace Peach.Pro.Core.Mutators.LLM
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CMutatorAttribute : Attribute
    {
        public string FunctionName { get; private set; }

        public CMutatorAttribute(string functionName)
        {
            FunctionName = functionName;
        }
    }
}


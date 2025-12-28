using System.Collections.Generic;
using Peach.Core;
using Peach.Core.Dom;

namespace Peach.Pro.Core.Fixups.LLM
{
    public abstract class LLMFixup : Fixup
    {
        public virtual bool ShouldFixup { get; set; } = true;

        public LLMFixup(DataElement parent, Dictionary<string, Variant> args, params string[] refs) : base(parent, args, refs)
        {
        }
    }
}
using System;
using System.IO;
using System.Linq;
using Peach.Core.Dom;
using Peach.Core;
using Peach.Pro.Core.Mutators;
using Peach.Pro.Core.Mutators.LLM.DTLS;
using System.ComponentModel;

namespace Peach.Pro.Core.Mutators.LLM.DTLS
{
    [Mutator("DtlsMutateServerKeyExchange")]
    [CMutator("mutate_server_key_exchange")]
    [Description("Mutates Server Key Exchange body.")]
    public class DtlsMutateServerKeyExchange : LLMMutator
    {
        public DtlsMutateServerKeyExchange(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Block && obj.Name == "body" && obj.IsIn("server_key_exchange");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            // Traverse down to find blobs like 'dh_p', 'ec_point' etc. and mutate them.
            // C code logic dispatches based on structure. Here we rely on recursive
            // finding of blobs or mutating the selected choice.
            var blk = obj as Block;
            var u = blk.find("u") as Choice;
            if(u != null && u.SelectedElement != null)
            {
                // Mutate all blobs found in the selected element
                foreach(var blob in u.SelectedElement.EnumerateAllElements().OfType<Blob>())
                {
                    byte[] val = blob.Bytes();
                    if(val.Length > 0) NextBytes(val);
                    blob.MutatedValue = new Variant(val);
                }
            }
        }
    }

    [Mutator("DtlsMutateClientKeyExchange")]
    [CMutator("mutate_client_key_exchange")]
    [Description("Mutates Client Key Exchange body.")]
    public class DtlsMutateClientKeyExchange : LLMMutator
    {
        public DtlsMutateClientKeyExchange(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Block && obj.Name == "body" && obj.IsIn("client_key_exchange");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            // Similar strategy as SKE
            var blk = obj as Block;
            var u = blk.find("u") as Choice;
            if(u != null && u.SelectedElement != null)
            {
                foreach(var blob in u.SelectedElement.EnumerateAllElements().OfType<Blob>())
                {
                    byte[] val = blob.Bytes();
                    if(val.Length > 0) NextBytes(val);
                    blob.MutatedValue = new Variant(val);
                }
            }
        }
    }
}
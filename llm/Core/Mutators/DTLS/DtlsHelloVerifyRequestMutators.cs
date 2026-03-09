using System;
using System.IO;
using System.Linq;
using Peach.Core.Dom;
using Peach.Core;
using Peach.Pro.Core.Mutators;
using Peach.LLM.Core.Mutators.DTLS;
using System.ComponentModel;

namespace Peach.LLM.Core.Mutators.DTLS
{
    [Mutator("DtlsMutateHelloVerifyRequestServerVersion")]
    [CMutator("mutate_hello_verify_request_server_version")]
    [Description("Mutates HVR Server Version.")]
    public class DtlsMutateHelloVerifyRequestServerVersion : LLMMutator
    {
        public DtlsMutateHelloVerifyRequestServerVersion(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Block && obj.Name == "server_version" && obj.IsIn("hello_verify_request");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blk = obj as Block;
            var maj = blk.find("major") as Number;
            var min = blk.find("minor") as Number;

            maj.MutatedValue = new Variant((ulong)(0xFE));
            min.MutatedValue = new Variant((ulong)((DtlsUtils.RndU32(2) == 0) ? 0xFD : 0xFF));
        }
    }

    [Mutator("DtlsMutateHelloVerifyRequestCookie")]
    [CMutator("mutate_hello_verify_request_cookie")]
    [Description("Mutates HVR Cookie.")]
    public class DtlsMutateHelloVerifyRequestCookie : LLMMutator
    {
        public DtlsMutateHelloVerifyRequestCookie(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "cookie" && obj.IsIn("hello_verify_request");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            int len = (int)(DtlsUtils.RndU32(32) + 1);
            byte[] val = new byte[len];
            NextBytes(val);
            blob.MutatedValue = new Variant(val);
        }
    }
}
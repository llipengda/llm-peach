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
    [Mutator("DtlsMutateServerHelloServerVersion")]
    [CMutator("mutate_server_hello_server_version")]
    [Description("Mutates ServerHello server_version.")]
    public class DtlsMutateServerHelloServerVersion : LLMMutator
    {
        public DtlsMutateServerHelloServerVersion(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Block && obj.Name == "server_version" && obj.IsIn("server_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blk = obj as Block;
            var maj = blk.find("major") as Number;
            var min = blk.find("minor") as Number;

            // Logic similar to ClientHello version
            maj.MutatedValue = new Variant((ulong)(0xFE));
            min.MutatedValue = new Variant((ulong)((DtlsUtils.RndU32(2) == 0) ? 0xFD : 0xFF));
        }
    }

    [Mutator("DtlsMutateServerHelloRandom")]
    [CMutator("mutate_server_hello_random")]
    [Description("Mutates ServerHello Random.")]
    public class DtlsMutateServerHelloRandom : LLMMutator
    {
        public DtlsMutateServerHelloRandom(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "random" && obj.IsIn("server_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] val = new byte[32];
            NextBytes(val);
            blob.MutatedValue = new Variant(val);
        }
    }

    [Mutator("DtlsAddServerHelloSessionId")]
    [CMutator("add_server_hello_session_id")]
    [Description("Adds Session ID to ServerHello.")]
    public class DtlsAddServerHelloSessionId : LLMMutator
    {
        public DtlsAddServerHelloSessionId(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "id" && obj.parent.Name == "session_id" && obj.IsIn("server_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            if (blob.Value.Length == 0)
            {
                byte[] val = new byte[32];
                NextBytes(val);
                blob.MutatedValue = new Variant(val);
            }
        }
    }

    [Mutator("DtlsDeleteServerHelloSessionId")]
    [CMutator("delete_server_hello_session_id")]
    [Description("Deletes Session ID from ServerHello.")]
    public class DtlsDeleteServerHelloSessionId : LLMMutator
    {
        public DtlsDeleteServerHelloSessionId(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "id" && obj.parent.Name == "session_id" && obj.IsIn("server_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            blob.MutatedValue = new Variant(new byte[0]);
        }
    }

    [Mutator("DtlsMutateServerHelloSessionId")]
    [CMutator("mutate_server_hello_session_id")]
    [Description("Mutates ServerHello Session ID.")]
    public class DtlsMutateServerHelloSessionId : LLMMutator
    {
        public DtlsMutateServerHelloSessionId(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "id" && obj.parent.Name == "session_id" && obj.IsIn("server_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] val = blob.Bytes();
            if (val.Length > 0) NextBytes(val);
            blob.MutatedValue = new Variant(val);
        }
    }

    [Mutator("DtlsMutateServerHelloCipherSuite")]
    [CMutator("mutate_server_hello_cipher_suite")]
    [Description("Mutates ServerHello Cipher Suite.")]
    public class DtlsMutateServerHelloCipherSuite : LLMMutator
    {
        public DtlsMutateServerHelloCipherSuite(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "cipher_suite" && obj.IsIn("server_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var num = obj as Number;
            ushort val = (ushort)DtlsUtils.RndU32(0xFFFF);
            num.MutatedValue = new Variant((ulong)val);
        }
    }

    [Mutator("DtlsMutateServerHelloCompressionMethod")]
    [CMutator("mutate_server_hello_compression_method")]
    [Description("Mutates ServerHello Compression Method.")]
    public class DtlsMutateServerHelloCompressionMethod : LLMMutator
    {
        public DtlsMutateServerHelloCompressionMethod(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Number && obj.Name == "compression_method" && obj.IsIn("server_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var num = obj as Number;
            byte val = (byte)((DtlsUtils.RndU32(2) == 0) ? 0 : 1);
            num.MutatedValue = new Variant((ulong)val);
        }
    }

    [Mutator("DtlsAddServerHelloExtensions")]
    [CMutator("add_server_hello_extensions")]
    [Description("Adds Extensions to ServerHello.")]
    public class DtlsAddServerHelloExtensions : LLMMutator
    {
        public DtlsAddServerHelloExtensions(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "raw" && obj.parent.Name == "extensions" && obj.IsIn("server_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            if (blob.Value.Length == 0)
            {
                // Empty EMS extension: 0017 0000
                blob.MutatedValue = new Variant(new byte[] { 0x00, 0x17, 0x00, 0x00 });
            }
        }
    }

    [Mutator("DtlsDeleteServerHelloExtensions")]
    [CMutator("delete_server_hello_extensions")]
    [Description("Deletes Extensions from ServerHello.")]
    public class DtlsDeleteServerHelloExtensions : LLMMutator
    {
        public DtlsDeleteServerHelloExtensions(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "raw" && obj.parent.Name == "extensions" && obj.IsIn("server_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            blob.MutatedValue = new Variant(new byte[0]);
        }
    }

    [Mutator("DtlsRepeatServerHelloExtensions")]
    [CMutator("repeat_server_hello_extensions")]
    [Description("Repeats Extensions in ServerHello.")]
    public class DtlsRepeatServerHelloExtensions : LLMMutator
    {
        public DtlsRepeatServerHelloExtensions(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "raw" && obj.parent.Name == "extensions" && obj.IsIn("server_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] cur = blob.Bytes();
            if (cur.Length > 0 && cur.Length * 2 <= 65535)
            {
                byte[] newVal = new byte[cur.Length * 2];
                cur.CopyTo(newVal, 0);
                cur.CopyTo(newVal, cur.Length);
                blob.MutatedValue = new Variant(newVal);
            }
        }
    }

    [Mutator("DtlsMutateServerHelloExtensions")]
    [CMutator("mutate_server_hello_extensions")]
    [Description("Mutates ServerHello Extensions raw blob.")]
    public class DtlsMutateServerHelloExtensions : LLMMutator
    {
        public DtlsMutateServerHelloExtensions(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "raw" && obj.parent.Name == "extensions" && obj.IsIn("server_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        protected override void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] val = blob.Bytes();
            if (val.Length > 0) NextBytes(val);
            blob.MutatedValue = new Variant(val);
        }
    }
}
using System;
using System.IO;
using System.Linq;
using Peach.Core.Dom;
using Peach.Core;
using Peach.Pro.Core.Mutators;
using Peach.Pro.Core.Mutators.LLM.DTLS;
using System.ComponentModel;
using Array = System.Array;

namespace Peach.Pro.Core.Mutators.LLM.DTLS
{
    [Mutator("DtlsMutateClientHelloClientVersion")]
    [CMutator("mutate_client_hello_client_version")]
    [Description("Mutates the ClientHello client_version.")]
    public class DtlsMutateClientHelloClientVersion : LLMMutator
    {
        public DtlsMutateClientHelloClientVersion(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Block && obj.Name == "client_version" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blk = obj as Block;
            var majElem = blk.find("major") as Number;
            var minElem = blk.find("minor") as Number;

            byte maj = (byte)(ulong)majElem.InternalValue;
            byte min = (byte)(ulong)minElem.InternalValue;

            uint cat = DtlsUtils.RndU32(100);
            if (cat < 18) { maj = 0xFE; min = 0xFD; } // DTLS 1.2
            else if (cat < 32)
            {
                uint r = DtlsUtils.RndU32(6);
                if (r == 0) { maj = 0; min = 0; }
                else if (r == 1) { maj = 0xFF; min = 0xFF; }
                else if (r == 2) { maj = 0xFE; min = 0; }
                else if (r == 3) { maj = 0xFE; min = 0xFF; }
                else { maj = 0x03; min = 0x03; }
            }
            else if (cat < 68)
            {
                maj = 0xFE;
                uint r = DtlsUtils.RndU32(8);
                if (r == 0)
                    min = 0xFD;
                else if (r == 1)
                    min = 0xFF;
                else
                    min = (byte)r;
            }
            else
            {
                maj = (byte)DtlsUtils.RndU32(256);
                min = (byte)DtlsUtils.RndU32(256);
            }

            majElem.MutatedValue = new Variant((ulong)maj);
            minElem.MutatedValue = new Variant((ulong)min);
        }
    }

    [Mutator("DtlsMutateClientHelloRandom")]
    [CMutator("mutate_client_hello_random")]
    [Description("Mutates the ClientHello Random field.")]
    public class DtlsMutateClientHelloRandom : LLMMutator
    {
        public DtlsMutateClientHelloRandom(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "random" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] val = blob.Bytes();
            if (val.Length != 32) val = new byte[32]; // Ensure 32 bytes

            uint cat = DtlsUtils.RndU32(100);

            if (cat < 10)
            {
                // Canonical: Time + Random
                uint t = DtlsUtils.RndU32(0xFFFFFFFF);
                val[0] = (byte)(t >> 24); val[1] = (byte)(t >> 16);
                val[2] = (byte)(t >> 8); val[3] = (byte)t;
                for (int i = 4; i < 32; i++) val[i] = (byte)DtlsUtils.RndU32(256);
            }
            else if (cat < 22)
            {
                // Boundaries
                uint b = DtlsUtils.RndU32(4);
                byte fill = (byte)((b == 0) ? 0 : (b == 1) ? 0xFF : 0xAA);
                for (int i = 0; i < 32; i++) val[i] = fill;
            }
            else
            {
                // Random
                for (int i = 0; i < 32; i++) val[i] = (byte)DtlsUtils.RndU32(256);
            }

            blob.MutatedValue = new Variant(val);
        }
    }

    [Mutator("DtlsAddClientHelloSessionId")]
    [CMutator("add_client_hello_session_id")]
    [Description("Adds a Session ID to ClientHello if empty.")]
    public class DtlsAddClientHelloSessionId : LLMMutator
    {
        public DtlsAddClientHelloSessionId(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "id" && obj.parent.Name == "session_id" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            if (blob.Value.Length == 0)
            {
                // Add canonical 32 bytes
                byte[] val = new byte[32];
                NextBytes(val);
                blob.MutatedValue = new Variant(val);
            }
        }
    }

    [Mutator("DtlsDeleteClientHelloSessionId")]
    [CMutator("delete_client_hello_session_id")]
    [Description("Deletes the Session ID from ClientHello.")]
    public class DtlsDeleteClientHelloSessionId : LLMMutator
    {
        public DtlsDeleteClientHelloSessionId(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "id" && obj.parent.Name == "session_id" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            blob.MutatedValue = new Variant(new byte[0]);
        }
    }

    [Mutator("DtlsMutateClientHelloSessionId")]
    [CMutator("mutate_client_hello_session_id")]
    [Description("Mutates the Session ID in ClientHello.")]
    public class DtlsMutateClientHelloSessionId : LLMMutator
    {
        public DtlsMutateClientHelloSessionId(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "id" && obj.parent.Name == "session_id" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] cur = blob.Bytes();

            uint cat = DtlsUtils.RndU32(100);
            int len = cur.Length;

            if (cat < 12)
            {
                // A: empty or full
                if (DtlsUtils.RndU32(2) == 0) len = 0;
                else len = 32;
            }
            else if (cat < 56)
            {
                len = (int)DtlsUtils.RndU32(33); // 0..32
            }

            byte[] newVal = new byte[len];
            NextBytes(newVal);
            blob.MutatedValue = new Variant(newVal);
        }
    }

    [Mutator("DtlsAddClientHelloCookie")]
    [CMutator("add_client_hello_cookie")]
    [Description("Adds a Cookie to ClientHello if empty.")]
    public class DtlsAddClientHelloCookie : LLMMutator
    {
        public DtlsAddClientHelloCookie(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "cookie" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            if (blob.Value.Length == 0)
            {
                int len = (int)(1 + DtlsUtils.RndU32(255));
                byte[] val = new byte[len];
                NextBytes(val);
                blob.MutatedValue = new Variant(val);
            }
        }
    }

    [Mutator("DtlsDeleteClientHelloCookie")]
    [CMutator("delete_client_hello_cookie")]
    [Description("Deletes the Cookie from ClientHello.")]
    public class DtlsDeleteClientHelloCookie : LLMMutator
    {
        public DtlsDeleteClientHelloCookie(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "cookie" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            blob.MutatedValue = new Variant(new byte[0]);
        }
    }

    [Mutator("DtlsMutateClientHelloCookie")]
    [CMutator("mutate_client_hello_cookie")]
    [Description("Mutates the Cookie in ClientHello.")]
    public class DtlsMutateClientHelloCookie : LLMMutator
    {
        public DtlsMutateClientHelloCookie(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "cookie" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] val = blob.Bytes();

            // C logic: mutate length 0..255, content random/structured
            int len = val.Length;
            uint cat = DtlsUtils.RndU32(100);

            if (cat < 10)
            {
                if (DtlsUtils.RndU32(2) == 0) len = 0;
                else len = (int)(16 + DtlsUtils.RndU32(17));
            }
            else if (cat < 57)
            {
                uint r = DtlsUtils.RndU32(100);
                if (r < 25) len = 0;
                else len = (int)(1 + DtlsUtils.RndU32(255));
            }

            byte[] newVal = new byte[len];
            NextBytes(newVal); // Fill random
            blob.MutatedValue = new Variant(newVal);
        }
    }

    [Mutator("DtlsAddClientHelloCipherSuites")]
    [CMutator("add_client_hello_cipher_suites")]
    [Description("Adds entries to Cipher Suites.")]
    public class DtlsAddClientHelloCipherSuites : LLMMutator
    {
        public DtlsAddClientHelloCipherSuites(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "cipher_suites" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            // Add a few common suites
            byte[] extra = { 0xC0, 0x2B, 0xC0, 0x2F }; // TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256 etc
            byte[] cur = blob.Bytes();
            byte[] newBytes = new byte[cur.Length + extra.Length];
            cur.CopyTo(newBytes, 0);
            extra.CopyTo(newBytes, cur.Length);
            blob.MutatedValue = new Variant(newBytes);
        }
    }

    [Mutator("DtlsDeleteClientHelloCipherSuites")]
    [CMutator("delete_client_hello_cipher_suites")]
    [Description("Deletes Cipher Suites (sets to empty).")]
    public class DtlsDeleteClientHelloCipherSuites : LLMMutator
    {
        public DtlsDeleteClientHelloCipherSuites(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "cipher_suites" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            blob.MutatedValue = new Variant(new byte[0]);
        }
    }

    [Mutator("DtlsMutateClientHelloCipherSuites")]
    [CMutator("mutate_client_hello_cipher_suites")]
    [Description("Mutates Cipher Suites list.")]
    public class DtlsMutateClientHelloCipherSuites : LLMMutator
    {
        public DtlsMutateClientHelloCipherSuites(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "cipher_suites" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] val = blob.Bytes();

            // Just randomize bytes for now as C logic does complex list manipulations
            // which are effectively just byte manipulations on the blob.
            if (val.Length > 0)
            {
                NextBytes(val);
                // Ensure length is even
                if (val.Length % 2 != 0) Array.Resize(ref val, val.Length - 1);
            }
            else
            {
                // Make some
                val = new byte[8];
                NextBytes(val);
            }
            blob.MutatedValue = new Variant(val);
        }
    }

    [Mutator("DtlsAddClientHelloCompressionMethods")]
    [CMutator("add_client_hello_compression_methods")]
    [Description("Adds Compression Methods.")]
    public class DtlsAddClientHelloCompressionMethods : LLMMutator
    {
        public DtlsAddClientHelloCompressionMethods(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "compression_methods" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            if (blob.Value.Length == 0)
            {
                blob.MutatedValue = new Variant(new byte[] { 0 }); // Null compression
            }
        }
    }

    [Mutator("DtlsDeleteClientHelloCompressionMethods")]
    [CMutator("delete_client_hello_compression_methods")]
    [Description("Deletes Compression Methods.")]
    public class DtlsDeleteClientHelloCompressionMethods : LLMMutator
    {
        public DtlsDeleteClientHelloCompressionMethods(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "compression_methods" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            blob.MutatedValue = new Variant(new byte[0]);
        }
    }

    [Mutator("DtlsMutateClientHelloCompressionMethods")]
    [CMutator("mutate_client_hello_compression_methods")]
    [Description("Mutates Compression Methods.")]
    public class DtlsMutateClientHelloCompressionMethods : LLMMutator
    {
        public DtlsMutateClientHelloCompressionMethods(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "compression_methods" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] val = blob.Bytes();

            // Randomize, ensure 0 is present sometimes
            if (val.Length > 0)
            {
                NextBytes(val);
                if (DtlsUtils.RndU32(2) == 0) val[0] = 0;
            }
            blob.MutatedValue = new Variant(val);
        }
    }

    [Mutator("DtlsAddClientHelloExtensions")]
    [CMutator("add_client_hello_extensions")]
    [Description("Adds Extensions to ClientHello.")]
    public class DtlsAddClientHelloExtensions : LLMMutator
    {
        public DtlsAddClientHelloExtensions(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "raw" && obj.parent.Name == "extensions" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            if (blob.Value.Length == 0)
            {
                // Add minimal extension (renegotiation info: 0xFF01 0001 00)
                byte[] ext = { 0xFF, 0x01, 0x00, 0x01, 0x00 };
                blob.MutatedValue = new Variant(ext);
            }
        }
    }

    [Mutator("DtlsDeleteClientHelloExtensions")]
    [CMutator("delete_client_hello_extensions")]
    [Description("Deletes Extensions from ClientHello.")]
    public class DtlsDeleteClientHelloExtensions : LLMMutator
    {
        public DtlsDeleteClientHelloExtensions(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "raw" && obj.parent.Name == "extensions" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            blob.MutatedValue = new Variant(new byte[0]);
        }
    }

    [Mutator("DtlsMutateClientHelloExtensions")]
    [CMutator("mutate_client_hello_extensions")]
    [Description("Mutates ClientHello Extensions raw blob.")]
    public class DtlsMutateClientHelloExtensions : LLMMutator
    {
        public DtlsMutateClientHelloExtensions(DataElement obj) : base(obj) { }

        public new static bool supportedDataElement(DataElement obj)
        {
            return obj is Blob && obj.Name == "raw" && obj.parent.Name == "extensions" && obj.IsIn("client_hello");
        }

        public override int count => 1;
        public override void sequentialMutation(DataElement obj) { PerformMutation(obj); }
        public override void randomMutation(DataElement obj) { PerformMutation(obj); }

        private void PerformMutation(DataElement obj)
        {
            var blob = obj as Blob;
            byte[] val = blob.Bytes();
            // Complex logic in C simplified to random bytes/fuzzing
            if (val.Length > 0)
            {
                NextBytes(val);
            }
            else
            {
                // Maybe create some
                val = new byte[16];
                NextBytes(val);
            }
            blob.MutatedValue = new Variant(val);
        }
    }
}
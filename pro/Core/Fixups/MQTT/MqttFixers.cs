using System;
using System.Collections.Generic;
using System.Linq;
using Peach.Core.Dom;

using uint8 = System.Byte;
using uint16 = System.UInt16;
using uint32 = System.UInt32;
using uint64 = System.UInt64;
using int8 = System.SByte;
using int16 = System.Int16;
using int32 = System.Int32;
using int64 = System.Int64;
using Peach.Core;

using Array = Peach.Core.Dom.Array;
using SArray = System.Array;
using Peach.Pro.Core.Dom;

namespace Peach.Pro.Core.Fixups.MQTT
{
    public class MqttFixers
    {
        private const int MaxPacketId = 65535;
        private const int MaxPropertiesLen = 256;

        private const uint8 PropIdPfi = 0x01;
        private const uint8 PropIdMei = 0x02;
        private const uint8 PropIdContentType = 0x03;
        private const uint8 PropIdResponseTopic = 0x08;
        private const uint8 PropIdCorrData = 0x09;
        private const uint8 PropIdSubId = 0x0B;
        private const uint8 PropIdTopicAlias = 0x23;
        private const uint8 PropIdUserProp = 0x26;

        private static readonly bool[] _packetIdUsed = new bool[MaxPacketId + 1];
        private static ushort _nextPacketId = 1;
        private static readonly System.Random _rng = new System.Random();

        private static ushort GetNextPacketId()
        {
            for (int i = 0; i < MaxPacketId; ++i)
            {
                ushort id = _nextPacketId++;
                if (_nextPacketId > MaxPacketId)
                    _nextPacketId = 1;
                if (!_packetIdUsed[id])
                {
                    _packetIdUsed[id] = true;
                    return id;
                }
            }
            return 1;
        }

        private static bool TryReadU16(byte[] buffer, int offset, int len, out ushort value)
        {
            if (offset + 2 > len)
            {
                value = 0;
                return false;
            }
            value = (ushort)((buffer[offset] << 8) | buffer[offset + 1]);
            return true;
        }

        private static bool TryReadVarInt(byte[] buffer, int offset, int len, out uint value, out int used)
        {
            uint mul = 1;
            value = 0;
            used = 0;
            while (used < 4 && offset + used < len)
            {
                byte b = buffer[offset + used];
                value += (uint)(b & 0x7F) * mul;
                used++;
                if ((b & 0x80) == 0)
                    return true;
                mul *= 128;
            }
            return false;
        }

        public static void FixConnectPacketWillRules(DataElement elem)
        {        
            unchecked
            {
                var connectFlags = elem.find("connect_flags") as Number;

                // bit:  7   6   5   4   3   2   1   0
                //       -----------------------------
                //       U   P   W   W   W   W   C   R
                //       S   W   R   Q   Q   F   L   E
                //       E   D   E   O   O   L   E   S
                //       R       T   S   S   A   A   V
                //       N       A           G   N   D
                //       A       I               S
                //       M       N               T
                //       E                       A
                //                               R
                //                               T
                var connectFlagsValue = connectFlags.GetUint8();

                connectFlagsValue &= 0b1111_1110;

                var willFlag = (uint8)(connectFlagsValue & 0b0000_0100) >> 2;
                var willQos = (uint8)(connectFlagsValue & 0b0001_1000) >> 3;
                // var willRetain = (uint8)((connectFlagsValue >> 5) & 0x01);

                var protocolLevel = (elem.find("protocol_level") as Number).GetUint8();

                var willPropertyLength = elem.find("will_property_length") as Array;
                var willProperties = elem.find("will_properties") as Array;
                var willTopic = elem.find("will_topic") as Array;
                var willPayloadLength = elem.find("will_payload_length") as Array;
                var willPayload = elem.find("will_payload") as Array;

                if (protocolLevel < 5)
                {
                    willPropertyLength?.parent?.Remove(willPropertyLength);
                    willProperties?.parent?.Remove(willProperties);
                }

                if (willFlag == 0)
                {
                    connectFlagsValue &= 0b1110_0111; // Clear Will QoS
                    connectFlagsValue &= 0b1101_1111; // Clear Will Retain

                    willPropertyLength?.parent?.Remove(willPropertyLength);
                    willProperties?.parent?.Remove(willProperties);
                    willPayloadLength?.parent?.Remove(willPayloadLength);
                    willPayload?.parent?.Remove(willPayload);
                    willTopic?.parent?.Remove(willTopic);
                }
                else
                {
                    if (willQos > 2)
                    {
                        connectFlagsValue &= 0b1110_0111; 
                    }

                    if (willTopic.Count == 0)
                    {
                        willTopic.SetUniqChild(new Variant("default/topic".ToMqttString()));
                    }

                    if (willPayloadLength.Count == 0)
                    {
                        var payload = "default_payload";
                        willPayloadLength.SetUniqChild(new Variant((uint16)payload.Length));
                        willPayload.SetUniqChild(new Variant(payload));
                    }

                    if (protocolLevel >= 5)
                    {
                        if (willPropertyLength.Count == 0 || willProperties.Count == 0)
                        {
                            willPropertyLength.SetUniqChild(new Variant(2));
                            willProperties.SetUniqChild(new Variant(new byte[] { 0x01, 0x00 }));
                        }
                    }
                }

                connectFlags.SetValue(new Variant(connectFlagsValue));
            }
        }

        public static void FixUserNameFlag(DataElement elem)
        {
            unchecked
            {
                var connectFlags = elem.find("connect_flags") as Number;
                if (connectFlags == null) return;

                var connectFlagsValue = connectFlags.GetUint8();
                var userNameFlag = (connectFlagsValue & 0b1000_0000) >> 7;

                var userName = elem.find("user_name") as Array;
                var passwordLength = elem.find("password_length") as Array;
                var password = elem.find("password") as Array;

                if (userNameFlag == 0)
                {
                    userName?.parent.Remove(userName);

                    connectFlagsValue &= 0b1011_1111; // Clear Password Flag
                    passwordLength?.parent.Remove(passwordLength);
                    password?.parent.Remove(password);

                    connectFlags.SetValue(new Variant(connectFlagsValue));
                }
                else
                {
                    if (userName != null && userName.Count == 0)
                    {
                        userName.SetUniqChild(new Variant("default_user".ToMqttString()));
                    }
                }
            }
        }

        public static void FixPasswordFlag(DataElement elem)
        {
            unchecked
            {
                var connectFlags = elem.find("connect_flags") as Number;
                if (connectFlags == null) return;

                var connectFlagsValue = connectFlags.GetUint8();
                var passwordFlag = (connectFlagsValue & 0b0100_0000) >> 6;

                var passwordLength = elem.find("password_length") as Array;
                var password = elem.find("password") as Array;

                if (passwordFlag == 0)
                {
                    passwordLength?.parent.Remove(passwordLength);
                    password?.parent.Remove(password);
                }
                else
                {
                    if (passwordLength != null && passwordLength.Count == 0)
                    {
                        var pwd = "default_pass";
                        passwordLength.SetUniqChild(new Variant((uint16)pwd.Length));
                        password?.SetUniqChild(new Variant(pwd));
                    }
                }
            }
        }

        private static uint8 GetPublishQosValue(DataElement elem)
        {
            var bodyVariant = elem.find("body_variant") as Choice;
            var variant = bodyVariant?.SelectedElement;

            var fixedHeader = variant?.find("fixed_header") as DataElementContainer;

            byte qos = 0;
            var qosElem = fixedHeader?.find("qos");
            if (qosElem is Number qn)
                qos = qn.GetUint8();
            else if (qosElem is Choice qc && qc.SelectedElement is Number qsel)
                qos = qsel.GetUint8();

            return qos;
        }

        private static void SetPublishQosValue(DataElement elem, uint8 qos)
        {
            var bodyVariant = elem.find("body_variant") as Choice;
            var variant = bodyVariant?.SelectedElement;

            var fixedHeader = variant?.find("fixed_header") as DataElementContainer;

            var qosElem = fixedHeader?.find("qos");
            qosElem?.SetValue(new Variant(new byte[] { qos }));
        }

        public static void FixPublishPacketIdentifier(DataElement elem)
        {
            var variableHeader = elem.find("msg_body")?.find("variable_header");

            var qos = GetPublishQosValue(elem);

            var pid = variableHeader?.find("packet_identifier") as Number;
            if (qos == 0 && pid != null)
                pid.SetValue(new Variant(0));
        }

        public static void FixPublishPacketIdentifierUnique(DataElement elem)
        {
            var qos = GetPublishQosValue(elem);

            var pid = elem?.find("packet_identifier") as Number;
            if (qos > 0)
            {
                if (pid != null)
                {
                    ushort val = pid.GetUint16();
                    if (val == 0 || _packetIdUsed[val])
                        val = GetNextPacketId();
                    _packetIdUsed[val] = true;
                    pid.SetValue(new Variant(val));
                }
            }
            else
            {
                pid?.parent?.Remove(pid);
            }
        }

        public static void FixPublishDupFlag(DataElement elem)
        {
            var dup = elem?.find("dup") as Number;

            var qos = GetPublishQosValue(elem);

            dup?.SetValue(new Variant(qos == 0 ? (uint8)0 : (uint8)_rng.Next(2)));
        }

        public static void FixPublishQosBits(DataElement elem)
        {
            var qos = GetPublishQosValue(elem);
            if (qos > 2)
            {
                SetPublishQosValue(elem, (uint8)_rng.Next(3));
            }
        }

        public static void FixPublishTopicAlias(DataElement elem, ushort connackAliasMax)
        {
            var propsElem = elem.find("properties") as Blob;
            var props = propsElem.Bytes();
            var lenElem = elem.find("property_length") as MqttVarInt;
            var len = lenElem.GetVarInt();
            var newProps = new List<byte>(MaxPropertiesLen);

            var topicNameElem = elem.find("topic_name");

            var allowAlias = connackAliasMax > 0;
            bool seenAlias = false;
            // TODO
            bool topicEmpty = (topicNameElem.find("length") as Number).GetUint16() == 0 || topicNameElem.find("value").Bytes().Length == 0;

            for (ulong j = 0; j < len && newProps.Count < MaxPropertiesLen;)
            {
                uint8 id = props[j++];

                switch (id)
                {
                    case PropIdPfi:
                        {
                            if (j + 1 > len) { j = len; break; }
                            byte v = props[j++];
                            v = (byte)(v != 0 ? 1 : 0);
                            if (newProps.Count + 2 > MaxPropertiesLen) { j = len; break; }
                            newProps.Add(PropIdPfi);
                            newProps.Add(v);
                            break;
                        }

                    case PropIdMei:
                        {
                            if (j + 4 > len) { j = len; break; }
                            if (newProps.Count + 5 > MaxPropertiesLen) { j = len; break; }
                            newProps.Add(PropIdMei);
                            newProps.AddRange(props.Skip((int)j).Take(4));
                            j += 4;
                            break;
                        }

                    case PropIdContentType:
                    case PropIdResponseTopic:
                        {
                            if (!TryReadU16(props, (int)j, (int)len, out ushort n)) { j = len; break; }
                            if (j + 2 + n > len) { j = len; break; }
                            int need = 1 + 2 + n;
                            if (newProps.Count + need > MaxPropertiesLen) { j = len; break; }
                            newProps.Add(id);
                            newProps.Add(props[j]);
                            newProps.Add(props[j + 1]);
                            newProps.AddRange(props.Skip((int)(j + 2)).Take(n));
                            j += (ulong)2 + n;
                            break;
                        }

                    case PropIdCorrData:
                        {
                            if (!TryReadU16(props, (int)j, (int)len, out ushort n)) { j = len; break; }
                            if (j + 2 + n > len) { j = len; break; }
                            int need = 1 + 2 + n;
                            if (newProps.Count + need > MaxPropertiesLen) { j = len; break; }
                            newProps.Add(PropIdCorrData);
                            newProps.Add(props[j]);
                            newProps.Add(props[j + 1]);
                            newProps.AddRange(props.Skip((int)(j + 2)).Take(n));
                            j += (ulong)2 + n;
                            break;
                        }

                    case PropIdSubId:
                        {
                            if (!TryReadVarInt(props, (int)j, (int)len, out uint _, out int used)) { j = len; break; }
                            if (newProps.Count + 1 + used > MaxPropertiesLen) { j = len; break; }
                            newProps.Add(PropIdSubId);
                            newProps.AddRange(props.Skip((int)j).Take(used));
                            j += (ulong)used;
                            break;
                        }

                    case PropIdTopicAlias:
                        {
                            if (j + 2 > len) { j = len; break; }
                            ushort alias = (uint16)((props[j] << 8) | props[j + 1]);
                            j += 2;

                            if (seenAlias)
                                break;

                            if (!allowAlias)
                            {
                                if (topicEmpty)
                                {
                                    alias = 1;
                                    if (newProps.Count + 3 > MaxPropertiesLen) { j = len; break; }
                                    newProps.Add(PropIdTopicAlias);
                                    newProps.Add((byte)(alias >> 8));
                                    newProps.Add((byte)(alias & 0xFF));
                                    seenAlias = true;
                                }
                                break;
                            }

                            if (alias == 0) alias = 1;
                            if (alias > connackAliasMax) alias = connackAliasMax;

                            if (newProps.Count + 3 > MaxPropertiesLen) { j = len; break; }
                            newProps.Add(PropIdTopicAlias);
                            newProps.Add((byte)(alias >> 8));
                            newProps.Add((byte)(alias & 0xFF));
                            seenAlias = true;
                            break;
                        }

                    case PropIdUserProp:
                        {
                            if (!TryReadU16(props, (int)j, (int)len, out ushort klen)) { j = len; break; }
                            if (j + 2 + klen + 2 > len) { j = len; break; }
                            if (!TryReadU16(props, (int)(j + 2 + klen), (int)len, out ushort vlen)) { j = len; break; }
                            if (j + 2 + klen + 2 + vlen > len) { j = len; break; }

                            int need = 1 + 2 + klen + 2 + vlen;
                            if (newProps.Count + need > MaxPropertiesLen) { j = len; break; }

                            newProps.Add(PropIdUserProp);
                            newProps.Add(props[j]);
                            newProps.Add(props[j + 1]);
                            newProps.AddRange(props.Skip((int)(j + 2)).Take(klen));
                            newProps.Add(props[j + 2 + klen]);
                            newProps.Add(props[j + 2 + klen + 1]);
                            newProps.AddRange(props.Skip((int)(j + 2 + klen + 2)).Take(vlen));
                            j += 2UL + klen + 2 + vlen;
                            break;
                        }

                    default:
                        {
                            int rem = (int)(len - (j - 1));
                            int cap = MaxPropertiesLen - newProps.Count;
                            if (rem > cap) rem = cap;
                            if (rem > 0)
                                newProps.AddRange(props.Skip((int)(j - 1)).Take(rem));
                            j = len;
                            break;
                        }
                }
            }

            lenElem.SetValue(new Variant((uint)newProps.Count));
            propsElem.SetValue(new Variant(newProps.ToArray()));
        }

        public static void FixPublishResponseTopic(DataElement elem)
        {
            var propsElem = elem.find("properties") as Blob;
            var props = propsElem.Bytes();
            var lenElem = elem.find("property_length") as MqttVarInt;
            var len = (int)lenElem.InternalValue;
            var newProps = new List<byte>(MaxPropertiesLen);

            bool seenRespTopic = false;

            for (int j = 0; j < len && newProps.Count < MaxPropertiesLen;)
            {
                byte id = props[j++];

                switch (id)
                {
                    case PropIdPfi:
                        {
                            if (j + 1 > len) { j = len; break; }
                            byte v = props[j++];
                            v = (byte)(v != 0 ? 1 : 0);
                            if (newProps.Count + 2 > MaxPropertiesLen) { j = len; break; }
                            newProps.Add(PropIdPfi);
                            newProps.Add(v);
                            break;
                        }

                    case PropIdMei:
                        {
                            if (j + 4 > len) { j = len; break; }
                            if (newProps.Count + 5 > MaxPropertiesLen) { j = len; break; }
                            newProps.Add(PropIdMei);
                            newProps.AddRange(props.Skip(j).Take(4));
                            j += 4;
                            break;
                        }

                    case PropIdContentType:
                        {
                            if (!TryReadU16(props, j, len, out ushort n)) { j = len; break; }
                            if (j + 2 + n > len) { j = len; break; }
                            int need = 1 + 2 + n;
                            if (newProps.Count + need > MaxPropertiesLen) { j = len; break; }
                            newProps.Add(PropIdContentType);
                            newProps.Add(props[j]);
                            newProps.Add(props[j + 1]);
                            newProps.AddRange(props.Skip(j + 2).Take(n));
                            j += 2 + n;
                            break;
                        }

                    case PropIdResponseTopic:
                        {
                            if (!TryReadU16(props, j, len, out ushort n)) { j = len; break; }
                            if (j + 2 + n > len) { j = len; break; }

                            bool hasWildcard = false;
                            for (int k = 0; k < n; ++k)
                            {
                                byte b = props[j + 2 + k];
                                if (b == (byte)'+' || b == (byte)'#')
                                {
                                    hasWildcard = true;
                                    break;
                                }
                            }

                            bool drop = seenRespTopic || hasWildcard;
                            if (!drop)
                            {
                                int need = 1 + 2 + n;
                                if (newProps.Count + need > MaxPropertiesLen) { j = len; break; }
                                newProps.Add(PropIdResponseTopic);
                                newProps.Add(props[j]);
                                newProps.Add(props[j + 1]);
                                newProps.AddRange(props.Skip(j + 2).Take(n));
                                seenRespTopic = true;
                            }
                            j += 2 + n;
                            break;
                        }

                    case PropIdCorrData:
                        {
                            if (!TryReadU16(props, j, len, out ushort n)) { j = len; break; }
                            if (j + 2 + n > len) { j = len; break; }
                            int need = 1 + 2 + n;
                            if (newProps.Count + need > MaxPropertiesLen) { j = len; break; }
                            newProps.Add(PropIdCorrData);
                            newProps.Add(props[j]);
                            newProps.Add(props[j + 1]);
                            newProps.AddRange(props.Skip(j + 2).Take(n));
                            j += 2 + n;
                            break;
                        }

                    case PropIdSubId:
                        {
                            if (!TryReadVarInt(props, j, len, out uint _, out int used)) { j = len; break; }
                            if (newProps.Count + 1 + used > MaxPropertiesLen) { j = len; break; }
                            newProps.Add(PropIdSubId);
                            newProps.AddRange(props.Skip(j).Take(used));
                            j += used;
                            break;
                        }

                    case PropIdTopicAlias:
                        {
                            if (j + 2 > len) { j = len; break; }
                            if (newProps.Count + 3 > MaxPropertiesLen) { j = len; break; }
                            newProps.Add(PropIdTopicAlias);
                            newProps.Add(props[j]);
                            newProps.Add(props[j + 1]);
                            j += 2;
                            break;
                        }

                    case PropIdUserProp:
                        {
                            if (!TryReadU16(props, j, len, out ushort klen)) { j = len; break; }
                            if (j + 2 + klen + 2 > len) { j = len; break; }
                            if (!TryReadU16(props, j + 2 + klen, len, out ushort vlen)) { j = len; break; }
                            if (j + 2 + klen + 2 + vlen > len) { j = len; break; }

                            int need = 1 + 2 + klen + 2 + vlen;
                            if (newProps.Count + need > MaxPropertiesLen) { j = len; break; }

                            newProps.Add(PropIdUserProp);
                            newProps.Add(props[j]);
                            newProps.Add(props[j + 1]);
                            newProps.AddRange(props.Skip(j + 2).Take(klen));
                            newProps.Add(props[j + 2 + klen]);
                            newProps.Add(props[j + 2 + klen + 1]);
                            newProps.AddRange(props.Skip(j + 2 + klen + 2).Take(vlen));
                            j += 2 + klen + 2 + vlen;
                            break;
                        }

                    default:
                        {
                            int rem = len - (j - 1);
                            int cap = MaxPropertiesLen - newProps.Count;
                            if (rem > cap) rem = cap;
                            if (rem > 0)
                                newProps.AddRange(props.Skip(j - 1).Take(rem));
                            j = len;
                            break;
                        }
                }
            }

            lenElem.SetValue(new Variant((uint)newProps.Count));
            propsElem.SetValue(new Variant(newProps.ToArray()));
        }

        public static void FixPublishSubscriptionIdentifier(DataElement elem)
        {
            var propsElem = elem.find("properties") as Blob;
            var props = propsElem.Bytes();
            var lenElem = elem.find("property_length") as MqttVarInt;
            var len = (int)lenElem.InternalValue;
            var newProps = new List<byte>(MaxPropertiesLen);

            for (int j = 0; j < len && newProps.Count < MaxPropertiesLen;)
            {
                byte id = props[j++];

                switch (id)
                {
                    case PropIdPfi:
                        {
                            if (j + 1 > len) { j = len; break; }
                            byte v = props[j++];
                            v = (byte)(v != 0 ? 1 : 0);
                            if (newProps.Count + 2 > MaxPropertiesLen) { j = len; break; }
                            newProps.Add(PropIdPfi);
                            newProps.Add(v);
                            break;
                        }

                    case PropIdMei:
                        {
                            if (j + 4 > len) { j = len; break; }
                            if (newProps.Count + 5 > MaxPropertiesLen) { j = len; break; }
                            newProps.Add(PropIdMei);
                            newProps.AddRange(props.Skip(j).Take(4));
                            j += 4;
                            break;
                        }

                    case PropIdContentType:
                    case PropIdResponseTopic:
                        {
                            if (!TryReadU16(props, j, len, out ushort n)) { j = len; break; }
                            if (j + 2 + n > len) { j = len; break; }
                            int need = 1 + 2 + n;
                            if (newProps.Count + need > MaxPropertiesLen) { j = len; break; }
                            newProps.Add(id);
                            newProps.Add(props[j]);
                            newProps.Add(props[j + 1]);
                            newProps.AddRange(props.Skip(j + 2).Take(n));
                            j += 2 + n;
                            break;
                        }

                    case PropIdCorrData:
                        {
                            if (!TryReadU16(props, j, len, out ushort n)) { j = len; break; }
                            if (j + 2 + n > len) { j = len; break; }
                            int need = 1 + 2 + n;
                            if (newProps.Count + need > MaxPropertiesLen) { j = len; break; }
                            newProps.Add(PropIdCorrData);
                            newProps.Add(props[j]);
                            newProps.Add(props[j + 1]);
                            newProps.AddRange(props.Skip(j + 2).Take(n));
                            j += 2 + n;
                            break;
                        }

                    case PropIdSubId:
                        {
                            if (!TryReadVarInt(props, j, len, out uint _, out int used)) { j = len; break; }
                            j += used; // drop it
                            break;
                        }

                    case PropIdTopicAlias:
                        {
                            if (j + 2 > len) { j = len; break; }
                            if (newProps.Count + 3 > MaxPropertiesLen) { j = len; break; }
                            newProps.Add(PropIdTopicAlias);
                            newProps.Add(props[j]);
                            newProps.Add(props[j + 1]);
                            j += 2;
                            break;
                        }

                    case PropIdUserProp:
                        {
                            if (!TryReadU16(props, j, len, out ushort klen)) { j = len; break; }
                            if (j + 2 + klen + 2 > len) { j = len; break; }
                            if (!TryReadU16(props, j + 2 + klen, len, out ushort vlen)) { j = len; break; }
                            if (j + 2 + klen + 2 + vlen > len) { j = len; break; }

                            int need = 1 + 2 + klen + 2 + vlen;
                            if (newProps.Count + need > MaxPropertiesLen) { j = len; break; }

                            newProps.Add(PropIdUserProp);
                            newProps.Add(props[j]);
                            newProps.Add(props[j + 1]);
                            newProps.AddRange(props.Skip(j + 2).Take(klen));
                            newProps.Add(props[j + 2 + klen]);
                            newProps.Add(props[j + 2 + klen + 1]);
                            newProps.AddRange(props.Skip(j + 2 + klen + 2).Take(vlen));
                            j += 2 + klen + 2 + vlen;
                            break;
                        }

                    default:
                        {
                            int rem = len - (j - 1);
                            int cap = MaxPropertiesLen - newProps.Count;
                            if (rem > cap) rem = cap;
                            if (rem > 0)
                                newProps.AddRange(props.Skip(j - 1).Take(rem));
                            j = len;
                            break;
                        }
                }
            }

            lenElem.SetValue(new Variant((uint)newProps.Count));
            propsElem.SetValue(new Variant(newProps.ToArray()));
        }

        public static void FixPublishDeliveryProtocol(DataElement elem)
        {
            var qos = GetPublishQosValue(elem);
            var dup = elem?.find("dup") as Number;
            var pid = elem?.find("packet_identifier") as Number;

            if (qos == 0)
            {
                dup?.SetValue(new Variant(0));
                pid?.parent?.Remove(pid);
            }
            else if (qos == 1 || qos == 2)
            {
                if (pid != null)
                {
                    ushort val = pid.GetUint16();
                    if (val == 0 || _packetIdUsed[val])
                        val = GetNextPacketId();
                    _packetIdUsed[val] = true;
                    pid.SetValue(new Variant(val));
                }

                dup?.SetValue(new Variant(0));
            }
            else
            {
                SetPublishQosValue(elem, 0);
                dup?.SetValue(new Variant(0));
                pid?.SetValue(new Variant(0));
            }
        }

        public static void FixSubscribeNoLocal(DataElement elem)
        {
            var topicFilters = elem.find("topic_filters") as Array;
            var len = topicFilters.Count;
            for (int i = 0; i < len; ++i)
            {
                var qos = topicFilters[i].find("qos") as Number;
                var v = qos.GetUint8();
                unchecked
                {
                    v &= (uint8)~(1 << 2);
                }
                qos.SetValue(new Variant(v));
            }
        }

        public static void FixSubscribePacketIdentifier(DataElement elem)
        {
            var pid = elem.find("packet_identifier") as Number;
            var val = pid.GetUint16();
            if (val == 0)
                val = GetNextPacketId();
            _packetIdUsed[val] = true;
            pid.SetValue(new Variant(val));
        }

        public static void FixSubscribePacketIdentifierUnique(DataElement elem)
        {
            var pid = elem.find("packet_identifier") as Number;
            var val = pid.GetUint16();
            if (val == 0 || _packetIdUsed[val])
                val = GetNextPacketId();
            _packetIdUsed[val] = true;
            pid.SetValue(new Variant(val));
        }

        public static void FixUnsubscribePacketIdentifier(DataElement elem)
        {
            var pid = elem.find("packet_identifier") as Number;
            var val = pid.GetUint16();
            if (val == 0 || _packetIdUsed[val])
                val = GetNextPacketId();
            _packetIdUsed[val] = true;
            pid.SetValue(new Variant(val));
        }

        public static void ResetPacketIdTracking()
        {
            System.Array.Clear(_packetIdUsed, 0, _packetIdUsed.Length);    
            _nextPacketId = 1;
        }

        public static void FixConnect(DataElement elem)
        {
            var willFlag = (elem.find("connect_flags") as Number).GetUint8() & 0b0000_0100;
            var before = elem.Bytes();
            FixConnectPacketWillRules(elem);
            FixUserNameFlag(elem);
            FixPasswordFlag(elem);
            var afterWillFlag = (elem.find("connect_flags") as Number).GetUint8() & 0b0000_0100;

            if (willFlag > 0 && afterWillFlag == 0)
            {
                Console.WriteLine("Warning: Will Flag was set, but got cleared after Fix");
                before.DumpDiff(elem.Bytes());
            }
        }

        static void _FixPublishTopicAlias(DataElement elem)
        {
            FixPublishTopicAlias(elem, 65535);
        }

        public static void FixPublish(DataElement elem)
        {
            FixPublishPacketIdentifier(elem);
            FixPublishPacketIdentifierUnique(elem);
            FixPublishDupFlag(elem);
            FixPublishQosBits(elem);
            FixPublishTopicAlias(elem, 65535);
            FixPublishResponseTopic(elem);
            FixPublishSubscriptionIdentifier(elem);
            // FixPublishDeliveryProtocol(elem);

            // var fixers = new List<Action<DataElement>>()
            // {
            //     FixPublishPacketIdentifier,
            //     FixPublishPacketIdentifierUnique,
            //     FixPublishDupFlag,
            //     FixPublishQosBits,
            //     _FixPublishTopicAlias,
            //     FixPublishResponseTopic,
            //     FixPublishSubscriptionIdentifier,
            //     // FixPublishDeliveryProtocol
            // };

            // Console.WriteLine($"Fix {elem.parent.parent.parent.Name}:");

            // fixers.ForEach(fixer =>
            // {
            //     var p = elem.Bytes();
            //     fixer(elem);
            //     Console.WriteLine($"After Fixer {fixer.Method.Name}:");
            //     p.DumpDiff(elem.Bytes());
            //     Console.WriteLine();
            // });
        }

        public static void FixSubscribe(DataElement elem)
        {
            FixSubscribeNoLocal(elem);
            FixSubscribePacketIdentifier(elem);
            FixSubscribePacketIdentifierUnique(elem);
        }

        public static void FixUnsubscribe(DataElement elem)
        {
            FixUnsubscribePacketIdentifier(elem);
        }
    }
}
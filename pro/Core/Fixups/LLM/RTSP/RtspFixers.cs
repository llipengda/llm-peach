using System;
using System.Collections.Generic;
using System.Linq;
using Peach.Core;
using Peach.Core.Dom;

using Array = Peach.Core.Dom.Array;
using Peach.Pro.Core.Dom;

namespace Peach.Pro.Core.Fixups.LLM.RTSP
{
    public class RtspFixers
    {
        // private static readonly System.Random _rng = new System.Random();
        private static readonly Dictionary<string, int> _requestCseqMap = new Dictionary<string, int>();
        private static int _lastAssignedCseq = 0;
        private static int _nextInterleavedChannel = 0;  // For TCP interleaved channel allocation

        // Helper: Safe string copy with null check
        private static void SetString(DataElement elem, string value)
        {
            if (elem != null && elem is Peach.Core.Dom.String str)
            {
                str.SetValue(new Variant(value));
            }
        }

        // Helper: Get string value safely
        private static string GetString(DataElement elem)
        {
            if (elem != null && elem is Peach.Core.Dom.String str)
            {
                var val = str.InternalValue;
                return val != null ? val.ToString() : "";
            }
            return "";
        }

        // Helper: Get number value safely
        private static int GetNumber(DataElement elem)
        {
            if (elem != null && elem is Number num)
            {
                return (int)num.InternalValue;
            }
            return 0;
        }

        // Helper: Set number value safely
        private static void SetNumber(DataElement elem, int value)
        {
            if (elem != null && elem is Number num)
            {
                num.SetValue(new Variant(value));
            }
        }

        // Helper: Check if string is empty or null
        private static bool IsEmpty(string s)
        {
            return string.IsNullOrEmpty(s);
        }

        // Helper: Case-insensitive string comparison
        private static bool StrIeq(string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        // Helper: Check if URI is absolute
        private static bool IsAbsoluteUri(string uri)
        {
            if (IsEmpty(uri)) return false;
            return uri.Contains("://");
        }

        // Helper: Check if URI is "*"
        private static bool IsStarUri(string uri)
        {
            return uri == "*";
        }

        // Helper: Get Content-Base header from packet if it exists and is absolute URI
        private static string GetContentBaseIfAbsFromPacket(DataElement elem)
        {
            // Check for Content-Base header in DESCRIBE and GET_PARAMETER messages
            var contentBaseBlock = elem.find("content_base");
            if (contentBaseBlock != null)
            {
                var contentBaseUri = contentBaseBlock.find("uri") as Peach.Core.Dom.String;
                if (contentBaseUri != null)
                {
                    var uri = GetString(contentBaseUri);
                    if (!IsEmpty(uri) && IsAbsoluteUri(uri))
                    {
                        return uri;
                    }
                }
            }
            return null;
        }

        // Helper: Parse NPT seconds (supports "now" special value)
        private static int ParseNptSeconds(string txt, out double outValue)
        {
            outValue = 0.0;
            if (IsEmpty(txt)) return -1;
            if (StrIeq(txt, "now")) return 1; // Special value: "now"
            
            if (double.TryParse(txt, out outValue))
            {
                return 0; // Success
            }
            return -1; // Parse failed
        }

        // Helper: Parse SMPTE time (hh:mm:ss[.frac] or hh:mm:ss;ff)
        private static int ParseSmpteSeconds(string txt, out double outValue)
        {
            outValue = 0.0;
            if (IsEmpty(txt)) return -1;
            
            // Try hh:mm:ss[.frac] format
            var parts = txt.Split(':');
            if (parts.Length == 3)
            {
                int h, m;
                double s;
                if (int.TryParse(parts[0], out h) && 
                    int.TryParse(parts[1], out m) && 
                    double.TryParse(parts[2], out s))
                {
                    outValue = h * 3600.0 + m * 60.0 + s;
                    return 0;
                }
            }
            
            // Try hh:mm:ss;ff format
            var semicolonParts = txt.Split(';');
            if (semicolonParts.Length >= 1)
            {
                var timeParts = semicolonParts[0].Split(':');
                if (timeParts.Length == 3)
                {
                    int h, m, ss;
                    if (int.TryParse(timeParts[0], out h) && 
                        int.TryParse(timeParts[1], out m) && 
                        int.TryParse(timeParts[2], out ss))
                    {
                        outValue = h * 3600.0 + m * 60.0 + ss;
                        return 0;
                    }
                }
            }
            
            return -1;
        }

        // Helper: Parse port range ("N" or "N-M")
        private static bool ParsePortRange(string s, out int p1, out int p2)
        {
            p1 = -1;
            p2 = -1;
            if (IsEmpty(s)) return false;
            
            var dashIdx = s.IndexOf('-');
            if (dashIdx < 0)
            {
                // Single port
                return int.TryParse(s, out p1);
            }
            else
            {
                // Port range
                var part1 = s.Substring(0, dashIdx);
                var part2 = s.Substring(dashIdx + 1);
                if (int.TryParse(part1, out p1))
                {
                    if (IsEmpty(part2))
                    {
                        p2 = -1;
                        return true;
                    }
                    return int.TryParse(part2, out p2);
                }
            }
            return false;
        }

        // Fix CSeq header
        public static void FixCseq(DataElement elem)
        {
            var commonHeaders = elem.find("common_headers");
            if (commonHeaders == null) return;

            var cseq = commonHeaders.find("cseq") as Peach.Core.Dom.String;
            
            if (cseq != null)
            {
                var cseqValue = GetString(cseq);
                int cseqNum = 0;
                
                if (!int.TryParse(cseqValue, out cseqNum) || cseqNum <= 0)
                {
                    cseqNum = ++_lastAssignedCseq;
                }

                // Generate key for request tracking
                var requestLine = elem.find("request_line");
                var method = requestLine != null ? GetString(requestLine.find("method")) : "";
                var uri = requestLine != null ? GetString(requestLine.find("uri")) : "";
                
                var sessionBlock = elem.find("session");
                var session = sessionBlock != null ? GetString(sessionBlock.find("session_id")) : "";
                
                var key = $"{method} {uri}";
                if (!IsEmpty(session))
                {
                    key += $" {session}";
                }

                // Check if this is a retransmission
                if (_requestCseqMap.ContainsKey(key))
                {
                    cseqNum = _requestCseqMap[key];
                }
                else
                {
                    _requestCseqMap[key] = cseqNum;
                }

                SetString(cseq, cseqNum.ToString());
            }
        }

        // Fix Accept header
        public static void FixAccept(DataElement elem)
        {
            var acceptBlock = elem.find("accept");
            if (acceptBlock == null) return;

            var acceptName = acceptBlock.find("h_accept_name") as Peach.Core.Dom.String;
            var accept = acceptBlock.find("accept") as Peach.Core.Dom.String;

            if (acceptName != null)
            {
                SetString(acceptName, "Accept:");
            }

            if (accept != null)
            {
                var value = GetString(accept);
                if (IsEmpty(value))
                {
                    SetString(accept, "application/sdp");
                }
                else if (!value.Contains("/"))
                {
                    // Ensure slash is present
                    SetString(accept, value.Contains("/") ? value : "application/sdp");
                }
            }
        }

        // Fix Transport header - with TCP interleaved support
        public static void FixTransport(DataElement elem)
        {
            var transportBlock = elem.find("transport");
            if (transportBlock == null) return;

            var transportName = transportBlock.find("h_transport_name") as Peach.Core.Dom.String;
            var transport = transportBlock.find("transport") as Peach.Core.Dom.String;

            if (transportName != null)
            {
                SetString(transportName, "Transport:");
            }

            if (transport != null)
            {
                var value = GetString(transport);
                if (IsEmpty(value))
                {
                    SetString(transport, "RTP/AVP/UDP;unicast;client_port=8000-8001");
                }
                else
                {
                    // Normalize protocol - distinguish UDP/TCP
                    bool isTcp = value.Contains("/TCP") || value.Contains("/Tcp") || value.Contains("/tcp");
                    bool isUdp = value.Contains("/UDP") || value.Contains("/Udp") || value.Contains("/udp");
                    
                    if (isTcp)
                    {
                        value = value.Replace(value.Split('/')[0] + "/", "RTP/AVP/");
                        if (!value.Contains("RTP/AVP/TCP"))
                        {
                            value = value.Replace("RTP/AVP", "RTP/AVP/TCP");
                        }
                    }
                    else if (isUdp)
                    {
                        value = value.Replace(value.Split('/')[0] + "/", "RTP/AVP/");
                        if (!value.Contains("RTP/AVP/UDP"))
                        {
                            value = value.Replace("RTP/AVP", "RTP/AVP/UDP");
                        }
                    }
                    else if (!value.Contains("RTP/AVP"))
                    {
                        // Default to UDP if protocol not specified
                        value = "RTP/AVP/UDP" + (value.Contains("/") ? value.Substring(value.IndexOf('/')) : "");
                    }
                    else
                    {
                        // Default to UDP if RTP/AVP without /UDP or /TCP
                        if (!value.Contains("/UDP") && !value.Contains("/TCP"))
                        {
                            value = value.Replace("RTP/AVP", "RTP/AVP/UDP");
                        }
                    }

                    // Normalize cast mode
                    if (!value.Contains("unicast") && !value.Contains("multicast"))
                    {
                        if (value.Contains(";"))
                        {
                            value = value.Replace(";", ";unicast;");
                        }
                        else
                        {
                            value += ";unicast";
                        }
                    }

                    // Handle TCP interleaved or UDP client_port
                    if (isTcp || value.Contains("RTP/AVP/TCP"))
                    {
                        // TCP: use interleaved= with channel pairs (0-1, 2-3, ...)
                        if (value.Contains("interleaved="))
                        {
                            var interleavedStart = value.IndexOf("interleaved=") + 12;
                            var interleavedEnd = value.IndexOfAny(new[] { ';', ' ' }, interleavedStart);
                            if (interleavedEnd < 0) interleavedEnd = value.Length;

                            var channelRange = value.Substring(interleavedStart, interleavedEnd - interleavedStart);
                            int c1, c2;
                            if (ParsePortRange(channelRange, out c1, out c2))
                            {
                                // Normalize to even-odd pair, range 0-255
                                if (c1 < 0 || c1 > 255)
                                {
                                    // Allocate new channel pair
                                    c1 = (_nextInterleavedChannel % 2 == 0) ? _nextInterleavedChannel : (_nextInterleavedChannel - 1);
                                    if (c1 < 0) c1 = 0;
                                    if (c1 > 254) c1 = 254;
                                    c2 = c1 + 1;
                                    _nextInterleavedChannel = c1 + 2;
                                    if (_nextInterleavedChannel > 254) _nextInterleavedChannel = 0;
                                }
                                else
                                {
                                    // Normalize existing value
                                    if (c2 < 0) c2 = c1 + 1;
                                    if (c1 % 2 != 0) { c1 = (c1 > 0) ? (c1 - 1) : 0; c2 = c1 + 1; }
                                    if (c1 < 0) c1 = 0;
                                    if (c2 != c1 + 1) c2 = c1 + 1;
                                    if (c2 > 255) { c1 = 254; c2 = 255; }
                                }
                                
                                var newChannelRange = $"{c1}-{c2}";
                                value = value.Substring(0, interleavedStart) + newChannelRange + value.Substring(interleavedEnd);
                            }
                            else
                            {
                                // Invalid, allocate new
                                c1 = (_nextInterleavedChannel % 2 == 0) ? _nextInterleavedChannel : (_nextInterleavedChannel - 1);
                                if (c1 < 0) c1 = 0;
                                if (c1 > 254) c1 = 254;
                                c2 = c1 + 1;
                                _nextInterleavedChannel = c1 + 2;
                                if (_nextInterleavedChannel > 254) _nextInterleavedChannel = 0;
                                var newChannelRange = $"{c1}-{c2}";
                                value = value.Substring(0, interleavedStart) + newChannelRange + value.Substring(interleavedEnd);
                            }
                        }
                        else
                        {
                            // Add interleaved= if missing
                            if (!value.EndsWith(";"))
                                value += ";";
                            // Allocate new channel pair
                            int c1 = (_nextInterleavedChannel % 2 == 0) ? _nextInterleavedChannel : (_nextInterleavedChannel - 1);
                            if (c1 < 0) c1 = 0;
                            if (c1 > 254) c1 = 254;
                            int c2 = c1 + 1;
                            _nextInterleavedChannel = c1 + 2;
                            if (_nextInterleavedChannel > 254) _nextInterleavedChannel = 0;
                            value += $"interleaved={c1}-{c2}";
                        }
                    }
                    else
                    {
                        // UDP: normalize client_port
                        if (value.Contains("client_port="))
                        {
                            var portStart = value.IndexOf("client_port=") + 12;
                            var portEnd = value.IndexOfAny(new[] { ';', ' ' }, portStart);
                            if (portEnd < 0) portEnd = value.Length;

                            var portRange = value.Substring(portStart, portEnd - portStart);
                            int p1, p2;
                            if (ParsePortRange(portRange, out p1, out p2))
                            {
                                // Ensure even-odd pair
                                if (p2 < 0) p2 = p1 + 1;
                                if (p1 < 0 || p1 > 65534) p1 = 5000;
                                if (p2 != p1 + 1) p2 = p1 + 1;
                                if (p1 % 2 != 0) { p1 = (p1 > 0) ? (p1 - 1) : 0; p2 = p1 + 1; }

                                var newPortRange = $"{p1}-{p2}";
                                value = value.Substring(0, portStart) + newPortRange + value.Substring(portEnd);
                            }
                        }
                        else
                        {
                            // Add default client_port if missing
                            if (!value.EndsWith(";"))
                                value += ";";
                            value += "client_port=8000-8001";
                        }
                    }

                    SetString(transport, value);
                }
            }
        }

        // Fix Range header - with improved PAUSE and unit parsing
        public static void FixRange(DataElement elem)
        {
            var rangeBlock = elem.find("range");
            if (rangeBlock == null) return;

            var rangeName = rangeBlock.find("h_range_name") as Peach.Core.Dom.String;
            var range = rangeBlock.find("range") as Peach.Core.Dom.String;

            if (rangeName != null)
            {
                SetString(rangeName, "Range:");
            }

            if (range != null)
            {
                var value = GetString(range);
                var requestLine = elem.find("request_line");
                var method = requestLine != null ? GetString(requestLine.find("method")) : "";
                bool isPause = StrIeq(method, "PAUSE");

                if (IsEmpty(value))
                {
                    SetString(range, "npt=0-");
                    return;
                }

                // Extract unit (npt, smpte, clock)
                string unit = "npt";
                string timeValue = value;
                
                if (value.StartsWith("npt="))
                {
                    unit = "npt";
                    timeValue = value.Substring(4);
                }
                else if (value.StartsWith("smpte="))
                {
                    unit = "smpte";
                    timeValue = value.Substring(6);
                }
                else if (value.StartsWith("clock="))
                {
                    unit = "clock";
                    timeValue = value.Substring(6);
                }
                else
                {
                    // Default to npt
                    unit = "npt";
                    if (!value.Contains("="))
                    {
                        timeValue = value;
                    }
                    else
                    {
                        var parts = value.Split('=');
                        if (parts.Length >= 2)
                        {
                            timeValue = parts[1];
                        }
                    }
                }

                timeValue = timeValue.Trim();

                // For PAUSE: ensure single point (start == end)
                if (isPause)
                {
                    string start = "";
                    string end = "";
                    
                    if (timeValue.Contains("-"))
                    {
                        var parts = timeValue.Split('-');
                        start = parts[0].Trim();
                        if (parts.Length > 1) end = parts[1].Trim();
                    }
                    else
                    {
                        start = timeValue;
                    }

                    // Handle "now" special value for npt
                    if (StrIeq(unit, "npt"))
                    {
                        if (StrIeq(start, "now") || StrIeq(end, "now"))
                        {
                            value = "npt=now-now";
                        }
                        else
                        {
                            double startNum;
                            int parseResult = ParseNptSeconds(start, out startNum);
                            if (parseResult == 0)
                            {
                                // Valid number, use it for both start and end
                                value = $"npt={startNum}-{startNum}";
                            }
                            else
                            {
                                // Use start value for both
                                if (IsEmpty(start) && !IsEmpty(end)) start = end;
                                value = $"npt={start}-{start}";
                            }
                        }
                    }
                    else if (StrIeq(unit, "smpte"))
                    {
                        double startNum;
                        if (ParseSmpteSeconds(start, out startNum) == 0)
                        {
                            // Valid SMPTE, use start for both
                            if (IsEmpty(start) && !IsEmpty(end)) start = end;
                            value = $"smpte={start}-{start}";
                        }
                        else
                        {
                            // Invalid, just copy
                            if (IsEmpty(start) && !IsEmpty(end)) start = end;
                            value = $"smpte={start}-{start}";
                        }
                    }
                    else // clock
                    {
                        if (IsEmpty(start) && !IsEmpty(end)) start = end;
                        value = $"clock={start}-{start}";
                    }
                }
                else
                {
                    // For PLAY/RECORD: ensure end > start or open-ended
                    string start = "";
                    string end = "";
                    
                    if (timeValue.Contains("-"))
                    {
                        var parts = timeValue.Split('-');
                        start = parts[0].Trim();
                        if (parts.Length > 1) end = parts[1].Trim();
                    }
                    else
                    {
                        start = timeValue;
                    }

                    if (StrIeq(unit, "npt"))
                    {
                        double startNum, endNum;
                        int parseStart = ParseNptSeconds(start, out startNum);
                        int parseEnd = ParseNptSeconds(end, out endNum);
                        
                        if (parseStart == 0 && parseEnd == 0)
                        {
                            // Both are valid numbers
                            if (endNum <= startNum)
                            {
                                // Make it open-ended
                                value = $"{unit}={start}-";
                            }
                            else
                            {
                                value = $"{unit}={start}-{end}";
                            }
                        }
                        else if (parseStart == 0)
                        {
                            // Only start is valid, keep as open-ended
                            value = $"{unit}={start}-";
                        }
                        else
                        {
                            // Keep original format
                            value = $"{unit}={timeValue}";
                        }
                    }
                    else if (StrIeq(unit, "smpte"))
                    {
                        double startNum, endNum;
                        int parseStart = ParseSmpteSeconds(start, out startNum);
                        int parseEnd = ParseSmpteSeconds(end, out endNum);
                        
                        if (parseStart == 0 && parseEnd == 0)
                        {
                            if (endNum <= startNum)
                            {
                                value = $"{unit}={start}-";
                            }
                            else
                            {
                                value = $"{unit}={start}-{end}";
                            }
                        }
                        else
                        {
                            value = $"{unit}={timeValue}";
                        }
                    }
                    else // clock
                    {
                        // Clock format: keep as-is, can't reliably compare
                        value = $"{unit}={timeValue}";
                    }
                }

                SetString(range, value);
            }
        }

        // Fix Session header
        public static void FixSession(DataElement elem)
        {
            var sessionBlock = elem.find("session");
            if (sessionBlock == null) return;

            var sessionName = sessionBlock.find("session_name") as Peach.Core.Dom.String;
            var sessionId = sessionBlock.find("session_id") as Peach.Core.Dom.String;

            if (sessionName != null)
            {
                SetString(sessionName, "Session:");
            }

            if (sessionId != null)
            {
                var value = GetString(sessionId);
                if (IsEmpty(value))
                {
                    SetString(sessionId, "12345678");
                }
            }
        }

        // Fix Content-Length header
        public static void FixContentLength(DataElement elem)
        {
            var clName = elem.find("h_cl_name") as Peach.Core.Dom.String;
            var cl = elem.find("content_length") as Peach.Core.Dom.String;
            var body = elem.find("body") as Blob;
            if (body == null)
            {
                body = elem.find("msg_body") as Blob;
            }

            if (clName != null)
            {
                SetString(clName, "Content-Length:");
            }

            if (cl != null)
            {
                int length = 0;
                if (body != null)
                {
                    var bodyBytes = body.Bytes();
                    length = bodyBytes != null ? bodyBytes.Length : 0;
                }

                SetString(cl, length.ToString());
            }
        }

        // Fix Request URI (absolute URI) - with Content-Base support
        public static void FixRequestUri(DataElement elem, ref string lastAbsBase)
        {
            var requestLine = elem.find("request_line");
            if (requestLine == null) return;

            var uri = requestLine.find("uri") as Peach.Core.Dom.String;
            if (uri == null) return;

            var uriValue = GetString(uri);
            var method = GetString(requestLine.find("method"));

            // Get Content-Base from packet if available (priority over lastAbsBase)
            var contentBase = GetContentBaseIfAbsFromPacket(elem);

            // Handle "*" URI (only allowed for OPTIONS)
            if (IsStarUri(uriValue))
            {
                if (!StrIeq(method, "OPTIONS"))
                {
                    // Replace with absolute URI
                    var baseUri = contentBase ?? (IsEmpty(lastAbsBase) ? "rtsp://localhost/" : lastAbsBase);
                    SetString(uri, baseUri);
                    uriValue = baseUri;
                }
                else
                {
                    // OPTIONS * is valid, don't update lastAbsBase
                    return;
                }
            }

            // Handle relative URI
            if (!IsAbsoluteUri(uriValue))
            {
                var baseUri = contentBase ?? (IsEmpty(lastAbsBase) ? "rtsp://localhost/" : lastAbsBase);
                
                // Simple URI joining
                if (uriValue.StartsWith("/"))
                {
                    var schemeEnd = baseUri.IndexOf("://");
                    if (schemeEnd >= 0)
                    {
                        var authorityEnd = baseUri.IndexOf("/", schemeEnd + 3);
                        if (authorityEnd >= 0)
                        {
                            baseUri = baseUri.Substring(0, authorityEnd);
                        }
                    }
                    SetString(uri, baseUri + uriValue);
                    uriValue = baseUri + uriValue;
                }
                else
                {
                    // Relative path
                    var lastSlash = baseUri.LastIndexOf("/");
                    if (lastSlash >= 0)
                    {
                        baseUri = baseUri.Substring(0, lastSlash + 1);
                    }
                    SetString(uri, baseUri + uriValue);
                    uriValue = baseUri + uriValue;
                }
            }

            // Update last absolute base
            if (IsAbsoluteUri(uriValue))
            {
                lastAbsBase = uriValue;
            }
        }

        // Fix method-specific headers
        public static void FixOptions(DataElement elem)
        {
            var requestLine = elem.find("request_line");
            if (requestLine != null)
            {
                var method = requestLine.find("method") as Peach.Core.Dom.String;
                if (method != null)
                {
                    SetString(method, "OPTIONS");
                }
            }

            FixCseq(elem);
        }

        public static void FixDescribe(DataElement elem)
        {
            var requestLine = elem.find("request_line");
            if (requestLine != null)
            {
                var method = requestLine.find("method") as Peach.Core.Dom.String;
                if (method != null)
                {
                    SetString(method, "DESCRIBE");
                }
            }

            FixCseq(elem);
            FixAccept(elem);
        }

        public static void FixSetup(DataElement elem)
        {
            var requestLine = elem.find("request_line");
            if (requestLine != null)
            {
                var method = requestLine.find("method") as Peach.Core.Dom.String;
                if (method != null)
                {
                    SetString(method, "SETUP");
                }
            }

            FixCseq(elem);
            FixTransport(elem);
        }

        public static void FixPlay(DataElement elem)
        {
            var requestLine = elem.find("request_line");
            if (requestLine != null)
            {
                var method = requestLine.find("method") as Peach.Core.Dom.String;
                if (method != null)
                {
                    SetString(method, "PLAY");
                }
            }

            FixCseq(elem);
            FixSession(elem);
            FixRange(elem);
        }

        public static void FixPause(DataElement elem)
        {
            var requestLine = elem.find("request_line");
            if (requestLine != null)
            {
                var method = requestLine.find("method") as Peach.Core.Dom.String;
                if (method != null)
                {
                    SetString(method, "PAUSE");
                }
            }

            FixCseq(elem);
            FixSession(elem);
            FixRange(elem);
        }

        public static void FixTeardown(DataElement elem)
        {
            var requestLine = elem.find("request_line");
            if (requestLine != null)
            {
                var method = requestLine.find("method") as Peach.Core.Dom.String;
                if (method != null)
                {
                    SetString(method, "TEARDOWN");
                }
            }

            FixCseq(elem);
            FixSession(elem);
        }

        public static void FixGetParameter(DataElement elem)
        {
            var requestLine = elem.find("request_line");
            if (requestLine != null)
            {
                var method = requestLine.find("method") as Peach.Core.Dom.String;
                if (method != null)
                {
                    SetString(method, "GET_PARAMETER");
                }
            }

            FixCseq(elem);
            FixSession(elem);
            FixContentLength(elem);
        }

        public static void FixSetParameter(DataElement elem)
        {
            var requestLine = elem.find("request_line");
            if (requestLine != null)
            {
                var method = requestLine.find("method") as Peach.Core.Dom.String;
                if (method != null)
                {
                    SetString(method, "SET_PARAMETER");
                }
            }

            FixCseq(elem);
            FixSession(elem);
            FixContentLength(elem);
        }

        // Reset tracking state
        public static void ResetTracking()
        {
            _requestCseqMap.Clear();
            _lastAssignedCseq = 0;
            _nextInterleavedChannel = 0;
        }

        // Main fix function for RTSP packets
        public static void FixRtsp(DataElement elem)
        {
            ResetTracking();

            var requests = elem.find("requests") as Array;
            if (requests == null) return;

            string lastAbsBase = "";

            for (int i = 0; i < requests.Count; i++)
            {
                var request = requests[i];
                var requestUnion = request.find("request_union") as Choice;
                if (requestUnion == null) continue;

                var selected = requestUnion.SelectedElement;
                if (selected == null) continue;

                // Fix request URI first
                FixRequestUri(selected, ref lastAbsBase);

                // Fix method-specific headers
                switch (selected.Name.ToUpper())
                {
                    case "OPTIONS":
                        FixOptions(selected);
                        break;
                    case "DESCRIBE":
                        FixDescribe(selected);
                        break;
                    case "SETUP":
                        FixSetup(selected);
                        break;
                    case "PLAY":
                        FixPlay(selected);
                        break;
                    case "PAUSE":
                        FixPause(selected);
                        break;
                    case "TEARDOWN":
                        FixTeardown(selected);
                        break;
                    case "GET_PARAMETER":
                        FixGetParameter(selected);
                        break;
                    case "SET_PARAMETER":
                        FixSetParameter(selected);
                        break;
                }
            }
        }
    }
}


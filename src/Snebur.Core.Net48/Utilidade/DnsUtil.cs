using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Snebur.Utilidade
{
    public static class DnsUtil
    {
        public static DnsRecord[] GetDnsRecords(string domain, QueryType queryType)
        {
            var dnsRecords = new DNSClient(domain, queryType);
            return dnsRecords.GetRecords();
        }
     
        private class DNSClient
        {
            private const string DNS_SERVER = "8.8.8.8";
            private readonly QueryType queryType;
            private readonly string Domain;
            private int[] Response;

            public DNSClient(string domain, QueryType queryType)
            {
                this.Domain = domain;
                this.queryType = queryType;
            }

            public DnsRecord[] GetRecords()
            {

                using (UdpClient udpClient = new UdpClient(DNS_SERVER, 53))
                {
                    byte[] request = this.CreateRequest();

                    udpClient.Send(request, request.Length);
                    IPEndPoint endpoint = null;
                    byte[] responseBytes = udpClient.Receive(ref endpoint);
                    this.Response = new int[responseBytes.Length];
                    for (int i = 0; i < this.Response.Length; i++)
                    {
                        this.Response[i] = Convert.ToInt32(responseBytes[i]);
                    }
                }
                return this.ParseResponse();
            }

            private byte[] CreateRequest()
            {
                List<byte> requestList = new List<byte>();
                requestList.AddRange(new byte[] { 88, 89, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0 });

                string[] segments = this.Domain.Split('.');
                foreach (string segment in segments)
                {
                    requestList.Add(Convert.ToByte(segment.Length));
                    char[] chars = segment.ToCharArray();
                    foreach (char c in chars)
                        requestList.Add(Convert.ToByte(Convert.ToInt32(c)));
                }

                requestList.AddRange(new byte[] { 0, 0, Convert.ToByte(this.queryType), 0, 1 });

                byte[] request = new byte[requestList.Count];
                for (int i = 0; i < requestList.Count; i++)
                {
                    request[i] = requestList[i];
                }

                return request;
            }

            private DnsRecord[] ParseResponse()
            {

                int status = this.Response[3];
                if (status != 128)
                {
                    return null;
                }

                int nAnswers = this.Response[7];
                if (nAnswers == 0) return null;

                int pos = this.Domain.Length + 18;
                if (this.queryType == QueryType.MX)
                {
                    var dnsRecords = new List<DnsRecord>();
                    while (nAnswers > 0)
                    {
                        int preference = this.Response[pos + 13];
                        pos += 14; //offset
                        string record = this.ParseMXRecord(pos, out pos);
                        if (!String.IsNullOrWhiteSpace(record))
                        {
                            dnsRecords.Add(new DnsRecord() { Preference = preference, Record = record });
                        }
                        nAnswers--;
                    }
                    return dnsRecords.ToArray();
                }

                if (this.queryType == QueryType.A)
                {
                    var dnsRecords = new List<DnsRecord>();
                    while (nAnswers > 0)
                    {
                        pos += 11; //offset
                        string record = this.ParseARecord(ref pos);
                        dnsRecords.Add(new DnsRecord() { Record = record });
                        nAnswers--;
                    }
                    return dnsRecords.ToArray();
                }

                throw new Exception("Query type not supported");

            }

            private string ParseARecord(ref int start)
            {
                StringBuilder sb = new StringBuilder();

                int length = this.Response[start];
                for (int i = start; i < start + length; i++)
                {
                    if (sb.Length > 0) sb.Append(".");
                    sb.Append(this.Response[i + 1]);
                }
                start += length + 1;
                return sb.ToString();
            }

            private string ParseMXRecord(int start, out int pos)
            {
                StringBuilder sb = new StringBuilder();
                int length = this.Response[start];
                while (length > 0)
                {
                    if (length != 192)
                    {
                        if (sb.Length > 0) sb.Append(".");
                        for (int i = start; i < start + length; i++)
                            sb.Append(Convert.ToChar(this.Response[i + 1])); ;
                        start += length + 1;
                        length = this.Response[start];
                    }
                    if (length == 192)
                    {
                        int newPos = this.Response[start + 1];
                        if (sb.Length > 0) sb.Append(".");
                        sb.Append(this.ParseMXRecord(newPos, out newPos));
                        start++;
                        break;
                    }
                }
                pos = start + 1;
                return sb.ToString();
            }

            public bool IsReusable { get { return false; } }
        }

        public class DnsRecord
        {
            public int Preference { get; set; }
            public string Record { get; set; }
        }
    }

    public enum QueryType : byte
    {
        A = 1,
        MX = 15,
        NS = 2,
        CNAME = 5,
        SOA = 6,
        PTR = 12,
        TXT = 16,
        AAAA = 28,
        SRV = 33,
        ANY = 255
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleNameResolver.Base
{
    public enum RRType { A = (short)1 }
    public enum RRClass { IN = (short)1 }

    public class DnsResourceRecord
    {
        public List<string> NameLabels { get; set; }
        public RRType Type { get; set; }
        public RRClass Class { get; set; }
        public int TTL { get; set; }
        public ushort DataLength { get; set; }
        public byte[] Data { get; set; }
    }
}

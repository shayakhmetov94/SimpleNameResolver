using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleNameResolver.Base
{
    public class DnsQuestion
    {
        public List<string> QNameLabels { get; set; }
        public RRType QType { get; set; }
        public RRClass QClass { get; set; }
    }
}

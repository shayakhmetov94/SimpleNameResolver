using SimpleNameResolver.Base.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleNameResolver.Base
{
    class DnsNameResolver
    {
        private int _clientPortNum = 62333;
        private List<IPAddress>  _localAuthoritives;
        private Random _msgIdRndGenerator;

        public DnsNameResolver(int port) {
            _clientPortNum = port;
            _localAuthoritives = FetchLocalAuthoritives();
            _msgIdRndGenerator = new Random();
        }

        public DnsNameResolver() {
            _localAuthoritives = FetchLocalAuthoritives();
            _msgIdRndGenerator = new Random();
        }

        private List<IPAddress> FetchLocalAuthoritives() {
            List<IPAddress> dnsAddresses = new List<IPAddress>(1);
            NetworkInterface[] netInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach(var netInterface in netInterfaces) {
                var netInterfaceIpProps = netInterface.GetIPProperties();
                foreach ( var dnsAddress in netInterfaceIpProps.DnsAddresses )
                    dnsAddresses.Add( dnsAddress );
            }

            return dnsAddresses;
        }

        public List<IPAddress> GetHostIpByName( string domainName ) {
            var query = CreateStandartQuery( domainName );
            query.IsReccursionDesired = true; //TODO: support iterative queries

            var resp  = SendQueryToFirstAvailable( _localAuthoritives, query );
            if ( resp == null )
                return null;

            List<IPAddress> hostAddresses = new List<IPAddress>(1);
            foreach ( var respRr in resp.AnswerRecords )
                if ( respRr.Class == RRClass.IN && respRr.Type == RRType.A ) //TODO: support CNAME rrs
                    hostAddresses.Add( new IPAddress( respRr.Data ) );

            return hostAddresses;
        }

        private DnsMessage CreateStandartQuery(string domainName){
            DnsMessage dnsMsg = DnsMessage.CreateMessage(GenerateMessageId(), OpCode.Query);
            dnsMsg.QueryQuestions.Add( new DnsQuestion() { QClass = RRClass.IN, QType = RRType.A, QNameLabels = GetDomainLabels(domainName) } );

            return dnsMsg;
        }
        
        private List<string> GetDomainLabels(string domainName) {
            return new List<string>( domainName.Split( '.' ) );
        }

        private ushort GenerateMessageId() {
            Random idGenerator = new Random((int)DateTime.Now.ToBinary());
            return (ushort)idGenerator.Next( ushort.MinValue, ushort.MaxValue );
        }

        private DnsMessage SendQueryToFirstAvailable( List<IPAddress> dnsAddresses, DnsMessage query ) {
            using ( UdpClient client = new UdpClient( _clientPortNum, AddressFamily.InterNetwork ) ) {
                var dgram = DnsMessageParser.GetBytes(query);
                foreach ( var address in dnsAddresses ) {
                    IPEndPoint dnsEp = new IPEndPoint( address, 53 );
                    client.Send( dgram, dgram.Length, dnsEp );
                    var responce = DnsMessageParser.Parse(client.Receive(ref dnsEp));
                    if ( responce.IsResponse && responce.AnswerRecords.Count > 0 )
                        return responce;
                }
            }

            return null;
        }


    }
}

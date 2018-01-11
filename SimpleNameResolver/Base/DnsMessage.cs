using SimpleNameResolver.Base.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleNameResolver.Base
{
    public enum OpCode { Query = 0x0, IQuery = 0x1, Status = 0x2 }
    public enum RCode { NoError = 0x0, FormatError = 0x1, ServerError = 0x2, NameError = 0x3, NotImplemented = 0x4, Refused = 0x5 }

    public class DnsMessage
    {
        public DnsMessage() { }

        public ushort Id { get; set; }
        public BitField Flags { get; set; }

        public OpCode OpCode {
            get {
                byte opCode = (byte)(Flags[1] ? 0x01 : 0x00); //zero bit
                for ( int i = 2; i < 5; i++ )
                    if ( Flags[i] )
                        opCode |= (byte)(0x01 << i);

                return (OpCode)opCode;
            }
        }

        public bool IsResponse {
            get {
                return Flags[0];
            }

            set {
                Flags[0] = value;
            }
        }

        public bool IsAuthorativeAnswer {
            get {
                return Flags[5];
            }

            set {
                Flags[5] = value;
            }
        }

        public bool IsTruncated {
            get {
                return Flags[6];
            }

            set {
                Flags[6] = value;
            }
        }

        public bool IsReccursionDesired {
            get {
                return Flags[7];
            }

            set {
                Flags[7] = value;
            }
        }

        public bool IsReccursionAvailable {
            get {
                return Flags[8];
            }

            set {
                Flags[8] = value;
            }
        }

        public bool IsNonAuthenticatedDataAcceptable {
            get {
                return Flags[11];
            }

            set {
                Flags[11] = value;
            }
        }

        public RCode ResponseCode { get; private set; }

        public List<DnsQuestion> QueryQuestions { get; private set; } = new List<DnsQuestion>();
        public List<DnsResourceRecord> AnswerRecords { get; private set; } = new List<DnsResourceRecord>();
        public List<DnsResourceRecord> AuthorityRecords { get; private set; } = new List<DnsResourceRecord>();
        public List<DnsResourceRecord> AdditionalRecords { get; private set; } = new List<DnsResourceRecord>();

        public static DnsMessage CreateMessage( ushort msgId, OpCode opCode ) {
            byte flagsByte =  (byte)((byte)opCode << 7);

            DnsMessage msg = new DnsMessage()
            {
                Id = msgId,
                Flags = new BitField(new byte[] { flagsByte, 0 }, 0, 16)
            };

            return msg;
        }

    }
}

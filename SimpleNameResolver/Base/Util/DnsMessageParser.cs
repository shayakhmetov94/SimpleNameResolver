using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleNameResolver.Base.Util
{
    public static class DnsMessageParser {
        public static DnsMessage Parse( byte[] msg ) {
            DnsMessage dnsMsg = new DnsMessage();
            int offset = 0;
            dnsMsg.Id = ReadUShort( msg, ref offset );
            dnsMsg.Flags = new BitField( new byte[] { msg[offset], msg[offset + 1] }, 0, 16 );
            offset += 2;
            int questionsCount  = ReadUShort(msg, ref offset),
                responseRrs     = ReadUShort(msg, ref offset),
                authorityRrs    = ReadUShort(msg, ref offset),
                additionalRrs   = ReadUShort(msg, ref offset);

            for ( int i = 0; i < questionsCount; i++ )
                dnsMsg.QueryQuestions.Add( ParseQuestion( msg, ref offset ) );

            for ( int i = 0; i < responseRrs; i++ )
                dnsMsg.AnswerRecords.Add( ParseRr( msg, ref offset ) );

            for ( int i = 0; i < authorityRrs; i++ )
                dnsMsg.AuthorityRecords.Add( ParseRr( msg, ref offset ) );

            for ( int i = 0; i < additionalRrs; i++ )
                dnsMsg.AdditionalRecords.Add( ParseRr( msg, ref offset ) );

            return dnsMsg;
        }

        private static ushort ReadUShort( byte[] buf, ref int start ) {
            ushort val = (ushort)((buf[start] << 8) | buf[start + 1]);
            start += 2;
            return val;
        }


        private static List<string> ReadLabels( byte[] buf, ref int start ) {
            List<string> labels = new List<string>(1);
            while (buf[start] != '\0') {
                byte len = buf[start];
                start++;
                string label = Encoding.ASCII.GetString(buf, start, len);
                labels.Add( label );
                start += len;
            }

            start++; // skip \0
            return labels;
        }

        public static string ReadCString( byte[] buf, ref int start ) {
            int pos = start;
            for ( ; buf[pos] != '\0'; pos++ ) ;
            int bytesToCopy = pos - start + 1;
            byte[] res = new byte[bytesToCopy];
            Buffer.BlockCopy( buf, start, res, 0, bytesToCopy );
            start += bytesToCopy;
            return Encoding.ASCII.GetString( res );
        }

        private static int ReadInt( byte[] buf, ref int start ) {
            int val = ((((buf[start] << 32) | buf[start + 1] << 24) | buf[start + 2] << 16) | buf[start + 3]); //???
            start += 4;
            return val;
        }

        private static DnsQuestion ParseQuestion( byte[] buf, ref int start ) {
            List<string> labels   = ReadLabels(buf, ref start);
            ushort qType  = ReadUShort(buf, ref start);
            ushort qClass = ReadUShort(buf, ref start);

            return new DnsQuestion()
            {
                QNameLabels = labels,
                QClass = (RRClass)qClass,
                QType = (RRType)qType
            };
        }

        private static DnsResourceRecord ParseRr( byte[] buf, ref int start ) {
            List<string> labels = null;
            if ( IsDomainNameOffset( buf[start] ) ) {
                int domainNameOffset = ReadDomainNameOffset(buf, ref start);
                labels = ReadLabels( buf, ref domainNameOffset );
            } else
                labels = ReadLabels( buf, ref start );

            ushort rType  = ReadUShort(buf, ref start);
            ushort rClass = ReadUShort(buf, ref start);
            int    ttl    = ReadInt(buf, ref start);

            ushort rdataLen = ReadUShort(buf, ref start);
            byte[] rdata    = new byte[rdataLen];
            Buffer.BlockCopy( buf, start, rdata, 0, rdataLen );

            start += rdataLen;

            return new DnsResourceRecord()
            {
                NameLabels = labels,
                Type = (RRType)rType,
                Class = (RRClass)rClass,
                TTL = ttl,
                DataLength = rdataLen,
                Data = rdata
            };
        }

        private static bool IsDomainNameOffset( byte b ) {
            return (b & (1 << 7)) != 0 && (b & (1 << 6)) != 0;
        }

        private static int ReadDomainNameOffset( byte[] buf, ref int start ) {
            int offset = ((byte)(buf[start] & 0x3f) << 8) | buf[start + 1];// till better times
            start += 2;
            return offset;
        }

        public static byte[] GetBytes(DnsMessage msg) {
            MemoryStream bytesBuilder = new MemoryStream(16);
            WriteUShort( bytesBuilder, msg.Id );
            bytesBuilder.Write( msg.Flags.ToByteArray(), 0, 2 );
            WriteUShort( bytesBuilder, (ushort)msg.QueryQuestions.Count ); 
            WriteUShort( bytesBuilder, (ushort)msg.AnswerRecords.Count ); 
            WriteUShort( bytesBuilder, (ushort)msg.AuthorityRecords.Count );
            WriteUShort( bytesBuilder, (ushort)msg.AdditionalRecords.Count );

            for ( int i = 0; i < msg.QueryQuestions.Count; i++ )
                WriteQuestion( bytesBuilder, msg.QueryQuestions[i] );

            for ( int i = 0; i < msg.AnswerRecords.Count; i++ )
                WriteRr( bytesBuilder, msg.AnswerRecords[i] );

            for ( int i = 0; i < msg.AuthorityRecords.Count; i++ )
                WriteRr( bytesBuilder, msg.AuthorityRecords[i] );

            for ( int i = 0; i < msg.AdditionalRecords.Count; i++ )
                WriteRr( bytesBuilder, msg.AdditionalRecords[i] );

            return bytesBuilder.ToArray();
        }

        private static void WriteUShort(MemoryStream ms, ushort val) {
            var bytes = BitConverter.GetBytes( val );
            Array.Reverse( bytes );
            ms.Write( bytes, 0, 2 );
        }

        private static void WriteInt( MemoryStream ms, int val ) {
            var bytes = BitConverter.GetBytes( val );
            Array.Reverse( bytes );
            ms.Write( bytes, 0, 4 );
        }

        private static void WriteQuestion( MemoryStream ms, DnsQuestion question ) {
            WriteLabels( ms, question.QNameLabels );
            WriteUShort( ms, (ushort)question.QType );
            WriteUShort( ms, (ushort)question.QClass );
        }

        private static void WriteLabels( MemoryStream ms, List<string> labels ) {
            for(int i = 0; i < labels.Count; i++) {
                byte len = (byte)labels[i].Length;

                ms.WriteByte( len );
                ms.Write( Encoding.ASCII.GetBytes( labels[i] ), 0, len);
            }

            ms.WriteByte( (byte)'\0' );
        }

        public static void WriteCString( MemoryStream ms, string str) {
            ms.Write( Encoding.ASCII.GetBytes( str ), 0, str.Length );
        }

        private static void WriteRr( MemoryStream ms, DnsResourceRecord rr ) {
            WriteLabels( ms, rr.NameLabels );
            WriteUShort( ms, (ushort)rr.Type );
            WriteUShort( ms, (ushort)rr.Class );
            WriteInt( ms, rr.TTL );
            WriteUShort( ms, rr.DataLength );
            ms.Write( rr.Data, 0, rr.DataLength );
        }

    }
}

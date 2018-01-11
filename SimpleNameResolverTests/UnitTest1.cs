using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleNameResolver.Base;
using SimpleNameResolver.Base.Util;
using System.Linq;
using System.Text;
using System.IO;

namespace SimpleNameResolverTests
{
    [TestClass]
    public class ParserTests
    {
        private static string ATypeQueryTestData ="vJYBAAABAAAAAAAAAzJjaAJoawAAAQAB";
        private static string ATypeQueryResponseTestData ="07GBgAABAAQACgAFA3YxMAp2b3J0ZXgtd2luBGRhdGEJbWljcm9zb2Z0A2NvbQAAAQABwAwABQABAAAFIgAuB3YxMC13aW4Gdm9ydGV4BGRhdGEJbWljcm9zb2Z0A2NvbQZha2FkbnMDbmV0AMA/AAUAAQAAAOgABgNnZW/AR8B5AAUAAQAAAOUABgNkYjXAR8CLAAEAAQAAAOgABChN4vrAYQACAAEAAAQjABQHYTEzLTEzMAZha2FkbnMDb3JnAMBhAAIAAQAABCMACQZhMS0xMjjAYcBhAAIAAQAABCMACQZhNS0xMzDAtcBhAAIAAQAABCMACQZhOS0xMjjAYcBhAAIAAQAABCMACQZhMy0xMjnAYcBhAAIAAQAABCMACQZhNy0xMzHAYcBhAAIAAQAABCMACgdhMjgtMTI5wLXAYQACAAEAAAQjAAoHYTEyLTEzMcC1wGEAAgABAAAEIwAKB2ExOC0xMjjAtcBhAAIAAQAABCMACgdhMTEtMTI5wGHAzQABAAEAAAQjAATBbFiAwQwAAQABAAAEIwAEYAcxgcEhAAEAAQAABCMABBc9x4PA9wABAAEAAAQjAAS4VfiAwXgAAQABAAAEIwAEVDWLgQ==";

        [TestMethod]
        public void DnsQuestionMsgParserTest() {
            DnsMessage msg = DnsMessageParser.Parse(Convert.FromBase64String(ATypeQueryTestData));
            Assert.IsNotNull( msg );

            Assert.AreEqual( 0xbc96, msg.Id );
            Assert.IsTrue( msg.Flags.Length == 16 );
            Assert.IsTrue( msg.OpCode == OpCode.Query, "OpCode should be a query" );
            Assert.IsTrue( !msg.IsResponse, "Should be a query" );
            Assert.IsTrue( !msg.IsTruncated, "Should not be truncated" );
            Assert.IsTrue( msg.IsReccursionDesired, "Should query recursively" );
            Assert.IsTrue( !msg.IsNonAuthenticatedDataAcceptable, "Non authenticated data should not be accepted" );
            Assert.IsTrue( msg.QueryQuestions.Count == 1, "Should contain 1 question" );
        }

        [TestMethod]
        public void DnsQuestionMsgToBytesTest() {
            var msgBytes = Convert.FromBase64String(ATypeQueryTestData);
            DnsMessage msg = DnsMessageParser.Parse(msgBytes);
            var converted = DnsMessageParser.GetBytes(msg);
            byte[] msgNumsPart = new byte[12];
            byte[] msgByteNumsPart = new byte[12];

            Buffer.BlockCopy( converted, 0, msgNumsPart, 0, 12 );
            Buffer.BlockCopy( msgBytes, 0, msgByteNumsPart, 0, 12 );

            Assert.IsTrue( msgNumsPart.SequenceEqual(msgByteNumsPart), $"{string.Join(", ", msgNumsPart)} does not equal {string.Join( ", ", msgByteNumsPart )}" );

            Assert.AreEqual<int>( msgBytes.Length, converted.Length, "Lengths are not equal" );

            Assert.IsTrue( msgBytes.SequenceEqual( converted ), $"{string.Join( ", ", msgBytes )} does not equal {string.Join( ", ", converted )}" );
        }

        /// <summary>
        /// Won't pass, because message compression does not supported
        /// </summary>
        [TestMethod]
        public void DnsQuestionResponseParseTest() {
            var msgBytes = Convert.FromBase64String(ATypeQueryResponseTestData);
            DnsMessage msg = DnsMessageParser.Parse(msgBytes);
            var converted = DnsMessageParser.GetBytes(msg);
            byte[] msgNumsPart = new byte[12];
            byte[] msgByteNumsPart = new byte[12];

            Buffer.BlockCopy( converted, 0, msgNumsPart, 0, 12 );
            Buffer.BlockCopy( msgBytes, 0, msgByteNumsPart, 0, 12 );

            Assert.IsTrue( msgNumsPart.SequenceEqual( msgByteNumsPart ), $"{string.Join( ", ", msgNumsPart )} does not equal {string.Join( ", ", msgByteNumsPart )}" );

            Assert.AreEqual<int>( msgBytes.Length, converted.Length, "Lengths are not equal" );

            Assert.IsTrue( msgBytes.SequenceEqual( converted ) );
        }

        [TestMethod]
        public void ReadCStringTest() {
            string testStr = "123141231231\0";
            byte[] testStrAsBytes = Encoding.ASCII.GetBytes(testStr);
            int pos = 0;

            var readStr = DnsMessageParser.ReadCString( testStrAsBytes, ref pos );

            MemoryStream ms = new MemoryStream(16);
            DnsMessageParser.WriteCString(ms, readStr);

            var writtenStr = Encoding.ASCII.GetString( ms.ToArray() );

            Assert.IsTrue( writtenStr == readStr, $"{writtenStr} != {readStr}" );
        }


    }
}

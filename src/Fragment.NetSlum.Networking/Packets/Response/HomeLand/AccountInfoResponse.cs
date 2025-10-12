using Fragment.NetSlum.Core.Buffers;
using Fragment.NetSlum.Core.Extensions;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using System;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Response.HomeLand
{
    public class AccountInfoResponse : BaseResponse
    {
        private OpCodes _responseCode;

        public override FragmentMessage Build()
        {
            byte error = 0x00; //0x00 for no error
            int acctId = 0x1; //Account ID

            //byte[] unk1 = new byte[16]; //Unknown
            int test1 = 0x00;
            int test2 = 0x00;
            int test3 = 0x00;
            int test4 = 0x00;

            byte unk2 = 0x00; //Unknown
            byte msgType = 0x00;
            int msg1Len = 0x0;
            //string msg1 = "Welcome to HomeLand!";
            char msg1 = 'A';
            int terminate = 0x0;
            int msg2Len = 0x0;
            //string msg2 = "test";
            char msg2 = 'B';
            //var writer = new MemoryWriter(31 + msg1Len + msg2Len);
            var writer = new MemoryWriter(40);

            writer.Write(error);
            writer.Write(acctId);
            //writer.Write(unk1.AsMemory());
            writer.Write(test1);
            writer.Write(test2);
            writer.Write(test3);
            writer.Write(test4);
            writer.Write(unk2);
            writer.Write(msgType);
            writer.Write(msg1Len);
            //writer.Write(System.Text.Encoding.UTF8.GetBytes(msg1).AsSpan());
            //writer.Write(msg1);
            writer.Write(terminate);
            writer.Write(msg2Len);
            //writer.Write(System.Text.Encoding.UTF8.GetBytes(msg2).AsSpan());
            //writer.Write(msg2);
            writer.Write(terminate);

            return new FragmentMessage
            {
                MessageType = MessageType.Data,
                DataPacketType = OpCodes.AccountInfo,
                Data = writer.Buffer,
            };
        }
    }
}

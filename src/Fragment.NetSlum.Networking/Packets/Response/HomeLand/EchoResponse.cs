using Fragment.NetSlum.Core.Buffers;
using Fragment.NetSlum.Core.Extensions;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using System;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Response.HomeLand
{
    public class EchoResponse : BaseResponse
    {
        private int _accountId = 0;

        public EchoResponse SetAccountId(int id)
        {
            _accountId = id;
            return this;
        }

        public override FragmentMessage Build()
        {
            //byte error = 0x00; //0x00 for no error
            byte unk1 = 0x00;
            uint unk2 = 0x00000000;
            ushort unk3 = 0x0000;

            var writer = new MemoryWriter(7);
            writer.Write(unk1);
            writer.Write(unk2);
            writer.Write(unk3);

            return new FragmentMessage
            {
                MessageType = MessageType.Data,
                DataPacketType = OpCodes.Echo,
                Data = writer.Buffer,
            };
        }
    }
}

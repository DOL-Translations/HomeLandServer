using Fragment.NetSlum.Core.Buffers;
using Fragment.NetSlum.Core.Extensions;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using System;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Response.HomeLand
{
    public class HL2009Response : BaseResponse
    {
        private OpCodes _responseCode;

        public HL2009Response SetStatusCode(OpCodes responseCode)
        {
            _responseCode = responseCode;
            return this;
        }
        public override FragmentMessage Build()
        {
            byte first = 0x01;
            uint second = 0x00000001;
            ushort third = 0x0001;

            var writer = new MemoryWriter(8);
            writer.Write(first);
            writer.Write(second);
            writer.Write(third);

            return new FragmentMessage
            {
                MessageType = MessageType.Data,
                DataPacketType = _responseCode,
                Data = writer.Buffer,
            };
        }
    }
}

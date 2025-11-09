using Fragment.NetSlum.Core.Buffers;
using Fragment.NetSlum.Core.Extensions;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using System;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Response.HomeLand
{
    public class HL2016Response : BaseResponse
    {
        private readonly byte _value;

        public HL2016Response(byte value = 0x00)
        {
            _value = value;
        }

        public override FragmentMessage Build()
        {
            var writer = new MemoryWriter(1);
            writer.Write(_value);

            return new FragmentMessage
            {
                MessageType = MessageType.Data,
                DataPacketType = OpCodes.HL2016Response,
                Data = writer.Buffer,
            };
        }
    }
}

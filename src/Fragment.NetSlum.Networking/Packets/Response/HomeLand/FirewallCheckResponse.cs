using Fragment.NetSlum.Core.Buffers;
using Fragment.NetSlum.Core.Extensions;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using System;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Response.HomeLand
{
    public class FirewallCheckResponse : BaseResponse
    {
        private readonly byte _result;

        public FirewallCheckResponse(byte result = 0x00)
        {
            _result = result;
        }

        public override FragmentMessage Build()
        {
            var writer = new MemoryWriter(1);
            writer.Write(_result);

            return new FragmentMessage
            {
                MessageType = MessageType.Data,
                DataPacketType = OpCodes.FirewallCheck,
                Data = writer.Buffer,
            };
        }
    }
}

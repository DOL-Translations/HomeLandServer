using Fragment.NetSlum.Core.Buffers;
using Fragment.NetSlum.Core.Extensions;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using System;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Response.HomeLand
{
    public class GetMatchingServerResponse : BaseResponse
    {
        private readonly byte _result;

        public GetMatchingServerResponse(byte result = 0x00)
        {
            _result = result;
        }

        public override FragmentMessage Build()
        {
            uint ipAddrs = 0x00000000;
            ushort port = 0x3081;

            var writer = new MemoryWriter(7);
            writer.Write(_result);
            writer.Write(ipAddrs);
            writer.Write(port);

            return new FragmentMessage
            {
                MessageType = MessageType.Data,
                DataPacketType = OpCodes.GetMatchingServer,
                Data = writer.Buffer,
            };
        }
    }
}

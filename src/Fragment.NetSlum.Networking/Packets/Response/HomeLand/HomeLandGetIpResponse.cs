using Fragment.NetSlum.Core.Buffers;
using Fragment.NetSlum.Core.Extensions;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using System;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Response.HomeLand
{
    public class HomeLandGetIpResponse : BaseResponse
    {
        private uint _ipAddress = 0;

        public HomeLandGetIpResponse SetIpAddress(uint ipAddress)
        {
            _ipAddress = ipAddress;
            return this;
        }

        public override FragmentMessage Build()
        {
            byte error = 0x00; //0x00 for no error
            //if (_ipAddress == 0) { error = 0x17; }
            var writer = new MemoryWriter(8);
            writer.Write(error);
            writer.Write(_ipAddress);

            return new FragmentMessage
            {
                MessageType = MessageType.Data,
                DataPacketType = OpCodes.HomeLandGetIp,
                Data = writer.Buffer,
            };
        }
    }
}

using System.Buffers.Binary;
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

        private byte _result;
        
        public HomeLandGetIpResponse SetResult(byte result)
        {
            _result = result;
            return this;
        }
        
        public HomeLandGetIpResponse SetIpAddress(uint ipAddress)
        {
            _ipAddress = ipAddress;
            return this;
        }

        public override FragmentMessage Build()
        {
            Console.WriteLine($"_ipAddress = 0x{_ipAddress:X8}");
            Span<byte> ipOut = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(ipOut, _ipAddress);
            
            var writer = new MemoryWriter(5);
            writer.Write(_result);
            writer.Write(ipOut);

            return new FragmentMessage
            {
                MessageType = MessageType.Data,
                DataPacketType = OpCodes.HomeLandGetIp,
                Data = writer.Buffer,
            };
        }
    }
}

using System;
using Fragment.NetSlum.Core.Buffers;
using Fragment.NetSlum.Core.Extensions;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Response.HomeLand
{
    public class HomeLandGetPendingWithdrawsResponse : BaseResponse
    {
        public override FragmentMessage Build()
        {
            byte result = 0x00; //0x00 for no error
            byte count  = 0x00;
            uint dummy  = 0x00000000;
            
            var writer = new MemoryWriter(6);
            writer.Write(result);
            writer.Write(count);
            writer.Write(dummy);

            return new FragmentMessage
            {
                MessageType = MessageType.Data,
                DataPacketType = OpCodes.HomeLandGetPendingWithdraws,
                Data = writer.Buffer,
            };
        }
    }
}

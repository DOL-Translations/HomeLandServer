using Fragment.NetSlum.Core.Buffers;
using Fragment.NetSlum.Core.Extensions;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using System;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Response.HomeLand
{
    public class HomeLandUpdateCommentResponse : BaseResponse
    {
        private byte _result = 0x00;

        public HomeLandUpdateCommentResponse SetResult(byte result)
        {
            _result = result;
            return this;
        }

        public override FragmentMessage Build()
        {
            var writer = new MemoryWriter(1);
            writer.Write(_result);

            return new FragmentMessage
            {
                MessageType = MessageType.Data,
                DataPacketType = OpCodes.HomeLandUpdateComment,
                Data = writer.Buffer,
            };
        }
    }
}

using Fragment.NetSlum.Core.Buffers;
using Fragment.NetSlum.Core.Extensions;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using System;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Response.HomeLand
{
    public class HomeLandSearchResponse : BaseResponse
    {
        private OpCodes _responseCode;

        public override FragmentMessage Build()
        {
            byte error = 0x00; //0x00 for no error
            byte resultCount = 0x1; //no results
            uint unk = 0x00; //unknown
            var writer = new MemoryWriter(1);
            writer.Write(error);

            return new FragmentMessage
            {
                MessageType = MessageType.Data,
                DataPacketType = OpCodes.HomeLandSearchResultCnt,
                Data = writer.Buffer,
            };
        }
    }
}

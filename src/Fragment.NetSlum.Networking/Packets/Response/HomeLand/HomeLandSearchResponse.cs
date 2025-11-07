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
        private byte _searchResultCnt = 0;

        public HomeLandSearchResponse SetResultCnt(byte searchResultCnt)
        {
            _searchResultCnt = searchResultCnt;
            return this;
        }
        
        private uint _totalCount = 0;
        
        public HomeLandSearchResponse SetTotalCount(uint totalCount)
        {
            _totalCount = totalCount;
            return this;
        }

        public override FragmentMessage Build()
        {
            byte result = 0x00; // 0x00 OK
            var writer = new MemoryWriter(6);
            writer.Write(result);
            writer.Write(_searchResultCnt);
            writer.Write(_totalCount);

            return new FragmentMessage
            {
                MessageType = MessageType.Data,
                DataPacketType = OpCodes.HomeLandSearchResultCnt,
                Data = writer.Buffer,
            };
        }
    }
}

using Fragment.NetSlum.Core.Buffers;
using Fragment.NetSlum.Core.Extensions;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using System;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Response.HomeLand
{
    public class HomeLandSearchResultsResponse : BaseResponse
    {
        private OpCodes _responseCode;

        private uint _homelandId = 1; //placeholder

        public void SetHomeLandId(uint homelandId)
        {
            _homelandId = homelandId;
        }

        public override FragmentMessage Build()
        {
            //placeholder
            uint homelandId = _homelandId;
            uint ip = 0x00;
            uint nameLen = 12;
            string name = "Test server"; //Max 12
            ushort unk = 0;
            ushort location = 2936; //North America
            uint time = 0; //Elapsed?
            uint passwordLen = 0;
            string password = ""; //Max 16
            uint commentLen = 23;
            string comment = "this is a test comment"; //Max 256
            sbyte registeredPlayerCnt = 0;
            sbyte maxPlayerCnt = 0;
            uint clearCnt = 0;
            byte isMostRecent = 0;
            ushort latency = 0;

            var writer = new MemoryWriter(37 + 12 + 0 + 23);
            writer.Write(homelandId);
            writer.Write(ip);
            writer.Write(nameLen);
            writer.Write(name.ToShiftJis());
            writer.Write(unk);
            writer.Write(location);
            writer.Write(time);
            writer.Write(passwordLen);
            writer.Write(password.ToShiftJis());
            writer.Write(commentLen);
            writer.Write(comment.ToShiftJis());
            writer.Write(registeredPlayerCnt);
            writer.Write(maxPlayerCnt);
            writer.Write(clearCnt);
            writer.Write(isMostRecent);
            writer.Write(latency);

            return new FragmentMessage
            {
                MessageType = MessageType.Data,
                DataPacketType = OpCodes.HomeLandSearchResult,
                Data = writer.Buffer,
            };
        }
    }
}

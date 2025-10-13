using Fragment.NetSlum.Core.Buffers;
using Fragment.NetSlum.Core.Extensions;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using Fragment.NetSlum.Persistence;
using Fragment.NetSlum.Persistence.Entities;
using System;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Response.HomeLand
{
    public class HomeLandSearchResultsResponse : BaseResponse
    {
        private HomeLandEntity _homeland;

        public HomeLandSearchResultsResponse SetHomeLand(HomeLandEntity homeland)
        {
            _homeland = homeland;
            return this;
        }

        public override FragmentMessage Build()
        {
            uint homelandId = _homeland.HomeLandId;
            uint ip = _homeland.LocalIp;
            uint nameLen = (uint)_homeland.HomeLandName.Length;
            string name = _homeland.HomeLandName; //Max 16
            ushort unk = 0;
            ushort location = _homeland.Location; //2936; //North America
            uint time = _homeland.Time; //Elapsed?
            uint passwordLen = (uint)_homeland.Password.Length;
            string password = _homeland.Password; //Max 16
            uint commentLen = (uint)_homeland.Comment.Length;
            string comment = _homeland.Comment; //Max 256
            sbyte registeredPlayerCnt = _homeland.RegisteredPlayerCnt;
            sbyte maxPlayerCnt = _homeland.MaxPlayerCnt;
            uint clearCnt = _homeland.ClearCnt;
            byte isMostRecent = _homeland.IsMostRecent;
            ushort latency = _homeland.Latency;

            var writer = new MemoryWriter(40 + (int)nameLen + (int)passwordLen + (int)commentLen);
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

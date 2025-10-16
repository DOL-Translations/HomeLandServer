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
        private bool _isOverseas;

        public HomeLandSearchResultsResponse(HomeLandEntity homeland, bool isOverseas)
        {
            _homeland = homeland;
            _isOverseas = isOverseas;
        }

        public override FragmentMessage Build()
        {
            byte terminator = 0x00; //End of list

            uint homelandId = _homeland.HomeLandId;
            uint ip = _homeland.LocalIp;
            string name = _homeland.HomeLandName; //Max 16
            uint nameLen = (uint)name.ToShiftJis().Length;
            ushort unk = 0;
            ushort location = _homeland.Location; //2936; //North America
            uint time = _homeland.Time; //Elapsed?
            string password = _homeland.Password; //Max 16
            uint passwordLen = (uint)password.ToShiftJis().Length + 1;
            string comment = _homeland.Comment; //Max 256
            uint commentLen = (uint)comment.ToShiftJis().Length + 1;
            sbyte registeredPlayerCnt = _homeland.RegisteredPlayerCnt;
            sbyte maxPlayerCnt = _homeland.MaxPlayerCnt;
            uint clearCnt = _homeland.ClearCnt;
            byte isMostRecent = _homeland.IsMostRecent;
            ushort latency = _homeland.Latency;

            if (_isOverseas)
            {
                //Before: Asia, Africa, Oceania, North America, Middle East, Antarctica, South America, Europe, Other
                //After: Africa, Antarctica, Asia, Europe, Middle East, North America, Oceania, South America, Other
                ushort[] locationMapping = new ushort[] { 2, 0, 6, 5, 4, 1, 7, 3, 8 };
                switch (location)
                {
                    case 5000:
                        break;
                    case <= 2932:
                        location = 9; //Asia (Japan)
                        break;
                    default:
                        location = locationMapping[(location - 2933)];
                        break;
                }
            }

            var writer = new MemoryWriter(328/*40 + (int)nameLen + (int)passwordLen + (int)commentLen*/);
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
            writer.Write(terminator);
            writer.Write(terminator);
            writer.Write(registeredPlayerCnt);
            writer.Write(maxPlayerCnt);
            writer.Write(clearCnt);
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

using System;
using System.Buffers.Binary;
using Fragment.NetSlum.Core.Buffers;
using Fragment.NetSlum.Core.Extensions;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using Fragment.NetSlum.Persistence;
using Fragment.NetSlum.Persistence.Entities;
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
          /* Response_HomelandSearchResult //OPCODE_HOMELAND_SEARCH_RESULT
          {
            uint HomelandID;
            uint ExternalIP;
            uint NameLength;
            char Name[NameLength]; //Max 12
            ushort Unk;
            ushort Location;
            uint Time; //Elapsed?
            uint PasswordLength;
            char Password[PasswordLength]; //Max 16
            uint CommentLength;
            char Comment[CommentLength]; //Max 256
            byte RegisteredPlayerCount; 
            byte MaxPlayerCount;
            uint ClearCount;
            byte IsMostRecent;
            ushort Latency;
          } */
            
            uint timeValue = 0;
            
            if (_homeland.Status == 0)
            {
                uint elapsed = (uint)(DateTime.UtcNow - _homeland.LastUpdate).TotalMinutes;
                uint countdownMinutes = _homeland.Countdown * 10u;
                countdownMinutes = (elapsed >= countdownMinutes) ? 0 : (countdownMinutes - elapsed);
                
                timeValue = 0xFFFFFF00u | (0xFF - (countdownMinutes & 0xFF));
            }
            else
            {
                // TODO: needs runtime calculation or correct information from game server
                // timeValue = (uint)(DateTime.UtcNow - _homeland.CreatedAt).TotalMinutes;
            }
            
            uint homelandId           = _homeland.HomeLandId;
            uint ip                   = _homeland.LocalIp;
            byte[] name               = System.Text.Encoding.GetEncoding("shift_jis").GetBytes(_homeland.HomeLandName + "\0");
            uint nameLen              = (uint) name.Length;
            ushort unk                = 0x0000;
            ushort location           = _homeland.Location;
            uint time                 = timeValue;
            byte[] password           = System.Text.Encoding.GetEncoding("shift_jis").GetBytes(_homeland.Password + "\0");
            uint passwordLen          = (uint) password.Length;
            byte[] comment            = System.Text.Encoding.GetEncoding("shift_jis").GetBytes(_homeland.Comment + "\0");
            uint commentLen           = (uint) comment.Length;
            byte registeredPlayerCnt  = _homeland.RegisteredPlayerCnt;
            byte maxPlayerCnt         = _homeland.MaxPlayerCnt;
            uint clearCnt             = _homeland.ClearCnt;
            byte isMostRecent         = 0x00;
            ushort latency            = _homeland.Latency;

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
            
            Span<byte> ipOut = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(ipOut, ip);
            
            int totalSize = 37 + (int) nameLen + (int) passwordLen + (int) commentLen;

            var writer = new MemoryWriter(totalSize);
            writer.Write(homelandId);
            writer.Write(ipOut);
            writer.Write(nameLen);
            writer.Write(name.AsSpan());
            writer.Write(unk);
            writer.Write(location);
            writer.Write(time);
            writer.Write(passwordLen);
            writer.Write(password.AsSpan());
            writer.Write(commentLen);
            writer.Write(comment.AsSpan());
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

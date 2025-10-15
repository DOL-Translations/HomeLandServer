using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fragment.NetSlum.Core.Buffers;
using Fragment.NetSlum.Core.Extensions;
using Fragment.NetSlum.Networking.Attributes;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using Fragment.NetSlum.Networking.Packets.Response.HomeLand;
using Fragment.NetSlum.Networking.Sessions;
using Fragment.NetSlum.Persistence;
using Fragment.NetSlum.Persistence.Entities;
using Fragment.NetSlum.TcpServer.Extensions;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Request.HomeLand;

[FragmentPacket(MessageType.Data, OpCodes.HomeLandSearch)]
public class HomeLandSearchRequest : BaseRequest
{
    private readonly FragmentContext _database;
    
    public HomeLandSearchRequest(FragmentContext database)
    {
        _database = database;
    }

    public override ValueTask<ICollection<FragmentMessage>> GetResponse(FragmentTcpSession session, FragmentMessage request)
    {
        /*
        Request_HomelandSearch //OPCODE_HOMELAND_SEARCH
        {
	        ushort Location;
	        HomelandSearchDistance Distance;
	        char Name[]; //null terminated
	        HomelandSearchPlayers Players;
	        HomelandSearchTime Time;
	        byte HasPassword;
	        HomelandSearchOrder Order;
        }
        */

        var reader = new SpanReader(request.Data.Span);

        ushort location = reader.ReadUInt16();
        byte distance = reader.ReadByte();
        string name = reader.ReadString(out _).ToShiftJisString();
        byte players = reader.ReadByte();
        byte time = reader.ReadByte();
        byte hasPassword = reader.ReadByte();
        byte order = reader.ReadByte();

        List<HomeLandEntity> homelands = _database.HomeLands.ToList();

        foreach (var homeland in homelands)
        {
            //Filter by location. If distance is 0, ignore location.
            //Ignore if search is 5000 (Worldwide), or is 2933 (Asia) and HL is <= 2932 (Japan)
            //Deal with distance == 1 (same prefecture) later.
            if (distance == 2 && homeland.Location != location && location != 5000 &&
                !(location == 2933 && homeland.Location <= 2932))
            {
                homelands.Remove(homeland);
                continue;
            }

            //Filter by name. If name is empty, ignore name.
            if (!string.IsNullOrWhiteSpace(name) && !homeland.HomeLandName.Contains(name))
            {
                homelands.Remove(homeland);
                continue;
            }

            //Filter by player count.
            sbyte[] playerCnt = new sbyte[] { 0, 5, 10, 10, 20, 30 };
            if (players == 2)
            {
                if (homeland.RegisteredPlayerCnt >= 10)
                {
                    homelands.Remove(homeland);
                    continue;
                }
            }
            else if (homeland.RegisteredPlayerCnt < playerCnt[(int)players])
            {
                homelands.Remove(homeland);
                continue;
            }

            //Filter by time.
            byte[] timeCnt = new byte[] { 0, 0, 1, 5, 10 };
            if (time != 0 && homeland.Time < timeCnt[(int)time])
            {
                homelands.Remove(homeland);
                continue;
            }

            //Filter by password.
            if (hasPassword == 1 && string.IsNullOrWhiteSpace(homeland.Password))
            {
                homelands.Remove(homeland);
                continue;
            }
            else if (hasPassword == 2 && !string.IsNullOrWhiteSpace(homeland.Password))
            {
                homelands.Remove(homeland);
                continue;
            }
        }

        //Sort the list.
        homelands = order switch 
        {
            0 => homelands,
            1 => homelands.OrderByDescending(h => h.RegisteredPlayerCnt).ToList(),
            2 => homelands.OrderBy(h => h.RegisteredPlayerCnt).ToList(),
            3 => homelands.OrderBy(h => h.Time).ToList(),
            4 => homelands.OrderByDescending(h => h.ClearCnt).ToList(),
            _ => homelands
        };

        var responses = new List<FragmentMessage>();
        responses.Add(new HomeLandSearchResponse().SetResultCnt((byte)homelands.Count).Build());
        foreach (HomeLandEntity homeland in homelands)
        {
            responses.Add(new HomeLandSearchResultsResponse().SetHomeLand(homeland).Build());
        }

        return new ValueTask<ICollection<FragmentMessage>>(responses);
    }
}

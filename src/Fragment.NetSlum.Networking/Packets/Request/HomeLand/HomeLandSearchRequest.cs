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

    private bool IsSamePrefecture(ushort loc1, ushort loc2)
    {
        if (loc1 == loc2) return true; //Same location
        if (loc1 == 5000 || loc2 == 5000) return true; //Any location
        if (loc1 == 2933 && loc2 < 2933) return true; //Asia and Japan
        if (loc2 == 2933 && loc1 < 2933) return true; //Japan and Asia

        //table time. For later.

        return false;
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

        if (session.IsOverseas && location != 5000 && location <= 8)
        {
            //Before: Africa, Antarctica, Asia, Europe, Middle East, North America, Oceania, South America, Other
            //After: Asia, Africa, Oceania, North America, Middle East, Antarctica, South America, Europe, Other
            ushort[] locationMapping = new ushort[] { 1, 5, 0, 7, 4, 3, 2, 6, 8 };
            location = (ushort)(locationMapping[location] + 2933);
        }

        List<HomeLandEntity> localHomeLands = new List<HomeLandEntity>();
        List<HomeLandEntity> globalHomeLands = _database.HomeLands.ToList();

        foreach (var homeland in globalHomeLands)
        {
            //Filter by name. If name is empty, ignore name.
            if (!string.IsNullOrWhiteSpace(name) && !homeland.HomeLandName.Contains(name))
            {
                globalHomeLands.Remove(homeland);
                continue;
            }

            //Filter by player count.
            sbyte[] playerCnt = new sbyte[] { 0, 5, 10, 10, 20, 30 };
            if (players > 0)
            {
                if (players < 3)
                {
                    if (homeland.RegisteredPlayerCnt >= playerCnt[(int)players])
                    {
                        globalHomeLands.Remove(homeland);
                        continue;
                    }
                }
                else if (homeland.RegisteredPlayerCnt < playerCnt[(int)players])
                {
                    globalHomeLands.Remove(homeland);
                    continue;
                }
            }

            //Filter by time.
            byte[] timeCnt = new byte[] { 0, 0, 1, 5, 10 };
            if (time != 0 && homeland.Time / 60 < timeCnt[(int)time])
            {
                globalHomeLands.Remove(homeland);
                continue;
            }

            //Filter by password.
            if (hasPassword == 1 && string.IsNullOrWhiteSpace(homeland.Password))
            {
                globalHomeLands.Remove(homeland);
                continue;
            }
            else if (hasPassword == 2 && !string.IsNullOrWhiteSpace(homeland.Password))
            {
                globalHomeLands.Remove(homeland);
                continue;
            }

            //Filter by location. 0: ignore. 1: local only. 2: local first.
            if (distance != 0 && IsSamePrefecture(location, homeland.Location))
            {
                localHomeLands.Add(homeland);
                globalHomeLands.Remove(homeland);
            }
        }

        //Sort the list.
        switch (order)
        {
            case 1:
                localHomeLands = localHomeLands.OrderByDescending(h => h.RegisteredPlayerCnt).ToList();
                globalHomeLands = globalHomeLands.OrderByDescending(h => h.RegisteredPlayerCnt).ToList();
                break;
            case 2:
                localHomeLands = localHomeLands.OrderBy(h => h.RegisteredPlayerCnt).ToList();
                globalHomeLands = globalHomeLands.OrderBy(h => h.RegisteredPlayerCnt).ToList();
                break;
            case 3:
                localHomeLands = localHomeLands.OrderBy(h => h.Time).ToList();
                globalHomeLands = globalHomeLands.OrderBy(h => h.Time).ToList();
                break;
            case 4:
                localHomeLands = localHomeLands.OrderByDescending(h => h.ClearCnt).ToList();
                globalHomeLands = globalHomeLands.OrderByDescending(h => h.ClearCnt).ToList();
                break;
            default:
                break;
        }

				var totalCount = localHomeLands.Count + globalHomeLands.Count;
				
        //Limit to 50 results.
        if (localHomeLands.Count > 50) localHomeLands = localHomeLands.Take(50).ToList();
        if (localHomeLands.Count + globalHomeLands.Count > 50) globalHomeLands =
                globalHomeLands.Take(50 - localHomeLands.Count).ToList();

        var responses = new List<FragmentMessage>();
        responses.Add(new HomeLandSearchResponse().SetResultCnt((byte)(localHomeLands.Count+globalHomeLands.Count)).SetTotalCount((uint) totalCount).Build());
        foreach (HomeLandEntity homeland in localHomeLands)
        {
            responses.Add(new HomeLandSearchResultsResponse(homeland, session.IsOverseas).Build());
        }
        if (distance != 1) //If distance is 1, skip global results.
        {
            foreach (HomeLandEntity homeland in globalHomeLands)
            {
                responses.Add(new HomeLandSearchResultsResponse(homeland, session.IsOverseas).Build());
            }
        }
        return new ValueTask<ICollection<FragmentMessage>>(responses);
    }
}

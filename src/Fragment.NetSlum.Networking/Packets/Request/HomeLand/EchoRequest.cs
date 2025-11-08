using System;
using System.Linq;
using System.Collections.Generic;
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
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;
using Result = Fragment.NetSlum.Networking.Constants.Result;

namespace Fragment.NetSlum.Networking.Packets.Request.HomeLand;

[FragmentPacket(MessageType.Data, OpCodes.Echo)]
public class EchoRequest : BaseRequest
{
    private readonly FragmentContext _database;
    
    public EchoRequest(FragmentContext database)
    {
        _database = database;
    }

    public override ValueTask<ICollection<FragmentMessage>> GetResponse(FragmentTcpSession session, FragmentMessage request)
    {
        var reader = new SpanReader(request.Data.Span);
        uint dummyData1   = reader.ReadUInt32();
        ushort dummyData2 = reader.ReadUInt16();

        HomeLandEntity? homeland = session.HomeLand;
        
        Result result = Result.Ok;
        int accountId = session.PlayerAccountId;
        uint homelandId = 0;

        if (homeland == null)
        {
            homeland = _database.HomeLands.FirstOrDefault(h => h.PlayerAccountId == session.PlayerAccountId && (h.Status < 3));
        }
        
        if(homeland != null)
        {
            homelandId = homeland.HomeLandId;
        }
        else
        {
            result = Result.Fail;
        }
        
        var responses = new List<FragmentMessage>
        {
            new LoginReadyResponse().SetResult((byte)result).SetAccountId(accountId).SetHomelandId(homelandId).Build(),
        };

        return new ValueTask<ICollection<FragmentMessage>>(responses);
    }
}

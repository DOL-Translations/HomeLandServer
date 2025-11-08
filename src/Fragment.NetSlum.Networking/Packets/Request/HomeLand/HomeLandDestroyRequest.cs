using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fragment.NetSlum.Core.Extensions;
using Fragment.NetSlum.Networking.Attributes;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using Fragment.NetSlum.Networking.Packets.Response.HomeLand;
using Fragment.NetSlum.Networking.Sessions;
using Fragment.NetSlum.Persistence;
using Fragment.NetSlum.Persistence.Entities;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Request.HomeLand;

[FragmentPacket(MessageType.Data, OpCodes.HomeLandDestroy)]
public class HomeLandDestroyRequest : BaseRequest
{
    private readonly FragmentContext _database;
    
    public HomeLandDestroyRequest(FragmentContext database)
    {
        _database = database;
    }

    public override ValueTask<ICollection<FragmentMessage>> GetResponse(FragmentTcpSession session, FragmentMessage request)
    {
        HomeLandEntity? homeland = session.HomeLand;
        
        Result result = Result.Ok;
        
        if (homeland == null)
        {
            homeland = _database.HomeLands.FirstOrDefault(h => h.PlayerAccountId == session.PlayerAccountId && (h.Status < 3));
        }
        
        if (homeland != null)
        {
            _database.HomeLands.Remove(homeland);
            _database.SaveChanges();
            
            session.HomeLand = null;
            result = Result.Ok;
        }
        else
        {
            // we have no homeland for this player but we should say it is destroyed, otherwise the game won't update
            result = Result.Ok;
        }

        var responses = new List<FragmentMessage>
        {
            new HomeLandDestroyResponse().SetResult((byte)result).Build(),
        };

        return new ValueTask<ICollection<FragmentMessage>>(responses);
    }
}

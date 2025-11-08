using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fragment.NetSlum.Core.Buffers;
using Fragment.NetSlum.Networking.Attributes;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using Fragment.NetSlum.Networking.Packets.Response.HomeLand;
using Fragment.NetSlum.Networking.Sessions;
using Fragment.NetSlum.Persistence;
using Fragment.NetSlum.Persistence.Entities;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Request.HomeLand;

[FragmentPacket(MessageType.Data, OpCodes.HomeLandActivate)]
public class HomeLandActivateRequest : BaseRequest
{
    private readonly FragmentContext _database;

    public HomeLandActivateRequest(FragmentContext database)
    {
        _database = database;
    }

    public override ValueTask<ICollection<FragmentMessage>> GetResponse(FragmentTcpSession session, FragmentMessage request)
    {
        var reader = new SpanReader(request.Data.Span);
        
        uint countdown             = reader.ReadUInt32();
        byte registeredPlayerCount = reader.ReadByte();
        byte maxPlayerCount        = reader.ReadByte();
        
        if(countdown == 0)
        {
          session.HomeLand!.Status = 1;
          session.HomeLand!.LastUpdate = DateTime.UtcNow;
        }
        else
        {
          session.HomeLand!.Countdown = (byte) (countdown/600u);
          session.HomeLand!.LastUpdate = DateTime.UtcNow;
        }
        
        session.HomeLand!.RegisteredPlayerCnt = registeredPlayerCount;
        session.HomeLand!.MaxPlayerCnt = maxPlayerCount;
        /*try {*/ _database.SaveChanges(); /*} catch { result = Result.Fail; }*/

        return SingleMessage(new HomeLandActivateResponse().Build());
    }
}

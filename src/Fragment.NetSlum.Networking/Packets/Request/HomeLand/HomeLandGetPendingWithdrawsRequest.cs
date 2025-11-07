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

[FragmentPacket(MessageType.Data, OpCodes.HomeLandGetPendingWithdraws)]
public class HomeLandGetPendingWithdrawsRequest : BaseRequest
{
    /* private readonly FragmentContext _database;

    public HomeLandGetPendingWithdrawsRequest(FragmentContext database)
    {
        _database = database;
    } */

    public override ValueTask<ICollection<FragmentMessage>> GetResponse(FragmentTcpSession session, FragmentMessage request)
    {
        var reader = new SpanReader(request.Data.Span);
        
        byte status    = reader.ReadByte();
        byte count     = reader.ReadByte();
        uint dummyData = reader.ReadUInt32();
        
        return SingleMessage(new HomeLandGetPendingWithdrawsResponse().Build());
    }
}

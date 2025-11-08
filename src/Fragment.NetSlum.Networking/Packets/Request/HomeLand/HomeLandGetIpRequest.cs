using System;
using System.Linq;
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
using Result = Fragment.NetSlum.Networking.Constants.Result;

namespace Fragment.NetSlum.Networking.Packets.Request.HomeLand;

[FragmentPacket(MessageType.Data, OpCodes.HomeLandGetIp)]
public class HomeLandGetIpRequest : BaseRequest
{
    private readonly FragmentContext _database;
    
    public HomeLandGetIpRequest(FragmentContext database)
    {
        _database = database;
    }

    public override ValueTask<ICollection<FragmentMessage>> GetResponse(FragmentTcpSession session, FragmentMessage request)
    {
        /*Request_GetHomelandIP //OPCODE_GET_HOMELAND_IP
        {
            uint HomelandID;
            ushort Unk;
        }
        */
        
        var reader = new SpanReader(request.Data.Span);
        
        uint homelandId = reader.ReadUInt32();
        ushort unk1     = reader.ReadUInt16();

        HomeLandEntity homeland = _database.HomeLands.FirstOrDefault(p => p.HomeLandId == homelandId);
        uint ipAddress = homeland?.LocalIp ?? 0;
        Result result = Result.Ok;

        if (homeland == null || homeland?.Status == 3)
        {
            result = Result.JoinRemovedHomeLand;
        }
        else if (homeland?.Status == 2)
        {
            result = Result.JoinSuspendedHomeLand;
        }
        else if (ipAddress == 0)
        {
            result = Result.Fail;
        }
        
        return SingleMessage(new HomeLandGetIpResponse().SetResult((byte)result).SetIpAddress(ipAddress).Build());
    }
}

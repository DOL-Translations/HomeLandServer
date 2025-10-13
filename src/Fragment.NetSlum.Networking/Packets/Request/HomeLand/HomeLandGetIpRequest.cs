using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fragment.NetSlum.Networking.Attributes;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using Fragment.NetSlum.Networking.Packets.Response.HomeLand;
using Fragment.NetSlum.Networking.Sessions;
using Fragment.NetSlum.Persistence;
using Fragment.NetSlum.Persistence.Entities;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

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

        uint homelandId = System.BitConverter.ToUInt32(request.Data.Span.Slice(0, 4));
        uint ipAddress = _database.HomeLands.FirstOrDefault(p => p.HomeLandId == homelandId)?.LocalIp ?? 0;

        return SingleMessage(new HomeLandGetIpResponse().SetIpAddress(ipAddress).Build());
    }
}

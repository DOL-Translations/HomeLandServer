using System.Collections.Generic;
using System.Threading.Tasks;
using Fragment.NetSlum.Networking.Attributes;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using Fragment.NetSlum.Networking.Packets.Response.HomeLand;
using Fragment.NetSlum.Networking.Sessions;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;
using Result = Fragment.NetSlum.Networking.Constants.Result;

namespace Fragment.NetSlum.Networking.Packets.Request.HomeLand;

[FragmentPacket(MessageType.Data, OpCodes.GetMatchingServer)]
public class GetMatchingServerRequest : BaseRequest
{
    public override ValueTask<ICollection<FragmentMessage>> GetResponse(FragmentTcpSession session, FragmentMessage request)
    {
        //check matching server IP, Result.MatchingServerNotFound if not found

        return SingleMessage(new GetMatchingServerResponse((byte)Result.Ok).Build());
    }
}

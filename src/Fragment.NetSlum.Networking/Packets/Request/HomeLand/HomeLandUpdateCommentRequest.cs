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
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Request.HomeLand;

[FragmentPacket(MessageType.Data, OpCodes.HomeLandUpdateComment)]
public class HomeLandUpdateCommentRequest : BaseRequest
{
    private readonly FragmentContext _database;

    public HomeLandUpdateCommentRequest(FragmentContext database)
    {
        _database = database;
    }

    public override ValueTask<ICollection<FragmentMessage>> GetResponse(FragmentTcpSession session, FragmentMessage request)
    {
        var reader = new SpanReader(request.Data.Span);
        string comment = reader.ReadString(out _).ToShiftJisString();
        
        if (comment == null)
        {
            comment = string.Empty;
        }
        
        session.HomeLand.Comment = comment;
        _database.SaveChanges();
        
        return SingleMessage(new HomeLandUpdateCommentResponse().Build());
    }
}

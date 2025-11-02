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

namespace Fragment.NetSlum.Networking.Packets.Request.HomeLand;

[FragmentPacket(MessageType.Data, OpCodes.HomeLandGetIp)]
public class HomeLandGetIpRequest : BaseRequest
{
    private readonly FragmentContext _database;
		
		private const byte RESULT_OK = 0x00;
		private const byte RESULT_FAIL = 0x01;
    
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
				
				Console.WriteLine($"Homeland ID : {homelandId} (0x{homelandId:X8})");
				Console.WriteLine($"Unk1        : {unk1} (0x{unk1:X4})");
				
				uint ipAddress = _database.HomeLands.FirstOrDefault(p => p.HomeLandId == homelandId)?.LocalIp ?? 0;
				Console.WriteLine($"DB decimal: {ipAddress}, hex: {ipAddress:X8}");
				Console.WriteLine($"DB value decimal: {ipAddress}, hex: {ipAddress:X}");
				Console.WriteLine($"IP_GETIP_REQUEST  : {ipAddress}");
				if(ipAddress == 0)
				{
					return SingleMessage(new HomeLandGetIpResponse().SetResult(RESULT_FAIL).SetIpAddress(ipAddress).Build());
				}
				
				return SingleMessage(new HomeLandGetIpResponse().SetResult(RESULT_OK).SetIpAddress(ipAddress).Build());
				
				
				////////////////////////////////////////////////////////

        /* uint homelandId = System.BitConverter.ToUInt32(request.Data.Span.Slice(0, 4));
        uint ipAddress = _database.HomeLands.FirstOrDefault(p => p.HomeLandId == homelandId)?.LocalIp ?? 0;
        var ipBytes = System.BitConverter.GetBytes(ipAddress);
        if (System.BitConverter.IsLittleEndian)
        {
            System.Array.Reverse(ipBytes);
        }
        ipAddress = System.BitConverter.ToUInt32(ipBytes, 0);
        return SingleMessage(new HomeLandGetIpResponse().SetIpAddress(ipAddress).Build()); */
    }
}

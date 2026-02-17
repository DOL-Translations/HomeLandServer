using System;
using System.Buffers.Binary;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
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
using Fragment.NetSlum.TcpServer.Extensions;

namespace Fragment.NetSlum.Networking.Packets.Request.HomeLand;

[FragmentPacket(MessageType.Data, OpCodes.HomeLandCreate)]
public class HomeLandCreateRequest : BaseRequest
{
    private readonly FragmentContext _database;

    private const int FIREWALL_CHECK_PORT = 9003;

    public HomeLandCreateRequest(FragmentContext database)
    {
        _database = database;
    }

    public static uint IPv4ToUInt32(IPAddress ip)
    {
        if (ip is null) throw new ArgumentNullException(nameof(ip));

        var v4 = ip.MapToIPv4();
        var b = v4.GetAddressBytes(); // 4 bytes

        return ((uint)b[0] << 24) | ((uint)b[1] << 16) | ((uint)b[2] << 8) | b[3];
    }

    public override ValueTask<ICollection<FragmentMessage>> GetResponse(FragmentTcpSession session, FragmentMessage request)
    {
        /*
        Request_HomelandInfo //OPCODE_HOMELAND_INFO
        {
          byte FreshHomeland;
          uint LocalIP;
          char name[]; //null terminated string
          ushort Location;
          byte Time;
          char Password[]; //null terminated string
          char Comment[]; //null terminated string
          byte RegisteredPlayerCount;
          uint32_t ClearCount; //? This looks like what the list says
          byte Unk2; // -> probably 0x01 as it is a game server
          byte Unk2; //Either 6 or 2, depending on some variable. -> probably 
        }
        */

        var reader = new SpanReader(request.Data.Span);

        byte freshHomeland          = reader.ReadByte();
        uint localIp                = reader.ReadUInt32();
        string name                 = reader.ReadString(out _).ToShiftJisString();
        ushort location             = reader.ReadUInt16();
        byte time                   = reader.ReadByte();
        string password             = reader.ReadString(out _).ToShiftJisString();
        string comment              = reader.ReadString(out _).ToShiftJisString();
        byte registeredPlayerCount  = reader.ReadByte();
        uint clearCount             = reader.ReadUInt32();
        byte repeat                 = reader.ReadByte();
        byte gameVersion               = reader.ReadByte();

        session.IsTestDisc = (gameVersion == 2 || gameVersion == 3);
        session.IsOverseas = (gameVersion == 3 || gameVersion == 7);

        Result result = Result.Ok;

        Console.WriteLine($"IP_CREATE_REQUEST  : {localIp}");

        //byte[] ipBytes = IPAddress.Parse(session.Socket!.GetClientIp()).GetAddressBytes();
        //localIp = (uint)(ipBytes[0] | (ipBytes[1] << 8) | (ipBytes[2] << 16) | (ipBytes[3] << 24));
        //localIp = ((uint)ipBytes[0] << 24) | ((uint)ipBytes[1] << 16) | ((uint)ipBytes[2] << 8) | ipBytes[3];
        
        Span<byte> ipBytes = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(ipBytes, localIp);

        var clientReportedIp = new IPAddress(ipBytes.ToArray());

        var actualClientIp = ((IPEndPoint)session.Socket!.RemoteEndPoint!).Address;
        var targetIp = clientReportedIp.IsPrivate() ? actualClientIp : clientReportedIp;

        //todo: store both local and global ip

        try
        {
            using var client = new TcpClient();

            var connectTask = client.ConnectAsync(targetIp, FIREWALL_CHECK_PORT);
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(5));

            // Wait for either connection or timeout to complete
            var completedTask = Task.WhenAny(connectTask, timeoutTask).GetAwaiter().GetResult();

            if (completedTask != connectTask || !client.Connected)
            {
                result = Result.PublishFailedFirewall;
            }
            else
            {
                result = Result.Ok;
            }

            client.Close();
        }
        catch
        {
            result = Result.PublishFailedFirewall;
        }
        Console.WriteLine($"FirewallCheckRequest: clientReportedIp={clientReportedIp}, actualClientIp={actualClientIp}, targetIp={targetIp}, result={result}");
        
        if (session.IsOverseas && location != (ushort)5000 && location <= (ushort)8)
        {
            //Before: Africa, Antarctica, Asia, Europe, Middle East, North America, Oceania, South America, Other
            //After: Asia, Africa, Oceania, North America, Middle East, Antarctica, South America, Europe, Other
            ushort[] locationMapping = new ushort[] { 1, 5, 0, 7, 4, 3, 2, 6, 8 };
            location = (ushort)(locationMapping[location] + 2933);
        }

        //todo, run the firewall check *again* and return Result.PublishFailedFirewall if it fails

        //search for existing homelands with this id
        var homeland = _database.HomeLands.FirstOrDefault(h => h.PlayerAccountId == session.PlayerAccountId && h.Status < 3);
        if (homeland != null)
        {
            /*if (homeland.LastUpdate + TimeSpan.FromHours(1) > DateTime.UtcNow)
            {
                result = Result.PublishFailedWaitOneHour;
            }
            else
            {*/
                homeland.Status = 1;
                homeland.LocalIp = IPv4ToUInt32(targetIp);
                homeland.RegisteredPlayerCnt = registeredPlayerCount;
                homeland.ClearCnt = clearCount;
                homeland.LastUpdate = DateTime.UtcNow;
            //}
        }
        else
        {
            byte status = 1;
            if(freshHomeland != 1 && time > 0)
            {
              status = 0;
            }
            
            homeland = new HomeLandEntity
            {
                HomeLandId = 0,
                Status              = status,
                PlayerAccountId     = session.PlayerAccountId,
                LocalIp             = IPv4ToUInt32(targetIp),
                HomeLandName        = name,
                Location            = location,
                Countdown           = time,
                Password            = password,
                Comment             = comment,
                RegisteredPlayerCnt = registeredPlayerCount,
                ClearCnt            = clearCount,
                CreatedAt           = DateTime.UtcNow,
                LastUpdate          = DateTime.UtcNow,
                Repeat              = repeat,
                HeartbeatMode       = 0,
            };
            _database.HomeLands.Add(homeland);
        }
        try { _database.SaveChanges(); } catch { result = Result.PublishFailed; }

        session.HomeLand = homeland;

        var responses = new List<FragmentMessage>
        {
            new HomeLandCreateResponse().SetResult((byte)result).Build(),
            new HL2016Response(0x01).Build(),
        };

        return new ValueTask<ICollection<FragmentMessage>>(responses);
    }
}

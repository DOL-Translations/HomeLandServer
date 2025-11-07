using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fragment.NetSlum.Core.Buffers;
using Fragment.NetSlum.Core.Extensions;
using Fragment.NetSlum.Networking.Attributes;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using Fragment.NetSlum.Networking.Packets.Response.HomeLand;
using Fragment.NetSlum.Networking.Sessions;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Request.HomeLand;

[FragmentPacket(MessageType.Data, OpCodes.FirewallCheck)]
public class FirewallCheckRequest : BaseRequest
{
    private const byte RESULT_OK = 0x00;
    private const byte RESULT_FAIL = 0x0B;
    private const int FIREWALL_CHECK_PORT = 9003;
    
    public override ValueTask<ICollection<FragmentMessage>> GetResponse(FragmentTcpSession session, FragmentMessage request)
    {
        var reader = new SpanReader(request.Data.Span);
        
        uint localIp = reader.ReadUInt32();
        ushort unk   = reader.ReadUInt16();

        byte result = RESULT_OK;

        try
        {
            // Convert uint IP to IPAddress (big endian)
            var ipBytes = BitConverter.GetBytes(localIp);

            var clientReportedIp = new IPAddress(ipBytes);
            
            var actualClientIp = ((IPEndPoint)session.Socket!.RemoteEndPoint!).Address;
            var targetIp = clientReportedIp.IsPrivate() ? actualClientIp : clientReportedIp;

            Console.WriteLine($"FirewallCheckRequest: clientReportedIp={clientReportedIp}, actualClientIp={actualClientIp}, targetIp={targetIp}, result={result}, unk={unk}");

            using var client = new TcpClient();

            var connectTask = client.ConnectAsync(targetIp, FIREWALL_CHECK_PORT);
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(5));

            // Wait for either connection or timeout to complete
            var completedTask = Task.WhenAny(connectTask, timeoutTask).GetAwaiter().GetResult();

            if (completedTask != connectTask || !client.Connected)
            {
                result = RESULT_FAIL;
            }
            else
            {
                result = RESULT_OK;
            }

            client.Close();
        }
        catch
        {
            result = RESULT_FAIL;
        }

        return SingleMessage(new FirewallCheckResponse(result).Build());
    }
}

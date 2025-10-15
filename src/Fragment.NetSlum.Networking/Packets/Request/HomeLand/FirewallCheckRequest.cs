using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public override ValueTask<ICollection<FragmentMessage>> GetResponse(FragmentTcpSession session, FragmentMessage request)
    {
        const int FIREWALL_CHECK_PORT = 9003;
        uint localIp = System.BitConverter.ToUInt32(request.Data.Span.Slice(0, 4));
        byte error = 0x00;

        /*try
        {
            var ipBytes = BitConverter.GetBytes(localIp);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(ipBytes);

            var clientReportedIp = new IPAddress(ipBytes);
            //var actualClientIp = IPAddress.Parse(session.Socket!.GetClientIp());
            var actualClientIp = ((IPEndPoint)session.Socket!.RemoteEndPoint!).Address;

            // If local IP is private, substitute with the real public IP from socket
            var targetIp = clientReportedIp.IsPrivate() ? actualClientIp : clientReportedIp;

            using var client = new TcpClient();
            var result = client.BeginConnect(targetIp, FIREWALL_CHECK_PORT, null, null);
            bool success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5));
            error = (!success || !client.Connected) ? (byte)0x0B : (byte)0x00;
            client.Close();
        }
        catch (Exception ex)
        {
            error = 0x0B;
        }*/

        return SingleMessage(new FirewallCheckResponse(error).Build());
    }
}

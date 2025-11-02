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
				uint localIp = BitConverter.ToUInt32(request.Data.Span.Slice(0, 4));
				byte error = 0x00;

				try
				{
						// Convert uint IP to IPAddress (big endian)
						var ipBytes = BitConverter.GetBytes(localIp);
						// if (BitConverter.IsLittleEndian)
						// 		Array.Reverse(ipBytes);

						var clientReportedIp = new IPAddress(ipBytes);
						var actualClientIp = ((IPEndPoint)session.Socket!.RemoteEndPoint!).Address;
						var targetIp = clientReportedIp.IsPrivate() ? actualClientIp : clientReportedIp;

						Console.WriteLine($"FirewallCheckRequest: clientReportedIp={clientReportedIp}, actualClientIp={actualClientIp}, targetIp={targetIp}, error={error}");

						using var client = new TcpClient();

						var connectTask = client.ConnectAsync(targetIp, FIREWALL_CHECK_PORT);
						var timeoutTask = Task.Delay(TimeSpan.FromSeconds(5));

						// Wait for either connection or timeout to complete
						var completedTask = Task.WhenAny(connectTask, timeoutTask).GetAwaiter().GetResult();

						if (completedTask != connectTask || !client.Connected)
						{
								error = 0x0B;
						}
						else
						{
								error = 0x00;
						}

						client.Close();
				}
				catch
				{
						error = 0x0B;
				}

				return SingleMessage(new FirewallCheckResponse(error).Build());
		}
}

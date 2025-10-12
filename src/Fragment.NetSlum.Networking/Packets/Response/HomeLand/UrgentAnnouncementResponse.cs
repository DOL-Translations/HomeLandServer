using Fragment.NetSlum.Core.Buffers;
using Fragment.NetSlum.Core.Extensions;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using System;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Response.HomeLand
{
    public class UrgentAnnouncementResponse : BaseResponse
    {
        private string _message = "Sample Message\nfrom the server :)";

        public void SetMessage(string message)
        {
            _message = message;
        }

        public override FragmentMessage Build()
        {
            var messageBytes = _message.ToShiftJis();
            var writer = new MemoryWriter(messageBytes.Length + 1);
            writer.Write(messageBytes);

            return new FragmentMessage
            {
                MessageType = MessageType.Data,
                DataPacketType = OpCodes.UrgentAnnouncement,
                Data = writer.Buffer,
            };
        }
    }
}

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Crypto;
using Fragment.NetSlum.Networking.Objects;
using Serilog;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;
using Fragment.NetSlum.Core.Extensions;

namespace Fragment.NetSlum.Networking.Pipeline.Decoders;

public class FragmentFrameDecoder : IPacketDecoder
{
    private readonly CryptoHandler _cryptoHandler;

    public FragmentFrameDecoder(CryptoHandler cryptoHandler)
    {
        _cryptoHandler = cryptoHandler;
    }

    public int Decode(Memory<byte> data, List<FragmentMessage> messages)
    {
        var stream = new MemoryStream(data.ToArray());
        var reader = new BinaryReader(stream);

        int pos = 0;

        // Not enough frame data to read
        if (data.Length < 2)
        {
            return 0;
        }

        ushort datalen = BinaryPrimitives.ReadUInt16BigEndian(data.Span[pos..2]);
        pos += 2;

        // If the length designated by the packet is larger than our incoming data,
        // then we haven't received all of the content
        if (datalen > data.Length - 2)
        {
            return 0;
        }

        if (datalen < 1)
        {
            return pos;
        }

        var messageContent = data.Span[pos..(pos+datalen)];
        pos += messageContent.Length;

        var messageType = (MessageType)BinaryPrimitives.ReadUInt16BigEndian(messageContent[..2]);

        messageContent = messageContent[2..];

        if (messageContent.Length < 1)
        {
            messages.Add(new FragmentMessage
            {
                MessageType = messageType,
                //Data = decrypted,
            });
            return pos;
        }

        var ok = _cryptoHandler.TryDecrypt(messageContent.ToArray(), out var decrypted);

        Log.Information("[CRYPTO] Decrypt Result: {Result}", ok ? "OK" : "FAIL");
        Log.Information("[CRYPTO] Message Type: {Type}", messageType);
        Log.Information("[CRYPTO] Decrypt Data: {Data}", decrypted.ToHexDump());

        var dataPacketType = OpCodes.None;

        // For data packets, the payload contains a envelope in the following format
        // [??:ushort][sequenceNum:ushort][envelopeContentLength:ushort][dataPacketType:ushort][...Data...]
        if (messageType == MessageType.Data)
        {
            var decryptedAsSpan = decrypted.AsSpan();
            var clientSequenceNumber = BinaryPrimitives.ReadUInt16BigEndian(decryptedAsSpan[2..4]);
            var dataLen = BinaryPrimitives.ReadUInt16BigEndian(decryptedAsSpan[4..6]);

            dataPacketType = (OpCodes) BinaryPrimitives.ReadUInt16BigEndian(decryptedAsSpan[6..8]);
            decrypted = decrypted[8..];
        }

        messages.Add(new FragmentMessage
        {
            MessageType = messageType,
            DataPacketType = dataPacketType,
            Data = decrypted,
        });

        return pos;
    }
}

namespace UDP.Core
{
    public class UdpMessage
    {
        private const ushort _maxPacketSize = ushort.MaxValue - 28; // 20 байт заголовка ip + 8 байт заголовка udp.
        // Максимальный размер данных в пакете через интернет 8192 байт.

        public int Size { get; init; }

        public IEnumerable<IReadOnlyCollection<byte>> Packets { get; init; }

        public UdpMessage(byte[] data)
        {
            var totalLength = data.Length;
            var chunkLength = (int)Math.Ceiling(totalLength / (double)_maxPacketSize);
            var packets = Enumerable.Range(0, chunkLength)
                                  .Select(i => data
                                  .Skip(i * _maxPacketSize)
                                  .Take(_maxPacketSize)
                                  .ToArray());

            Size = data.Length;
            Packets ??= packets;
        }

        
    }
}

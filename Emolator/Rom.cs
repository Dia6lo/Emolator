using System;
using System.Collections.Generic;

namespace Emolator
{
    public class Rom
    {
        private readonly byte[] bytes;
        private readonly ArraySegment<byte> prg;

        public Rom(byte[] bytes)
        {
            this.bytes = bytes;
            prg = new ArraySegment<byte>(bytes, 16, 16384 * PrgSize);
        }

        public byte PrgSize => bytes[4];

        public IReadOnlyCollection<byte> Prg => prg;

        public static Rom Create(byte[] bytes)
        {
            switch (bytes[7] & 0x0C)
            {
                case 0x08:
                    // TODO: Add ROM size check.
                    return new NewNesRom(bytes);
                case 0x00:
                    //TODO: Add 12-15 emptiness check.
                    return new CommonNesRom(bytes);
                default:
                    return new ArchaicNesRom(bytes);
            }
        }
    }
}
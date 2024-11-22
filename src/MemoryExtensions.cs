using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NcmSharp;
public static class MemoryExtensions
{
    public static int Read(this MemoryStream source, Span<byte> buffer, int offset, int count)
    {
        var read = 0;
        while (true)
        {
            var pos = read + offset;
            if (pos == buffer.Length)
                return read;
            var data = source.ReadByte();
            if (data == -1)
                return read;
            buffer[pos] = (byte)data;
            read++;
        }
    }
}

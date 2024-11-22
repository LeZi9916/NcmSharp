using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NcmSharp.Extensions;

namespace NcmSharp;
public struct NcmFileInfo
{
    public required string Path { get; init; }
    public required NcmMeta Meta { get; init; }
    public required byte[] KeyBox { get; init; }

    long _albumCoverLen;
    long _albumCoverPos;
    long _rawAudioTrackPos;
    long _rawAudioTrackLen;

    public NcmFileInfo(long albumCoverPos,long albumCoverLen,long rawAudioTrackPos,long rawAudioTrackLen)
    {
        _albumCoverPos = albumCoverPos;
        _albumCoverLen = albumCoverLen;
        _rawAudioTrackLen = rawAudioTrackLen;
        _rawAudioTrackPos = rawAudioTrackPos;
    }
    public async Task DumpAsFileAsync(string outputPath)
    {
        var keyBox = KeyBox;

        using var resultBufferWriter = File.Create(outputPath);
        using var rawAudioTrackReader = File.OpenRead(Path);
        rawAudioTrackReader.Position = _rawAudioTrackPos;
        await Task.Run(async () =>
        {
            var buffer = new byte[0x8000];
            var read = await rawAudioTrackReader.ReadAsync(buffer, 0, buffer.Length);

            while (read > 0)
            {
                for (var i = 0; i < read; i++)
                {
                    var j = i + 1 & 0xff;
                    buffer[i] ^= keyBox[keyBox[j] + keyBox[keyBox[j] + j & 0xff] & 0xff];
                }
                await resultBufferWriter.WriteAsync(buffer.AsMemory().Slice(0, read));
                read = await rawAudioTrackReader.ReadAsync(buffer, 0, buffer.Length);
            }
        });
    }
    public async Task<NcmStruct> DumpAsStructAsync()
    {
        using var fileReader = File.OpenRead(Path);
        using var binReader = new BinaryReader(fileReader);
        fileReader.Position = _albumCoverPos;
        var rawAlbumCover = new byte[_albumCoverLen];
        var rawAudioTrack = new byte[_rawAudioTrackLen];
        var _ = await fileReader.ReadAsync(rawAlbumCover);
        _ = await fileReader.ReadAsync(rawAudioTrack);

        return new NcmStruct()
        {
            KeyBox = KeyBox,
            Meta = Meta,
            RawAlbumCover = rawAlbumCover,
            RawAudioTrack = rawAudioTrack,
        };
    }
}

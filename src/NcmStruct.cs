using NcmSharp.Extensions;

namespace NcmSharp;
public struct NcmStruct
{
    public required byte[] KeyBox { get; init; }
    public required NcmMeta Meta { get; init; }
    public required byte[] RawAlbumCover { get; init; }
    public required byte[] RawAudioTrack { get; init; }


    public NcmStruct()
    {

    }
    public async Task<byte[]> DumpAsync()
    {
        var resultBuffer = new byte[RawAudioTrack.Length];
        var keyBox = KeyBox;

        using var resultBufferWriter = new MemoryStream(resultBuffer);
        using var rawAudioTrackReader = new MemoryStream(RawAudioTrack);
        await Task.Run(() =>
        {
            Span<byte> buffer = stackalloc byte[0x8000];
            var read = rawAudioTrackReader.Read(buffer, 0, buffer.Length);

            while (read > 0)
            {
                for (var i = 0; i < read; i++)
                {
                    var j = i + 1 & 0xff;
                    buffer[i] ^= keyBox[keyBox[j] + keyBox[keyBox[j] + j & 0xff] & 0xff];
                }
                resultBufferWriter.Write(buffer.Slice(0, read));
                read = rawAudioTrackReader.Read(buffer, 0, buffer.Length);
            }
        });
        return resultBuffer;
    }
    public async Task DumpAsync(string outputPath)
    {
        using var fileWriter = File.Create(outputPath);
        var result = await DumpAsync();
        await fileWriter.WriteAsync(result);
    }
}

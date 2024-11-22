using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NcmSharp;
public struct NcmMeta
{
    [JsonPropertyName("musicId")]
    [JsonConverter(typeof(IdConverter))]
    public long? MusicId { get; set; } = 0;

    [JsonPropertyName("musicName")]
    public string MusicName { get; set; } = string.Empty;

    [JsonPropertyName("artist")]
    [JsonConverter(typeof(NMSLConverter))]
    public string[][] Artist { get; set; } = Array.Empty<string[]>();

    [JsonPropertyName("albumId")]
    [JsonConverter(typeof(IdConverter))]
    public long? AlbumId { get; set; } = 0;

    [JsonPropertyName("album")]
    public string Album { get; set; } = string.Empty;

    [JsonPropertyName("albumPicDocId")]
    [JsonConverter(typeof(IdConverter))]
    public long? AlbumPicDocId { get; set; } = 0;

    [JsonPropertyName("albumPic")]
    public string AlbumPic { get; set; } = string.Empty;

    [JsonPropertyName("bitrate")]
    public int Bitrate { get; set; }

    [JsonPropertyName("mp3DocId")]
    [JsonConverter(typeof(IdConverter))]
    public long? Mp3DocId { get; set; } = 0;

    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("mvId")]
    [JsonConverter(typeof(IdConverter))]
    public long? MvId { get; set; } = 0;

    [JsonPropertyName("alias")]
    public string[] Alias { get; set; } = Array.Empty<string>();

    [JsonPropertyName("transNames")]
    public string[] TransNames { get; set; } = Array.Empty<string>();

    [JsonPropertyName("format")]
    public string Format { get; set; } = string.Empty;

    [JsonPropertyName("fee")]
    public int Fee { get; set; }

    [JsonPropertyName("volumeDelta")]
    public double VolumeDelta { get; set; }

    [JsonPropertyName("privilege")]
    public Privilege Privilege { get; set; } = new();
    public NcmMeta()
    {

    }
}

public struct Privilege
{
    [JsonPropertyName("flag")]
    public int Flag { get; set; }
}
[JsonSerializable(typeof(NcmMeta))]
internal partial class NcmMetaJsonContect: JsonSerializerContext
{

}

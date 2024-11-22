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
    public string MusicId { get; set; } = string.Empty;

    [JsonPropertyName("musicName")]
    public string MusicName { get; set; } = string.Empty;

    [JsonPropertyName("artist")]
    public string[][] Artist { get; set; } = Array.Empty<string[]>();

    [JsonPropertyName("albumId")]
    public string AlbumId { get; set; } = string.Empty;

    [JsonPropertyName("album")]
    public string Album { get; set; } = string.Empty;

    [JsonPropertyName("albumPicDocId")]
    public string AlbumPicDocId { get; set; } = string.Empty;

    [JsonPropertyName("albumPic")]
    public string AlbumPic { get; set; } = string.Empty;

    [JsonPropertyName("bitrate")]
    public int Bitrate { get; set; }

    [JsonPropertyName("mp3DocId")]
    public string Mp3DocId { get; set; } = string.Empty;

    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("mvId")]
    public string MvId { get; set; } = string.Empty;

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
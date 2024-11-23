# NcmSharp

An ncm format encrypted audio file CSharp decryption library

Can convert encrypted audio files in ncm format to flac or mp3 audio files

This library is intended for learning and research. Please comply with local laws and regulations, and abide the [project license](https://github.com/LeZi9916/NcmSharp/blob/master/LICENSE.txt).

## Usage

```C#
using NcmSharp;

var filePath = "example.ncm";
var fileInfo = await NcmHelper.ReadFileAsync(filePath);
var meta = fileInfo.Meta;
var artist = meta.Artist?.FirstOrDefault()?.FirstOrDefault() ?? "Undefined";
var outputPath = $"{meta.MusicName} - {artist}.{meta.Format}";

await fileInfo.DumpAsFileAsync(outputPath);
```

## License

MIT license

## References

- [Majjcom/ncmpp](https://github.com/Majjcom/ncmpp)

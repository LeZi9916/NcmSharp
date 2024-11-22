using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NcmSharp;
public static class NcmHelper
{
    public static byte[] MAGIC_HEADER { get; } = new byte[8]
    {
        0x43,0x54,0x45,0x4e,0x46,0x44,0x41,0x4d
    };
    public static byte[] KEY_AES_KEY { get; } = new byte[16]
    {
        104,122,72,82,65,109,115,111,53,107,73,110,98,97,120,87
    };
    public static byte[] META_AES_KEY { get; } = new byte[16]
    {
        35,49,52,108,106,107,95,33,92,93,38,48,85,60,39,40
    };

    public static bool IsValidFile(string filePath)
    {
        try
        {
            using var fileReader = File.OpenRead(filePath);
            using var binReader = new BinaryReader(fileReader);
            var header = binReader.ReadBytes(8);

            for (var i = 0; i < 8; i++)
            {
                if (header[i] != NcmHelper.MAGIC_HEADER[i])
                    return false;
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
    public static async Task<NcmFileInfo> ReadFileAsync(string filePath)
    {
        if (!IsValidFile(filePath))
            throw new ArgumentException("");
        return await Task.Run(async () =>
        {
            using var fileReader = File.OpenRead(filePath);
            using var binReader = new BinaryReader(fileReader);

            var header = binReader.ReadBytes(8);
            fileReader.Seek(2, SeekOrigin.Current);

            var keyLen = binReader.ReadInt32();
            var rawKey = XOR(binReader.ReadBytes(keyLen), 0x64);

            var decryptedKey = PKCS7Unpadding(await DecryptAsync(KEY_AES_KEY, rawKey.ToArray()));
            var key = decryptedKey.Slice(17);
            var keyBox = await GenerateKeyBoxAsync(key.ToArray());

            var metaLen = binReader.ReadInt32();
            var rawMeta = XOR(binReader.ReadBytes(metaLen), 0x63);

            var metaBase64 = Encoding.UTF8.GetString(rawMeta.Slice(22));
            var decodedMetaData = Convert.FromBase64String(metaBase64);
            var decryptedMetaData = PKCS7Unpadding(await DecryptAsync(META_AES_KEY, decodedMetaData));
            using var metaStream = new MemoryStream(decryptedMetaData.Slice(6).ToArray());
            var ncmMeta = await JsonSerializer.DeserializeAsync(metaStream, NcmMetaJsonContect.Default.NcmMeta);

            // Skip cover crc data
            fileReader.Seek(9, SeekOrigin.Current);
            var albumCoverLen = binReader.ReadInt32();
            var albumCoverPos = fileReader.Position;
            fileReader.Seek(albumCoverLen, SeekOrigin.Current);
            var rawAudioTrackPos = fileReader.Position;
            var rawAudioTrackLen = fileReader.Length - fileReader.Position;

            return new NcmFileInfo(albumCoverPos, albumCoverLen, rawAudioTrackPos, rawAudioTrackLen)
            {
                Path = filePath,
                Meta = ncmMeta,
                KeyBox = keyBox,
            };
        });
    }
    static async ValueTask<byte[]> GenerateKeyBoxAsync(Memory<byte> key)
    {
        return await Task.Run(() =>
        {
            Span<byte> keyBox = stackalloc byte[256];
            var _key = key.Span;
            for (var i = 0; i < keyBox.Length; i++)
                keyBox[i] = (byte)i;
            byte c = 0;
            var keyOffset = 0;

            for (var i = 0; i < keyBox.Length; i++)
            {
                c = (byte)(keyBox[i] + c + _key[keyOffset] & 0xff);
                keyOffset++;
                if (keyOffset >= key.Length)
                    keyOffset = 0;

                (keyBox[i], keyBox[c]) = (keyBox[c], keyBox[i]);
            }
            return keyBox.ToArray();
        });
    }
    static Span<byte> XOR(Span<byte> source, byte value)
    {
        for (var i = 0; i < source.Length; i++)
            source[i] ^= value;
        return source;
    }
    static async ValueTask<byte[]> DecryptAsync(byte[] key, byte[] raw)
    {
        return await Task.Run(() =>
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;
            using var decryptor = aes.CreateDecryptor();

            return decryptor.TransformFinalBlock(raw, 0, raw.Length);
        });
    }
    static Span<byte> PKCS7Unpadding(Span<byte> data)
    {
        var pad = data[data.Length - 1];

        return data.Slice(0, data.Length - pad);
    }
}

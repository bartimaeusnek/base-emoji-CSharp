namespace base_emoji_CSharp;

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BaseEmoji
{
    public static string Encode(ReadOnlySpan<byte> bytes, EncodeOptions options = default)
    {
        if (bytes.Length == 0)
        {
            return string.Empty;
        }

        var result = Transpose(bytes, 8, 10, out int bitsRemaining, out int remainder);

        if (bitsRemaining > 0)
        {
            result.Add(remainder);
        }

        var emojiRepresentation = result.Select(i => SpecialEmojis.Emojis[i]);

        if (bitsRemaining > 0)
        {
            emojiRepresentation = emojiRepresentation.Append(SpecialEmojis.Padding[10 - bitsRemaining]);
        }

        var emojiString = string.Join("", emojiRepresentation);

        if (options.Wrap != null)
        {
            return Wrap(emojiString, options.Wrap.Value);
        }

        return emojiString;
    }

    public static string Encode(string buffer, EncodeOptions options = default)
    {
        var bytes = options.Encoding.GetBytes(buffer).AsSpan();
        return Encode(bytes, options);
    }

    public static string DecodeString(string buffer, EncodeOptions options = default)
    {
        return options.Encoding.GetString(DecodeBinary(buffer));
    }

    public static string DecodeString(byte[] buffer, EncodeOptions options = default)
    {
        return options.Encoding.GetString(DecodeBinary(options.Encoding.GetString(buffer)));
    }

    public static byte[] DecodeBinary(byte[] buffer, EncodeOptions options = default)
    {
        return DecodeBinary(options.Encoding.GetString(buffer));
    }

    public static byte[] DecodeBinary(string buffer)
    {
        buffer = buffer.Trim().Replace("\n", "").Replace("\r", "");
        Rune.DecodeLastFromUtf16(buffer, out var paddingRune, out _);
        var padding = Array.IndexOf(SpecialEmojis.Padding, paddingRune);
        var withoutPaddingChar = padding != -1 ? buffer.AsSpan()[..^2] : buffer.AsSpan();
        var runes = new List<int>();
        while (Rune.DecodeFromUtf16(withoutPaddingChar, out var nextRune, out var consumed) == OperationStatus.Done)
        {
            withoutPaddingChar = withoutPaddingChar[consumed..];
            var runeIndex = Array.IndexOf(SpecialEmojis.Emojis, nextRune);
            if (runeIndex is -1)
            {
                throw new InvalidOperationException("Your input is invalid. Check Encoding! Offending rune: "+ nextRune);
            }
            runes.Add(runeIndex);
        }
        var transposed = Transpose(runes.ToArray(), 10, 8);
        if (padding >= 8)
        {
            transposed.RemoveAt(transposed.Count - 1);
        }
        return transposed.Select(x => (byte)x).ToArray();
    }

    private static List<int> Transpose(ReadOnlySpan<int> mTuples, int m, int n)
    {
        var nTuples = new List<int>();
        var nTuple = 0;
        var bitsFilled = 0;
        foreach (var mTuple in mTuples)
        {
            var bitsRemaininglocal = m;

            while (bitsRemaininglocal != 0)
            {
                var bitsTaken = Math.Min(n - bitsFilled, bitsRemaininglocal);
                int mask = ((1 << bitsTaken) - 1) << (bitsRemaininglocal - bitsTaken);
                var bits = (mTuple & mask) >> (bitsRemaininglocal - bitsTaken);
                var shift = n - bitsFilled - bitsTaken;
                nTuple |= bits << shift;
                bitsRemaininglocal -= bitsTaken;
                bitsFilled += bitsTaken;
                if (bitsFilled == n)
                {
                    nTuples.Add(nTuple);
                    bitsFilled = 0;
                    nTuple = 0;
                }
            }
        }
        return nTuples;
    }
    private static List<int> Transpose(ReadOnlySpan<byte> mTuples, int m, int n, out int bitsRemaining, out int remainder)
    {
        var nTuples = new List<int>();
        var nTuple = 0;
        var bitsFilled = 0;
        foreach (var mTuple in mTuples)
        {
            var bitsRemaininglocal = m;

            while (bitsRemaininglocal != 0)
            {
                var bitsTaken = Math.Min(n - bitsFilled, bitsRemaininglocal);
                int mask = ((1 << bitsTaken) - 1) << (bitsRemaininglocal - bitsTaken);
                var bits = (mTuple & mask) >> (bitsRemaininglocal - bitsTaken);
                var shift = n - bitsFilled - bitsTaken;
                nTuple |= bits << shift;
                bitsRemaininglocal -= bitsTaken;
                bitsFilled += bitsTaken;
                if (bitsFilled == n)
                {
                    nTuples.Add(nTuple);
                    bitsFilled = 0;
                    nTuple = 0;
                }
            }
        }

        remainder = nTuple;
        bitsRemaining = bitsFilled;
        return nTuples;
    }

    private static string Wrap(string emojiString, int wrap)
    {
        return System.Text.RegularExpressions.Regex.Replace(emojiString, $".{{{wrap}}}(?!$)", "$0\r\n");
    }

    public struct EncodeOptions
    {
        public EncodeOptions()
        {
            Armor = false;
            ArmorDescriptor = null;
            Wrap = null;
            Encoding = Encoding.Default;
        }
        public EncodeOptions(bool armor, string armorDescriptor, int? wrap, Encoding encoding)
        {
            Armor = armor;
            ArmorDescriptor = armorDescriptor;
            Wrap = wrap;
            Encoding = encoding;
        }
        public Encoding Encoding;
        public bool Armor;
        public string ArmorDescriptor;
        public int? Wrap;
    }
}
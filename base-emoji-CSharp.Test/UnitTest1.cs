using NUnit.Framework;

namespace base_emoji_CSharp.Test;

using System;
using System.Linq;

public class Tests
{
    
    // [Test]
    // public void TestTranspose()
    // {
    //     var arr = new byte[] { 1, 2, 3, 4, 5 };
    //     var encode = BaseEmoji.Transpose(arr, 8, 10, out var remaining, out var remainder);
    //     var decoded =BaseEmoji.Transpose(encode.Select(x => (byte) x).ToArray(), 10, 8, out var remaining2, out var remainder2);
    //     for (int index = 0; index < arr.Length; index++)
    //     {
    //         Assert.AreEqual(arr[index], decoded[index], "At index: {0}", index);
    //     }
    // }

    [Test]
    public void TestBinaryDecode()
    {
        var arr = new byte[] { 1, 2, 3, 4, 5 };
        var encode = BaseEmoji.Encode(arr);
        var decodeBinary = BaseEmoji.DecodeBinary(encode);
     
        for (int index = 0; index < arr.Length; index++)
        {
            Assert.AreEqual(arr[index], decodeBinary[index], "At index: {0}", index);
        }
        Assert.AreEqual(arr.Length, decodeBinary.Length);
    }
    
    [Test]
    public void TestEqualityToOriginal()
    {
        const string helloWorld = "🐅🚓📿🙉🤍🐝🕎🚥🌿🥄🕓";
        const string hi = "🍒🌒😟🕕";
        const string hidecoded = "hi!";
        const string baseemoji = "🌱🗾🤵🐷🍇🥅🔝🚏";
        {
            string encoded = BaseEmoji.Encode(hidecoded);
            Assert.AreEqual(hi, encoded);
            Assert.AreEqual(hidecoded, BaseEmoji.DecodeString(encoded));
        }
        {
            string encoded = BaseEmoji.Encode("base-emoji");
            Assert.AreEqual(baseemoji, encoded);
        }
        {
            string encoded = BaseEmoji.Encode("Hello World!");
            Assert.AreEqual(helloWorld, encoded);
        }
    }
    
    [Test]
    public void TestWrap()
    {
        const string helloWorld = @"🐅
🚓
📿
🙉
🤍
🐝
🕎
🚥
🌿
🥄
🕓";
        string encoded = BaseEmoji.Encode("Hello World!", new BaseEmoji.EncodeOptions{Wrap = 2});
        Assert.AreEqual(helloWorld, encoded);
    }
}
using System;
using System.IO;
using System.Text;
using base_emoji_CSharp;
int output =  Array.IndexOf(args, "-o");
int input = Array.IndexOf(args, "-i");
if (args.Length == 0 || Array.IndexOf(args, "-h") != -1 || Array.IndexOf(args, "--help") != -1 || output == -1 || input == -1)
{
    Console.WriteLine(@"Help for baseEmoji:
Since neither powershell nor cmd allow for emojis:
you need to pass -o for an output file
and -i for an input file

encode files:
baseEmoji -o <somePath> -i <somePath>

use -d to decode:
baseEmoji -d -o <somePath> -i <somePath>

-o and -i may be placed where-ever
");
    return;
}

int decode = Array.IndexOf(args, "-d");
var outputFi = new FileInfo(args[output + 1]);
await using var write = outputFi.Create();
var inputFi = new FileInfo(args[input + 1]);
await using var read = inputFi.OpenRead();
byte[] mem = new byte[inputFi.Length];
if (await read.ReadAsync(mem) != inputFi.Length)
{
    throw new IOException();
}
if (decode != -1)
{
    await write.WriteAsync(BaseEmoji.DecodeBinary(mem));
    return;
}
await write.WriteAsync(Encoding.UTF8.GetBytes(BaseEmoji.Encode(mem)));
return;

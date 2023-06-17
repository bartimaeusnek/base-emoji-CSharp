# 👪🗾🤵🐯 Base Emoji - C# Edition 🦧🥅🔝🚏

There is base32, there is base64, now there is base-emoji!

## How does it differ from [oktupol/base-emoji](https://github.com/oktupol/base-emoji)?
1. Its written in C#, therefore usable in .NET
2. It does not support armor yet.

## Are the base-emoji-encoded files/texts compatible?
Currently (as of the latest commit to this repo), yes, if there is no armor.

## How to use

The intended use for this is as a library. HOWEVER you can build it with [bflat](https://flattened.net/) 
as a console application. The default implementation of the main method favors windows, therefore uses files for en- and de-code base-emoji ↔ UTF8

## How does it work

The principle is identical to that of base64. In base64, data bits are
rearranged from their original 8-tuple bytes into 6-tuples, of which there are
64, and each of these 6-tuples is then represented with one ascii character.

```
bytes  |    104 = h    |    105 = i    |     33 = !    | ...
DATA   |0 1 1 0 1 0.0 0'0 1 1 0.1 0 0 1'0 0.1 0 0 0 0 1| ...
base64 |   26 = a  |    6 = G  |   36 = k  |   33 = h  | ...
```

Therefore, the base64 representation of `hi!` is `aGkh`.

In base-emoji, 1024 different symbols are used for representing 10-tuples.

```
bytes      |    104 = h    |    105 = i    |     33 = !    |              ...
DATA       |0 1 1 0 1 0 0 0'0 1.1 0 1 0 0 1'0 0 1 0.0 0 0 1'0 0 0 0 0 0.0 ...
base-emoji |      417 = 🍒     |      658 = 🌒     |       64 = 😟     |  ...
```

### Padding

Since 10 quite obviously doesn't divide evenly into 8, base-emoji-encoded data
contains a few bits more of information at the end than the original data. In
case of above example, the base-emoji encoded representation of the string
`hi!` has 6 bits of information overhanging. This is important to know
especially once there are is an overhang of 8 bits, because then it would
otherwise be ambiguous whether the last 8 bits are a byte of the original
information or not.

To indicate the length of the overhang, following symbols are appended to the
end of the base-emoji encoded string:

| Padding character | 🕛 | 🕐 | 🕑 | 🕒 | 🕓 | 🕔 | 🕕 | 🕖 | 🕗 | 🕘 |
|-------------------|----|----|----|----|----|----|----|----|----|----|
| Bits of overhang  |  0 |  1 |  2 |  3 |  4 |  5 |  6 |  7 |  8 |  9 |

Whereas the padding character for 0 bits of overhang is optional, and the
characters for 1, 3, 5, 7 and 9 bits can't realistically occur.

In above example, there are six bits of overhang, meaning the emoji
representation receives the padding character 🕕. Hence, the full base-emoji
representation of `hi!` is 🍒🌒😟🕕.

### Efficiency

All that being said, base-emoji is _horribly_ inefficient at encoding data.

In base64, where every 6-tuple of bits is encoded in one ascii character of one
byte, the encoded data size is 4/3 times the original data size, i.e. around
33.3% larger.

In base-emoji, we use 1024 symbols to encode 10-tuples, however, these 1024
symbols are _Unicode!_ An exact number can't be given due to unicode characters
being of variable size, but a quick test with 1000 random bytes showed a
threefold increase.

```
head -c 1000 /dev/urandom | base64 | wc -c
==> 1354

head -c 1000 /dev/urandom | base32 | wc -c
==> 1622

head -c 1000 /dev/urandom | base-emoji | wc -c
==> about 3175
```
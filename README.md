## Neliva.DataFormat

This repository provides efficient encoding and decoding of data as lowercase hexadecimal and a modified Base32 (base32hex–derived) format.
Both the specification and the reference implementation are released into the public domain. See the [UNLICENSE](UNLICENSE.md) file for details.

[![main](https://github.com/neliva/Neliva.DataFormat/actions/workflows/main.yml/badge.svg)](https://github.com/neliva/Neliva.DataFormat/actions/workflows/main.yml)
[![.NET 8.0](https://img.shields.io/badge/dotnet-8.0-green)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
[![NuGet (with prereleases)](https://img.shields.io/nuget/vpre/Neliva.DataFormat)](https://www.nuget.org/packages/Neliva.DataFormat)

## Overview

Encoders produce lowercase output suitable for **naming files and cloud storage objects**.
Decoders accept both lowercase and uppercase input.

Output is deterministic, lowercase, and unpadded.

The custom Base32 alphabet `0123456789abcdefghjkmnpqrstvwxyz` omits `I, L, O, U` letters.

### Usage

```C#
// using Neliva;

string dataHex = DataFormat.ToHex(new byte[] { 1, 2, 3 });
bool isHex = DataFormat.IsHex(dataHex);
byte[] bytesFromHex = DataFormat.FromHex(dataHex);

string guidHex = DataFormat.ToHexGuid(Guid.NewGuid());
Guid guidFromHex = DataFormat.FromHexGuid(guidHex);

string dataBase32 = DataFormat.ToBase32(new byte[] { 1, 2, 3 });
bool isBase32 = DataFormat.IsBase32(dataBase32);
byte[] bytesFromBase32 = DataFormat.FromBase32(dataBase32);

string guidBase32 = DataFormat.ToBase32Guid(Guid.NewGuid());
Guid guidFromBase32 = DataFormat.FromBase32Guid(guidBase32);
```

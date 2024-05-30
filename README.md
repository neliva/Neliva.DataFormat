## Neliva.DataFormat

This repository provides support to efficiently encode and decode data in hex and modified base32hex formats.

[![main](https://github.com/neliva/Neliva.DataFormat/actions/workflows/main.yml/badge.svg)](https://github.com/neliva/Neliva.DataFormat/actions/workflows/main.yml)
[![dotnet 6.0](https://img.shields.io/badge/dotnet-6.0-green)](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Neliva.DataFormat)](https://www.nuget.org/packages/Neliva.DataFormat)

## Overview

Encoder uses lowercase alphabet suitable for **naming files and cloud objects**. Decoder understands lower and uppercase characters.

The custom base32 utilizes the `0123456789abcdefghjkmnpqrstvwxyz` alphabet that omits the `I L O U` letters.

### Usage
```C#
// using Neliva;

string dataHex = DataFormat.ToHex(new byte[] { 1, 2, 3 });
byte[] bytesFromHex = DataFormat.FromHex(dataHex);

string guidHex = DataFormat.ToHexGuid(Guid.NewGuid());
Guid guidFromHex = DataFormat.FromHexGuid(guidHex);

string dataBase32 = DataFormat.ToBase32(new byte[] { 1, 2, 3 });
byte[] bytesFromBase32 = DataFormat.FromBase32(dataBase32);

string guidBase32 = DataFormat.ToBase32Guid(Guid.NewGuid());
Guid guidFromBase32 = DataFormat.FromBase32Guid(guidBase32);
```

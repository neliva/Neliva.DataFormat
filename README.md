## Neliva.DataFormat

This repository provides support to efficiently encode and decode data in hex and base32.

[![main](https://github.com/neliva/Neliva.DataFormat/actions/workflows/main.yml/badge.svg)](https://github.com/neliva/Neliva.DataFormat/actions/workflows/main.yml)
[![dotnet 6.0](https://img.shields.io/badge/dotnet-6.0-green)](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Neliva.DataFormat)](https://www.nuget.org/packages/Neliva.DataFormat)

## Overview

Encoder uses lowercase alphabet suitable for **naming files and cloud objects**. Decoder understands lower and uppercase characters.

The custom base32 utilizes the `0123456789abcdefghjkmnpqrstvwxyz` alphabet that omits the `I L O U` letters.

### Usage
```C#
// using Neliva;

var dataBytes = new byte[] { 1, 2, 3 };

// Encode to lowercase
string dataHex = DataFormat.ToHex(dataBytes);
string dataBase32 = DataFormat.ToBase32(dataBytes);

// Decode from lower or uppercase
var dataBytes2 = DataFormat.FromHex(dataHex);
var dataBytes3 = DataFormat.FromBase32(dataBase32);
```

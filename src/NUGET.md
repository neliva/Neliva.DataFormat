# Neliva.DataFormat

Efficient encoding and decoding of data as lowercase hexadecimal and a modified Base32 format.

## Features

- **Hex encoding/decoding** - Convert byte spans and GUIDs to/from hexadecimal strings
- **Base32 encoding/decoding** - Convert byte spans and GUIDs to/from Base32 strings
- **Lowercase output** - Suitable for naming files and cloud storage objects
- **Case-insensitive decoding** - Accepts both lowercase and uppercase input
- **Unpadded output** - Deterministic and compact representation
- **Custom Base32 alphabet** - Uses `0123456789abcdefghjkmnpqrstvwxyz` (omits `I, L, O, U` to avoid ambiguity)

## Usage

```csharp
// using Neliva;

// Hex encoding/decoding
string dataHex = DataFormat.ToHex(new byte[] { 1, 2, 3 });
bool isHex = DataFormat.IsHex(dataHex);
byte[] bytesFromHex = DataFormat.FromHex(dataHex);

// GUID hex encoding/decoding
string guidHex = DataFormat.ToHexGuid(Guid.NewGuid());
Guid guidFromHex = DataFormat.FromHexGuid(guidHex);

// Base32 encoding/decoding
string dataBase32 = DataFormat.ToBase32(new byte[] { 1, 2, 3 });
bool isBase32 = DataFormat.IsBase32(dataBase32);
byte[] bytesFromBase32 = DataFormat.FromBase32(dataBase32);

// GUID Base32 encoding/decoding
string guidBase32 = DataFormat.ToBase32Guid(Guid.NewGuid());
Guid guidFromBase32 = DataFormat.FromBase32Guid(guidBase32);
```
using MoorTools.TLV.Models;

namespace MoorTools.TLV.UnitTests.Data;

internal static class TlvData
{
    internal const string HexTlvFirst = "6F1A840E315041592E5359532E4444463031A5088801025F2D02656E";

    internal static readonly byte[] BytesTlvFirst =
    [
        0x6F, 0x1A, 0x84, 0x0E, 0x31, 0x50, 0x41, 0x59, 0x2E, 0x53, 0x59, 0x53, 0x2E, 0x44, 0x44, 0x46, 0x30, 0x31,
        0xA5, 0x08, 0x88, 0x01, 0x02, 0x5F, 0x2D, 0x02, 0x65, 0x6E
    ];

    internal static readonly ParsedTlv ExceptedFirst = new([
        new Tlv
        {
            Tag = 0x6F,
            Length = 26,
            IsConstructed = true,
            Data = Convert.FromHexString("840E315041592E5359532E4444463031A5088801025F2D02656E"),
            Children =
            [
                new Tlv
                {
                    Tag = 0x84,
                    Length = 14,
                    IsConstructed = false,
                    Data = Convert.FromHexString("315041592E5359532E4444463031")
                },
                new Tlv
                {
                    Tag = 0xA5,
                    Length = 8,
                    IsConstructed = true,
                    Data = Convert.FromHexString("8801025F2D02656E"),
                    Children =
                    [
                        new Tlv
                        {
                            Tag = 0x88,
                            Length = 1,
                            IsConstructed = false,
                            Data = Convert.FromHexString("02")
                        },
                        new Tlv
                        {
                            Tag = 0x5F2D,
                            Length = 2,
                            IsConstructed = false,
                            Data = Convert.FromHexString("656E"),
                        }
                    ]
                }
            ]
        }
    ]);
}
using System;
using Kaleido.Core;
using Xunit;

namespace Core.Tests
{
    public class CursorCodecTests
    {
        [Fact]
        public void EncodeDecode_Roundtrip()
        {
            var encoded = CursorCodec.EncodeOffset(5);
            Assert.False(string.IsNullOrWhiteSpace(encoded));
            var decoded = CursorCodec.DecodeOffset(encoded);
            Assert.Equal(5, decoded);
        }

        [Fact]
        public void Decode_NullOrWhitespace_ReturnsZero()
        {
            Assert.Equal(0, CursorCodec.DecodeOffset(null));
            Assert.Equal(0, CursorCodec.DecodeOffset(" "));
        }

        [Fact]
        public void Decode_Invalid_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() => CursorCodec.DecodeOffset("not-base64"));
        }
    }
}

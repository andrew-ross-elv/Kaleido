using System.Text;

namespace Kaleido.Core;

public static class CursorCodec
{
    public static string EncodeOffset(int offset) => 
        Convert.ToBase64String(Encoding.UTF8.GetBytes(offset.ToString()));

    public static int DecodeOffset(string? cursor)
    {
        if (string.IsNullOrWhiteSpace(cursor)) return 0;
        return int.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(cursor)));
    }
}

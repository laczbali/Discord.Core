using System;

namespace Discord.Core.Utils.ED25519.Ed25519Ref10
{
    internal static partial class ScalarOperations
    {
        internal static void sc_clamp(byte[] s, int offset)
        {
            s[offset + 0] &= 248;
            s[offset + 31] &= 127;
            s[offset + 31] |= 64;
        }
    }
}

#if !NET5_0_OR_GREATER
using System.Runtime.InteropServices;

namespace GBX.NET;

internal static class HalfUtility
{
    [StructLayout(LayoutKind.Explicit)]
    private struct IntFloatUnion
    {
        [FieldOffset(0)]
        public int IntValue;

        [FieldOffset(0)]
        public float FloatValue;
    }

    public static float HalfToFloat(ushort h)
    {
        int s = (h >> 15) & 0x00000001;
        int e = (h >> 10) & 0x0000001F;
        int m = h & 0x000003FF;

        if (e == 0)
        {
            if (m == 0)
            {
                // Plus or minus zero
                int zeroInt = s << 31;
                return new IntFloatUnion { IntValue = zeroInt }.FloatValue;
            }

            // Denormalized number - renormalize it for 32-bit float
            while ((m & 0x00000400) == 0)
            {
                m <<= 1;
                e -= 1;
            }
            e += 1;
            m &= ~0x00000400;
        }
        else if (e == 31)
        {
            // Infinity or NaN
            int infNanInt = (s << 31) | 0x7F800000 | (m << 13);
            return new IntFloatUnion { IntValue = infNanInt }.FloatValue;
        }

        // Normalized number
        e += 127 - 15;
        m <<= 13;

        int resultInt = (s << 31) | (e << 23) | m;
        return new IntFloatUnion { IntValue = resultInt }.FloatValue;
    }

    public static ushort FloatToHalf(float f)
    {
        // Get the raw 32-bit integer representation of the float using the union (Zero allocation)
        int i = new IntFloatUnion { FloatValue = f }.IntValue;

        int s = (i >> 16) & 0x00008000;                     // Sign
        int e = ((i >> 23) & 0x000000FF) - (127 - 15);      // Exponent
        int m = i & 0x007FFFFF;                             // Mantissa

        if (e <= 0)
        {
            if (e < -10) return (ushort)s; // Underflow to zero

            // Denormalized number
            m = (m | 0x00800000) >> (1 - e);
            return (ushort)(s | (m >> 13));
        }
        else if (e == 0xFF - (127 - 15))
        {
            if (m == 0) return (ushort)(s | 0x7C00); // Infinity

            // NaN
            m >>= 13;
            return (ushort)(s | 0x7C00 | m | (m == 0 ? 1 : 0));
        }

        if (e > 30)
        {
            return (ushort)(s | 0x7C00); // Overflow to Infinity
        }

        return (ushort)(s | (e << 10) | (m >> 13));
    }
}
#endif
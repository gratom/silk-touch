using System;
using UnityEngine;

namespace Tools
{
    public static class Hash128Utils
    {
        public static Hash128 CombineXor(this Hash128[] hashes)
        {
            if (hashes == null || hashes.Length == 0)
            {
                return new Hash128();
            }

            ulong a = 0;
            ulong b = 0;

            for (int i = 0; i < hashes.Length; i++)
            {
                hashes[i].GetHashParts(out ulong ha, out ulong hb);
                a ^= ha;
                b ^= hb;
            }

            return new Hash128(a, b);
        }

        public static Hash128 CombineWith(this Hash128 origin, Hash128 other)
        {
            origin.GetHashParts(out ulong a, out ulong b);
            other.GetHashParts(out ulong ha, out ulong hb);
            a ^= ha;
            b ^= hb;
            return new Hash128(a, b);
        }

        private static void GetHashParts(this Hash128 hash, out ulong low, out ulong high)
        {
            unsafe
            {
                ulong* ptr = (ulong*)&hash;
                low = ptr[0];
                high = ptr[1];
            }
        }
    }
}
﻿using System.Collections;

namespace Nevermind.Core.Encoding
{
    public class Bloom
    {
        /// <summary>
        /// https://stackoverflow.com/questions/560123/convert-from-bitarray-to-byte
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        private static byte[] BitArrayToByteArray(BitArray bits)
        {
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }

        private readonly BitArray _bits = new BitArray(2048);

        public byte[] Bytes => BitArrayToByteArray(_bits);

        public void Set(byte[] sequence)
        {
            Keccak keccak = Keccak.Compute(sequence);
            for (int i = 0; i < 6; i += 2)
            {
                int index = (keccak.Bytes[i] + keccak.Bytes[i + 1]) % 2048;
                _bits.Set(index, true);
            }
        }
    }
}
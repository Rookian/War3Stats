using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WC3Stats
{
    public static class Extensions
    {
        public static byte[] HexToString(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }

        public static IEnumerable<int> GetIndexes(this byte[] source, byte[] pattern)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern)) yield return i;
            }
        }

        public static byte[] TakeFromIndexWhileNotZeroByte(this byte[] bytes, int index)
        {
            var destinationBytes = new byte[bytes.Length - index];

            Array.Copy(bytes, index, destinationBytes, 0, destinationBytes.Length);
            return destinationBytes.TakeWhile(x => x != 0x00).ToArray();
        }

        public static string AsString(this byte[] bytes, int? from = null)
        {
            if (@from.HasValue)
                return Encoding.UTF8.GetString(bytes, @from.Value, bytes.Length - @from.Value);

            return Encoding.UTF8.GetString(bytes);
        }

        public static byte[] StringToByteArray(this string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}
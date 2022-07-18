using System;
using System.Collections.Generic;
using System.Linq;

namespace ExamUniverse.Converter.VCE.Utilits
{
    /// <summary>
    ///     Separator
    /// </summary>
    public class Separator
    {
        private byte[] _bytes;
        private byte[][] _splitters;

        public Separator(byte[] data, byte[] splitter)
        {
            _bytes = data;
            _splitters = new byte[][] { splitter };
        }

        public Separator(byte[] data, byte[][] splitters)
        {
            _bytes = data;
            _splitters = splitters;
        }

        /// <summary>
        ///     Has value
        /// </summary>
        public bool HasValue => _bytes.Length > 0;

        /// <summary>
        ///     Read int 32
        /// </summary>
        /// <returns></returns>
        public int ReadInt32()
        {
            byte[] bytes = ReadBytes(4);
            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        ///     Read bytes
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] ReadBytes(int length)
        {
            byte[] bytes = _bytes.Take(length).ToArray();
            _bytes = _bytes.Skip(bytes.Length).ToArray();

            return bytes;
        }

        /// <summary>
        ///     Push
        /// </summary>
        /// <param name="bytes"></param>
        public void Push(byte[] bytes)
        {
            List<byte> data = new List<byte>();

            data.AddRange(_bytes);
            data.AddRange(bytes);

            _bytes = data.ToArray();
        }

        /// <summary>
        ///     Pop
        /// </summary>
        /// <returns></returns>
        public byte[] Pop()
        {
            byte[] data = GetPatternBytes();
            _bytes = _bytes.Skip(data.Length).ToArray();

            return data;
        }

        /// <summary>
        ///     Peek
        /// </summary>
        /// <returns></returns>
        public byte[] Peek()
        {
            return GetPatternBytes();
        }

        /// <summary>
        ///     Get pattern bytes
        /// </summary>
        /// <returns></returns>
        private byte[] GetPatternBytes()
        {
            for (int i = 0; i < _bytes.Length; i++)
            {
                for (int j = 0; j < _splitters.Length; j++)
                {
                    if (IsMatch(_bytes, i, _splitters[j]))
                    {
                        int count = i != 0 ? i : _splitters[j].Length;
                        return _bytes.Take(count).ToArray();
                    }
                }
            }

            return _bytes.Take(_bytes.Length).ToArray();
        }

        /// <summary>
        ///     Is match
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="position"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private bool IsMatch(byte[] bytes, int position, byte[] pattern)
        {
            if (pattern.Length > (bytes.Length - position))
            {
                return false;
            }

            for (int i = 0; i < pattern.Length; i++)
            {
                if (bytes[position + i] != pattern[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}

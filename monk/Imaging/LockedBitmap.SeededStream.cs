// =====
//
// Copyright (c) 2013-2020 Timothy Baxendale
//
// =====
using System;
using System.Collections.Generic;
using System.IO;

using Monk.Memory;

namespace Monk.Imaging
{
    public partial class LockedBitmap
    {
        public ByteStream GetStream(int pixelIndex, IEnumerable<ushort> seed)
        {
            return GetStream(pixelIndex, seed, SupportedColors);
        }

        public ByteStream GetStream(int pixelIndex, PixelColor colors)
        {
            return GetStream(pixelIndex, new ushort[] { 0 }, colors);
        }

        public ByteStream GetStream(int pixelIndex, IEnumerable<ushort> seed, PixelColor colors)
        {
            return GetStream(pixelIndex, Size - pixelIndex, seed, colors);
        }

        public ByteStream GetStream(int pixelIndex, int pixelCount, IEnumerable<ushort> seed, PixelColor colors)
        {
            if (pixelIndex < 0 || pixelIndex >= Size) throw new ArgumentOutOfRangeException(nameof(pixelIndex));
            if ((SupportedColors & colors) != colors) ThrowHelper.ColorUnsupported(nameof(colors), colors, SupportedColors);
            if (colors == PixelColor.None) throw new ArgumentException("you must specify at least one color", nameof(colors));
            if (pixelCount < 1 || pixelIndex + pixelCount > Size) throw new ArgumentOutOfRangeException(nameof(pixelCount));
            return new SeededBitmapStream(this, seed, colors, pixelIndex, pixelCount);
        }

        private sealed class SeededBitmapStream : ByteStream
        {
            private int[] _indices;

            public LockedBitmap Bitmap { get; }

            public override int IntLength => _indices.Length;

            public SeededBitmapStream(LockedBitmap bitmap, IEnumerable<ushort> seed, PixelColor colors, int pixelIndex, int pixelCount)
            {
                Bitmap = bitmap;
                bitmap.LockBits();
                CachePixels(bitmap, seed, colors, pixelIndex, pixelCount);
            }

            private void CachePixels(LockedBitmap bitmap, IEnumerable<ushort> seed, PixelColor colors, int pixelIndex, int pixelCount)
            {
                int imageArea = bitmap.Size;

                _indices = new int[Math.Min(pixelCount, imageArea) * colors.Count];

                ArithmeticProgression pixelIndices = new ArithmeticProgression(pixelIndex, seed);

                for (int buffIdx = 0; pixelIndex < imageArea && buffIdx < _indices.Length; pixelIndex = pixelIndices.Next()) {
                    foreach (PixelColor color in colors.SeparateColors()) {
                        _indices[buffIdx++] = bitmap.GetBufferIndex(pixelIndex, color);
                        if (buffIdx >= _indices.Length) break;
                    }
                }
            }

            public override byte Peek()
            {
                if (IntPosition >= IntLength) throw new EndOfStreamException();
                return Bitmap.GetByteAt(_indices[IntPosition]);
            }

            public override int ReadByte()
            {
                if (IntPosition >= IntLength) return -1;
                return Bitmap.GetByteAt(_indices[IntPosition++]);
            }

            public override byte ReadNext()
            {
                if (IntPosition >= IntLength) throw new EndOfStreamException();
                return Bitmap.GetByteAt(_indices[IntPosition++]);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (IntPosition >= IntLength) return -1;
                count = Math.Min(count, IntLength - IntPosition);
                for (int i = 0; i < count; ++i) {
                    buffer[i] = Bitmap.GetByteAt(_indices[IntPosition++]);
                }
                return count;
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void WriteByte(byte value)
            {
                if (IntPosition >= IntLength) throw new EndOfStreamException();
                Bitmap.SetByteAt(_indices[IntPosition++], value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                if (count + IntPosition > IntLength) throw new EndOfStreamException();
                count = Math.Min(count, IntLength - IntPosition);
                for (int i = 0; i < count; ++i) {
                    Bitmap.SetByteAt(_indices[IntPosition++], buffer[i]);
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing && Bitmap.Locked) {
                    Flush();
                    Bitmap.UnlockBits();
                    _indices = null;
                }
                base.Dispose(disposing);
            }
        }
    }
}

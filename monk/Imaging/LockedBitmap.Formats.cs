// =====
//
// Copyright (c) 2013-2020 Timothy Baxendale
//
// =====
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Monk.Collections.Immutable;

namespace Monk.Imaging
{
    public partial class LockedBitmap
    {
        private class LockedBitmap32Bpp : LockedBitmap
        {
            public override int Depth => 32;

            public override PixelColor SupportedColors { get; } = PixelColor.ARGB;

            public LockedBitmap32Bpp(Bitmap bitmap)
            {
                Bitmap = bitmap;
                Width  = bitmap.Width;
                Height = bitmap.Height;
            }

            public override unsafe int GetPixel(int pixelOffset)
            {
                return Marshal.ReadInt32(new IntPtr(PixelAt(pixelOffset)));
            }

            public override unsafe void SetPixel(int pixelOffset, int argb)
            {
                Marshal.WriteInt32(new IntPtr(PixelAt(pixelOffset)), argb);
            }

            internal override int GetBufferIndex(int pixelIndex, PixelColor color)
            {
                return PixelOffsetToByteOffset(pixelIndex) + color.Order;
            }
        }

        private class LockedBitmap24Bpp : LockedBitmap
        {
            public override int Depth => 24;
            public override PixelColor SupportedColors { get; } = PixelColor.RGB;

            public LockedBitmap24Bpp(Bitmap bitmap)
            {
                Bitmap = bitmap;
                Width  = bitmap.Width;
                Height = bitmap.Height;
            }

            public override unsafe int GetPixel(int pixelOffset)
            {
                byte* pixel = PixelAt(pixelOffset);
                int value = 0xff << PixelColor.Alpha.Shift;
                value |= pixel[2] << PixelColor.Red.Shift;
                value |= pixel[1] << PixelColor.Green.Shift;
                value |= pixel[0] << PixelColor.Blue.Shift;
                return value;
            }

            public override unsafe void SetPixel(int pixelOffset, int argb)
            {
                byte* pixel = PixelAt(pixelOffset);
                pixel[0] = (byte)((argb >> PixelColor.Blue.Shift) & 0xff);
                pixel[1] = (byte)((argb >> PixelColor.Green.Shift) & 0xff);
                pixel[2] = (byte)((argb >> PixelColor.Red.Shift) & 0xff);
            }

            internal override int GetBufferIndex(int pixelIndex, PixelColor color)
            {
                if (color == PixelColor.Alpha) ThrowHelper.ColorUnsupported(nameof(color), color);
                return PixelOffsetToByteOffset(pixelIndex) + color.Order;
            }
        }

        private class LockedBitmap8Bpp : LockedBitmap
        {
            public override int Depth => 8;
            public override PixelColor SupportedColors { get; } = PixelColor.Blue;

            public LockedBitmap8Bpp(Bitmap bitmap)
            {
                Bitmap = bitmap;
                Width  = bitmap.Width;
                Height = bitmap.Height;
            }

            public override unsafe int GetPixel(int pixelOffset)
            {
                return PixelAt(pixelOffset)[0];
            }

            public override unsafe void SetPixel(int pixelOffset, int argb)
            {
                PixelAt(pixelOffset)[0] = (byte)argb;
            }

            internal override int GetBufferIndex(int pixelIndex, PixelColor color)
            {
                if (color != PixelColor.Blue) ThrowHelper.ColorUnsupported(nameof(color), color);
                return PixelOffsetToByteOffset(pixelIndex);
            }
        }
    }
}

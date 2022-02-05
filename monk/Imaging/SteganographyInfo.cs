// =====
//
// Copyright (c) 2013-2020 Timothy Baxendale
//
// =====
using System;
using System.Collections.Generic;

using Monk.Memory.Bittwiddling;

namespace Monk.Imaging
{
    public class SteganographyInfo
    {
        public static SteganographyInfo PresetsA0001 => new SteganographyInfo()
        {
            Endianness           = EndianMode.BigEndian,
            LeastSignificantBits = 1,
            ZeroBasedSize        = true,
            InvertPrefixBits     = true,
            InvertDataBits       = true,
            Offset               = 0
        };

        public static SteganographyInfo PresetsB0003 => new SteganographyInfo()
        {
            Endianness           = EndianMode.BigEndian,
            LeastSignificantBits = 1,
            ZeroBasedSize        = true,
            InvertPrefixBits     = false,
            InvertDataBits       = false,
            Offset               = 0
        };

        private int _lsb = 1;
        public int LeastSignificantBits
        {
            get => _lsb;
            set {
                if (value < 1 || value > 4) throw new ArgumentOutOfRangeException(nameof(value),"Least significant bits must be between 1 and 4");
                _lsb = value;
            }
        }

        private IList<ushort> _seed = new ushort[] { 0 };
        public IList<ushort> Seed
        {
            get => _seed;
            set {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (value.Count == 0) throw new ArgumentException(nameof(value));
                _seed = value;
            }
        }

        public bool InvertPrefixBits { get; set; } = false;
        public bool InvertDataBits { get; set; } = false;
        public bool ZeroBasedSize { get; set; } = false;
        public EndianMode Endianness { get; set; } = EndianMode.LittleEndian;
        public PixelColor Colors { get; set; } = PixelColor.ARGB;
        public int Offset { get; set; } = 0;
    }
}

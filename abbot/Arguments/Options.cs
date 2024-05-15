// =====
//
// Copyright (c) 2013-2020 Timothy Baxendale
//
// =====
using CommandLine;

using Monk.Imaging;
using Monk.Memory.Bittwiddling;

namespace Abbot.Arguments
{
    internal class Options
    {
        [Option("image", Required = true, HelpText = "path to the image this is being encoded/decoded")]
        public string Path { get; set; }

        [Option("seed", Required = false, HelpText = "encoding seed")]
        public uint Seed { get; set; } = 0;

        [Option("key", Required = false, HelpText = "encryption key for message or file")]
        public string Key { get; set; } = string.Empty;

        [Option('i', "invert", Required = false, HelpText = "invert bits when being encoded", Default = false)]
        public bool Invert { get; set; } = false;

        [Option('a', "alpha", Required = false, HelpText = "Use the alpha value of the pixels to encode/decode")]
        public bool Alpha { get; set; }

        [Option('r', "red", Required = false, HelpText = "Use the red value of the pixels to encode/decode")]
        public bool Red { get; set; }

        [Option('g', "green", Required = false, HelpText = "Use the green value of the pixels to encode/decode")]
        public bool Green { get; set; }

        [Option('b', "blue", Required = false, HelpText = "Use the blue value of the pixels to encode/decode")]
        public bool Blue { get; set; }

        [Option('l', "lsb", Required = false, HelpText = "the least significant bits to use", Default = 1)]
        public int LeastSignificantBits { get; set; } = 1;

        [Option("order", Required = false, HelpText = "the order the bits for each byte are encoded", Default = EndianMode.LittleEndian)]
        public EndianMode Order { get; set; }

        [Option("offset", Required = false, HelpText = "Start index of the pixel to begin reading or writing", Default = 0)]
        public int Offset { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Show more detailed information", Default = false)]
        public bool Verbose { get; set; }

        public virtual SteganographyInfo BuildTrithemius()
        {
            SteganographyInfo trithemius = new SteganographyInfo()
            {
                LeastSignificantBits = LeastSignificantBits,
                InvertDataBits       = Invert,
                InvertPrefixBits     = Invert,
                Endianness           = Order,
                Offset               = Offset
            };

            trithemius.Colors = PixelColor.None;
            if (Alpha) trithemius.Colors |= PixelColor.Alpha;
            if (Red) trithemius.Colors |= PixelColor.Red;
            if (Green) trithemius.Colors |= PixelColor.Green;
            if (Blue) trithemius.Colors |= PixelColor.Blue;

            return trithemius;
        }
    }
}

// =====
//
// Copyright (c) 2013-2021 Timothy Baxendale
//
// =====
using System;
using System.Collections.Generic;
using System.Linq;

namespace Monk.Imaging
{
    public struct PixelColor : IEquatable<PixelColor>, IComparable<PixelColor>
    {
        public static readonly PixelColor None =  default(PixelColor);
        public static readonly PixelColor Blue  = new PixelColor("Blue" , 0x01, 0, 0x00);
        public static readonly PixelColor Green = new PixelColor("Green", 0x02, 1, 0x08);
        public static readonly PixelColor Red   = new PixelColor("Red"  , 0x04, 2, 0x10);
        public static readonly PixelColor Alpha = new PixelColor("Alpha", 0x08, 3, 0x18);

        public static readonly PixelColor ARGB  = Blue | Green | Red | Alpha;
        public static readonly PixelColor RGB   = Blue | Green | Red;

        public int Value { get; }
        public int Shift { get; }
        public int Order { get; }
        public string Name { get; }

        public int Count {
            get {
                int count = 0;
                if ((Value & Blue.Value) == Blue.Value) ++count;
                if ((Value & Green.Value) == Green.Value) ++count;
                if ((Value & Red.Value) == Red.Value) ++count;
                if ((Value & Alpha.Value) == Alpha.Value) ++count;
                return count;
            }
        }

        private PixelColor(string name, int value, int order, int shift)
        {
            Name = name;
            Value = value;
            Order = order;
            Shift = shift;
        }

        private PixelColor(int value)
            : this(null, value, value, 0)
        {
            Name = string.Join(",", SeparateColors().Select(c => c.Name));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if ((object)this == obj) return true;
            PixelColor? color = obj as PixelColor?;
            if (color == null)
                return false;
            return Equals(color.Value);
        }

        public bool Equals(PixelColor other)
        {
            return Value == other.Value;
        }

        public int CompareTo(PixelColor other)
        {
            return Order.CompareTo(other.Order);
        }

        public override int GetHashCode()
        {
            return Value;
        }

        public override string ToString()
        {
            return Name ?? "None";
        }

        public IEnumerable<PixelColor> SeparateColors()
        {
            if ((Value & Blue.Value) == Blue.Value) yield return Blue;
            if ((Value & Green.Value) == Green.Value) yield return Green;
            if ((Value & Red.Value) == Red.Value) yield return Red;
            if ((Value & Alpha.Value) == Alpha.Value) yield return Alpha;
        }

        public bool Contains(PixelColor color)
        {
            return (Value & color.Value) == color.Value;
        }

        public PixelColor Add(PixelColor color)
        {
            return GetColor((int)(this | color));
        }

        public PixelColor Remove(PixelColor color)
        {
            return GetColor((int)(this & ~color));
        }

        public static explicit operator int(PixelColor color)
        {
            return color.Value;
        }

        public static explicit operator PixelColor(int value)
        {
            return GetColor(value);
        }

        public static bool operator ==(PixelColor left, PixelColor right)
        {
            return left.Value == right.Value;
        }

        public static bool operator !=(PixelColor left, PixelColor right)
        {
            return left.Value != right.Value;
        }

        public static PixelColor operator ~(PixelColor color)
        {
            return GetColor(~color.Value);
        }

        public static PixelColor operator ^(PixelColor left, PixelColor right)
        {
            return GetColor(left.Value ^ right.Value);
        }

        public static PixelColor operator &(PixelColor left, PixelColor right)
        {
            return GetColor(left.Value & right.Value);
        }

        public static PixelColor operator |(PixelColor left, PixelColor right)
        {
            return GetColor(left.Value | right.Value);
        }

        private static PixelColor GetColor(int value)
        {
            if (Blue.Value == value) {
                return Blue;
            }
            else if (Green.Value == value) {
                return Green;
            }
            else if (Red.Value == value) {
                return Red;
            }
            else if (Alpha.Value == value) {
                return Alpha;
            }
            else if (ARGB.Value == value) {
                return ARGB;
            }
            else if (RGB.Value == value) {
                return RGB;
            }
            return new PixelColor(value);
        }
    }
}
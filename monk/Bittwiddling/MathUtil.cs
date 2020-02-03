﻿/**
*  Monk
*  Copyright (C) Timothy Baxendale
*
*  This program is free software; you can redistribute it and/or modify
*  it under the terms of the GNU General Public License as published by
*  the Free Software Foundation; either version 2 of the License, or
*  (at your option) any later version.
*
*  This program is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*  GNU General Public License for more details.
*
*  You should have received a copy of the GNU General Public License along
*  with this program; if not, write to the Free Software Foundation, Inc.,
*  51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
**/
using System;

namespace Monk.Bittwiddling
{
    internal class MathUtil
    {
        /// <summary>
        /// Performs integer division but rounds up
        /// </summary>
        public static int DivideUp(int dividend, int divisor)
        {
            return dividend / divisor + Math.Min(1, dividend % divisor);
        }

        /// <summary>
        /// Count the number of digits in an integer
        /// </summary>
        public static int CountDigits(long value)
        {
            return (int)Math.Floor(Math.Log10(value) + 1);
        }
    }
}

// =====
//
// Copyright (c) 2013-2020 Timothy Baxendale
//
// =====
using System;
using System.Runtime.Serialization;

namespace Monk.Imaging
{
    [Serializable]
    public class InvalidImageOptionException : ArgumentException
    {
        public InvalidImageOptionException()
        {
        }

        public InvalidImageOptionException(string message) : base(message)
        {
        }

        public InvalidImageOptionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public InvalidImageOptionException(string message, string paramName)
            : base(message, paramName)
        {
        }
    }
}

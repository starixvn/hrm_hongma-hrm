// --------------------------------------------------------------------------
// <copyright file="StringValueAttribute.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Core.Enums
{
    using System;

    /// <summary>
    /// StringValue Attribute.
    /// </summary>
    public class StringValueAttribute : Attribute
    {
        private readonly string _value;

        public StringValueAttribute(string value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}

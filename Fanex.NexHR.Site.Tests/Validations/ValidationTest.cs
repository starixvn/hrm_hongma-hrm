// --------------------------------------------------------------------------
// <copyright file="ValidationTest.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Site.Tests.Validations
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Validation Test.
    /// </summary>
    [TestClass]
    public class ValidationTest
    {
        [TestMethod]
        public void LeaveDayValidate()
        {
            Assert.AreNotEqual(0, 1.3 % 0.5);
            Assert.AreEqual(0, 1.5 % 0.5);
            Assert.AreEqual(0, 0 % 0.5);
            Assert.AreEqual(0, 2 % 0.5);
            Assert.AreNotEqual(0, 3.7 % 0.5);
        }
    }
}

// --------------------------------------------------------------------------
// <copyright file="EmployeeControllerTest.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Site.Tests.Controllers
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    /// <summary>
    /// Employee Controller Test.
    /// </summary>
    [TestClass]
    public class EmployeeControllerTest
    {
        private TestContext _testContextInstance;

        public EmployeeControllerTest()
        {
        }

        public TestContext TestContext
        {
            get
            {
                return this._testContextInstance;
            }

            set
            {
                this._testContextInstance = value;
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}

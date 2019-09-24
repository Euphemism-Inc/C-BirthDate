// (c) Euphemism Inc. All right reserved.

using Coconut.Library.Common.Extensions;
using Coconut.Library.Common.Tests.Extensions.TestResources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace Coconut.Library.Common.Tests.Extensions
{
    /// <summary>
    /// Tests for <see cref="ResourceExtensions"/>.
    /// </summary>
    [TestClass]
    public class ResourceExtensionsTests
    {
        private Assembly _assembly;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Type type = GetType();
            _assembly = type.Assembly;
        }

        /// <summary>
        /// Test what happens when an existing resource identifier is being accessed and if it returs the expected content..
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void ResourceExtensions_GetEmbeddedResourceContent_Succeeds()
        {
            // Act
            string resourceString = _assembly.GetEmbeddedResourceContent(ResourceKeys.ResourceExtensions);

            // Assert
            Assert.AreEqual("Sample Content.", resourceString);
        }

        /// <summary>
        /// Test what happens when an non existing resource identifier is being accessed.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void ResourceExtensions_GetEmbeddedResourceContent_Fails()
        {
            // Act
            string resourceString = _assembly.GetEmbeddedResourceContent("Non existing resources");
        }
    }
}

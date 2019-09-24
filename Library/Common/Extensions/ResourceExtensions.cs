// (c) Euphemism Inc. All right reserved.

using System;
using System.IO;
using System.Reflection;

namespace Coconut.Library.Common.Extensions
{
    /// <summary>
    /// Extension methods to help with resources.
    /// </summary>
    public static class ResourceExtensions
    {
        /// <summary>
        /// Gets the content of the embedded resource.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="embeddedResourceName">Name of the embedded resource.</param>
        /// <exception cref="ArgumentNullException">embeddedResourceName</exception>
        public static string GetEmbeddedResourceContent(this Assembly assembly, string embeddedResourceName)
        {
            if (embeddedResourceName == null)
                throw new ArgumentNullException(nameof(embeddedResourceName));

            using (Stream stream = assembly.GetManifestResourceStream(embeddedResourceName))
            {
                if (stream == null)
                {
                    throw new ArgumentException($"'{embeddedResourceName}' cannot be found as a resource.");
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}

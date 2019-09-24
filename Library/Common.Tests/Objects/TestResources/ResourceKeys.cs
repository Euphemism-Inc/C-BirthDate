// (c) Euphemism Inc. All right reserved.

namespace Coconut.Library.Common.Tests.Objects.TestResources
{
    internal static class ResourceKeys
    {
        private static readonly string nameSpace = "Coconut.Library.Common.Tests.Objects.TestResources";

        public static readonly string BirthDate_Serialized = $"{nameSpace}.{nameof(BirthDate_Serialized)}.xml";
        public static readonly string BirthDate_Serialized_Empty = $"{nameSpace}.{nameof(BirthDate_Serialized_Empty)}.xml";
        public static readonly string BirthDate_Serialized_MissingChildElement = $"{nameSpace}.{nameof(BirthDate_Serialized_MissingChildElement)}.xml";
        public static readonly string BirthDate_Serialized_UnknownChildElement = $"{nameSpace}.{nameof(BirthDate_Serialized_UnknownChildElement)}.xml";
    }
}

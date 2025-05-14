using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Snipper.Templates.Images;

[JsonSerializable(typeof(Segment))]
[JsonSerializable(typeof(IReadOnlyList<Segment>))]
internal partial class SegmentContext : JsonSerializerContext
{
}
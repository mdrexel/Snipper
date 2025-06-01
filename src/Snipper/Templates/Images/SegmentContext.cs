using System.Collections.Generic;
using System.Text.Json.Serialization;
using Snipper.Templates.Images.Models;

namespace Snipper.Templates.Images;

[JsonSerializable(typeof(BoundingBox))]
[JsonSerializable(typeof(Scaling))]
[JsonSerializable(typeof(Pattern))]
[JsonSerializable(typeof(Segment))]
[JsonSerializable(typeof(IReadOnlyList<Segment>))]
internal partial class ModelContext : JsonSerializerContext
{
}
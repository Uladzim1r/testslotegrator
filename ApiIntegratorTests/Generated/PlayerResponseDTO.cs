using System.Text.Json.Serialization;
using ApiIntegratorTests.Converters;

namespace ApiIntegratorTests.Generated;

[JsonConverter(typeof(PlayerResponseDTOJsonConverter))]
public partial class PlayerResponseDTO
{
}

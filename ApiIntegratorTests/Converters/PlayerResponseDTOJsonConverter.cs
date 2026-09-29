using System.Text.Json;
using System.Text.Json.Serialization;
using ApiIntegratorTests.Generated;

namespace ApiIntegratorTests.Converters;

public sealed class PlayerResponseDTOJsonConverter : JsonConverter<PlayerResponseDTO>
{
    public override PlayerResponseDTO Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dto = new PlayerResponseDTO();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return dto;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();
            reader.Read();

            switch (propertyName)
            {
                case "id" or "_id":
                    dto.Id = reader.GetString() ?? string.Empty;
                    break;
                case "currency_code":
                    dto.Currency_code = reader.GetString() ?? string.Empty;
                    break;
                case "username":
                    dto.Username = reader.GetString() ?? string.Empty;
                    break;
                case "email":
                    dto.Email = reader.GetString() ?? string.Empty;
                    break;
                case "name":
                    dto.Name = reader.GetString() ?? string.Empty;
                    break;
                case "surname":
                    dto.Surname = reader.GetString() ?? string.Empty;
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        throw new JsonException("Unexpected end of JSON while deserializing PlayerResponseDTO.");
    }

    public override void Write(Utf8JsonWriter writer, PlayerResponseDTO value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("id", value.Id);
        writer.WriteString("currency_code", value.Currency_code);
        writer.WriteString("username", value.Username);
        writer.WriteString("email", value.Email);
        writer.WriteString("name", value.Name);
        writer.WriteString("surname", value.Surname);
        writer.WriteEndObject();
    }
}

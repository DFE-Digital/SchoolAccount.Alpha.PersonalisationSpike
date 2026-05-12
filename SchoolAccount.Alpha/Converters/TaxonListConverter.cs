using System.Text.Json;
using System.Text.Json.Serialization;
using SchoolAccount.Alpha.Services;

namespace SchoolAccount.Alpha.Converters
{
    public class TaxonListConverter(ITaxonService taxonService) : JsonConverter<List<string>>
    {
        public override List<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var taxonNames = new List<string>();

            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("Expected start of array");
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    break;
                }

                if (reader.TokenType == JsonTokenType.String)
                {
                    var taxonId = reader.GetString();
                    if (!string.IsNullOrEmpty(taxonId))
                    {
                        var taxonName = taxonService.GetTaxonName(taxonId);
                        if (!string.IsNullOrEmpty(taxonName))
                        {
                            taxonNames.Add(taxonName);
                        }
                    }
                }
            }

            return taxonNames;
        }

        public override void Write(Utf8JsonWriter writer, List<string> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
            {
                writer.WriteStringValue(item);
            }
            writer.WriteEndArray();
        }
    }
}
using System.Text.Json.Serialization;

namespace Verifile.Models
{
    public class Person
    {
        [JsonPropertyName("member_id")]
        public string MemberId { get; set; }

        [JsonPropertyName("house")]
        public string House { get; set; }

        [JsonPropertyName("constituency")]
        public string Constituency { get; set; }

        [JsonPropertyName("party")]
        public string Party { get; set; }

        [JsonPropertyName("entered_house")]
        public string EnteredHouse { get; set; }

        [JsonPropertyName("left_house")]
        public string LeftHouse { get; set; }

        [JsonPropertyName("entered_reason")]
        public string EnteredReason { get; set; }

        [JsonPropertyName("left_reason")]
        public string LeftReason { get; set; }

        [JsonPropertyName("person_id")]
        public string PersonId { get; set; }

        [JsonPropertyName("lastupdate")]
        public string LastUpdate { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("given_name")]
        public string GivenName { get; set; }

        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; }

        [JsonPropertyName("full_name")]
        public string FullName { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("image_height")]
        public int? ImageHeight { get; set; }

        [JsonPropertyName("image_width")]
        public int? ImageWidth { get; set; }

        // Propriedade helper para parse da data
        public DateTime? LastUpdateDate
        {
            get
            {
                if (string.IsNullOrEmpty(LastUpdate))
                    return null;

                if (DateTime.TryParseExact(
                    LastUpdate,
                    "yyyy-MM-dd HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime date))
                {
                    return date;
                }

                return null;
            }
        }
    }
}
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Voluntary.App.Models
{
    public class DatatablesQueryModel <T>
    {
        [JsonProperty("recordsTotal")]
        public int RecordsTotal { get; set; }
        [JsonProperty("recordsFilterd")]
        public int RecordsFilterd { get; set; }
        [JsonProperty("data")]
        public IEnumerable<T> Data { get; set; }
    }
}

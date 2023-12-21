using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.StageContents.BattleDome
{
    [Serializable]
    [JsonObject]
    public class BGMData
    {
        [JsonProperty]
        public float time_span;
        [JsonProperty]
        public List<float> f_table;
        [JsonProperty]
        public List<List<float>> data;
    }
}

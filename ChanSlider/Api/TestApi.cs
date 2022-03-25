#if DEBUG

using ChanSlider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChanSlider.Api
{
    class TestApi : BaseApi
    {
        public override async Task<List<ApiItemMdl>> GetItemsAsync(string[] tags, bool highRes = false)
        {
            var list = new List<ApiItemMdl>();

            using var stream = new System.IO.FileStream(@"test.json", System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read, 4096, System.IO.FileOptions.SequentialScan);

            using var streamReader = new System.IO.StreamReader(stream, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            using var jsonReader = new Newtonsoft.Json.JsonTextReader(streamReader);

            ApiItemMdl current = null;

            while (await jsonReader.ReadAsync())
            {
                switch (jsonReader.TokenType)
                {
                    case Newtonsoft.Json.JsonToken.StartObject:
                        current = new ApiItemMdl();
                        break;
                    case Newtonsoft.Json.JsonToken.EndObject:
                        list.Add(current);
                        current = null;
                        break;
                    case Newtonsoft.Json.JsonToken.PropertyName:
                        string propName = (string)jsonReader.Value;
                        await jsonReader.ReadAsync();

                        switch (propName)
                        {
                            case "url":
                                current.Url = new Uri((string)jsonReader.Value);
                                break;
                        }

                        break;
                }
            }

            return list;
        }
    }
}

#endif
using ChanSlider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChanSlider.Api
{
    class KonachanApi : BaseApi
    {
        private const string URL = "https://konachan.com/post.json";
        private const string POSTURL = "https://konachan.com/show/";

        public KonachanApi()
            : base()
        {
            
        }

        public override async Task<List<ApiItemMdl>> GetItemsAsync(string[] tags, bool highRes = false, int? page = null)
        {
            var list = new List<ApiItemMdl>();

            string fullUrl = $"{URL}?tags={string.Join("%20", tags)}";

            if (page != null)
                fullUrl += "&page=" + page.Value;

            using var stream = await httpClient.GetStreamAsync(fullUrl);

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
                            case "id":
                                current.PostUrl = POSTURL + jsonReader.Value;
                                break;
                            case "file_url":
                                if (highRes)
                                    current.Url = new Uri((string)jsonReader.Value);
                                break;
                            case "sample_url":
                                if (!highRes)
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

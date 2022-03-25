﻿using ChanSlider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChanSlider.Api
{
    class LoliApi : BaseApi
    {
        private const string URL = "https://lolibooru.moe/post/index.json";
        private const string POSTURL = "https://lolibooru.moe/post/show/";

        public LoliApi()
            : base()
        {

        }

        public override async Task<List<ApiItemMdl>> GetItemsAsync(string[] tags, bool highRes = false)
        {
            var list = new List<ApiItemMdl>();

            string fullUrl = $"{URL}?tags={string.Join("%20", tags)}";

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
                                current.PostUrl = POSTURL + (string)jsonReader.Value;
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
using ChanSlider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChanSlider.Api
{
    abstract class BaseApi
    {
        protected static readonly HttpClient httpClient;

        static BaseApi()
        {
            httpClient = new HttpClient();

            string userAgent = NativeMethods.ObtainUserAgentString();

            if (!string.IsNullOrWhiteSpace(userAgent))
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);

            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
        }

        public abstract Task<List<ApiItemMdl>> GetItemsAsync(string[] tags, bool highRes = false);

        private static void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_DomainUnload;
            httpClient.Dispose();
        }
    }
}

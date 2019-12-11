﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MarvelDemo.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MarvelDemo.Services
{
    public class MarvelDataService : IMarvelDataService
    {
        // TODO: Add your Marvel Developer Account keys
        const string _API_PRIVATE_KEY = "c6ab693a9871d285e2bfbb1b13b57a2a48d27338";
        const string _API_PUBLIC_KEY = "2628856d97040a8d6ee09f117662864a";

        readonly IHashService _hashService;

        public MarvelDataService(IHashService hashService)
        {
            _hashService = hashService;
        }

        public async Task<IEnumerable<Comic>> GetComicsBySeries(int seriesId, string orderBy = null)
        {
            var ts = Guid.NewGuid().ToString();
            var hash = _hashService.CreateMd5Hash(ts + _API_PRIVATE_KEY + _API_PUBLIC_KEY);

            if (string.IsNullOrWhiteSpace(orderBy))
                orderBy = "issueNumber";

            var url =
                $@"http://gateway.marvel.com/v1/public/series/{seriesId}/comics?orderBy={orderBy}&apikey={_API_PUBLIC_KEY}&hash={hash}&ts={ts}";
            
            var client = new HttpClient();
            var response = await client.GetStringAsync(url);
            
            var responseObject = JObject.Parse(response);

            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<IEnumerable<Comic>>(responseObject["data"]["results"].ToString()));
        }
    }
}

using System;
using System.Net;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class YouTubeApi
{
    public static readonly Uri BaseUri = new Uri("https://www.googleapis.com/youtube/v3/");

    private readonly (string key, string value) ApiKeyValue;
    public YouTubeApi(string apiKey)
    {
        ApiKeyValue = ("key", apiKey);
    }

    public class ApiResponseContainer<T>
    {
        [JsonProperty("kind")]
        public string Kind { get; private set; }

        [JsonProperty("nextPageToken")]
        public string NextPageToken { get; private set; }

        [JsonProperty("items")]
        public T Items { get; private set; }
    }

    [JsonConverter(typeof(PlaylistItemConverter))]
    public class PlaylistItem
    {
        public string Title { get; set; }
        public string ID { get; set; }

        class PlaylistItemConverter : JsonConverter
        {
            public override bool CanConvert(Type type) { return type == typeof(PlaylistItem); }
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){ throw new NotImplementedException(); }
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject jObject = JObject.Load(reader);
                PlaylistItem item = new PlaylistItem();
                item.Title = jObject["snippet"]["title"].Value<string>();
                item.ID = jObject["snippet"]["resourceId"]["videoId"].Value<string>();
                return item;
            }
        }
    }

    public const string PlaylistItemsApi = "playlistItems";
    public List<PlaylistItem> GetPlaylist(string id)
    {
        try
        {
            (string key, string value)[] props = { ApiKeyValue, ("maxResults", "50"), ("part", "snippet"), ("playlistId", id) };
            string propsString = props.Aggregate("", (acc, p) => acc + "&" + p.key + "=" + p.value);
            Uri playlistItemsUri = new Uri(BaseUri, PlaylistItemsApi + "?" + propsString);
            var json = HttpRequest.Request<ApiResponseContainer<List<PlaylistItem>>>(playlistItemsUri);
            return json.Items;
        }
        catch(WebException e)
        {
            HttpStatusCode? statusCode = (e.Response as HttpWebResponse)?.StatusCode;
            if (statusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine("Playlist not found.");
            }
            else Console.WriteLine("Error accessing PlaylistItems API. [" + (statusCode != null ? statusCode.ToString() : e.Status.ToString()) + "]");
        }

        return new List<PlaylistItem>();
    }
}
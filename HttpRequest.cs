using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

class HttpRequest
{
    public static T Request<T>(Uri uri)
    {
        var request = WebRequest.Create(uri) as HttpWebRequest;
        using (var response = request.GetResponse() as HttpWebResponse)
        {
            using (var stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    var result = reader.ReadToEnd();
                    return ParseJson<T>(result);
                }
            }
        }
    }

    static T ParseJson<T>(string jsonString)
    {
        return JsonConvert.DeserializeObject<T>(jsonString);
    }
}
using System;
using CommandLine;

namespace youtube_to_spotify
{
    class Program
    {
        class CommandLineOptions
        {
            private string _YouTubeKey;
            private string _YouTubePlaylistID;

            public CommandLineOptions(string youTubeKey, string youTubePlaylistID)
            {
                _YouTubeKey = youTubeKey;
                _YouTubePlaylistID = youTubePlaylistID;
            }

            [Option("ytkey", Required = true, HelpText = "YouTube API key.")]
            public string YouTubeKey { get { return _YouTubeKey; } }

            [Option("ytplaylist", Required = true, HelpText = "YouTube playlist ID.")]
            public string YouTubePlaylistID { get { return _YouTubePlaylistID; } }
        }

        static void Run(CommandLineOptions options)
        {
            var youTubeApi = new YouTubeApi(options.YouTubeKey);
            var playlist = youTubeApi.GetPlaylist(options.YouTubePlaylistID);

            foreach (var p in playlist)
            {
                Console.WriteLine(p.Title);
            }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed(options => Run(options));
        }
    }
}

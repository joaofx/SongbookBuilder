using Spectre.Console.Cli;

namespace SongbookBuilder;

class Program
{
    static void Main(string[] args)
    {
        var app = new CommandApp();
            
        app.Configure(config =>
        {
            config.AddCommand<ServeCommand>("serve");
            config.AddCommand<ExportCommand>("export");
        });
            
        app.Run(args);
    }
}
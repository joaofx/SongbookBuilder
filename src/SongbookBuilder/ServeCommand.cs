using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Spectre.Console.Cli;

namespace SongbookBuilder;

public class ServeCommand : Command<Settings>
{
    public override int Execute(CommandContext context, Settings settings)
    {
        Settings.Current = settings;
        
        var builder = WebApplication.CreateBuilder(Environment.GetCommandLineArgs());
        var app = builder.Build();

        app.MapGet("/", IndexAction());
        app.MapGet("/index.html", IndexAction());
        app.MapGet("/index", IndexAction());
        
        app.MapGet("/songs.html", SongsAction());
        app.MapGet("/songs", SongsAction());
        
        app.MapGet("/artists", Artists());
        app.MapGet("/artists.html", Artists());
        
        app.MapGet("/easy.html", SongsAction("Easy"));
        app.MapGet("/medium.html", SongsAction("Medium"));
        app.MapGet("/hard.html", SongsAction("Hard"));
        
        app.MapGet("/{song}", Song());
        app.MapGet("/{song}.html", Song());

        app.MapGet("/chords/{file}", (string file) => Results.File(Path.Combine(settings.ChordsDirectory, file)));
        app.MapGet("/images/{file}", (string file) => Results.File(Path.Combine(settings.ImagesDirectory, file)));

        app.Run();

        return 0;
    }

    private static Func<string, IResult> Song() =>
        song =>
        {
            var songTemplate = Songs.GetSongTemplate();
            var parsedSong = Songs.ParseSong(song);

            if (parsedSong == null)
            {
                Console.WriteLine(@$"
==========================================================
Could not find song with title ""{song}"". Check the song's text file name or check the song's Title inside the txt file
==========================================================
");
                return Results.NotFound();
            }
            
            var songFile = Songs.ExportSong(parsedSong, songTemplate);

            return Results.File(songFile, "text/html");
        };

    private static Func<IResult> SongsAction(string level = null) => () =>
        {
            var template = Songs.GetSongsTemplate();
            var songs = Songs.GetSongs(level);
            var htmlFile = Songs.Export(songs, template);

            return Results.File(htmlFile, "text/html");
        };

    private static Func<IResult> IndexAction() => 
        () => Results.File(Index.Export(), "text/html");

    private static Func<IResult> Artists() => () =>
        {
            var songs = Songs.GetSongs().OrderBy(x => x.Artist);
            var htmlFile = Songs.ExportByArtists(songs);

            return Results.File(htmlFile, "text/html");
        };
}
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Spectre.Console;
using Spectre.Console.Cli;

namespace SongbookBuilder;

public class ExportCommand : Command<Settings>
{
    public override int Execute(CommandContext context, Settings settings)
    {
        Settings.Current = settings;
        
        Console.WriteLine("Exporting...");
        
        var songs = Songs.GetSongs(parseBody: true);
        
        // index
        Console.WriteLine("Exporting Index...");
        Index.Export();
        
        // song list
        Console.WriteLine("Exporting Song List...");
        var songsTemplate = Songs.GetSongsTemplate();
        Songs.Export(songs, songsTemplate);
        
        // song list by level
        Console.WriteLine("Exporting Songs by Level...");
        
        var easySongs = Songs.GetSongs("Easy");
        Songs.Export(easySongs, songsTemplate, "easy.html");
        
        var mediumSongs = Songs.GetSongs("Medium");
        Songs.Export(mediumSongs, songsTemplate, "medium.html");
        
        var hardSongs = Songs.GetSongs("Hard");
        Songs.Export(hardSongs, songsTemplate, "hard.html");
        
        // song list by artists
        Console.WriteLine("Exporting Songs by Artists...");
        var songsByArtists = Songs.GetSongs().OrderBy(x => x.Artist);
        Songs.ExportByArtists(songsByArtists);
        
        // songs
        Console.WriteLine("Exporting Songs...");
        var songTemplate = Songs.GetSongTemplate();
        foreach (var song in songs)
        {
            Songs.ExportSong(song, songTemplate);
        }
        
        // images and chords
        Console.WriteLine("Exporting Images...");
        
        Extensions.CopyDirectory(
            settings.ImagesDirectory,
            Path.Combine(settings.ExportDirectory, "images"));
        
        Console.WriteLine("Exporting Chords...");
        Extensions.CopyDirectory(
            settings.ChordsDirectory,
            Path.Combine(settings.ExportDirectory, "chords"));
        
        Console.WriteLine("Done!");

        return 0;
    }
}
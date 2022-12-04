using System.Collections.Generic;
using System.IO;
using System.Linq;
using Scriban;

namespace SongbookBuilder;

public class Songs
{
    // private static readonly Template SongTemplate = 
    //     Template.Parse(File.ReadAllText(
    //         Path.Combine(Settings.Current.TemplateDirectory, "song.html")));
    
    public static IEnumerable<Song> GetSongs(string level = null, bool parseBody = false)
    {
        var songs = new List<Song>();
        
        var songFiles = Directory
            .GetFiles(Path.Combine(Settings.Current.AssetsDirectory, "songs"))
            .Where(f => !Path.GetFileName(f).StartsWith("_"))
            .ToList();

        var number = 1;
            
        foreach (var songFile in songFiles)
        {
            if (number == 54) number++;

            if (songFile.Contains("Imagine.txt")) 
            {
                var imagine = Song.BuildSong(songFile, 54, parseBody);
                songs.Insert(53, imagine);
                continue;
            }
                
            var song = Song.BuildSong(songFile, number, parseBody);
            songs.Add(song);
                
            number++;
        }

        if (string.IsNullOrEmpty(level) == false)
        {
            if (level == "Medium")
                songs = songs.Where(x => x.Level == "Easy" || x.Level == "Medium").ToList();
            else
                songs = songs.Where(x => x.Level == level).ToList();
        }

        return songs;
    }
    
    public static Song ParseSong(string name)
    {
        var songName = CleanName(name);
        
        var songFile = Directory
            .GetFiles(Path.Combine(Settings.Current.AssetsDirectory, "songs"))
            .Where(f => !Path.GetFileName(f).StartsWith("_"))
            .Where(f => Path.GetFileName(f).Equals($"{songName}.txt"))
            .SingleOrDefault();

        if (songFile == null)
            return null;

        return Song.BuildSong(songFile, 54, parseBody: true);
    }
    
    public static string ExportSong(Song song, Template template)
    {
        var result = template.Render(new
        {
            song = song
        });
        
        var file = Path.Combine(Settings.Current.ExportDirectory, $"{song.UrlTitle}.html");
        
        if (File.Exists(file)) 
            File.Delete(file);
        
        File.WriteAllText(file, result);

        return file;
    }

    public static Template GetSongsTemplate()
    {
        var templateFile = Path.Combine(Settings.Current.TemplateDirectory, "songs.html");
        
        return Template.Parse(File.ReadAllText(templateFile));
    }
    
    public static Template GetSongTemplate()
    {
        return Template.Parse(File.ReadAllText(
            Path.Combine(Settings.Current.TemplateDirectory, "song.html")));
    }

    public static string CleanName(string song)
    {
        return song
            .Replace("⭐", string.Empty)
            .Replace("🎅", string.Empty)
            .Replace("-", " ");
    }

    public static string ExportByArtists(IEnumerable<Song> songs, Template template = null)
    {
        if (template == null)
            template = Index.GetArtistsTemplate();
            
        songs = songs.OrderBy(x => x.Artist);
            
        return Export(songs, template, "artists.html");
    }

    public static string Export(
        IEnumerable<Song> songs, 
        Template template = null,
        string outputFile = "songs.html")
    {
        if (template == null)
            template = GetSongsTemplate();
        
        var totalEasy = songs.Count(x => x.Level == "Easy");
        var totalMedium = songs.Count(x => x.Level == "Medium");
        var totalHard = songs.Count(x => x.Level == "Hard");
        var total = songs.Count();

        var result = template.Render(new
        {
            songs, 
            total, 
            totalEasy = totalEasy, 
            totalMedium, 
            totalHard
        });

        var file = Path.Combine(Settings.Current.ExportDirectory, outputFile);

        if (Directory.Exists(Path.GetDirectoryName(file)) == false)
            Directory.CreateDirectory(Path.GetDirectoryName(file)!);
            
        File.WriteAllText(file, result);

        return file;
    }
}
using System.ComponentModel;
using System.IO;
using Spectre.Console.Cli;

namespace SongbookBuilder;

public class Settings : CommandSettings
{
    public static Settings Current { get; set; }
    
    [Description("Directory where songs, tempates, and images are")]
    [CommandArgument(0, "[assets-directory]")]
    public string AssetsDirectory { get; set; }

    public string ExportDirectory => Path.Combine(AssetsDirectory, "export");
    public string TemplateDirectory => Path.Combine(AssetsDirectory, "template");
    public string SongsDirectory => Path.Combine(AssetsDirectory, "songs");
    public string ChordsDirectory => Path.Combine(AssetsDirectory, "chords");
    public string ImagesDirectory => Path.Combine(AssetsDirectory, "images");
}
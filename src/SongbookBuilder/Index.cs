using System.Collections.Generic;
using System.IO;
using System.Linq;
using Scriban;

namespace SongbookBuilder;

public class Index
{
    public static string Export(Template template = null)
    {
        if (template == null)
            template = GetTemplate();
        
        var result = template.Render();

        var index = Path.Combine(Settings.Current.ExportDirectory, "index.html");

        if (Directory.Exists(Path.GetDirectoryName(index)) == false)
            Directory.CreateDirectory(Path.GetDirectoryName(index)!);
            
        File.WriteAllText(index, result);

        return index;
    }

    public static Template GetTemplate()
    {
        var templateFile = Path.Combine(Settings.Current.TemplateDirectory, "index.html");
        
        return Template.Parse(File.ReadAllText(templateFile));
    }
    
    public static Template GetArtistsTemplate()
    {
        var templateFile = Path.Combine(Settings.Current.TemplateDirectory, "artists.html");
        
        return Template.Parse(File.ReadAllText(templateFile));
    }
}
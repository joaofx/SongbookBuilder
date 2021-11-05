using System.Collections.Generic;
using System.Text;

namespace SongbookBuilder
{
    public class Song
    {
        public string Title { get; set; }

        public string UrlTitle => Title.Replace(" ", "-");
        
        public string Level { get; set; }
        
        public string Artist { get; set; }
        
        public int Number { get; set; }
        
        public string Spotify { get; set; }

        public string Youtube { get; set; }
        
        public bool IsNew { get; set; }
        
        public bool Sticky { get; set; } = true;

        public StringBuilder Tab { get; set; }

        public List<string> Chords = new List<string>();
        
        public string LevelClass
        {
            get
            {
                if (Level == "Easy")
                    return "text-success";
            
                if (Level == "Medium")
                    return "text-info";
            
                if (Level == "Hard")
                    return "text-warning";

                return string.Empty;
            }
        }
    }
}
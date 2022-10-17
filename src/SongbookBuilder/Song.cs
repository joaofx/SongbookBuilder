using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Scriban;

namespace SongbookBuilder;

public class Song
{
    private static readonly Regex FindChordsRegex = 
        new Regex(@"[^\[]+(?=\])", RegexOptions.Compiled);
    
    public string Title { get; set; }

    public string FilePath { get; set; }

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
                return "success";
            
            if (Level == "Medium")
                return "warning";
            
            if (Level == "Hard")
                return "danger";

            return string.Empty;
        }
    }

    public static Song BuildSong(
        string songFile, 
        int number, 
        bool parseBody = true)
    {
        var song = new Song { Number = number, FilePath = songFile };
        var lines = File.ReadLines(songFile);
        var lineNumber = 0;
        var outputLines = new StringBuilder();
        
        foreach (var line in lines)
        {
            lineNumber++;

            // if we are just looking for metadata, we can leave after few lines
            // we don't expect metadata higher than 7 lines
            if (lineNumber > 8 && parseBody == false)
                break;
            
            // TODO: Convert all this logic to use regex

            FindChordsInTheLineAndAddToTheSong(line, song);
            
            if (IsLineBreak(line))
            {
                outputLines.Append("<br/>");
                continue;
            }

            if (line.StartsWith("{t:"))
            {
                var title = line
                    .Replace("{t:", string.Empty)
                    .Replace("}", string.Empty)
                    .Trim();
                    
                song.Title = title;
            }
            else if (line.StartsWith("{level:"))
            {
                var level = line
                    .Replace("{level:", string.Empty)
                    .Replace("}", string.Empty)
                    .Trim();

                song.Level = level;
            }
            else if (line.StartsWith("{spotify:"))
            {
                var spotify = line
                    .Replace("{spotify:", string.Empty)
                    .Replace("}", string.Empty)
                    .Trim();

                song.Spotify = spotify;
            }
            else if (line.StartsWith("{youtube:"))
            {
                var youtube = line
                    .Replace("{youtube:", string.Empty)
                    .Replace("}", string.Empty)
                    .Trim();

                song.Youtube = youtube;
            }
            else if (line.StartsWith("{artist:"))
            {
                var artist = line
                    .Replace("{artist:", string.Empty)
                    .Replace("}", string.Empty)
                    .Trim();
                
                song.Artist = artist;
            }
            else if (line.StartsWith("{nosticky:"))
            {
                song.Sticky = false;
            }
            else if (line.StartsWith("{"))
                continue;
            else
            {
                var convertingLine = line
                    .Replace("<grey>", "<span class='text-muted'>")
                    .Replace("</grey>", "</span>")
                    .Replace("[stop]", "<strong>[stop]</strong>")
                    .Replace("[riff]", "<span class='text-muted'><strong>(riff)</strong></span>")
                    .Replace("[back]", "<b><i>")
                    .Replace("[/back]", "</b></i>");

                if (convertingLine.Contains("[") && convertingLine.Contains("]"))
                {
                    convertingLine = convertingLine
                        .Replace("[", "<span class=\"chord\">[")
                        .Replace("]", "]</span>");
                }

                outputLines.Append(convertingLine);
                outputLines.Append("<br/>");
            }
        }

        song.Tab = outputLines;

        return song;
    }

    private static void FindChordsInTheLineAndAddToTheSong(string line, Song song)
    {
        var chords = FindChordsRegex.Matches(line)
            .Select(m => m.Value
                .IfEndsWithReplace("/", string.Empty)
                .IfEndsWithReplace("//", string.Empty)
                .IfEndsWithReplace("///", string.Empty)
                .Replace("/", "-")
                .Replace("#", "sharp")
                .Trim())
            .Where(m => !m.ToLower().Equals("nc"))
            .Where(m => !m.Contains(" "))
            .Where(m => !m.ToLower().Equals("stop"))
            .Where(m => !m.ToLower().Equals("riff"))
            .Where(m => !m.ToLower().Equals("back"))
            .Where(m => !m.ToLower().Equals("-back"))
            .ToList();

        foreach (var chord in chords)
        {
            if (song.Chords.Contains(chord) == false)
                song.Chords.Add(chord);
        }
    }
    
    private static bool IsLineBreak(string line)
    {
        return line.StartsWith("{c:") || 
               line.Equals(Environment.NewLine) || 
               string.IsNullOrEmpty(line) ||
               line.StartsWith("\t\t");
    }
}

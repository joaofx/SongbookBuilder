using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Scriban;

namespace SongbookBuilder
{
    class Program
    {
        private static string OutputPath = @"D:\Ukulele\krakow-ukulele\songs";
        
        private static string SongsPath = @"D:\Ukulele\SongbookBuilder\songs";
        
        private static string TemplatesPath = @"D:\Ukulele\SongbookBuilder\template";
        
        private static readonly Regex FindChordsRegex = 
            new Regex(@"[^\[]+(?=\])", RegexOptions.Compiled);
        
        private static readonly Template SongTemplate = 
            Template.Parse(File.ReadAllText(Path.Combine(TemplatesPath, "song.html")));
        
        private static readonly Template IndexTemplate = 
            Template.Parse(File.ReadAllText(Path.Combine(TemplatesPath, "index.html")));

        static void Main(string[] args)
        {
            DeleteOutput();
            
            var songs = BuildSongs();
            
            SaveSongs(songs);
            SaveIndex(songs);
            // SaveFilter(songs, songs.Where(m => m.IsNew), "new.html");
            SaveFilter(songs, songs.Where(m => m.Level == "Easy"), "easy.html");
            SaveFilter(songs, songs.Where(m => m.Level == "Easy" || m.Level == "Medium"), "medium.html");
            SaveFilter(songs, songs.Where(m => m.Level == "Hard"), "hard.html");
        }

        private static void SaveFilter(IEnumerable<Song> allSongs, IEnumerable<Song> songs, string html)
        {
            var totalEasy = allSongs.Count(x => x.Level == "Easy");
            var totalMedium = allSongs.Count(x => x.Level == "Medium");
            var totalHard = allSongs.Count(x => x.Level == "Hard");
            var total = allSongs.Count();

            var result = IndexTemplate.Render(new { songs, total, totalEasy = totalEasy, totalMedium, totalHard });
            File.WriteAllText(Path.Combine(OutputPath, html), result);
        }

        private static List<Song> BuildSongs()
        {
            var songs = new List<Song>();
            var songFiles = Directory
                .GetFiles(SongsPath)
                .Where(f => !Path.GetFileName(f).StartsWith("_"))
                .ToList();

            // songFiles.ForEach(s => Console.WriteLine(s));
            
            var number = 1;
            
            foreach (var songFile in songFiles)
            {
                if (number == 54) number++;

                if (songFile.Contains("Imagine.txt")) 
                {
                    var imagine = BuildSong(songFile, 54);
                    songs.Insert(53, imagine);
                    continue;
                }
                
                var song = BuildSong(songFile, number);
                songs.Add(song);
                
                number++;
            }

            return songs;
        }
        
        private static void SaveSongs(List<Song> songs)
        {
            foreach (var song in songs)
            {
                SaveSong(song);
            }
        }

        private static Song BuildSong(string songFile, int number)
        {
            var song = new Song { Number = number };
            var lines = File.ReadLines(songFile);
            var outputLines = new StringBuilder();
            
            foreach (var line in lines)
            {
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
                        .Replace("[stop]", "<strong>[stop]</strong>")
                        .Replace("[riff]", "<strong>[riff]</strong>")
                        .Replace("[back]", "<b><i>")
                        .Replace("[/back]", "</i></b>");

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

        private static bool IsLineBreak(string line)
        {
            return line.StartsWith("{c:") || 
                   line.Equals(Environment.NewLine) || 
                   string.IsNullOrEmpty(line) ||
                   line.StartsWith("\t\t");
        }
        
        private static void SaveIndex(List<Song> songs)
        {
            var totalEasy = songs.Count(x => x.Level == "Easy");
            var totalMedium = songs.Count(x => x.Level == "Medium");
            var totalHard = songs.Count(x => x.Level == "Hard");
            var total = songs.Count();

            var result = IndexTemplate.Render(new { songs, total, totalEasy = totalEasy, totalMedium, totalHard });
            File.WriteAllText(Path.Combine(OutputPath, $"index.html"), result);
        }
        
        private static void SaveSong(Song song)
        {
            var result = SongTemplate.Render(new { song = song });
            File.WriteAllText(Path.Combine(OutputPath, $"{song.UrlTitle}.html"), result);
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

        private static void DeleteOutput()
        {
            var files = Directory.GetFiles(OutputPath, "*.html");

            foreach (var file in files)
            {
                File.Delete(file);
            }
        }
    }

    public static class StringExtensions
    {
        public static string IfEndsWithReplace(this string current, string toReplace, string replacement)
        {
            if (current.EndsWith(toReplace))
                return current.Replace(toReplace, replacement);

            return current;
        }
    }
}
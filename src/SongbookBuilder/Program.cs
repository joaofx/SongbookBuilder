using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Scriban;
using Spectre.Console.Cli;

namespace SongbookBuilder
{
    class Program
    {
        private static readonly Regex FindChordsRegex = 
            new Regex(@"[^\[]+(?=\])", RegexOptions.Compiled);
        
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
}
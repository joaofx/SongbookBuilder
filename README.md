# SongbookBuilder

SongbookBuilder is a tool to build an online ukulele "songbook" with lyrics and chords.

It is developed to support [Krakow Ukulele Meetup](https://krakow-ukulele.netlify.com/)

# Getting Started

## Requirements

[.NET SDK 6.0 or higher](https://dotnet.microsoft.com/download/dotnet-core)

## Run

### Configuration

You must have a directory with needed artifacts, like templates, songs, and etc.

Create a directory with this structure (or use the one already set in this repository):

```
/ArtifactsDirectory
    /chords     => where the chords images are
    /export     => where this tool will export the songbook
    /images     => other images for the songbook
    /songs      => where the songs in .txt format are
    /template   => where templates that will be transformed to html are
```

### Running Locally

Run the command at `src/SongbookBuilder` directory:

```shell
dotnet run serve PATH_TO_REPOSITORY_ROOT
```
For example, if the root of this project in your local machine is `C:\Ukulele\SongbookBuilder`, then run:
```shell
dotnet run serve C:\Ukulele\SongbookBuilder
```
<!-- dotnet run serve D:\Ukulele\SongbookBuilder\ -->

### Exporting

To export the songbook html's to be uploaded to a web server (run the command at `src/SongbookBuilder`:)

```shell
dotnet run export C:\Ukulele\SongbookBuilder
```
<!-- dotnet run export D:\Ukulele\SongbookBuilder\ -->

<!-- 
dotnet run export /Users/joao/projects/SongbookBuilder --project src/SongbookBuilder
dotnet serve --directory export
cp -a export/. ../krakow-ukulele/
cd ../krakow-ukulele
git add -A && git commit -m "Commit" && git push origin master 
cd -
git add -A && git commit -m "Commit" && git push origin master 
-->

The htmls will be exported at `PATH_TO_YOUR_ARTIFACTS_DIRECTORY\export`
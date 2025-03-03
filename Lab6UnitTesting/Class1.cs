﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;
using File = TagLib.File;

namespace ClassLibraryMedia
{
    public abstract class Media
    {
        public abstract string title { get; set; }
        public abstract string fileLocation { get; set; }
    }

    public class Audio : Media
    {
        public override string title { get; set; }

        public override string fileLocation { get; set; }

        public string album { get; set; }

        public string[] artists { get; set; }

        public string[] genres { get; set; }

        public string duration { get; set; }

        public Image coverArt { get; set; }

    }

    public class Video : Media
    {
        public override string title { get; set; }

        public override string fileLocation { get; set; }

        public string[] genres { get; set; }
    
        public string director { get; set; }

        public string duration { get; set; }

        public string description { get; set; }
    }

    public class Picture : Media
    {
        public override string title { get; set; }

        public override string fileLocation { get; set; }

        public Size resolution { get; set; }

        public int date { get; set; }

        public string description { get; set; }
    }

    public static class MediaScanner
    {
        private static ArrayList _audios = new ArrayList();
        private static ArrayList _videos = new ArrayList();
        private static ArrayList _images = new ArrayList();

        public static ArrayList Audios
        {
            get { return _audios; }
            set { _audios = value; }
        }

        public static ArrayList Videos
        {
            get { return _videos; }
            set { _videos = value; }
        }

        public static ArrayList Images
        {
            get { return _images; }
            set { _images = value; }
        }

        /// <summary>
        /// Scans for 
        /// </summary>
        /// <returns></returns>
        public static bool scanAudio()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            string[] fileTypes = { ".mp3", ".wav", ".flac", ".m4a", ".ogg" };

            try
            {
                string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                                 .Where(file => fileTypes.Contains(Path.GetExtension(file)))
                                 .ToArray();
                foreach (string filePath in files)
                {
                    File file = File.Create(filePath);
                    Audio audio = new Audio();
                    audio.title = file.Tag.Title;
                    audio.fileLocation = filePath;
                    audio.genres = file.Tag.Genres;
                    audio.album = file.Tag.Album;
                    audio.artists = file.Tag.Performers;
                    audio.duration = file.Tag.Length;
                    audio.coverArt = getCoverArt(file);
                    _audios.Add(audio);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }

        public static bool scanVideo()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            string[] fileTypes = { ".mp4", ".mkv", ".mov" };

            try
            {
                string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                                 .Where(file => fileTypes.Contains(Path.GetExtension(file)))
                                 .ToArray();
                foreach (string filePath in files)
                {
                    File file = File.Create(filePath);

                    Video video = new Video();
                    video.title = file.Name;
                    video.fileLocation = filePath;
                    video.description = file.Tag.Description;
                    video.title = file.Tag.Title;
                    video.duration = file.Tag.Length;
                    video.genres = file.Tag.Genres;
                    _videos.Add(video);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static Image getCoverArt(File file)
        {
            IPicture pic = file.Tag.Pictures.Length > 0 ? file.Tag.Pictures[0] : null;
            if (pic != null)
            {
                // Convert the picture data to an Image object
                MemoryStream ms = new MemoryStream(pic.Data.Data);
                Image image = Image.FromStream(ms);
                return image;
            }
            else
            {
                return null;
            }
        }
    }



}

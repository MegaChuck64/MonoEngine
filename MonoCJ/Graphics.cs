using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.IO;

namespace MonoCJ
{
    public static class Graphics
    {
        static MonoGame game;


        private static string fontDirectory;
        private static string fontPath;


        public static void Init(MonoGame gme)
        {
            game = gme;

            fontDirectory = Directory.GetCurrentDirectory() + @"\..\..\..\..\Content\Fonts\";

            fontPath = "defaultFont.spritefont";
        }

        public static Texture2D Rect(int width, int height, Color col)
        {
            Texture2D text = new Texture2D(game.GraphicsDevice, width, height);

            Color[] cols = new Color[width * height];

            for (int i = 0; i < width * height; i++)
            {
                cols[i] = col;
            }

            text.SetData<Color>(cols);

            return text;
        }

        public static Color RandomColor()
        {
            var r = (float)game.rand.NextDouble();
            var g = (float)game.rand.NextDouble();
            var b = (float)game.rand.NextDouble();

            return new Color(r, g, b);
        }

        public static Texture2D TextureFromURL(string url)
        {
            Texture2D image = null;

            try
            {
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 30000;

                System.Net.WebResponse webResponse = webRequest.GetResponse();

                System.IO.Stream stream = webResponse.GetResponseStream();

                image = Texture2D.FromStream(game.GraphicsDevice, stream);

                webResponse.Close();
            }
            catch
            {
                return null;
            }

            return image;
        }

        public static void CreateFontAsset(string name, int size)
        {



            var str =
         @"<?xml version=""1.0"" encoding=""utf-8""?>"
+
         @"<XnaContent xmlns:Graphics=""Microsoft.Xna.Framework.Content.Pipeline.Graphics"">"
+
             @"<Asset Type=""Graphics:FontDescription"">"
+
                 @"<FontName>" + name + @"</FontName>"
+
                 @"<Size>" + size + @"</Size>"
+
                 @"<Spacing>0</Spacing>"
+
                 @"<UseKerning>true</UseKerning>"
+
                 @"<Style>Regular</Style>"
+
                 @"<CharacterRegions>"
+
                     @"<CharacterRegion>"
+
                         @"<Start>&#32;</Start>"
+
                         @"<End>&#126;</End>"
+
                     @"</CharacterRegion>"
+
                 @"</CharacterRegions>"
+
             @"</Asset>"
+
         @"</XnaContent>";

            Directory.CreateDirectory(fontDirectory);

            using (var fs = new FileStream(fontDirectory + fontPath, FileMode.Create))
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(str);
                }
            }


            string outputDir = Directory.GetCurrentDirectory() + @"\x86\Debug\Content\Fonts\";

            var proc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    Arguments = "/outputDir:" + outputDir + " /intermediateDir:" + outputDir + " /build:" + fontDirectory + fontPath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    FileName = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\MSBuild\MonoGame\v3.0\Tools\MGCB.exe",
                }
            };


            proc.Start();
            string ret = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
        }
    }
}

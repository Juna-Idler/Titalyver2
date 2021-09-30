using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace Titalyver2
{
    public class WindowPositionStrage
    {
        public static WindowPositionStrage Default;
        public static string DefaultPath = Path.Combine(AppContext.BaseDirectory, "WindowPositon.json");

        public double Left { get; set; }
        public double Top { get; set; }
        public double Width{ get; set; }
        public double Heightt { get; set; }



        private static readonly JsonSerializerOptions options = new()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true,
        };
        public bool Save(string path)
        {
            try
            {
                using FileStream fs = File.Create(path);
                JsonSerializer.SerializeAsync(fs, this, options).Wait();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public static WindowPositionStrage Load(string path)
        {
            try
            {
                using FileStream fs = File.OpenRead(path);
                return JsonSerializer.DeserializeAsync<WindowPositionStrage>(fs).AsTask().Result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool Save(System.Windows.Window window)
        {
            WindowPositionStrage wps = new() { Left = window.Left, Top = window.Top, Width = window.Width, Heightt = window.Height };
            return wps.Save(DefaultPath);
        }
        public static bool Load(System.Windows.Window window)
        {
            WindowPositionStrage wps = Load(DefaultPath);
            if (wps != null)
            {
                window.Left = wps.Left;
                window.Top = wps.Top;
                window.Width = wps.Width;
                window.Height = wps.Heightt;
                return true;
            }
            return false;
        }
    }
}

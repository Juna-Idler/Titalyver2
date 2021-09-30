using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Text.Json;

namespace Titalyver2
{
    public class SettingsStorage
    {
        public static SettingsStorage Default;
        public static string DefaultPath = Path.Combine(AppContext.BaseDirectory, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + "Settings.json");

        public string FontFamily { get; set; } = "";
        public double FontSize { get; set; } = 20;
        public string FontStyle { get; set; } = "Normal";
        public string FontWeight { get; set; } = "Normal";
        public string FontStretch { get; set; } = "Normal";
        public double Outline { get; set; } = 2;


        public string ActiveFill { get; set; } = "White";
        public string StandbyFill { get; set; } = "White";
        public string SleepFill { get; set; } = "White";
        public string ActiveStroke { get; set; } = "Red";
        public string StandbyStroke { get; set; } = "Blue";
        public string SleepStroke { get; set; } = "DarkBlue";
        public string ActiveBack { get; set; } = "#7F004000";
        public string WindowBack { get; set; } = "#7F000000";


        public string TextAlignment { get; set; } = "Left";
        public string VerticalAlignment { get; set; } = "Top";
        public double OffsetLeft { get; set; } = 16;
        public double OffsetRight { get; set; } = 16;
        public double OffsetVertical { get; set; } = 0;

        public double LineTopSpace { get; set; } = 0;
        public double LineBottomSpace { get; set; } = 0;
        public double RubyBottomSpace { get; set; } = 0;
        public double NoRubySpace { get; set; } = 0;


        public string UnsyncFontFamily { get; set; } = "";
        public double UnsyncFontSize { get; set; } = 20;
        public string UnsyncFontStyle { get; set; } = "Normal";
        public string UnsyncFontWeight { get; set; } = "Normal";
        public string UnsyncFontStretch { get; set; } = "Normal";
        public double UnsyncOutline { get; set; } = 0;

        public string UnsyncFill { get; set; } = "White";
        public string UnsyncStroke { get; set; } = "Black";

        public string UnsyncTextAlignment { get; set; } = "Left";
        public double UnsyncOffsetLeft { get; set; } = 16;
        public double UnsyncOffsetRight { get; set; } = 16;
        public double UnsyncOffsetVertical { get; set; } = 16;
        public double UnsyncLineTopSpace { get; set; } = 0;
        public double UnsyncLineBottomSpace { get; set; } = 0;
        public double UnsyncRubyBottomSpace { get; set; } = 0;
        public double UnsyncNoRubySpace { get; set; } = 0;


        public string[] LyricsSearchList { get; set; } =
        {
            "file:%directoryname%/%filename%.kra",
            "file:%directoryname%/%filename%.lrc",
            "file:%directoryname%/%filename%.txt",
            "file:%mydocuments%/Lyrics/%artists%/%album%-%title%.lrc",
            "file:%mydocuments%/Lyrics/%artists%/%album%-%title%.txt",
            "string:<lyrics>",
            "shortcut:%path%%artists%%album%%title%",
            " plugin:",
            "set_empty:%path%%artists%%album%%title%"
        };

        public string[] ManualSearchList { get; set; } = { "" };

        public string NoLyricsFormat { get; set; } = "%artists%\n%title%\n%album%\n%path%\n";

        public bool IgnoreKaraoke { get; set; } = false;
        public int PluginTimeout { get; set; } = 30000;


        public string[] SavePathList { get; set; } =
        {
            "%mydocuments%/Lyrics/%artists%/%album%-%title",
            "%directoryname%/%filename%"
        };
        public int SaveExtension { get; set; } = 0;
        public int SaveOverwrite { get; set; } = 0;
        public bool AutoSave { get; set; } = false;



        public int WheelDelta { get; set; } = 30;

        public bool SpecifyWheelDelta { get; set; } = false;




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
        public static SettingsStorage Load(string path)
        {
            try
            {
                using FileStream fs = File.OpenRead(path);
                return JsonSerializer.DeserializeAsync<SettingsStorage>(fs).AsTask().Result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool Save() { return Save(DefaultPath); }
        public static SettingsStorage Load() { return Load(DefaultPath); }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Titalyver2
{
    public class LyricsSaver
    {

        public List<string> SaveList { get; private set; } = new();

        public enum EnumExtension { DependSync = 0, DependSync3 = 1, Lrc = 2, Txt = 3 }
        public EnumExtension Extension { get; set; } = EnumExtension.DependSync;

        public enum EnumOverwrite { Silently = 0, Dialog = 1, Dont = 2 }
        public EnumOverwrite Overwrite { get; set; } = EnumOverwrite.Silently;

        public LyricsSaver()
        { }

        public void SetSaveList(string list)
        {
            SaveList.Clear();
            using StringReader sr = new(list);
            for (string line = sr.ReadLine(); line != null; line = sr.ReadLine())
            {
                SaveList.Add(line);
            }
        }

        public bool Save(string lyrics, LyricsContainer.SyncMode sync, string musicfilepath, Dictionary<string, string[]> metaData, out string saved_path)
        {
            string directoryname = Path.GetDirectoryName(musicfilepath) ?? "";
            string filename = Path.GetFileNameWithoutExtension(musicfilepath);
            string filename_ext = Path.GetFileName(musicfilepath);

            if (sync == LyricsContainer.SyncMode.Null)
            {
                LyricsContainer container = new(lyrics);
                sync = container.Sync;
            }

            foreach (string l in SaveList)
            {
                string savename = LyricsSearchers.Replace(l, directoryname, filename, filename_ext, musicfilepath, metaData);
                string ext = "";
                switch (Extension)
                {
                    case EnumExtension.DependSync3:
                        if (sync == LyricsContainer.SyncMode.Karaoke)
                        {
                            ext = ".kra";
                            break;
                        }
                        goto case EnumExtension.DependSync;
                    case EnumExtension.DependSync:
                        ext = (sync == LyricsContainer.SyncMode.Unsync) ? ".txt" : ".lrc";
                        break;
                    case EnumExtension.Lrc:
                        ext = ".lrc";
                        break;
                    case EnumExtension.Txt:
                        ext = ".txt";
                        break;
                }
                saved_path = savename + ext;
                try
                {
                    if (File.Exists(saved_path))
                    {
                        switch(Overwrite)
                        {
                            case EnumOverwrite.Dont:
                                continue;
                            case EnumOverwrite.Silently:
                                break;
                            case EnumOverwrite.Dialog:
                                if (MessageBox.Show("OverWrite?\n" + saved_path, "Save Lyrics", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                                {
                                    continue;
                                }
                                break;
                        }
                    }
                    else
                    {
                        string saved_d = Path.GetDirectoryName(saved_path);
                        if (!Directory.Exists(saved_d))
                        {
                            Directory.CreateDirectory(saved_d);
                        }
                    }
                    File.WriteAllText(saved_path, lyrics);
                    return true;
                }
                catch (Exception e)
                {
                }
            }
            saved_path = null;
            return false;
        }

    }
}

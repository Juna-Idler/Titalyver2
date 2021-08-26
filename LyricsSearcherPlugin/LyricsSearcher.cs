

//アセンブリ名はユニークなものへ変更
//
//ここの名前空間名、クラス名、メソッド名は全てこのまま固定

namespace Titalyver2
{
    public class LyricsSearcher
    {
        public string[] Search(string title,
                               string[] artists,
                               string album,
                               string path,     //ファイルの場合ファイルのパス
                               string param)    //SeachListに書かれたパラメータ
        {
            string testoutput = "Test Plugin String\n" + title + "\n" + artists[0] + "\n" + album + "\n" + path + "\n" + param;
            return new string[] { testoutput };
        }
    }
}

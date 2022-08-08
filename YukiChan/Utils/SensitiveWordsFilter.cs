using System.Collections;
using System.Text;

namespace YukiChan.Utils;

public static class SensitiveWordsFilter
{
    private static Hashtable _map = new();
    public const string WordsDirectory = "Assets/SensitiveWords";

    public static void Initialize()
    {
        var wordList = new List<string>();
        foreach (var file in Directory.GetFiles(WordsDirectory))
        {
            var txt = File.ReadAllText(file).ToLower();
            wordList.AddRange(txt.Split(","));
        }

        _map = new Hashtable(wordList.Count);
        foreach (var word in wordList)
        {
            var indexMap = _map;
            for (var i = 0; i < word.Length; i++)
            {
                var c = word[i];
                if (indexMap.ContainsKey(c))
                    indexMap = (Hashtable)indexMap[c]!;
                else
                {
                    var newMap = new Hashtable();
                    newMap.Add("IsEnd", false);
                    indexMap.Add(c, newMap);
                    indexMap = newMap;
                }

                if (i == word.Length - 1)
                {
                    if (indexMap.ContainsKey("IsEnd"))
                        indexMap["IsEnd"] = true;
                    else
                        indexMap.Add("IsEnd", true);
                }
            }
        }
    }

    private static int CheckWord(string text, int beginIndex)
    {
        var flag = false;
        var length = 0;
        var currentMap = _map;
        for (var i = beginIndex; i < text.Length; i++)
        {
            var c = text[i];
            if (currentMap[c] is Hashtable tempTable)
            {
                if ((bool)tempTable["IsEnd"]!) flag = true;
                else currentMap = tempTable;
                length++;
            }
            else break;
        }

        if (!flag) length = 0;
        return length;
    }

    public static string ReplaceSensitiveWords(this string txt)
    {
        var text = txt.ToLower();
        var i = 0;
        var sb = new StringBuilder(text);
        while (i < text.Length)
        {
            var length = CheckWord(text, i);
            if (length > 0)
            {
                for (var j = 0; j < length; j++)
                    sb[i + j] = '*';
                i += length;
            }
            else i++;
        }

        return sb.ToString();
    }

    public static bool HasSensitiveWords(this string txt)
    {
        var text = txt.ToLower();
        var i = 0;
        while (i < text.Length)
        {
            var length = CheckWord(text, i);
            if (length > 0) return true;
            i++;
        }

        return false;
    }
}
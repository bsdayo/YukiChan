﻿using SQLite;
using YukiChan.Modules.Arcaea;
using YukiChan.Modules.Arcaea.Models;
using YukiChan.Modules.Phrase;

// ReSharper disable CheckNamespace

namespace YukiChan.Database;

[Table("phrase_words")]
public class PhraseWord
{
    [PrimaryKey] [Column("word")]
    public string Word { get; set; }
    
    // noun, verb...
    [Column("type")]
    public string Type { get; set; }
}

[YukiDatabase(PhraseDbName, typeof(PhraseWord))]
public partial class YukiDbManager
{
    private const string PhraseDbName = "Phrase";

    public void AddPhraseWord(string word, string type)
    {
        Databases[PhraseDbName].InsertOrReplace(new PhraseWord
        {
            Word = word,
            Type = type
        }, typeof(PhraseWord));
    }

    public void RemovePhraseWord(string word)
    {
        Databases[PhraseDbName].Delete<PhraseWord>(new PhraseWord
        {
            Word = word
        });
    }

    public string GetPhraseWord(string type)
    {
        var phraseWord = Databases[PhraseDbName].FindWithQuery<PhraseWord>(
            "SELECT * FROM phrase_words WHERE type = ?", type);

        return phraseWord?.Word ?? "";
    }
}
namespace JustWordle.Core;

public static class WordLoader
{
    public static string[] WordList { get; private set; }

    public static void Load(string path)
    {
        WordList = File.ReadAllLines(path);
        for (int i = 0; i < WordList.Length; i++)
        {
            WordList[i] = WordList[i].ToUpper();
        }
    }
}

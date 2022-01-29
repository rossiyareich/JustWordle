namespace JustWordle.Core;

public class WordleGame
{
    public WordleGame(string correctWord)
    {
        GuessedWords = new List<string>(6);
        CorrectWord = correctWord;
    }

    public int GameStatus => GuessedWords.Count >= 6 ? 1 : GuessedWords.Contains(CorrectWord) ? 2 : 0;
    public string CorrectWord { get; }
    public List<string> GuessedWords { get; }
    public int CurrentGuessCount => GuessedWords.Count;
    public event Action<(char letter, int code)[], int, int, int> OnQueryFinished;

    public void KeyInWord(string word)
    {
        (char letter, int code)[] result = new (char letter, int code)[5];
        GuessedWords.Add(word);
        for (int i = 0; i < 5; i++)
        {
            result[i].letter = word[i];
            if (CorrectWord[i] == word[i])
            {
                result[i].code = 2;
            }
            else if (CorrectWord.Contains(word[i]) && !result.Any(x => x.code == 1 && x.letter == word[i]))
            {
                result[i].code = 1;
            }
            else
            {
                result[i].code = 0;
            }
        }

        OnQueryFinished?.Invoke(result, GameStatus, GuessedWords.IndexOf(word), GuessedWords.IndexOf(CorrectWord));
    }
}

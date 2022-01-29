namespace JustWordle.Core;

public class WordleHint
{
    private readonly char[] _vowels = { 'A', 'E', 'I', 'O', 'U', 'Y' };

    string[] _wordList;
    List<string> predictions;

    List<(char x, int pos)> greenExceptions = new();

    public WordleHint(string[] wordList)
    {
        _wordList = wordList;
        predictions = _wordList.ToList();
    }

    public string GetPrediction((char letter, int code)[] result, IEnumerable<string> exception)
    {
        if (result == null)
        {
            return null;
        }

        predictions.RemoveAll(exception.Contains);
        for (int i = 0; i < result.Length; i++)
        {
            if (result[i].code == 0)
            {
                predictions.RemoveAll(x =>
                    x.Contains(result[i].letter) &&
                    !greenExceptions.Contains((x[i], i)));
            }
            else if (result[i].code == 1)
            {
                predictions.RemoveAll(x =>
                    !x.Contains(result[i].letter) ||
                    x[i] == result[i].letter);
            }
            else if (result[i].code == 2)
            {
                greenExceptions.Add((result[i].letter, i));
                predictions.RemoveAll(x => x[i] != result[i].letter);
            }
        }

        predictions = predictions.Count <= 0 ? _wordList.ToList() : predictions;
        predictions.RemoveAll(exception.Contains);
        return predictions.OrderBy(x => x.Count(_vowels.Contains))?.LastOrDefault();
    }
}

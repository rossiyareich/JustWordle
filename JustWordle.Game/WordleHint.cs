namespace JustWordle.Core;

public class WordleHint
{
    private readonly char[] _vowels = { 'A', 'E', 'I', 'O', 'U', 'Y' };

    private readonly string[] _wordList;

    public WordleHint(string[] wordList) => _wordList = wordList;

    public string GetPrediction((char letter, int code)[] result, IEnumerable<string> exception)
    {
        if (result == null)
        {
            return null;
        }

        IEnumerable<string> exclusive = _wordList.Where(x => !exception.Contains(x));
        List<string> predictions = exclusive.ToList();
        List<(char x, int pos)> greenExceptions = new();
        for (int i = 0; i < result.Length; i++)
        {
            if (result[i].code == 0)
            {
                predictions.RemoveAll(x =>
                    x.Contains(result[i].letter) &&
                    greenExceptions.All(y => y.pos != i && y.x != result[i].letter));
            }
            else if (result[i].code == 1)
            {
                predictions.RemoveAll(x =>
                    !x.Contains(result[i].letter) ||
                    x.Select((y, j) => (j, y)).Contains((i, result[i].letter)));
            }
            else if (result[i].code == 2)
            {
                greenExceptions.Add((result[i].letter, i));
                predictions.RemoveAll(x => !x.Select((y, j) => (j, y)).Contains((i, result[i].letter)));
            }
        }

        return predictions.OrderBy(x => x.Count(_vowels.Contains))?.LastOrDefault() ??
               exclusive.ToArray()[new Random().Next(0, exclusive.Count())];
    }
}

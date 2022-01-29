using JustWordle.Core;
using static JustWordle.Core.WordLoader;

Load(@"wordle-answers-alphabetical.txt");

Random rng = new();
WordleHint wordleHint;
string lastValidPrediction;
string correctWord;
WordleGame wordleGame;
List<(char letter, int code)[]> guessTable;

string whiteSpaces = string.Concat(Enumerable.Repeat(Environment.NewLine, 2));

InitializeGame();

void InitializeGame()
{
    correctWord = WordList[rng.Next(0, WordList.Length)];
    wordleGame = new WordleGame(correctWord);
    guessTable = new List<(char letter, int code)[]>(6);
    wordleHint = new(WordList);
    lastValidPrediction = null;

    for (int i = 0; i < 6; i++)
    {
        (char letter, int code)[] x = new (char letter, int code)[5];
        Array.Fill(x, ('#', -1));
        guessTable.Add(x);
    }

    Console.ForegroundColor = ConsoleColor.White;

    wordleGame.OnQueryFinished += OnQueryFinished;
    SkipFrame();
}

void DeleteGame()
{
    wordleGame.OnQueryFinished -= OnQueryFinished;
}

void OnQueryFinished((char letter, int code)[] @return, int gameStatus, int guessedWordIndex, int correctWordIndex)
{
    switch (gameStatus)
    {
        case 0:
            {
                NextFrame(@return, guessedWordIndex);
            }
            break;
        case 1:
            do
            {
                InsertGrid(@return, guessedWordIndex);
                Console.Write($"{whiteSpaces}You lost. The correct word was {correctWord}!  Continue? (y/n) ");
            } while (!Retry());

            break;
        case 2:
            {
                do
                {
                    InsertGrid(@return, guessedWordIndex);
                    Console.Write($"{whiteSpaces}You won in {guessedWordIndex + 1} tries! Continue? (y/n) ");
                } while (!Retry());
            }
            break;
        default:
            {
                throw new ArgumentOutOfRangeException("Unknown status");
            }
    }
}

bool Retry()
{
    char key = char.ToLower(Console.ReadKey().KeyChar);
    if (key == 'y')
    {
        DeleteGame();
        InitializeGame();
        return true;
    }
    else if (key == 'n')
    {
        Console.WriteLine("OK, you have a great day");
        return true;
    }
    else
    {
        Console.WriteLine("\nInvalid input, try again.");
        Thread.Sleep(1000);
        return false;
    }
}

void InsertGrid((char letter, int code)[] @return, int guessedWordIndex)
{
    if (guessedWordIndex != -1)
    {
        guessTable[guessedWordIndex] = @return;
    }

    Console.Clear();
    Console.SetCursorPosition(0, 0);

    foreach ((char letter, int code)[] row in guessTable)
    {
        foreach ((char letter, int code) result in row)
        {
            Console.BackgroundColor = result.code switch
            {
                -1 => ConsoleColor.Black,
                0 => ConsoleColor.Gray,
                1 => ConsoleColor.DarkYellow,
                2 => ConsoleColor.DarkGreen,
                _ => throw new ArgumentOutOfRangeException("Unknown result")
            };
            Console.Write(result.letter);
        }

        Console.WriteLine();
    }

    Console.BackgroundColor = ConsoleColor.Black;
}

void NextFrame((char letter, int code)[] @return, int guessedWordIndex)
{
    InsertGrid(@return, guessedWordIndex);
    lastValidPrediction = wordleHint.GetPrediction(@return, wordleGame.GuessedWords) ?? lastValidPrediction;
    Console.WriteLine($"\nPrediction: {lastValidPrediction}");

    Console.Write($"{whiteSpaces}Waiting for input... - ");
    string input = Console.ReadLine().ToUpper();
    if (input.Length <= 0)
    {
        SkipFrame();
    }
    else if (input.Length != 5)
    {
        Console.WriteLine("Invalid length");
        Thread.Sleep(1000);
        SkipFrame();
    }
    else if (!input.All(char.IsLetter))
    {
        Console.WriteLine("Invalid character(s)");
        Thread.Sleep(1000);
        SkipFrame();
    }
    else if (!WordList.Contains(input))
    {
        Console.WriteLine("Word does not exist");
        Thread.Sleep(1000);
        SkipFrame();
    }
    else if (wordleGame.GuessedWords.Contains(input))
    {
        Console.WriteLine("You already guessed the input!");
        Thread.Sleep(1000);
        SkipFrame();
    }
    else
    {
        wordleGame.KeyInWord(input);
    }
}

void SkipFrame()
{
    NextFrame(null, -1);
}

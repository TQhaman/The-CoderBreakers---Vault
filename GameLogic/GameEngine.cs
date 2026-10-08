namespace The_codebreakers___The_Vault.GameLogic
{
    public sealed class GameEngine
    {
        private readonly List<GuessResult> _guessHistory = new List<GuessResult>();
        private string _secretCode = string.Empty;

        public GameEngine(int codeLength = 3)
        {
            StartNewGame(codeLength);
        }

        public int CodeLength { get; private set; }

        public int GuessCount { get; private set; }

        public bool IsGameActive { get; private set; }

        public bool IsGameWon { get; private set; }

        public IReadOnlyList<GuessResult> GuessHistory => _guessHistory.AsReadOnly();

        public string? RevealedSecretCode => IsGameWon ? _secretCode : null;

        public void StartNewGame()
        {
            StartNewGame(CodeLength);
        }

        public void StartNewGame(int codeLength)
        {
            if (codeLength != 3 && codeLength != 4)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(codeLength),
                    "The code length must be either 3 or 4 digits.");
            }

            CodeLength = codeLength;
            _secretCode = GenerateSecretCode(codeLength);
            GuessCount = 0;
            _guessHistory.Clear();
            IsGameWon = false;
            IsGameActive = true;
        }

        public GuessResult SubmitGuess(string? guess)
        {
            string enteredGuess = guess ?? string.Empty;

            if (!IsGameActive)
            {
                return GuessResult.Invalid(
                    enteredGuess,
                    "This game has already been won. Start a new game to continue.");
            }

            string? validationError = GetValidationError(guess);

            if (validationError is not null)
            {
                return GuessResult.Invalid(enteredGuess, validationError);
            }

            (int hits, int matches) = CalculateScore(enteredGuess);
            GuessCount++;

            bool isWin = hits == CodeLength;

            if (isWin)
            {
                IsGameWon = true;
                IsGameActive = false;
            }

            GuessResult result = GuessResult.Valid(
                enteredGuess,
                hits,
                matches,
                GuessCount,
                isWin);

            _guessHistory.Add(result);
            return result;
        }

        private static string GenerateSecretCode(int codeLength)
        {
            var availableDigits = new List<char>
            {
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
            };

            var secretDigits = new char[codeLength];

            int firstDigitIndex = Random.Shared.Next(1, availableDigits.Count);
            secretDigits[0] = availableDigits[firstDigitIndex];
            availableDigits.RemoveAt(firstDigitIndex);

            for (int position = 1; position < codeLength; position++)
            {
                int randomIndex = Random.Shared.Next(availableDigits.Count);
                secretDigits[position] = availableDigits[randomIndex];
                availableDigits.RemoveAt(randomIndex);
            }

            return new string(secretDigits);
        }

        private string? GetValidationError(string? guess)
        {
            if (string.IsNullOrEmpty(guess))
            {
                return "Enter a guess before checking it.";
            }

            foreach (char character in guess)
            {
                if (char.IsWhiteSpace(character))
                {
                    return "The guess must not contain spaces.";
                }
            }

            if (guess.Length != CodeLength)
            {
                return $"Enter exactly {CodeLength} digits.";
            }

            foreach (char character in guess)
            {
                if (character < '0' || character > '9')
                {
                    return "The guess may contain digits only.";
                }
            }

            if (guess[0] == '0')
            {
                return "The first digit cannot be zero.";
            }

            var usedDigits = new HashSet<char>();

            foreach (char digit in guess)
            {
                if (!usedDigits.Add(digit))
                {
                    return "Each digit in the guess must be different.";
                }
            }

            return null;
        }

        private (int Hits, int Matches) CalculateScore(string guess)
        {
            int hits = 0;
            int matches = 0;
            var usedSecretPositions = new bool[CodeLength];
            var usedGuessPositions = new bool[CodeLength];

            for (int position = 0; position < CodeLength; position++)
            {
                if (_secretCode[position] == guess[position])
                {
                    hits++;
                    usedSecretPositions[position] = true;
                    usedGuessPositions[position] = true;
                }
            }

            for (int guessPosition = 0; guessPosition < CodeLength; guessPosition++)
            {
                if (usedGuessPositions[guessPosition])
                {
                    continue;
                }

                for (int secretPosition = 0; secretPosition < CodeLength; secretPosition++)
                {
                    if (usedSecretPositions[secretPosition])
                    {
                        continue;
                    }

                    if (guess[guessPosition] == _secretCode[secretPosition])
                    {
                        matches++;
                        usedSecretPositions[secretPosition] = true;
                        usedGuessPositions[guessPosition] = true;
                        break;
                    }
                }
            }

            return (hits, matches);
        }
    }
}

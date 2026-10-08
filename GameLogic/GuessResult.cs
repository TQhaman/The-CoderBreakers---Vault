namespace The_codebreakers___The_Vault.GameLogic
{
    public sealed class GuessResult
    {
        private GuessResult(
            string guess,
            bool isValid,
            string? errorMessage,
            int hits,
            int matches,
            int guessNumber,
            bool isWin)
        {
            Guess = guess;
            IsValid = isValid;
            ErrorMessage = errorMessage;
            Hits = hits;
            Matches = matches;
            GuessNumber = guessNumber;
            IsWin = isWin;
        }

        public string Guess { get; }

        public bool IsValid { get; }

        public string? ErrorMessage { get; }

        public int Hits { get; }

        public int Matches { get; }

        public int GuessNumber { get; }

        public bool IsWin { get; }

        internal static GuessResult Invalid(string guess, string errorMessage)
        {
            return new GuessResult(guess, false, errorMessage, 0, 0, 0, false);
        }

        internal static GuessResult Valid(
            string guess,
            int hits,
            int matches,
            int guessNumber,
            bool isWin)
        {
            return new GuessResult(guess, true, null, hits, matches, guessNumber, isWin);
        }
    }
}

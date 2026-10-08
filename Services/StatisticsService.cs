using Microsoft.Maui.Storage;

namespace The_codebreakers___The_Vault.Services
{
    public sealed class DifficultyStatistics
    {
        internal DifficultyStatistics(
            int gamesStarted,
            int gamesWon,
            int bestGuessCount,
            int bestTimeSeconds)
        {
            GamesStarted = gamesStarted;
            GamesWon = gamesWon;
            BestGuessCount = bestGuessCount;
            BestTimeSeconds = bestTimeSeconds;
        }

        public int GamesStarted { get; }

        public int GamesWon { get; }

        public int BestGuessCount { get; }

        public int BestTimeSeconds { get; }

        public bool HasBestGuessCount => BestGuessCount > 0;

        public bool HasBestTime => BestTimeSeconds >= 0;

        public double WinRatePercentage => GamesStarted == 0
            ? 0
            : (double)GamesWon / GamesStarted * 100;
    }

    public static class StatisticsService
    {
        private const string PlayerNicknameKey = "Vault.PlayerNickname";

        private const string NormalGamesStartedKey = "Vault.Stats.3.GamesStarted";
        private const string NormalGamesWonKey = "Vault.Stats.3.GamesWon";
        private const string NormalBestGuessCountKey = "Vault.Stats.3.BestGuessCount";
        private const string NormalBestTimeSecondsKey = "Vault.Stats.3.BestTimeSeconds";

        private const string ExpertGamesStartedKey = "Vault.Stats.4.GamesStarted";
        private const string ExpertGamesWonKey = "Vault.Stats.4.GamesWon";
        private const string ExpertBestGuessCountKey = "Vault.Stats.4.BestGuessCount";
        private const string ExpertBestTimeSecondsKey = "Vault.Stats.4.BestTimeSeconds";

        public static string GetPlayerNickname()
        {
            return Preferences.Default.Get(PlayerNicknameKey, string.Empty);
        }

        public static void SavePlayerNickname(string nickname)
        {
            string trimmedNickname = nickname.Trim();

            if (!string.IsNullOrWhiteSpace(trimmedNickname))
            {
                Preferences.Default.Set(PlayerNicknameKey, trimmedNickname);
            }
        }

        public static DifficultyStatistics GetStatistics(int codeLength)
        {
            var keys = GetKeys(codeLength);

            return new DifficultyStatistics(
                Preferences.Default.Get(keys.GamesStarted, 0),
                Preferences.Default.Get(keys.GamesWon, 0),
                Preferences.Default.Get(keys.BestGuessCount, 0),
                Preferences.Default.Get(keys.BestTimeSeconds, -1));
        }

        public static void RecordGameStarted(int codeLength)
        {
            var keys = GetKeys(codeLength);
            int currentGamesStarted = Preferences.Default.Get(keys.GamesStarted, 0);
            Preferences.Default.Set(keys.GamesStarted, currentGamesStarted + 1);
        }

        public static void RecordWin(int codeLength, int guessCount, int elapsedSeconds)
        {
            if (guessCount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(guessCount),
                    "A winning game must contain at least one valid guess.");
            }

            if (elapsedSeconds < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(elapsedSeconds),
                    "Elapsed time cannot be negative.");
            }

            var keys = GetKeys(codeLength);
            int currentGamesWon = Preferences.Default.Get(keys.GamesWon, 0);
            Preferences.Default.Set(keys.GamesWon, currentGamesWon + 1);

            int currentBestGuessCount = Preferences.Default.Get(keys.BestGuessCount, 0);

            if (currentBestGuessCount == 0 || guessCount < currentBestGuessCount)
            {
                Preferences.Default.Set(keys.BestGuessCount, guessCount);
            }

            int currentBestTimeSeconds = Preferences.Default.Get(keys.BestTimeSeconds, -1);

            if (currentBestTimeSeconds < 0 || elapsedSeconds < currentBestTimeSeconds)
            {
                Preferences.Default.Set(keys.BestTimeSeconds, elapsedSeconds);
            }
        }

        public static void ResetStatistics()
        {
            foreach (string key in GetAllStatisticsKeys())
            {
                Preferences.Default.Remove(key);
            }
        }

        private static (
            string GamesStarted,
            string GamesWon,
            string BestGuessCount,
            string BestTimeSeconds) GetKeys(int codeLength)
        {
            return codeLength switch
            {
                3 => (
                    NormalGamesStartedKey,
                    NormalGamesWonKey,
                    NormalBestGuessCountKey,
                    NormalBestTimeSecondsKey),
                4 => (
                    ExpertGamesStartedKey,
                    ExpertGamesWonKey,
                    ExpertBestGuessCountKey,
                    ExpertBestTimeSecondsKey),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(codeLength),
                    "Statistics are available only for 3-digit and 4-digit games.")
            };
        }

        private static IEnumerable<string> GetAllStatisticsKeys()
        {
            yield return NormalGamesStartedKey;
            yield return NormalGamesWonKey;
            yield return NormalBestGuessCountKey;
            yield return NormalBestTimeSecondsKey;
            yield return ExpertGamesStartedKey;
            yield return ExpertGamesWonKey;
            yield return ExpertBestGuessCountKey;
            yield return ExpertBestTimeSecondsKey;
        }
    }
}

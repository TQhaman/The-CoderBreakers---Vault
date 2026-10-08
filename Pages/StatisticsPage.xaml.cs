using The_codebreakers___The_Vault.Services;

namespace The_codebreakers___The_Vault.Pages
{
    public partial class StatisticsPage : ContentPage
    {
        public StatisticsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            RefreshStatistics();
        }

        private void RefreshStatistics()
        {
            DifficultyStatistics normalStatistics = StatisticsService.GetStatistics(3);
            DifficultyStatistics expertStatistics = StatisticsService.GetStatistics(4);

            NormalGamesPlayedLabel.Text = normalStatistics.GamesStarted.ToString();
            NormalGamesWonLabel.Text = normalStatistics.GamesWon.ToString();
            NormalWinRateLabel.Text = $"{normalStatistics.WinRatePercentage:0.#}%";
            NormalBestScoreLabel.Text = FormatBestScore(normalStatistics);
            NormalBestTimeLabel.Text = FormatBestTime(normalStatistics);

            ExpertGamesPlayedLabel.Text = expertStatistics.GamesStarted.ToString();
            ExpertGamesWonLabel.Text = expertStatistics.GamesWon.ToString();
            ExpertWinRateLabel.Text = $"{expertStatistics.WinRatePercentage:0.#}%";
            ExpertBestScoreLabel.Text = FormatBestScore(expertStatistics);
            ExpertBestTimeLabel.Text = FormatBestTime(expertStatistics);
        }

        private static string FormatBestScore(DifficultyStatistics statistics)
        {
            if (!statistics.HasBestGuessCount)
            {
                return "—";
            }

            return statistics.BestGuessCount == 1
                ? "1 guess"
                : $"{statistics.BestGuessCount} guesses";
        }

        private static string FormatBestTime(DifficultyStatistics statistics)
        {
            if (!statistics.HasBestTime)
            {
                return "—";
            }

            int totalMinutes = statistics.BestTimeSeconds / 60;
            int remainingSeconds = statistics.BestTimeSeconds % 60;
            return $"{totalMinutes:D2}:{remainingSeconds:D2}";
        }

        private async void OnResetStatisticsClicked(object? sender, EventArgs e)
        {
            bool shouldReset = await DisplayAlert(
                "Reset Statistics?",
                "This will clear Normal and Expert game statistics. Your nickname will be kept.",
                "Reset",
                "Cancel");

            if (shouldReset)
            {
                StatisticsService.ResetStatistics();
                RefreshStatistics();
            }
        }

        private async void OnHomeClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        }
    }
}

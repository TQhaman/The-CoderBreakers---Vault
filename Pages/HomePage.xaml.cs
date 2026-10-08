using The_codebreakers___The_Vault.Services;

namespace The_codebreakers___The_Vault.Pages
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();

            string savedNickname = StatisticsService.GetPlayerNickname();

            if (!string.IsNullOrWhiteSpace(savedNickname))
            {
                NicknameEntry.Text = savedNickname;
            }
        }

        private async void OnPlayClicked(object? sender, EventArgs e)
        {
            string nickname = NicknameEntry.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(nickname))
            {
                NicknameErrorLabel.Text = "Enter a nickname before starting the game.";
                NicknameErrorLabel.IsVisible = true;
                return;
            }

            NicknameErrorLabel.IsVisible = false;
            int codeLength = ExpertRadioButton.IsChecked ? 4 : 3;
            StatisticsService.SavePlayerNickname(nickname);

            var navigationParameters = new Dictionary<string, object>
            {
                ["Nickname"] = nickname,
                ["CodeLength"] = codeLength
            };

            await Shell.Current.GoToAsync(nameof(GamePage), navigationParameters);
        }

        private async void OnStatisticsClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(StatisticsPage));
        }

        private async void OnHowToPlayClicked(object? sender, EventArgs e)
        {
            await DisplayAlert(
                "How to Play",
                "The full guide will be added in the next phase. For now, enter unique digits and use the hit and match clues to crack the code.",
                "Got it");
        }
    }
}

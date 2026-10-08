namespace The_codebreakers___The_Vault.Pages
{
    public partial class ResultsPage : ContentPage, IQueryAttributable
    {
        private string _nickname = "Player";
        private int _codeLength = 3;

        public ResultsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await AnimateResultsEntranceAsync();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            _nickname = query.TryGetValue("Nickname", out object? nicknameValue)
                ? nicknameValue?.ToString() ?? "Player"
                : "Player";

            _codeLength = query.TryGetValue("CodeLength", out object? codeLengthValue)
                ? Convert.ToInt32(codeLengthValue)
                : 3;

            int guessCount = query.TryGetValue("GuessCount", out object? guessCountValue)
                ? Convert.ToInt32(guessCountValue)
                : 0;

            string secretCode = query.TryGetValue("SecretCode", out object? secretCodeValue)
                ? secretCodeValue?.ToString() ?? string.Empty
                : string.Empty;

            int elapsedSeconds = query.TryGetValue("ElapsedSeconds", out object? elapsedSecondsValue)
                ? Convert.ToInt32(elapsedSecondsValue)
                : 0;

            SuccessLabel.Text = $"Excellent work, {_nickname}!";
            PlayerResultLabel.Text = _nickname;
            DifficultyResultLabel.Text = _codeLength == 4 ? "Expert · 4 digits" : "Normal · 3 digits";
            GuessCountResultLabel.Text = guessCount.ToString();
            SecretCodeLabel.Text = secretCode;
            ElapsedTimeResultLabel.Text = FormatElapsedTime(elapsedSeconds);
        }

        private async Task AnimateResultsEntranceAsync()
        {
            ResultsContent.CancelAnimations();
            ResultsContent.Opacity = 0;
            ResultsContent.Scale = 0.88;

            try
            {
                await Task.Delay(50);

                await Task.WhenAll(
                    ResultsContent.FadeTo(1, 550, Easing.CubicOut),
                    ResultsContent.ScaleTo(1, 550, Easing.CubicOut));
            }
            catch (Exception)
            {
                // Results remain readable even if animation is unavailable.
            }
            finally
            {
                ResultsContent.Opacity = 1;
                ResultsContent.Scale = 1;
            }
        }

        private static string FormatElapsedTime(int totalSeconds)
        {
            int totalMinutes = totalSeconds / 60;
            int remainingSeconds = totalSeconds % 60;
            return $"{totalMinutes:D2}:{remainingSeconds:D2}";
        }

        private async void OnPlayAgainClicked(object? sender, EventArgs e)
        {
            var navigationParameters = new Dictionary<string, object>
            {
                ["Nickname"] = _nickname,
                ["CodeLength"] = _codeLength
            };

            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            await Shell.Current.GoToAsync(nameof(GamePage), navigationParameters);
        }

        private async void OnHomeClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        }
    }
}

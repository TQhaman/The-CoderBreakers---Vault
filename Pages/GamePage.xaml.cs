using System.Collections.ObjectModel;
using The_codebreakers___The_Vault.GameLogic;

namespace The_codebreakers___The_Vault.Pages
{
    public partial class GamePage : ContentPage, IQueryAttributable
    {
        private readonly ObservableCollection<GuessResult> _visibleHistory = new();
        private GameEngine? _gameEngine;
        private string _nickname = "Player";
        private int _codeLength = 3;

        public GamePage()
        {
            InitializeComponent();
            GuessHistoryView.ItemsSource = _visibleHistory;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("Nickname", out object? nicknameValue))
            {
                _nickname = nicknameValue?.ToString()?.Trim() ?? "Player";
            }

            if (query.TryGetValue("CodeLength", out object? codeLengthValue))
            {
                _codeLength = Convert.ToInt32(codeLengthValue);
            }

            StartGame();
        }

        private void StartGame()
        {
            _gameEngine = new GameEngine(_codeLength);
            _visibleHistory.Clear();

            PlayerLabel.Text = $"Player: {_nickname}";
            DifficultyLabel.Text = _codeLength == 4 ? "EXPERT · 4 DIGITS" : "NORMAL · 3 DIGITS";
            InstructionLabel.Text = $"Enter {_codeLength} unique digits";
            GuessCountLabel.Text = "Guesses: 0";
            GuessEntry.Text = string.Empty;
            GuessEntry.MaxLength = _codeLength;
            GuessEntry.IsEnabled = true;
            CheckGuessButton.IsEnabled = true;
            ValidationLabel.IsVisible = false;
            FeedbackLabel.IsVisible = false;
        }

        private async void OnCheckGuessClicked(object? sender, EventArgs e)
        {
            if (_gameEngine is null)
            {
                return;
            }

            GuessResult result = _gameEngine.SubmitGuess(GuessEntry.Text);

            if (!result.IsValid)
            {
                ValidationLabel.Text = result.ErrorMessage;
                ValidationLabel.IsVisible = true;
                FeedbackLabel.IsVisible = false;
                return;
            }

            ValidationLabel.IsVisible = false;
            FeedbackLabel.Text = $"{result.Hits} Hit(s)  ·  {result.Matches} Match(es)";
            FeedbackLabel.IsVisible = true;
            GuessCountLabel.Text = $"Guesses: {_gameEngine.GuessCount}";
            _visibleHistory.Add(result);
            GuessEntry.Text = string.Empty;

            if (!result.IsWin)
            {
                GuessEntry.Focus();
                return;
            }

            CheckGuessButton.IsEnabled = false;
            GuessEntry.IsEnabled = false;

            string? revealedSecretCode = _gameEngine.RevealedSecretCode;

            if (revealedSecretCode is null)
            {
                ValidationLabel.Text = "The winning code could not be loaded.";
                ValidationLabel.IsVisible = true;
                return;
            }

            var navigationParameters = new Dictionary<string, object>
            {
                ["Nickname"] = _nickname,
                ["CodeLength"] = _gameEngine.CodeLength,
                ["GuessCount"] = _gameEngine.GuessCount,
                ["SecretCode"] = revealedSecretCode
            };

            await Shell.Current.GoToAsync(nameof(ResultsPage), navigationParameters);
        }

        private async void OnQuitGameClicked(object? sender, EventArgs e)
        {
            bool shouldQuit = await DisplayAlert(
                "Quit Game?",
                "Your current game and guess history will be abandoned.",
                "Quit",
                "Cancel");

            if (shouldQuit)
            {
                await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            }
        }
    }
}

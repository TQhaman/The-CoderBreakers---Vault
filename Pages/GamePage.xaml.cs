using System.Collections.ObjectModel;
using Microsoft.Maui.Dispatching;
using The_codebreakers___The_Vault.GameLogic;

namespace The_codebreakers___The_Vault.Pages
{
    public partial class GamePage : ContentPage, IQueryAttributable
    {
        private readonly ObservableCollection<GuessResult> _visibleHistory = new();
        private GameEngine? _gameEngine;
        private IDispatcherTimer? _gameTimer;
        private string _nickname = "Player";
        private int _codeLength = 3;
        private int _elapsedSeconds;
        private bool _isQuitConfirmationOpen;

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
            StopGameTimer();
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

            StartGameTimer();
        }

        private void StartGameTimer()
        {
            _elapsedSeconds = 0;
            ElapsedTimeLabel.Text = FormatElapsedTime(_elapsedSeconds);

            _gameTimer = Dispatcher.CreateTimer();
            _gameTimer.Interval = TimeSpan.FromSeconds(1);
            _gameTimer.IsRepeating = true;
            _gameTimer.Tick += OnGameTimerTick;
            _gameTimer.Start();
        }

        private void OnGameTimerTick(object? sender, EventArgs e)
        {
            _elapsedSeconds++;
            ElapsedTimeLabel.Text = FormatElapsedTime(_elapsedSeconds);
        }

        private void StopGameTimer()
        {
            if (_gameTimer is null)
            {
                return;
            }

            _gameTimer.Stop();
            _gameTimer.Tick -= OnGameTimerTick;
            _gameTimer = null;
        }

        private static string FormatElapsedTime(int totalSeconds)
        {
            int totalMinutes = totalSeconds / 60;
            int remainingSeconds = totalSeconds % 60;
            return $"{totalMinutes:D2}:{remainingSeconds:D2}";
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

            StopGameTimer();
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
                ["SecretCode"] = revealedSecretCode,
                ["ElapsedSeconds"] = _elapsedSeconds
            };

            await Shell.Current.GoToAsync(nameof(ResultsPage), navigationParameters);
        }

        private async void OnQuitGameClicked(object? sender, EventArgs e)
        {
            _isQuitConfirmationOpen = true;
            bool shouldQuit;

            try
            {
                shouldQuit = await DisplayAlert(
                    "Quit Game?",
                    "Your current game and guess history will be abandoned.",
                    "Quit",
                    "Cancel");
            }
            finally
            {
                _isQuitConfirmationOpen = false;
            }

            if (shouldQuit)
            {
                StopGameTimer();
                await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            }
        }

        protected override void OnDisappearing()
        {
            if (!_isQuitConfirmationOpen)
            {
                StopGameTimer();
            }

            base.OnDisappearing();
        }
    }
}

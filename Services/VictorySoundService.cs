using System.Diagnostics;
using Plugin.Maui.Audio;

namespace The_codebreakers___The_Vault.Services
{
    public static class VictorySoundService
    {
        private const string VictorySoundFileName = "vault_success.mp3";
        private static IAudioPlayer? _currentPlayer;

        public static async Task PlayAsync()
        {
            try
            {
                DisposeCurrentPlayer();

                Stream audioStream = await FileSystem.Current.OpenAppPackageFileAsync(
                    VictorySoundFileName);
                IAudioPlayer player = AudioManager.Current.CreatePlayer(audioStream);

                player.PlaybackEnded += OnPlaybackEnded;
                _currentPlayer = player;
                player.Play();
            }
            catch (Exception exception)
            {
#if DEBUG
                Debug.WriteLine($"VAULT victory sound failed: {exception.Message}");
#endif
                DisposeCurrentPlayer();
            }
        }

        private static void OnPlaybackEnded(object? sender, EventArgs e)
        {
            if (sender is not IAudioPlayer completedPlayer)
            {
                DisposeCurrentPlayer();
                return;
            }

            try
            {
                completedPlayer.PlaybackEnded -= OnPlaybackEnded;
                completedPlayer.Dispose();
            }
            catch (Exception exception)
            {
#if DEBUG
                Debug.WriteLine($"VAULT victory sound cleanup failed: {exception.Message}");
#endif
            }
            finally
            {
                if (ReferenceEquals(_currentPlayer, completedPlayer))
                {
                    _currentPlayer = null;
                }
            }
        }

        private static void DisposeCurrentPlayer()
        {
            if (_currentPlayer is null)
            {
                return;
            }

            try
            {
                _currentPlayer.PlaybackEnded -= OnPlaybackEnded;
                _currentPlayer.Dispose();
            }
            catch (Exception exception)
            {
#if DEBUG
                Debug.WriteLine($"VAULT victory sound cleanup failed: {exception.Message}");
#endif
            }
            finally
            {
                _currentPlayer = null;
            }
        }
    }
}

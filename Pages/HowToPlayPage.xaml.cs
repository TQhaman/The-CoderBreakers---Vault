namespace The_codebreakers___The_Vault.Pages
{
    public partial class HowToPlayPage : ContentPage
    {
        public HowToPlayPage()
        {
            InitializeComponent();
        }

        private async void OnBackClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

namespace The_codebreakers___The_Vault
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(Pages.GamePage), typeof(Pages.GamePage));
            Routing.RegisterRoute(nameof(Pages.ResultsPage), typeof(Pages.ResultsPage));
            Routing.RegisterRoute(nameof(Pages.StatisticsPage), typeof(Pages.StatisticsPage));
        }
    }
}

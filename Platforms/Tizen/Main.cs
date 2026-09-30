using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace The_codebreakers___The_Vault
{
    internal class Program : MauiApplication
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }
    }
}

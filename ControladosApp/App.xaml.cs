using ControladosApp.Services;

namespace ControladosApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
        //MainPage = new NavigationPage(new MainPage());
    }
	
	protected override Window CreateWindow(IActivationState? activationState)
	{
        //return new Window(new AppShell());
        return new Window(new NavigationPage(new MainPage()));
    }
	
}
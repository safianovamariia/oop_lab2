namespace ScheduleApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell())
		{
			Title = "Display Class Schedule",
			Width = 1100,
			Height = 860
		};
	}
}
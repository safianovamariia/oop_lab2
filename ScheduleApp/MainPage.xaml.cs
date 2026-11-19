using System.Xml;
using System.Xml.Xsl;
using ScheduleApp.Models;

namespace ScheduleApp;

public partial class MainPage : ContentPage
{
    private const string AllValues = "All";

    private readonly ScheduleController controller = new();
    private bool isExitDialogOpen;

    internal static bool ExitConfirmed { get; private set; }

    public MainPage()
    {
        InitializeComponent();
    }

    private async void BtnClickOpenFile(object? sender, EventArgs e)
    {
        ShowMessage("Choose the XML file with the schedule.");
        var options = new PickOptions
        {
            PickerTitle = "Choose the XML file with the schedule",
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                [DevicePlatform.WinUI] = new[] { ".xml" }
            })
        };

        FileResult? file = await FilePicker.Default.PickAsync(options);
        if (file != null)
            SelectFile(file.FullPath);
        else
            ShowMessage("");
    }

    public void SelectFile(string path)
    {
        try
        {
            Dictionary<string, List<string>> values = controller.OpenFile(path);
            ShowAttributeValues(values);
            ResultsView.ItemsSource = null;
            ShowMessage($"File opened: {Path.GetFileName(path)}");
        }
        catch (Exception ex) when (ex is XmlException or IOException or InvalidOperationException)
        {
            ShowMessage($"Cannot open the file: {ex.Message}");
        }
    }

    public void ShowAttributeValues(Dictionary<string, List<string>> values)
    {
        FillPicker(RoomPicker, values["room"]);
        FillPicker(DayPicker, values["day"]);
        FillPicker(TeacherPicker, values["teacher"]);
        FillPicker(GroupPicker, values["group"]);
        FillPicker(SubjectPicker, values["subject"]);
    }

    private void OnRoomChanged(object? sender, EventArgs e) =>
        SelectAttributeValue("room", GetSelectedValue(RoomPicker));

    private void OnDayChanged(object? sender, EventArgs e) =>
        SelectAttributeValue("day", GetSelectedValue(DayPicker));

    private void OnTeacherChanged(object? sender, EventArgs e) =>
        SelectAttributeValue("teacher", GetSelectedValue(TeacherPicker));

    private void OnGroupChanged(object? sender, EventArgs e) =>
        SelectAttributeValue("group", GetSelectedValue(GroupPicker));

    private void OnSubjectChanged(object? sender, EventArgs e) =>
        SelectAttributeValue("subject", GetSelectedValue(SubjectPicker));

    public void SelectAttributeValue(string attribute, string value)
    {
        controller.AddFilter(attribute, value);
    }

    private void OnStrategyChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (e.Value && sender is RadioButton radio)
            SelectStrategy(radio.Content?.ToString() ?? "");
    }

    public void SelectStrategy(string strategyName)
    {
        controller.SetStrategy(strategyName);
    }

    private void BtnClickSearch(object? sender, EventArgs e)
    {
        try
        {
            ShowResults(controller.Search());
        }
        catch (Exception ex) when (ex is InvalidOperationException or XmlException or IOException)
        {
            ShowMessage(ex.Message);
        }
    }

    public void ShowResults(List<Session> sessions)
    {
        ResultsView.ItemsSource = sessions.Select(s => s.GetInfo()).ToList();
        ShowMessage(sessions.Count == 0 ? "Nothing found." : $"Records found: {sessions.Count}");
    }

    private async void BtnClickTransform(object? sender, EventArgs e)
    {
        try
        {
            if (controller.TransformToHtml("schedule.xsl", "schedule.html"))
            {
                ShowMessage($"HTML file saved: {controller.HtmlFilePath}");
                await OpenHtmlInBrowser(controller.HtmlFilePath);
            }
            else
            {
                ShowMessage("The HTML file was not created.");
            }
        }
        catch (Exception ex) when (ex is InvalidOperationException or XsltException or XmlException
                                      or IOException or UnauthorizedAccessException)
        {
            ShowMessage($"Cannot transform the file: {ex.Message}");
        }
    }

    public async Task OpenHtmlInBrowser(string htmlPath)
    {
        await Launcher.Default.OpenAsync(new OpenFileRequest("Schedule", new ReadOnlyFile(htmlPath)));
    }

    private void BtnClickClear(object? sender, EventArgs e)
    {
        controller.Clear();
        ResetPicker(RoomPicker);
        ResetPicker(DayPicker);
        ResetPicker(TeacherPicker);
        ResetPicker(GroupPicker);
        ResetPicker(SubjectPicker);
        SaxRadio.IsChecked = false;
        DomRadio.IsChecked = false;
        LinqRadio.IsChecked = false;
        ResultsView.ItemsSource = null;
        ShowMessage("");
    }

    private async void BtnClickExit(object? sender, EventArgs e)
    {
        if (await ConfirmExit())
            Application.Current?.Quit();
    }

    public async Task<bool> ConfirmExit()
    {
        if (isExitDialogOpen)
            return false;

        isExitDialogOpen = true;
        try
        {
            ExitConfirmed = await DisplayAlertAsync("Exit",
                "Do you really want to exit the program?", "Yes", "No");
            return ExitConfirmed;
        }
        finally
        {
            isExitDialogOpen = false;
        }
    }

    public void ShowMessage(string text)
    {
        MessageLabel.Text = text;
    }

    private static void FillPicker(Picker picker, List<string> values)
    {
        picker.Title = "";
        picker.ItemsSource = new List<string> { AllValues }.Concat(values).ToList();
        picker.SelectedIndex = 0;
    }

    private static void ResetPicker(Picker picker)
    {
        picker.SelectedIndex = picker.ItemsSource?.Count > 0 ? 0 : -1;
    }

    private static string GetSelectedValue(Picker picker)
    {
        return picker.SelectedIndex <= 0 ? "" : picker.SelectedItem?.ToString() ?? "";
    }
}

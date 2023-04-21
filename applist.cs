// WinForms panel that launches apps and URLs from a list

// . Add a `FlowLayoutPanel` control to the panel. This will be used to display the list of apps and URLs.

public class AppInfo
{
    public string Name { get; set; }
    public string Path { get; set; }
    public bool IsUrl { get; set; }
}

private List<AppInfo> _appInfos = new List<AppInfo>();

_appInfos.Add(new AppInfo
{
    Name = "Google",
    Path = "https://www.google.com/",
    IsUrl = true
});

_appInfos.Add(new AppInfo
{
    Name = "Notepad",
    Path = "notepad.exe",
    IsUrl = false
});

private void LoadButtons()
{
    foreach (var appInfo in _appInfos)
    {
        var button = new Button
        {
            Text = appInfo.Name
        };

        if (appInfo.IsUrl)
        {
            button.Click += (sender, e) => Process.Start(appInfo.Path);
        }
        else
        {
            button.Click += (sender, e) => Process.Start(new ProcessStartInfo
            {
                FileName = appInfo.Path
            });
        }

        flowLayoutPanel1.Controls.Add(button);
    }
}
```

public Form1()
{
    InitializeComponent();
    LoadButtons();
}

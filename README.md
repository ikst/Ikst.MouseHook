# Ikst.MouseHook
This is a library to realize global mouse hooks in .net windows applications.
Mouse events can be triggered for applications that do not have a screen, or for mouse operations outside of a window.
The events that can occur are as follows
- MouseMove
- MouseWheel
- LeftButtonDown
- LeftButtonUp
- RightButtonDown
- RightButtonUp
- MiddleButtonDown
- MiddleButtonUp

All events contain a [MSLLHOOKSTRUCT](https://docs.microsoft.com/windows/win32/api/winuser/ns-winuser-msllhookstruct)  structure.

### usage
- Create an instance and use the start method to start the hook.
- The event is triggered by a mouse operation.
- In the event, you can get the MSLLHOOKSTRUCT structure.

[e.g.] Display the mouse coordinates of the left click in the window title.
```C#
public partial class MainWindow : Window
{
    private readonly MouseHook mh = new MouseHook();

    public MainWindow()
    {
        InitializeComponent();

        mh.LeftButtonDown += (st) =>
        {
            this.Title = $"X:{st.pt.x} Y:{st.pt.y}";
        };

        mh.Start();
    }
}
```

## nuget
https://www.nuget.org/packages/Ikst.MouseHook/

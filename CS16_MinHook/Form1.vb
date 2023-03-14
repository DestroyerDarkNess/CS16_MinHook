Imports ProcessHacker
Imports System.Runtime.InteropServices
Imports System.Threading
Imports Memory

Public Class Form1

#Region " PInvokes "

    <DllImport("user32.dll")> _
    Private Shared Function GetAsyncKeyState(ByVal vKey As System.Windows.Forms.Keys) As Short
    End Function

    <DllImport("user32.dll")> _
    Private Shared Sub mouse_event(dwFlags As UInteger, dx As UInteger, dy As UInteger, dwData As UInteger, dwExtraInfo As Integer)
    End Sub

    <DllImport("user32.dll", SetLastError:=True)> _
    Private Shared Function GetForegroundWindow() As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True)> _
    Private Shared Function GetWindowThreadProcessId(ByVal hwnd As IntPtr, ByRef lpdwProcessId As Integer) As Integer
    End Function

    <DllImport("user32.dll", SetLastError:=True)> _
    Private Shared Function SetCursorPos(ByVal X As Integer, ByVal Y As Integer) As Boolean
    End Function

    <DllImport("user32.dll", ExactSpelling:=True, SetLastError:=True)> _
    Public Shared Function GetCursorPos(ByRef lpPoint As POINT) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

#End Region

#Region " Enum/Structure "

    <Flags()> _
    Public Enum MouseEventFlags As UInteger
        MOUSEEVENTF_ABSOLUTE = &H8000
        MOUSEEVENTF_LEFTDOWN = &H2
        MOUSEEVENTF_LEFTUP = &H4
        MOUSEEVENTF_MIDDLEDOWN = &H20
        MOUSEEVENTF_MIDDLEUP = &H40
        MOUSEEVENTF_MOVE = &H1
        MOUSEEVENTF_RIGHTDOWN = &H8
        MOUSEEVENTF_RIGHTUP = &H10
        MOUSEEVENTF_XDOWN = &H80
        MOUSEEVENTF_XUP = &H100
        MOUSEEVENTF_WHEEL = &H800
        MOUSEEVENTF_HWHEEL = &H1000
    End Enum

    <System.Runtime.InteropServices.StructLayout(Runtime.InteropServices.LayoutKind.Sequential)> _
    Public Structure POINT
        Public X As Integer
        Public Y As Integer

        Public Sub New(ByVal X As Integer, ByVal Y As Integer)
            Me.X = X
            Me.Y = Y
        End Sub
    End Structure

#End Region

#Region " Properties "

    Public Property MenuOptions As Core.Options = New Core.Options

    Public Property ProcessTarget As Process = Nothing
    Public Property ProcessAccessToken As Native.Objects.ProcessHandle = Nothing
    Public Property RespondingThread As Thread = Nothing
    Public Property CheatT As Thread = Nothing

#End Region

#Region " Declare "

    Private Memory As Mem = New Mem()
    Private FirsShow As Boolean = False

#End Region

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Me.Hide()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        ProcessTarget = Process.GetProcessesByName("hl").FirstOrDefault
        If MenuOptions.Triggerbot = True Then TriggerbotOnOffSwitch.Toggled = LogInOnOffSwitch.Toggles.Toggled Else TriggerbotOnOffSwitch.Toggled = LogInOnOffSwitch.Toggles.NotToggled
        If MenuOptions.No_Recoil = True Then NoRecoilOnOffSwitch.Toggled = LogInOnOffSwitch.Toggles.Toggled Else NoRecoilOnOffSwitch.Toggled = LogInOnOffSwitch.Toggles.NotToggled
        NoRecoilTrackBar.Value = MenuOptions.No_Recoil_Val
        If MenuOptions.Glasswall = True Then GlasswallOnOffSwitch.Toggled = LogInOnOffSwitch.Toggles.Toggled Else GlasswallOnOffSwitch.Toggled = LogInOnOffSwitch.Toggles.NotToggled
        If MenuOptions.No_Smoke = True Then NoSmokeOnOffSwitch.Toggled = LogInOnOffSwitch.Toggles.Toggled Else NoSmokeOnOffSwitch.Toggled = LogInOnOffSwitch.Toggles.NotToggled
        Hook()
    End Sub

#Region " Public Methods "

    Public Sub Hook()
        Try
            If ProcessTarget Is Nothing Then Throw New Exception("Game Process not found, please open the game First!")
            ProcessAccessToken = New Native.Objects.ProcessHandle(ProcessTarget.Id, Native.Security.ProcessAccess.SuspendResume)
            ProcessAccessToken.Resume()
            RespondingThread = New Thread(AddressOf RespondingSecurity) With {.Name = "RespondingMoon", .IsBackground = True, .Priority = ThreadPriority.Highest}

            Dim OpenToken = Memory.OpenProcess(ProcessTarget.Id)

            If OpenToken = False Then
                Throw New Exception("Error In OpenProcess")
            End If

            MouseHook = New InputHelper.Hooks.MouseHook
            KeyboardHook = New InputHelper.Hooks.KeyboardHook
            CheatT = New Thread(AddressOf CheatsThread) With {.Name = "CheatHook", .IsBackground = True, .Priority = ThreadPriority.Highest}
            RespondingThread.Start()
            CheatT.Start()

        Catch ex As Exception
            MessageBox.Show(ex.Message, "CS 1.6 Mini Hook By Destroyer", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Environment.Exit(0)
        End Try
    End Sub

#End Region

#Region " Private Methods "

    Private Sub ShowMenu()
        Core.Embeder.SetParent(Me.Handle, ProcessTarget.MainWindowHandle)
        Core.Embeder.ShowInactiveTopmost(Me)
        Core.Embeder.SetForegroundWindow(Me.Handle)
        If FirsShow = False Then
            Core.Embeder.MoveWindowToCorner = False
            FirsShow = True
        End If
    End Sub

    Private Sub Reset()
        ProcessAccessToken.Resume()
        System.Threading.Thread.Sleep(500)
        Me.BeginInvoke(Sub()
                           If Me.Visible = True Then ProcessAccessToken.Suspend() : ShowMenu()
                       End Sub)
    End Sub

    Private Sub RespondingSecurity()
        Do While True
            Dim Responding As Boolean = ProcessTarget.Responding
            If Responding = False Then
                Reset()
            End If
        Loop
    End Sub

    Private Function GetModuleByName(ByVal ModuleName As String) As ProcessModule
        For Each ModuleHandle As ProcessModule In ProcessTarget.Modules
            If String.Equals(IO.Path.GetFileNameWithoutExtension(ModuleHandle.ModuleName), IO.Path.GetFileNameWithoutExtension(ModuleName), StringComparison.OrdinalIgnoreCase) Then
                Return ModuleHandle
            End If
        Next
        Return Nothing
    End Function

    Private Function IsGameFocus() As Boolean
        Dim FocusPID As Integer = 0
        GetWindowThreadProcessId(GetForegroundWindow, FocusPID)
        Dim Result As Boolean = False

        If ProcessTarget.Id = FocusPID Then
            Result = True
        ElseIf Process.GetCurrentProcess.Id = FocusPID Then
            Result = True
        End If

        Return Result
    End Function

#End Region

#Region " No Windows Focus "

    Protected Overrides ReadOnly Property ShowWithoutActivation As Boolean
        Get
            Return True
        End Get
    End Property

    Protected Overrides ReadOnly Property CreateParams As CreateParams
        Get
            Dim createParamsA As CreateParams = MyBase.CreateParams
            createParamsA.ExStyle = createParamsA.ExStyle Or Core.Win32.WindowStylesEx.WS_EX_OVERLAPPEDWINDOW Or Core.Win32.WindowStylesEx.WS_EX_TOPMOST Or Core.Win32.WindowStylesEx.WS_EX_NOACTIVATE Or Core.Win32.WindowStylesEx.WS_EX_TOOLWINDOW
            Return createParamsA
        End Get
    End Property

#End Region

#Region " Menu/FrontEnd "

    Private Sub TriggerbotOnOffSwitch_ToggleChanged(sender As Object) Handles TriggerbotOnOffSwitch.ToggleChanged
        MenuOptions.Triggerbot = (TriggerbotOnOffSwitch.Toggled = LogInOnOffSwitch.Toggles.Toggled)
        MenuOptions.Save()
    End Sub

    Private Sub NoRecoilOnOffSwitch_ToggleChanged(sender As Object) Handles NoRecoilOnOffSwitch.ToggleChanged
        MenuOptions.No_Recoil = (NoRecoilOnOffSwitch.Toggled = LogInOnOffSwitch.Toggles.Toggled)
        MenuOptions.Save()
    End Sub

    Private Sub GlasswallOnOffSwitch_ToggleChanged(sender As Object) Handles GlasswallOnOffSwitch.ToggleChanged
        MenuOptions.Glasswall = (GlasswallOnOffSwitch.Toggled = LogInOnOffSwitch.Toggles.Toggled)
        MenuOptions.Save()

        If MenuOptions.Glasswall = True Then
            Memory.WriteMemory(Core.Cheats.GLDepthFunc, "int", "519")
        Else
            Memory.WriteMemory(Core.Cheats.GLDepthFunc, "int", "515")
        End If
    End Sub

    Private Sub NoSmokeOnOffSwitch_ToggleChanged(sender As Object) Handles NoSmokeOnOffSwitch.ToggleChanged
        MenuOptions.No_Smoke = (NoSmokeOnOffSwitch.Toggled = LogInOnOffSwitch.Toggles.Toggled)
        MenuOptions.Save()

        If MenuOptions.No_Smoke = True Then
            Memory.WriteMemory(Core.Cheats.NoSmoke, "string", "Null.spr")
        Else
            Memory.WriteMemory(Core.Cheats.NoSmoke, "string", "gas_puff_01.spr")
        End If
    End Sub

    Private Sub NoRecoilTrackBar_ValueChanged() Handles NoRecoilTrackBar.ValueChanged
        MenuOptions.No_Recoil_Val = NoRecoilTrackBar.Value
        MenuOptions.Save()
    End Sub

#End Region

#Region " Cheats "

    Private WithEvents MouseHook As InputHelper.Hooks.MouseHook = Nothing
    Private WithEvents KeyboardHook As InputHelper.Hooks.KeyboardHook = Nothing

    Private Sub CheatsThread()

        Do While True

            If IsGameFocus() = True Then

                If Me.Visible = False Then
                    Dim EntityAim As Core.Cheats.EntityType = Memory.ReadInt(Core.Cheats.Entity_AIM)

                    Select Case EntityAim
                        Case Core.Cheats.EntityType.Null

                        Case Core.Cheats.EntityType.Friend

                        Case Core.Cheats.EntityType.Enemy
                            If MenuOptions.Triggerbot = True Then
                                mouse_event(MouseEventFlags.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0)
                                mouse_event(MouseEventFlags.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0)
                            End If
                    End Select
                End If

            Else

                Me.BeginInvoke(Sub()
                                   If Me.Visible = True Then Me.Hide()
                               End Sub)

            End If

            System.Threading.Thread.Sleep(100)
        Loop

    End Sub

    Private Sub MouseHook_MouseDown(sender As Object, e As InputHelper.EventArgs.MouseHookEventArgs) Handles MouseHook.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left Then
            If Me.Visible = False Then
                If MenuOptions.No_Recoil = True Then
                    Dim CurrentPos As POINT = Nothing
                    GetCursorPos(CurrentPos)
                    SetCursorPos(CurrentPos.X, CurrentPos.Y + MenuOptions.No_Recoil_Val)
                End If
            End If
        End If
    End Sub

    Private Sub KeyboardHook_KeyDown(sender As Object, e As InputHelper.EventArgs.KeyboardHookEventArgs) Handles KeyboardHook.KeyDown
        If e.KeyCode = Keys.Insert Then
            If Me.Visible = True Then
                Me.Hide()
                ProcessAccessToken.Resume()
            Else
                ProcessAccessToken.Suspend()
                ShowMenu()
            End If
        End If
    End Sub

#End Region

#Region " Dragger "

    Public Const WM_NCLBUTTONDOWN As Integer = &HA1
    Public Const HT_CAPTION As Integer = &H2

    <DllImportAttribute("user32.dll")>
    Public Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer
    End Function

    <DllImportAttribute("user32.dll")>
    Public Shared Function ReleaseCapture() As Boolean
    End Function

    Private Sub DragsHandler_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown, Label1.MouseDown, Label2.MouseDown, LogInLabel1.MouseDown, LogInLabel2.MouseDown, LogInLabel3.MouseDown, LogInLabel4.MouseDown, LogInLabel5.MouseDown
        If e.Button = MouseButtons.Left Then
            ReleaseCapture()
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0)
        End If
    End Sub

#End Region

End Class

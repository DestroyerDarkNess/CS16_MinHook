Imports System.Runtime.InteropServices

Namespace Core

    Public Class Embeder

#Region " PInvoke "

        <DllImport("user32.dll", SetLastError:=True)>
        Public Shared Function SetParent(ByVal hWndChild As IntPtr, ByVal hWndNewParent As IntPtr) As IntPtr
        End Function

        <DllImport("user32.dll")>
        Public Shared Function SetForegroundWindow(ByVal hwnd As IntPtr) As Integer
        End Function

        <DllImport("user32.dll", EntryPoint:="SetWindowPos")>
        Private Shared Function SetWindowPos(ByVal hWnd As Integer, ByVal hWndInsertAfter As Integer, ByVal X As Integer, ByVal Y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal uFlags As UInteger) As Boolean
        End Function

        <DllImport("user32.dll")>
        Private Shared Function ShowWindow(ByVal hWnd As System.IntPtr, ByVal nCmdShow As Integer) As Boolean
        End Function

        <DllImport("user32.dll")> _
        Private Shared Function MoveWindow(ByVal hWnd As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal bRepaint As Boolean) As Boolean
        End Function

#End Region

#Region " Const "

        Private Const SW_SHOWNOACTIVATE As Integer = 4
        Private Const HWND_TOPMOST As Integer = -1
        Private Const SWP_NOACTIVATE As UInteger = &H10
        Private Const SPI_SETDESKWALLPAPER As UInteger = 20
        Private Const SPIF_UPDATEINIFILE As UInteger = &H1
        Private Const WM_COMMAND As Integer = &H111

#End Region

        Public Shared Property MoveWindowToCorner As Boolean = True

        Public Shared Sub ShowInactiveTopmost(ByVal frm As System.Windows.Forms.Form)
            ShowWindow(frm.Handle, SW_SHOWNOACTIVATE)
            SetWindowPos(frm.Handle.ToInt32(), HWND_TOPMOST, frm.Left, frm.Top, frm.Width, frm.Height, SWP_NOACTIVATE)
            If MoveWindowToCorner = True Then MoveWindow(frm.Handle, 10, 10, frm.Width, frm.Height, True)
            frm.Refresh()
        End Sub

    End Class

End Namespace



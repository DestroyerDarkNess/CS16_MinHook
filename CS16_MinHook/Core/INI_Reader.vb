Namespace Core
    Public Class INI_Reader

        Declare Function GetPrivateProfileStringA Lib "kernel32" (ByVal lpAppName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As System.Text.StringBuilder, ByVal nSize As Integer, ByVal lpFileName As String) As Integer
        Declare Function WritePrivateProfileStringA Lib "kernel32" (ByVal lpAppName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Integer

        Public Shared Property IniFile As String = "Cs16_MinHook.ini"

        Public Shared Function ReadIni(ByVal Section As String, ByVal Key As String, Optional ByVal DefaultValue As String = Nothing) As String
            Dim buffer As New System.Text.StringBuilder(260)
            GetPrivateProfileStringA(Section, Key, DefaultValue, buffer, buffer.Capacity, IniFile)
            Return buffer.ToString
        End Function

        Public Shared Function WriteIni(ByVal Section As String, ByVal Key As String, ByVal Value As String) As Boolean
            Return (WritePrivateProfileStringA(Section, Key, Value, IniFile) <> 0)
        End Function

    End Class
End Namespace


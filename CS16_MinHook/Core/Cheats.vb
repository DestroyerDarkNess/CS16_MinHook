Imports System.Runtime.InteropServices

Namespace Core

    'For CounterStrike1.6v43i 

    'Protocol version 48
    'Exe version 1.1.2.6/2.0.0.0 (cstrike)
    'Exe build: 16:05:41 Jun 15 2009 (4554)
    Public Class Cheats

#Region " PInvoke "

        <DllImport("kernel32.dll", SetLastError:=True)> _
        Public Shared Function WriteProcessMemory(
    ByVal hProcess As IntPtr,
    ByVal lpBaseAddress As IntPtr,
    ByVal lpBuffer As Byte(),
    ByVal nSize As Int32,
    <Out()> ByRef lpNumberOfBytesWritten As IntPtr) As Boolean
        End Function

#End Region

        Public Shared Property GLDepthFunc As String = "hw.dll+51AA5"

        Public Shared Property Entity_AIM As String = "client.dll+0x1211F4"

        Public Enum EntityType
            Null = 0
            [Friend] = 1
            Enemy = 2
        End Enum

        'Remove Smoke Sprite
        Public Shared Property NoSmoke As String = "client.dll+E980C"



    End Class

End Namespace


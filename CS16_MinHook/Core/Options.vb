Namespace Core

    Public Class Options

        Public Property Triggerbot As Boolean = False

        Public Property No_Recoil As Boolean = False

        Public Property No_Recoil_Val As Integer = 3

        Public Property Glasswall As Boolean = False

        Public Property No_Smoke As Boolean = False

        Public Sub New()
            Load()
        End Sub

        Public Sub Load()
            Triggerbot = Core.INI_Reader.ReadIni("Config", "Triggerbot", Triggerbot)
            No_Recoil = Core.INI_Reader.ReadIni("Config", "No_Recoil", No_Recoil)
            No_Recoil_Val = Core.INI_Reader.ReadIni("Config", "No_Recoil_Val", No_Recoil_Val)
            Glasswall = Core.INI_Reader.ReadIni("Config", "Glasswall", Glasswall)
            No_Smoke = Core.INI_Reader.ReadIni("Config", "No_Smoke", No_Smoke)
        End Sub

        Public Sub Save()
            Core.INI_Reader.WriteIni("Config", "Triggerbot", Triggerbot)
            Core.INI_Reader.WriteIni("Config", "No_Recoil", No_Recoil)
            Core.INI_Reader.WriteIni("Config", "No_Recoil_Val", No_Recoil_Val)
            Core.INI_Reader.WriteIni("Config", "Glasswall", Glasswall)
            Core.INI_Reader.WriteIni("Config", "No_Smoke", No_Smoke)
        End Sub

    End Class

End Namespace

Imports JHSoftware.SimpleDNS.Plugin

Public Class Form1

  Dim oui As JHSoftware.SimpleDNS.Plugin.OptionsUI
  ' Dim pi As IPlugInBase

  Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
    Dim pi = New GeoDnsPlugIn
    oui = pi.GetOptionsUI(Guid.NewGuid, "")
    Me.Controls.Add(oui)

    oui.LoadData(Nothing)
  End Sub
End Class

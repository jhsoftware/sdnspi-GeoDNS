Imports JHSoftware.SimpleDNS.Plugin

Public Class Form1

  Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
    Dim pi = New GeoDnsPlugIn_CNAME
    OptionsUI1.UseClone = True
    OptionsUI1.LoadData(Nothing)
  End Sub

  Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
    Dim x = OptionsUI1.SaveData()
    Stop
  End Sub
End Class

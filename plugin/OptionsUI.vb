Public Class OptionsUI

  Friend Cfg As GeoDnsConfig
  Public UseClone As Boolean

  Public Overrides Sub LoadData(ByVal config As String)
    If UseClone Then
      lblDefault.Text = "Default clone from host name:"
    End If
    If config Is Nothing Then
      REM new instance
      Cfg = GeoDnsConfig.LoadDefault
    Else
      REM existing instance
      Cfg = GeoDnsConfig.Load(config)
      txtHostName.Text = Cfg.HostName.ToString
      txtFile.Text = Cfg.DataFile
      chkMonitor.Checked = Cfg.AutoReload
      CtlTTL1.Value = Cfg.RespTTL
      txtServer.Text = Cfg.DefaultServer
    End If
  End Sub

  Private Sub btnCountries_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCountries.Click
    Dim frm As New frmCountryList
    frm.OptUI = Me
    frm.ShowDialog()
  End Sub

  Private Sub btnRegions_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRegions.Click
    Dim frm As New frmRegionList
    frm.OptUI = Me
    frm.ShowDialog()
  End Sub

  Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click
    If RemoteGUI Then MessageBox.Show("This function is not available during remote management", _
                                  "Browse for file/folder", MessageBoxButtons.OK, _
                                  MessageBoxIcon.Warning) : Exit Sub

    OpenFileDialog1.FileName = txtFile.Text.Trim
    Try
      If OpenFileDialog1.ShowDialog <> Windows.Forms.DialogResult.OK Then Exit Sub
    Catch ex As System.InvalidOperationException
      REM reported by Kento for file name "C:\"
      OpenFileDialog1.FileName = ""
      If OpenFileDialog1.ShowDialog <> Windows.Forms.DialogResult.OK Then Exit Sub
    End Try
    txtFile.Text = OpenFileDialog1.FileName
  End Sub

  Private Sub btnTest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTest.Click
    Dim cap = "Test IP-to-Country data file"
    If RemoteGUI Then MessageBox.Show("This function is not available during remote management", _
                              cap, MessageBoxButtons.OK, _
                              MessageBoxIcon.Warning) : Exit Sub

    If txtFile.Text.Trim.Length = 0 Then
      MessageBox.Show("No IP-to-Country data file specified", cap, MessageBoxButtons.OK, MessageBoxIcon.Error)
      Exit Sub
    End If

    Dim ds = New ITCDataSet
    Me.Cursor = Cursors.WaitCursor
    Application.DoEvents()
    Try
      ds.LoadFile(txtFile.Text.Trim, False)
    Catch ex As Exception
      Me.Cursor = Cursors.Default
      MessageBox.Show("Error loading file:" & vbCrLf & vbCrLf & _
                      ex.Message, cap, MessageBoxButtons.OK, MessageBoxIcon.Error)
      Exit Sub
    End Try
    Me.Cursor = Cursors.Default

    If ds.Ip4Ranges.Count + ds.Ip4Ranges.Count = 0 Then
      MessageBox.Show("No IP-to-Country entries found in file", cap, MessageBoxButtons.OK, MessageBoxIcon.Error)
      Exit Sub
    End If

    Dim x = ""
    Dim ip As SdnsIP
    Dim cID As String ' ITCDataSet.Country

    Do
      x = InputBox("IP-to-Country data file contains " &
                  (ds.Ip4Ranges.Count + ds.Ip6Ranges.Count) & " IP address ranges in " &
                   ds.CountryIDs.Count & " countries." & vbCrLf & vbCrLf &
                   "Enter IP address to look up:", cap, x)
      If x.Length = 0 Then Exit Sub

      If Not SdnsIP.TryParse(x.Trim, ip) Then
        MessageBox.Show("Invalid IP address", cap, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Continue Do
      End If

      cID = ds.Lookup(ip)
      If cID Is Nothing Then
        MessageBox.Show(ip.ToString & " not found in data file", cap, MessageBoxButtons.OK, MessageBoxIcon.Information)
      Else
        Dim ctry As GeoDnsConfig.Country = Nothing
        If Not Cfg.Countries.TryGetValue(cID, ctry) Then ctry = Nothing
        MessageBox.Show(ip.ToString & " = " & cID & " - " & If(ctry Is Nothing, "?", ctry.Name), cap, MessageBoxButtons.OK, MessageBoxIcon.Information)
      End If
    Loop

  End Sub

  Public Overrides Function ValidateData() As Boolean
    Dim cap = "GeoDNS"
    If txtHostName.Text.Trim.Length = 0 Then
      MessageBox.Show("Host name is required", cap, MessageBoxButtons.OK, MessageBoxIcon.Error)
      Return False
    End If
    Dim dom As DomName
    If Not DomName.TryParse(txtHostName.Text.Trim, dom) Then
      MessageBox.Show("Invalid host name", cap, MessageBoxButtons.OK, MessageBoxIcon.Error)
      Return False
    End If

    If txtFile.Text.Trim.Length = 0 Then
      MessageBox.Show("IP-to-Country data file is required", cap, MessageBoxButtons.OK, MessageBoxIcon.Error)
      Return False
    End If
    If Not RemoteGUI Then
      If Not My.Computer.FileSystem.FileExists(txtFile.Text.Trim) Then
        MessageBox.Show("IP-to-Country data file does not exist", cap, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Return False
      End If
    End If

    If txtServer.Text.Trim.Length = 0 Then
      MessageBox.Show("Default server alias is required", cap, MessageBoxButtons.OK, MessageBoxIcon.Error)
      Return False
    End If
    If Not DomName.TryParse(txtServer.Text.Trim, dom) Then
      MessageBox.Show("Invalid default server alias", cap, MessageBoxButtons.OK, MessageBoxIcon.Error)
      Return False
    End If

    Return True
  End Function

  Public Overrides Function SaveData() As String
    Cfg.HostName = DomName.Parse(txtHostName.Text.Trim)
    Cfg.DataFile = txtFile.Text.Trim
    Cfg.AutoReload = chkMonitor.Checked
    Cfg.RespTTL = CtlTTL1.Value
    Cfg.DefaultServer = txtServer.Text.Trim
    Return Cfg.Save
  End Function

End Class

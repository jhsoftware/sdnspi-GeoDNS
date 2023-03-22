Public Class OptionsUI

  Friend Cfg As GeoDnsConfig
  Friend UseCNAME As Boolean
  Friend IpCtrl As ctlIP

  Public Overrides Sub LoadData(ByVal config As String)
    If Not UseCNAME Then
      lblDefault.Text = "Default server IP address:"
      IpCtrl = New ctlIP With {.IPVersion = IPVersionEnum.IPv4} ' GetIPCtrl(True, False)
      Me.Controls.Add(IpCtrl)
      IpCtrl.Location = txtServer.Location
      IpCtrl.TabIndex = txtServer.TabIndex
      txtServer.Visible = False
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
      If UseCNAME Then
        txtServer.Text = Cfg.DefaultServer
      Else
        IpCtrl.Text = Cfg.DefaultServer
      End If
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

  Private Sub lnkSoft77_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkSoft77.LinkClicked
    Try
      System.Diagnostics.Process.Start(lnkSoft77.Text.Substring(lnkSoft77.LinkArea.Start, lnkSoft77.LinkArea.Length))
    Catch ex As Exception
      MessageBox.Show("Failed to open link in your default Internet browser" & vbCrLf & _
                      vbCrLf & _
                      "Error: " & ex.Message, _
                      "Internet Link", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Try
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

    If ds.IpRanges.Count = 0 Then
      MessageBox.Show("No IP-to-Country entries found in file", cap, MessageBoxButtons.OK, MessageBoxIcon.Error)
      Exit Sub
    End If

    Dim x = ""
    Dim ip As SdnsIPv4
    Dim c As ITCDataSet.Country

    Do
      x = InputBox("IP-to-Country data file contains " & _
                   ds.OrigRangeCt & " IP address ranges (" & ds.IpRanges.Count & " merged) in " & _
                   ds.Countries.Count & " countries." & vbCrLf & vbCrLf & _
                   "Enter IP address to look up:", cap, x)
      If x.Length = 0 Then Exit Sub

      If Not SdnsIPv4.TryParse(x.Trim, ip) Then
        MessageBox.Show("Invalid IP address", cap, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Continue Do
      End If

      c = ds.Lookup(ip)
      If c Is Nothing Then
        MessageBox.Show(ip.ToString & " not found in data file", cap, MessageBoxButtons.OK, MessageBoxIcon.Information)
      Else
        MessageBox.Show(ip.ToString & " = " & c.ID & " - " & c.Name, cap, MessageBoxButtons.OK, MessageBoxIcon.Information)
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

    If UseCNAME Then
      If txtServer.Text.Trim.Length = 0 Then
        MessageBox.Show("Default server alias is required", cap, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Return False
      End If
      If Not DomName.TryParse(txtServer.Text.Trim, dom) Then
        MessageBox.Show("Invalid default server alias", cap, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Return False
      End If
    Else
      If IpCtrl.Text.Trim.Length = 0 Then
        MessageBox.Show("Default server IP address is required", cap, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Return False
      End If
      Dim ip As SdnsIPv4
      If Not SdnsIPv4.TryParse(IpCtrl.Text.Trim, ip) Then
        MessageBox.Show("Invalid default server IP address", cap, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Return False
      End If
    End If

    Return True
  End Function

  Public Overrides Function SaveData() As String
    Cfg.HostName = DomName.Parse(txtHostName.Text.Trim)
    Cfg.DataFile = txtFile.Text.Trim
    Cfg.AutoReload = chkMonitor.Checked
    Cfg.RespTTL = CtlTTL1.Value
    Cfg.DefaultServer = If(UseCNAME, txtServer.Text.Trim, IpCtrl.Text.Trim)
    Return Cfg.Save
  End Function

End Class

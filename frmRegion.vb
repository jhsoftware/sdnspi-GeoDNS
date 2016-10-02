Imports System.Windows.Forms

Public Class frmRegion

  Friend CurID As Integer = 0
  Friend Lst As AERListBoxMC
  Friend IpCtrl As Windows.Forms.Control

  Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
    If txtName.Text.Trim.Length = 0 Then
      MessageBox.Show("Name is required", "Region", MessageBoxButtons.OK, MessageBoxIcon.Error)
      Exit Sub
    End If

    If IpCtrl IsNot Nothing Then
      If IpCtrl.Text.Trim.ToLower = "<default>" Then IpCtrl.Text = ""
      If IpCtrl.Text.Trim.Length > 0 Then
        Dim ip As JHSoftware.SimpleDNS.Plugin.IPAddressV4
        If Not JHSoftware.SimpleDNS.Plugin.IPAddressV4.TryParse(IpCtrl.Text.Trim, ip) Then
          MessageBox.Show("Invalid server IP address", "Region", MessageBoxButtons.OK, MessageBoxIcon.Error)
          Exit Sub
        End If
        IpCtrl.Text = ip.ToString
      End If
    Else
      If txtServer.Text.Trim.ToLower = "<default>" Then txtServer.Text = ""
      If txtServer.Text.Trim.Length > 0 Then
        Dim dom As JHSoftware.SimpleDNS.Plugin.DomainName
        If Not JHSoftware.SimpleDNS.Plugin.DomainName.TryParse(txtServer.Text.Trim, dom) Then
          MessageBox.Show("Invalid server alias", "Region", MessageBoxButtons.OK, MessageBoxIcon.Error)
          Exit Sub
        End If
        txtServer.Text = dom.ToString
      End If
    End If

    Dim itm As New frmRegionList.LstItm
    itm.ID = CurID
    itm.Name = txtName.Text.Trim
    itm.Server = If(IpCtrl IsNot Nothing, IpCtrl.Text, txtServer.Text).Trim

    If Not Lst.CompleteEditItem(itm, AddressOf ItemsEqual) Then Exit Sub

    Me.DialogResult = System.Windows.Forms.DialogResult.OK
    Me.Close()
  End Sub

  Private Function ItemsEqual(ByVal item1 As Object, ByVal item2 As Object) As Boolean
    If String.Compare(DirectCast(item1, frmRegionList.LstItm).Name, DirectCast(item2, frmRegionList.LstItm).Name, True) = 0 Then
      MessageBox.Show("Region name is already in the list", "Region", MessageBoxButtons.OK, MessageBoxIcon.Error)
      Return True
    End If
    Return False
  End Function

  Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
    Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.Close()
  End Sub

End Class

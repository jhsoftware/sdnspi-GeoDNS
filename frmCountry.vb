Imports System.Windows.Forms

Public Class frmCountry

  Friend Lst As AERListBoxMC

  Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
    If Not txtID.ReadOnly Then
      Select Case txtID.Text.Trim.Length
        Case 0
          MessageBox.Show("Country ID is required", "Country", MessageBoxButtons.OK, MessageBoxIcon.Error)
          Exit Sub
        Case 2
          REM OK
        Case Else
          MessageBox.Show("Country ID must be 2 characters", "Country", MessageBoxButtons.OK, MessageBoxIcon.Error)
          Exit Sub
      End Select
    End If

    If txtName.Text.Trim.Length = 0 Then
      MessageBox.Show("Country name is required", "Country", MessageBoxButtons.OK, MessageBoxIcon.Error)
      Exit Sub
    End If

    Dim itm As New frmCountryList.LstItm
    itm.ID = txtID.Text.Trim
    itm.Name = txtName.Text.Trim
    itm.Region = DirectCast(ddRegion.SelectedItem, GeoDnsConfig.Region).ID
    itm.RegionName = ddRegion.SelectedItem.ToString
    If Not Lst.CompleteEditItem(itm, AddressOf ItemsEqual) Then Exit Sub

    Me.DialogResult = System.Windows.Forms.DialogResult.OK
    Me.Close()
  End Sub

  Private Function ItemsEqual(ByVal item1 As Object, ByVal item2 As Object) As Boolean
    If DirectCast(item1, frmCountryList.LstItm).ID = DirectCast(item2, frmCountryList.LstItm).ID Then
      MessageBox.Show("Country ID is already in the list", "Country", MessageBoxButtons.OK, MessageBoxIcon.Error)
      Return True
    End If
    Return False
  End Function


  Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
    Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.Close()
  End Sub

End Class

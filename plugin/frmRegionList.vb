Imports System.Windows.Forms

Public Class frmRegionList

  Friend OptUI As OptionsUI

  Private Sub frmRegionList_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    For Each r In OptUI.Cfg.Regions.Values
      List1.Add(New LstItm With {.ID = r.ID, .Name = r.Name, .Server = r.Server})
    Next
    List1.Columns(1).Text = "Server alias"
  End Sub

  Private Sub List1_EditItem(ByVal curItem As Object) Handles List1.EditItem
    Dim frm = New frmRegion
    frm.Lst = List1
    If curItem IsNot Nothing Then
      With DirectCast(curItem, LstItm)
        frm.txtName.Text = .Name
        frm.txtServer.Text = .Server
        frm.CurID = .ID
      End With
    End If
    frm.ShowDialog()
  End Sub

  Private Sub List1_BeforeRemove(ByVal item As AERListBoxMC.IItem, ByVal e As System.ComponentModel.CancelEventArgs) Handles List1.BeforeRemove
    Dim RegID = DirectCast(item, LstItm).ID
    If RegID = 0 Then Exit Sub
    Dim cct = 0
    For Each c In OptUI.Cfg.Countries.Values
      If c.Region = RegID Then cct += 1
    Next
    If cct = 0 Then Exit Sub
    If MessageBox.Show("WARNING: " & cct & If(cct > 1, " contries", " country") & " are associated with this region." & vbCrLf & vbCrLf & _
                       "Do you want to remove the region?", "Remove Region", _
                       MessageBoxButtons.YesNo, MessageBoxIcon.Warning, _
                       MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.No Then e.Cancel = True
  End Sub

  Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
    For Each r In OptUI.Cfg.Regions.Values
      If r.ID >= OptUI.Cfg.NextRegionID Then OptUI.Cfg.NextRegionID = r.ID + 1
    Next
    OptUI.Cfg.Regions.Clear()
    Dim itm As LstItm
    For i = 0 To List1.Count - 1
      itm = DirectCast(List1.Item(i), LstItm)
      If itm.ID = 0 Then
        itm.ID = OptUI.Cfg.NextRegionID
        OptUI.Cfg.NextRegionID += 1
      End If
      OptUI.Cfg.Regions.Add(itm.ID, New GeoDnsConfig.Region With {.ID = itm.ID, .Name = itm.Name, .Server = itm.Server})
    Next

    Me.DialogResult = System.Windows.Forms.DialogResult.OK
    Me.Close()
  End Sub

  Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
    Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.Close()
  End Sub

  Friend Class LstItm
    Implements AERListBoxMC.IItem
    Friend ID As Integer
    Friend Name As String
    Friend Server As String
    Public Function ColumnCompareTo(ByVal colIndex As Integer, ByVal otherItem As AERListBoxMC.IItem) As Integer Implements AERListBoxMC.IItem.ColumnCompareTo
      Select Case colIndex
        Case 0
          Return Name.CompareTo(DirectCast(otherItem, LstItm).Name)
        Case 1
          Return Server.CompareTo(DirectCast(otherItem, LstItm).Server)
      End Select
    End Function

    Public Function ColumnText(ByVal index As Integer) As String Implements AERListBoxMC.IItem.ColumnText
      Select Case index
        Case 0
          Return Name
        Case 1
          Return If(String.IsNullOrEmpty(Server), "<default>", Server)
        Case Else
          Return "???"
      End Select
    End Function
  End Class

End Class

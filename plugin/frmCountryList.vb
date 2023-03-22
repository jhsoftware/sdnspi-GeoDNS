Public Class frmCountryList

  Friend OptUI As OptionsUI

  Private Sub frmCountryList_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    For Each c In OptUI.Cfg.Countries.Values
      list1.Add(New LstItm With {.ID = c.ID, .Name = c.Name, .Region = c.Region, .RegionName = GetRegionName(c.Region)})
    Next
  End Sub

  Private Function GetRegionName(ByVal id As Integer) As String
    Dim r As GeoDnsConfig.Region
    If OptUI.Cfg.Regions.TryGetValue(id, r) Then Return r.Name
    Return "<none>"
  End Function

  Private Sub list1_EditItem(ByVal curItem As Object) Handles list1.EditItem
    Dim frm = New frmCountry
    frm.Lst = list1
    frm.ddRegion.Items.Add(New GeoDnsConfig.Region With {.ID = 0, .Name = "<none>"})
    Dim regs(OptUI.Cfg.Regions.Count - 1) As GeoDnsConfig.Region
    OptUI.Cfg.Regions.Values.CopyTo(regs, 0)
    Array.Sort(regs)
    frm.ddRegion.Items.AddRange(regs)
    frm.ddRegion.SelectedIndex = 0
    If curItem IsNot Nothing Then
      With DirectCast(curItem, LstItm)
        frm.txtID.Text = .ID
        frm.txtID.ReadOnly = True
        frm.txtID.TabStop = False
        frm.txtName.Text = .Name
        For i = 0 To frm.ddRegion.Items.Count - 1
          If DirectCast(frm.ddRegion.Items(i), GeoDnsConfig.Region).ID = .Region Then frm.ddRegion.SelectedIndex = i : Exit For
        Next
      End With
    End If
    frm.ShowDialog()
  End Sub

  Class LstItm
    Implements AERListBoxMC.IItem
    Friend ID As String
    Friend Name As String
    Friend Region As Integer
    Friend RegionName As String

    Public Function ColumnCompareTo(ByVal colIndex As Integer, ByVal otherItem As AERListBoxMC.IItem) As Integer Implements AERListBoxMC.IItem.ColumnCompareTo
      Select Case colIndex
        Case 0
          Return ID.CompareTo(DirectCast(otherItem, LstItm).ID)
        Case 1
          Return Name.CompareTo(DirectCast(otherItem, LstItm).Name)
        Case 2
          Return RegionName.CompareTo(DirectCast(otherItem, LstItm).RegionName)
      End Select
    End Function
    Public Function ColumnText(ByVal index As Integer) As String Implements AERListBoxMC.IItem.ColumnText
      Select Case index
        Case 0
          Return ID
        Case 1
          Return Name
        Case 2
          Return RegionName
        Case Else
          Return "???"
      End Select
    End Function
  End Class

  Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
    OptUI.Cfg.Countries.Clear()
    Dim itm As LstItm
    For i = 0 To list1.Count - 1
      itm = DirectCast(list1.Item(i), LstItm)
      OptUI.Cfg.Countries.Add(itm.ID, New GeoDnsConfig.Country With {.ID = itm.ID, .Name = itm.Name, .Region = itm.Region})
    Next
    DialogResult = Windows.Forms.DialogResult.OK
    Me.Close()
  End Sub

  Private Sub btnLoad_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLoad.Click
    Dim cap = "Load countries from IP-to-Country data file"
    If OptUI.RemoteGUI Then MessageBox.Show("This function is not available during remote management", _
                              cap, MessageBoxButtons.OK, _
                              MessageBoxIcon.Warning) : Exit Sub

    OpenFileDialog1.FileName = OptUI.txtFile.Text.Trim
    Try
      If OpenFileDialog1.ShowDialog <> Windows.Forms.DialogResult.OK Then Exit Sub
    Catch ex As System.InvalidOperationException
      REM reported by Kento for file name "C:\"
      OpenFileDialog1.FileName = ""
      If OpenFileDialog1.ShowDialog <> Windows.Forms.DialogResult.OK Then Exit Sub
    End Try
    Dim fileName = OpenFileDialog1.FileName

    Dim ds As New ITCDataSet
    Try
      ds.LoadFile(fileName, False)
    Catch ex As Exception
      MessageBox.Show("Error reading data file:" & vbCrLf & vbCrLf & _
                      ex.Message, cap, MessageBoxButtons.OK, MessageBoxIcon.Error)
      Exit Sub
    End Try

    If ds.Countries.Count = 0 Then
      MessageBox.Show("Data file does not contain any countries", cap, MessageBoxButtons.OK, MessageBoxIcon.Error)
      Exit Sub
    End If

    Dim tmp As New Dictionary(Of String, Object)
    For i = 0 To list1.Count - 1
      tmp.Add(DirectCast(list1.Item(i), LstItm).ID, Nothing)
    Next
    Dim ncct = 0
    For Each c In ds.Countries.Values
      If Not tmp.ContainsKey(c.ID) Then ncct += 1
    Next

    If ncct = 0 Then
      MessageBox.Show("The specified IP-to-Country data file does not contain " & vbCrLf & _
                      "any countries that are not already in the list.", cap, MessageBoxButtons.OK, MessageBoxIcon.Information)
      Exit Sub
    End If

    Dim msg As String
    If ncct > 10 Then
      msg = "File contains " & ncct & " countries which are not already listed." & vbCrLf
    Else
      msg = "File contains the following " & If(ncct > 1, ncct & " countries which are", "country which is") & " not already listed:" & vbCrLf & vbCrLf
      For Each c In ds.Countries.Values
        If Not tmp.ContainsKey(c.ID) Then msg &= c.ID & " - " & c.Name & vbCrLf
      Next
    End If
    If MessageBox.Show(msg & vbCrLf & _
             "Do you want to add " & If(ncct > 1, "these countries?", "this country?"), cap, _
             MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
             MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.No Then Exit Sub

    For Each c In ds.Countries.Values
      If Not tmp.ContainsKey(c.ID) Then
        list1.Add(New LstItm With {.ID = c.ID, .Name = c.Name, .Region = 0, .RegionName = "<none>"})
      End If
    Next
  End Sub

End Class
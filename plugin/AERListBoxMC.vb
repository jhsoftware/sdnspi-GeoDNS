Imports System.ComponentModel

Public Class AERListBoxMC

  Private Adding As Boolean

  Public Event EditItem(ByVal curItem As Object)
  Public Event BeforeAdd(ByVal e As System.ComponentModel.CancelEventArgs)
  Public Event BeforeRemove(ByVal item As IItem, ByVal e As System.ComponentModel.CancelEventArgs)
  Public Event AfterRemove(ByVal item As IItem)

  Public Delegate Function ItemsEqualDG(ByVal item1 As Object, ByVal item2 As Object) As Boolean

  <DesignerSerializationVisibility(DesignerSerializationVisibility.Content)> _
  Public Property Columns() As Windows.Forms.ListView.ColumnHeaderCollection
    Get
      Return ListView1.Columns
    End Get
    Set(ByVal value As Windows.Forms.ListView.ColumnHeaderCollection)
      For Each c As Windows.Forms.ColumnHeader In value
        ListView1.Columns.Add(c)
      Next
    End Set
  End Property

  <DefaultValue(False)> _
  Public Property Sortable() As Boolean
    Get
      Return (ListView1.HeaderStyle = ColumnHeaderStyle.Clickable)
    End Get
    Set(ByVal value As Boolean)
      If value Then
        ListView1.HeaderStyle = ColumnHeaderStyle.Clickable
        ListView1.ListViewItemSorter = New MySorter
      Else
        ListView1.HeaderStyle = ColumnHeaderStyle.Nonclickable
        ListView1.ListViewItemSorter = Nothing
      End If
    End Set
  End Property

  Public Function CompleteEditItem(ByVal newItem As IItem, ByVal itemsEqual As ItemsEqualDG) As Boolean
    For i = 0 To ListView1.Items.Count - 1
      If Not Adding AndAlso ListView1.Items(i).Selected Then Continue For
      If itemsEqual(newItem, ListView1.Items(i).Tag) Then Return False
    Next
    Dim newLVI = MakeLVI(newItem)
    If Adding Then
      ListView1.SelectedIndices.Clear()
      ListView1.Items.Add(newLVI)
    Else
      ListView1.Items(ListView1.SelectedIndices(0)) = newLVI
    End If
    newLVI.Selected = True
    newLVI.EnsureVisible()
    Return True
  End Function

  Private Function MakeLVI(ByVal itm As IItem) As ListViewItem
    Dim li = New ListViewItem(itm.ColumnText(0))
    For i = 1 To ListView1.Columns.Count - 1
      li.SubItems.Add(itm.ColumnText(i))
    Next
    li.Tag = itm
    Return li
  End Function

  Sub Add(ByVal itm As IItem)
    ListView1.Items.Add(MakeLVI(itm))
  End Sub

  Function Count() As Integer
    Return ListView1.Items.Count
  End Function

  Function Item(ByVal index As Integer) As IItem
    Return DirectCast(ListView1.Items(index).Tag, IItem)
  End Function

  Private Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
    Dim ee As New System.ComponentModel.CancelEventArgs
    RaiseEvent BeforeAdd(ee)
    If ee.Cancel Then Exit Sub

    Adding = True
    RaiseEvent EditItem(Nothing)
  End Sub

  Private Sub ListView1_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles ListView1.ColumnClick
    DirectCast(ListView1.ListViewItemSorter, MySorter).ColClick(e.Column)
    ListView1.Sort()
  End Sub

  Private Sub btnEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEdit.Click, ListView1.DoubleClick
    If ListView1.SelectedIndices.Count <= 0 Then Exit Sub
    Adding = False
    RaiseEvent EditItem(ListView1.SelectedItems(0).Tag)
  End Sub

  Private Sub btnRemove_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRemove.Click
    If ListView1.SelectedIndices.Count <= 0 Then Exit Sub
    Dim rmItem = DirectCast(ListView1.Items(ListView1.SelectedIndices(0)).Tag, IItem)
    Dim ee As New System.ComponentModel.CancelEventArgs
    RaiseEvent BeforeRemove(rmItem, ee)
    If ee.Cancel Then Exit Sub
    ListView1.Items.RemoveAt(ListView1.SelectedIndices(0))
    RaiseEvent AfterRemove(rmItem)
  End Sub

  Private Sub ListBox1_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles ListView1.KeyUp
    If e.KeyData = Keys.Delete Then btnRemove_Click(Nothing, Nothing)
  End Sub

  Private Sub ListBox1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.SelectedIndexChanged
    btnEdit.Enabled = (ListView1.SelectedIndices.Count > 0)
    btnRemove.Enabled = (ListView1.SelectedIndices.Count > 0)
  End Sub

  Friend Sub ClearItems()
    ListView1.Items.Clear()
    btnEdit.Enabled = False
    btnRemove.Enabled = False
  End Sub

  Public Interface IItem
    Function ColumnText(ByVal index As Integer) As String
    Function ColumnCompareTo(ByVal colIndex As Integer, ByVal otherItem As IItem) As Integer
  End Interface

  Private Class MySorter
    Implements Collections.IComparer

    Dim Order As Windows.Forms.SortOrder = SortOrder.Ascending
    Dim Columns As New List(Of Integer)

    Sub New()
      Columns.Add(0)
    End Sub

    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
      Dim itmX = DirectCast(DirectCast(x, ListViewItem).Tag, IItem)
      Dim itmY = DirectCast(DirectCast(y, ListViewItem).Tag, IItem)
      Dim cp As Integer
      For i = 0 To Columns.Count - 1
        cp = itmX.ColumnCompareTo(Columns(i), itmY)
        If cp <> 0 Then Return If(Order = SortOrder.Ascending, cp, -cp)
      Next
      Return 0
    End Function

    Public Sub ColClick(ByVal colIndex As Integer)
      If Columns.Count > 0 Then
        If Columns(0) = colIndex Then
          Order = If(Order = SortOrder.Ascending, SortOrder.Descending, SortOrder.Ascending)
          Exit Sub
        End If
        For i = 1 To Columns.Count - 1
          If Columns(i) = colIndex Then Columns.RemoveAt(i) : Exit For
        Next
      End If
      Columns.Insert(0, colIndex)
      Order = SortOrder.Ascending
    End Sub
  End Class

End Class

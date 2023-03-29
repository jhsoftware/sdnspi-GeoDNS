<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AERListBoxMC
  Inherits System.Windows.Forms.UserControl

  'UserControl overrides dispose to clean up the component list.
  <System.Diagnostics.DebuggerNonUserCode()> _
  Protected Overrides Sub Dispose(ByVal disposing As Boolean)
    Try
      If disposing AndAlso components IsNot Nothing Then
        components.Dispose()
      End If
    Finally
      MyBase.Dispose(disposing)
    End Try
  End Sub

  'Required by the Windows Form Designer
  Private components As System.ComponentModel.IContainer

  'NOTE: The following procedure is required by the Windows Form Designer
  'It can be modified using the Windows Form Designer.  
  'Do not modify it using the code editor.
  <System.Diagnostics.DebuggerStepThrough()> _
  Private Sub InitializeComponent()
    Me.ListView1 = New System.Windows.Forms.ListView
    Me.btnAdd = New System.Windows.Forms.Button
    Me.btnEdit = New System.Windows.Forms.Button
    Me.btnRemove = New System.Windows.Forms.Button
    Me.SuspendLayout()
    '
    'ListView1
    '
    Me.ListView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.ListView1.FullRowSelect = True
    Me.ListView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
    Me.ListView1.HideSelection = False
    Me.ListView1.Location = New System.Drawing.Point(0, 0)
    Me.ListView1.MultiSelect = False
    Me.ListView1.Name = "ListView1"
    Me.ListView1.ShowGroups = False
    Me.ListView1.Size = New System.Drawing.Size(308, 241)
    Me.ListView1.TabIndex = 0
    Me.ListView1.UseCompatibleStateImageBehavior = False
    Me.ListView1.View = System.Windows.Forms.View.Details
    '
    'btnAdd
    '
    Me.btnAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.btnAdd.Location = New System.Drawing.Point(314, 0)
    Me.btnAdd.Name = "btnAdd"
    Me.btnAdd.Size = New System.Drawing.Size(65, 21)
    Me.btnAdd.TabIndex = 1
    Me.btnAdd.Text = "Add..."
    Me.btnAdd.UseVisualStyleBackColor = True
    '
    'btnEdit
    '
    Me.btnEdit.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.btnEdit.Enabled = False
    Me.btnEdit.Location = New System.Drawing.Point(314, 27)
    Me.btnEdit.Name = "btnEdit"
    Me.btnEdit.Size = New System.Drawing.Size(65, 21)
    Me.btnEdit.TabIndex = 2
    Me.btnEdit.Text = "Edit..."
    Me.btnEdit.UseVisualStyleBackColor = True
    '
    'btnRemove
    '
    Me.btnRemove.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.btnRemove.Enabled = False
    Me.btnRemove.Location = New System.Drawing.Point(314, 54)
    Me.btnRemove.Name = "btnRemove"
    Me.btnRemove.Size = New System.Drawing.Size(65, 21)
    Me.btnRemove.TabIndex = 3
    Me.btnRemove.Text = "Remove"
    Me.btnRemove.UseVisualStyleBackColor = True
    '
    'AERListBoxMC
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.Controls.Add(Me.btnRemove)
    Me.Controls.Add(Me.btnEdit)
    Me.Controls.Add(Me.btnAdd)
    Me.Controls.Add(Me.ListView1)
    Me.Name = "AERListBoxMC"
    Me.Size = New System.Drawing.Size(379, 241)
    Me.ResumeLayout(False)

  End Sub
  Protected WithEvents ListView1 As System.Windows.Forms.ListView
  Protected WithEvents btnAdd As System.Windows.Forms.Button
  Protected WithEvents btnEdit As System.Windows.Forms.Button
  Protected WithEvents btnRemove As System.Windows.Forms.Button

End Class

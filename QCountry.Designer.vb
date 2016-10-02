<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class QCountry
    Inherits JHSoftware.SimpleDNS.Plugin.OptionsUI

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
    Me.Label1 = New System.Windows.Forms.Label
    Me.txtCountry = New System.Windows.Forms.TextBox
    Me.SuspendLayout()
    '
    'Label1
    '
    Me.Label1.AutoSize = True
    Me.Label1.Location = New System.Drawing.Point(-3, 3)
    Me.Label1.Name = "Label1"
    Me.Label1.Size = New System.Drawing.Size(266, 13)
    Me.Label1.TabIndex = 1
    Me.Label1.Text = "DNS request originates from county (2 letter ISO code):"
    '
    'txtCountry
    '
    Me.txtCountry.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
    Me.txtCountry.Location = New System.Drawing.Point(269, 0)
    Me.txtCountry.MaxLength = 2
    Me.txtCountry.Name = "txtCountry"
    Me.txtCountry.Size = New System.Drawing.Size(34, 20)
    Me.txtCountry.TabIndex = 2
    '
    'QCountry
    '
    Me.Controls.Add(Me.txtCountry)
    Me.Controls.Add(Me.Label1)
    Me.Name = "QCountry"
    Me.Size = New System.Drawing.Size(306, 23)
    Me.ResumeLayout(False)
    Me.PerformLayout()

  End Sub
  Friend WithEvents Label1 As System.Windows.Forms.Label
  Friend WithEvents txtCountry As System.Windows.Forms.TextBox

End Class

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OptionsUI
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
        Me.components = New System.ComponentModel.Container()
        Me.btnCountries = New System.Windows.Forms.Button()
        Me.btnRegions = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtHostName = New System.Windows.Forms.TextBox()
        Me.lblDefault = New System.Windows.Forms.Label()
        Me.txtServer = New System.Windows.Forms.TextBox()
        Me.chkMonitor = New System.Windows.Forms.CheckBox()
        Me.btnBrowse = New System.Windows.Forms.Button()
        Me.txtFile = New System.Windows.Forms.TextBox()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.btnTest = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.CtlTTL1 = New JHSoftware.SimpleDNS.ctlTTL()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'btnCountries
        '
        Me.btnCountries.Location = New System.Drawing.Point(182, 220)
        Me.btnCountries.Margin = New System.Windows.Forms.Padding(3, 3, 3, 13)
        Me.btnCountries.Name = "btnCountries"
        Me.btnCountries.Size = New System.Drawing.Size(176, 23)
        Me.btnCountries.TabIndex = 13
        Me.btnCountries.Text = "Edit Countries / Regions..."
        Me.btnCountries.UseVisualStyleBackColor = True
        '
        'btnRegions
        '
        Me.btnRegions.Location = New System.Drawing.Point(0, 220)
        Me.btnRegions.Margin = New System.Windows.Forms.Padding(3, 3, 3, 13)
        Me.btnRegions.Name = "btnRegions"
        Me.btnRegions.Size = New System.Drawing.Size(176, 23)
        Me.btnRegions.TabIndex = 12
        Me.btnRegions.Text = "Edit Regions / Servers..."
        Me.btnRegions.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(-3, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(61, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Host name:"
        '
        'txtHostName
        '
        Me.txtHostName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtHostName.Location = New System.Drawing.Point(0, 16)
        Me.txtHostName.Margin = New System.Windows.Forms.Padding(3, 3, 3, 13)
        Me.txtHostName.Name = "txtHostName"
        Me.txtHostName.Size = New System.Drawing.Size(361, 20)
        Me.txtHostName.TabIndex = 1
        '
        'lblDefault
        '
        Me.lblDefault.AutoSize = True
        Me.lblDefault.Location = New System.Drawing.Point(-3, 118)
        Me.lblDefault.Name = "lblDefault"
        Me.lblDefault.Size = New System.Drawing.Size(100, 13)
        Me.lblDefault.TabIndex = 7
        Me.lblDefault.Text = "Default server alias:"
        '
        'txtServer
        '
        Me.txtServer.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtServer.Location = New System.Drawing.Point(0, 134)
        Me.txtServer.Margin = New System.Windows.Forms.Padding(3, 3, 3, 13)
        Me.txtServer.Name = "txtServer"
        Me.txtServer.Size = New System.Drawing.Size(361, 20)
        Me.txtServer.TabIndex = 8
        '
        'chkMonitor
        '
        Me.chkMonitor.AutoSize = True
        Me.chkMonitor.Checked = True
        Me.chkMonitor.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkMonitor.Location = New System.Drawing.Point(0, 88)
        Me.chkMonitor.Margin = New System.Windows.Forms.Padding(3, 0, 3, 13)
        Me.chkMonitor.Name = "chkMonitor"
        Me.chkMonitor.Size = New System.Drawing.Size(252, 17)
        Me.chkMonitor.TabIndex = 6
        Me.chkMonitor.Text = "Automatically re-load data file when it is updated"
        Me.chkMonitor.UseVisualStyleBackColor = True
        '
        'btnBrowse
        '
        Me.btnBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnBrowse.Location = New System.Drawing.Point(278, 63)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(27, 23)
        Me.btnBrowse.TabIndex = 4
        Me.btnBrowse.Text = "..."
        Me.ToolTip1.SetToolTip(Me.btnBrowse, "Browse")
        Me.btnBrowse.UseVisualStyleBackColor = True
        '
        'txtFile
        '
        Me.txtFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtFile.Location = New System.Drawing.Point(0, 65)
        Me.txtFile.Name = "txtFile"
        Me.txtFile.Size = New System.Drawing.Size(272, 20)
        Me.txtFile.TabIndex = 3
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.AddExtension = False
        Me.OpenFileDialog1.Filter = "GeoDNS plug-in data file (*.geodns)|*.geodns"
        Me.OpenFileDialog1.Title = "Select IP-to-Country data file"
        '
        'btnTest
        '
        Me.btnTest.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnTest.Location = New System.Drawing.Point(311, 63)
        Me.btnTest.Name = "btnTest"
        Me.btnTest.Size = New System.Drawing.Size(50, 23)
        Me.btnTest.TabIndex = 5
        Me.btnTest.Text = "Test"
        Me.btnTest.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(-3, 167)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(160, 13)
        Me.Label2.TabIndex = 10
        Me.Label2.Text = "DNS record TTL (Time To Live):"
        '
        'CtlTTL1
        '
        Me.CtlTTL1.AutoSize = True
        Me.CtlTTL1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.CtlTTL1.BackColor = System.Drawing.Color.Transparent
        Me.CtlTTL1.Location = New System.Drawing.Point(0, 183)
        Me.CtlTTL1.Margin = New System.Windows.Forms.Padding(3, 3, 3, 13)
        Me.CtlTTL1.Name = "CtlTTL1"
        Me.CtlTTL1.ReadOnly = False
        Me.CtlTTL1.Size = New System.Drawing.Size(156, 21)
        Me.CtlTTL1.TabIndex = 11
        Me.CtlTTL1.Value = 1800
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(0, 49)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(49, 13)
        Me.Label3.TabIndex = 14
        Me.Label3.Text = "Data file:"
        '
        'OptionsUI
        '
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.CtlTTL1)
        Me.Controls.Add(Me.btnTest)
        Me.Controls.Add(Me.txtFile)
        Me.Controls.Add(Me.btnBrowse)
        Me.Controls.Add(Me.chkMonitor)
        Me.Controls.Add(Me.txtServer)
        Me.Controls.Add(Me.lblDefault)
        Me.Controls.Add(Me.btnCountries)
        Me.Controls.Add(Me.btnRegions)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtHostName)
        Me.Name = "OptionsUI"
        Me.Size = New System.Drawing.Size(361, 247)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnCountries As System.Windows.Forms.Button
  Friend WithEvents btnRegions As System.Windows.Forms.Button
  Friend WithEvents Label1 As System.Windows.Forms.Label
  Friend WithEvents txtHostName As System.Windows.Forms.TextBox
  Friend WithEvents lblDefault As System.Windows.Forms.Label
  Friend WithEvents txtServer As System.Windows.Forms.TextBox
  Friend WithEvents chkMonitor As System.Windows.Forms.CheckBox
  Friend WithEvents btnBrowse As System.Windows.Forms.Button
  Friend WithEvents txtFile As System.Windows.Forms.TextBox
  Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
  Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents btnTest As System.Windows.Forms.Button
    Friend WithEvents CtlTTL1 As ctlTTL
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As Label
End Class

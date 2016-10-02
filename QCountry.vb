Public Class QCountry

  Public Overrides Sub LoadData(ByVal config As String)
    txtCountry.Text = config
  End Sub

  Public Overrides Function SaveData() As String
    Return txtCountry.Text.Trim
  End Function

  Public Overrides Function ValidateData() As Boolean
    If txtCountry.Text.Trim.Length = 2 Then Return True
    MessageBox.Show("Invalid country code", "Country", MessageBoxButtons.OK, MessageBoxIcon.Error)
    Return False
  End Function

End Class

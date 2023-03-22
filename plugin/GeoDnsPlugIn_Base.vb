Imports JHSoftware.SimpleDNS.Plugin

Public MustInherit Class GeoDnsPlugIn_Base
  Implements JHSoftware.SimpleDNS.Plugin.IQuestions

  Friend MyDataSet As ITCDataSet
  Friend MyConfig As GeoDnsConfig

  Protected LastReload As DateTime
  Protected WithEvents fMon As System.IO.FileSystemWatcher

  MustOverride Sub BaseLog(ByVal s As String)
  MustOverride Sub BaseAsyncError(ByVal ex As System.Exception)

  Protected Function LookUpServer(ByVal FromIP As SdnsIP) As String
    If MyDataSet Is Nothing Then
      BaseLog("Cannot proccess DNS request - data file not loaded")
      Return Nothing
    End If

    If FromIP.IPVersion <> 4 Then
      Return MyConfig.DefaultServer
    End If

    Dim c = MyDataSet.Lookup(DirectCast(FromIP, SdnsIPv4))
    If c Is Nothing Then
      BaseLog(FromIP.ToString & " not found in IP-to-Country database")
      Return MyConfig.DefaultServer
    End If

    Dim cc As GeoDnsConfig.Country
    If Not MyConfig.Countries.TryGetValue(c.ID, cc) OrElse cc.Region <= 0 Then
      BaseLog("No region specified for country: " & FromIP.ToString & " = " & c.ID & " - " & c.Name)
      Return MyConfig.DefaultServer
    End If

    Dim rg As GeoDnsConfig.Region
    If Not MyConfig.Regions.TryGetValue(cc.Region, rg) Then
      BaseLog("No region specified for country: " & FromIP.ToString & " = " & c.ID & " - " & cc.Name)
      Return MyConfig.DefaultServer
    End If

    If String.IsNullOrEmpty(rg.Server) Then
      BaseLog("No server specified for region: " & FromIP.ToString & " = " & c.ID & " - " & cc.Name & " = " & rg.Name)
      Return MyConfig.DefaultServer
    End If

    BaseLog("Matched " & FromIP.ToString & " = " & c.ID & " - " & cc.Name & " = " & rg.Name & " -> " & rg.Server)
    Return rg.Server
  End Function

  Protected Sub BaseStartService()
    MyDataSet = New ITCDataSet
    MyDataSet.LoadFile(MyConfig.DataFile, True)
    If MyConfig.AutoReload Then
      fMon = New System.IO.FileSystemWatcher
      fMon.Path = System.IO.Path.GetDirectoryName(MyConfig.DataFile)
      fMon.Filter = System.IO.Path.GetFileName(MyConfig.DataFile)
      fMon.IncludeSubdirectories = False
      fMon.NotifyFilter = IO.NotifyFilters.LastWrite
      fMon.EnableRaisingEvents = True
    End If
  End Sub

  Protected Sub BaseStopService()
    MyDataSet = Nothing
    If fMon IsNot Nothing Then
      fMon.EnableRaisingEvents = False
      fMon.Dispose()
      fMon = Nothing
    End If
  End Sub


  Private Sub fMon_Changed(ByVal sender As Object, ByVal e As System.IO.FileSystemEventArgs) Handles fMon.Changed
    Try
      If DateTime.UtcNow.Subtract(LastReload).TotalSeconds < 5 Then Exit Sub
      BaseLog("IP-to-Country data file update detected - reloading")
      Try
        MyDataSet = New ITCDataSet
        MyDataSet.LoadFile(MyConfig.DataFile, True)
      Catch ex As Exception
        MyDataSet = Nothing
        BaseLog("Error reloading IP-to-Country data file: " & ex.Message)
      End Try

    Catch ex As Exception
      BaseAsyncError(ex)
    End Try
  End Sub

#Region "IQuestions"

  Public Function QuestionList() As JHSoftware.SimpleDNS.Plugin.IQuestions.QuestionInfo() Implements JHSoftware.SimpleDNS.Plugin.IQuestions.QuestionList
    Dim rv(0) As IQuestions.QuestionInfo
    rv(0).Question = "Request originates from country"
    rv(0).ValuePrompt = "DNS request originates from county (2 letter ISO code)"
    Return rv
  End Function

  Public Async Function QuestionAsk(ByVal id As Integer, value As String, ByVal req As IRequestContext) As Threading.Tasks.Task(Of Boolean) Implements JHSoftware.SimpleDNS.Plugin.IQuestions.QuestionAsk
    If req.FromIP Is Nothing Then Return False
    If Not TypeOf req.FromIP Is SdnsIPv4 Then Return False
    Dim c = MyDataSet.Lookup(DirectCast(req.FromIP, SdnsIPv4))
    If c Is Nothing Then Return False
    Return (c.ID = value.ToUpper)
  End Function

#End Region


End Class

Imports JHSoftware.SimpleDNS.Plugin

Public MustInherit Class GeoDnsPlugIn_Base
  Implements JHSoftware.SimpleDNS.Plugin.IQuestions

  Friend MyDataSet As ITCDataSet
  Friend MyConfig As GeoDnsConfig

  Protected LastReload As DateTime
  Protected WithEvents fMon As System.IO.FileSystemWatcher

  MustOverride Sub BaseLog(ByVal s As String)
  MustOverride Sub BaseAsyncError(ByVal ex As System.Exception)

  Protected Function LookUpServer(ByVal FromIP As IPAddress) As String
    If MyDataSet Is Nothing Then
      BaseLog("Cannot proccess DNS request - data file not loaded")
      Return Nothing
    End If

    If FromIP.IPVersion <> 4 Then
      Return MyConfig.DefaultServer
    End If

    Dim c = MyDataSet.Lookup(DirectCast(FromIP, IPAddressV4))
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

  REM removed in v. 5.2.1
  'Protected Function BaseInstanceConflict(ByVal config1 As String, ByVal config2 As String, ByRef errorMsg As String) As Boolean
  '  Dim cfg1 = GeoDnsConfig.Load(config1)
  '  Dim cfg2 = GeoDnsConfig.Load(config2)
  '  If String.Compare(cfg1.DataFile, cfg2.DataFile, True) = 0 Then
  '    errorMsg = "Another plug-in is using the same data file"
  '    Return True
  '  End If
  '  Return False
  'End Function

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
    rv(0).Description = "Request originates from country"
    rv(0).HasUI = True
    rv(0).ID = 1
    Return rv
  End Function

  Public Function QuestionGetUI(ByVal id As Integer) As JHSoftware.SimpleDNS.Plugin.OptionsUI Implements JHSoftware.SimpleDNS.Plugin.IQuestions.QuestionGetUI
    Return New QCountry
  End Function

  Public Function QuestionLoadConfig(ByVal id As Integer, ByVal configStr As String) As Object Implements JHSoftware.SimpleDNS.Plugin.IQuestions.QuestionLoadConfig
    Return configStr
  End Function

  Public Function QuestionAsk(ByVal id As Integer, ByVal configObj As Object, ByVal req As JHSoftware.SimpleDNS.Plugin.IDNSRequest) As Boolean Implements JHSoftware.SimpleDNS.Plugin.IQuestions.QuestionAsk
    If req.FromIP Is Nothing Then Return False
    If Not TypeOf req.FromIP Is IPAddressV4 Then Return False
    Dim c = MyDataSet.Lookup(DirectCast(req.FromIP, IPAddressV4))
    If c Is Nothing Then Return False
    Return (c.ID = DirectCast(configObj, String))
  End Function

#End Region


End Class

Imports JHSoftware.SimpleDNS.Plugin

Public Class GeoDnsPlugIn_CNAME
  Inherits GeoDnsPlugIn_Base
  Implements IGetAnswerPlugIn

  Public Event AsyncError(ByVal ex As System.Exception) Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.AsyncError
  Public Event LogLine(ByVal text As String) Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.LogLine
  Public Event SaveConfig(ByVal config As String) Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.SaveConfig

#Region "not implemented"

  Public Function SaveState() As String Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.SaveState
    Return ""
  End Function

  Public Sub LoadState(ByVal state As String) Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.LoadState
    REM nothing 
  End Sub

  Public Function InstanceConflict(ByVal config1 As String, ByVal config2 As String, ByRef errorMsg As String) As Boolean Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.InstanceConflict
    Return False
  End Function

#End Region

  Public Function GetPlugInTypeInfo() As JHSoftware.SimpleDNS.Plugin.IPlugInBase.PlugInTypeInfo Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.GetPlugInTypeInfo
    Dim rv As IPlugInBase.PlugInTypeInfo
    rv.Name = "GeoDNS (CNAME)"
    rv.Description = "Serve different host name alias (CNAME) depending on which country a DNS request originates from"
    rv.InfoURL = "http://simpledns.com/plugin-geodns"
    rv.ConfigFile = False
    rv.MultiThreaded = False
    Return rv
  End Function

  Public Function GetDNSAskAbout() As JHSoftware.SimpleDNS.Plugin.DNSAskAbout Implements JHSoftware.SimpleDNS.Plugin.IGetAnswerPlugIn.GetDNSAskAbout
    Dim rv As DNSAskAbout
    rv.RRTypes = Nothing ' all types
    rv.Domain = MyConfig.HostName
    Return rv
  End Function

  Public Function Lookup(ByVal request As JHSoftware.SimpleDNS.Plugin.IDNSRequest) As JHSoftware.SimpleDNS.Plugin.DNSAnswer Implements JHSoftware.SimpleDNS.Plugin.IGetAnswerPlugIn.Lookup
    Dim serv = LookUpServer(request.FromIP)
    If serv Is Nothing Then Return Nothing
    Dim rv = New DNSAnswer
    rv.Records.Add(New DNSRecord With {.Name = request.QName, _
                                       .RRType = DNSRRType.Parse("CNAME"), _
                                       .Data = serv, _
                                       .TTL = MyConfig.RespTTL})
    Return rv
  End Function

  Public Function GetOptionsUI(ByVal instanceID As System.Guid, ByVal dataPath As String) As JHSoftware.SimpleDNS.Plugin.OptionsUI Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.GetOptionsUI
    Return New OptionsUI With {.UseCNAME = True}
  End Function

  Public Sub LoadConfig(ByVal config As String, ByVal instanceID As System.Guid, ByVal dataPath As String, ByRef maxThreads As Integer) Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.LoadConfig
    MyConfig = GeoDnsConfig.Load(config)
  End Sub

  Public Sub StartService() Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.StartService
    BaseStartService()
  End Sub

  Public Sub StopService() Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.StopService
    BaseStopService()
  End Sub

  Public Overrides Sub BaseLog(ByVal s As String)
    RaiseEvent LogLine(s)
  End Sub

  Public Overrides Sub BaseAsyncError(ByVal ex As System.Exception)
    RaiseEvent AsyncError(ex)
  End Sub

End Class

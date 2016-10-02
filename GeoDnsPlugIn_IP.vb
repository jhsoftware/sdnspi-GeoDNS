Imports JHSoftware.SimpleDNS.Plugin

Public Class GeoDnsPlugIn_IP
  Inherits GeoDnsPlugIn_Base
  Implements IGetHostPlugIn

  Public Event AsyncError(ByVal ex As System.Exception) Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.AsyncError
  Public Event LogLine(ByVal text As String) Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.LogLine
  Public Event SaveConfig(ByVal config As String) Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.SaveConfig

#Region "not implemented"

  Public Sub LookupReverse(ByVal req As JHSoftware.SimpleDNS.Plugin.IDNSRequest, ByRef resultName As JHSoftware.SimpleDNS.Plugin.DomainName, ByRef resultTTL As Integer) Implements JHSoftware.SimpleDNS.Plugin.IGetHostPlugIn.LookupReverse
    Throw New NotSupportedException
  End Sub

  Public Sub LookupTXT(ByVal req As JHSoftware.SimpleDNS.Plugin.IDNSRequest, ByRef resultText As String, ByRef resultTTL As Integer) Implements JHSoftware.SimpleDNS.Plugin.IGetHostPlugIn.LookupTXT
    Throw New NotSupportedException
  End Sub

  Public Sub LoadState(ByVal state As String) Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.LoadState
    REM nothing
  End Sub

  Public Function SaveState() As String Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.SaveState
    Return ""
  End Function

  Public Function InstanceConflict(ByVal config1 As String, ByVal config2 As String, ByRef errorMsg As String) As Boolean Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.InstanceConflict
    Return False
  End Function

#End Region

  Public Function GetPlugInTypeInfo() As JHSoftware.SimpleDNS.Plugin.IPlugInBase.PlugInTypeInfo Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.GetPlugInTypeInfo
    Dim rv As IPlugInBase.PlugInTypeInfo
    rv.Name = "GeoDNS (IP)"
    rv.Description = "Serve different IP addresses depending on which country a DNS request originates from"
    rv.InfoURL = "http://simpledns.com/plugin-geodns"
    rv.ConfigFile = False
    rv.MultiThreaded = False
    Return rv
  End Function

  Public Function GetDNSAskAbout() As JHSoftware.SimpleDNS.Plugin.DNSAskAboutGH Implements JHSoftware.SimpleDNS.Plugin.IGetHostPlugIn.GetDNSAskAbout
    Dim rv As DNSAskAboutGH
    rv.Domain = MyConfig.HostName
    rv.ForwardIPv4 = True
    rv.ForwardIPv6 = False
    rv.TXT = False
    Return rv
  End Function

  Public Sub Lookup(ByVal req As JHSoftware.SimpleDNS.Plugin.IDNSRequest, ByRef resultIP As JHSoftware.SimpleDNS.Plugin.IPAddress, ByRef resultTTL As Integer) Implements JHSoftware.SimpleDNS.Plugin.IGetHostPlugIn.Lookup
    Dim serv = LookUpServer(req.FromIP)
    If serv Is Nothing Then Exit Sub
    resultIP = IPAddressV4.Parse(serv)
    resultTTL = MyConfig.RespTTL
  End Sub

  Public Function GetOptionsUI(ByVal instanceID As System.Guid, ByVal dataPath As String) As JHSoftware.SimpleDNS.Plugin.OptionsUI Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.GetOptionsUI
    Return New OptionsUI With {.UseCNAME = False}
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

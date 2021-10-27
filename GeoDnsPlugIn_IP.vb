Imports System.Threading.Tasks
Imports JHSoftware.SimpleDNS.Plugin

Public Class GeoDnsPlugIn_IP
  Inherits GeoDnsPlugIn_Base
  Implements ILookupHost
  Implements IOptionsUI

  Public Property Host As IHost Implements IPlugInBase.Host

  Public Function InstanceConflict(ByVal config1 As String, ByVal config2 As String, ByRef errorMsg As String) As Boolean Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.InstanceConflict
    Return False
  End Function

  Public Function GetPlugInTypeInfo() As TypeInfo Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.GetTypeInfo
    Dim rv As TypeInfo
    rv.Name = "GeoDNS (IP)"
    rv.Description = "Serve different IP addresses depending on which country a DNS request originates from"
    rv.InfoURL = "https://simpledns.plus/plugin-geodns"
    Return rv
  End Function

  Public Async Function LookupHost(name As DomName, ipv6 As Boolean, req As IRequestContext) As Threading.Tasks.Task(Of LookupResult(Of SdnsIP)) Implements JHSoftware.SimpleDNS.Plugin.ILookupHost.LookupHost
    If ipv6 OrElse name <> MyConfig.HostName Then Return Nothing
    Dim serv = LookUpServer(req.FromIP)
    If serv Is Nothing Then Return Nothing
    Return New LookupResult(Of SdnsIP) With {.Value = SdnsIPv4.Parse(serv), .TTL = MyConfig.RespTTL}
  End Function

  Public Function GetOptionsUI(ByVal instanceID As System.Guid, ByVal dataPath As String) As JHSoftware.SimpleDNS.Plugin.OptionsUI Implements JHSoftware.SimpleDNS.Plugin.IOptionsUI.GetOptionsUI
    Return New OptionsUI With {.UseCNAME = False}
  End Function

  Public Sub LoadConfig(ByVal config As String, ByVal instanceID As System.Guid, ByVal dataPath As String) Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.LoadConfig
    MyConfig = GeoDnsConfig.Load(config)
  End Sub

  Private Async Function IPlugInBase_StartService() As Task Implements IPlugInBase.StartService
    BaseStartService()
  End Function

  Public Sub StopService() Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.StopService
    BaseStopService()
  End Sub

  Public Overrides Sub BaseLog(s As String)
    Host.LogLine(s)
  End Sub

  Public Overrides Sub BaseAsyncError(ex As System.Exception)
    Host.AsyncError(ex)
  End Sub

End Class

Imports System.Threading.Tasks
Imports JHSoftware.SimpleDNS.Plugin

Public Class GeoDnsPlugIn_Clone
  Inherits GeoDnsPlugIn_Base
  Implements ICloneAnswer
  Implements IOptionsUI

  Public Property Host As IHost Implements IPlugInBase.Host

  Public Function InstanceConflict(ByVal config1 As String, ByVal config2 As String, ByRef errorMsg As String) As Boolean Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.InstanceConflict
    Return False
  End Function

  Public Function GetPlugInTypeInfo() As TypeInfo Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.GetTypeInfo
    Dim rv As TypeInfo
    rv.Name = "GeoDNS (Clone)"
    rv.Description = "Serve host records cloned from different source records depending on which country a DNS request originates from"
    rv.InfoURL = "https://simpledns.plus/plugin-geodns"
    Return rv
  End Function

  Public Async Function LookupCloneAnswer(ctx As IRequestContext) As Task(Of ICloneAnswer.Result) Implements ICloneAnswer.LookupCloneAnswer
    If (ctx.QType <> DNSRecType.A AndAlso ctx.QType <> DNSRecType.AAAA) OrElse
       ctx.QName <> MyConfig.HostName Then Return Nothing
    Dim serv = LookUpServer(ctx.FromIP)
    If serv Is Nothing Then Return Nothing
    Return New ICloneAnswer.Result With {.CloneFromZone = DomName.Parse(serv), .PrefixLabels = 0, .ForceAA = False}
  End Function

  Public Function GetOptionsUI(ByVal instanceID As System.Guid, ByVal dataPath As String) As JHSoftware.SimpleDNS.Plugin.OptionsUI Implements JHSoftware.SimpleDNS.Plugin.IOptionsUI.GetOptionsUI
    Return New OptionsUI
  End Function

  Public Sub LoadConfig(ByVal config As String, ByVal instanceID As System.Guid, ByVal dataPath As String) Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.LoadConfig
    MyConfig = GeoDnsConfig.Load(config)
  End Sub

  Public Async Function StartService() As Threading.Tasks.Task Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.StartService
    BaseStartService()
  End Function

  Public Sub StopService() Implements JHSoftware.SimpleDNS.Plugin.IPlugInBase.StopService
    BaseStopService()
  End Sub

  Public Overrides Sub BaseLog(s As String)
    Host.LogLine(s)
  End Sub

  Public Overrides Sub BaseAsyncError(ex As Exception)
    Host.AsyncError(ex)
  End Sub

End Class

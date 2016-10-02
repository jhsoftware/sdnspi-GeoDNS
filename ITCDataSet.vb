Class ITCDataSet

  Friend IpRanges As List(Of IpEntry)
  Friend Countries As Dictionary(Of String, Country)
  Friend OrigRangeCt As Integer

  Class Country
    Friend ID As String
    Friend Name As String
  End Class

  Structure IpEntry
    Dim FirstIP As UInteger
    Dim LastIP As UInteger
    Dim Country As Country
  End Structure

  Class IpEntryCmp
    Implements IComparer(Of IpEntry)
    Public Function Compare(ByVal x As IpEntry, ByVal y As IpEntry) As Integer Implements System.Collections.Generic.IComparer(Of IpEntry).Compare
      Dim i = x.FirstIP.CompareTo(y.FirstIP)
      If i <> 0 Then Return i
      Return x.LastIP.CompareTo(y.LastIP)
    End Function
  End Class

  Function Lookup(ByVal ip As JHSoftware.SimpleDNS.Plugin.IPAddressV4) As Country
    If IpRanges.Count = 0 Then Return Nothing
    Dim lv, hv, mv As Integer
    lv = 0
    hv = IpRanges.Count - 1
    Do
      If lv = hv Then
        If IpRanges(lv).FirstIP <= ip.Data AndAlso IpRanges(lv).LastIP >= ip.Data Then Return IpRanges(lv).Country
        Return Nothing
      End If
      mv = lv + (hv - lv) \ 2
      If ip.Data > IpRanges(mv).LastIP Then lv = mv + 1 Else hv = mv
    Loop
  End Function

  Sub LoadFile(ByVal fileName As String, ByVal retry As Boolean)
    Dim f As System.IO.StreamReader
    Dim failCt As Integer
    Do
      Try
        f = My.Computer.FileSystem.OpenTextFileReader(fileName)
        Exit Do
      Catch ex As System.IO.FileNotFoundException
        Throw ex
      Catch ex As System.IO.IOException
        failCt += 1
        REM continue trying for 5 seconds (20 x 1/4 second)
        If Not retry OrElse failCt >= 20 Then Throw ex
        Threading.Thread.Sleep(250)
      End Try
    Loop

    IpRanges = New List(Of IpEntry)
    Countries = New Dictionary(Of String, Country)
    Dim tmpRanges = New List(Of IpEntry)
    Dim en As IpEntry
    Dim x, y As String
    Dim LandID As String

    Dim p As Integer
    While Not f.EndOfStream
      x = f.ReadLine()
      If x.Length = 0 Then Continue While
      If x(0) = "#"c OrElse x(0) = " "c OrElse x(0) = ChrW(9) Then Continue While
      p = 0

      REM from-ip
      If Not TryGetCSVPart(x, p, y) Then Continue While
      If Not UInteger.TryParse(y, en.FirstIP) Then Continue While

      REM to-ip
      If Not TryGetCSVPart(x, p, y) Then Continue While
      If Not UInteger.TryParse(y, en.LastIP) Then Continue While

      REM registry,assigned - skip
      If Not TryGetCSVPart(x, p, y) Then Continue While
      If Not TryGetCSVPart(x, p, y) Then Continue While

      REM country - 2 letter
      If Not TryGetCSVPart(x, p, LandID) Then Continue While
      If LandID.Length <> 2 Then Continue While
      LandID = LandID.ToUpper

      If Not Countries.TryGetValue(LandID, en.Country) Then
        REM country - 3 letter
        If Not TryGetCSVPart(x, p, y) Then Continue While
        REM country - full name
        If Not TryGetCSVPart(x, p, y) Then Continue While
        en.Country = New Country With {.ID = LandID, .Name = y.Trim}
        Countries.Add(LandID, en.Country)
      End If

      tmpRanges.Add(en)

    End While
    f.Close()
    OrigRangeCt = tmpRanges.Count

    If tmpRanges.Count = 0 Then Exit Sub

    REM find and remove overlaps
    tmpRanges.Sort(New IpEntryCmp)
    Dim curEn As IpEntry = tmpRanges(0)
    Dim nxtEn As IpEntry
    Dim i = 1
    While i < tmpRanges.Count
      nxtEn = tmpRanges(i)
      If curEn.LastIP >= nxtEn.FirstIP Then
        If curEn.FirstIP = nxtEn.FirstIP Then
          REM curEn completely included in nxtEn
          curEn = nxtEn
          i += 1
          Continue While
        End If
        If curEn.LastIP >= nxtEn.LastIP Then
          REM nxtEn completely included in CurEn
          i += 1
          Continue While
        End If
        REM overlap 
        If curEn.Country.ID = nxtEn.Country.ID Then
          curEn.LastIP = nxtEn.LastIP
          i += 1
          Continue While
        Else
          curEn.LastIP = nxtEn.FirstIP - 1UI
          REM fall through to End While...
        End If

        REM ranges right next to each other with same country?
      ElseIf curEn.LastIP = nxtEn.FirstIP - 1UI AndAlso _
             curEn.Country.ID = nxtEn.Country.ID Then
        curEn.LastIP = nxtEn.LastIP
        i += 1
        Continue While
      End If

      IpRanges.Add(curEn)
      curEn = nxtEn
    End While

    IpRanges.Add(curEn)

  End Sub

  Friend Shared Function TryGetCSVPart(ByVal s As String, ByRef p As Integer, ByRef rv As String) As Boolean
    If p >= s.Length Then Return False
    If s(p) = """"c Then
      Dim i = s.IndexOf(""""c, p + 1)
      If i < 0 Then Return False
      rv = s.Substring(p + 1, i - p - 1)
      p = i + 1
      If p < s.Length AndAlso s(p) = ","c Then p += 1
      Return True
    Else
      Dim i = s.IndexOf(","c)
      If i < 0 Then
        rv = s.Substring(p)
        p = s.Length
      Else
        rv = s.Substring(p, i - p)
        p = i + 1
      End If
      Return True
    End If

  End Function
End Class

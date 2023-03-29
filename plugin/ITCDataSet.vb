Class ITCDataSet

  Friend Ip4Ranges As List(Of Ip4Entry)
  Friend Ip6Ranges As List(Of Ip6Entry)
  Friend CountryIDs As HashSet(Of String)

  Class Country
    Friend ID As String
    Friend Name As String
  End Class

  Structure Ip4Entry
    Dim FirstIP As SdnsIPv4
    Dim LastIP As SdnsIPv4
    Dim CountryID As String
  End Structure

  Structure Ip6Entry
    Dim FirstIP As SdnsIPv6
    Dim LastIP As SdnsIPv6
    Dim CountryID As String
  End Structure

  Function Lookup(luIP As SdnsIP) As String
    Dim lv, hv, mv As Integer
    If luIP.IPVersion = 4 Then
      If Ip4Ranges.Count = 0 Then Return Nothing
      Dim ip = DirectCast(luIP, SdnsIPv4)
      lv = 0
      hv = Ip4Ranges.Count - 1
      Do
        If lv = hv Then
          If Ip4Ranges(lv).FirstIP.CompareTo(ip) <= 0 AndAlso Ip4Ranges(lv).LastIP.CompareTo(ip) >= 0 Then Return Ip4Ranges(lv).CountryID
          Return Nothing
        End If
        mv = lv + (hv - lv) \ 2
        If ip.CompareTo(Ip4Ranges(mv).LastIP) > 0 Then lv = mv + 1 Else hv = mv
      Loop
    Else
      If Ip6Ranges.Count = 0 Then Return Nothing
      Dim ip = DirectCast(luIP, SdnsIPv6)
      lv = 0
      hv = Ip6Ranges.Count - 1
      Do
        If lv = hv Then
          If Ip6Ranges(lv).FirstIP.CompareTo(ip) <= 0 AndAlso Ip6Ranges(lv).LastIP.CompareTo(ip) >= 0 Then Return Ip6Ranges(lv).CountryID
          Return Nothing
        End If
        mv = lv + (hv - lv) \ 2
        If ip.CompareTo(Ip6Ranges(mv).LastIP) > 0 Then lv = mv + 1 Else hv = mv
      Loop
    End If
  End Function

  Sub LoadFile(fileName As String, retry As Boolean)
    Dim f As System.IO.FileStream
    Dim failCt As Integer
    Do
      Try
        f = New System.IO.FileStream(fileName, IO.FileMode.Open, IO.FileAccess.Read)
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

    Dim ba(5) As Byte
    Dim ct = f.Read(ba, 0, 6)
    If ct < 6 OrElse System.Text.Encoding.ASCII.GetString(ba) <> "GEODNS" Then Throw New Exception("Not a GeoDNS plug-in data file (bad MM)")
    Dim df = New System.IO.Compression.DeflateStream(f, IO.Compression.CompressionMode.Decompress)

    Ip4Ranges = New List(Of Ip4Entry)
    Ip6Ranges = New List(Of Ip6Entry)
    CountryIDs = New HashSet(Of String)

    ReDim ba(3)
    Dim ba16(15) As Byte
    Dim en4 As Ip4Entry, en6 As Ip6Entry
    For Each ipVer In {4, 6}
      df.Read(ba, 0, 4)
      ct = System.BitConverter.ToInt32(ba, 0)
      For i = 1 To ct
        If ipVer = 4 Then
          en4 = New Ip4Entry
          df.Read(ba, 0, 4)
          en4.FirstIP = DirectCast(SdnsIP.FromBytes(ba), SdnsIPv4)
          df.Read(ba, 0, 4)
          en4.LastIP = DirectCast(SdnsIP.FromBytes(ba), SdnsIPv4)
          df.Read(ba, 0, 2)
          en4.CountryID = ChrW(ba(0)) & ChrW(ba(1))
          Ip4Ranges.Add(en4)
          CountryIDs.Add(en4.CountryID)
        Else 'ipver 6
          en6 = New Ip6Entry
          df.Read(ba16, 0, 16)
          en6.FirstIP = DirectCast(SdnsIP.FromBytes(ba16), SdnsIPv6)
          df.Read(ba16, 0, 16)
          en6.LastIP = DirectCast(SdnsIP.FromBytes(ba16), SdnsIPv6)
          df.Read(ba, 0, 2)
          en6.CountryID = ChrW(ba(0)) & ChrW(ba(1))
          Ip6Ranges.Add(en6)
          CountryIDs.Add(en6.CountryID)
        End If
      Next
    Next
    df.Close()
    f.Close()
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

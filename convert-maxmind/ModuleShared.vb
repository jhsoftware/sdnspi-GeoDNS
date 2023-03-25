'--------------------------------------------------------------------------------
'----------- NOTE: This file is shared between "convert-..." projects -----------
'--------------------------------------------------------------------------------

Module ModuleShared

  Public ip4s As New List(Of Item)
  Public ip6s As New List(Of Item)

  Sub AfterLoad(outFile As String)
    ip4s.Sort()
    ip6s.Sort()
    Console.WriteLine("Items sorted")

    Console.WriteLine(CleanOverlaps(ip4s) & " IPv4 overlaps removed")
    Console.WriteLine(CleanOverlaps(ip6s) & " IPv6 overlaps removed")

    Dim newIps = MergeNeighbours(ip4s)
    Console.WriteLine((ip4s.Count - newIps.Count) & " IPv4 neighbours merged")
    ip4s = newIps

    newIps = MergeNeighbours(ip6s)
    Console.WriteLine((ip6s.Count - newIps.Count) & " IPv6 neighbours merged")
    ip6s = newIps

    Console.WriteLine("Left with " & ip4s.Count & " IPv4 sets and " & ip6s.Count & " IPv6 sets")

    Console.WriteLine("Writing output file: " & outFile)
    Dim s1 = New System.IO.FileStream(outFile, IO.FileMode.Create)
    s1.Write(System.Text.Encoding.ASCII.GetBytes("GEODNS"), 0, 6)
    Dim s2 = New System.IO.Compression.DeflateStream(s1, System.IO.Compression.CompressionMode.Compress)
    For Each lst In {ip4s, ip6s}
      s2.Write(System.BitConverter.GetBytes(lst.Count), 0, 4)
      For Each itm In lst
        s2.Write(itm.From, 0, itm.From.Length)
        s2.Write(itm.To, 0, itm.To.Length)
        s2.WriteByte(CByte(AscW(itm.Country(0))))
        s2.WriteByte(CByte(AscW(itm.Country(1))))
      Next
    Next
    s2.Close() : s1.Close()
  End Sub

  Class Item
    Implements IComparable(Of Item)

    Public From As Byte()
    Public [To] As Byte()
    Public Country As String

    Shared Function FromIPCountry(ip As String, country As String) As Item
      Dim i = ip.IndexOf("/")
      If i >= 0 Then
        Dim rv = New Item With {.Country = country}
        rv.From = System.Net.IPAddress.Parse(ip.Substring(0, i)).GetAddressBytes
        Dim snLen = Integer.Parse(ip.Substring(i + 1))
        ReDim rv.To(rv.From.Length - 1)
        Array.Copy(rv.From, rv.To, rv.From.Length)
        Dim bts As Integer, msk As Byte
        For j = 0 To rv.From.Length - 1
          If (j + 1) * 8 <= snLen Then Continue For
          If (j + 1) * 8 >= (snLen + 8) Then
            rv.From(j) = 0
            rv.To(j) = 255
          Else
            bts = (j + 1) * 8 - snLen
            msk = CByte(255) << bts
            rv.From(j) = rv.From(j) And msk
            rv.To(j) = rv.To(j) Or (CByte(255) - msk)
          End If
        Next
        Return rv
      End If

      Throw New Exception("Unexpected format")
    End Function

    Public Function CompareTo(other As Item) As Integer Implements IComparable(Of Item).CompareTo
      Dim c = CompareIP(Me.From, other.From)
      If c <> 0 Then Return c
      Return CompareIP(Me.To, other.To)
    End Function

  End Class

  Function CompareIP(ip1 As Byte(), ip2 As Byte()) As Integer
    Dim c As Integer
    For i = 0 To ip1.Length - 1
      c = ip1(i).CompareTo(ip2(i))
      If c <> 0 Then Return c
    Next
    Return 0
  End Function

  Function IpMinus1(ip As Byte()) As Byte()
    Dim rv(ip.Length - 1) As Byte
    Array.Copy(ip, rv, ip.Length)
    Dim p = rv.Length - 1
    Do
      If rv(p) > 0 Then rv(p) = rv(p) - CByte(1) : Return rv
      rv(p) = 255
      p -= 1
    Loop
  End Function

  Function CleanOverlaps(lst As List(Of Item)) As Integer
    Dim i = 0
    Dim rv = 0
    While i < lst.Count - 1
      REM if they start with the same ip - the second item will have higher or equal to-ip - so remove first
      If CompareIP(lst(i).From, lst(i + 1).From) = 0 Then lst.RemoveAt(i) : rv += 1 : Continue While
      REM if first completely covers second - remove second
      If CompareIP(lst(i).To, lst(i + 1).To) >= 0 Then lst.RemoveAt(i + 1) : rv += 1 : Continue While
      REM if overlap - reduce first range
      If CompareIP(lst(i).To, lst(i + 1).From) >= 0 Then
        lst(i).To = IpMinus1(lst(i + 1).From)
        rv += 1
      End If
      i += 1
    End While
    Return rv
  End Function

  Function MergeNeighbours(lst As List(Of Item)) As List(Of Item)
    Dim rv = New List(Of Item)
    Dim p1 = 0, p2 As Integer, EndIP As Byte()
    While p1 < lst.Count
      EndIP = lst(p1).To
      p2 = p1 + 1
      While p2 < lst.Count AndAlso
            lst(p1).Country = lst(p2).Country AndAlso
            CompareIP(EndIP, IpMinus1(lst(p2).From)) = 0
        EndIP = lst(p2).To
        p2 += 1
      End While
      If p1 = p2 - 1 Then
        rv.Add(lst(p1))
      Else
        rv.Add(New Item With {.Country = lst(p1).Country, .From = lst(p1).From, .[To] = lst(p2 - 1).To})
      End If
      p1 = p2
    End While
    Return rv
  End Function

  REM --------------------------------- CSV ------------------------------------

  Function TryGetCSVPart(s As String, ByRef p As Integer, ByRef rv As String) As Boolean
    If p >= s.Length Then Return False
    If s(p) = """"c Then
      Dim i = s.IndexOf(""""c, p + 1)
      If i < 0 Then Return False
      rv = s.Substring(p + 1, i - p - 1)
      p = i + 1
      If p < s.Length AndAlso s(p) = ","c Then p += 1
      Return True
    Else
      Dim i = s.IndexOf(","c, p)
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

  Iterator Function ParseCSVLine(x As String) As IEnumerable(Of String)
    Dim part As String = Nothing
    Dim pos As Integer = 0
    While TryGetCSVPart(x, pos, part)
      Yield part
    End While
  End Function


End Module

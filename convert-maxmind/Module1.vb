
Module Module1

  Function Main(args As String()) As Integer
    Dim rv = Main2(args)
    If System.Diagnostics.Debugger.IsAttached Then Console.ReadKey()
    Return rv
  End Function

  Public zip As System.IO.Compression.ZipArchive

  Function Main2(args As String()) As Integer
    If args.Count <> 2 Then Console.WriteLine("Error: You must provide 2 arguments - input file and output file") : Return 1

    If Not args(0).EndsWith(".zip") Then Console.WriteLine("Error: Input file name must end with '.zip'") : Return 1
    If Not args(1).EndsWith(".geodns") Then Console.WriteLine("Error: Output file name must end with '.geodns'") : Return 1

    Console.WriteLine("Reading from zip archive '" & args(0) & "':")
    Dim s1 = System.IO.File.OpenRead(args(0))
    zip = New System.IO.Compression.ZipArchive(s1)

    If Not ReadLocations() Then Return 1
    If Not ReadIpList(4) Then Return 1
    If Not ReadIpList(6) Then Return 1

    zip.Dispose()
    s1.Close()

    AfterLoad(args(1))

    Console.WriteLine("Done")
    Return 0
  End Function

  Public Locations As New Dictionary(Of Integer, String)

  Function ReadLocations() As Boolean
    Dim EntryName = "GeoLite2-Country-Locations-en.csv"
    Dim sr = OpenZipEntryReader(EntryName)
    If sr Is Nothing Then Return False
    sr.ReadLine() 'first line is header
    Dim LineNo = 1
    Dim parts As List(Of String)
    While Not sr.EndOfStream
      LineNo += 1
      Try
        parts = ParseCSVLine(sr.ReadLine())
        If parts(4).Length = 2 Then Locations.Add(Integer.Parse(parts(0)), parts(4))
      Catch ex As Exception
        Console.WriteLine("Error in line " & LineNo & ": " & ex.Message)
        Return False
      End Try
    End While
    sr.Close()
    Console.WriteLine("Got " & Locations.Count & " locations")
    Return True
  End Function

  Function ReadIpList(ipVer As Integer) As Boolean
    Dim EntryName = "GeoLite2-Country-Blocks-IPv" & ipVer & ".csv"
    Dim sr = OpenZipEntryReader(EntryName)
    If sr Is Nothing Then Return False
    sr.ReadLine() 'first line is header
    Dim LineNo = 1
    Dim loc1 As Integer, loc2 As Integer, cntry As String
    Dim itm As Item, parts As List(Of String)
    While Not sr.EndOfStream
      LineNo += 1
      Try
        parts = ParseCSVLine(sr.ReadLine())
        loc1 = If(parts(1).Length > 0, Integer.Parse(parts(1)), -1)
        loc2 = If(parts(2).Length > 0, Integer.Parse(parts(2)), -1)
        If loc1 < 0 Then loc1 = loc2
        If loc1 < 0 Then Continue While
        If Not Locations.TryGetValue(loc1, cntry) Then Continue While
        itm = Item.FromIPCountry(parts(0), cntry)
        If itm.From.Length <> If(ipVer = 4, 4, 16) Then Console.WriteLine("Error in line " & LineNo & " - not IPv" & ipVer & " address") : Return False
        If ipVer = 4 Then ip4s.Add(itm) Else ip6s.Add(itm)
      Catch ex As Exception
        Console.WriteLine("Error in line " & LineNo & ": " & ex.Message)
        Return False
      End Try
    End While
    sr.Close()
    Console.WriteLine("Read " & ip4s.Count & " IPv" & ipVer & " ranges")
    Return True
  End Function

  Function OpenZipEntryReader(name As String) As System.IO.StreamReader
    name = name.ToLower()
    For Each ze In zip.Entries
      If ze.Name.ToLower() = name Then
        Console.WriteLine("Reading zip archive entry '" & name & "'")
        Return New System.IO.StreamReader(ze.Open())
      End If
    Next
    Console.WriteLine("Error: zip archive does not contains a '" & name & "' file")
    Return Nothing
  End Function

End Module

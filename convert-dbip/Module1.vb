Module Module1

  Function Main(args As String()) As Integer
    Dim rv = Main2(args)
    If System.Diagnostics.Debugger.IsAttached Then Console.ReadKey()
    Return rv
  End Function

  Function Main2(args As String()) As Integer
    If args.Count <> 2 Then Console.WriteLine("Error: You must provide 2 arguments - input file and output file") : Return 1

    If Not args(0).EndsWith(".gz") Then Console.WriteLine("Error: Input file name must end with '.gz' (be a GZip compressed file)") : Return 1
    If Not args(1).EndsWith(".geodns") Then Console.WriteLine("Error: Output file name must end with '.geodns'") : Return 1

    Dim s1 = System.IO.File.OpenRead(args(0))
    Dim s2 = New System.IO.Compression.GZipStream(s1, System.IO.Compression.CompressionMode.Decompress)
    Dim s3 = New System.IO.StreamReader(s2)

    Console.WriteLine("Reading file input file: " & args(0))
    Dim p As Integer
    Dim x As String, parts As String()
    Dim itm As Item
    Dim LineNo As Integer
    While Not s3.EndOfStream
      x = s3.ReadLine()
      LineNo += 1
      If x.Length = 0 Then Continue While
      parts = ParseCSVLine(x).ToArray()
      If parts(2) = "ZZ" Then Continue While
      itm = New Item With {
        .From = System.Net.IPAddress.Parse(parts(0)).GetAddressBytes(),
        .[To] = System.Net.IPAddress.Parse(parts(1)).GetAddressBytes(),
        .Country = parts(2).ToUpper()}
      If itm.From.Length <> itm.To.Length Then Console.WriteLine("Error: Invalid data in line " & LineNo & " - diffent IP address types") : Return 1
      If itm.Country.Length <> 2 Then Console.WriteLine("Error: Invalid data in line " & LineNo & " - not 2 letter country code") : Return 1
      If itm.From.Length = 4 Then ip4s.Add(itm) Else ip6s.Add(itm)
    End While
    s3.Close() : s2.Close() : s1.Close()
    Console.WriteLine("Read " & ip4s.Count & " IPv4 sets and " & ip6s.Count & " IPv6 sets")

    AfterLoad(args(1))

    Console.WriteLine("Done")
    Return 0
  End Function

End Module

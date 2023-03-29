Friend Class GeoDnsConfig
  Friend HostName As DomName
  Friend DataFile As String
  Friend AutoReload As Boolean
  Friend Regions As New SortedList(Of Integer, Region)
  Friend NextRegionID As Integer = 1
  Friend Countries As New SortedList(Of String, Country)
  Friend RespTTL As Integer
  Friend DefaultServer As String

  Friend Function Save() As String
    Dim sb As New System.Text.StringBuilder
    sb.Append("1") 'version
    sb.Append("|" & PipeEncode(HostName.ToString))
    sb.Append("|" & PipeEncode(DataFile))
    sb.Append(If(AutoReload, "|1", "|0"))
    sb.Append("|" & RespTTL)
    sb.Append("|" & PipeEncode(DefaultServer))
    sb.Append("|" & NextRegionID)
    sb.Append("|" & Regions.Count)
    For Each r In Regions.Values
      sb.Append("|" & r.ID)
      sb.Append("|" & PipeEncode(r.Name))
      sb.Append("|" & PipeEncode(r.Server))
    Next
    sb.Append("|" & Countries.Count)
    For Each c In Countries.Values
      sb.Append("|" & PipeEncode(c.ID))
      sb.Append("|" & PipeEncode(c.Name))
      sb.Append("|" & c.Region)
    Next
    Return sb.ToString
  End Function

  Friend Shared Function Load(ByVal cfg As String) As GeoDnsConfig
    Dim ca = PipeDecode(cfg)
    If ca.Length < 1 OrElse ca(0) <> "1" Then Throw New Exception("Unknown configuration data version")
    Dim rv As New GeoDnsConfig
    rv.HostName = DomName.Parse(ca(1))
    rv.DataFile = ca(2)
    rv.AutoReload = (ca(3) = "1")
    rv.RespTTL = Integer.Parse(ca(4))
    rv.DefaultServer = ca(5)
    rv.NextRegionID = Integer.Parse(ca(6))
    Dim ct = Integer.Parse(ca(7)) 'region count
    Dim reg As Region
    For i = 0 To ct - 1
      reg = New Region With {.ID = Integer.Parse(ca(8 + i * 3)), .Name = ca(9 + i * 3), .Server = ca(10 + i * 3)}
      rv.Regions.Add(reg.ID, reg)
    Next
    Dim p = 8 + ct * 3
    ct = Integer.Parse(ca(p))
    p += 1
    Dim cou As Country
    For i = 0 To ct - 1
      cou = New Country With {.ID = ca(p + i * 3), .Name = ca(p + i * 3 + 1), .Region = Integer.Parse(ca(p + i * 3 + 2))}
      rv.Countries.Add(cou.ID, cou)
    Next
    Return rv
  End Function

  Friend Shared Function LoadDefault() As GeoDnsConfig
    Return Load("1|dummy||1|1800||12|11|1|Africa||2|Europe||3|South Asia||4|Australia - Oceania||5|Central America and Caribbean||6|South America||7|Middle East||8|East & Southeast Asia||9|Central Asia||10|North America||11|Antarctica||255|AD|Andorra|2|AE|United Arab Emirates|7|AF|Afghanistan|3|AG|Antigua and Barbuda|5|AI|Anguilla|5|AL|Albania|2|AM|Armenia|7|AN|Netherlands Antilles|5|AO|Angola|1|AP|Non-spec Asia Pas Location|0|AQ|Antarctica|11|AR|Argentina|6|AS|American Samoa|4|AT|Austria|2|AU|Australia|4|AW|Aruba|5|AX|AAland Islands|2|AZ|Azerbaijan|7|BA|Bosnia and Herzegowina|2|BB|Barbados|5|BD|Bangladesh|3|BE|Belgium|2|BF|Burkina Faso|1|BG|Bulgaria|2|BH|Bahrain|7|BI|Burundi|1|BJ|Benin|1|BL|Saint Barthélemy|10|BM|Bermuda|10|BN|Brunei Darussalam|8|BO|Bolivia|6|BQ|Bonaire, Sint Eustatius, and Saba|10|BR|Brazil|6|BS|Bahamas|5|BT|Bhutan|3|BV|Bouvet Island|11|BW|Botswana|1|BY|Belarus|2|BZ|Belize|5|CA|Canada|10|CC|Cocos (Keeling) Islands|8|CD|Congo The Democratic Republic of The|1|CF|Central African Republic|1|CG|Congo|1|CH|Switzerland|2|CI|Cote D'ivoire|1|CK|Cook Islands|4|CL|Chile|6|CM|Cameroon|1|CN|China|8|CO|Colombia|6|CR|Costa Rica|5|CS|Serbia and Montenegro|2|CU|Cuba|5|CV|Cape Verde|1|CW|Curaçao|10|CX|Christmas Island|4|CY|Cyprus|2|CZ|Czech Republic|2|DE|Germany|2|DJ|Djibouti|1|DK|Denmark|2|DM|Dominica|10|DO|Dominican Republic|5|DZ|Algeria|1|EC|Ecuador|6|EE|Estonia|2|EG|Egypt|1|EH|Western Sahara|1|ER|Eritrea|1|ES|Spain|2|ET|Ethiopia|1|EU|European Union|4|FI|Finland|2|FJ|Fiji|4|FK|Falkland Islands|6|FM|Micronesia Federated States of|4|FO|Faroe Islands|2|FR|France|2|GA|Gabon|1|GB|United Kingdom|2|GD|Grenada|5|GE|Georgia|7|GF|French Guiana|1|GG|Guernsey|2|GH|Ghana|1|GI|Gibraltar|2|GL|Greenland|10|GM|Gambia|1|GN|Guinea|1|GP|Guadeloupe|5|GQ|Equatorial Guinea|1|GR|Greece|2|GS|South Georgia and the South Sandwich Islands|11|GT|Guatemala|5|GU|Guam|4|GW|Guinea-bissau|1|GY|Guyana|6|HK|Hong Kong|8|HM|Heard and McDonald Islands|11|HN|Honduras|5|HR|Croatia (LOCAL Name: Hrvatska)|2|HT|Haiti|5|HU|Hungary|2|ID|Indonesia|8|IE|Ireland|2|IL|Israel|7|IM|Isle of Man|2|IN|India|3|IO|British Indian Ocean Territory|3|IQ|Iraq|7|IR|Iran (ISLAMIC Republic Of)|7|IS|Iceland|2|IT|Italy|2|JE|Jersey|2|JM|Jamaica|5|JO|Jordan|7|JP|Japan|8|KE|Kenya|1|KG|Kyrgyzstan|9|KH|Cambodia|8|KI|Kiribati|4|KM|Comoros|1|KN|Saint Kitts and Nevis|5|KP|North Korea|8|KR|Korea Republic of|8|KW|Kuwait|7|KY|Cayman Islands|5|KZ|Kazakhstan|9|LA|Lao People's Democratic Republic|8|LB|Lebanon|7|LC|Saint Lucia|5|LI|Liechtenstein|2|LK|Sri Lanka|3|LR|Liberia|1|LS|Lesotho|1|LT|Lithuania|2|LU|Luxembourg|2|LV|Latvia|2|LY|Libyan Arab Jamahiriya|1|MA|Morocco|1|MC|Monaco|2|MD|Moldova Republic of|2|ME|Montenegro|2|MF|Saint Martin|5|MG|Madagascar|1|MH|Marshall Islands|4|MK|Macedonia|2|ML|Mali|1|MM|Myanmar|8|MN|Mongolia|8|MO|Macau|8|MP|Northern Mariana Islands|4|MQ|Martinique|10|MR|Mauritania|1|MS|Montserrat|5|MT|Malta|2|MU|Mauritius|1|MV|Maldives|3|MW|Malawi|1|MX|Mexico|10|MY|Malaysia|8|MZ|Mozambique|1|NA|Namibia|1|NC|New Caledonia|4|NE|Niger|1|NF|Norfolk Island|4|NG|Nigeria|1|NI|Nicaragua|5|NL|Netherlands|2|NO|Norway|2|NP|Nepal|3|NR|Nauru|4|NU|Niue|4|NZ|New Zealand|4|OM|Oman|7|PA|Panama|5|PE|Peru|6|PF|French Polynesia|4|PG|Papua New Guinea|8|PH|Philippines|8|PK|Pakistan|3|PL|Poland|2|PM|St. Pierre and Miquelon|10|PN|Pitcairn Islands|4|PR|Puerto Rico|5|PS|Palestinian Territory Occupied|7|PT|Portugal|2|PW|Palau|4|PY|Paraguay|6|QA|Qatar|7|RE|Reunion|1|RO|Romania|2|RS|Serbia|2|RU|Russian Federation|9|RW|Rwanda|1|SA|Saudi Arabia|7|SB|Solomon Islands|4|SC|Seychelles|1|SD|Sudan|1|SE|Sweden|2|SG|Singapore|8|SH|Saint Helena|1|SI|Slovenia|2|SJ|Svalbard and Jan Mayen|2|SK|Slovakia (SLOVAK Republic)|2|SL|Sierra Leone|1|SM|San Marino|2|SN|Senegal|1|SO|Somalia|1|SR|Suriname|6|SS|South Sudan|1|ST|São Tomé and Príncipe|1|SV|El Salvador|5|SX|Sint Maarten|10|SY|Syrian Arab Republic|7|SZ|Swaziland|1|TC|Turks and Caicos Islands|5|TD|Chad|1|TF|French Southern Territories|11|TG|Togo|1|TH|Thailand|8|TJ|Tajikistan|9|TK|Tokelau|4|TL|Timor-Leste|4|TM|Turkmenistan|9|TN|Tunisia|1|TO|Tonga|4|TR|Turkey|7|TT|Trinidad and Tobago|5|TV|Tuvalu|4|TW|Taiwan; Republic of China (ROC)|8|TZ|Tanzania United Republic of|1|UA|Ukraine|2|UG|Uganda|1|UM|U.S. Outlying Islands|4|US|United States|10|UY|Uruguay|6|UZ|Uzbekistan|9|VA|Holy See (VATICAN City State)|2|VC|Saint Vincent and The Grenadines|5|VE|Venezuela|6|VG|Virgin Islands (BRITISH)|5|VI|Virgin Islands (U.S.)|5|VN|Viet Nam|8|VU|Vanuatu|4|WF|Wallis and Futuna Islands|4|WS|Samoa|4|XK|Kosovo|2|YE|Yemen|7|YT|Mayotte|1|ZA|South Africa|1|ZM|Zambia|1|ZW|Zimbabwe|1|ZZ|Reserved|0")
  End Function

  Friend Class Country
    Friend ID As String
    Friend Name As String
    Friend Region As Integer
  End Class

  Friend Class Region
    Implements IComparable
    Friend ID As Integer
    Friend Name As String
    Friend Server As String
    Public Overrides Function ToString() As String
      Return Name
    End Function
    Public Function CompareTo(ByVal obj As Object) As Integer Implements System.IComparable.CompareTo
      Return Name.CompareTo(DirectCast(obj, Region).Name)
    End Function
  End Class

End Class
Imports System.Data
Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Security.Cryptography
Imports Microsoft.VisualBasic
Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json
Imports System.Drawing
Imports System.Drawing.Drawing2D

Public Class cls_master
    Public ConnString As String = ConfigurationManager.ConnectionStrings("conn1").ConnectionString
    Public dateNow As DateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"))

    Public Function autoID(mode As String) As String
        Using con As New MySqlConnection(ConnString)
            Try
                Dim idFormat As String = Format(dateNow, "yyMM")
                Dim table, refNo As String
                table = Nothing
                refNo = Nothing

                If mode = "U" Then
                    table = "users_tbl"
                    refNo = "refID"
                ElseIf mode = "I" Then
                    table = "reports_tbl"
                    refNo = "refID"
                ElseIf mode = "F" Then
                    table = "flightcontrol_tbl"
                    refNo = "refID"
                ElseIf mode = "D" Then
                    table = "documents_tbl"
                    refNo = "refID"
                ElseIf mode = "DP" Then
                    table = "documents_tbl"
                    refNo = "docID"
                ElseIf mode = "PS" Then
                    table = "pilotsettings_tbl"
                    refNo = "refID"
                End If

                idFormat = mode & idFormat & "-"

                Return dynamicQuery("Select Concat('" & idFormat & "', Case When der1.refNo < 1000 Then LPad(der1.refNo, 4, 0) Else der1.refNo End) As refNo" &
                        " From (Select SubString_Index(" & refNo & ", '-', -1) + 1 As refNo From " & table & " WHERE " + refNo + " Like '" & idFormat & "%' Union" &
                        " Select 1 As refNo) As der1 Order By refNo Desc").Rows(0).Item("refNo").ToString

            Catch ex As Exception
                con.Close()
                con.Dispose()
            End Try
        End Using

    End Function

    Public Function Encrypt(ByVal vstrTextToBeEncrypted As String, Optional vstrEncryptionKey As String = "iai2022") As String

        Dim bytValue() As Byte
        Dim bytKey() As Byte
        Dim bytEncoded() As Byte
        Dim bytIV() As Byte = {121, 241, 10, 1, 132, 74, 11, 39, 255, 91, 45, 78, 14, 211, 22, 62}
        Dim intLength As Integer
        Dim intRemaining As Integer
        Dim objMemoryStream As New MemoryStream()
        Dim objCryptoStream As CryptoStream
        Dim objRijndaelManaged As RijndaelManaged


        '   **********************************************************************
        '   ******  Strip any null character from string to be encrypted    ******
        '   **********************************************************************

        vstrTextToBeEncrypted = StripNullCharacters(vstrTextToBeEncrypted)

        '   **********************************************************************
        '   ******  Value must be within ASCII range (i.e., no DBCS chars)  ******
        '   **********************************************************************

        bytValue = Encoding.ASCII.GetBytes(vstrTextToBeEncrypted.ToCharArray)

        intLength = Len(vstrEncryptionKey)

        '   ********************************************************************
        '   ******   Encryption Key must be 256 bits long (32 bytes)      ******
        '   ******   If it is longer than 32 bytes it will be truncated.  ******
        '   ******   If it is shorter than 32 bytes it will be padded     ******
        '   ******   with upper-case Xs.                                  ****** 
        '   ********************************************************************

        If intLength >= 32 Then
            vstrEncryptionKey = Strings.Left(vstrEncryptionKey, 32)
        Else
            intLength = Len(vstrEncryptionKey)
            intRemaining = 32 - intLength
            vstrEncryptionKey = vstrEncryptionKey & Strings.StrDup(intRemaining, "X")
        End If

        bytKey = Encoding.ASCII.GetBytes(vstrEncryptionKey.ToCharArray)

        objRijndaelManaged = New RijndaelManaged()

        '   ***********************************************************************
        '   ******  Create the encryptor and write value to it after it is   ******
        '   ******  converted into a byte array                              ******
        '   ***********************************************************************

        Try

            objCryptoStream = New CryptoStream(objMemoryStream,
              objRijndaelManaged.CreateEncryptor(bytKey, bytIV),
              CryptoStreamMode.Write)
            objCryptoStream.Write(bytValue, 0, bytValue.Length)

            objCryptoStream.FlushFinalBlock()

            bytEncoded = objMemoryStream.ToArray
            objMemoryStream.Close()
            objCryptoStream.Close()
        Catch

        End Try

        '   ***********************************************************************
        '   ******   Return encryptes value (converted from  byte Array to   ******
        '   ******   a base64 string).  Base64 is MIME encoding)             ******
        '   ***********************************************************************
        Return Convert.ToBase64String(bytEncoded)

    End Function

    Public Function Decrypt(ByVal vstrStringToBeDecrypted As String, Optional vstrDecryptionKey As String = "iai2022") As String

        Dim bytDataToBeDecrypted() As Byte
        Dim bytTemp() As Byte
        Dim bytIV() As Byte = {121, 241, 10, 1, 132, 74, 11, 39, 255, 91, 45, 78, 14, 211, 22, 62}
        Dim objRijndaelManaged As New RijndaelManaged()
        Dim objMemoryStream As MemoryStream
        Dim objCryptoStream As CryptoStream
        Dim bytDecryptionKey() As Byte

        Dim intLength As Integer
        Dim intRemaining As Integer
        Dim intCtr As Integer
        Dim strReturnString As String = String.Empty
        Dim achrCharacterArray() As Char
        Dim intIndex As Integer

        '   *****************************************************************
        '   ******   Convert base64 encrypted value to byte array      ******
        '   *****************************************************************

        bytDataToBeDecrypted = Convert.FromBase64String(vstrStringToBeDecrypted)

        '   ********************************************************************
        '   ******   Encryption Key must be 256 bits long (32 bytes)      ******
        '   ******   If it is longer than 32 bytes it will be truncated.  ******
        '   ******   If it is shorter than 32 bytes it will be padded     ******
        '   ******   with upper-case Xs.                                  ****** 
        '   ********************************************************************

        intLength = Len(vstrDecryptionKey)

        If intLength >= 32 Then
            vstrDecryptionKey = Strings.Left(vstrDecryptionKey, 32)
        Else
            intLength = Len(vstrDecryptionKey)
            intRemaining = 32 - intLength
            vstrDecryptionKey = vstrDecryptionKey & Strings.StrDup(intRemaining, "X")
        End If

        bytDecryptionKey = Encoding.ASCII.GetBytes(vstrDecryptionKey.ToCharArray)

        ReDim bytTemp(bytDataToBeDecrypted.Length)

        objMemoryStream = New MemoryStream(bytDataToBeDecrypted)

        '   ***********************************************************************
        '   ******  Create the decryptor and write value to it after it is   ******
        '   ******  converted into a byte array                              ******
        '   ***********************************************************************

        Try

            objCryptoStream = New CryptoStream(objMemoryStream,
               objRijndaelManaged.CreateDecryptor(bytDecryptionKey, bytIV),
               CryptoStreamMode.Read)

            objCryptoStream.Read(bytTemp, 0, bytTemp.Length)

            objCryptoStream.FlushFinalBlock()
            objMemoryStream.Close()
            objCryptoStream.Close()

        Catch

        End Try

        '   *****************************************
        '   ******   Return decypted value     ******
        '   *****************************************

        Return StripNullCharacters(Encoding.ASCII.GetString(bytTemp))

    End Function

    Public Function StripNullCharacters(ByVal vstrStringWithNulls As String) As String

        Dim intPosition As Integer
        Dim strStringWithOutNulls As String

        intPosition = 1
        strStringWithOutNulls = vstrStringWithNulls

        Do While intPosition > 0
            intPosition = InStr(intPosition, vstrStringWithNulls, vbNullChar)

            If intPosition > 0 Then
                strStringWithOutNulls = Left$(strStringWithOutNulls, intPosition - 1) &
                                  Right$(strStringWithOutNulls, Len(strStringWithOutNulls) - intPosition)
            End If

            If intPosition > strStringWithOutNulls.Length Then
                Exit Do
            End If
        Loop

        Return strStringWithOutNulls

    End Function

    Public Sub SendEmail(_email As String, _givenName As String, _userID As String, _password As String, Optional mode As String = Nothing)
        'On Error Resume Next
        Using mm As New MailMessage("noreply@jmpitsolutions.com", _email)

            mm.Subject = "AutoEmail [Do Not Reply]"

            If mode = "issue" Then
                mm.Body = "Hi " & _givenName & "," & vbNewLine & vbNewLine &
                        "Issue Notification Update" & vbNewLine &
                        "Ref ID : " & _userID & vbNewLine &
                        "Details : " & _password & vbNewLine & vbNewLine &
                        "URL: http://iai.jmpitsolutions.com" & vbNewLine

            Else
                mm.Body = "Hi " & _givenName & "," & vbNewLine & vbNewLine &
                        "URL: http://iai.jmpitsolutions.com" & vbNewLine & vbNewLine &
                        "This Is your Log In Credentials: " & vbNewLine &
                        "UserID : " & _userID & vbNewLine &
                        "Password : " & _password & vbNewLine &
                        vbNewLine & vbNewLine &
                        "- IAI Admin"
            End If


            mm.IsBodyHtml = False
            Dim smtp As New SmtpClient()
            smtp.Host = "mail5011.site4now.net"
            smtp.EnableSsl = False
            smtp.UseDefaultCredentials = False
            Dim NetworkCred As New NetworkCredential("noreply@jmpitsolutions.com", "noreply@jmp2022")
            smtp.Credentials = NetworkCred
            smtp.Port = 8889
            smtp.Send(mm)
        End Using

    End Sub

    Public Sub addLogs(refID As String, action As String, remarks As String)
        Dim userID As String = HttpContext.Current.Session("userID")
        Using con As New MySqlConnection(ConnString)
            con.Open()
            Dim sql As String = "INSERT INTO log_tbl (refID, action, remarks, createBy, createDate) VALUES (@refID, @action, @remarks, @createBy, @createDate)"

            Using cmd As New MySqlCommand
                cmd.Parameters.AddWithValue("@refID", refID)
                cmd.Parameters.AddWithValue("@action", action)
                cmd.Parameters.AddWithValue("@remarks", remarks)
                cmd.Parameters.AddWithValue("@createBy", userID)
                cmd.Parameters.AddWithValue("@createDate", dateNow)

                cmd.Connection = con
                cmd.CommandText = sql
                cmd.ExecuteNonQuery()
            End Using
            con.Close()
            con.Dispose()
        End Using
    End Sub

    Public Function dynamicTbl(mode As String, Optional filter As String = Nothing, Optional spName As String = Nothing) As DataTable
        Using con As New MySqlConnection(ConnString)
            con.Open()

            Using cmd As New MySqlCommand(If(spName = Nothing, "sp_details", spName), con)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@_mode", mode)
                cmd.Parameters.AddWithValue("@_where", If(filter = Nothing, Nothing, " WHERE " & filter))

                Using sda As New MySqlDataAdapter()
                    sda.SelectCommand = cmd
                    Using dt As New DataTable()

                        sda.Fill(dt)
                        Return dt
                    End Using
                End Using
            End Using

            con.Close()
            con.Dispose()

        End Using
    End Function

    Public Function dynamicQuery(sql As String) As DataTable
        Using con As New MySqlConnection(ConnString)
            con.Open()

            Using cmd As New MySqlCommand(sql, con)
                Using sda As New MySqlDataAdapter()
                    sda.SelectCommand = cmd
                    Using dt As New DataTable()
                        sda.Fill(dt)
                        Return dt
                    End Using
                End Using
            End Using
            con.Close()
            con.Dispose()
        End Using
    End Function

    Public Sub postOperation(sql As String)
        Using con As New MySqlConnection(ConnString)
            con.Open()
            Using cmd As New MySqlCommand
                cmd.Connection = con
                cmd.CommandText = sql
                cmd.ExecuteNonQuery()
            End Using

            con.Close()
            con.Dispose()
        End Using
    End Sub

    Public Function getActivePage() As String
        Return HttpContext.Current.Session("activePage").ToString
    End Function

    Public Function deserializeJSON(json As String) As DataTable
        Return JsonConvert.DeserializeObject(Of DataTable)(json)
    End Function

    Public Function serializeJSON(json As Object) As String
        Return JsonConvert.SerializeObject(json)
    End Function

    Public Function loadTimeline(refID As String, Optional anonymous As Integer = 0) As String
        Dim htmlStr As String = Nothing
        Dim timelineDT As DataTable = dynamicQuery("Select der1.refID, der1.emailNotif, der1.createDate, der1.action, der1.remarks, der1.dateOnly, der1.duration, requestor.creator, der1.createBy" &
            " From (Select log_tbl.refID, '' as emailNotif, log_tbl.createDate, log_tbl.action, log_tbl.remarks, Str_To_Date(Date_Format(log_tbl.createDate, ""%m/%d/%Y""), ""%m/%d/%Y"") As dateOnly," &
            " TimestampDiff(minute, log_tbl.createDate, '" + Format(dateNow, "yyyy-MM-dd HH:mm:ss") + "') As duration, log_tbl.createBy From log_tbl Where log_tbl.refID = '" + refID + "' Union" &
            " Select reportassessment_tbl.refID, reportassessment_tbl.emailNotif, reportassessment_tbl.createDate, reportassessment_tbl.assessmentID, Concat(" &
            " Case When reportassessment_tbl.safetyIndex Is Not Null AND reportassessment_tbl.safetyIndex <> '' Then Concat('<strong>Safety Index: </strong>', concat(severity_tbl.likelihood, severity_tbl.severity), '<br/>') Else '' End, " &
            " Case When reportassessment_tbl.risk Is Not Null AND reportassessment_tbl.risk <> '' Then Concat('<strong>Risk: </strong>', reportassessment_tbl.risk, '<br/>') Else '' End, " &
            " Case When reportassessment_tbl.consequence Is Not Null AND reportassessment_tbl.consequence <> '' Then Concat('<strong>Consequence: </strong>', reportassessment_tbl.consequence, '<br/>') Else '' End, " &
            " Case When reportassessment_tbl.action Is Not Null AND reportassessment_tbl.action <> '' Then Concat('<strong>Action: </strong>', reportassessment_tbl.action, '<br/>') Else '' End, " &
            " Case When reportassessment_tbl.reviewDate Is Not Null Then Concat('<strong>Review Date: </strong>', reportassessment_tbl.reviewDate, '<br/>') Else '' End, " &
            " Case When reportassessment_tbl.deadlineDate Is Not Null Then Concat('<strong>Deadline Date: </strong>', reportassessment_tbl.deadlineDate, '<br/>') Else '' End, " &
            " Case When Query1.deptname Is Not Null AND reportassessment_tbl.assessmentID = 1 Then Concat('<strong>Forwarded To: </strong>', Query1.deptname, '<br/>') Else '' End," &
            " Case When Query1.assignedTo Is Not Null Then Concat('<strong>Assigned To: </strong>', Query1.assignedTo, '<br/>') Else '' End," &
            " Case When reportassessment_tbl.remarks Is Not Null Then Concat('<strong>Remarks: </strong>', reportassessment_tbl.remarks, '<br/>') Else '' End) As remarks, Str_To_Date(Date_Format(reportassessment_tbl.createDate, ""%m/%d/%Y""), ""%m/%d/%Y"") As dateOnly, " &
            " TimestampDiff(minute, reportassessment_tbl.createDate, '" + Format(dateNow, "yyyy-MM-dd HH:mm:ss") + "') As createDate1," &
            " reportassessment_tbl.createBy From severity_tbl Right Join reportassessment_tbl On severity_tbl.ID = reportassessment_tbl.safetyIndex" &
            " Inner Join (Select reports_tbl.refID, dept_tbl.deptName, Concat(Left(users_tbl.fName, 1), ' ', users_tbl.lName) as assignedTo From reports_tbl Left Join dept_tbl On dept_tbl.ID = reports_tbl.deptID Left Join users_tbl On users_tbl.refID = reports_tbl.assignedID) Query1 On reportassessment_tbl.refID = Query1.refID" &
            " Where reportassessment_tbl.refID = '" + refID + "') As der1 Inner Join" &
            " (Select users_tbl.refID, Concat(users_tbl.fName, ' ', users_tbl.lName) As creator From users_tbl Union Select anonymous_tbl.email," &
            " anonymous_tbl.email As createBy From anonymous_tbl) as requestor On requestor.refID = der1.createBy Order By der1.dateOnly Desc," &
            " der1.createDate Desc")

        htmlStr += "<div class='timeline'>"
        If timelineDT.Rows.Count > 0 Then
            For Each row In timelineDT.AsDataView.ToTable(True, "dateOnly").AsEnumerable
                htmlStr += "<div class='time-label'><span class='bg-red'>" + Format(row.Item("dateOnly"), "dd MMM yyyy") + "</span></div>"

                For Each trow In timelineDT.Select("dateOnly = '" & row.Item("dateOnly") & "'")
                    Dim duration As String = Nothing
                    Select Case trow.Item("duration")
                        Case < 3
                            duration = "a few moments ago"
                            Exit Select
                        Case < 60
                            duration = trow.Item("duration").ToString & " minutes ago"
                            Exit Select
                        Case < 1440
                            Dim totalHours As Integer = Math.Floor(trow.Item("duration") / 60)
                            duration = totalHours & If(totalHours <= 1, " hour ago", " hours ago")
                            Exit Select
                        Case Else
                            duration = Format(trow.Item("createDate"), "hh:mm tt")
                    End Select

                    Dim icon, action As String
                    Dim creator As String = trow.Item("creator").ToString.ToUpper
                    Select Case trow.Item("action").ToString
                        Case "C"
                            icon = "fa fa-file-alt bg-green"
                            action = "created request"
                            If anonymous = 1 Then
                                creator = "ANONYMOUS"
                            End If
                            Exit Select
                        Case "U"
                            icon = "fa fa-edit bg-yellow text-white"
                            action = "updated request"
                            If anonymous = 1 Then
                                creator = "ANONYMOUS"
                            End If
                            Exit Select
                        Case "X"
                            icon = "fa fa-ban bg-maroon"
                            action = "cancelled request"
                            Exit Select
                        Case Else
                            icon = "fa fa-search-plus bg-blue"
                            action = "performed evaluation and tagged request as "

                            Select Case trow.Item("action").ToString
                                Case 0
                                    action += "CLOSED"
                                    Exit Select
                                Case 1
                                    action += "OPEN"
                                    Exit Select
                                'Case 2
                                '    action += "CANCELLED"
                                '    Exit Select
                                Case 3
                                    action += "DEFERRED"
                                    Exit Select
                                Case 4
                                    action += "FOR REVIEW"
                                    Exit Select
                                Case 5
                                    action += "FOR CHECKING"
                                    Exit Select
                                Case 6
                                    action += "AFFIRMED ACTION"
                                    Exit Select
                                Case 7
                                    action += "RESOLVED"
                                    Exit Select
                            End Select
                    End Select

                    htmlStr += "<div>"
                    htmlStr += "<i Class='" + icon + "'></i>"
                    htmlStr += "<div Class='timeline-item'>"
                    htmlStr += "<span Class='time'><i class='fas fa-clock'></i> " + duration + "</span>"
                    htmlStr += "<h3 Class='timeline-header'><a href='#'>" + creator + "</a> " + action + "</h3>"

                    If Not trow.Item("remarks").ToString = Nothing Then
                        Dim emailNotifs As String = Nothing
                        If Not trow.Item("emailNotif").ToString = Nothing Then
                            Dim emailList As New List(Of String)
                            Dim userDT As DataTable = dynamicQuery("Select refID, concat(fname, ' ', lName) as name from users_tbl")
                            emailNotifs = "<strong> Email Notif(s): </strong>"
                            For Each itm In trow.Item("emailNotif").ToString.Split(",")
                                Dim userDR As DataRow = userDT.Select("refID = '" & itm & "'").LastOrDefault
                                If Not IsNothing(userDR) Then
                                    emailList.Add(userDR.Item("name").ToString)
                                End If
                            Next
                            emailNotifs += String.Join(", ", emailList)
                        End If
                        htmlStr += "<div Class='timeline-body'>" + trow.Item("remarks").ToString + emailNotifs + "</div>"
                    End If

                    htmlStr += "</div>"
                    htmlStr += "</div>"
                Next
            Next
        Else
            htmlStr += "<div>"
            htmlStr += "<i Class='fas fa fa-exclamation-triangle bg-yellow text-white'></i>"
            htmlStr += "<div Class='timeline-item'>"
            htmlStr += "<h3 Class='timeline-header'>No timeline record found!</h3>"
            htmlStr += "</div>"
            htmlStr += "</div>"
        End If

        htmlStr += "<div><i Class='fas fa-clock bg-gray'></i></div>"
        htmlStr += "</div>"

        Return htmlStr
    End Function

    Public Sub reduceImageSize(ByVal scaleFactor As Double, ByVal sourcePath As Stream, ByVal targetPath As String)
        Using image = System.Drawing.Image.FromStream(sourcePath)
            Dim newWidth = CInt((image.Width * scaleFactor))
            Dim newHeight = CInt((image.Height * scaleFactor))
            Dim thumbnailImg = New Bitmap(newWidth, newHeight)
            Dim thumbGraph = Graphics.FromImage(thumbnailImg)
            thumbGraph.CompositingQuality = CompositingQuality.HighQuality
            thumbGraph.SmoothingMode = SmoothingMode.HighQuality
            thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic
            Dim imageRectangle = New Rectangle(0, 0, newWidth, newHeight)
            thumbGraph.DrawImage(image, imageRectangle)
            thumbnailImg.Save(targetPath, image.RawFormat)
        End Using
    End Sub

    Public Function checkUser(pageLink As String) As Integer
        Dim clsMaster2 As New cls_master
        Dim userType As String = HttpContext.Current.Session("userType")
        Dim userID As String = HttpContext.Current.Session("userID")

        With clsMaster2
            If userType = 1 Then
                Return 1
            Else
                Dim userDR As DataRow() = .dynamicQuery("Select useraccess_tbl.accessType From useraccess_tbl Inner Join modules_tbl On modules_tbl.ID = useraccess_tbl.moduleID" &
                    " Where modules_tbl.pagelink = '" & pageLink & "' And useraccess_tbl.refID = '" & userID & "'").Select()

                If userDR.Length > 0 Then
                    Return userDR(0).Item("accessType")
                Else
                    Return 0
                End If
            End If

        End With

        Return True

    End Function

    Protected Function romanList(ByVal n As Integer) As String
        Select Case (n)
            ' Test Cases
            Case 1
                Return "I"
            Case 4
                Return "IV"
            Case 5
                Return "V"
            Case 9
                Return "IX"
            Case 10
                Return "X"
            Case 40
                Return "XL"
            Case 50
                Return "L"
            Case 90
                Return "XC"
            Case 100
                Return "C"
            Case 400
                Return "CD"
            Case 500
                Return "D"
            Case 900
                Return "DM"
            Case 1000
                Return "M"
        End Select
    End Function

    Public Function romanNo(ByVal number As Long) As String
        Dim newRomanNo As String = Nothing
        If (number <= 0) Then
            ' When is not a natural number
            Return 0
        End If
        ' Base case collection
        Dim collection As Integer() =
        {1, 4, 5, 9, 10, 40, 50, 90, 100, 400, 500, 900, 1000}
        ' Get the size of collection
        Dim size As Integer = collection.Length

        While (number > 0)
            For Each col In collection
                If number >= col Then
                    newRomanNo += romanList(col)
                    number -= col
                    Exit For
                End If
            Next
        End While
        ' Add new line
        Return newRomanNo

    End Function

    'Public Function generateManual(row As DataRow, Optional mode As Integer = 0, Optional draft As Integer = 0) As String
    '    Dim htmlStr As String = Nothing
    '    Dim mainPNo As String = Nothing

    '    Select Case row.Item("cPageFormat").ToString
    '        Case 2
    '            mainPNo = romanNo(row.Item("pNo").ToString)
    '            Exit Select
    '        Case 3
    '            mainPNo = romanNo(row.Item("pNo").ToString).ToLower
    '            Exit Select
    '        Case Else
    '            mainPNo = row.Item("pNo").ToString
    '    End Select

    '    Dim previousPageVersion As String = Nothing
    '    If mode = 1 Then
    '        Dim previousPageDR As DataRow() = dynamicQuery("Select documentspages_tbl.pDesc From documentspages_tbl Inner Join documentschapters_tbl On documentspages_tbl.chapterID = documentschapters_tbl.ID Inner Join" &
    '            " documents_tbl On documentschapters_tbl.refID = documents_tbl.refID Where documentspages_tbl.pNo = '" & row.Item("pNo").ToString & "' And documentspages_tbl.pVersion < '" & row.Item("pVersion").ToString & "' And" &
    '            " documentschapters_tbl.cNo = '" & row.Item("cNo").ToString & "' And documentschapters_tbl.cType = '" & row.Item("cType").ToString & "' And documents_tbl.docID = '" & row.Item("docID").ToString & "' Order By documentspages_tbl.pVersion Desc limit 1 ").Select()
    '        If previousPageDR.Length > 0 Then
    '            previousPageVersion = previousPageDR(0).Item("pDesc").ToString
    '        End If
    '    End If

    '    htmlStr += " <div class='page-break table-responsive'><table class='table table-bordered tableHeader table-condensed' style='font-size: 16pt'>" &
    '                    " <tr>" &
    '                    " <th rowspan='2' class='text-center'><img src='Images/logo.png' style='margin-top: 20px; width: 350px'/></th>" &
    '                    " <td>ISSUE: <br/> <div style='text-align: center; width: 100%'><strong>" + row.Item("pVersion").ToString + "</strong></div></td>" &
    '                    " </tr>" &
    '                    " <tr>" &
    '                    " <td>DATE: <br/> <div style='text-align: center; width: 100%'><strong>" + Format(row.Item("pLastReviseDate"), "MM/dd/yyyyy") + "</strong></div></td>" &
    '                    " </tr>" &
    '                    " <tr>" &
    '                    " <th class='text-center' style='vertical-align: middle'>" + row.Item("title").ToString + "</th>" &
    '                    " <td>CHAPTER/PAGE <br/> <div style='text-align: center; width: 100%'><strong>" + If(row.Item("cType").ToString = 2, "A", row.Item("cNo").ToString) + " / " + mainPNo + "</strong></div> </td>" &
    '                    " </tr>" &
    '                    " </table></div>"

    '    htmlStr += "<div style='word-wrap: break-word;' class='hide' id='div_previousVersion'>" & previousPageVersion & "</div>"
    '    htmlStr += "<div style='word-wrap: break-word;' id='div_currentVersion'>" & row.Item("pDesc").ToString & ""

    '    If row.Item("pType").ToString = 3 Then
    '        htmlStr += "<table class='table table-bordered tableHeader table-condensed' style='font-size: 14pt'><tr><th class='text-center'>REVISION NO</th><th class='text-center'>REVISION DATE</th><th class='text-center'>CHAPTER/PAGE</th><th class='text-center'>INCORPORATED BY</th></tr>"
    '        For Each trow In dynamicQuery("Select der1.revNo, der1.revDate, der1.chapterNo, der1.pageNo, Concat(Left(users_tbl.fName, 1), '. ', users_tbl.lName) As createBy From (Select documentsrevisions_tbl.revNo," &
    '            " documentsrevisions_tbl.revDate, documentsrevisions_tbl.chapterNo, documentsrevisions_tbl.pageNo, documentsrevisions_tbl.createBy From documentsrevisions_tbl Where" &
    '            " documentsrevisions_tbl.refID = '" + row.Item("refID").ToString + "' Union Select documentsrevisions_tbl.revNo, documentsrevisions_tbl.revDate, documentsrevisions_tbl.chapterNo," &
    '            " documentsrevisions_tbl.pageNo, documentsrevisions_tbl.createBy From documentsrevisions_tbl Inner Join (Select documents_tbl.docID From documents_tbl Where documents_tbl.refID = '" + row.Item("refID").ToString + "') Query1 On Query1.docID = documentsrevisions_tbl.docID Inner Join" &
    '            " documents_tbl On documents_tbl.docID = Query1.docID Where documents_tbl.status = 1) As der1 INNER JOIN users_tbl On users_tbl.refID = der1.createBy Order By der1.revDate, der1.revNo").AsEnumerable
    '            htmlStr += "<tr>"
    '            htmlStr += "<td class='text-center' style='width: 25%'>REVISION " + trow.Item("revNo").ToString + "</td>"
    '            htmlStr += "<td class='text-center' style='width: 25%'>" + Format(trow.Item("revDate"), "MM-dd-yyyy") + "</td>"
    '            htmlStr += "<td class='text-center' style='width: 25%'>CHAPTER " + trow.Item("chapterNo").ToString + "/" + trow.Item("pageNo").ToString + "</td>"
    '            htmlStr += "<td class='text-center'>" + trow.Item("createBy").ToString.ToUpper + "</td>"
    '            htmlStr += "</tr>"
    '        Next
    '        htmlStr += "</table>"
    '    ElseIf row.Item("pType").ToString = 4 Then
    '        Dim pagesDR As DataRow() = dynamicQuery("Select documentschapters_tbl.cPageFormat, case when documentschapters_tbl.cType = 2 then 'A' else documentschapters_tbl.cNo END as cNo, documentspages_tbl.pNo, Case When documentspages_tbl.pVersion = 0 Then 1" &
    '        " Else documentspages_tbl.pVersion End As pVersion, documentspages_tbl.pLastReviseDate From documentspages_tbl Inner Join" &
    '        " documentschapters_tbl On documentspages_tbl.chapterID = documentschapters_tbl.ID Where documentschapters_tbl.refID = '" & row.Item("refID").ToString & "'" &
    '        " Order By documentschapters_tbl.cType, documentschapters_tbl.cNo, documentspages_tbl.pNo").Select()
    '        Dim tableRowCounter As Integer = 1

    '        While tableRowCounter <= pagesDR.Length
    '            htmlStr += "<div class='row'>"
    '            For i As Integer = 0 To 2
    '                htmlStr += "<div class='col-xs-4 col-md-4'><table class='table table-bordered tableHeader table-condensed' style='font-size: 14pt'><tr><th class='text-center'>Page No</th><th class='text-center'>Rev No</th><th class='text-center'>Date</th></tr>"
    '                For r As Integer = 0 To 32
    '                    Dim cpNo As String = "&nbsp;"
    '                    Dim pVersion As String = "&nbsp;"
    '                    Dim pRevisedDate As String = "&nbsp;"

    '                    If pagesDR.Length >= tableRowCounter Then
    '                        Dim pNo As String = Nothing
    '                        Select Case pagesDR(tableRowCounter - 1).Item("cPageFormat").ToString
    '                            Case 2
    '                                pNo = romanNo(pagesDR(tableRowCounter - 1).Item("pNo").ToString)
    '                                Exit Select
    '                            Case 3
    '                                pNo = romanNo(pagesDR(tableRowCounter - 1).Item("pNo").ToString).ToLower
    '                                Exit Select
    '                            Case Else
    '                                pNo = pagesDR(tableRowCounter - 1).Item("pNo").ToString
    '                        End Select

    '                        cpNo = pagesDR(tableRowCounter - 1).Item("cNo").ToString & " / " & pNo
    '                        pVersion = pagesDR(tableRowCounter - 1).Item("pVersion").ToString
    '                        If Not pagesDR(tableRowCounter - 1).Item("pLastReviseDate").ToString = Nothing Then
    '                            pRevisedDate = Format(pagesDR(tableRowCounter - 1).Item("pLastReviseDate"), "MM-dd-yyyy")
    '                        End If
    '                    End If
    '                    htmlStr += "<tr>"
    '                    htmlStr += "<td class='text-center' style='width: 30%'>" + cpNo + "</td>"
    '                    htmlStr += "<td class='text-center' style='width: 30%'>" + pVersion + "</td>"
    '                    htmlStr += "<td class='text-center' >" + pRevisedDate + "</td>"
    '                    htmlStr += "</tr>"

    '                    tableRowCounter += 1
    '                Next
    '                htmlStr += "</table></div>"
    '            Next
    '            htmlStr += "</div>"

    '            htmlStr += "<label>Signatories</label><table class='table table-bordered'>" &
    '                "<tr><td style='width: 50%'>&nbsp;</td><td>&nbsp;</td></tr>" &
    '                "</table>"
    '        End While
    '    End If

    '    htmlStr += "<div class='manualFooter' style='position: fixed; bottom: 0; width: 100%; display: flex; justify-content: center;'><strong>UNCONTROLLED IF PRINTED</strong></div>"
    '    Return htmlStr
    'End Function

    Public Function generateManual(row As DataRow, Optional counter As Integer = 1) As Dictionary(Of String, String)

        Dim previousVersion As String = Nothing
        Dim htmlStr As String = Nothing
        Dim mainPNo As String = Nothing

        Select Case row.Item("cPageFormat").ToString
            Case 2
                mainPNo = romanNo(row.Item("pNo").ToString)
                Exit Select
            Case 3
                mainPNo = romanNo(row.Item("pNo").ToString).ToLower
                Exit Select
            Case Else
                mainPNo = row.Item("pNo").ToString
        End Select

        'Dim previousPageVersion As String = Nothing
        Dim filterList As New List(Of String)
        filterList.Add("documentspages_tbl.pNo = '" & row.Item("pNo").ToString & "'")
        filterList.Add("documentspages_tbl.pVersion <" & If(row.Item("pVersion").ToString = "0", "= " & row.Item("pVersion").ToString, row.Item("pVersion").ToString))
        filterList.Add("documentschapters_tbl.cNo = '" & row.Item("cNo").ToString & "'")
        filterList.Add("documentschapters_tbl.cType = '" & row.Item("cType").ToString & "'")
        filterList.Add("documents_tbl.docID = '" & row.Item("docID").ToString & "'")

        Dim previousPageDR As DataRow() = dynamicQuery("Select documentspages_tbl.pDesc, documentspages_tbl.pVersion From documentspages_tbl Inner Join documentschapters_tbl On documentspages_tbl.chapterID = documentschapters_tbl.ID Inner Join" &
            " documents_tbl On documentschapters_tbl.refID = documents_tbl.refID Where " + String.Join(" AND ", filterList) + " Order By documentspages_tbl.pVersion Desc limit 1 ").Select()

        If previousPageDR.Length > 0 AndAlso previousPageDR(0).Item("pVersion").ToString <> row.Item("pVersion").ToString Then
            previousVersion = previousPageDR(0).Item("pDesc").ToString
            'previousPageVersion = previousPageDR(0).Item("pDesc").ToString
        End If

        htmlStr += " <div><table class='table table-bordered tableHeader table-condensed' style='font-size: 16pt'>" &
                        " <tr>" &
                        " <th rowspan='2' class='text-center'><img src='Images/logo.png' style='margin-top: 20px; width: 350px'/></th>" &
                        " <td>ISSUE: <br/> <div style='text-align: center; width: 100%'><strong>" + row.Item("pVersion").ToString + "</strong></div></td>" &
                        " </tr>" &
                        " <tr>" &
                        " <td>DATE: <br/> <div style='text-align: center; width: 100%'><strong>" + If(row.Item("pLastReviseDate").ToString = Nothing, Nothing, Format(row.Item("pLastReviseDate"), "MM/dd/yyyyy")) + "</strong></div></td>" &
                        " </tr>" &
                        " <tr>" &
                        " <th class='text-center' style='vertical-align: middle'>" + row.Item("title").ToString + "</th>" &
                        " <td>CHAPTER/PAGE <br/> <div style='text-align: center; width: 100%'><strong>" + If(row.Item("cType").ToString = 2, "A", row.Item("cNo").ToString) + " / " + mainPNo + "</strong></div> </td>" &
                        " </tr>" &
                        " </table>"

        'htmlStr += "<div style='word-wrap: break-word;' class='hide div_previousVersion'>" & previousPageVersion & "</div>"
        htmlStr += "<div style='word-wrap: break-word;' class='div_currentVersion' id='div_cVersionID" + counter.ToString + "'>" & row.Item("pDesc").ToString & "</div>"

        If row.Item("pType").ToString = 3 Then
            htmlStr += "<table class='table table-bordered tableHeader table-condensed' style='font-size: 14pt'><tr><th class='text-center'>REVISION NO</th><th class='text-center'>REVISION DATE</th><th class='text-center'>CHAPTER/PAGE</th><th class='text-center'>INCORPORATED BY</th></tr>"
            For Each trow In dynamicQuery("Select der1.revNo, der1.revDate, der1.chapterNo, der1.pageNo, Concat(Left(users_tbl.fName, 1), '. ', users_tbl.lName) As createBy From (Select documentsrevisions_tbl.revNo," &
                " documentsrevisions_tbl.revDate, documentsrevisions_tbl.chapterNo, documentsrevisions_tbl.pageNo, documentsrevisions_tbl.createBy From documentsrevisions_tbl Where" &
                " documentsrevisions_tbl.refID = '" + row.Item("refID").ToString + "' Union Select documentsrevisions_tbl.revNo, documentsrevisions_tbl.revDate, documentsrevisions_tbl.chapterNo," &
                " documentsrevisions_tbl.pageNo, documentsrevisions_tbl.createBy From documentsrevisions_tbl Inner Join (Select documents_tbl.docID From documents_tbl Where documents_tbl.refID = '" + row.Item("refID").ToString + "') Query1 On Query1.docID = documentsrevisions_tbl.docID Inner Join" &
                " documents_tbl On documents_tbl.docID = Query1.docID Where documents_tbl.status = 1) As der1 INNER JOIN users_tbl On users_tbl.refID = der1.createBy Order By der1.revDate, der1.revNo").AsEnumerable
                htmlStr += "<tr>"
                htmlStr += "<td class='text-center' style='width: 25%'>REVISION " + trow.Item("revNo").ToString + "</td>"
                htmlStr += "<td class='text-center' style='width: 25%'>" + Format(trow.Item("revDate"), "MM-dd-yyyy") + "</td>"
                htmlStr += "<td class='text-center' style='width: 25%'>CHAPTER " + trow.Item("chapterNo").ToString + "/" + trow.Item("pageNo").ToString + "</td>"
                htmlStr += "<td class='text-center'>" + trow.Item("createBy").ToString.ToUpper + "</td>"
                htmlStr += "</tr>"
            Next
            htmlStr += "</table>"
        ElseIf row.Item("pType").ToString = 4 Then
            Dim pagesDR As DataRow() = dynamicQuery("Select documents_tbl.layoutSize, documentschapters_tbl.cPageFormat, case when documentschapters_tbl.cType = 2 then 'A' else documentschapters_tbl.cNo END as cNo, documentspages_tbl.pNo, Case When documentspages_tbl.pVersion = 0 Then 1" &
            " Else documentspages_tbl.pVersion End As pVersion, documentspages_tbl.pLastReviseDate From documentspages_tbl Inner Join" &
            " documentschapters_tbl On documentspages_tbl.chapterID = documentschapters_tbl.ID INNER JOIN documents_tbl On documents_tbl.refID = documentschapters_tbl.refID" &
            " Where documentschapters_tbl.refID = '" & row.Item("refID").ToString & "'" &
            " Order By documentschapters_tbl.cType, documentschapters_tbl.cNo, documentspages_tbl.pNo").Select()
            Dim tableRowCounter As Integer = 1
            Dim rowPerPage As Integer = 0

            If pagesDR(0).Item("layoutSize") Then
                rowPerPage = 30
            Else
                rowPerPage = 36
            End If

            While tableRowCounter <= pagesDR.Length
                htmlStr += "<div class='row'>"
                For i As Integer = 0 To 2
                    htmlStr += "<div class='col-xs-4 col-md-4'><table class='table table-bordered tableHeader table-condensed' style='font-size: 14pt'><tr><th class='text-center'>Page No</th><th class='text-center'>Rev No</th><th class='text-center'>Date</th></tr>"
                    For r As Integer = 0 To rowPerPage
                        Dim cpNo As String = "&nbsp;"
                        Dim pVersion As String = "&nbsp;"
                        Dim pRevisedDate As String = "&nbsp;"

                        If pagesDR.Length >= tableRowCounter Then
                            Dim pNo As String = Nothing
                            Select Case pagesDR(tableRowCounter - 1).Item("cPageFormat").ToString
                                Case 2
                                    pNo = romanNo(pagesDR(tableRowCounter - 1).Item("pNo").ToString)
                                    Exit Select
                                Case 3
                                    pNo = romanNo(pagesDR(tableRowCounter - 1).Item("pNo").ToString).ToLower
                                    Exit Select
                                Case Else
                                    pNo = pagesDR(tableRowCounter - 1).Item("pNo").ToString
                            End Select

                            cpNo = pagesDR(tableRowCounter - 1).Item("cNo").ToString & " / " & pNo
                            pVersion = pagesDR(tableRowCounter - 1).Item("pVersion").ToString
                            If Not pagesDR(tableRowCounter - 1).Item("pLastReviseDate").ToString = Nothing Then
                                pRevisedDate = Format(pagesDR(tableRowCounter - 1).Item("pLastReviseDate"), "MM-dd-yyyy")
                            End If
                        End If
                        htmlStr += "<tr>"
                        htmlStr += "<td class='text-center' style='width: 30%'>" + cpNo + "</td>"
                        htmlStr += "<td class='text-center' style='width: 30%'>" + pVersion + "</td>"
                        htmlStr += "<td class='text-center' >" + pRevisedDate + "</td>"
                        htmlStr += "</tr>"

                        tableRowCounter += 1
                        If tableRowCounter > pagesDR.Length Then
                            Exit For
                        End If
                    Next
                    htmlStr += "</table></div>"
                Next
                htmlStr += "</div>"

                    htmlStr += "<label>Signatories</label><table class='table table-bordered'>" &
                        "<tr><td style='width: 50%'>&nbsp;</td><td>&nbsp;</td></tr>" &
                        "</table>"

                End While
            End If

            htmlStr += "</div>"

        Dim dict As New Dictionary(Of String, String)
        dict.Add("previousVersion", previousVersion)
        dict.Add("currentVersion", htmlStr)

        Return dict
    End Function

    Public Function previewManual(refID As String, mode As String) As String
        Dim htmlstr As New StringBuilder
        Dim pagePart As New List(Of String)

        Dim filter As String = Nothing
        If mode = "Chapter" Then
            filter = "documentschapters_tbl.ID = '" & refID & "'"
        ElseIf mode = "Manual" Then
            filter = "documents_tbl.refID = '" & refID & "'"
        End If

        Dim foundDT As DataTable = dynamicQuery("Select documents_tbl.refID, documents_tbl.docID, documents_tbl.title, documents_tbl.coverPage, " &
                    " documentschapters_tbl.ID As ID1, documentschapters_tbl.cType, documentschapters_tbl.cName, Case When documentschapters_tbl.cType = 2 Then 'Appendix' Else 'Chapter' End As chapterTypeName,  " &
                    " documentschapters_tbl.cPageFormat, documentschapters_tbl.cNo, documentspages_tbl.pNo, documentspages_tbl.pDesc, documentspages_tbl.pType, documentspages_tbl.pVersion, documentspages_tbl.pLastReviseDate, documentspages_tbl.headerList" &
                    " From documentschapters_tbl Inner Join documents_tbl On documents_tbl.refID = documentschapters_tbl.refID Inner Join" &
                    " documentspages_tbl On documentspages_tbl.chapterID = documentschapters_tbl.ID Where " + filter + "" &
                    " Order By documentschapters_tbl.cType, documentschapters_tbl.cNo, documentspages_tbl.pNo")

        If Not foundDT.Rows(0).Item("coverPage").ToString = Nothing Then
            pagePart.Add("<div>" + foundDT.Rows(0).Item("coverPage").ToString + "</div>")
        End If

        Dim toc As String = Nothing
        toc += "<div><table class='table table-bordered table-condensed' style='font-size: 22px'>" &
                       "<thead>" &
                       "<tr>" &
                       "<th style='width: 20%' class='text-center'>SUBPART</th>" &
                       "<th class='text-center'>TABLE OF CONTENTS</th>" &
                       "<th style='width: 20%' class='text-center'>PAGE NO</th>" &
                       "</thead><tbody>"
        For i As Integer = 1 To 2
            For Each row In foundDT.AsDataView.ToTable(True, "ID1", "cName", "cNo", "chapterTypeName", "cPageFormat", "cType").Select("cType = '" & i & "'", "cNo")
                toc += "<tr><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>" &
                "<tr><td></td><th class='text-center'>" + row.Item("cName").ToString.ToUpper + "</th><th class='text-center'>" + row.Item("chapterTypeName").ToString.ToUpper + " " + row.Item("cNo").ToString + "</th></tr>"

                For Each trow In foundDT.Select("ID1 = '" & row.Item("ID1").ToString & "'", "pNo")
                    If trow.Item("pType").ToString = 2 Then
                        toc += "<tr><td></td><td>Cover Page</td><td class='text-center'>" + trow.Item("pNo").ToString + "</td></tr>"
                    ElseIf Not trow.Item("headerList").ToString = Nothing Then
                        For Each tocRow In deserializeJSON(trow.Item("headerList").ToString).AsEnumerable
                            Dim splitCol1 As String() = tocRow.Item("col1").ToString.Split(" ")
                            If splitCol1.Length > 1 AndAlso splitCol1(0).Contains(".") Then
                                toc += "<tr><td class='text-right'>" + splitCol1(0).Trim + "</td><td>" + tocRow.Item("col1").ToString.Replace(splitCol1(0), "").Trim + "</td><td class='text-center'>" + trow.Item("pNo").ToString + "</td></tr>"
                            Else
                                toc += "<tr><td></td><td>" + tocRow.Item("col1").ToString + "</td><td class='text-center'>" + trow.Item("pNo").ToString + "</td></tr>"
                            End If
                        Next
                    End If
                Next
            Next
        Next
        toc += "</tbody></table></div>"
        pagePart.Add(toc)

        Dim counter As Integer = 1
        For Each row In foundDT.AsEnumerable
            Dim dict As Dictionary(Of String, String) = generateManual(row, counter)
            pagePart.Add(dict.Item("currentVersion").ToString)

            'previous version
            htmlstr.Append("<div style='word-wrap: break-word;' class='hide div_previousVersion' id='div_pVersionID" + counter.ToString + "'>" & dict.Item("previousVersion") & "</div>")

            counter += 1
        Next

        For Each itm In pagePart
            htmlstr.Append("<table style='width: 100%' class='page'>")
            htmlstr.Append("<thead><tr><td><div style='color: red; font-weight: bold; text-align: center; display: fixed; bottom: 0'>UNCONTROLLED IF PRINTED</div></td></tr></thead>")
            htmlstr.Append("<tbody><tr><td>" + itm + "</td></tr></tbody>")
            htmlstr.Append("</table>")
        Next


        Return htmlstr.ToString
    End Function

    Public Sub addNotifViews(userID As String, requestID As String)
        If dynamicTbl("Notifs", "createBy = '" & userID & "' AND RefID = '" & requestID & "'").Rows.Count > 0 Then
            postOperation("INSERT INTO notifviews_tbl (RefID, ViewedBy, LastViewDate) VALUES ('" + requestID + "', '" + userID + "', '" + Format(dateNow, "yyyy-MM-dd HH:mm:ss") + "')")
        End If
    End Sub

End Class

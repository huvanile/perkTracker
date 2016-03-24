Module Module1
    Dim ID() As String = {}
    Dim TOKEN() As String = {}
    Dim Name() As String = {}
    Dim PreviousPoints() As String = {}
    Dim TimerData As Integer = 0
    Sub Main()
        Console.WriteLine("Welcome to PerkTV Tracker")

        LoadUser()
        LoadUser()
        
redo:
        Console.WriteLine("Please Choose An Option")
        Console.WriteLine("0-Check every 30 minutes       1-Check every hour       2-Check every 15 minutes")
        Dim tmp As String = Trim(Console.ReadLine)
        If tmp = "0" Or tmp = "1" Or tmp = "2" Then
            TimerData = tmp
        Else
            GoTo redo
        End If
        If TimerData = 0 Then
            Console.WriteLine()
            Console.WriteLine("Checking every 30 minutes!")
        Else
            If TimerData = 1 Then
                Console.WriteLine()
                Console.WriteLine("Checking every hour!")
            Else
                Console.WriteLine()
                Console.WriteLine("Checking every 15 minutes!")
            End If

        End If
        Console.WriteLine()
        Console.WriteLine("Name|TimeDate|Points|Points Per Min|Points Per Hr|Points Per Day|Points Per Mon")
        Console.WriteLine("--------------------------------------------------------------------------------")
        Dim a As Boolean = False

        Do Until a = True
            GetJsonData()
            Console.WriteLine()
            Dim sleeptimer As Integer = 0
            If TimerData = 0 Then
                sleeptimer = 1800000
            Else
                If TimerData = 1 Then
                    sleeptimer = 1800000 * 2
                Else
                    sleeptimer = 1800000 / 2
                End If

            End If
            System.Threading.Thread.Sleep(sleeptimer)
        Loop
    End Sub
    Sub GetJsonData()
        For i = 0 To ID.Length - 2
            Dim TheSite As String = "http://api.perk.com/api/user/id/"
            Dim MidTheSite As String = "/token/"
            Dim PostTheSite As String = "/"
            Dim WC As New System.Net.WebClient
            Dim Result As String = WC.DownloadString(TheSite & ID(i) & MidTheSite & TOKEN(i) & PostTheSite)
            Dim x() As String = Split(Result, "availableperks")
            x = Split(x(1), ",")
            x(0) = Trim(Replace(x(0), ":", ""))
            x(0) = Replace(x(0), """", "")
            Dim Points As String = x(0)
            If PreviousPoints(i) = 0 Then PreviousPoints(i) = Points
            If FileIO.FileSystem.FileExists(Name(i) & "-" & ID(i) & ".log") Then
            Else
                FileIO.FileSystem.WriteAllText(Name(i) & "-" & ID(i) & ".log", "Name|TimeDate|Points|Points Per Min|Points Per Hr|Points Per Day|Points Per Mon" & Environment.NewLine & Environment.NewLine, False)
            End If
            If TimerData = 0 Then
                CalculateData30(Points, i)
            Else
                If TimerData = 1 Then
                    CalculateDataHour(Points, i)
                Else
                    CalculateData15(Points, i)
                End If

            End If
        Next
        'calculate data for all users
        Dim TmpPoints As Integer = 0

        For i = 0 To PreviousPoints.Length - 2
            TmpPoints += PreviousPoints(i)
        Next
        If PreviousPoints(Name.Length - 1) = 0 Then PreviousPoints(Name.Length - 1) = TmpPoints
        If TimerData = 0 Then
            CalculateData30(TmpPoints, Name.Length - 1)
        Else
            If TimerData = 1 Then
                CalculateDataHour(TmpPoints, Name.Length - 1)
            Else
                CalculateData15(TmpPoints, Name.Length - 1)
            End If

        End If
    End Sub

    Sub CalculateData15(ByRef CurPoints As String, ByRef CurUser As Integer)
        Dim SPACE As String = "|"
        Dim PPM As String = Strings.Left(Math.Round((CurPoints - PreviousPoints(CurUser)) / 15, 2), 14)
        Dim PPH As String = Strings.Left((CurPoints - PreviousPoints(CurUser)) * 4, 13)
        Dim PPD As String = Strings.Left(((CurPoints - PreviousPoints(CurUser)) * 4) * 24, 13)
        Dim PPMonth As String = Strings.Left((((CurPoints - PreviousPoints(CurUser)) * 4) * 24) * 30, 13)
        Dim ActualPoints As String = Strings.Left(CurPoints, 8)
        Dim tmpstr As String = Name(CurUser)
        If tmpstr.Length < 4 Then
            Do Until tmpstr.Length = 4
                tmpstr = " " & tmpstr
            Loop
        End If
        If ActualPoints.Length < 6 Then
            Do Until ActualPoints.Length = 6
                ActualPoints = " " & ActualPoints
            Loop
        End If
        If PPM.Contains(".") Then
            Dim xx() As String = Split(PPM, ".")
            If xx(1).Length = 1 Then
                PPM = PPM & "0"
            End If
        Else
            PPM = PPM & ".00"
        End If
        If PPM.Length < 14 Then
            Do Until PPM.Length = 14
                PPM = " " & PPM
            Loop
        End If
        If PPH.Contains(".") Then
            Dim xx() As String = Split(PPH, ".")
            If xx(1).Length = 1 Then
                PPH = PPH & "0"
            End If
        Else
            PPH = PPH & ".00"
        End If
        If PPH.Length < 13 Then
            Do Until PPH.Length = 13
                PPH = " " & PPH
            Loop
        End If
        If PPD.Contains(".") Then
            Dim xx() As String = Split(PPD, ".")
            If xx(1).Length = 1 Then
                PPD = PPD & "0"
            End If
        Else
            PPD = PPD & ".00"
        End If
        If PPD.Length < 13 Then
            Do Until PPD.Length = 13
                PPD = " " & PPD
            Loop
        End If
        If PPMonth.Contains(".") Then
            Dim xx() As String = Split(PPMonth, ".")
            If xx(1).Length = 1 Then
                PPMonth = PPMonth & "0"
            End If
        Else
            PPMonth = PPMonth & ".00"
        End If
        If PPMonth.Length < 13 Then
            Do Until PPMonth.Length = 13
                PPMonth = " " & PPMonth
            Loop
        End If
        Dim TmpDate As String = DateTime.Now.ToShortTimeString
        Dim x() As String = Split(TmpDate, ":")
        If x(0).Length = 1 Then
            TmpDate = "0" & TmpDate
        End If
        Dim OutString As String = tmpstr & SPACE & " " & TmpDate & SPACE & ActualPoints & SPACE & PPM & SPACE & PPH & SPACE & PPD & SPACE & PPMonth
        'Name|TimeDate|Points|Points Per Min|Points Per Hr|Points Per Day|Points Per Mon
        Console.WriteLine(OutString)
        FileIO.FileSystem.WriteAllText(Name(CurUser) & "-" & ID(CurUser) & ".log", OutString & Environment.NewLine, True)
        PreviousPoints(CurUser) = CurPoints
    End Sub

    Sub CalculateData30(ByRef CurPoints As String, ByRef CurUser As Integer)
        Dim SPACE As String = "|"
        Dim PPM As String = Strings.Left(Math.Round((CurPoints - PreviousPoints(CurUser)) / 30, 2), 14)
        Dim PPH As String = Strings.Left((CurPoints - PreviousPoints(CurUser)) * 2, 13)
        Dim PPD As String = Strings.Left(((CurPoints - PreviousPoints(CurUser)) * 2) * 24, 13)
        Dim PPMonth As String = Strings.Left((((CurPoints - PreviousPoints(CurUser)) * 2) * 24) * 30, 13)
        Dim ActualPoints As String = Strings.Left(CurPoints, 8)
        Dim tmpstr As String = Name(CurUser)
        If tmpstr.Length < 4 Then
            Do Until tmpstr.Length = 4
                tmpstr = " " & tmpstr
            Loop
        End If
        If ActualPoints.Length < 6 Then
            Do Until ActualPoints.Length = 6
                ActualPoints = " " & ActualPoints
            Loop
        End If
        If PPM.Contains(".") Then
            Dim xx() As String = Split(PPM, ".")
            If xx(1).Length = 1 Then
                PPM = PPM & "0"
            End If
        Else
            PPM = PPM & ".00"
        End If
        If PPM.Length < 14 Then
            Do Until PPM.Length = 14
                PPM = " " & PPM
            Loop
        End If
        If PPH.Contains(".") Then
            Dim xx() As String = Split(PPH, ".")
            If xx(1).Length = 1 Then
                PPH = PPH & "0"
            End If
        Else
            PPH = PPH & ".00"
        End If
        If PPH.Length < 13 Then
            Do Until PPH.Length = 13
                PPH = " " & PPH
            Loop
        End If
        If PPD.Contains(".") Then
            Dim xx() As String = Split(PPD, ".")
            If xx(1).Length = 1 Then
                PPD = PPD & "0"
            End If
        Else
            PPD = PPD & ".00"
        End If
        If PPD.Length < 13 Then
            Do Until PPD.Length = 13
                PPD = " " & PPD
            Loop
        End If
        If PPMonth.Contains(".") Then
            Dim xx() As String = Split(PPMonth, ".")
            If xx(1).Length = 1 Then
                PPMonth = PPMonth & "0"
            End If
        Else
            PPMonth = PPMonth & ".00"
        End If
        If PPMonth.Length < 13 Then
            Do Until PPMonth.Length = 13
                PPMonth = " " & PPMonth
            Loop
        End If
        Dim TmpDate As String = DateTime.Now.ToShortTimeString
        Dim x() As String = Split(TmpDate, ":")
        If x(0).Length = 1 Then
            TmpDate = "0" & TmpDate
        End If
        Dim OutString As String = tmpstr & SPACE & " " & TmpDate & SPACE & ActualPoints & SPACE & PPM & SPACE & PPH & SPACE & PPD & SPACE & PPMonth
        'Name|TimeDate|Points|Points Per Min|Points Per Hr|Points Per Day|Points Per Mon
        Console.WriteLine(OutString)
        FileIO.FileSystem.WriteAllText(Name(CurUser) & "-" & ID(CurUser) & ".log", OutString & Environment.NewLine, True)
        PreviousPoints(CurUser) = CurPoints
    End Sub

    Sub CalculateDataHour(ByRef CurPoints As String, ByRef CurUser As Integer)
        Dim SPACE As String = "|"
        Dim PPM As String = Strings.Left(Math.Round((CurPoints - PreviousPoints(CurUser)) / 60, 2), 14)
        Dim PPH As String = Strings.Left((CurPoints - PreviousPoints(CurUser)), 13)
        Dim PPD As String = Strings.Left((CurPoints - PreviousPoints(CurUser)) * 24, 13)
        Dim PPMonth As String = Strings.Left(((CurPoints - PreviousPoints(CurUser)) * 24) * 30, 13)
        Dim ActualPoints As String = Strings.Left(CurPoints, 8)
        Dim tmpstr As String = Name(CurUser)
        If tmpstr.Length < 4 Then
            Do Until tmpstr.Length = 4
                tmpstr = " " & tmpstr
            Loop
        End If
        If ActualPoints.Length < 6 Then
            Do Until ActualPoints.Length = 6
                ActualPoints = " " & ActualPoints
            Loop
        End If
        If PPM.Contains(".") Then
            Dim xx() As String = Split(PPM, ".")
            If xx(1).Length = 1 Then
                PPM = PPM & "0"
            End If
        Else
            PPM = PPM & ".00"
        End If
        If PPM.Length < 14 Then
            Do Until PPM.Length = 14
                PPM = " " & PPM
            Loop
        End If
        If PPH.Contains(".") Then
            Dim xx() As String = Split(PPH, ".")
            If xx(1).Length = 1 Then
                PPH = PPH & "0"
            End If
        Else
            PPH = PPH & ".00"
        End If
        If PPH.Length < 13 Then
            Do Until PPH.Length = 13
                PPH = " " & PPH
            Loop
        End If
        If PPD.Contains(".") Then
            Dim xx() As String = Split(PPD, ".")
            If xx(1).Length = 1 Then
                PPD = PPD & "0"
            End If
        Else
            PPD = PPD & ".00"
        End If
        If PPD.Length < 13 Then
            Do Until PPD.Length = 13
                PPD = " " & PPD
            Loop
        End If
        If PPMonth.Contains(".") Then
            Dim xx() As String = Split(PPMonth, ".")
            If xx(1).Length = 1 Then
                PPMonth = PPMonth & "0"
            End If
        Else
            PPMonth = PPMonth & ".00"
        End If
        If PPMonth.Length < 13 Then
            Do Until PPMonth.Length = 13
                PPMonth = " " & PPMonth
            Loop
        End If
        Dim TmpDate As String = DateTime.Now.ToShortTimeString
        Dim x() As String = Split(TmpDate, ":")
        If x(0).Length = 1 Then
            TmpDate = "0" & TmpDate
        End If
        Dim OutString As String = tmpstr & SPACE & " " & TmpDate & SPACE & ActualPoints & SPACE & PPM & SPACE & PPH & SPACE & PPD & SPACE & PPMonth
        'Name|TimeDate|Points|Points Per Min|Points Per Hr|Points Per Day|Points Per Mon
        Console.WriteLine(OutString)
        FileIO.FileSystem.WriteAllText(Name(CurUser) & "-" & ID(CurUser) & ".log", OutString & Environment.NewLine, True)
        PreviousPoints(CurUser) = CurPoints
    End Sub

    Sub LoadUser()
        If FileIO.FileSystem.FileExists("data.dat") Then
            Dim str As String = FileIO.FileSystem.ReadAllText("data.dat")
            str = Strings.Left(str, str.LastIndexOf(Environment.NewLine))
            Dim x() As String = Split(str, Environment.NewLine)
            ReDim ID(0 To x.Length)
            ReDim TOKEN(0 To x.Length)
            ReDim Name(0 To x.Length)
            ReDim PreviousPoints(0 To x.Length)
            For i = 0 To x.Length - 1
                Dim xx() As String = Split(x(i), ":")
                Name(i) = Strings.Left(xx(0), 3)
                ID(i) = xx(1)
                TOKEN(i) = xx(2)
            Next
            Name(Name.Length - 1) = "Sum"
            ID(Name.Length - 1) = "NULL"
            TOKEN(Name.Length - 1) = "NULL"
        Else
redo:
            Dim Ky As String = ""
            Dim UsrID As String = ""
            Console.WriteLine("Enter your username")
            Dim Name As String = Console.ReadLine
            Console.WriteLine("Enter your password")
            Dim Pass As String = Console.ReadLine
            Console.WriteLine("Enter Bot_Name")
            Dim TmpName As String = Console.ReadLine
            Dim TheSite As String = "http://api.perk.com/oauth/token"
            '  Using sendto As New Net.WebClient
            '   Dim param As New Specialized.NameValueCollection
            Dim param As String = "grant_type=password&username=" & Name & "&password=" & Pass & "&type=email&device_type=web_browser&client_id=11111&client_secret=c437a24bf277dfea375f"
            '  Dim response_bytes = sendto.UploadValues(TheSite, "POST", param)
            Dim response_body = WRequest(TheSite, "POST", param) '(New Text.UTF8Encoding).GetString(response_bytes)
            Dim x() As String = Split(response_body, "{")
            x(1) = Replace(x(1), """", "")
                x(1) = Replace(x(1), "}", "")
                x = Split(x(1), ",")
                Console.WriteLine(x(5))
                Console.WriteLine(x(0))
                Console.WriteLine()
                Dim R() As String = Split(x(5), ":")
                Dim RX() As String = Split(x(0), ":")
                FileIO.FileSystem.WriteAllText("data.dat", TmpName & ":" & R(1) & ":" & RX(1) & Environment.NewLine, True)
BadResult:
                Console.WriteLine("Do you have anoter account to add?")
                Console.WriteLine("Y-(Yes)" & vbTab & "N-(NO)")
                Dim Rslt As String = Console.ReadKey.KeyChar
                If Rslt.ToLower = "y" Or Rslt.ToLower = "n" Then
                    If Rslt.ToLower = "y" Then
                        GoTo redo
                    End If
                Else
                    GoTo BadResult
                End If
            'End Using
        End If
    End Sub

    Function WRequest(URL As String, method As String, POSTdata As String) As String
        Dim responseData As String = ""
        Try
            Dim cookieJar As New Net.CookieContainer()
            Dim hwrequest As Net.HttpWebRequest = Net.WebRequest.Create(URL)
            hwrequest.CookieContainer = cookieJar
            hwrequest.Accept = "*/*"
            hwrequest.AllowAutoRedirect = True
            hwrequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36"
            hwrequest.Timeout = 60000
            hwrequest.Method = method
            If hwrequest.Method = "POST" Then
                hwrequest.ContentType = "application/x-www-form-urlencoded"
                Dim encoding As New Text.ASCIIEncoding() 'Use UTF8Encoding for XML requests
                Dim postByteArray() As Byte = encoding.GetBytes(POSTdata)
                hwrequest.ContentLength = postByteArray.Length
                Dim postStream As IO.Stream = hwrequest.GetRequestStream()
                postStream.Write(postByteArray, 0, postByteArray.Length)
                postStream.Close()
            End If
            Dim hwresponse As Net.HttpWebResponse = hwrequest.GetResponse()
            If hwresponse.StatusCode = Net.HttpStatusCode.OK Then
                Dim responseStream As IO.StreamReader =
                  New IO.StreamReader(hwresponse.GetResponseStream())
                responseData = responseStream.ReadToEnd()
            End If
            hwresponse.Close()
        Catch e As Exception
            responseData = "An error occurred: " & e.Message
        End Try
        Return responseData
    End Function
End Module

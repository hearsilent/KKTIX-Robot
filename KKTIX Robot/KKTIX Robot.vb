Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Net
Imports System.IO
Imports System.IO.Compression
Imports System.Text.RegularExpressions
Imports KKTIX_Robot.SilentWebModule
Imports System.Security.Cryptography
Imports System.Security
Imports System.Xml
Imports System.ComponentModel
Imports System.Xml.Serialization
Imports System.Threading
Public Class KKTIX_Robot
    Public cookies As New CookieContainer()
    Dim parameters As IDictionary(Of String, String) = New Dictionary(Of String, String)()
    Dim _challenge As String = Nothing
    Dim _registrations As String = Nothing
    Dim _TicketURL As String = Nothing
    Dim _recaptchaPublicKey As String = Nothing
    Dim CheckTicketThread As Thread
    Dim GetTicketThread As Thread
    Dim GetThread As Thread
    Dim RegetCaptchaThread As Thread
    Sub CheckTicketBackground()
        While (1)
            Try
                parameters.Clear()
                _challenge = Nothing
                _registrations = Nothing
                _TicketURL = Nothing
                _recaptchaPublicKey = Nothing

                Dim response As HttpWebResponse = HttpWebResponseUtility.CreateGetHttpResponse(KKTIX_URL.Text, Nothing, Nothing, cookies)
                Dim reader As StreamReader = New StreamReader(response.GetResponseStream, System.Text.Encoding.GetEncoding("UTF-8"))
                Dim Line As String = reader.ReadLine
                Dim _authenticityToken As String = Nothing
                Dim _EventTickets As String = Nothing
                Dim _EventFields As String = Nothing

                While (Not reader.EndOfStream)
                    If Regex.Match(Line, "<title>([^<]+)", RegexOptions.IgnoreCase).Success Then
                        'Debug.Print(Regex.Match(Line, "<title>([^<]+)", RegexOptions.IgnoreCase).Groups(1).Value)
                    ElseIf Regex.Match(Line, "<td class=""name"">([^<]+)", RegexOptions.IgnoreCase).Success Then
                        'Debug.Print(Regex.Match(Line, "<td class=""name"">([^<]+)", RegexOptions.IgnoreCase).Groups(1).Value)
                    ElseIf Line.Contains("http://kktix.com/events/") Then
                        If Not _registrations = Nothing Then
                            Exit While
                        End If
                        _registrations = Regex.Match(Line, "<a href=""([^""]+)", RegexOptions.IgnoreCase).Groups(1).Value
                        'Debug.Print(_registrations)
                    End If
                    Line = reader.ReadLine
                End While
                'Try
                '    response.Close()
                'Catch ex As Exception

                'End Try


                If _registrations = Nothing Then
                    ToolStripStatusLabel.Text = "尚未開放購買"
                    ToolStripStatusLabel.ForeColor = Color.DarkOrange
                    Continue While
                End If

                'Registrations
                If Not _registrations = Nothing Then
                    response = HttpWebResponseUtility.CreateGetHttpResponse(_registrations, Nothing, Nothing, cookies)
                    reader = New StreamReader(response.GetResponseStream, System.Text.Encoding.GetEncoding("UTF-8"))
                    Line = reader.ReadLine
                    parameters.Add("_method", "post")
                    While (Not reader.EndOfStream)
                        If Regex.Match(Line, "'recaptchaPublicKey': '([^']+)", RegexOptions.IgnoreCase).Success Then
                            _recaptchaPublicKey = Regex.Match(Line, "'recaptchaPublicKey': '([^']+)", RegexOptions.IgnoreCase).Groups(1).Value
                        ElseIf Regex.Match(Line, "<meta content=""([^""]+)", RegexOptions.IgnoreCase).Success And Line.Contains("csrf-token") Then
                            _authenticityToken = Regex.Match(Line, "<meta content=""([^""]+)", RegexOptions.IgnoreCase).Groups(1).Value
                            parameters.Add("authenticity_token", System.Uri.EscapeDataString(_authenticityToken))
                        ElseIf Line.Contains("app.constant(""EventTickets") Then
                            _EventTickets = "{""Silent"":" & Replace(Replace(Line, "app.constant(""EventTickets"",", "").Substring(0, Len(Replace(Line, "app.constant(""EventTickets"",", "")) - 1), " ", "") & "}"
                        ElseIf Line.Contains("app.constant(""EventFields") Then
                            _EventFields = "{""Silent"":" & Replace(Replace(Line, "app.constant(""EventFields"",", "").Substring(0, Len(Replace(Line, "app.constant(""EventFields"",", "")) - 1), " ", "") & "}"
                        End If
                        Line = reader.ReadLine
                        'Try
                        '    response.Close()
                        'Catch ex As Exception

                        'End Try
                    End While
                End If

                'field
                Dim jsonDoc As Newtonsoft.Json.Linq.JObject = Newtonsoft.Json.JsonConvert.DeserializeObject(_EventFields)
                For i = 0 To jsonDoc.Item("Silent").Count - 1
                    parameters.Add("attendees[0][" & jsonDoc.Item("Silent")(i)("field_key").ToString & "]", "")
                Next
                jsonDoc = Newtonsoft.Json.JsonConvert.DeserializeObject(_EventTickets)
                For i = 0 To jsonDoc.Item("Silent").Count - 1
                    If Replace(TicketComboBox.Items(TicketComboBox.SelectedIndex), " ", "") = jsonDoc.Item("Silent")(i)("name") Then
                        parameters.Add("attendees[0][ticket_id]", jsonDoc.Item("Silent")(i)("id"))
                        'Debug.Print(jsonDoc.Item("Silent")(i)("id"))
                    End If
                Next
                'For Each pair In parameters
                '    Console.WriteLine("{0}, {1}", pair.Key, pair.Value)
                'Next

                'reCAPTCHA
                If Not _recaptchaPublicKey = Nothing Then
                    'https://www.google.com/recaptcha/api/js/recaptcha_ajax.js
                    'response = HttpWebResponseUtility.CreateGetHttpResponse("https://www.google.com/recaptcha/api/js/recaptcha_ajax.js", Nothing, Nothing, cookies)
                    '_recaptchaPublicKey = "6LfHvOwSAAAAAPyW9YF6KybNUYKz8p3lK-CX_d6X"
                    'https://www.google.com/recaptcha/api/challenge?k=6LfHvOwSAAAAAPyW9YF6KybNUYKz8p3lK-CX_d6X&ajax=1&cachestop=0.9726113274587315&lang=locale
                    'https://www.google.com/recaptcha/api/reload?c=
                    'reader = New StreamReader(response.GetResponseStream, System.Text.Encoding.GetEncoding("UTF-8"))
                    'Line = reader.ReadLine
                    Dim CacheStop As String = "0." & GetRandomNumber(111111, 999999999)
                    response = HttpWebResponseUtility.CreateGetHttpResponse("https://www.google.com/recaptcha/api/challenge?k=" & _recaptchaPublicKey & "&ajax=1&cachestop=" & CacheStop & "&lang=zh-TW", Nothing, Nothing, cookies)
                    Dim responseStream As Stream = New GZipStream(response.GetResponseStream, CompressionMode.Decompress)
                    reader = New StreamReader(responseStream, Encoding.[Default])
                    Line = reader.ReadLine
                    'Debug.Print(reader.ReadToEnd)
                    While (Not reader.EndOfStream)
                        If Regex.Match(Line, "challenge : '([^']+)", RegexOptions.IgnoreCase).Success Then
                            _challenge = Regex.Match(Line, "challenge : '([^']+)", RegexOptions.IgnoreCase).Groups(1).Value
                            'Debug.Print(_challenge)
                            Exit While
                        End If
                        Line = reader.ReadLine
                    End While
                    Try
                        response.Close()
                    Catch ex As Exception

                    End Try
                    response = HttpWebResponseUtility.CreateGetHttpResponse("https://www.google.com/recaptcha/api/reload?c=" & _challenge & "&k=" & _recaptchaPublicKey & "&reason=r&type=image&lang=zh-TW", Nothing, Nothing, cookies)
                    responseStream = New GZipStream(response.GetResponseStream, CompressionMode.Decompress)
                    reader = New StreamReader(responseStream, Encoding.[Default])
                    _challenge = Replace(Regex.Match(reader.ReadToEnd, "('[^']+)", RegexOptions.IgnoreCase).Groups(1).Value, "'", "")
                    Try
                        response.Close()
                    Catch ex As Exception

                    End Try
                    response = HttpWebResponseUtility.CreateGetHttpResponse("https://www.google.com/recaptcha/api/image?c=" & _challenge, Nothing, Nothing, cookies)
                    Dim captcha As Bitmap = New Bitmap(response.GetResponseStream())
                    reCAPTCHA.Image = captcha
                    'reCAPTCHA.ImageLocation = "https://www.google.com/recaptcha/api/image?c=" & _challenge
                    Try
                        response.Close()
                    Catch ex As Exception

                    End Try
                End If


                Me.Size = New Point(340, 263)
                ToolStripStatusLabel.Text = "發現野生的票，請輸入驗證碼！"
                ToolStripStatusLabel.ForeColor = Color.DodgerBlue
                CheckBox.Location = New Point(252, 204)
                Exit While

            Catch ex As Exception
                If CheckBox.Checked = True Then
                    ToolStripStatusLabel.Text = "連線逾時"
                Else
                    ToolStripStatusLabel.Text = "程式尚未開始搶票"
                End If
                ToolStripStatusLabel.ForeColor = Color.DarkOrange
            End Try
        End While
    End Sub
    Dim objRandom As New System.Random(CType(System.DateTime.Now.Ticks Mod System.Int32.MaxValue, Integer))
    Public Function GetRandomNumber(Optional ByVal Low As Integer = 1, Optional ByVal High As Integer = 100) As Integer
        Return objRandom.Next(Low, High + 1)
    End Function
    Sub RegetCaptchaBackground()
        Try
            'Try
            '    parameters.Remove("recaptcha_response_field")
            '    parameters.Remove("recaptcha_challenge_field")
            '    parameters.Remove("agree")
            '    parameters.Remove("X-Requested-With")
            '    parameters.Remove("X-HTTP-Accept")
            'Catch ex As Exception

            'End Try
            Dim response As HttpWebResponse
            Dim reader As StreamReader
            Dim Line As String

            If Not _recaptchaPublicKey = Nothing Then
                Dim CacheStop As String = "0." & GetRandomNumber(111111, 999999999)
                response = HttpWebResponseUtility.CreateGetHttpResponse("https://www.google.com/recaptcha/api/challenge?k=" & _recaptchaPublicKey & "&ajax=1&cachestop=" & CacheStop & "&lang=zh-TW", Nothing, Nothing, cookies)
                Dim responseStream As Stream = New GZipStream(response.GetResponseStream, CompressionMode.Decompress)
                reader = New StreamReader(responseStream, Encoding.[Default])
                Line = reader.ReadLine
                'Debug.Print(reader.ReadToEnd)
                While (Not reader.EndOfStream)
                    If Regex.Match(Line, "challenge : '([^']+)", RegexOptions.IgnoreCase).Success Then
                        _challenge = Regex.Match(Line, "challenge : '([^']+)", RegexOptions.IgnoreCase).Groups(1).Value
                        'Debug.Print(_challenge)
                        Exit While
                    End If
                    Line = reader.ReadLine
                End While
                Try
                    response.Close()
                Catch ex As Exception

                End Try
                response = HttpWebResponseUtility.CreateGetHttpResponse("https://www.google.com/recaptcha/api/reload?c=" & _challenge & "&k=" & _recaptchaPublicKey & "&reason=r&type=image&lang=zh-TW", Nothing, Nothing, cookies)
                responseStream = New GZipStream(response.GetResponseStream, CompressionMode.Decompress)
                reader = New StreamReader(responseStream, Encoding.[Default])
                _challenge = Replace(Regex.Match(reader.ReadToEnd, "('[^']+)", RegexOptions.IgnoreCase).Groups(1).Value, "'", "")
                Try
                    response.Close()
                Catch ex As Exception

                End Try
                response = HttpWebResponseUtility.CreateGetHttpResponse("https://www.google.com/recaptcha/api/image?c=" & _challenge, Nothing, Nothing, cookies)
                Dim captcha As Bitmap = New Bitmap(response.GetResponseStream())
                reCAPTCHA.Image = captcha
                'reCAPTCHA.ImageLocation = "https://www.google.com/recaptcha/api/image?c=" & _challenge
                Try
                    response.Close()
                Catch ex As Exception

                End Try
            End If
        Catch ex As Exception

        End Try
    End Sub
    Sub GetTicketBackground()
        Try
            reCaptchaTextBox.Enabled = False

            parameters.Add("recaptcha_response_field", System.Uri.EscapeDataString(reCaptchaTextBox.Text)) 'System.Uri.EscapeDataString(reCaptchaTextBox.Text)
            parameters.Add("recaptcha_challenge_field", System.Uri.EscapeDataString(_challenge)) '_challenge
            parameters.Add("agree", "true")
            parameters.Add("X-Requested-With", "IFrame")
            parameters.Add("X-HTTP-Accept", "*/*")
            'For Each pair In parameters
            '    Console.WriteLine("{0}, {1}", pair.Key, pair.Value)
            'Next

            'Debug.Print(Replace(Replace(_registrations, "/new", "") & "?X-Requested-With=IFrame", "http", "https"))
            Dim response As HttpWebResponse = HttpWebResponseUtility.CreatePostHttpResponse(Replace(Replace(_registrations, "/new", "") & "?X-Requested-With=IFrame HTTP/1.1", "http", "https"), parameters, Nothing, Nothing, Encoding.UTF8, cookies)
            Dim reader As StreamReader = New StreamReader(response.GetResponseStream, System.Text.Encoding.GetEncoding("UTF-8"))
            Dim respHTML As String = reader.ReadToEnd
            'Debug.Print(WebUtility.HtmlDecode(respHTML))
            If WebUtility.HtmlDecode(respHTML).Contains("驗證碼錯誤") Or WebUtility.HtmlDecode(respHTML).Contains("Error") Then
                parameters.Remove("recaptcha_response_field")
                parameters.Remove("recaptcha_challenge_field")
                parameters.Remove("agree")
                parameters.Remove("X-Requested-With")
                parameters.Remove("X-HTTP-Accept")

                RegetCaptchaThread = New Thread(AddressOf Me.RegetCaptchaBackground)
                RegetCaptchaThread.Start()
                MessageBox.Show(Me, "驗證碼輸入錯誤！", "KKTIX Robot")
                'MsgBox("驗證碼輸入錯誤！", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal)
                reCaptchaTextBox.Enabled = True
                reCaptchaTextBox.Text = ""
                Exit Sub
            End If

            'Debug.Print(Regex.Match(WebUtility.HtmlDecode(respHTML), "charset=utf-8'>([^<]+)", RegexOptions.IgnoreCase).Groups(1).Value)
            Dim jsonDoc As Newtonsoft.Json.Linq.JObject = Newtonsoft.Json.JsonConvert.DeserializeObject(Regex.Match(WebUtility.HtmlDecode(respHTML), "charset=utf-8'>([^<]+)", RegexOptions.IgnoreCase).Groups(1).Value)
            ToolStripStatusLabel.Text = "票劵保留至" & jsonDoc.Item("expires_at").ToString
            _TicketURL = Replace(_registrations, "http", "https") & jsonDoc.Item("to_param").ToString & "#/"
            CheckBox.Enabled = False
            Try
                response.Close()
            Catch ex As Exception

            End Try
            MessageBox.Show(Me, "成功搶到票劵，請點擊下方工具列複製票劵網址！", "KKTIX Robot")
            'MsgBox("成功搶到票劵，請點擊下方工具列複製票劵網址！", MsgBoxStyle.Information + MsgBoxStyle.ApplicationModal)
        Catch ex As Exception
            parameters.Remove("recaptcha_response_field")
            parameters.Remove("recaptcha_challenge_field")
            parameters.Remove("agree")
            parameters.Remove("X-Requested-With")
            parameters.Remove("X-HTTP-Accept")

            RegetCaptchaThread = New Thread(AddressOf Me.RegetCaptchaBackground)
            RegetCaptchaThread.Start()
            'MessageBox.Show(Me, "驗證碼輸入錯誤！", "KKTIX Robot")
            MsgBox("驗證碼輸入錯誤！", MsgBoxStyle.Critical)
            reCaptchaTextBox.Enabled = True
            reCaptchaTextBox.Text = ""
        End Try
    End Sub
    Private Sub SendButton_Click(sender As Object, e As EventArgs) Handles SendButton.Click
        GetTicketThread = New Thread(AddressOf Me.GetTicketBackground)
        GetTicketThread.Start()
        SendButton.Enabled = False
    End Sub
    Sub GetBackground()
        Try
            TicketComboBox.Items.Clear()
            Dim response As HttpWebResponse = HttpWebResponseUtility.CreateGetHttpResponse(KKTIX_URL.Text, Nothing, Nothing, cookies)
            Dim reader As StreamReader = New StreamReader(response.GetResponseStream, System.Text.Encoding.GetEncoding("UTF-8"))
            Dim Line As String = reader.ReadLine
            Dim _registrations As String = Nothing

            While (Not reader.EndOfStream)
                If Regex.Match(Line, "<title>([^<]+)", RegexOptions.IgnoreCase).Success Then
                    Me.Text = "KKTIX Robot - " & Regex.Match(Line, "<title>([^<]+)", RegexOptions.IgnoreCase).Groups(1).Value
                ElseIf Regex.Match(Line, "<td class=""name"">([^<]+)", RegexOptions.IgnoreCase).Success Then
                    TicketComboBox.Items.Add(Regex.Match(Line, "<td class=""name"">([^<]+)", RegexOptions.IgnoreCase).Groups(1).Value)
                    TicketComboBox.SelectedIndex = 0
                ElseIf Line.Contains("http://kktix.com/events/") Then
                    If Not _registrations = Nothing Then
                        Exit While
                    End If
                    _registrations = Regex.Match(Line, "<a href=""([^""]+)", RegexOptions.IgnoreCase).Groups(1).Value
                    'Debug.Print(_registrations)
                End If
                Line = reader.ReadLine
            End While
            Try
                response.Close()
            Catch ex As Exception

            End Try
        Catch ex As Exception
            MsgBox("讀取活動票劵失敗，請確認活動網址是否正確！", MsgBoxStyle.Critical)
        End Try
    End Sub
    Private Sub GetButton_Click(sender As Object, e As EventArgs) Handles GetButton.Click
        Me.Text = "KKTIX Robot"
        GetThread = New Thread(AddressOf Me.GetBackground)
        GetThread.Start()
    End Sub

    Private Sub CheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox.CheckedChanged
        If CheckBox.Checked = True Then
            KKTIX_URL.Enabled = False
            GetButton.Enabled = False
            CheckTicketThread = New Thread(AddressOf Me.CheckTicketBackground)
            CheckTicketThread.Start()
            TicketComboBox.Enabled = False
            ToolStripStatusLabel.Text = "尋找票劵中 ..."
        Else
            KKTIX_URL.Enabled = True
            GetButton.Enabled = True
            TicketComboBox.Enabled = True
            Try
                CheckTicketThread.Abort()
            Catch ex As Exception

            End Try
            ToolStripStatusLabel.Text = "程式尚未開始搶票"
        End If
    End Sub

    Private Sub KKTIX_URL_TextChanged(sender As Object, e As EventArgs) Handles KKTIX_URL.TextChanged
        TicketComboBox.Items.Clear()
        ToolStripStatusLabel.Text = "程式尚未開始搶票"
        ToolStripStatusLabel.ForeColor = Color.DarkOrange
        Me.Size = New Point(340, 130)
        CheckBox.Location = New Point(252, 72)
        If KKTIX_URL.Text.Contains("kktix.cc/events/") Then
            GetButton.Enabled = True
        Else
            GetButton.Enabled = False
        End If
    End Sub

    Private Sub reCaptchaTextBox_TextChanged(sender As Object, e As EventArgs) Handles reCaptchaTextBox.TextChanged
        If reCaptchaTextBox.Text.Length > 0 Then
            SendButton.Enabled = True
        Else
            SendButton.Enabled = False
        End If
    End Sub

    Private Sub KKTIX_Robot_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Try
            CheckTicketThread.Abort()
        Catch ex As Exception

        End Try
        Try
            GetTicketThread.Abort()
        Catch ex As Exception

        End Try
        Try
            GetThread.Abort()
        Catch ex As Exception

        End Try
        Try
            RegetCaptchaThread.Abort()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TicketComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TicketComboBox.SelectedIndexChanged
        If Not TicketComboBox.Items.Count = 0 Then
            CheckBox.Enabled = True
        Else
            CheckBox.Enabled = False
        End If
    End Sub

    Private Sub KKTIX_Robot_Load(sender As Object, e As EventArgs) Handles Me.Load
        Form.CheckForIllegalCrossThreadCalls = False
        System.Net.ServicePointManager.Expect100Continue = False
    End Sub

    Private Sub ToolStripStatusLabel_Click(sender As Object, e As EventArgs) Handles ToolStripStatusLabel.Click
        If Not _TicketURL = Nothing Then
            Clipboard.Clear()
            Clipboard.SetText(_TicketURL)
            MsgBox("票劵已複製至剪貼簿 !", MsgBoxStyle.Information)
        End If
    End Sub

    Private Sub ToolStripStatusLabel_TextChanged(sender As Object, e As EventArgs) Handles ToolStripStatusLabel.TextChanged
        If Not _TicketURL = Nothing Then
            Clipboard.Clear()
            Clipboard.SetText(_TicketURL)
            MsgBox("票劵已複製至剪貼簿 !", MsgBoxStyle.Information)
        End If
    End Sub

    Private Sub reCAPTCHA_Click(sender As Object, e As EventArgs) Handles reCAPTCHA.Click
        If Not reCaptchaTextBox.Text = "" And SendButton.Enabled = False Then
            Exit Sub
        End If
        RegetCaptchaThread = New Thread(AddressOf Me.RegetCaptchaBackground)
        RegetCaptchaThread.Start()
        reCaptchaTextBox.Text = ""
    End Sub

    Private Sub KKTIX_URL_Click(sender As Object, e As EventArgs) Handles KKTIX_URL.Click
        KKTIX_URL.SelectionStart = 0
        KKTIX_URL.SelectionLength = Me.KKTIX_URL.Text.Length
    End Sub
End Class
Namespace SilentWebModule
    ''' <summary>
    ''' 有關HTTP請求的模組
    ''' </summary>
    Public Class HttpWebResponseUtility
        'Private Shared ReadOnly DefaultUserAgent As String = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0; MAMIJS)"
        Private Shared ReadOnly DefaultUserAgent As String = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36"
        ''' <summary>
        ''' 創建GET方式的HTTP請求
        ''' </summary>
        ''' <param name="url">請求的URL</param>
        ''' <param name="timeout">請求的超時時間</param>
        ''' <param name="userAgent">請求的客戶端瀏覽器信息，可以為空</param>
        ''' <param name="cookies">隨同HTTP請求發送的Cookie信息，如果不需要身分驗證可以為空</param>
        ''' <returns></returns>
        Public Shared Function CreateGetHttpResponse(url As String, timeout As System.Nullable(Of Integer), userAgent As String, cookies As CookieContainer) As HttpWebResponse
            If String.IsNullOrEmpty(url) Then
                Throw New ArgumentNullException("url")
            End If
            Dim request As HttpWebRequest = Nothing
            'Dim request As HttpWebRequest = TryCast(WebRequest.Create(url), HttpWebRequest)
            If url.StartsWith("https", StringComparison.OrdinalIgnoreCase) Then
                ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CheckValidationResult)
                request = TryCast(WebRequest.Create(url), HttpWebRequest)
                request.ProtocolVersion = HttpVersion.Version11
            Else
                request = TryCast(WebRequest.Create(url), HttpWebRequest)
            End If
            request.Method = "GET"
            request.KeepAlive = True
            request.UserAgent = DefaultUserAgent

            If url.Contains("https://www.google.com/recaptcha/api/") Then
                request.Referer = "https://kktix.com/events/hitcon-x-ent/registrations/new"
                request.Host = "www.google.com"
                request.Date = DateTime.UtcNow
                request.ContentType = "text/javascript"
                request.Accept = "application/javascript, */*;q=0.8"
                request.Headers.Add("Accept-Encoding", "gzip, deflate")
                request.Headers.Add("cache-control", "no-cache, no-store, max-age=0, must-revalidate")
                request.Headers.Add("Accept-Language", "zh-Hant-TW,zh-Hant;q=0.5")
                request.Headers.Add("content-encoding", "gzip")
                request.Headers.Add("alternate-protocol", "443:quic")
                request.Headers.Add("DNT", "1")
            End If

            request.AllowAutoRedirect = True
            If Not String.IsNullOrEmpty(userAgent) Then
                request.UserAgent = userAgent
            End If
            If timeout.HasValue Then
                request.Timeout = timeout.Value
            End If
            If cookies IsNot Nothing Then
                request.CookieContainer = cookies
                'request.CookieContainer = New CookieContainer()
                'request.CookieContainer.Add(cookies)
            End If
            Return TryCast(request.GetResponse(), HttpWebResponse)
        End Function
        ''' <summary>
        ''' 創建POST方式的HTTP請求
        ''' </summary>
        ''' <param name="url">請求的URL</param>
        ''' <param name="parameters">隨同請求POST的參數名稱及參數值字典</param>
        ''' <param name="timeout">請求的超時時間</param>
        ''' <param name="userAgent">請求的客戶端瀏覽器信息，可以為空</param>
        ''' <param name="requestEncoding">發送HTTP請求時所用的編碼</param>
        ''' <param name="cookies">隨同HTTP請求發送的Cookie信息，如果不需要身分驗證可以為空</param>
        ''' <returns></returns>
        Public Shared Function CreatePostHttpResponse(url As String, parameters As IDictionary(Of String, String), timeout As System.Nullable(Of Integer), userAgent As String, requestEncoding As Encoding, cookies As CookieContainer) As HttpWebResponse
            If String.IsNullOrEmpty(url) Then
                Throw New ArgumentNullException("url")
            End If
            If requestEncoding Is Nothing Then
                Throw New ArgumentNullException("requestEncoding")
            End If
            Dim request As HttpWebRequest = Nothing
            '如果是發送HTTPS請求
            If url.StartsWith("https", StringComparison.OrdinalIgnoreCase) Then
                ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf CheckValidationResult)
                request = TryCast(WebRequest.Create(url), HttpWebRequest)
                request.ProtocolVersion = HttpVersion.Version11
            Else
                request = TryCast(WebRequest.Create(url), HttpWebRequest)
            End If
            request.Method = "POST"
            request.KeepAlive = True
            request.ContentType = "application/x-www-form-urlencoded"
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8"
            request.Headers.Add("Accept-Language", "zh-TW,zh;q=0.8,en-US;q=0.6,en;q=0.4")
            request.AllowAutoRedirect = True
            'request.Accept = "text/html, application/xhtml+xml, */*;" & "zh-Hant-TW,zh-Hant;q=0.5"
            If Not String.IsNullOrEmpty(userAgent) Then
                request.UserAgent = userAgent
            Else
                request.UserAgent = DefaultUserAgent
            End If

            If timeout.HasValue Then
                request.Timeout = timeout.Value
            End If
            If cookies IsNot Nothing Then
                request.CookieContainer = cookies
                'request.CookieContainer = New CookieContainer()
                'request.CookieContainer.Add(cookies)
            End If
            '如果需要POST數據
            If Not (parameters Is Nothing OrElse parameters.Count = 0) Then
                Dim buffer As New StringBuilder()
                Dim i As Integer = 0
                For Each key As String In parameters.Keys
                    If i > 0 Then
                        buffer.AppendFormat("&{0}={1}", key, parameters(key))
                    Else
                        buffer.AppendFormat("{0}={1}", key, parameters(key))
                    End If
                    i += 1
                Next
                Dim data As Byte() = requestEncoding.GetBytes(buffer.ToString())
                request.ContentLength = data.Length
                Using stream As Stream = request.GetRequestStream()
                    stream.Write(data, 0, data.Length)
                End Using
            End If
            Return TryCast(request.GetResponse(), HttpWebResponse)

        End Function

        Private Shared Function CheckValidationResult(sender As Object, certificate As X509Certificate, chain As X509Chain, errors As SslPolicyErrors) As Boolean
            Return True
            '總是接受
        End Function
    End Class
End Namespace

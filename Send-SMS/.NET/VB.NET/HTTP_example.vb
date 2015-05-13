
Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Web

Module Module1

    Const SMS_TYPE_CLASSIC As String = "classic"
    Const SMS_TYPE_CLASSIC_PLUS As String = "classic_plus"
    Const SMS_TYPE_BASIC As String = "basic"
    Const SMS_TYPE_TEST_CLASSIC As String = "test_classic"
    Const SMS_TYPE_TEST_CLASSIC_PLUS As String = "test_classic_plus"
    Const SMS_TYPE_TEST_BASIC As String = "test_basic"

    Private Function skebbyGatewaySendSMS(
                         ByVal username As String, _
                         ByVal password As String, _
                         ByVal recipients() As String, _
                         ByVal text As String, _
                         Optional ByVal sms_type As String = "basic", _
                         Optional ByVal sender_number As String = "", _
                         Optional ByVal sender_string As String = "", _
                         Optional ByVal user_reference As String = "", _
                         Optional ByVal charset As String = "" _
                     ) As Dictionary(Of String, String)
        Dim parameters, method As String
        Dim URL As String
        Dim result As New Dictionary(Of String, String)
        Dim values() As String
        Dim temp

        URL = "http://gateway.skebby.it/api/send/smseasy/advanced/http.php"

        method = "send_sms_classic"

        Select Case sms_type
            Case SMS_TYPE_CLASSIC
                method = "send_sms_classic"
            Case SMS_TYPE_CLASSIC_PLUS
                method = "send_sms_classic_report"
            Case SMS_TYPE_BASIC
                method = "send_sms_basic"
            Case SMS_TYPE_TEST_CLASSIC
                method = "test_send_sms_classic"
            Case SMS_TYPE_TEST_CLASSIC_PLUS
                method = "test_send_sms_classic_report"
            Case SMS_TYPE_TEST_BASIC
                method = "test_send_sms_basic"
            Case Else
                method = "send_sms_classic"
        End Select

        parameters = "method=" + HttpUtility.UrlEncode(method) + "&" _
                    + "username=" + HttpUtility.UrlEncode(username) + "&" _
                    + "password=" + HttpUtility.UrlEncode(password) + "&" _
                    + "text=" + HttpUtility.UrlEncode(text) + "&" _
                    + "recipients[]=" + String.Join("&recipients[]=", recipients)

        If (sender_number <> "") And (sender_string <> "") Then
            result.Add("status", "failed")
            result.Add("code", "0")
            result.Add("message", "You can specify only one type of sender, numeric or alphanumeric")
            Return result
        End If

        If sender_number <> "" Then parameters = parameters + "&sender_number=" + HttpUtility.UrlEncode(sender_number)
        If sender_string <> "" Then parameters = parameters + "&sender_string=" + HttpUtility.UrlEncode(sender_string)
        If user_reference <> "" Then parameters = parameters + "&user_reference=" + HttpUtility.UrlEncode(user_reference)

        Select Case charset
            Case "UTF-8"
                parameters = parameters + "&charset=" + HttpUtility.UrlEncode("UTF-8")
            Case Else
        End Select

        ' Create POST
        Dim request As WebRequest = WebRequest.Create(URL)
        request.Method = "POST"
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(parameters)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()

        Dim response As WebResponse
        Try
            'Trying to get the response.
            response = request.GetResponse()
        Catch ex As System.Net.WebException
            result.Add("status", "failed")
            result.Add("code", "0")
            result.Add("message", "Network error, unable to send the message")
            Return result
        End Try

        dataStream = response.GetResponseStream()
        Dim reader As StreamReader = New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()
        ' Clean up the streams.
        reader.Close()
        dataStream.Close()
        response.Close()

        ' Return result to calling function
        If responseFromServer.Length > 0 Then
            values = responseFromServer.Split(New Char() {"&"c})
            For Each value In values
                temp = value.Split(New Char() {"="c})
                result.Add(temp(0), temp(1))
            Next
            Return result
        End If
    End Function

    Function skebbyGatewayGetCredit(username As String, password As String, Optional charset As String = "") As Dictionary(Of String, String)
        Dim parameters, method As String
        Dim URL As String
        Dim result As New Dictionary(Of String, String)
        Dim values() As String
        Dim temp

        URL = "http://gateway.skebby.it/api/send/smseasy/advanced/http.php"
        method = "get_credit"

        parameters = "method=" + HttpUtility.UrlEncode(method) + "&" _
                   + "username=" + HttpUtility.UrlEncode(username) + "&" _
                   + "password=" + HttpUtility.UrlEncode(password)

        Select Case charset
            Case "UTF-8"
                parameters = parameters + "&charset=" + HttpUtility.UrlEncode("UTF-8")
            Case Else
        End Select

        ' Create POST
        Dim request As WebRequest = WebRequest.Create(URL)
        request.Method = "POST"
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(parameters)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()

        Dim response As WebResponse
        Try
            'Trying to get the response.
            response = request.GetResponse()
        Catch ex As System.Net.WebException
            result.Add("status", "failed")
            result.Add("code", "0")
            result.Add("message", "Network error, unable to send the message")
            Return result
        End Try

        dataStream = response.GetResponseStream()
        Dim reader As StreamReader = New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()
        ' Clean up the streams.
        reader.Close()
        dataStream.Close()
        response.Close()

        ' Return result to calling function
        If responseFromServer.Length > 0 Then
            values = responseFromServer.Split(New Char() {"&"c})
            For Each value In values
                temp = value.Split(New Char() {"="c})
                result.Add(temp(0), temp(1))
            Next
            Return result
        End If

    End Function

    Sub Main()

        Dim result
        Dim credit_result

        ' Single dispatch
        Dim recipients(0) As String
        recipients(0) = "393471234567"

        ' Multiple dispatch
        ' Dim recipients(1) As String
        ' recipients(0) = "393471234567"
        ' recipients(1) = "393477654321"

        ' ------------ SMS Classic dispatch --------------

        ' SMS CLASSIC dispatch with custom alphanumeric sender
        result = skebbyGatewaySendSMS("username", "password", recipients, "Hi Mike, how are you?", SMS_TYPE_CLASSIC, "", "John")

        ' SMS CLASSIC dispatch with custom numeric sender
        ' result = skebbyGatewaySendSMS("username", "password", recipients, "Hi Mike, how are you?", SMS_TYPE_CLASSIC, "393471234567")


        ' ------------- SMS Basic dispatch ----------------
        ' result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you? By John", SMS_TYPE_BASIC)


        ' ------------ SMS Classic Plus dispatch -----------

        ' SMS CLASSIC PLUS dispatch (with delivery report) with custom alphanumeric sender
        ' result = skebbyGatewaySendSMS("username", "password", recipients, "Hi Mike, how are you?", SMS_TYPE_CLASSIC_PLUS, "", "John")

        ' SMS CLASSIC PLUS dispatch (with delivery report) with custom numeric sender
        ' result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?", SMS_TYPE_CLASSIC_PLUS,"393471234567")

        ' SMS CLASSIC PLUS dispatch (with delivery report) with custom alphanumeric sender and custom reference string
        ' result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?", SMS_TYPE_CLASSIC_PLUS,"393471234567","","reference")

        ' ------------------------------------------------------------------
        '     WARNING! THE SMS_TYPE_TEST* SMS TYPES DOESN'T DISPATCH ANY SMS 
        '     USE THEM ONLY TO CHECK IF YOU CAN REACH THE SKEBBY SERVER 
        ' ------------------------------------------------------------------


        ' ------------- SMS Classic test dispatch ---------

        ' SMS CLASSIC test dispatch with custom alphanumeric sender
        ' result = skebbyGatewaySendSMS("username", "password", recipients, "Hi Mike, how are you?", SMS_TYPE_TEST_CLASSIC, "", "John")

        ' SMS CLASSIC test dispatch with custom numeric sender
        ' result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?", SMS_TYPE_TEST_CLASSIC,"393471234567")


        ' ------------- Testing invio SMS Classic Plus ---------

        ' SMS CLASSIC PLUS test dispatch (with delivery report) with custom alphanumeric sender
        ' result = skebbyGatewaySendSMS("username", "password", recipients, "Hi Mike, how are you?", SMS_TYPE_TEST_CLASSIC_PLUS, "", "John")

        ' SMS CLASSIC PLUS test dispatch (with delivery report) with custom numeric sender
        ' result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?", SMS_TYPE_TEST_CLASSIC_PLUS,"393471234567")


        ' ------------- SMS Basic test dispatch ---------------
        ' result = skebbyGatewaySendSMS("username", "password", recipients, "Hi Mike, how are you? By John", SMS_TYPE_TEST_BASIC)


        If result("status") = "success" Then
            Console.WriteLine("Message Sent!")
            Console.WriteLine("Remaining SMSs: " + result("remaining_sms"))

            If (result.ContainsKey("id")) Then
                Console.WriteLine("ID: " + result("id"))
            End If
        End If

		' ------------------------------------------------------------------
		' Check the complete documentation at http:#www.skebby.com/business/index/send-docs/ 
		' ------------------------------------------------------------------
		' For eventual errors see http:#www.skebby.com/business/index/send-docs/#errorCodesSection
		' WARNING: in case of error DON'T retry the sending, since they are blocking errors
		' ------------------------------------------------------------------					
        If result("status") = "failed" Then
            Console.WriteLine("Sendign Failed")
            Console.WriteLine("Code:" + result("code"))
            Console.WriteLine("Description:" + result("message"))
        End If
        Console.ReadLine()

        ' ------------ REMAINING CREDIT Check -------------
        'credit_result = skebbyGatewayGetCredit("username", "password")

        'If credit_result("status") = "success" Then
        '    Console.WriteLine("Current Balance: " + credit_result("credit_left"))
        '    Console.WriteLine("Remaining SMS Classic: " + credit_result("classic_sms"))
        '    Console.WriteLine("Remaining SMS Basic: " + credit_result("basic_sms"))
        'End If
        'If credit_result("status") = "failed" Then
        '    Console.WriteLine("Sending request failed")
        'End If
        'Console.ReadLine()

    End Sub

End Module
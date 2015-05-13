
Const SMS_TYPE_CLASSIC As String = "classic"
Const SMS_TYPE_CLASSIC_PLUS As String = "classic_plus"
Const SMS_TYPE_BASIC As String = "basic"
Const SMS_TYPE_TEST_CLASSIC As String = "test_classic"
Const SMS_TYPE_TEST_CLASSIC_PLUS As String = "test_classic_plus"
Const SMS_TYPE_TEST_BASIC As String = "test_basic"

Function URLEncode(ByVal Text As String) As String

	Dim i As Integer
	Dim acode As Integer
	Dim chr As String
	Dim hexValue As String
	Dim finalString As String

	finalString = ""

	For i = 1 To Len(Text) Step 1
		acode = Asc(Mid$(Text, i, 1))
		Select Case acode
			Case 48 To 57, 65 To 90, 97 To 122
				' don't touch alphanumeric chars
				finalString = finalString & Mid$(Text, i, 1)
			Case 32
				' replace space with "+"
				'Mid$(Text, i, 1) = "+"
				finalString = finalString & "+"
			Case Else
				hexValue = Hex$(acode)
				Select Case Len(hexValue)
					Case 1
						hexValue = "0" & hexValue
					Case 2
						'ok
					Case Else
						'carattere non valido
						'skip
						hexValue = ""
				End Select
				
				' replace punctuation chars with "%hex"
				finalString = finalString & "%" & hexValue


		End Select
	Next
	Return finalString
End Function


Function skebbyGatewaySendSMS(username As String, password As String, recipients() As String, Text As String, sms_type As String, Optional sender_number As String = "", Optional sender_string As String = "", Optional user_reference As String = "", Optional charset As String = "") As String
	Dim sender_error, url, method, parameters, msg As String

	Dim xmlhttp As Object
	xmlhttp = CreateObject("WinHttp.WinHttpRequest.5.1")


	url = "http://gateway.skebby.it/api/send/smseasy/advanced/http.php"
	sender_error = "You can specify only one type of sender, numeric or alphanumeric"
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

	parameters = "method=" & method & "&" _
				 & "username=" & URLEncode(username) & "&" _
				 & "password=" & URLEncode(password) & "&" _
				 & "text=" & URLEncode(Text) & "&" _
				 & "recipients[]=" & Join(recipients, "&recipients[]=")

	If sender_number <> "" And sender_string <> "" Then
		skebbyGatewaySendSMS = "status=failed&message=" & sender_error
		Exit Function
	End If

	If sender_number <> "" Then parameters = parameters & "&sender_number=" & URLEncode(sender_number)
	If sender_string <> "" Then parameters = parameters & "&sender_string=" & URLEncode(sender_string)

	If user_reference <> "" Then parameters = parameters & "&user_reference=" & URLEncode(user_reference)

	Select Case charset
		Case "UTF-8"
			parameters = parameters & "&charset=UTF-8"
		Case Else
	End Select



	xmlhttp.open("POST", url, False)
	xmlhttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded")
	xmlhttp.setRequestHeader("Content-Length", Len(parameters))
	xmlhttp.Send(parameters)

	If xmlhttp.Status >= 400 And xmlhttp.Status <= 599 Then
		skebbyGatewaySendSMS = "status=failed&message=" & xmlhttp.Status & " - " & xmlhttp.statusText
		Exit Function
	End If

	' ------------------------------------------------------------------
	' Check the complete documentation at http:#www.skebby.com/business/index/send-docs/ 
	' ------------------------------------------------------------------
	' For eventual errors see http:#www.skebby.com/business/index/send-docs/#errorCodesSection
	' WARNING: in case of error DON'T retry the sending, since they are blocking errors
	' ------------------------------------------------------------------    

	msg = xmlhttp.responseText
	xmlhttp = Nothing

	skebbyGatewaySendSMS = msg

End Function

Function skebbyGatewayGetCredit(username As String, password As String, Optional charset As String = "") As String
	Dim url, method, parameters, msg As String
	Dim xmlhttp As Object
	xmlhttp = CreateObject("WinHttp.WinHttpRequest.5.1")

	url = "http://gateway.skebby.it/api/send/smseasy/advanced/http.php"
	method = "get_credit"

	parameters = "method=" & method & "&" _
				 & "username=" & URLEncode(username) & "&" _
				 & "password=" & URLEncode(password)

	Select Case charset
		Case "UTF-8"
			parameters = parameters & "&charset=UTF-8"
		Case Else
	End Select



	xmlhttp.open("POST", url, False)
	xmlhttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded")
	xmlhttp.setRequestHeader("Content-Length", Len(parameters))
	xmlhttp.Send(parameters)

	If xmlhttp.Status >= 400 And xmlhttp.Status <= 599 Then
		skebbyGatewayGetCredit = "status=failed&message=" & xmlhttp.Status & " - " & xmlhttp.statusText
		Exit Function
	End If

	msg = xmlhttp.responseText
	xmlhttp = Nothing

	skebbyGatewayGetCredit = msg

End Function


Private Sub Form_Load()

	Dim recipients(0) As String
	Dim i As Integer

	' Single dispatch
	recipients(0) = "393471234567"

	' Multiple dispatch
	' recipients(0) = "393471234567"
	' recipients(1) = "393497654321"

	' ------------ SMS Classic dispatch --------------

    ' SMS CLASSIC dispatch with custom alphanumeric sender
	Dim result As String
	result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?", SMS_TYPE_CLASSIC,"","John")

	' SMS CLASSIC dispatch with custom numeric sender
	' result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?", SMS_TYPE_CLASSIC,"393471234567")


	' ------------- SMS Basic dispatch ----------------
	' result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you? By John", SMS_TYPE_BASIC)


	' ------------ SMS Classic Plus dispatch -----------

	' SMS CLASSIC PLUS dispatch (with delivery report) with custom alphanumeric sender
	' result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?", SMS_TYPE_CLASSIC_PLUS,"","John")

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
	' result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?", SMS_TYPE_TEST_CLASSIC,"","John")

	' SMS CLASSIC test dispatch with custom numeric sender
	' result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?", SMS_TYPE_TEST_CLASSIC,"393471234567")


	' ------------- SMS Classic Plus test dispatch ---------

	' SMS CLASSIC PLUS test dispatch (with delivery report) with custom alphanumeric sender
	' result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?", SMS_TYPE_TEST_CLASSIC_PLUS,"","John")

	' SMS CLASSIC PLUS test dispatch (with delivery report) with custom numeric sender
	' result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?", SMS_TYPE_TEST_CLASSIC_PLUS,"393471234567")


	' ------------- SMS Basic test dispatch ---------------
	' result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you? By John", SMS_TYPE_TEST_BASIC)


	' ------------ REMAINING CREDIT Check -------------
	' result = skebbyGatewayGetCredit("username", "password")

	Dim responses As String()
	responses = Split(result, "&")
	Dim Response As String = ""
	For Each Item In responses
		Response = Response & Item & vbCrLf
	Next
	MsgBox(Response, vbOKOnly + vbInformation, "Result")
	
End Sub
<%
Const SENDER_ERROR="You can specify only one type of sender, numeric or alphanumeric"
 
function skebbyGatewaySendSMS(username,password,recipients,text,sms_type,sender_number,sender_string,charset)
    url = "http://gateway.skebby.it/api/send/smseasy/advanced/http.php"
    method = ""
    select case sms_type
        case "classic"
            method = "send_sms_classic"
        case "report"
            method = "send_sms_classic_report"
        case else
            method = "send_sms_basic"
    end select
 
    parameters = "method=" & Server.urlencode(method) & "&" _
                & "username=" & Server.urlencode(username) & "&" _
                & "password=" & Server.urlencode(password) & "&" _
                & "text=" & Server.urlencode(text) & "&" _
                & "recipients[]=" & join(recipients,"&recipients[]=")
 
    if sender_number <> "" And sender_string <> "" then
        skebbyGatewaySendSMS = "status=failed&message=" & SENDER_ERROR
        exit function
    end if
 
    if sender_number <> "" then parameters = parameters & "&sender_number=" & Server.urlencode(sender_number)
    if sender_string <> "" then parameters = parameters & "&sender_string=" & Server.urlencode(sender_string)
 
    select case charset
        case "UTF-8"
            parameters = parameters & "&charset=" & Server.urlencode("UTF-8")
        case else
    end select
 
    set xmlhttp = CreateObject("MSXML2.ServerXMLHTTP")
    xmlhttp.open "POST", url, false
    xmlhttp.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
    xmlhttp.setRequestHeader "Content-Length", Len(parameters)
    xmlhttp.Send parameters 
 
    If xmlhttp.Status >= 400 And xmlhttp.Status <= 599 Then
        skebbyGatewaySendSMS = "status=failed&message=" & xmlhttp.Status & " - " & xmlhttp.statusText
        exit function
    End If
 
    msg = xmlhttp.responseText
    set xmlhttp = nothing
 
    skebbyGatewaySendSMS = msg
end function
 
Dim recipients
Dim i
 
' Single dispatch
recipients = array("393471234567")
 
' Multiple dispatch
' recipients = array("393471234567","393497654321")
 
' ------------------------------------------------------------------
' Check the complete documentation at http:#www.skebby.com/business/index/send-docs/ 
' ------------------------------------------------------------------
' For eventual errors see http:#www.skebby.com/business/index/send-docs/#errorCodesSection
' WARNING: in case of error DON'T retry the sending, since they are blocking errors
' ------------------------------------------------------------------    
 
' SMS CLASSIC dispatch with custom alphanumeric sender
result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?","classic","","John","")
 
' SMS Basic dispatch
' result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?","basic","","","")
 
' SMS CLASSIC dispatch with custom numeric sender
' result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?","classic","393471234567","","")
 
' SMS CLASSIC PLUS dispatch (with delivery report) with custom alphanumeric sender
' result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?","report","","John","")
 
' SMS CLASSIC PLUS dispatch (with delivery report) with custom numeric sender
' result = skebbyGatewaySendSMS("username","password",recipients,"Hi Mike, how are you?","report","393471234567","","")
 
responses = split(result,"&")
 
for each item in responses
    Response.write item & "<br/>"
next
 
%>
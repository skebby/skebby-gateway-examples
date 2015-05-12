
<%
	sender = Request.Form("sender")
	receiver = Request.Form("receiver")
	text = Request.Form("text")
	encoding = Request.Form("encoding")
	date = Request.Form("date")
	time = Request.Form("time")
	timestamp = Request.Form("timestamp")
	smsType = Request.Form("smsType")
	
	' data MUST BE SAVED in a file or in a database for memorization to be stored and managed

%>

var qs = require('querystring');
var https = require('https');

var send_sms_skebby = function(input,cb,cb_err){
    var text = input.text;
    var sender_number = input.sender_number || "";
    var sender_string = input.sender_string || "";
	var method = input.method;
    var lrecipients = input.recipients || [];
    var username = input.username;
    var password = input.password;
    
	if(!method){
		cb_err("No Method!");
		return;
    }

	switch(method) {
        case 'classic':
            method='send_sms_classic';
            break;
        case 'report':
            method='send_sms_classic_report';
            break;
        case 'basic':
        default:
            method='send_sms_basic';
    }
    
	var test = input.test || false;
    
    // Check params
    if(lrecipients.length == 0){
		cb_err("No recipient!");
		return;
    }

    if(!sender_string && !sender_number){
		cb_err("No sender!");
		return;
    }

    if(!text){
		cb_err("No text!");
		return;
    }    
    
    var params = {
		method : method,
		username : username,
		password : password,
		"recipients[]" : lrecipients,
		text : text,
		charset : "UTF-8",
    };

    if(sender_number){
		params.sender_number = sender_number;
    }
    else if(sender_string){
		params.sender_string = sender_string;
    }

    if(test){
		params.method = "test_"+params.method;
    }
    
    var res_done = false;
    var data = qs.stringify(params);

    var client = https.request({
    	port : 443,
    	path : "/api/send/smseasy/advanced/http.php",
    	host: "gateway.skebby.it",
    	method : "POST", 
    	headers: { 
    	    "Content-Type" : "application/x-www-form-urlencoded",
    	    "Content-Length": data.length,
    	    "Content-Encoding" : "utf8",
    	}
    },function(res){
    	var res_data = "";
    	res.on('data', function(data) {
    	    res_data+=data;
    	});
    	res.on("end",function(){
    	    if (!res_done){
		var res_parsed = qs.parse(res_data);
		if(res_parsed.status == "success"){
		    cb({data:res_parsed});
		}
		else{
			// ------------------------------------------------------------------
			// Check the complete documentation at http://www.skebby.com/business/index/send-docs/
			// ------------------------------------------------------------------
			// For eventual errors see http:#www.skebby.com/business/index/send-docs/#errorCodesSection
			// WARNING: in case of error DON'T retry the sending, since they are blocking errors
			// ------------------------------------------------------------------		
		    cb_err(res_parsed);
		}
    		res_done = true;
    	    }
    	});
    });	

    client.end(data);
    client.on('error', function(e) {
    	if (!res_done){
    	    cb_err(e);
    	    res_done = true;
    	}
    });
};

// SMS CLASSIC dispatch
send_sms_skebby( {
	method : "classic",
	username : "username",
	password : "password", 
    recipients : ["393396803445"],
    //recipients : ["393396803445","393395352490"],
    text : "Hi Mike, how are you?2",
},function(res){
    console.log(res.data);
},function(err){
    console.log(err);
});

/*
// SMS Basic dispatch
send_sms_skebby( {
	method : "basic",
	username : "username",
	password : "password", 
    recipients : ["393396803445"],
    //recipients : ["393396803445","393395352490"],
    text : "Hi Mike, how are you? By John",
},function(res){
    console.log(res.data);
},function(err){
    console.log(err);
});

// SMS CLASSIC dispatch with custom numeric sender
send_sms_skebby( {
	method : "classic",
	username : "username",
	password : "password",
    sender_number : "393471234567",
    recipients : ["393396803445"],
    //recipients : ["393396803445","393395352490"],
    text : "Hi Mike, how are you?",
},function(res){
    console.log(res.data);
},function(err){
    console.log(err);
});

// SMS CLASSIC PLUS dispatch (with delivery report) with custom alphanumeric sender
send_sms_skebby( {
	method : "report",
	username : "username",
	password : "password",
    sender_string : "John",   
    recipients : ["393396803445"],
    //recipients : ["393396803445","393395352490"],
    text : "Hi Mike, how are you?",
},function(res){
    console.log(res.data);
},function(err){
    console.log(err);
});

// SMS CLASSIC PLUS dispatch (with delivery report) with custom numeric sender
send_sms_skebby( {
	method : "report",
	username : "username",
	password : "password",
    sender_number : "393471234567",  
    recipients : ["393396803445"],
    //recipients : ["393396803445","393395352490"],
    text : "Hi Mike, how are you?",
},function(res){
    console.log(res.data);
},function(err){
    console.log(err);
});
*/					
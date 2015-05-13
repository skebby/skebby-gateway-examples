
#!/usr/bin/perl

use LWP::UserAgent;
use URI::Escape;
use Switch;

use constant NET_ERROR => "Network+error,+unable+to+send+the+message";
use constant SENDER_ERROR => "You+can+specify+only+one+type+of+sender,+numeric+or+alphanumeric";

sub URLEncode {
	my $theURL = $_[0];
	$theURL =~ s/([\W])/"%" . uc(sprintf("%2.2x",ord($1)))/eg;
	return $theURL;
}

sub URLDecode {
	my $theURL = $_[0];
	$theURL =~ tr/+/ /;
	$theURL =~ s/%([a-fA-F0-9]{2,2})/chr(hex($1))/eg;
	$theURL =~ s/<!-(.|\n)*->//g;
	return $theURL;
}

sub skebbyGatewaySendSMS {

	my ($username, $password, $recipients, $text, $sms_type, $sender_number, $sender_string, $charset) = @_;

	$ua = new LWP::UserAgent;
	$req = new HTTP::Request "POST","http://gateway.skebby.it/api/send/smseasy/advanced/http.php";
	$req->content_type("application/x-www-form-urlencoded");

	$method = "";

	switch($sms_type) {
		case "classic" { $method="send_sms_classic"; }
		case "report" { $method="send_sms_classic_report"; }
		else { $method="send_sms_basic"; }
	}

	$query = "method=".$method."&username=".URLEncode($username)."&password=".URLEncode($password).$recipients."&text=".URLEncode($text);

	if($sender_number ne "" && $sender_string ne "") {
		%results = ();
		$results{"status"} = "failed";
		$results{"code"} = "0";
		$results{"message"} = URLDecode(SENDER_ERROR);
		return %results;
	}

	$query = $query.($sender_number ne "" ? "&sender_number=".URLEncode($sender_number) : "");
	$query = $query.($sender_string ne "" ? "&sender_string=".URLEncode($sender_string) : "");

	switch($charset) {
		case "UTF-8" { $query = $query."&charset=".URLEncode("UTF-8"); }
		else {}
	}

	$req->content($query);

	$res = $ua->request($req);

	if ($res->is_error) {
		%results = ();
		$results{"status"} = "failed";
		$results{"code"} = "0";
		$results{"message"} = URLDecode(NET_ERROR);
		return %results;
	}

	%results = ();
	@result = split("&", $res->content);
	foreach (@result) {
		@temp = split("=",$_);
		$results{$temp[0]} = $temp[1];
	}

	return %results;
}

# Single dispatch
@recipients = ("393471234567");

# Multiple dispatch
# @recipients = ("393471234567","393337654321");

$recipients = "&recipients[]=".join("&recipients[]=", @recipients);

# SMS CLASSIC dispatch with custom alphanumeric sender
%results = skebbyGatewaySendSMS("username","password",$recipients,"Hi Mike, how are you?","classic","","John","");

#  SMS BASIC dispatch
# %results = skebbyGatewaySendSMS("username","password",$recipients,"Hi Mike, how are you? By John","basic","","","");

# SMS CLASSIC dispatch with custom numeric sender
# %results = skebbyGatewaySendSMS("username","password",$recipients,"Hi Mike, how are you?","classic","393471234567","","");

# SMS CLASSIC PLUS dispatch (with delivery report) with custom alphanumeric sender
# %results = skebbyGatewaySendSMS("username","password",$recipients,"Hi Mike, how are you?","report","","John","");

# SMS CLASSIC PLUS dispatch (with delivery report) with custom numeric sender
# %results = skebbyGatewaySendSMS("username","password",$recipients,"Hi Mike, how are you?","report","393471234567","","");

if ($results{"status"} eq "success") {
	print "Message Sent!\n";
	print "Remaining SMS: ".$results{"remaining_sms"};
}

# ------------------------------------------------------------------
# Check the complete documentation at http://www.skebby.com/business/index/send-docs/
# ------------------------------------------------------------------
# For eventual errors see http:#www.skebby.com/business/index/send-docs/#errorCodesSection
# WARNING: in case of error DON'T retry the sending, since they are blocking errors
# ------------------------------------------------------------------
if ($results{"status"} eq "failed") {
	print "Sending Failed\n";
	print "Code: ".$results{'code'}."\n";
	print "Description: ".URLDecode($results{'message'})."\n";
}					
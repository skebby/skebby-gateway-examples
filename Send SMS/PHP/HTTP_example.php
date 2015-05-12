<?php
define("NET_ERROR", "Network+error,+unable+to+send+the+message");
define("SENDER_ERROR", "You+can+specify+only+one+type+of+sender,+numeric+or+alphanumeric");
 
define ("SMS_TYPE_CLASSIC", "classic");
define ("SMS_TYPE_CLASSIC_PLUS", "classic_plus");
define ("SMS_TYPE_BASIC", "basic");
define ("SMS_TYPE_TEST_CLASSIC", "test_classic");
define ("SMS_TYPE_TEST_CLASSIC_PLUS", "test_classic_plus");
define ("SMS_TYPE_TEST_BASIC", "test_basic");
 
function do_post_request($url, $data, $optional_headers = null){
    if(!function_exists('curl_init')) {
        $params = array(
            'http' => array(
                'method' => 'POST',
                'content' => $data
            )
        );
        if ($optional_headers !== null) {
            $params['http']['header'] = $optional_headers;
        }
        $ctx = stream_context_create($params);
        $fp = @fopen($url, 'rb', false, $ctx);
        if (!$fp) {
            return 'status=failed&message='.NET_ERROR;
        }
        $response = @stream_get_contents($fp);
        if ($response === false) {
            return 'status=failed&message='.NET_ERROR;
        }
        return $response;
    } else {
        $ch = curl_init();
        curl_setopt($ch,CURLOPT_CONNECTTIMEOUT,10);
        curl_setopt($ch,CURLOPT_RETURNTRANSFER,true);
        curl_setopt($ch,CURLOPT_TIMEOUT,60);
        curl_setopt($ch,CURLOPT_USERAGENT,'Generic Client');
        curl_setopt($ch,CURLOPT_POSTFIELDS,$data);
        curl_setopt($ch,CURLOPT_URL,$url);
 
        if ($optional_headers !== null) {
            curl_setopt($ch,CURLOPT_HTTPHEADER,$optional_headers);
        }
 
        $response = curl_exec($ch);
        curl_close($ch);
        if(!$response){
            return 'status=failed&message='.NET_ERROR;
        }
        return $response;
    }
}
 
function skebbyGatewaySendSMS($username,$password,$recipients,$text,$sms_type=SMS_TYPE_CLASSIC,$sender_number='',$sender_string='',$user_reference='',$charset='',$optional_headers=null) {
    $url = 'http://gateway.skebby.it/api/send/smseasy/advanced/http.php';
 
    if (!is_array($recipients)){
        $recipients = array($recipients);
    }
 
    switch($sms_type) {
        case SMS_TYPE_CLASSIC:
        default:
            $method='send_sms_classic';
            break;
        case SMS_TYPE_CLASSIC_PLUS:
            $method='send_sms_classic_report';
            break;
        case SMS_TYPE_BASIC:
            $method='send_sms_basic';
            break;
        case SMS_TYPE_TEST_CLASSIC:
            $method='test_send_sms_classic';
            break;
        case SMS_TYPE_TEST_CLASSIC_PLUS:
            $method='test_send_sms_classic_report';
            break;
        case SMS_TYPE_TEST_BASIC:
            $method='test_send_sms_basic';
            break;
   }
 
    $parameters = 'method='
                  .urlencode($method).'&'
                  .'username='
                  .urlencode($username).'&'
                  .'password='
                  .urlencode($password).'&'
                  .'text='
                  .urlencode($text).'&'
                  .'recipients[]='.implode('&recipients[]=',$recipients)
                  ;
                   
    if($sender_number != '' && $sender_string != '') {
        parse_str('status=failed&message='.SENDER_ERROR,$result);
        return $result;
    }
    $parameters .= $sender_number != '' ? '&sender_number='.urlencode($sender_number) : '';
    $parameters .= $sender_string != '' ? '&sender_string='.urlencode($sender_string) : '';
 
    $parameters .= $user_reference != '' ? '&user_reference='.urlencode($user_reference) : '';
 
     
    switch($charset) {
        case 'UTF-8':
            $parameters .= '&charset='.urlencode('UTF-8');
            break;
        case '':
        case 'ISO-8859-1':
        default:
            break;
    }
     
    parse_str(do_post_request($url,$parameters,$optional_headers),$result);
 
    return $result;
}
 
function skebbyGatewayGetCredit($username,$password,$charset=''){
    $url = "http://gateway.skebby.it/api/send/smseasy/advanced/http.php";
    $method = "get_credit";
     
    $parameters = 'method='
                .urlencode($method).'&'
                .'username='
                .urlencode($username).'&'
                .'password='
                .urlencode($password);
                 
    switch($charset) {
        case 'UTF-8':
            $parameters .= '&charset='.urlencode('UTF-8');
            break;
        default:
    }
     
    parse_str(do_post_request($url,$parameters),$result);
    return $result;
}
 
// Single dispatch
$recipients = array('393471234567');
 
// Multiple dispatch
// $recipients = array('393471234567','393497654321');
 
 
// ------------ SMS Classic dispatch --------------
 
// SMS CLASSIC dispatch with custom alphanumeric sender
 $result = skebbyGatewaySendSMS('username','password',$recipients,'Hi Mike, how are you?', SMS_TYPE_CLASSIC,'','John');
 
// SMS CLASSIC dispatch with custom numeric sender
// $result = skebbyGatewaySendSMS('username','password',$recipients,'Hi Mike, how are you?', SMS_TYPE_CLASSIC,'393471234567');
 
 
// ------------- SMS Basic dispatch ----------------
// $result = skebbyGatewaySendSMS('username','password',$recipients,'Hi Mike, how are you? By John', SMS_TYPE_BASIC);
 
 
// ------------ SMS Classic Plus dispatch -----------
 
// SMS CLASSIC PLUS dispatch (with delivery report) with custom alphanumeric sender
// $result = skebbyGatewaySendSMS('username','password',$recipients,'Hi Mike, how are you?', SMS_TYPE_CLASSIC_PLUS,'','John');
 
// SMS CLASSIC PLUS dispatch (with delivery report) with custom numeric sender
// $result = skebbyGatewaySendSMS('username','password',$recipients,'Hi Mike, how are you?', SMS_TYPE_CLASSIC_PLUS,'393471234567');
 
// SMS CLASSIC PLUS dispatch (with delivery report) with custom alphanumeric sender and custom reference string
// $result = skebbyGatewaySendSMS('username','password',$recipients,'Hi Mike, how are you?', SMS_TYPE_CLASSIC_PLUS,'393471234567','','reference');
 
 
 
 
//  ------------------------------------------------------------------
//     WARNING! THE SMS_TYPE_TEST* SMS TYPES DOESN'T DISPATCH ANY SMS 
//     USE THEM ONLY TO CHECK IF YOU CAN REACH THE SKEBBY SERVER 
//  ------------------------------------------------------------------
 
// ------------- SMS Classic test dispatch ---------
// SMS CLASSIC test dispatch with custom alphanumeric sender
// $result = skebbyGatewaySendSMS('username','password',$recipients,'Hi Mike, how are you?', SMS_TYPE_TEST_CLASSIC,'','John');
 
// SMS CLASSIC test dispatch with custom numeric sender
// $result = skebbyGatewaySendSMS('username','password',$recipients,'Hi Mike, how are you?', SMS_TYPE_TEST_CLASSIC,'393471234567');
 
// ------------ SMS Classic Plus test dispatch -----------
 
// SMS CLASSIC PLUS test dispatch (with delivery report) with custom alphanumeric sender
// $result = skebbyGatewaySendSMS('username','password',$recipients,'Hi Mike, how are you?', SMS_TYPE_TEST_CLASSIC_PLUS,'','John');
 
// SMS CLASSIC PLUS test dispatch (with delivery report) with custom numeric sender
// $result = skebbyGatewaySendSMS('username','password',$recipients,'Hi Mike, how are you?', SMS_TYPE_TEST_CLASSIC_PLUS,'393471234567');
 
// ------------- SMS Basic test dispatch ----------------
// $result = skebbyGatewaySendSMS('username','password',$recipients,'Hi Mike, how are you? By John', SMS_TYPE_TEST_BASIC);
 
// ------------------------------------------------------------------
//    WARNING! THE SMS_TYPE_TEST* SMS TYPES DOESN'T DISPATCH ANY SMS
//    USE THEM ONLY TO CHECK IF YOU CAN REACH THE SKEBBY SERVER 
// ------------------------------------------------------------------
 
if($result['status']=='success') {
    echo '<b style="color:#8dc63f;">Message Sent!</b><br/>';
    if (isset($result['remaining_sms'])){
        echo '<b>Remaining SMS:</b> '.$result['remaining_sms'];
    }
    if (isset($result['id'])){
        echo '<b>ID:</b> '.$result['id'];
    }
}
 
// ------------------------------------------------------------------
// Check the complete documentation at http://www.skebby.com/business/index/send-docs/
// ------------------------------------------------------------------
// For eventual errors see http:#www.skebby.com/business/index/send-docs/#errorCodesSection
// WARNING: in case of error DON'T retry the sending, since they are blocking errors
// ------------------------------------------------------------------
if($result['status']=='failed') {
    echo '<b style="color:#ed1c24;">Sending failed</b><br/>';
    if(isset($result['code'])) {
        echo '<b>Code:</b> '.$result['code'].'<br/>';
    }
        echo '<b>Description:</b> '.urldecode($result['message']);
}

// ------------ REMAINING CREDIT Check -------------
// $credit_result = skebbyGatewayGetCredit('username', 'password');
 
 
//if($credit_result['status']=='success') {
//  echo 'Current Balance: ' .$credit_result['credit_left']."\n";
//  echo 'Remaining SMS Classic: ' .$credit_result['classic_sms']."\n";
//  echo 'Remaining SMS Basic: ' .$credit_result['basic_sms']."\n";
//}
 
//if($credit_result['status']=='failed') {
//  echo 'Sending request failed';
//}
?> 
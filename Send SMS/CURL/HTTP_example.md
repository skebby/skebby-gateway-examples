# CURL Examples

## SMS Classic dispatch
 
### SMS CLASSIC dispatch with custom alphanumeric sender (single dispatch)
```curl
curl --data "method=send_sms_classic&username=username&password=password&recipients[]=393471234567&text=Hi+Mike%2C+how+are+you%3F&sender_string=John" http://gateway.skebby.it/api/send/smseasy/advanced/http.php
```

### SMS CLASSIC dispatch with custom numeric sender (multiple dispatch)
```curl
curl --data "method=send_sms_classic&username=username&password=password&recipients[]=393471234567&recipients[]=393477654321&text=Hi+Mike%2C+how+are+you%3F&sender_string=John" http://gateway.skebby.it/api/send/smseasy/advanced/http.php
```
 
### SMS CLASSIC dispatch with custom numeric sender
```curl
curl --data "method=send_sms_classic&username=username&password=password&recipients[]=393471234567&text=Hi+Mike%2C+how+are+you%3F&sender_number=393471234567" http://gateway.skebby.it/api/send/smseasy/advanced/http.php
```
 
## SMS Basic dispatch
```curl
curl --data "method=send_sms_basic&username=username&password=password&recipients[]=393471234567&text=Hi+Mike%2C+how+are+you" http://gateway.skebby.it/api/send/smseasy/advanced/http.php
```
 
## SMS Classic Plus dispatch
 
### SMS CLASSIC PLUS dispatch (with delivery report) with custom alphanumeric sender
```curl
curl --data "method=send_sms_classic_report&username=username&password=password&recipients[]=393471234567&text=Hi+Mike%2C+how+are+you%3F&sender_string=John" http://gateway.skebby.it/api/send/smseasy/advanced/http.php
```
 
### SMS CLASSIC PLUS dispatch (with delivery report) with custom numeric sender
```curl
curl --data "method=send_sms_classic_report&username=username&password=password&recipients[]=393471234567&text=Hi+Mike%2C+how+are+you%3F&sender_number=393471234567" http://gateway.skebby.it/api/send/smseasy/advanced/http.php
```
 
### SMS CLASSIC PLUS dispatch (with delivery report) with custom alphanumeric sender and custom reference string
```curl
curl --data "method=send_sms_classic_report&username=username&password=password&recipients[]=393471234567&text=Hi+Mike%2C+how+are+you%3F&sender_number=393471234567&user_reference=riferimento" http://gateway.skebby.it/api/send/smseasy/advanced/http.php
```

> _WARNING!_ THE "SMS_TYPE_TEST" SMS TYPES DOESN'T DISPATCH ANY SMS
> USE THEM ONLY TO CHECK IF YOU CAN REACH THE SKEBBY SERVER 
 
## SMS Classic test dispatch

### SMS CLASSIC test dispatch with custom alphanumeric sender
```curl
curl --data "method=test_send_sms_classic&username=username&password=password&recipients[]=393471234567&text=Hi+Mike%2C+how+are+you%3F&sender_string=John" http://gateway.skebby.it/api/send/smseasy/advanced/http.php
```

# SMS CLASSIC test dispatch with custom numeric sender
```curl
curl --data "method=test_send_sms_classic&username=username&password=password&recipients[]=393471234567&text=Hi+Mike%2C+how+are+you%3F&sender_number=393471234567" http://gateway.skebby.it/api/send/smseasy/advanced/http.php
```
 
## SMS Classic Plus test dispatch
 
### SMS CLASSIC PLUS test dispatch (with delivery report) with custom alphanumeric sender
```curl
curl --data "method=test_send_sms_classic_report&username=username&password=password&recipients[]=393471234567&text=Hi+Mike%2C+how+are+you%3F&sender_string=John" http://gateway.skebby.it/api/send/smseasy/advanced/http.php
```
 
### SMS CLASSIC PLUS test dispatch (with delivery report) with custom numeric sender
```curl
curl --data "method=test_send_sms_classic_report&username=username&password=password&recipients[]=393471234567&text=Hi+Mike%2C+how+are+you%3F&sender_number=393471234567" http://gateway.skebby.it/api/send/smseasy/advanced/http.php
```
 
## SMS Basic test dispatch
```curl
curl --data "method=test_send_sms_basic&username=username&password=password&recipients[]=393471234567&text=Hi+Mike%2C+how+are+you" http://gateway.skebby.it/api/send/smseasy/advanced/http.php
```
 
## REMAINING CREDIT Check
```curl
curl --data "method=get_credit&username=username&password=password" http://gateway.skebby.it/api/send/smseasy/advanced/http.php
```             
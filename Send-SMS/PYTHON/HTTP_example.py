
import urllib
import urllib2

NET_ERROR = "Network error, unable to send the message"
SENDER_ERROR = "You can specify only one type of sender, numeric or alphanumeric"

def UrlEncode(recipients):
        resultString = ''
        for number in recipients:
                resultString = resultString + 'recipients[]=' + urllib.quote_plus(number) + '&'
        return resultString[:-1]

def skebbyGatewaySendSMS(username,password,recipients,text,sms_type='basic',sender_number='',sender_string='',charset='ISO-8859-1',options={ 'User-Agent' : 'Generic Client' }):
        url = 'http://gateway.skebby.it/api/send/smseasy/advanced/http.php'

        method = 'send_sms_basic'

        if sms_type=='classic' : method = 'send_sms_classic'
        if sms_type=='report' : method = 'send_sms_classic_report'

        parameters = {
                'method' : method,
                'username' : username,
                'password' : password,
                'text' : text
        }

        if sender_number != '' and sender_string != '' :
                result = {}
                result['status'] = 'failed'
                result['message'] = SENDER_ERROR
                return result

        if sender_number != '' : parameters['sender_number'] = sender_number
        if sender_string != '' : parameters['sender_string'] = sender_string

        if charset != 'ISO-8859-1' : parameters['charset'] = 'UTF-8'

        headers = options
        data = urllib.urlencode(parameters) + '&' + UrlEncode(recipients)

        req = urllib2.Request(url, data, headers)
        try:
                response = urllib2.urlopen(req)
        except urllib2.HTTPError as e:
                result = {}
                result['status'] = 'failed'
                result['code'] = e.code
                result['message'] = NET_ERROR
                return result
        except urllib2.URLError as e:
                result = {}
                result['status'] = 'failed'
                result['message'] = e.reason
                return result

        resultString = response.read()

        results = resultString.split('&')
        result = {}
        for r in results:
                temp = r.split('=')
                result[temp[0]] = temp[1]

        return result

# Single dispatch
recipients = ["393471234567"]

# Multiple dispatch
# recipients = ["393331234567","393331234567"]

# SMS CLASSIC dispatch with custom alphanumeric sender
result = skebbyGatewaySendSMS('username','password',recipients,'Hi Mike, how are you?','classic','','John')

# SMS CLASSIC dispatch with custom numeric sender
# result = skebbyGatewaySendSMS('username','password',recipients,'Hi Mike, how are you?','classic','393471234567')

# SMS Basic dispatch
# result = skebbyGatewaySendSMS('username','password',recipients,'Hi Mike, how are you? By John')

# SMS CLASSIC PLUS dispatch (with delivery report) with custom alphanumeric sender
# result = skebbyGatewaySendSMS('username','password',recipients,'Hi Mike, how are you?','report','','John')

# SMS CLASSIC PLUS dispatch (with delivery report) with custom numeric sender
# result = skebbyGatewaySendSMS('username','password',recipients,'Hi Mike, how are you?','report','393471234567')
if(result['status']=='success'):
        print "Message sent!\n"
        print "Remaining SMS: " + result['remaining_sms']

# ------------------------------------------------------------------
# Controlla la documentazione completa all'indirizzo http:#www.skebby.it/business/index/send-docs/ 
# ------------------------------------------------------------------
# Per i possibili errori si veda http:#www.skebby.it/business/index/send-docs/#errorCodesSection
# ATTENZIONE: in caso di errore Non si deve riprovare l'invio, trattandosi di errori bloccanti
# ------------------------------------------------------------------
if(result['status']=='failed'):
        print "Sending failed\n"
        try: print "Code: " + str(result['code']) + "\n"
        except KeyError: pass
        print "Description: " + str(result['message']) + "\n"
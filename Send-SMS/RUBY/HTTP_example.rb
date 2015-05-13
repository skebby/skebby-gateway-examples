
require 'net/http'
require 'uri'
require 'cgi'

class SkebbyGatewaySendSMS
  
  def initialize(username = '', password = '')
    @url = 'http://gateway.skebby.it/api/send/smseasy/advanced/http.php'

    @parameters = {
      'username'    => username,
      'password'    => password,
    }
  end

  def sendSMS(method, text, recipients, options = {})
    unless recipients.kind_of?(Array)
      raise("recipients must be an array")
    end
  
    @parameters['method'] = method
    @parameters['text'] = text
    
    @parameters["recipients[]"] = recipients
  
    unless options[:senderNumber].nil?
     @parameters['sender_number'] = options[:senderNumber]
    end

    unless options[:senderString].nil?
     @parameters['sender_string'] = options[:senderString]
    end

    unless options[:charset].nil?
     @parameters['charset'] = options[:charset]
    end
        
    @parameters.each {|key, value| puts "#{key} is #{value}" }    
        
    @response = Net::HTTP.post_form(URI(@url), @parameters)
    if @response.message == "OK"
      true
    else
      false
    end
    
  end
  
  def getCredit()
    
    @parameters['method'] = 'get_credit'
    
    @response = Net::HTTP.post_form(URI(@url), @parameters)
    if @response.message == "OK"
      true
    else
      false
    end
  end
  
  def getResponse
    result = {}
    @response.body.split('&').each do |res|
      if res != ''
        temp = res.split('=')
        if temp.size > 1
          result[temp[0]] = temp[1]
        end
      end
    end
    return result
  end

  def printResponse
    result = self.getResponse
    if result.has_key?('status') and result['status'] == 'success'
      puts "Success, response contains:"
      result.each do |key,value|
        puts "\t#{key} => #{CGI::unescape(value)}"
      end
      true
    else
      # ------------------------------------------------------------------
      # Check the complete documentation at http:#www.skebby.com/business/index/send-docs/
      # ------------------------------------------------------------------
      # Per i possibili errori si veda http:#www.skebby.com/business/index/send-docs/#errorCodesSection
      # ATTENZIONE: in caso di errore Non si deve riprovare l'invio, trattandosi di errori bloccanti
      # ------------------------------------------------------------------    
      puts "Error, trace is:"
      result.each do |key,value|
        puts "\t#{key} => #{CGI::unescape(value)}"
      end
      false
    end
  end

end

gw = SkebbyGatewaySendSMS.new('username', 'password')

#Single dispatch
recipients = ["393471234567"]

#Multiple dispatch
#recipients = ["393471234567","393497654321"]

#SMS CLASSIC dispatch with custom alphanumeric sender
result = gw.sendSMS('send_sms_classic', 'Hi Mike, how are you', recipients, { :senderString => 'Jhon' } )

#SMS CLASSIC dispatch with custom numeric sender
#result = gw.sendSMS('send_sms_classic', 'Hi Mike, how are you', recipients, { :senderNumber => '393471234567' } )

#SMS Basic dispatch
#result = gw.sendSMS('send_sms_basic', 'Hi Mike, how are you? By John', recipients )

#SMS CLASSIC PLUS dispatch (with delivery report) with custom alphanumeric sender
#result = gw.sendSMS('send_sms_classic_report', 'Hi Mike, how are you', recipients, { :senderString => 'Jhon' } )

#SMS CLASSIC PLUS dispatch (with delivery report) with custom numeric sender
#result = gw.sendSMS('send_sms_classic_report', 'Hi Mike, how are you', recipients, { :senderNumber => '393471234567' } )

#Remaining Credit Check
#result = gw.getCredit()

if result
  gw.printResponse
else
  puts "Error in the HTTP request"
end         
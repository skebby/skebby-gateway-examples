/*
SKEBBYGATEWAYAPI C++ EXAMPLE
TOTI PIERANDREA
SKEBBY
MOBILE SOLUTIONS S.R.L.

WARNING: THIS EXAMPLE USES CROSSPLATFORM LIBRARY "LIBCURL": YOU CAN DOWNLOAD IT AT http://curl.haxx.se/
*/

#include <iostream>
#include <string>
#include <vector>
#include "curl\curl.h"

using namespace std;

#define SMS_TYPE_CLASSIC 0
#define SMS_TYPE_CLASSIC_PLUS  1
#define SMS_TYPE_BASIC   2
#define SMS_TYPE_TEST_CLASSIC  3
#define SMS_TYPE_TEST_CLASSIC_PLUS  4
#define SMS_TYPE_TEST_BASIC  5

#define utf8(str)  ConvertToUTF8(L##str)

#ifdef _WIN32

const string ConvertToUTF8(const wchar_t * pStr)
{
	static char szBuf[1024];
	WideCharToMultiByte(CP_UTF8, 0, pStr, -1, szBuf, sizeof(szBuf), NULL, NULL);
	string ret(szBuf);
	return ret;
}

#else

const string ConvertToUTF8(const wstring& src)
{
	string ret = "";
	for (size_t i = 0; i < src.size(); i++){
		wchar_t w = src[i];
		if (w <= 0x7f)
			ret.push_back((char)w);
		else if (w <= 0x7ff)
		{
			ret.push_back(0xc0 | ((w >> 6)& 0x1f));
			ret.push_back(0x80| (w & 0x3f));
		}
		else if (w <= 0xffff)
		{
			ret.push_back(0xe0 | ((w >> 12)& 0x0f));
			ret.push_back(0x80| ((w >> 6) & 0x3f));
			ret.push_back(0x80| (w & 0x3f));
		}
		else if (w <= 0x10ffff)
		{
			ret.push_back(0xf0 | ((w >> 18)& 0x07));
			ret.push_back(0x80| ((w >> 12) & 0x3f));
			ret.push_back(0x80| ((w >> 6) & 0x3f));
			ret.push_back(0x80| (w & 0x3f));
		}
		else
			ret.push_back('?');
	}
	return ret;
}

#endif

size_t write_to_string(void *ptr, size_t size, size_t count, void *stream)
{
	((string*)stream)->append((char*)ptr, 0, size*count);
	return size*count;
}

static string skebbyGatewaySendSMS(const string &username, const string &password,
	vector<string> &recipients,const string &text,const string &sms_type,
	const string &sender_number, const string &sender_string, const string &user_reference)
{
	CURL *curl;
	CURLcode res;

	struct curl_httppost *formpost=NULL;
	struct curl_httppost *lastptr=NULL;
	struct curl_slist *headerlist=NULL;
	static const char buf[] = "Expect:";

	curl_global_init(CURL_GLOBAL_ALL);

	curl_formadd(&formpost,
		&lastptr,
		CURLFORM_COPYNAME, "method",
		CURLFORM_COPYCONTENTS, &sms_type[0],
		CURLFORM_END);

	curl_formadd(&formpost,
		&lastptr,
		CURLFORM_COPYNAME, "username",
		CURLFORM_COPYCONTENTS, &username[0],
		CURLFORM_END);

	curl_formadd(&formpost,
		&lastptr,
		CURLFORM_COPYNAME, "password",
		CURLFORM_COPYCONTENTS, &password[0],
		CURLFORM_END);

	curl_formadd(&formpost,
		&lastptr,
		CURLFORM_COPYNAME, "charset",
		CURLFORM_COPYCONTENTS, "UTF-8",
		CURLFORM_END);

	curl_formadd(&formpost,
		&lastptr,
		CURLFORM_COPYNAME, "text",
		CURLFORM_COPYCONTENTS, &text[0],
		CURLFORM_END);

	for (vector<string>::iterator i = recipients.begin();i != recipients.end();++i)
	{
		curl_formadd(&formpost,
			&lastptr,
			CURLFORM_COPYNAME, "recipients[]",
			CURLFORM_COPYCONTENTS, &(*i)[0],
			CURLFORM_END);
	}

	if(sender_number.length() > 0 )
	{
		curl_formadd(&formpost,
			&lastptr,
			CURLFORM_COPYNAME, "sender_number",
			CURLFORM_COPYCONTENTS, &sender_number[0],
			CURLFORM_END);
	}

	if(sender_string.length() > 0 )
	{
		curl_formadd(&formpost,
			&lastptr,
			CURLFORM_COPYNAME, "sender_string",
			CURLFORM_COPYCONTENTS, &sender_string[0],
			CURLFORM_END);
	}

	if(user_reference.length() > 0 )
	{
		curl_formadd(&formpost,
			&lastptr,
			CURLFORM_COPYNAME, "user_reference",
			CURLFORM_COPYCONTENTS, &user_reference[0],
			CURLFORM_END);
	}


	headerlist = curl_slist_append(headerlist, buf);
	curl = curl_easy_init();

	if(curl)
	{

		string response;
		curl_easy_setopt(curl, CURLOPT_URL, "http://gateway.skebby.it/api/send/smseasy/advanced/http.php");
		curl_easy_setopt(curl, CURLOPT_HTTPPOST, formpost);
		curl_easy_setopt(curl, CURLOPT_HTTPHEADER, headerlist);
		curl_easy_setopt(curl, CURLOPT_TIMEOUT, 100);
		curl_easy_setopt(curl, CURLOPT_ENCODING, "UTF-8" );
		curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, write_to_string);
		curl_easy_setopt(curl, CURLOPT_WRITEDATA, &response);
		res = curl_easy_perform(curl);

		// always cleanup
		curl_easy_cleanup(curl);
		curl_formfree(formpost);
		curl_slist_free_all (headerlist);

		if(res == CURLE_OK )
			return response;
	}
	return "Error Occured";
}


static string skebbyGatewayGetCredit(const string &username,const string &password)
{
	CURL *curl;
	CURLcode res;

	struct curl_httppost *formpost=NULL;
	struct curl_httppost *lastptr=NULL;

	curl_global_init(CURL_GLOBAL_ALL);

	curl_formadd(&formpost,
		&lastptr,
		CURLFORM_COPYNAME, "method",
		CURLFORM_COPYCONTENTS, "get_credit",
		CURLFORM_END);


	curl_formadd(&formpost,
		&lastptr,
		CURLFORM_COPYNAME, "username",
		CURLFORM_COPYCONTENTS, &username[0],
		CURLFORM_END);

	curl_formadd(&formpost,
		&lastptr,
		CURLFORM_COPYNAME, "password",
		CURLFORM_COPYCONTENTS, &password[0],
		CURLFORM_END);

	curl = curl_easy_init();

	if(curl)
	{
		string response;
		curl_easy_setopt(curl, CURLOPT_URL, "http://gateway.skebby.it/api/send/smseasy/advanced/http.php");
		curl_easy_setopt(curl, CURLOPT_HTTPPOST, formpost);
		curl_easy_setopt(curl, CURLOPT_TIMEOUT, 100);
		curl_easy_setopt(curl, CURLOPT_ENCODING, "UTF-8" );
		curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, write_to_string);
		curl_easy_setopt(curl, CURLOPT_WRITEDATA, &response);
		res = curl_easy_perform(curl);
		curl_easy_cleanup(curl);
		curl_formfree(formpost);

		if(res == CURLE_OK )
			return response;
	}

	return "Error Occured";
}

int main()
{

	string username = utf8("username");
	string password = utf8("password");
	vector<string> recipients;

	// Invio singolo
	recipients.push_back("393471234567");	
	
	// Per invio multiplo
	// recipients.push_back("393471234567");
	// recipients.push_back("393477654321");

	string smstype[6] = {"send_sms_classic","send_sms_classic_report","send_sms_basic","test_send_sms_classic","test_send_sms_classic_report","test_send_sms_basic"} ;
	string message = utf8("Hi Mike, how are you?");
	string result;

	// ------------------------------------------------------------------
	// Check the complete documentation at http://www.skebby.com/business/index/send-docs/
	// ------------------------------------------------------------------
	// For eventual errors see http:#www.skebby.com/business/index/send-docs/#errorCodesSection
	// WARNING: in case of error DON'T retry the sending, since they are blocking errors
	// ------------------------------------------------------------------
	
	// ------------ SMS Classic dispatch --------------

	// SMS CLASSIC dispatch with custom alphanumeric sender
	 result = skebbyGatewaySendSMS(username, password, recipients, message, smstype[SMS_TYPE_CLASSIC], "393471234567", "John", "");

	// SMS CLASSIC dispatch with custom numeric sender
    // result = skebbyGatewaySendSMS(username, password, recipients, message, smstype[SMS_TYPE_CLASSIC], "393471234567", "", "");

	// ------------- SMS Basic dispatch ----------------
    // result = skebbyGatewaySendSMS(username, password, recipients, message, smstype[SMS_TYPE_BASIC], "", "", "");

	// ------------ SMS Classic Plus dispatch -----------

    // SMS CLASSIC PLUS dispatch (with delivery report) with custom alphanumeric sender
    // result = skebbyGatewaySendSMS(username, password, recipients, message, smstype[SMS_TYPE_CLASSIC_PLUS], "", "John", "");

    // SMS CLASSIC PLUS dispatch (with delivery report) with custom numeric sender
	// result = skebbyGatewaySendSMS(username, password, recipients, message, smstype[SMS_TYPE_CLASSIC_PLUS], "393471234567", "", "");

	// SMS CLASSIC PLUS dispatch (with delivery report) with custom alphanumeric sender and custom reference string
	// result = skebbyGatewaySendSMS(username, password, recipients, message, smstype[SMS_TYPE_CLASSIC_PLUS], "393471234567", "", "riferimento");


    //  ------------------------------------------------------------------
	//     WARNING! THE SMS_TYPE_TEST* SMS TYPES DOESN'T DISPATCH ANY SMS 
	//     USE THEM ONLY TO CHECK IF YOU CAN REACH THE SKEBBY SERVER 
	//  ------------------------------------------------------------------

    // ------------- SMS Classic test dispatch ---------

    // SMS CLASSIC test dispatch with custom alphanumeric sender
	// result = skebbyGatewaySendSMS(username, password, recipients, message, smstype[SMS_TYPE_TEST_CLASSIC], "", "John", "");

    // SMS CLASSIC test dispatch with custom numeric sender
	// result = skebbyGatewaySendSMS(username, password, recipients, message, smstype[SMS_TYPE_TEST_CLASSIC], "393471234567", "", "");


    // ------------ SMS Classic Plus test dispatch -----------

    // SMS CLASSIC PLUS test dispatch (with delivery report) with custom alphanumeric sender
    // result = skebbyGatewaySendSMS(username, password, recipients, message, smstype[SMS_TYPE_TEST_CLASSIC_PLUS], "", "John", "");

    // SMS CLASSIC PLUS test dispatch (with delivery report) with custom numeric sender
    // result = skebbyGatewaySendSMS(username, password, recipients, message, smstype[SMS_TYPE_TEST_CLASSIC_PLUS], "393471234567", "", "");

	
    // ------------- SMS Basic test dispatch ----------------
    // result = skebbyGatewaySendSMS(username, password, recipients, message, smstype[SMS_TYPE_TEST_BASIC], "", "", "");

	// ------------ REMAINING CREDIT Check -------------
	// result = skebbyGatewayGetCredit(username,password);

	cout << result << endl;
	return 0;
}					
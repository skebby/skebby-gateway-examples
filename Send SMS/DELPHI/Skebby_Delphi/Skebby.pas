{==============================================================================|
| Project : Skebby Gateway SMS                                                 |
|==============================================================================|
| Content: Esempio Gateway in Delphi                                           |
|==============================================================================|
| L'esempio e' stato testato su Embarcadero RAD Studio XE5                     |
| Suppone la creazione di un Form TFormEsempio con un bottone SkebbySendSMS    |
| e l'integrazione delle librerie Delphi Synapse:                              |
| URL: http://www.ararat.cz/synapse/ prendere l'ultima versione da SVN         |
|==============================================================================}

unit Skebby;

interface

uses
  Winapi.Windows, Winapi.Messages, System.SysUtils, System.Variants,
System.Classes, Vcl.Graphics,
  Vcl.Controls, Vcl.Forms, Vcl.Dialogs, Vcl.StdCtrls, blcksock, synacode,
httpsend;

type
  TFormEsempio = class(TForm)
    SkebbySendSMS: TButton;
    procedure SkebbySendSMSClick(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  FormEsempio: TFormEsempio;


type TSkebbySMSType = (smsTestClassic, smsBasic, smsClassic,
smsClassicPlus);// 0=Test Classic, 1=Basic, 2=Classic, 3=Classic Plus

type TSkebbyResponse = record
  status: string;
  code: string;
  msg: string;
  remainingSMS: string;
  credit: Double;
  sms_basic_left: integer;
  sms_classic_left: integer;
end;

type TSkebbyGateway = class
  private
    FURLGateway: string;
    FUsername: string;
    FPassword: string;
    FTipoSMS: integer;
    FNumeroMittente: string;
    FNomeMittente: string;
    FRecipients: TStringList;
    FMessaggio: string;

    FParams: string;

    procedure PrepareSMSParams(smsType: TSkebbySMSType);
    procedure PrepareCreditParams;

    function ParseSMSResponse(resp: string): TSkebbyResponse;
    function ParseCreditResponse(resp: string): TSkebbyResponse;
  public

    constructor Create;
    destructor Destroy; override;

    procedure AddDestinatario(numero: string);
    procedure SetSMSText(testo: string);

    function TestServizio: TSkebbyResponse;
    function GetCredito: TSkebbyResponse;
    function SendSMS(tipo: TSkebbySMSType): TSkebbyResponse;
end;

const
  TEST_SMS_CLASSIC = 'test_send_sms_classic';
  TEST_SMS_BASIC = 'test_send_sms_basic';

  SMS_BASIC = 'send_sms_basic';
  SMS_CLASSIC = 'send_sms_classic';
  SMS_CLASSIC_PLUS = 'send_sms_classic_report';

  SMS_SUCCESS = 'success';
  SMS_FAILED = 'failed';

implementation
{$R *.dfm}


{ TSkebbyGateway }

procedure TSkebbyGateway.AddDestinatario(numero: string);
begin
  FRecipients.Add(numero);
  inherited;
end;

constructor TSkebbyGateway.Create;
begin
  FRecipients := TStringList.Create;

// VARIABILI DI CONFIGURAZIONE
// INSERIRE PROPRIE CREDENZIALI ED IMPOSTAZIONI SMS QUI
  FMessaggio := '';
  FURLGateway := 'http://gateway.skebby.it/api/send/smseasy/advanced/http.php';
  FUsername := 'username';
  FPassword := 'password';
  FTipoSMS := 2;
  FNumeroMittente := '';
  FNomeMittente := 'John';
end;

destructor TSkebbyGateway.Destroy;
begin
  FRecipients.Free;
end;


function TSkebbyGateway.GetCredito: TSkebbyResponse;
var
  chk: Boolean;
  stm: TStringStream;
begin
  stm := TStringStream.Create('');

  PrepareCreditParams;

  try
    chk := HttpPostURL(FURLGateway, FParams, stm);
    if chk then
    begin
      Result := ParseCreditResponse(stm.DataString);
    end
    else
    begin
      Result.status := SMS_FAILED;
      Result.msg := 'Errore durante il collegamento con il server SMS!';
    end;
  finally
    stm.Free;
  end;
end;

function TSkebbyGateway.ParseSMSResponse(resp: string): TSkebbyResponse;
var
  list: TStringList;
begin
  list := TStringList.Create;
  try
    list.Text := StringReplace(resp, '&', #13#10, [rfReplaceAll]);
    Result.status := list.Values['status'];
    if Result.status = SMS_SUCCESS then
    begin
      Result.msg := 'Invio SMS effettuato con successo';
      Result.remainingSMS := list.Values['remaining_sms'];
    end;

    if Result.status = SMS_FAILED then
    begin
      Result.code := list.Values['code'];
      Result.msg := list.Values['message'];
    end;
  finally
    list.Free;
  end;
end;

function TSkebbyGateway.ParseCreditResponse(resp: string): TSkebbyResponse;
var
  list: TStringList;
begin
  list := TStringList.Create;
  try
    list.Text := StringReplace(resp, '&', #13#10, [rfReplaceAll]);
    Result.status := list.Values['status'];
    if Result.status = SMS_SUCCESS then
    begin
      Result.credit := StrToFloatDef(list.Values['credit_left'], 0.0);
      Result.sms_basic_left := StrToIntDef(list.Values['basic_sms'], 0);
      Result.sms_classic_left := StrToIntDef(list.Values['classic_sms'], 0);
    end;

    if Result.status = SMS_FAILED then
    begin
      Result.code := '';
      Result.msg := 'Invio richiesta fallito!';
    end;
  finally
    list.Free;
  end;
end;

procedure TSkebbyGateway.PrepareCreditParams;
var
  urlParams: string;
begin
  FParams := '';

  urlParams := '';
  urlParams := urlParams + 'method=get_credit&';
  urlParams := urlParams + 'username=' + EncodeURLElement(FUsername) + '&';
  urlParams := urlParams + 'password=' + EncodeURLElement(FPassword) + '&';

  FParams := urlParams;
end;

procedure TSkebbyGateway.PrepareSMSParams(smsType: TSkebbySMSType);
var
  k: Integer;
  urlParams: string;
begin
  FParams := '';

  urlParams := '';

  case smsType of
    smsTestClassic:
      urlParams := urlParams + 'method=' +
EncodeURLElement(TEST_SMS_CLASSIC) + '&';
    smsBasic:
      urlParams := urlParams + 'method=' + EncodeURLElement(SMS_BASIC) +
'&';
    smsClassic:
      urlParams := urlParams + 'method=' + EncodeURLElement(SMS_CLASSIC) +
'&';
    smsClassicPlus:
      urlParams := urlParams + 'method=' +
EncodeURLElement(SMS_CLASSIC_PLUS) + '&';
  else // default usa il metodo TEST
    urlParams := urlParams + 'method=' + EncodeURLElement(TEST_SMS_CLASSIC)
+ '&';
  end;

  urlParams := urlParams + 'username=' + EncodeURLElement(FUsername) + '&';
  urlParams := urlParams + 'password=' + EncodeURLElement(FPassword) + '&';
  urlParams := urlParams + 'text=' + EncodeURLElement(FMessaggio) + '&';

  for k:=0 to FRecipients.Count - 1 do
  begin
    urlParams := urlParams + 'recipients[]=' +
EncodeURLElement(FRecipients[k]) + '&';
  end;

  if FNumeroMittente <> '' then
    urlParams := urlParams + 'sender_number=' +
EncodeURLElement(FNumeroMittente) + '&';
  if FNomeMittente <> '' then
    urlParams := urlParams + 'sender_string=' +
EncodeURLElement(FNomeMittente) + '&';


  FParams := urlParams;
end;

procedure TSkebbyGateway.SetSMSText(testo: string);
begin
  FMessaggio := testo;
end;

function TSkebbyGateway.TestServizio: TSkebbyResponse;
begin
  Result := SendSMS(smsTestClassic);
end;

function TSkebbyGateway.SendSMS(tipo: TSkebbySMSType): TSkebbyResponse;
var
  chk: Boolean;
  stm: TStringStream;
begin
  PrepareSMSParams(tipo);

  stm := TStringStream.Create('');
  try
    chk := HttpPostURL(FURLGateway, FParams, stm);
    if chk then
    begin
      Result := ParseSMSResponse(stm.DataString);
    end
    else
    begin
      Result.status := SMS_FAILED;
      Result.msg := 'Errore durante il collegamento con il server SMS!';
    end;
  finally
    stm.Free;
  end;
end;



procedure TFormEsempio.SkebbySendSMSClick(Sender: TObject);
var
  sms: TSkebbyGateway;
  resp: TSkebbyResponse;
begin
  sms := TSkebbyGateway.Create;
  try
  // Destinatari:
    sms.AddDestinatario('393491234567');
    // sms.AddDestinatario('393331234567');
    // Testo:
    sms.SetSMSText('Hi Mike, how are you?');
    // Tipo Messaggio:
    // smsBasic   = send_sms_basic
    // smsClassic = send_sms_classic
    // smsClassicPlus = send_sms_report
    // testSmsClassic = test_send_sms_classic
    resp := sms.SendSMS(smsClassic);
    if resp.status = SMS_SUCCESS then
    begin
      // resp.remainingSMS contiene il numero di SMS rimanenti
      ShowMessage('SMS inviato con successo');
    end
    else
    begin
      // resp.msg contiene il codice di errore
      ShowMessage('Invio fallito');
    end;
  finally
    sms.free;
  end;
end;

end.


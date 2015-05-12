program SkebbySmsGateway;

uses
  Vcl.Forms,
  Skebby in 'Skebby.pas' {FormEsempio};

{$R *.res}

begin
  Application.Initialize;
  Application.MainFormOnTaskbar := True;
  Application.CreateForm(TFormEsempio, FormEsempio);
  Application.Run;
end.

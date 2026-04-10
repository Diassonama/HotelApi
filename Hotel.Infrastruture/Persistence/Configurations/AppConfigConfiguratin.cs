using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class AppConfigConfiguratin : IEntityTypeConfiguration<AppConfig>
    {
        public void Configure(EntityTypeBuilder<AppConfig> builder)
        {
           builder.HasData(
            
               new AppConfig{Id = 1,Key = "smsServiceEnabled",Value = "1"},
               new AppConfig{Id = 2,Key = "emailServiceEnabled",Value = "1"},
               new AppConfig{Id = 3,Key = "emailSMTPServer",Value = "smtp.gmail.com"},
               new AppConfig{Id = 4,Key = "emailSMTPPort",Value = "587"},
               new AppConfig{Id = 5,Key = "emailUser",Value = "ghotelweb@gmail.com"},
               new AppConfig{Id = 6,Key = "emailPassword",Value = "vztxbucwohdiolnq"},
               new AppConfig{Id = 7,Key = "emailFrom",Value = "ghotelweb@gmail.com"},
               new AppConfig{Id = 8,Key = "emailBodyRegisteredBO",Value = "<p>Olá,</p><br><p>encontra-se registado no backoffice do GHOTEL.</p><p>Por favor confirme o registo clicando no seguinte <a href={0}>confirmar</a></p><br><p>Atentamente</p><p>GHOTEL</p>"},
               new AppConfig{Id = 9,Key = "emailSubjectRegisteredBO",Value = "Bem-vindo ao GHOTEL"},
               new AppConfig{Id = 10,Key = "emailConfirmUrl",Value = "http://localhost:5055/api/usuario/confirm-email?email={0}&token={1}"},
               new AppConfig{Id = 11,Key = "emailBodyBeforeChangeEmail",Value = "<p>Olá,</p><br><p>foi efetuado um pedido de mudança do seu email na plataforma GHOTEL.</p><p>Por favor confirme a alteração para o novo email:{0} clicando no seguinte <a href={1}>confirmar</a></p><br><p>Atentamente</p><p>GHOTEL</p>"},
               new AppConfig{Id = 12,Key = "emailSubjectBeforeChangeEmail",Value = "GHOTEL - Pedido de alteração de email"},
               new AppConfig{Id = 13,Key = "emailChangeUrl",Value = "https://GHOTEL-dev.northeurope.cloudapp.azure.com:447/internal_api/Account/change-email?email={0}&token={1}"},
               new AppConfig{Id = 14,Key = "emailBodyAfterChangeEmail",Value = "<p>Olá,</p><br><p>o seu email foi alterado com sucesso na plataforma GHOTEL.</p><br><p>Atentamente</p><p>GHOTEL</p>"},
               new AppConfig{Id = 15,Key = "emailSubjectAfterChangeEmail",Value = "GHOTEL - email alterado"},
               new AppConfig{Id = 16,Key = "smsBodyConfirmation",Value = "GHOTEL - Código de confirmação: {0}"},
               new AppConfig{Id = 17,Key = "emailBodyRegisteredFO",Value = "<p>Olá,</p><br><p>encontra-se registado a plataforma do GHOTEL.</p><p>Por favor confirme o email clicando no seguinte <a href={0}>confirmar</a></p><br><p>Atentamente</p><p>GHOTEL</p>"},
               new AppConfig{Id = 18,Key = "emailSubjectRegisteredFO",Value = "Bem-vindo ao GHOTEL"},
               new AppConfig{Id = 19,Key = "smsBodyAfterConfirmation",Value = "Bem-vindo ao GHOTEL."},
               new AppConfig{Id = 20,Key = "smsApiLogin",Value = "/user/login"},
               new AppConfig{Id = 21,Key = "smsApiTestToken",Value = "/sender-id/list-one"},
               new AppConfig{Id = 22,Key = "smsApiUsername",Value = "929011521"},
               new AppConfig{Id = 23,Key = "smsApiPassword",Value = "maptss12345"},
               new AppConfig{Id = 24,Key = "smsSenderName",Value = "INEFOP"},
               new AppConfig{Id = 25,Key = "smsUserToken",Value = "1a95521ba4c88479b24f59cd5a2a7b83929011521"},
               new AppConfig{Id = 26,Key = "useSsl",Value = "true"},
               new AppConfig{Id = 27,Key = "backOfficeUrl",Value = "http://oinquilino.ao"},
               new AppConfig{Id = 28,Key = "frontOfficeUrl",Value = "http://oinquilino.ao"},
               new AppConfig{Id = 29,Key = "smsApiUri",Value = "http://52.30.114.86:8080/mimosms/v1"},
               new AppConfig{Id = 30,Key = "smsApiSendSMS",Value = "/message/send"},
               new AppConfig{Id = 31,Key = "smsBodyUserNotification",Value = "Motivo: {0} - Assunto : {1}. {2}"},
               new AppConfig{Id = 32,Key = "emailChangePassswordUrl",Value = "http://localhost:5055/api/usuario/change-password?token={0}&username={1}"},
               new AppConfig{Id = 33,Key = "emailBodyBeforeChangePassword",Value = "<p>Olá,</p><br><p>Foi efectuado um pedido de mudança de palavra-passe na plataforma do GHOTEL.</p><p>Por favor confirme a alteração clicando em <a href={0}>confirmar</a></p><br><p>Atentamente</p><p>Equipa do Ghotel</p>"},
               new AppConfig{Id = 34,Key = "emailSubjectBeforeChangePassword",Value = "GHOTEL - Pedido de alteração de palavra-passe"}
/* <p>Olá ,</p> <p>Sua conta foi registrada com sucesso! Para ativá-la e começar a explorar todos os recursos, clique no botão abaixo:</p> <p><a href={0} style="font-family:'Google Sans',Roboto,RobotoDraft,Helvetica,Arial,sans-serif;line-height:16px;color:#ffffff;font-weight:400;text-decoration:none;font-size:14px;display:inline-block;padding:10px 24px;background-color:#4184f3;border-radius:5px;min-width:90px" >Ativar Conta</a></p>  <p>Caso tenha alguma dúvida, nossa equipe de suporte está disponível para ajudar.</p>    <p>Bem-vindo(a) à nossa plataforma!</p>    <p>Atenciosamente,<br>GHOTEL</p> */
           
           );
        }
    }
}
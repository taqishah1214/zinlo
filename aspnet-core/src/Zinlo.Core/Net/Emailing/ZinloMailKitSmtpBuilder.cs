﻿using Abp.MailKit;
using Abp.Net.Mail.Smtp;
using MailKit.Net.Smtp;

namespace Zinlo.Net.Emailing
{
    public class ZinloMailKitSmtpBuilder : DefaultMailKitSmtpBuilder
    {
        public ZinloMailKitSmtpBuilder(
            ISmtpEmailSenderConfiguration smtpEmailSenderConfiguration,
            IAbpMailKitConfiguration abpMailKitConfiguration) : base(smtpEmailSenderConfiguration, abpMailKitConfiguration)
        {

        }

        protected override void ConfigureClient(SmtpClient client)
        {
            client.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
            base.ConfigureClient(client);
        }
    }
}

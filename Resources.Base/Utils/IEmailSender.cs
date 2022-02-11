using System;

namespace Resources.Base.Utils
{
    public interface IEmailSender
    {
        bool SendGenericEmail(string email, string subject, string body);

        bool SendProducerEmailAfterAdminValidateAccount(string email, string producerName);

        bool SendAdminEmailAfterProducerRegisters(string producerId, string producerName, string producerEmail, string userName);

        bool SendAdminEmailAfterUserUpdatedToProducer(string producerId, string producerName);

        bool SendRegisterConfirmationEmail(Guid guid, string email, string firstName);

        bool SendSNSConfirmationToken(string token);

        bool SendEmailToResetPassword(string email, string token, string userName);

        bool SendAdminEmailAboutAVideoReport(string videoId, string userId, string reportReason);
    }
}

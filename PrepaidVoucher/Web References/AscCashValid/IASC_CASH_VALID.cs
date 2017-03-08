using System;
namespace PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid
{
    public interface IASC_CASH_VALID : IDisposable
    {
        void CancelAsync(object userState);
        string Url { get; set; }
        bool UseDefaultCredentials { get; set; }
        string user_auth(string accountno, string usercode, string password);
        string user_auth_amt(string accountno, string usercode, string password, double amt, out float amount);
        void user_auth_amtAsync(string accountno, string usercode, string password, double amt, object userState);
        void user_auth_amtAsync(string accountno, string usercode, string password, double amt);
        event user_auth_amtCompletedEventHandler user_auth_amtCompleted;
        void user_authAsync(string accountno, string usercode, string password, object userState);
        void user_authAsync(string accountno, string usercode, string password);
        event user_authCompletedEventHandler user_authCompleted;
    }

    public partial class ASC_CASH_VALID : System.Web.Services.Protocols.SoapHttpClientProtocol, IASC_CASH_VALID
    {
    }
}

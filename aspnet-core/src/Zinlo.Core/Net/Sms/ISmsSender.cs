using System.Threading.Tasks;

namespace Zinlo.Net.Sms
{
    public interface ISmsSender
    {
        Task SendAsync(string number, string message);
    }
}
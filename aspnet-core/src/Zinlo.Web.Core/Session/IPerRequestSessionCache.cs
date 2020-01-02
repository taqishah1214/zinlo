using System.Threading.Tasks;
using Zinlo.Sessions.Dto;

namespace Zinlo.Web.Session
{
    public interface IPerRequestSessionCache
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformationsAsync();
    }
}

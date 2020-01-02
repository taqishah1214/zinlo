using System.Threading.Tasks;
using Abp.Dependency;

namespace Zinlo.MultiTenancy.Accounting
{
    public interface IInvoiceNumberGenerator : ITransientDependency
    {
        Task<string> GetNewInvoiceNumber();
    }
}
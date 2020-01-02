using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Zinlo.MultiTenancy.Accounting.Dto;

namespace Zinlo.MultiTenancy.Accounting
{
    public interface IInvoiceAppService
    {
        Task<InvoiceDto> GetInvoiceInfo(EntityDto<long> input);

        Task CreateInvoice(CreateInvoiceDto input);
    }
}

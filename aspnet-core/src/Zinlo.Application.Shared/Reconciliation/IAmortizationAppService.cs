using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Reconciliation.Dtos;

namespace Zinlo.Reconciliation
{
  public  interface IAmortizationAppService
    {
        Task CreateOrEdit(CreateOrEditAmortizationDto input);
    }
}

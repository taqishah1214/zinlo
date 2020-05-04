using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.Dto;
using Zinlo.Reporting.Dtos;

namespace Zinlo.Reporting.Importing
    {
        public interface ITrialBalanceExporter
        {
            FileDto ExportToFile(List<CompareTrialBalanceViewDto> List, string FirstMonth, string SecondMonth);
        }
    }



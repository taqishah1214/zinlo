﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zinlo.MultiTenancy.HostDashboard.Dto;

namespace Zinlo.MultiTenancy.HostDashboard
{
    public interface IIncomeStatisticsService
    {
        Task<List<IncomeStastistic>> GetIncomeStatisticsData(DateTime startDate, DateTime endDate,
            ChartDateInterval dateInterval);
    }
}
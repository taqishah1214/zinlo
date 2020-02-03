using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Zinlo.TimeManagements.Dtos
{
    public class GetTimeManagementForEditOutput
    {
		public CreateOrEditTimeManagementDto TimeManagement { get; set; }


    }
}
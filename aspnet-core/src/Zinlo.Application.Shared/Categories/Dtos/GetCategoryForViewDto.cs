using System;
using Abp.Application.Services.Dto;

namespace Zinlo.Categories.Dtos
{
    public class GetCategoryForViewDto : EntityDto<long>
    {
        public string Title { get; set; }

        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public long? UserId { get; set; }


    }
}
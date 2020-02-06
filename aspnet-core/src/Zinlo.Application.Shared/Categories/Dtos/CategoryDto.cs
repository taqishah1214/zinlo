
using System;
using Abp.Application.Services.Dto;

namespace Zinlo.Categories.Dtos
{
    public class CategoryDto : EntityDto<long>
    {
		public string Title { get; set; }

		public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; }
        public long? UserId { get; set; }
        public string ProfilePicture { get; set; }


    }
}
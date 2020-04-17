using System.ComponentModel.DataAnnotations;

namespace Zinlo.Editions.Dto
{
    public class EditionEditDto
    {
        public int? Id { get; set; }

        [Required]
        public string DisplayName { get; set; }
        public bool IsCustom { get; set; }

        public int? ExpiringEditionId { get; set; }
    }
}
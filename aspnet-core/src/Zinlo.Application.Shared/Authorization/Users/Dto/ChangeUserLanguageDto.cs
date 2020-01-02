using System.ComponentModel.DataAnnotations;

namespace Zinlo.Authorization.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}

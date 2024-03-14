using Algar.Hours.Domain.Entities.Country;
using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Template.Commands.Create
{
    public class TemplateModelC {

        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string FileName { get; set; }
        [Required]
        public string FileContentType { get; set; }
        [Required]
        public byte[] FileData { get; set; }
    }
}

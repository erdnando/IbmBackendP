using Algar.Hours.Domain.Entities.Country;
using Algar.Hours.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Domain.Entities.Template
{
    public class TemplateEntity
    {
        [Key]
        public Guid IdTemplate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string FileContentType { get; set; }
        public byte[] FileData { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        
    }
}

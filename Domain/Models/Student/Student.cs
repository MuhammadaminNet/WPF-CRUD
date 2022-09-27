using Domain.Models.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Student
{
    public class Student : Auditable
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Faculty { get; set; }
        public long? PassportId { get; set; }
        [ForeignKey(nameof(PassportId))]
        public Attachment Passport { get; set; }
        public long? ImageId { get; set; }
        [ForeignKey(nameof(ImageId))]
        public Attachment Image { get; set; }
    }
}

using Domain.Models.Student;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam.Services.DTOs
{
    public class StudentForCreation
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Faculty { get; set; }
        public long? PassportId { get; set; }
        [ForeignKey(nameof(PassportId))]
        public Attachment? Passport { get; set; }
        public long? ImageId { get; set; }
        [ForeignKey(nameof(ImageId))]
        public Attachment? Image { get; set; }

    }
}

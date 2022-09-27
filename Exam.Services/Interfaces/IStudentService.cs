using Domain.Models.Student;
using Exam.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam.Services.Interfaces
{
    public interface IStudentService : IDisposable
    {
        Task<IEnumerable<Student>> GetAllAsync();
        Task<Student> GetAsync(long id);
        Task<Student> CreateAsync(StudentForCreation dto);
        Task<Student> UpdateAsync(long id,StudentForCreation dto);
        Task<bool> DeleteAsync(long id);
        Task UploadFileAsync(long id, string imagePath, string passportPath);
    }
}

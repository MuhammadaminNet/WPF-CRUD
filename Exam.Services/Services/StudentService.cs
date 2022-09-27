using Domain.Models.Student;
using Exam.Services.DTOs;
using Exam.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Exam.Services.Services
{
    public class StudentService : IStudentService
    {
        public async Task<Student> CreateAsync(StudentForCreation dto)
        {
            using (HttpClient client = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(dto);

                var respons = await client.PostAsync(AppSettings.BASE_URL + "students/", new StringContent(json, Encoding.UTF8, "application/json"));
                
                if (respons.IsSuccessStatusCode)
                {
                    var message = await respons.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<Student>(message);
                }

                return null;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.DeleteAsync(AppSettings.BASE_URL + "students/" + id);

                return response.IsSuccessStatusCode;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:12345")));

                string json = await client.GetStringAsync(AppSettings.BASE_URL + "Students");

                var students = JsonConvert.DeserializeObject<IEnumerable<Student>>(json);

                return students;
            }
        }

        public async Task<Student> GetAsync(long id)
        {
            using (HttpClient client = new HttpClient())
            {
                var json = await client.GetStringAsync(AppSettings.BASE_URL + "students/"+id);

                var student = JsonConvert.DeserializeObject<Student>(json);

                return student;
            }
        }

        public async Task<Student> UpdateAsync(long id, StudentForCreation dto)
        {
            using (HttpClient client = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(dto);

                var respons = await client.PostAsync(AppSettings.BASE_URL + "students/"+id, new StringContent(json, Encoding.Default, "application/json"));

                var message = await respons.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(message))
                {
                    return null;
                }

                return JsonConvert.DeserializeObject<Student>(message);
            }
        }

        public async Task UploadFileAsync(long id, string imagePath, string passportPath)
        {
            
            var imageFile = await File.ReadAllBytesAsync(imagePath);
            var passportFile = await File.ReadAllBytesAsync(passportPath);

            using (HttpClient client = new HttpClient())
            {
                MultipartFormDataContent formData = new MultipartFormDataContent();

                formData.Add(new ByteArrayContent(imageFile), "Image", "image.png");
                formData.Add(new ByteArrayContent(passportFile), "Passport", "passport.png");

                await client.PostAsync(AppSettings.BASE_URL + "students/attachments/" + id, formData);
            }
            
        }
    }
}

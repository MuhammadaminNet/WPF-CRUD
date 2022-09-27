using Exam.Services.Interfaces;
using Exam.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Exam.Services;
using System.IO;
using Domain.Models.Student;
using System.Threading;
using Exam.UI.Controls;

namespace Exam
{
    #pragma warning disable
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IStudentService service;
        public MainWindow()
        {
            service = new StudentService();
            InitializeComponent();
        }

        private IEnumerable<Student> allUsers;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(async () =>
            {
                this.Dispatcher.Invoke(() => Students.Items.Clear());

                allUsers = await service.GetAllAsync();

                await LoadStudents(allUsers);
            });

            thread.Start();
        }

        private async Task LoadStudents(IEnumerable<Student> students)
        {
            foreach (var student in students)
            {
                await this.Dispatcher.InvokeAsync(() =>
                {
                    UserCards userCards = new UserCards();
                    userCards.IdOfStudent.Text = student.Id.ToString();
                    userCards.NameTxtBlock.Text = student.FirstName;
                    userCards.FacultyTxtBlock.Text = student.Faculty;
                    userCards.StudentImg.ImageSource = student.Image is not null
                    ? new BitmapImage(new Uri("https://talabamiz.uz/" + student.Image.Path))
                        : new BitmapImage(new Uri("https://talabamiz.uz/Images//99daf8ac38de4433aa36a61baf4c9c4d.png"));

                    Students.Items.Add(userCards);
                });
            }
        }
        private Thread thread;

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            Students.Items.Clear();

            string text = Search.Text.ToLower();

            var users = allUsers.Where(p => p.FirstName.ToLower().Contains(text)
                || p.LastName.ToLower().Contains(text)
                || p.Faculty.ToLower().Contains(text));

            thread = new Thread(async () =>
            {
                await LoadStudents(users);
            });
            thread.Start();
        }

        private async void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            string fName = Name.Text;
            string lName = LastName.Text;
            string faculty = Faculty.Text;
            string passportPath = PassportImage.Text;
            string imagePath = Image.Text;

            if(File.Exists(passportPath) && File.Exists(imagePath))
            {
                using (IStudentService service = new StudentService())
                {
                    var student = await service.CreateAsync(new Services.DTOs.StudentForCreation()
                    {
                        Faculty = faculty,
                        FirstName = fName,
                        LastName = lName
                    });

                    if (student != null)
                    {
                        Id.Text = student.Id.ToString();

                        await service.UploadFileAsync(student.Id, passportPath, imagePath);

                        GetBtn_Click(sender, e);

                        Thread thread = new Thread(async () =>
                        {
                            this.Dispatcher.Invoke(() => Students.Items.Clear());

                            allUsers = await service.GetAllAsync();

                            await LoadStudents(allUsers);
                        });

                        thread.Start();

                        Name.Text = "";
                        LastName.Text = "";
                        Faculty.Text = "";
                        PassportImage.Text = "";
                        Image.Text = "";
                    }

                    else
                    {
                        Name.Text = "Error";
                    }
                }
            }

            else
            {
                PassportImage.Text = "Image not found";
                Image.Text = "Image not found";
            }
        }

        private async void GetBtn_Click(object sender, RoutedEventArgs e)
        {

            if (Id.Text.All(char.IsDigit))
            {
                long id = long.Parse(Id.Text);

                IStudentService service = new StudentService();

                var student = await service.GetAsync(id);

                if (student != null)
                {
                    FullNameOutPut.Text = student.FirstName + student.LastName;

                    FacultyOutPut.Text = student.Faculty;

                    ImageOutPut.Source = student.Image is not null
                    ? new BitmapImage(new Uri("https://talabamiz.uz/" + student.Image.Path))
                        : new BitmapImage(new Uri("https://talabamiz.uz/Images//99daf8ac38de4433aa36a61baf4c9c4d.png"));

                    PassportOutPut.Source = student.Image is not null
                    ? new BitmapImage(new Uri("https://talabamiz.uz/" + student.Passport.Path))
                        : new BitmapImage(new Uri("https://talabamiz.uz/Images//99daf8ac38de4433aa36a61baf4c9c4d.png"));
                }

                else
                {
                    FullNameOutPut.Text = "Student not found";
                }
            }
            else
            {
                Id.Text = "Not valid format of id";
            }
        }

        private async void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            IStudentService service = new StudentService();

            await service.UpdateAsync(long.Parse(Id.Text), new Services.DTOs.StudentForCreation()
            {
                Faculty = Faculty.Text,
                FirstName = Name.Text,
                LastName = LastName.Text,
            });

            await service.UploadFileAsync(long.Parse(Id.Text), PassportImage.Text, Image.Text);

            GetBtn_Click(sender, e);

            Name.Text = "";
            LastName.Text = "";
            Faculty.Text = "";
            PassportImage.Text = "";
            Image.Text = "";

            Thread thread = new Thread(async () =>
            {
                this.Dispatcher.Invoke(() => Students.Items.Clear());

                allUsers = await service.GetAllAsync();

                await LoadStudents(allUsers);
            });

            thread.Start();
        }

        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            IStudentService service = new StudentService();

            await service.DeleteAsync(long.Parse(Id.Text));

            Thread thread = new Thread(async () =>
            {
                this.Dispatcher.Invoke(() => Students.Items.Clear());

                allUsers = await service.GetAllAsync();

                await LoadStudents(allUsers);
            });

            thread.Start();

        }
    }
}

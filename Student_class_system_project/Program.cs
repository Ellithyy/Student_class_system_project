using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentManagementSystem
{
    // Base class: Person
    public abstract class Person
    {
        public string Name { get; protected set; }
        public int Age { get; protected set; }
        public string Gender { get; protected set; }

        protected Person(string name, int age, string gender)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Age = age;
            Gender = gender ?? throw new ArgumentNullException(nameof(gender));
        }

        public virtual void DisplayInfo()
        {
            Console.WriteLine($"{"Name:",-15} {Name}");
            Console.WriteLine($"{"Age:",-15} {Age}");
            Console.WriteLine($"{"Gender:",-15} {Gender}");
        }
    }

    // Teacher class
    public class Teacher : Person
    {
        public string Subject { get; private set; }
        public string TeacherId { get; private set; }

        public Teacher(string name, int age, string gender, string subject, string teacherId)
            : base(name, age, gender)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            TeacherId = teacherId ?? throw new ArgumentNullException(nameof(teacherId));
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"{"Subject:",-15} {Subject}");
            Console.WriteLine($"{"Teacher ID:",-15} {TeacherId}");
        }
    }

    // Change the access modifier of the EnrolledStudents property in the Course class
    public class Course
    {
        public string CourseCode { get; }
        public string CourseName { get; }
        public Teacher Instructor { get; private set; }
        public List<Student> EnrolledStudents { get; } = new List<Student>(); // Changed from private to public

        public Course(string courseCode, string courseName, Teacher instructor)
        {
            CourseCode = courseCode ?? throw new ArgumentNullException(nameof(courseCode));
            CourseName = courseName ?? throw new ArgumentNullException(nameof(courseName));
            Instructor = instructor ?? throw new ArgumentNullException(nameof(instructor));
        }

        public void EnrollStudent(Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));

            if (!EnrolledStudents.Contains(student))
            {
                EnrolledStudents.Add(student);
                Console.WriteLine($"{student.Name} enrolled in {CourseName}");
            }
            else
            {
                Console.WriteLine($"{student.Name} is already enrolled in {CourseName}");
            }
        }

        public void DisplayCourseInfo()
        {
            Console.WriteLine($"\n{"Course Code:",-15} {CourseCode}");
            Console.WriteLine($"{"Course Name:",-15} {CourseName}");
            Console.WriteLine("\nInstructor:");
            Instructor.DisplayInfo();

            Console.WriteLine("\nEnrolled Students:");
            if (EnrolledStudents.Any())
            {
                foreach (var student in EnrolledStudents)
                {
                    student.DisplayBasicInfo();
                    Console.WriteLine(new string('-', 40));
                }
            }
            else
            {
                Console.WriteLine("No students enrolled yet.");
            }
        }

        public void DisplayStudentGrades()
        {
            Console.WriteLine($"\nGrades for {CourseName}:");
            foreach (var student in EnrolledStudents)
            {
                Console.WriteLine($"\n{student.Name} (ID: {student.StudentId})");
                student.DisplayGrades();
            }
        }
    }

    // Student class
    public class Student : Person
    {
        public string StudentId { get; }
        public string Major { get; private set; }
        private Dictionary<string, List<int>> CourseGrades { get; } = new Dictionary<string, List<int>>();

        public Student(string name, int age, string gender, string studentId, string major)
            : base(name, age, gender)
        {
            StudentId = studentId ?? throw new ArgumentNullException(nameof(studentId));
            Major = major ?? throw new ArgumentNullException(nameof(major));
        }

        public void AddGrade(string courseCode, int grade)
        {
            if (string.IsNullOrWhiteSpace(courseCode))
                throw new ArgumentException("Course code cannot be empty", nameof(courseCode));

            if (grade < 0 || grade > 100)
            {
                Console.WriteLine("Invalid grade. Please enter a number between 0 and 100.");
                return;
            }

            if (!CourseGrades.ContainsKey(courseCode))
            {
                CourseGrades[courseCode] = new List<int>();
            }

            CourseGrades[courseCode].Add(grade);
            Console.WriteLine($"Grade {grade} added for {Name} in course {courseCode}");
        }

        public double GetAverageGrade(string courseCode)
        {
            if (!CourseGrades.ContainsKey(courseCode) || !CourseGrades[courseCode].Any())
            {
                Console.WriteLine($"No grades available for {courseCode}");
                return 0;
            }

            return CourseGrades[courseCode].Average();
        }

        public int GetHighestGrade(string courseCode)
        {
            if (!CourseGrades.ContainsKey(courseCode) || !CourseGrades[courseCode].Any())
            {
                Console.WriteLine($"No grades available for {courseCode}");
                return 0;
            }

            return CourseGrades[courseCode].Max();
        }

        public string GetStatus(string courseCode)
        {
            double average = GetAverageGrade(courseCode);
            return average >= 60 ? "Pass" : "Fail";
        }

        public double GetGPA()
        {
            if (!CourseGrades.Any()) return 0;

            double totalPoints = 0;
            int totalCourses = 0;

            foreach (var course in CourseGrades)
            {
                double avg = course.Value.Average();
                totalPoints += ConvertAverageToGpaPoints(avg);
                totalCourses++;
            }

            return totalPoints / totalCourses;
        }

        private double ConvertAverageToGpaPoints(double average)
        {
            if (average >= 90) return 4.0;
            if (average >= 80) return 3.0;
            if (average >= 70) return 2.0;
            if (average >= 60) return 1.0;
            return 0.0;
        }

        public void DisplayBasicInfo()
        {
            Console.WriteLine($"{"Name:",-15} {Name}");
            Console.WriteLine($"{"Student ID:",-15} {StudentId}");
            Console.WriteLine($"{"Major:",-15} {Major}");
        }

        public void DisplayGrades(string courseCode = null)
        {
            if (!string.IsNullOrEmpty(courseCode))
            {
                if (CourseGrades.ContainsKey(courseCode))
                {
                    Console.WriteLine($"Grades for {courseCode}:");
                    Console.WriteLine(string.Join(", ", CourseGrades[courseCode]));
                    Console.WriteLine($"Average: {GetAverageGrade(courseCode):F1}");
                    Console.WriteLine($"Status: {GetStatus(courseCode)}");
                }
                else
                {
                    Console.WriteLine($"No grades recorded for {courseCode}");
                }
            }
            else
            {
                if (CourseGrades.Any())
                {
                    foreach (var course in CourseGrades)
                    {
                        Console.WriteLine($"\n{course.Key}:");
                        Console.WriteLine($"Grades: {string.Join(", ", course.Value)}");
                        Console.WriteLine($"Average: {GetAverageGrade(course.Key):F1}");
                        Console.WriteLine($"Status: {GetStatus(course.Key)}");
                    }
                    Console.WriteLine($"\nOverall GPA: {GetGPA():F2}");
                }
                else
                {
                    Console.WriteLine("No grades recorded yet.");
                }
            }
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"{"Student ID:",-15} {StudentId}");
            Console.WriteLine($"{"Major:",-15} {Major}");

            DisplayGrades();
        }
    }

    // Main Program with menu system
    public class Program
    {
        private static List<Student> students = new List<Student>();
        private static List<Teacher> teachers = new List<Teacher>();
        private static List<Course> courses = new List<Course>();

        static void Main(string[] args)
        {
            InitializeSampleData();

            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("Student Management System");
                Console.WriteLine("1. Student Operations");
                Console.WriteLine("2. Teacher Operations");
                Console.WriteLine("3. Course Operations");
                Console.WriteLine("4. View Reports");
                Console.WriteLine("5. Exit");
                Console.Write("Select an option: ");

                if (int.TryParse(Console.ReadLine(), out int mainChoice))
                {
                    switch (mainChoice)
                    {
                        case 1:
                            StudentMenu();
                            break;
                        case 2:
                            TeacherMenu();
                            break;
                        case 3:
                            CourseMenu();
                            break;
                        case 4:
                            ReportMenu();
                            break;
                        case 5:
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }

                if (!exit)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void StudentMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                Console.WriteLine("Student Operations");
                Console.WriteLine("1. Add New Student");
                Console.WriteLine("2. Add Grade to Student");
                Console.WriteLine("3. View Student Details");
                Console.WriteLine("4. List All Students");
                Console.WriteLine("5. Back to Main Menu");
                Console.Write("Select an option: ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            AddStudent();
                            break;
                        case 2:
                            AddGradeToStudent();
                            break;
                        case 3:
                            ViewStudentDetails();
                            break;
                        case 4:
                            ListAllStudents();
                            break;
                        case 5:
                            back = true;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }

                if (!back)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void TeacherMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                Console.WriteLine("Teacher Operations");
                Console.WriteLine("1. Add New Teacher");
                Console.WriteLine("2. View Teacher Details");
                Console.WriteLine("3. List All Teachers");
                Console.WriteLine("4. Back to Main Menu");
                Console.Write("Select an option: ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            AddTeacher();
                            break;
                        case 2:
                            ViewTeacherDetails();
                            break;
                        case 3:
                            ListAllTeachers();
                            break;
                        case 4:
                            back = true;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }

                if (!back)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void CourseMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                Console.WriteLine("Course Operations");
                Console.WriteLine("1. Create New Course");
                Console.WriteLine("2. Enroll Student in Course");
                Console.WriteLine("3. View Course Details");
                Console.WriteLine("4. List All Courses");
                Console.WriteLine("5. Back to Main Menu");
                Console.Write("Select an option: ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            CreateCourse();
                            break;
                        case 2:
                            EnrollStudentInCourse();
                            break;
                        case 3:
                            ViewCourseDetails();
                            break;
                        case 4:
                            ListAllCourses();
                            break;
                        case 5:
                            back = true;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }

                if (!back)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void ReportMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                Console.WriteLine("Reports");
                Console.WriteLine("1. Student Grade Report");
                Console.WriteLine("2. Course Enrollment Report");
                Console.WriteLine("3. Teacher Course Report");
                Console.WriteLine("4. Back to Main Menu");
                Console.Write("Select an option: ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            GenerateStudentGradeReport();
                            break;
                        case 2:
                            GenerateCourseEnrollmentReport();
                            break;
                        case 3:
                            GenerateTeacherCourseReport();
                            break;
                        case 4:
                            back = true;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }

                if (!back)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void AddStudent()
        {
            Console.Write("Enter student name: ");
            string name = Console.ReadLine();

            Console.Write("Enter age: ");
            if (!int.TryParse(Console.ReadLine(), out int age))
            {
                Console.WriteLine("Invalid age. Operation cancelled.");
                return;
            }

            Console.Write("Enter gender: ");
            string gender = Console.ReadLine();

            Console.Write("Enter student ID: ");
            string studentId = Console.ReadLine();

            Console.Write("Enter major: ");
            string major = Console.ReadLine();

            var student = new Student(name, age, gender, studentId, major);
            students.Add(student);
            Console.WriteLine($"Student {name} added successfully.");
        }

        static void AddGradeToStudent()
        {
            if (!students.Any())
            {
                Console.WriteLine("No students available. Please add students first.");
                return;
            }

            Console.WriteLine("Available Students:");
            for (int i = 0; i < students.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {students[i].Name} (ID: {students[i].StudentId})");
            }

            Console.Write("Select student (number): ");
            if (!int.TryParse(Console.ReadLine(), out int studentIndex) || studentIndex < 1 || studentIndex > students.Count)
            {
                Console.WriteLine("Invalid selection. Operation cancelled.");
                return;
            }

            var student = students[studentIndex - 1];

            if (!courses.Any())
            {
                Console.WriteLine("No courses available. Please create courses first.");
                return;
            }

            Console.WriteLine("Available Courses:");
            for (int i = 0; i < courses.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {courses[i].CourseCode} - {courses[i].CourseName}");
            }

            Console.Write("Select course (number): ");
            if (!int.TryParse(Console.ReadLine(), out int courseIndex) || courseIndex < 1 || courseIndex > courses.Count)
            {
                Console.WriteLine("Invalid selection. Operation cancelled.");
                return;
            }

            var course = courses[courseIndex - 1];

            Console.Write("Enter grade (0-100): ");
            if (!int.TryParse(Console.ReadLine(), out int grade))
            {
                Console.WriteLine("Invalid grade. Operation cancelled.");
                return;
            }

            student.AddGrade(course.CourseCode, grade);
        }

        static void ViewStudentDetails()
        {
            if (!students.Any())
            {
                Console.WriteLine("No students available.");
                return;
            }

            Console.WriteLine("Available Students:");
            for (int i = 0; i < students.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {students[i].Name} (ID: {students[i].StudentId})");
            }

            Console.Write("Select student (number) or 0 to cancel: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > students.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            if (choice == 0) return;

            students[choice - 1].DisplayInfo();
        }

        static void ListAllStudents()
        {
            if (!students.Any())
            {
                Console.WriteLine("No students available.");
                return;
            }

            Console.WriteLine("\nList of All Students:");
            Console.WriteLine(new string('=', 50));
            foreach (var student in students)
            {
                student.DisplayBasicInfo();
                Console.WriteLine(new string('-', 50));
            }
        }

        static void AddTeacher()
        {
            Console.Write("Enter teacher name: ");
            string name = Console.ReadLine();

            Console.Write("Enter age: ");
            if (!int.TryParse(Console.ReadLine(), out int age))
            {
                Console.WriteLine("Invalid age. Operation cancelled.");
                return;
            }

            Console.Write("Enter gender: ");
            string gender = Console.ReadLine();

            Console.Write("Enter subject: ");
            string subject = Console.ReadLine();

            Console.Write("Enter teacher ID: ");
            string teacherId = Console.ReadLine();

            var teacher = new Teacher(name, age, gender, subject, teacherId);
            teachers.Add(teacher);
            Console.WriteLine($"Teacher {name} added successfully.");
        }

        static void ViewTeacherDetails()
        {
            if (!teachers.Any())
            {
                Console.WriteLine("No teachers available.");
                return;
            }

            Console.WriteLine("Available Teachers:");
            for (int i = 0; i < teachers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {teachers[i].Name} (ID: {teachers[i].TeacherId})");
            }

            Console.Write("Select teacher (number) or 0 to cancel: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > teachers.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            if (choice == 0) return;

            teachers[choice - 1].DisplayInfo();
        }

        static void ListAllTeachers()
        {
            if (!teachers.Any())
            {
                Console.WriteLine("No teachers available.");
                return;
            }

            Console.WriteLine("\nList of All Teachers:");
            Console.WriteLine(new string('=', 50));
            foreach (var teacher in teachers)
            {
                teacher.DisplayInfo();
                Console.WriteLine(new string('-', 50));
            }
        }

        static void CreateCourse()
        {
            if (!teachers.Any())
            {
                Console.WriteLine("No teachers available. Please add teachers first.");
                return;
            }

            Console.Write("Enter course code: ");
            string code = Console.ReadLine();

            Console.Write("Enter course name: ");
            string name = Console.ReadLine();

            Console.WriteLine("Available Teachers:");
            for (int i = 0; i < teachers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {teachers[i].Name} ({teachers[i].Subject})");
            }

            Console.Write("Select teacher (number): ");
            if (!int.TryParse(Console.ReadLine(), out int teacherIndex) || teacherIndex < 1 || teacherIndex > teachers.Count)
            {
                Console.WriteLine("Invalid selection. Operation cancelled.");
                return;
            }

            var teacher = teachers[teacherIndex - 1];
            var course = new Course(code, name, teacher);
            courses.Add(course);
            Console.WriteLine($"Course {code} - {name} created successfully with instructor {teacher.Name}.");
        }

        static void EnrollStudentInCourse()
        {
            if (!courses.Any())
            {
                Console.WriteLine("No courses available. Please create courses first.");
                return;
            }

            if (!students.Any())
            {
                Console.WriteLine("No students available. Please add students first.");
                return;
            }

            Console.WriteLine("Available Courses:");
            for (int i = 0; i < courses.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {courses[i].CourseCode} - {courses[i].CourseName}");
            }

            Console.Write("Select course (number): ");
            if (!int.TryParse(Console.ReadLine(), out int courseIndex) || courseIndex < 1 || courseIndex > courses.Count)
            {
                Console.WriteLine("Invalid selection. Operation cancelled.");
                return;
            }

            var course = courses[courseIndex - 1];

            Console.WriteLine("Available Students:");
            for (int i = 0; i < students.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {students[i].Name} (ID: {students[i].StudentId})");
            }

            Console.Write("Select student (number): ");
            if (!int.TryParse(Console.ReadLine(), out int studentIndex) || studentIndex < 1 || studentIndex > students.Count)
            {
                Console.WriteLine("Invalid selection. Operation cancelled.");
                return;
            }

            var student = students[studentIndex - 1];
            course.EnrollStudent(student);
        }

        static void ViewCourseDetails()
        {
            if (!courses.Any())
            {
                Console.WriteLine("No courses available.");
                return;
            }

            Console.WriteLine("Available Courses:");
            for (int i = 0; i < courses.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {courses[i].CourseCode} - {courses[i].CourseName}");
            }

            Console.Write("Select course (number) or 0 to cancel: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > courses.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            if (choice == 0) return;

            courses[choice - 1].DisplayCourseInfo();
        }

        static void ListAllCourses()
        {
            if (!courses.Any())
            {
                Console.WriteLine("No courses available.");
                return;
            }

            Console.WriteLine("\nList of All Courses:");
            Console.WriteLine(new string('=', 70));
            foreach (var course in courses)
            {
                Console.WriteLine($"{course.CourseCode} - {course.CourseName}");
                Console.WriteLine($"Instructor: {course.Instructor.Name} ({course.Instructor.Subject})");
                Console.WriteLine($"Enrolled Students: {course.EnrolledStudents.Count}");
                Console.WriteLine(new string('-', 70));
            }
        }

        static void GenerateStudentGradeReport()
        {
            if (!students.Any())
            {
                Console.WriteLine("No students available.");
                return;
            }

            Console.WriteLine("Available Students:");
            for (int i = 0; i < students.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {students[i].Name} (ID: {students[i].StudentId})");
            }

            Console.Write("Select student (number) or 0 for all students: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > students.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            Console.WriteLine("\nGRADE REPORT");
            Console.WriteLine(new string('=', 50));

            if (choice == 0)
            {
                foreach (var student in students)
                {
                    Console.WriteLine($"\nStudent: {student.Name} (ID: {student.StudentId})");
                    student.DisplayGrades();
                    Console.WriteLine(new string('-', 50));
                }
            }
            else
            {
                var student = students[choice - 1];
                Console.WriteLine($"\nStudent: {student.Name} (ID: {student.StudentId})");
                student.DisplayGrades();
            }
        }

        static void GenerateCourseEnrollmentReport()
        {
            if (!courses.Any())
            {
                Console.WriteLine("No courses available.");
                return;
            }

            Console.WriteLine("\nCOURSE ENROLLMENT REPORT");
            Console.WriteLine(new string('=', 70));

            foreach (var course in courses)
            {
                Console.WriteLine($"\nCourse: {course.CourseCode} - {course.CourseName}");
                Console.WriteLine($"Instructor: {course.Instructor.Name}");
                Console.WriteLine($"Enrolled Students: {course.EnrolledStudents.Count}");

                if (course.EnrolledStudents.Any())
                {
                    Console.WriteLine("\nEnrolled Students:");
                    foreach (var student in course.EnrolledStudents)
                    {
                        Console.WriteLine($"- {student.Name} (ID: {student.StudentId})");
                    }
                }
                else
                {
                    Console.WriteLine("No students enrolled.");
                }
                Console.WriteLine(new string('-', 70));
            }
        }

        static void GenerateTeacherCourseReport()
        {
            if (!teachers.Any())
            {
                Console.WriteLine("No teachers available.");
                return;
            }

            Console.WriteLine("\nTEACHER COURSE REPORT");
            Console.WriteLine(new string('=', 70));

            foreach (var teacher in teachers)
            {
                Console.WriteLine($"\nTeacher: {teacher.Name} (ID: {teacher.TeacherId})");
                Console.WriteLine($"Subject: {teacher.Subject}");

                var teacherCourses = courses.Where(c => c.Instructor == teacher).ToList();
                Console.WriteLine($"Courses Teaching: {teacherCourses.Count}");

                if (teacherCourses.Any())
                {
                    Console.WriteLine("\nCourses:");
                    foreach (var course in teacherCourses)
                    {
                        Console.WriteLine($"- {course.CourseCode} - {course.CourseName}");
                        Console.WriteLine($"  Enrolled Students: {course.EnrolledStudents.Count}");
                    }
                }
                else
                {
                    Console.WriteLine("No courses assigned.");
                }
                Console.WriteLine(new string('-', 70));
            }
        }

        static void InitializeSampleData()
        {
            // Create sample teachers
            var teacher1 = new Teacher("Dr. Sabry", 45, "Male", "Computer Science", "T001");
            var teacher2 = new Teacher("Prof. Rania", 50, "Female", "Mathematics", "T002");
            teachers.AddRange(new[] { teacher1, teacher2 });

            // Create sample students
            var student1 = new Student("Muhammad", 20, "Male", "S001", "Computer Science");
            var student2 = new Student("Nour", 21, "Male", "S002", "Mathematics");
            var student3 = new Student("Youssef", 22, "Male", "S003", "Computer Science");
            students.AddRange(new[] { student1, student2, student3 });

            // Create sample courses
            var course1 = new Course("CS101", "Introduction to Programming", teacher1);
            var course2 = new Course("MATH201", "Advanced Calculus", teacher2);
            courses.AddRange(new[] { course1, course2 });

            // Enroll students in courses
            course1.EnrollStudent(student1);
            course1.EnrollStudent(student3);
            course2.EnrollStudent(student2);
            course2.EnrollStudent(student3);

            // Add sample grades
            student1.AddGrade("CS101", 90);
            student1.AddGrade("CS101", 85);
            student3.AddGrade("CS101", 78);
            student3.AddGrade("CS101", 92);
            student2.AddGrade("MATH201", 88);
            student2.AddGrade("MATH201", 95);
            student3.AddGrade("MATH201", 82);
        }
    }
}
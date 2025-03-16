namespace SchoolManagementApp.MVC.Models
{
    public class DashboardViewModel
    {
        public int TotalCourses { get; set; }
        public int TotalStudents { get; set; }
        public int TotalLecturers { get; set; }
        public int GradedStudents { get; set; }
        public decimal GradingProgress => TotalStudents == 0 ? 0 : 
            (decimal)GradedStudents / TotalStudents * 100;
        public IEnumerable<Grade> RecentGrades { get; set; } = new List<Grade>();
        public decimal Gpa { get; set; }

        // public virtual 

        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string Program { get; set; }
        public IEnumerable<Course> EnrolledCourses { get; set; } = new List<Course>();
        public int TotalMaterials { get; set; }
        public double? AverageGrade { get; set; }
        public IEnumerable<StudentActivity> RecentActivities { get; set; } = new List<StudentActivity>();

        public List<SystemActivity> AdminRecentActivities { get; set; } = new();
        public List<QuickAction> QuickActions { get; set; } = new();

    }

    public class StudentActivity
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string ActivityType { get; set; }
        public string RelatedItemId { get; set; }
        public string IconClass { get; set; }
    }

    public class SystemActivity
    {
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string RelatedItemId { get; set; }
        public string IconClass { get; set; }
    }  

    public class QuickAction
    {
        public string Title { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string Icon { get; set; }
    }

}
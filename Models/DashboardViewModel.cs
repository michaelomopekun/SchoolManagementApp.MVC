namespace SchoolManagementApp.MVC.Models
{
    public class DashboardViewModel
    {
        public int TotalCourses { get; set; }
        public int TotalStudents { get; set; }
        public int GradedStudents { get; set; }
        public decimal GradingProgress => TotalStudents == 0 ? 0 : 
            (decimal)GradedStudents / TotalStudents * 100;
        public IEnumerable<Grade> RecentGrades { get; set; } = new List<Grade>();
    }
}
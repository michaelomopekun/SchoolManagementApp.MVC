namespace SchoolManagementApp.MVC.Authorization
{
    public static class PolicyNames
    {
        // Role-based policies
        public const string RequireAdmin = "RequireAdmin";
        public const string RequireLecturer = "RequireLecturer";
        public const string RequireStudent = "RequireStudent";

        // Permission-based policies
        public const string ManageUsers = "ManageUsers";
        public const string ManageCourses = "ManageCourses";
        public const string GradeStudents = "GradeStudents";
        public const string ViewGrades = "ViewGrades";
        public const string EnrollCourse = "EnrollCourse";
    }
}
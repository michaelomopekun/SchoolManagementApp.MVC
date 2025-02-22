public class Grade
{
    public int GradeId {get;set;}
    public int userId{get;set;}
    public User User{get;set;}
    public int CourseId{get;set;}
    public Course Course{get;set;}
    public decimal Score{get;set;}
    public string? Comments{get;set;}
    public DateTime GradedDate {get;set;}
}
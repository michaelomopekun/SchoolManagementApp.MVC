using Microsoft.EntityFrameworkCore;
using SchoolManagementApp.MVC.Models;

public class CourseMaterialService : ICourseMaterialService
{

    private readonly SchoolManagementAppDbContext _context;

    public CourseMaterialService(SchoolManagementAppDbContext context)
    {
        _context = context;
    }


    public async Task<CourseMaterial> UploadMaterialAsync(IFormFile file, int courseId, string title, string description, int uploaderId)
    {

    try
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var material = new CourseMaterial
        {
            FileName = file.FileName,
            CourseId = courseId,
            Title = title,
            Description = description,
            FileContent = memoryStream.ToArray(),
            ContentType = file.ContentType,
            FileSize = file.Length.ToString(),
            UploadDate = DateTime.Now,
            UploaderId = uploaderId
        };

        _context.CourseMaterials.Add(material);
        await _context.SaveChangesAsync();
        return material;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error uploading material: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
        }
        throw;
    }
    }

    public async Task<IEnumerable<CourseMaterial>> GetCourseMaterialsAsync(int courseId)
    {
        var materials = _context.CourseMaterials
            .Include(m => m.Downloads)
            .Include(m => m.Uploader)
            .Where(m => m.CourseId == courseId)
            .ToList();

        return materials;
    }

    public async Task<IEnumerable<CourseMaterialDownload>> GetDownloadHistoryAsync(int materialId)
    {
        var DownloadHistory = await _context.CourseMaterialDownloads
            .Include(d => d.Student)
            .Where(d => d.CourseMaterialId == materialId)
            .ToListAsync();

        return DownloadHistory;
    }

    public async Task<CourseMaterial> GetMaterialAsync(int materialId)
    {
        var material = await _context.CourseMaterials
            .Include(D => D.Downloads)
            .Include(m => m.Uploader)
            .FirstOrDefaultAsync(m => m.Id == materialId);

        return material;
    }

    public async Task<IEnumerable<CourseMaterial>> GetMaterialsByStudentAsync(int studentId)
    {
        
    var studentCourses = await _context.UserCourses
        .Where(e => e.UserId == studentId)
        .Select(e => e.CourseId)
        .ToListAsync();

    // Get all materials for those courses
    var materials = await _context.CourseMaterials
        .Include(m => m.Course)
        .Include(m => m.Uploader)
        .Include(m => m.Downloads)
        .Where(m => studentCourses.Contains(m.CourseId))
        .OrderByDescending(m => m.UploadDate)
        .ToListAsync();
     
    return materials;
    }

    public async Task<IEnumerable<CourseMaterial>> GetMaterialsByUploaderAsync(int uploaderId)
    {
        var materials = await _context.CourseMaterials
            .Include(m => m.Downloads)
            .Include(m => m.Course)
            .Where(m => m.UploaderId == uploaderId)
            .ToListAsync();

        return materials;
    }

    public async Task RecordDownloadAsync(int materialId, int studentId)
    {
    // Check if material exists
    var material = await _context.CourseMaterials
        .FirstOrDefaultAsync(m => m.Id == materialId);

    if (material == null)
    {
        throw new ArgumentException("Course material not found", nameof(materialId));
    }

    // Check if student exists
    var student = await _context.Users
        .FirstOrDefaultAsync(s => s.Id == studentId);

    if (student == null)
    {
        throw new ArgumentException("Student not found", nameof(studentId));
    }

        var downLoadRecord = new CourseMaterialDownload
        {
            CourseMaterialId = materialId,
            StudentId = studentId,
            DownloadDate = DateTime.Now
        };

        await _context.CourseMaterialDownloads.AddAsync(downLoadRecord);
        
        await _context.SaveChangesAsync();
    }

    public async Task UpdateMaterialAsync(int materialId, string title, string description)
    {
        var material = _context.CourseMaterials.Find(materialId);

        if (material != null)
        {
            throw new ArgumentException("Course material not found", nameof(materialId));
        }
            material.Title = title;
            material.Description = description;
            material.UploadDate = DateTime.Now;

            _context.CourseMaterials.Update(material);

         await _context.SaveChangesAsync();
    }

    public async Task DeleteMaterialAsync(int materialId)
    {
        var material = await _context.CourseMaterials
        .Include(m => m.Downloads)
        .FirstOrDefaultAsync(m => m.Id == materialId);

        if (material == null)
        {
            throw new ArgumentException("Course material not found", nameof(materialId));
        }

        // Remove all download records first
        // if (material.Downloads != null && material.Downloads.Any())
        // {
        //     _context.CourseMaterialDownloads.RemoveRange(material.Downloads);
        // }

        // Remove the material
        _context.CourseMaterials.Remove(material);
        await _context.SaveChangesAsync();
    
    }
}
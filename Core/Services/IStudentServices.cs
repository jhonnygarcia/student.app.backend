using System.Threading.Tasks;
using Core.Dto;
using Core.Dto.Base;
using Core.Dto.Class;
using Core.Dto.Student;

namespace Core.Services
{
    public interface IStudentServices
    {
        Task<PagedListDto<StudentDto>> GetFilteredAsync(QueryStudentDto query);
        Task<StudentDto> NewAsync(CreateOrEditStudentDto model);
        Task<bool> EditAsync(int id, CreateOrEditStudentDto model);
        Task<bool> DeleteAsync(int id);
        Task<StudentDto> GetSingleAsync(int id);
        Task<ClassDto[]> GetClassesAsync(int studentId);
    }
}

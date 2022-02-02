using System.Threading.Tasks;
using Core.Dto.Base;
using Core.Dto.Class;
using Core.Dto.Student;

namespace Core.Services
{
    public interface IClassServices
    {
        Task<PagedListDto<ClassDto>> GetFilteredAsync(QueryClassDto query);
        Task<ClassDto> NewAsync(CreateOrEditClassDto model);
        Task<bool> EditAsync(int id, CreateOrEditClassDto model);
        Task<bool> DeleteAsync(int id);
        Task<ClassDto> GetSingleAsync(int id);
        Task<StudentDto[]> GetStudentsAsync(int classId);
        Task<int[]> AddStudents(int classId, int[] studentIds);
    }
}

using System.Threading.Tasks;
using Core.Dto.Base;
using Core.Dto.Teacher;

namespace Core.Services
{
    public interface ITeacherServices
    {
        Task<PagedListDto<TeacherDto>> GetFilteredAsync(PagedQueryDto query);
        Task<TeacherDto> NewAsync(CreateOrEditTeacherDto model);
        Task<bool> EditAsync(int id, CreateOrEditTeacherDto model);
        Task<bool> DeleteAsync(int id);
        Task<TeacherDto> GetSingleAsync(int id);
    }
}

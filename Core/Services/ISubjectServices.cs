using System.Threading.Tasks;
using Core.Dto.Base;
using Core.Dto.Subject;

namespace Core.Services
{
    public interface ISubjectServices
    {
        Task<PagedListDto<SubjectDto>> GetFilteredAsync(PagedQueryDto query);
        Task<SubjectDto> NewAsync(CreateOrEditSubjectDto model);
        Task<bool> EditAsync(int id, CreateOrEditSubjectDto model);
        Task<bool> DeleteAsync(int id);
        Task<SubjectDto> GetSingleAsync(int id);
    }
}

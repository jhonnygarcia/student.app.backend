using System.Threading.Tasks;
using Core.Dto.Base;
using Core.Dto.Score;

namespace Core.Services
{
    public interface IScoreServices
    {
        Task<PagedListDto<ScoreDto>> GetFilteredAsync(PagedQueryDto query);
        Task<ScoreDto> GetSingleAsync(int id);
        Task<ScoreDto> NewAsync(CreateOrEditScoreDto model);
        Task<bool> EditAsync(int id, CreateOrEditScoreDto model);
        Task<bool> DeleteAsync(int id);
        Task<BetterFiveStudensDto> GetBetterFiveStudens(string subject);
        Task<BetterTenStudensDto> GetBetterTenStudens(int teacherId);
        Task GenerateDataTest();
        Task CleanAllData();
    }
}

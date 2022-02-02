using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.DbModel.Contexts;
using Core.DbModel.Entities;
using Core.Dto.Base;
using Core.Dto.Score;
using Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Impl
{
    public class ScoreServices : IScoreServices
    {
        private readonly StudentContext _context;
        private readonly IMapper _mapper;
        public ScoreServices(StudentContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedListDto<ScoreDto>> GetFilteredAsync(PagedQueryDto model)
        {
            var query = _context.Scores.AsQueryable();
            if (!string.IsNullOrWhiteSpace(model.Search))
            {
                query = query.Where(s => s.Description.Contains(model.Search));
            }

            var totalElements = await query.CountAsync();
            if (model.Page.HasValue && model.PerPage.HasValue)
            {
                query = query
                    .OrderByDescending(s => s.Qualification)
                    .Skip((model.Page.Value - 1) * model.PerPage.Value)
                    .Take(model.PerPage.Value);
            }

            var elements = await query.ToArrayAsync();
            var result = _mapper.Map<ScoreDto[]>(elements);

            return new PagedListDto<ScoreDto>
            {
                Data = result,
                TotalElements = totalElements
            };
        }

        public async Task<ScoreDto> NewAsync(CreateOrEditScoreDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Description))
            {
                throw new BadRequestException($"{model.Description} is required");
            }

            if (model.Qualification is null or < 0)
            {
                throw new BadRequestException($"the {nameof(model.Qualification)} must be greater than zero and is required");
            }

            if (!model.StudentId.HasValue)
            {
                throw new BadRequestException($"{model.StudentId} is required");
            }
            if (!model.SubjectId.HasValue)
            {
                throw new BadRequestException($"{model.SubjectId} is required");
            }
            if (!model.TeacherId.HasValue)
            {
                throw new BadRequestException($"{model.TeacherId} is required");
            }

            var student = await _context.Students.FindAsync(model.StudentId);
            if (student == null)
            {
                throw new BadRequestException($"{nameof(model.StudentId)} is invalid, " +
                                              "dont's exists none Student");
            }

            var subject = await _context.Subjects.FindAsync(model.SubjectId);
            if (subject == null)
            {
                throw new BadRequestException($"{nameof(model.SubjectId)} is invalid, " +
                                              "dont's exists none Subject");
            }

            var teacher = await _context.Teachers.FindAsync(model.TeacherId);
            if (teacher == null)
            {
                throw new BadRequestException($"{nameof(model.TeacherId)} is invalid, " +
                                              "dont's exists none Teacher");
            }

            var score = new Score
            {
                Description = model.Description,
                Qualification = model.Qualification.Value,
                Subject = subject,
                Student = student,
                Teacher = teacher
            };
            _context.Scores.Add(score);
            await _context.SaveChangesAsync();

            var res = _mapper.Map<ScoreDto>(score);
            return res;
        }

        public async Task<ScoreDto> GetSingleAsync(int id)
        {
            var clas = await _context.Scores.FindAsync(id);
            if (clas == null)
            {
                throw new NotFoundException(nameof(Score), id);
            }
            return _mapper.Map<ScoreDto>(clas);
        }
        public async Task<bool> EditAsync(int id, CreateOrEditScoreDto model)
        {
            var score = await _context.Scores.FindAsync(id);
            if (score == null)
            {
                throw new NotFoundException(nameof(Score), id);
            }

            if (!string.IsNullOrWhiteSpace(model.Description))
                score.Description = model.Description;

            if (model.Qualification.HasValue)
            {
                if (model.Qualification.Value < 0)
                {
                    throw new BadRequestException($"the {nameof(model.Qualification)} " +
                                                  $"must be greater than zero");
                }
                score.Qualification = model.Qualification.Value;
            }
            
            if (model.SubjectId.HasValue)
            {
                var newSubject = await _context.Subjects.FindAsync(model.SubjectId);
                if (newSubject == null)
                {
                    throw new BadRequestException($"{nameof(model.SubjectId)} is invalid, don't exist");
                }
                score.Subject = newSubject;
            }

            if (model.StudentId.HasValue)
            {
                var newStudent = await _context.Students.FindAsync(model.StudentId);
                if (newStudent == null)
                {
                    throw new BadRequestException($"{nameof(model.StudentId)} is invalid, don't exist");
                }
                score.Student = newStudent;
            }

            if (model.TeacherId.HasValue)
            {
                var newTeacher = await _context.Teachers.FindAsync(model.TeacherId);
                if (newTeacher == null)
                {
                    throw new BadRequestException($"{nameof(model.TeacherId)} is invalid, don't exist");
                }
                score.Teacher = newTeacher;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var clas = await _context.Scores.FindAsync(id);
            if (clas == null)
            {
                throw new NotFoundException(nameof(Score), id);
            }

            _context.Scores.Remove(clas);

            await _context.SaveChangesAsync();

            return true;
        }

    }
}

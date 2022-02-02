using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.DbModel.Contexts;
using Core.DbModel.Entities;
using Core.Dto.Base;
using Core.Dto.Class;
using Core.Dto.Subject;
using Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Impl
{
    public class SubjectServices : ISubjectServices
    {
        private readonly StudentContext _context;
        private readonly IMapper _mapper;
        public SubjectServices(StudentContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedListDto<SubjectDto>> GetFilteredAsync(PagedQueryDto model)
        {
            var query = _context.Subjects.AsQueryable();
            if (!string.IsNullOrWhiteSpace(model.Search))
            {
                query = query.Where(s => s.Name.Contains(model.Search));
            }

            var totalElements = await query.CountAsync();
            if (model.Page.HasValue && model.PerPage.HasValue)
            {
                query = query
                    .OrderBy(s => s.Name)
                    .Skip((model.Page.Value - 1) * model.PerPage.Value)
                    .Take(model.PerPage.Value);
            }

            var elements = await query.ToArrayAsync();
            var result = _mapper.Map<SubjectDto[]>(elements);

            return new PagedListDto<SubjectDto>
            {
                Data = result,
                TotalElements = totalElements
            };
        }

        public async Task<SubjectDto> NewAsync(CreateOrEditSubjectDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new BadRequestException($"{model.Name} is required");
            }

            if (string.IsNullOrWhiteSpace(model.Semester))
            {
                throw new BadRequestException($"{model.Semester} is required");
            }

            if (!model.TeacherId.HasValue)
            {
                throw new BadRequestException($"{model.TeacherId} is required");
            }

            var teacher = await _context.Teachers.FindAsync(model.TeacherId);
            if (teacher == null)
            {
                throw new BadRequestException($"{nameof(model.TeacherId)} is invalid, " +
                                              "dont's exists none Teacher");
            }


            var existsSubjectAndSemester = await _context.Subjects.AnyAsync(s =>
                s.Name == model.Name && s.Semester == model.Semester);

            if (existsSubjectAndSemester)
            {
                throw new ValidationException("A subject with the same " +
                                              "semester is already registered");
            }

            var subject = new Subject
            {
                Name = model.Name,
                Description = model.Description,
                Semester = model.Semester,
                Teacher = teacher
            };
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            var res = _mapper.Map<SubjectDto>(subject);
            return res;
        }

        public async Task<SubjectDto> GetSingleAsync(int id)
        {
            var clas = await _context.Classes.FindAsync(id);
            if (clas == null)
            {
                throw new NotFoundException(nameof(SubjectDto), id);
            }
            return _mapper.Map<SubjectDto>(clas);
        }
        public async Task<bool> EditAsync(int id, CreateOrEditSubjectDto model)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                throw new NotFoundException(nameof(Class), id);
            }

            var name = !string.IsNullOrWhiteSpace(model.Name) ? model.Name : subject.Name;
            var semester = !string.IsNullOrWhiteSpace(model.Semester) ? model.Semester : subject.Semester;

            if (!string.IsNullOrWhiteSpace(model.Name))
                subject.Name = model.Name;

            if (!string.IsNullOrWhiteSpace(model.Semester))
                subject.Semester = model.Semester;

            if (!string.IsNullOrWhiteSpace(model.Description))
                subject.Description = model.Description;

            if (model.TeacherId.HasValue)
            {
                var newTeacher = await _context.Teachers.FindAsync(model.TeacherId);
                if (newTeacher == null)
                {
                    throw new BadRequestException($"{nameof(model.TeacherId)} is invalid, don't exist");
                }
                subject.Teacher = newTeacher;
            }


            var existsSubjectAndSemester = await _context.Subjects.AnyAsync(s =>
                s.Id != subject.Id && s.Name == name && s.Semester == semester);

            if (existsSubjectAndSemester)
            {
                throw new ValidationException("A subject with the same " +
                                              "semester is already registered");
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                throw new NotFoundException(nameof(Subject), id);
            }

            if (subject.Classes.Any())
                throw new ValidationException("the subject has classes, can't be erased");

            if (subject.Scores.Any())
                throw new ValidationException("the subject has scores, can't be erased");

            _context.Subjects.Remove(subject);

            await _context.SaveChangesAsync();

            return true;
        }

    }
}

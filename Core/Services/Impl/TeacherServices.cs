using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.DbModel.Contexts;
using Core.DbModel.Entities;
using Core.Dto.Base;
using Core.Dto.Teacher;
using Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Impl
{
    public class TeacherServices : ITeacherServices
    {
        private readonly StudentContext _context;
        private readonly IMapper _mapper;
        public TeacherServices(StudentContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedListDto<TeacherDto>> GetFilteredAsync(PagedQueryDto model)
        {
            var query = _context.Teachers.AsQueryable();
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
            var result = _mapper.Map<TeacherDto[]>(elements);

            return new PagedListDto<TeacherDto>
            {
                Data = result,
                TotalElements = totalElements
            };
        }

        public async Task<TeacherDto> NewAsync(CreateOrEditTeacherDto model)
        {
            if (string.IsNullOrWhiteSpace(model.LastName) || string.IsNullOrWhiteSpace(model.Name))
            {
                throw new BadRequestException("name and last name is required");
            }

            var Teacher = _mapper.Map<Teacher>(model);
            _context.Teachers.Add(Teacher);
            await _context.SaveChangesAsync();

            return _mapper.Map<TeacherDto>(Teacher);
        }

        public async Task<TeacherDto> GetSingleAsync(int id)
        {
            var Teacher = await _context.Teachers.FindAsync(id);
            if (Teacher == null)
            {
                throw new NotFoundException(nameof(Teacher), id);
            }
            return _mapper.Map<TeacherDto>(Teacher);
        }
        public async Task<bool> EditAsync(int id, CreateOrEditTeacherDto model)
        {
            var Teacher = await _context.Teachers.FindAsync(id);
            if (Teacher == null)
            {
                throw new NotFoundException(nameof(Teacher), id);
            }

            if (!string.IsNullOrWhiteSpace(model.Name))
                Teacher.Name = model.Name;

            if (!string.IsNullOrWhiteSpace(model.LastName))
                Teacher.LastName = model.LastName;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var Teacher = await _context.Teachers.FindAsync(id);
            if (Teacher == null)
            {
                throw new NotFoundException(nameof(Teacher), id);
            }

            if (Teacher.Scores.Any())
                throw new ValidationException("the teacher has scores, can't be erased");

            if (Teacher.Subject.Any())
                throw new ValidationException("the teacher has subjects, can't be erased");

            _context.Teachers.Remove(Teacher);

            await _context.SaveChangesAsync();

            return true;
        }

    }
}

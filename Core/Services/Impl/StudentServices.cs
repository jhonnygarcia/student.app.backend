using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.DbModel.Contexts;
using Core.DbModel.Entities;
using Core.Dto.Base;
using Core.Dto.Class;
using Core.Dto.Student;
using Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Impl
{
    public class StudentServices : IStudentServices
    {
        private readonly StudentContext _context;
        private readonly IMapper _mapper;
        public StudentServices(StudentContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedListDto<StudentDto>> GetFilteredAsync(QueryStudentDto model)
        {
            var query = _context.Students.AsQueryable();
            if (!string.IsNullOrWhiteSpace(model.Search))
            {
                query = query.Where(s => (s.Name + " " + s.LastName).Contains(model.Search));
            }

            if (!string.IsNullOrWhiteSpace(model.ClassName))
            {
                query = query.Where(s => s.Classes.Any(c => c.Name.Contains(model.ClassName)));
            }

            if (!string.IsNullOrWhiteSpace(model.SubjectName))
            {
                query = query.Where(s => s.Classes.Any(c => c.Subject.Name.Contains(model.SubjectName)));
            }

            if (!string.IsNullOrWhiteSpace(model.TeacherName))
            {
                query = query.Where(s => s.Classes.Any(c =>
                    (c.Subject.Teacher.Name + " " + c.Subject.Teacher.LastName).Contains(model.TeacherName)));
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
            var result = _mapper.Map<StudentDto[]>(elements);

            return new PagedListDto<StudentDto>
            {
                Data = result,
                TotalElements = totalElements
            };
        }

        public async Task<StudentDto> NewAsync(CreateOrEditStudentDto model)
        {
            if (string.IsNullOrWhiteSpace(model.LastName) || string.IsNullOrWhiteSpace(model.Name))
            {
                throw new BadRequestException("name and last name is required");
            }

            var student = _mapper.Map<Student>(model);
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return _mapper.Map<StudentDto>(student);
        }

        public async Task<StudentDto> GetSingleAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                throw new NotFoundException(nameof(Student), id);
            }
            return _mapper.Map<StudentDto>(student);
        }
        public async Task<ClassDto[]> GetClassesAsync(int studentId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                throw new NotFoundException(nameof(Student), studentId);
            }

            var classes = student.Classes.ToArray();
            var res = _mapper.Map<ClassDto[]>(classes);
            return res;
        }

        public async Task<bool> EditAsync(int id, CreateOrEditStudentDto model)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                throw new NotFoundException(nameof(Student), id);
            }

            if (!string.IsNullOrWhiteSpace(model.Name))
                student.Name = model.Name;

            if (!string.IsNullOrWhiteSpace(model.LastName))
                student.LastName = model.LastName;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                throw new NotFoundException(nameof(Student), id);
            }

            if (student.Classes.Any())
                throw new ValidationException("the student has classes, can't be erased");

            _context.Students.Remove(student);

            await _context.SaveChangesAsync();

            return true;
        }

    }
}

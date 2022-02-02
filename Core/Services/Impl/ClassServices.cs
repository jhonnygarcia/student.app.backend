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
    public class ClassServices : IClassServices
    {
        private readonly StudentContext _context;
        private readonly IMapper _mapper;
        public ClassServices(StudentContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedListDto<ClassDto>> GetFilteredAsync(QueryClassDto model)
        {
            var query = _context.Classes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(model.Search))
            {
                query = query.Where(c => c.Name.Contains(model.Search));
            }

            if (!string.IsNullOrWhiteSpace(model.TeacherName))
            {
                query = query.Where(c => (c.Subject.Teacher.Name + " " + 
                                          c.Subject.Teacher.LastName).Contains(model.TeacherName));
            }

            if (!string.IsNullOrWhiteSpace(model.SubjectName))
            {
                query = query.Where(c => c.Subject.Name.Contains(model.SubjectName));
            }

            if (!string.IsNullOrWhiteSpace(model.StudentName))
            {
                query = query.Where(c => c.Students.Any(student =>
                    (student.Name + " " + student.LastName).Contains(model.StudentName)));
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
            var result = _mapper.Map<ClassDto[]>(elements);

            return new PagedListDto<ClassDto>
            {
                Data = result,
                TotalElements = totalElements
            };
        }

        public async Task<ClassDto> NewAsync(CreateOrEditClassDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new BadRequestException($"{model.Name} is required");
            }

            if (!model.SubjectId.HasValue)
            {
                throw new BadRequestException($"{model.SubjectId} is required");
            }

            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == model.SubjectId);
            if (subject == null)
            {
                throw new BadRequestException($"{nameof(model.SubjectId)} is invalid, " +
                                              "dont's exists none Subject");
            }

            var clas = new Class
            {
                Name = model.Name,
                Description = model.Description,
                Subject = subject
            };
            _context.Classes.Add(clas);
            await _context.SaveChangesAsync();

            var res = _mapper.Map<ClassDto>(clas);
            return res;
        }

        public async Task<ClassDto> GetSingleAsync(int id)
        {
            var clas = await _context.Classes.FindAsync(id);
            if (clas == null)
            {
                throw new NotFoundException(nameof(Class), id);
            }
            return _mapper.Map<ClassDto>(clas);
        }

        public async Task<StudentDto[]> GetStudentsAsync(int classId)
        {
            var clas = await _context.Classes.FindAsync(classId);
            if (clas == null)
            {
                throw new NotFoundException(nameof(Class), classId);
            }

            var students = clas.Students.ToArray();
            var res = _mapper.Map<StudentDto[]>(students);
            return res;
        }
        public async Task<int[]> AddStudents(int classId, int[] studentIds)
        {
            var clas = await _context.Classes.FindAsync(classId);
            if (clas == null)
            {
                throw new NotFoundException(nameof(Class), classId);
            }

            var currentStudentsInClass = clas.Students.Select(s => s.Id).ToArray();

            var students = await _context.Students.Where(s =>
                studentIds.Contains(s.Id)).ToArrayAsync();
            foreach (var student in students)
            {
                if(currentStudentsInClass.Contains(student.Id))
                    continue;

                clas.Students.Add(student);
            }
            await _context.SaveChangesAsync();

            var notExists = studentIds.Where(value => students.All(s => s.Id != value)).ToArray();
            return notExists;
        }
        public async Task<bool> EditAsync(int id, CreateOrEditClassDto model)
        {
            var clas = await _context.Classes.FindAsync(id);
            if (clas == null)
            {
                throw new NotFoundException(nameof(Class), id);
            }

            if (!string.IsNullOrWhiteSpace(model.Name))
                clas.Name = model.Name;

            if (!string.IsNullOrWhiteSpace(model.Description))
                clas.Description = model.Description;

            if (model.SubjectId.HasValue)
            {
                var newSubject = await _context.Subjects.FindAsync(model.SubjectId);
                if (newSubject == null)
                {
                    throw new BadRequestException($"{nameof(model.SubjectId)} is invalid, don't exist");
                }
                clas.Subject = newSubject;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var clas = await _context.Classes.FindAsync(id);
            if (clas == null)
            {
                throw new NotFoundException(nameof(Class), id);
            }

            if (clas.Students.Any())
                throw new ValidationException("the class has students, can't be erased");

            _context.Classes.Remove(clas);

            await _context.SaveChangesAsync();

            return true;
        }

    }
}

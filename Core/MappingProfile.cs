using AutoMapper;
using Core.DbModel.Entities;
using Core.Dto.Class;
using Core.Dto.Score;
using Core.Dto.Student;
using Core.Dto.Subject;
using Core.Dto.Teacher;

namespace Core
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Student, StudentDto>();
            CreateMap<CreateOrEditStudentDto, Student>();

            CreateMap<Teacher, TeacherDto>();
            CreateMap<CreateOrEditTeacherDto, Teacher>();

            CreateMap<Class, ClassDto>()
                .ForMember(des => des.Subject, act => act.MapFrom(src => src != null
                    ? new SubjectDto
                    {
                        Id = src.Subject.Id,
                        Description = src.Subject.Description,
                        Name = src.Subject.Name,
                        Semester = src.Subject.Semester
                    }
                    : null));

            CreateMap<Subject, SubjectDto>()
                .ForMember(des => des.Teacher, act => act.Ignore());

            CreateMap<Score, ScoreDto>()
                .ForMember(des => des.Subject, act => act.MapFrom(src => src != null
                    ? new SubjectDto
                    {
                        Id = src.Subject.Id,
                        Description = src.Subject.Description,
                        Name = src.Subject.Name,
                        Semester = src.Subject.Semester
                    }
                    : null))
                .ForMember(des => des.Student, act => act.MapFrom(src => src != null
                    ? new StudentDto
                    {
                        Id = src.Student.Id,
                        Name = src.Student.Name,
                        LastName = src.Student.LastName
                    }
                    : null))
                .ForMember(des => des.Teacher, act => act.MapFrom(src => src != null
                    ? new TeacherDto
                    {
                        Id = src.Teacher.Id,
                        Name = src.Teacher.Name,
                        LastName = src.Teacher.LastName
                    }
                    : null));
        }
    }
}
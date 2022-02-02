using AutoMapper;
using Core.Dto.Class;
using Core.Dto.Score;
using Core.Dto.Student;
using Core.Dto.Subject;
using Core.Dto.Teacher;
using WebApplication.Dto.RequestDto.Class;
using WebApplication.Dto.RequestDto.Score;
using WebApplication.Dto.RequestDto.Student;
using WebApplication.Dto.RequestDto.Subject;
using WebApplication.Dto.RequestDto.Teacher;
using WebApplication.Dto.ResponseDto.Class;
using WebApplication.Dto.ResponseDto.Score;
using WebApplication.Dto.ResponseDto.Student;
using WebApplication.Dto.ResponseDto.Subject;
using WebApplication.Dto.ResponseDto.Teacher;

namespace WebApplication
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<StudentDto, StudentResDto>();
            CreateMap<CreateStudentReqDto, CreateOrEditStudentDto>();

            CreateMap<TeacherDto, TeacherResDto>();
            CreateMap<CreateTeacherReqDto, CreateOrEditTeacherDto>();

            CreateMap<SubjectDto, SubjectResDto>()
                .ForMember(des => des.Teacher, act => act.MapFrom(src => src != null
                    ? new TeacherResDto
                    {
                        Id = src.Teacher.Id,
                        Name = src.Teacher.Name,
                        LastName = src.Teacher.Name
                    }
                    : null));
            CreateMap<CreateSubjectReqDto, CreateOrEditSubjectDto>();


            CreateMap<ClassDto, ClassResDto>()
                .ForMember(des => des.Subject, act => act.MapFrom(src => src != null
                    ? new SubjectResDto
                    {
                        Id = src.Subject.Id,
                        Name = src.Subject.Name,
                        Description = src.Subject.Description,
                        Semester = src.Subject.Semester
                    }
                    : null));
            CreateMap<CreateClassReqDto, CreateOrEditClassDto>();


            CreateMap<ScoreDto, ScoreResDto>()
                .ForMember(des => des.Subject, act => act.MapFrom(src => src != null
                    ? new SubjectResDto
                    {
                        Id = src.Subject.Id,
                        Name = src.Subject.Name,
                        Description = src.Subject.Description,
                        Semester = src.Subject.Semester
                    }
                    : null))
                .ForMember(des => des.Teacher, act => act.MapFrom(src => src != null
                    ? new TeacherResDto
                    {
                        Id = src.Teacher.Id,
                        Name = src.Teacher.Name,
                        LastName = src.Teacher.LastName
                    }
                    : null))
                .ForMember(des => des.Student, act => act.MapFrom(src => src != null
                    ? new StudentResDto
                    {
                        Id = src.Student.Id,
                        Name = src.Student.Name,
                        LastName = src.Student.LastName
                    }
                    : null));
            CreateMap<CreateScoreReqDto, CreateOrEditScoreDto>();
        }
    }
}
using System.Collections.Generic;
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
        private const int SCORES_COUNT = 4;
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

            if (model.Qualification is null)
            {
                throw new BadRequestException($"the {nameof(model.Qualification)} is required");
            }

            if (model.Qualification.Value < 0 || model.Qualification > 100)
            {
                throw new BadRequestException("The score must be between 0 and 100");
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

            var countScoresByStudent = await _context.Scores
                .Where(s => s.Student.Id == student.Id && s.Subject.Id == subject.Id).CountAsync();

            if (countScoresByStudent == SCORES_COUNT)
            {
                throw new ValidationException("The 4 qualifications of this same subject and " +
                                              "semester have already been registered for this same student");
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
                if (model.Qualification.Value < 0 || model.Qualification > 100)
                {
                    throw new BadRequestException($"The score must be between 0 and 100");
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

        public async Task<BetterFiveStudensDto> GetBetterFiveStudens(string subjectName)
        {
            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Name == subjectName);
            if (subject == null)
            {
                throw new NotFoundException(nameof(Subject), subjectName);
            }

            var query = _context.Scores.Where(s => s.Subject.Name == subjectName);


            var plainData = await query.Select(p => new
            {
                p.Subject.Semester,
                StudentId = p.Student.Id,
                StudentName = p.Student.Name,
                StudentLastName = p.Student.LastName,
                p.Qualification
            }).ToListAsync();

            var group = plainData.GroupBy(s => s.Semester).Select(g => new
            {
                Semester = g.Key,
                Scores = g.Select(s => new
                {
                    Student = new
                    {
                        Id = s.StudentId,
                        Name = s.StudentName,
                        LastName = s.StudentLastName
                    },
                    s.Qualification
                })
            }).ToList();

            var result = new BetterFiveStudensDto
            {
                SubjectId = subject.Id,
                SubjectName = subject.Name,
                Semesters = group.Select(res => new SemesterBetterFive
                {
                    Semester = res.Semester,
                    Students = res.Scores.GroupBy(score => score.Student.Id).Select(scoreGroup =>
                    {
                        var first = scoreGroup.First().Student;
                        return new QualificationBetterFive
                        {
                            Id = first.Id,
                            Name = first.Name,
                            LastName = first.LastName,
                            Score = scoreGroup.Sum(e => e.Qualification) / SCORES_COUNT
                        };
                    }).OrderByDescending(s => s.Score).Take(5).ToArray()
                }).ToArray()
            };

            return result;
        }
        public async Task<BetterTenStudensDto> GetBetterTenStudens(int teacherId)
        {
            var teacher = await _context.Teachers.FindAsync(teacherId);
            if (teacher == null)
            {
                throw new NotFoundException(nameof(Teacher), teacherId);
            }

            var query = _context.Scores.Where(s => s.Teacher.Id == teacherId);

            var plainData = await query.Select(p => new
            {
                SubjectId = p.Subject.Id,
                SubjectName = p.Subject.Name,
                SubjectSemester = p.Subject.Semester,

                StudentId = p.Student.Id,
                StudentName = p.Student.Name,
                StudentLastName = p.Student.LastName,
                p.Qualification
            }).ToListAsync();

            var group = plainData.GroupBy(s => s.SubjectName).Select(g => new
            {
                SubjectName = g.Key,
                Scores = g.Select(s => new
                {
                    Semester = s.SubjectSemester,
                    Student = new
                    {
                        Id = s.StudentId,
                        Name = s.StudentName,
                        LastName = s.StudentLastName
                    },
                    s.Qualification
                })
            }).ToList();

            var result = new BetterTenStudensDto
            {
                TeacherId = teacher.Id,
                TeacherName = teacher.Name + " " + teacher.LastName,
                Subjects = group.Select(res => new SubjectBetterTenDto
                {
                    SubjectName = res.SubjectName,
                    Semesters = res.Scores.GroupBy(score => score.Semester).Select(scoreGroup =>
                    {
                        var semester = scoreGroup.First().Semester;
                        return new SemesterBetterTenDto
                        {
                            Semester = semester,
                            Students = scoreGroup
                                .GroupBy(g => g.Student.Id)
                                .Select(s =>
                                {
                                    var firstStudent = s.First();
                                    return new QualificationBetterTen
                                    {
                                        Id = firstStudent.Student.Id,
                                        Name = firstStudent.Student.Name,
                                        LastName = firstStudent.Student.LastName,
                                        Score = s.Sum(e => e.Qualification) / SCORES_COUNT
                                    };
                                })
                                .OrderByDescending(o => o.Score)
                                .Take(10)
                                .ToArray()
                        };
                    }).ToArray()
                }).ToArray()
            };

            return result;
        }

        public async Task GenerateDataTest()
        {
            var student1 = new Student
            {
                Name = "Alberto",
                LastName = "Gomez"
            };
            var student2 = new Student
            {
                Name = "Lila",
                LastName = "Espinoza"
            };
            var student3 = new Student
            {
                Name = "Daniel",
                LastName = "Lopez"
            };
            var student4 = new Student
            {
                Name = "Marco",
                LastName = "Mendez"
            };
            var student5 = new Student
            {
                Name = "Yanis",
                LastName = "Frometa"
            };
            var student6 = new Student
            {
                Name = "Erick",
                LastName = "Bauer"
            };
            var student7 = new Student
            {
                Name = "Rene",
                LastName = "Morales"
            };
            var student8 = new Student
            {
                Name = "Buby",
                LastName = "Davalos"
            };
            var student9 = new Student
            {
                Name = "Luis",
                LastName = "Vega"
            };
            var student10 = new Student
            {
                Name = "Cristian",
                LastName = "Flores"
            };
            var student11 = new Student
            {
                Name = "Milenka",
                LastName = "Flores"
            };
            var student12 = new Student
            {
                Name = "Pamela",
                LastName = "Flores"
            };
            var student13 = new Student
            {
                Name = "Victor",
                LastName = "Lopez"
            };
            var student14 = new Student
            {
                Name = "Jorge",
                LastName = "Vasquez"
            };

            _context.Students.AddRange(student1, student2, student3, student4,
                student5, student6, student7, student8, student9, student10,
                student11, student12, student13, student14);
            await _context.SaveChangesAsync();

            var teacher1 = new Teacher
            {
                Name = "Maria",
                LastName = "Gomez"
            };
            var teacher2 = new Teacher
            {
                Name = "Fernando",
                LastName = "Espinoza"
            };
            var teacher3 = new Teacher
            {
                Name = "Nadia",
                LastName = "Parada"
            };

            _context.Teachers.AddRange(teacher1, teacher2, teacher3);
            await _context.SaveChangesAsync();

            var subject1 = new Subject
            {
                Name = "Cálculo",
                Description = "Cálculo 1",
                Semester = "Primer Semestre",
                Teacher = teacher1
            };
            var subject2 = new Subject
            {
                Name = "Cálculo",
                Description = "Cálculo 2",
                Semester = "Segundo Semestre",
                Teacher = teacher1
            };
            var subject3 = new Subject
            {
                Name = "Cálculo",
                Description = "Cálculo 3",
                Semester = "Tercer Semestre",
                Teacher = teacher1
            };
            var subject4 = new Subject
            {
                Name = "Programación",
                Description = "Programación 1",
                Semester = "Primer Semestre",
                Teacher = teacher2
            };
            var subject5 = new Subject
            {
                Name = "Programación",
                Description = "Programación 2",
                Semester = "Segundo Semestre",
                Teacher = teacher3
            };
            var subject6 = new Subject
            {
                Name = "Programación",
                Description = "Programación 3",
                Semester = "Tercer Semestre",
                Teacher = teacher3
            };
            _context.Subjects.AddRange(subject1, subject2, subject3, subject4, subject5, subject6);
            await _context.SaveChangesAsync();

            var class1 = new Class
            {
                Name = "Clase 1",
                Description = "Descripción clase 1",
                Subject = subject1,
                Students = new List<Student>
                {
                    student1, student2, student3, student4, student5,student6
                }
            };
            var class2 = new Class
            {
                Name = "Clase 2",
                Description = "Descripción clase 2",
                Subject = subject2,
                Students = new List<Student>
                {
                    student4, student5,student6, student7, student8, student9, student10
                }
            };
            var class3 = new Class
            {
                Name = "Clase 3",
                Description = "Descripción clase 3",
                Subject = subject3,
                Students = new List<Student>
                {
                    student1, student2, student3, student4, student5, student6, student7, 
                    student8, student9, student10
                }
            };
            _context.Classes.AddRange(class1, class2, class3);
            await _context.SaveChangesAsync();

            var scores1 = new[]
            {
                new Score
                {
                    Description = "Examen 1",
                    Qualification = 80,
                    Student = student1,
                    Teacher = teacher1,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 2",
                    Qualification = 50,
                    Student = student1,
                    Teacher = teacher1,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 3",
                    Qualification = 60,
                    Student = student1,
                    Teacher = teacher1,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Practíco",
                    Qualification = 70,
                    Student = student1,
                    Teacher = teacher1,
                    Subject = subject1
                },
            };

            var scores2 = new[]
            {
                new Score
                {
                    Description = "Examen 1",
                    Qualification = 50,
                    Student = student2,
                    Teacher = teacher1,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 2",
                    Qualification = 50,
                    Student = student2,
                    Teacher = teacher1,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 3",
                    Qualification = 60,
                    Student = student2,
                    Teacher = teacher1,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Practíco",
                    Qualification = 70,
                    Student = student2,
                    Teacher = teacher1,
                    Subject = subject1
                }
            };

            var scores3 = new[]
            {
                new Score
                {
                    Description = "Examen 1",
                    Qualification = 50,
                    Student = student3,
                    Teacher = teacher2,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 2",
                    Qualification = 50,
                    Student = student3,
                    Teacher = teacher2,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 3",
                    Qualification = 60,
                    Student = student3,
                    Teacher = teacher2,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Practíco",
                    Qualification = 30,
                    Student = student3,
                    Teacher = teacher2,
                    Subject = subject1
                }
            };

            var scores4 = new[]
            {
                new Score
                {
                    Description = "Examen 1",
                    Qualification = 50,
                    Student = student4,
                    Teacher = teacher2,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 2",
                    Qualification = 40,
                    Student = student4,
                    Teacher = teacher2,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 3",
                    Qualification = 33.5,
                    Student = student4,
                    Teacher = teacher2,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Practíco",
                    Qualification = 30,
                    Student = student4,
                    Teacher = teacher2,
                    Subject = subject1
                }
            };

            var scores5 = new[]
            {
                new Score
                {
                    Description = "Examen 1",
                    Qualification = 50,
                    Student = student5,
                    Teacher = teacher2,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 2",
                    Qualification = 40,
                    Student = student5,
                    Teacher = teacher2,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 3",
                    Qualification = 33.5,
                    Student = student5,
                    Teacher = teacher2,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Practíco",
                    Qualification = 30,
                    Student = student5,
                    Teacher = teacher2,
                    Subject = subject1
                }
            };

            var scores6 = new[]
            {
                new Score
                {
                    Description = "Examen 1",
                    Qualification = 50,
                    Student = student6,
                    Teacher = teacher2,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 2",
                    Qualification = 40,
                    Student = student6,
                    Teacher = teacher2,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 3",
                    Qualification = 33.5,
                    Student = student6,
                    Teacher = teacher2,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Practíco",
                    Qualification = 30,
                    Student = student6,
                    Teacher = teacher2,
                    Subject = subject1
                }
            };

            var scores7 = new[]
            {
                new Score
                {
                    Description = "Examen 1",
                    Qualification = 60,
                    Student = student7,
                    Teacher = teacher3,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 2",
                    Qualification = 20,
                    Student = student7,
                    Teacher = teacher3,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 3",
                    Qualification = 33.5,
                    Student = student7,
                    Teacher = teacher3,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Practíco",
                    Qualification = 50,
                    Student = student7,
                    Teacher = teacher3,
                    Subject = subject1
                }
            };

            var scores8 = new[]
            {
                new Score
                {
                    Description = "Examen 1",
                    Qualification = 60,
                    Student = student8,
                    Teacher = teacher3,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 2",
                    Qualification = 20,
                    Student = student8,
                    Teacher = teacher3,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 3",
                    Qualification = 45.5,
                    Student = student8,
                    Teacher = teacher3,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Practíco",
                    Qualification = 40,
                    Student = student8,
                    Teacher = teacher3,
                    Subject = subject1
                }
            };

            var scores9 = new[]
            {
                new Score
                {
                    Description = "Examen 1",
                    Qualification = 60,
                    Student = student9,
                    Teacher = teacher3,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 2",
                    Qualification = 60,
                    Student = student9,
                    Teacher = teacher3,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 3",
                    Qualification = 33.5,
                    Student = student9,
                    Teacher = teacher3,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Practíco",
                    Qualification = 10,
                    Student = student9,
                    Teacher = teacher3,
                    Subject = subject1
                }
            };

            var scores10 = new[]
            {
                new Score
                {
                    Description = "Examen 1",
                    Qualification = 60,
                    Student = student10,
                    Teacher = teacher3,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 2",
                    Qualification = 20,
                    Student = student10,
                    Teacher = teacher3,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 3",
                    Qualification = 33.5,
                    Student = student10,
                    Teacher = teacher3,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Practíco",
                    Qualification = 50,
                    Student = student10,
                    Teacher = teacher3,
                    Subject = subject1
                }
            };

            var scores11 = new[]
            {
                new Score
                {
                    Description = "Examen 1",
                    Qualification = 60,
                    Student = student11,
                    Teacher = teacher3,
                    Subject = subject3
                },
                new Score
                {
                    Description = "Examen 2",
                    Qualification = 70,
                    Student = student11,
                    Teacher = teacher3,
                    Subject = subject3
                },
                new Score
                {
                    Description = "Examen 3",
                    Qualification = 80,
                    Student = student11,
                    Teacher = teacher3,
                    Subject = subject3
                },
                new Score
                {
                    Description = "Practíco",
                    Qualification = 0,
                    Student = student11,
                    Teacher = teacher3,
                    Subject = subject3
                }
            };

            var scores12 = new[]
            {
                new Score
                {
                    Description = "Examen 1",
                    Qualification = 100,
                    Student = student11,
                    Teacher = teacher3,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 2",
                    Qualification = 20,
                    Student = student11,
                    Teacher = teacher3,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 3",
                    Qualification = 10,
                    Student = student11,
                    Teacher = teacher3,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Practíco",
                    Qualification = 20,
                    Student = student11,
                    Teacher = teacher3,
                    Subject = subject1
                }
            };

            var scores13 = new[]
            {
                new Score
                {
                    Description = "Examen 1",
                    Qualification = 10,
                    Student = student13,
                    Teacher = teacher3,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 2",
                    Qualification = 40,
                    Student = student13,
                    Teacher = teacher3,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 3",
                    Qualification = 35,
                    Student = student13,
                    Teacher = teacher3,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Practíco",
                    Qualification = 30,
                    Student = student13,
                    Teacher = teacher3,
                    Subject = subject1
                }
            };

            var scores14 = new[]
            {
                new Score
                {
                    Description = "Examen 1",
                    Qualification = 100,
                    Student = student14,
                    Teacher = teacher3,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 2",
                    Qualification = 100,
                    Student = student14,
                    Teacher = teacher3,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Examen 3",
                    Qualification = 90,
                    Student = student14,
                    Teacher = teacher3,
                    Subject = subject1
                },
                new Score
                {
                    Description = "Practíco",
                    Qualification = 80,
                    Student = student14,
                    Teacher = teacher3,
                    Subject = subject1
                }
            };

            var scores15 = new[]
            {
                new Score
                {
                    Description = "Examen 1",
                    Qualification = 100,
                    Student = student13,
                    Teacher = teacher3,
                    Subject = subject2
                },
                new Score
                {
                    Description = "Examen 2",
                    Qualification = 100,
                    Student = student13,
                    Teacher = teacher3,
                    Subject = subject2
                },
                new Score
                {
                    Description = "Examen 3",
                    Qualification = 90,
                    Student = student13,
                    Teacher = teacher3,
                    Subject = subject2
                },
                new Score
                {
                    Description = "Practíco",
                    Qualification = 80,
                    Student = student13,
                    Teacher = teacher3,
                    Subject = subject2
                }
            };

            var scores16 = new[]
            {
                new Score
                {
                    Description = "Examen 1",
                    Qualification = 100,
                    Student = student13,
                    Teacher = teacher3,
                    Subject = subject4
                },
                new Score
                {
                    Description = "Examen 2",
                    Qualification = 100,
                    Student = student13,
                    Teacher = teacher3,
                    Subject = subject4
                },
                new Score
                {
                    Description = "Examen 3",
                    Qualification = 90,
                    Student = student13,
                    Teacher = teacher3,
                    Subject = subject4
                },
                new Score
                {
                    Description = "Practíco",
                    Qualification = 80,
                    Student = student13,
                    Teacher = teacher3,
                    Subject = subject4
                }
            };

            var scores17 = new[]
            {
                new Score
                {
                    Description = "Examen 1",
                    Qualification = 45,
                    Student = student1,
                    Teacher = teacher3,
                    Subject = subject4
                },
                new Score
                {
                    Description = "Examen 2",
                    Qualification = 20,
                    Student = student1,
                    Teacher = teacher3,
                    Subject = subject4
                },
                new Score
                {
                    Description = "Examen 3",
                    Qualification = 10,
                    Student = student1,
                    Teacher = teacher3,
                    Subject = subject4
                },
                new Score
                {
                    Description = "Practíco",
                    Qualification = 20,
                    Student = student1,
                    Teacher = teacher3,
                    Subject = subject4
                }
            };

            var scores18 = new[]
            {
                new Score
                {
                    Description = "Examen 1",
                    Qualification = 45,
                    Student = student1,
                    Teacher = teacher3,
                    Subject = subject5
                },
                new Score
                {
                    Description = "Examen 2",
                    Qualification = 20,
                    Student = student1,
                    Teacher = teacher3,
                    Subject = subject5
                },
                new Score
                {
                    Description = "Examen 3",
                    Qualification = 10,
                    Student = student1,
                    Teacher = teacher3,
                    Subject = subject5
                },
                new Score
                {
                    Description = "Practíco",
                    Qualification = 20,
                    Student = student1,
                    Teacher = teacher3,
                    Subject = subject5
                }
            };

            var scores19 = new[]
            {
                new Score
                {
                    Description = "Examen 1",
                    Qualification = 15,
                    Student = student2,
                    Teacher = teacher3,
                    Subject = subject5
                },
                new Score
                {
                    Description = "Examen 2",
                    Qualification = 20,
                    Student = student2,
                    Teacher = teacher3,
                    Subject = subject5
                },
                new Score
                {
                    Description = "Examen 3",
                    Qualification = 10,
                    Student = student2,
                    Teacher = teacher3,
                    Subject = subject5
                },
                new Score
                {
                    Description = "Practíco",
                    Qualification = 20,
                    Student = student2,
                    Teacher = teacher3,
                    Subject = subject5
                }
            };

            var scores20 = new[]
            {
                new Score
                {
                    Description = "Examen 1",
                    Qualification = 40,
                    Student = student3,
                    Teacher = teacher3,
                    Subject = subject5
                },
                new Score
                {
                    Description = "Examen 2",
                    Qualification = 20,
                    Student = student3,
                    Teacher = teacher3,
                    Subject = subject5
                },
                new Score
                {
                    Description = "Examen 3",
                    Qualification = 50,
                    Student = student3,
                    Teacher = teacher3,
                    Subject = subject5
                },
                new Score
                {
                    Description = "Practíco",
                    Qualification = 80,
                    Student = student3,
                    Teacher = teacher3,
                    Subject = subject5
                }
            };

            _context.Scores.AddRange(scores1);
            _context.Scores.AddRange(scores2);
            _context.Scores.AddRange(scores3);
            _context.Scores.AddRange(scores4);
            _context.Scores.AddRange(scores5);
            _context.Scores.AddRange(scores6);
            _context.Scores.AddRange(scores7);
            _context.Scores.AddRange(scores8);
            _context.Scores.AddRange(scores9);
            _context.Scores.AddRange(scores10);
            _context.Scores.AddRange(scores11);
            _context.Scores.AddRange(scores12);
            _context.Scores.AddRange(scores13);
            _context.Scores.AddRange(scores14);
            _context.Scores.AddRange(scores15);
            _context.Scores.AddRange(scores16);
            _context.Scores.AddRange(scores17);
            _context.Scores.AddRange(scores18);
            _context.Scores.AddRange(scores19);
            _context.Scores.AddRange(scores20);

            await _context.SaveChangesAsync();
        }

        public async Task CleanAllData()
        {
            var scores = await _context.Scores.ToListAsync();
            _context.Scores.RemoveRange(scores);
            await _context.SaveChangesAsync();

            var classes = await _context.Classes.ToListAsync();
            _context.Classes.RemoveRange(classes);
            await _context.SaveChangesAsync();

            var subjects = await _context.Subjects.ToListAsync();
            _context.Subjects.RemoveRange(subjects);
            await _context.SaveChangesAsync();

            var teachers = await _context.Teachers.ToListAsync();
            _context.Teachers.RemoveRange(teachers);
            await _context.SaveChangesAsync();

            var students = await _context.Students.ToListAsync();
            _context.Students.RemoveRange(students);
            await _context.SaveChangesAsync();
        }
    }
}

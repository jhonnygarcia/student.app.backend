using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Core.Dto.Base;
using Core.Dto.Student;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using WebApplication.Auth.Handlers.ApiKey;
using WebApplication.Auth.Handlers.Oidc;
using WebApplication.Dto;
using WebApplication.Dto.RequestDto.Student;
using WebApplication.Dto.ResponseDto.Class;
using WebApplication.Dto.ResponseDto.Student;

namespace WebApplication.Controllers
{
    [Route("api/v1/students")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    public class StudentController : ControllerBase
    {
        private readonly IStudentServices _studentServices;
        private readonly IMapper _mapper;

        public StudentController(IStudentServices studentServices, IMapper mapper)
        {
            _studentServices = studentServices;
            _mapper = mapper;
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(PagedListDto<StudentDto>))]
        public async Task<ActionResult<PagedListDto<StudentDto>>> GetFiltered(
            string search = null, string className = null, string subjectName = null,
            string teacherName = null,  int? page = null, int? perPage = null)
        {
            var result = await _studentServices.GetFilteredAsync(new QueryStudentDto
            {
                Search = search,
                PerPage = perPage,
                Page = page,

                TeacherName = teacherName,
                SubjectName = subjectName,
                ClassName = className
            });
            return Ok(result);
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(StudentResDto))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ErrorResponse))]
        [Route("{id}")]
        public async Task<ActionResult<StudentResDto>> Get(int id)
        {
            var result = await _studentServices.GetSingleAsync(id);
            var res = _mapper.Map<StudentResDto>(result);
            return Ok(res);
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ClassResDto[]))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ErrorResponse))]
        [Route("{id}/classes")]
        public async Task<ActionResult<StudentResDto>> GetClasses(int id)
        {
            var result = await _studentServices.GetClassesAsync(id);
            var res = _mapper.Map<ClassResDto[]>(result);
            return Ok(res);
        }

        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(ErrorResponse))]
        public async Task<ActionResult<StudentResDto>> New([FromBody] CreateStudentReqDto model)
        {
            var param = _mapper.Map<CreateOrEditStudentDto>(model);
            var result = await _studentServices.NewAsync(param);
            var res = _mapper.Map<StudentResDto>(result);
            return CreatedAtAction("Get", new { id = res.Id }, res);
        }

        [HttpPut]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ErrorResponse))]
        [Route("{id}")]
        public async Task<ActionResult> Edit(int id, [FromBody] CreateStudentReqDto model)
        {
            var param = _mapper.Map<CreateOrEditStudentDto>(model);
            await _studentServices.EditAsync(id, param);
            return Ok();
        }

        [HttpDelete]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(ErrorResponse))]
        [Route("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _studentServices.DeleteAsync(id);
            return Ok();
        }
    }
}

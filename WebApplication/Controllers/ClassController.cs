using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Core.Dto.Base;
using Core.Dto.Class;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using WebApplication.Auth.Handlers.ApiKey;
using WebApplication.Auth.Handlers.Oidc;
using WebApplication.Dto;
using WebApplication.Dto.RequestDto.Class;
using WebApplication.Dto.ResponseDto.Class;
using WebApplication.Dto.ResponseDto.Student;

namespace WebApplication.Controllers
{
    [Route("api/v1/classes")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    public class ClassController : ControllerBase
    {
        private readonly IClassServices _classServices;
        private readonly IMapper _mapper;

        public ClassController(IClassServices studentServices, IMapper mapper)
        {
            _classServices = studentServices;
            _mapper = mapper;
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(PagedListResDto<ClassResDto>))]
        public async Task<ActionResult<PagedListResDto<ClassResDto>>> GetFiltered(string search = null,
            string studentName = null, string subjectName = null,
            string teacherName = null, int? page = null,
            int? perPage = null)
        {
            var result = await _classServices.GetFilteredAsync(new QueryClassDto
            {
                Search = search,
                PerPage = perPage,
                Page = page,

                TeacherName = teacherName,
                SubjectName = subjectName,
                StudentName = studentName
            });
            return Ok(new PagedListResDto<ClassResDto>
            {
                Data = _mapper.Map<ClassResDto[]>(result.Data),
                TotalElements = result.TotalElements
            });
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ErrorResponse))]
        [Route("{id}")]
        public async Task<ActionResult<ClassResDto>> Get(int id)
        {
            var result = await _classServices.GetSingleAsync(id);
            var res = _mapper.Map<ClassResDto>(result);
            return Ok(res);
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ErrorResponse))]
        [Route("{id}/students")]
        public async Task<ActionResult<ClassResDto>> GetStudents(int id)
        {
            var students = await _classServices.GetStudentsAsync(id);
            var res = _mapper.Map<StudentResDto[]>(students);
            return Ok(res);
        }

        [HttpPost]
        [Route("{id}/students")]
        public async Task<ActionResult> GetStudents(int id, [FromBody] int[] studentIds)
        {
            var students = await _classServices.AddStudents(id, studentIds);
            return Ok(new
            {
                InvalidStudentIds = students
            });
        }

        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(ErrorResponse))]
        public async Task<ActionResult<ClassResDto>> New([FromBody] CreateClassReqDto model)
        {
            var param = _mapper.Map<CreateOrEditClassDto>(model);
            var result = await _classServices.NewAsync(param);
            var res = _mapper.Map<ClassResDto>(result);
            return CreatedAtAction("Get", new { id = res.Id }, res);
        }

        [HttpPut]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(ErrorResponse))]
        [Route("{id}")]
        public async Task<ActionResult<ClassDto>> Edit(int id, [FromBody] CreateClassReqDto model)
        {
            var param = _mapper.Map<CreateOrEditClassDto>(model);
            await _classServices.EditAsync(id, param);
            return Ok();
        }

        [HttpDelete]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(ErrorResponse))]
        [Route("{id}")]
        public async Task<ActionResult<ClassDto>> Delete(int id)
        {
            await _classServices.DeleteAsync(id);
            return Ok();
        }
    }
}

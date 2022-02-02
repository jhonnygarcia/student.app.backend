using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Core.Dto.Base;
using Core.Dto.Teacher;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using WebApplication.Auth.Handlers.ApiKey;
using WebApplication.Auth.Handlers.Oidc;
using WebApplication.Dto;
using WebApplication.Dto.RequestDto.Teacher;
using WebApplication.Dto.ResponseDto.Teacher;

namespace WebApplication.Controllers
{
    [Route("api/v1/teachers")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherServices _teacherServices;
        private readonly IMapper _mapper;

        public TeacherController(ITeacherServices studentServices, IMapper mapper)
        {
            _teacherServices = studentServices;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PagedListDto<TeacherDto>>> GetFiltered(string search = null, int? page = null, int? perPage = null)
        {
            var result = await _teacherServices.GetFilteredAsync(new PagedQueryDto
            {
                Search = search,
                PerPage = perPage,
                Page = page
            });
            return Ok(result);
        }
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ErrorResponse))]
        [Route("{id}")]
        public async Task<ActionResult<TeacherResDto>> Get(int id)
        {
            var result = await _teacherServices.GetSingleAsync(id);
            var res = _mapper.Map<TeacherResDto>(result);
            return Ok(res);
        }
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(ErrorResponse))]
        public async Task<ActionResult<TeacherResDto>> New([FromBody] CreateTeacherReqDto model)
        {
            var param = _mapper.Map<CreateOrEditTeacherDto>(model);
            var result = await _teacherServices.NewAsync(param);
            var res = _mapper.Map<TeacherResDto>(result);
            return CreatedAtAction("Get", new { id = res.Id }, res);
        }
        [HttpPut]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ErrorResponse))]
        [Route("{id}")]
        public async Task<ActionResult<TeacherDto>> Edit(int id, [FromBody] CreateTeacherReqDto model)
        {
            var param = _mapper.Map<CreateOrEditTeacherDto>(model);
            await _teacherServices.EditAsync(id, param);
            return Ok();
        }
        [HttpDelete]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(ErrorResponse))]
        [Route("{id}")]
        public async Task<ActionResult<TeacherDto>> Delete(int id)
        {
            await _teacherServices.DeleteAsync(id);
            return Ok();
        }
    }
}

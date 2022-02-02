using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Core.Dto.Base;
using Core.Dto.Subject;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using WebApplication.Auth.Handlers.ApiKey;
using WebApplication.Auth.Handlers.Oidc;
using WebApplication.Dto;
using WebApplication.Dto.RequestDto.Subject;
using WebApplication.Dto.ResponseDto.Subject;

namespace WebApplication.Controllers
{
    [Route("api/v1/subjects")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectServices _subjectServices;
        private readonly IMapper _mapper;

        public SubjectController(ISubjectServices studentServices, IMapper mapper)
        {
            _subjectServices = studentServices;
            _mapper = mapper;
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(PagedListResDto<SubjectResDto>))]
        public async Task<ActionResult<PagedListResDto<SubjectResDto>>> GetFiltered(string search = null, int? page = null, int? perPage = null)
        {
            var result = await _subjectServices.GetFilteredAsync(new PagedQueryDto
            {
                Search = search,
                PerPage = perPage,
                Page = page
            });
            return Ok(new PagedListResDto<SubjectResDto>
            {
                Data = _mapper.Map<SubjectResDto[]>(result.Data),
                TotalElements = result.TotalElements
            });
        }
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ErrorResponse))]
        [Route("{id}")]
        public async Task<ActionResult<SubjectResDto>> Get(int id)
        {
            var result = await _subjectServices.GetSingleAsync(id);
            var res = _mapper.Map<SubjectResDto>(result);
            return Ok(res);
        }
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(ErrorResponse))]
        public async Task<ActionResult<SubjectResDto>> New([FromBody] CreateSubjectReqDto model)
        {
            var param = _mapper.Map<CreateOrEditSubjectDto>(model);
            var result = await _subjectServices.NewAsync(param);
            var res = _mapper.Map<SubjectResDto>(result);
            return CreatedAtAction("Get", new { id = res.Id }, res);
        }
        [HttpPut]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(ErrorResponse))]
        [Route("{id}")]
        public async Task<ActionResult> Edit(int id, [FromBody] CreateSubjectReqDto model)
        {
            var param = _mapper.Map<CreateOrEditSubjectDto>(model);
            await _subjectServices.EditAsync(id, param);
            return Ok();
        }
        [HttpDelete]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(ErrorResponse))]
        [Route("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _subjectServices.DeleteAsync(id);
            return Ok();
        }
    }
}

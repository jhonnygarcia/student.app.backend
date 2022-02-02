using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Core.Dto.Base;
using Core.Dto.Score;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using WebApplication.Auth.Handlers.ApiKey;
using WebApplication.Auth.Handlers.Oidc;
using WebApplication.Dto;
using WebApplication.Dto.RequestDto.Score;
using WebApplication.Dto.ResponseDto.Score;

namespace WebApplication.Controllers
{
    [Route("api/v1/scores")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    public class ScoreController : ControllerBase
    {
        private readonly IScoreServices _scoreServices;
        private readonly IMapper _mapper;

        public ScoreController(IScoreServices studentServices, IMapper mapper)
        {
            _scoreServices = studentServices;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PagedListDto<ScoreResDto>>> GetFiltered(string search = null, int? page = null, int? perPage = null)
        {
            var result = await _scoreServices.GetFilteredAsync(new PagedQueryDto
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
        public async Task<ActionResult<ScoreResDto>> Get(int id)
        {
            var result = await _scoreServices.GetSingleAsync(id);
            var res = _mapper.Map<ScoreResDto>(result);
            return Ok(res);
        }
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(ErrorResponse))]
        public async Task<ActionResult<ScoreResDto>> New([FromBody] CreateScoreReqDto model)
        {
            var param = _mapper.Map<CreateOrEditScoreDto>(model);
            var result = await _scoreServices.NewAsync(param);
            var res = _mapper.Map<ScoreResDto>(result);
            return CreatedAtAction("Get", new { id = res.Id }, res);
        }
        [HttpPut]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(ErrorResponse))]
        [Route("{id}")]
        public async Task<ActionResult> Edit(int id, [FromBody] CreateScoreReqDto model)
        {
            var param = _mapper.Map<CreateOrEditScoreDto>(model);
            await _scoreServices.EditAsync(id, param);
            return Ok();
        }
        [HttpDelete]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ErrorResponse))]
        [Route("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _scoreServices.DeleteAsync(id);
            return Ok();
        }
    }
}

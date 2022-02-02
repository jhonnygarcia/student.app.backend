using System.Net;
using System.Threading.Tasks;
using Core.Dto.Score;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using WebApplication.Auth.Handlers.ApiKey;
using WebApplication.Auth.Handlers.Oidc;
using WebApplication.Dto;

namespace WebApplication.Controllers
{
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    public class ReportController : ControllerBase
    {
        private readonly IScoreServices _scoreServices;

        public ReportController(IScoreServices scoreServices)
        {
            _scoreServices = scoreServices;
        }
        
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(BetterFiveStudensDto))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ErrorResponse))]
        [Route("better-five-by-subject/{subject}")]
        public async Task<ActionResult<BetterFiveStudensDto>> GetBetterFiveBySubject(string subject)
        {
            var result = await _scoreServices.GetBetterFiveStudens(subject);
            return Ok(result);
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(BetterTenStudensDto))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ErrorResponse))]
        [Route("better-ten-by-teacher/{teacherId}")]
        public async Task<ActionResult<BetterTenStudensDto>> GetBetterTenByTeacher(int teacherId)
        {
            var result = await _scoreServices.GetBetterTenStudens(teacherId);
            return Ok(result);
        }
    }
}

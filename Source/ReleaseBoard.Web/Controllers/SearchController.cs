using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReleaseBoard.Application.QueryHandlers;
using ReleaseBoard.Web.Providers;

namespace ReleaseBoard.Web.Controllers
{
    /// <summary>
    /// Api контроллер для поиска.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SearchController : Controller
    {
        private readonly IMediator mediator;
        private readonly IAuthorizedUserProvider authorizedUserProvider;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="mediator"><see cref="IMediator"/>.</param>
        /// <param name="authorizedUserProvider"><see cref="IAuthorizedUserProvider"/>.</param>
        public SearchController(
            IMediator mediator,
            IAuthorizedUserProvider authorizedUserProvider)
        {
            this.mediator = mediator;
            this.authorizedUserProvider = authorizedUserProvider;
        }

        /// <summary>
        /// Производит поиск по дистрибутивам, сборкам, тегам и суффиксам сборок.
        /// </summary>
        /// <param name="filter">Строка фильтра.</param>
        /// <returns><see cref="Task"/>.</returns>
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery][Required]string filter)
        {
            var user = authorizedUserProvider.Get();

            var results = await mediator.Send(new SearchByFilter.Query()
            {
                SearchText = filter,
                UserSid = user.Sid,
                IsAdmin = user.IsAdmin
            });

            return Ok(results);
        }
    }
}

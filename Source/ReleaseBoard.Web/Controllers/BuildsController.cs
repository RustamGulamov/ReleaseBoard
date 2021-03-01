using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReleaseBoard.Application.Interfaces;
using ReleaseBoard.Application.Models.Filters;
using ReleaseBoard.Application.QueryHandlers;
using ReleaseBoard.Domain.Builds.Commands;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.Domain.ValueObjects;
using ReleaseBoard.ReadModels;
using ReleaseBoard.Web.ApiModels.BuildModels;
using ReleaseBoard.Web.ApiModels.SearchBuildsModels;
using ReleaseBoard.Web.Authorization.Security;
using ReleaseBoard.Web.Extensions;
using ReleaseBoard.Web.Factories;
using ReleaseBoard.Web.Providers;

namespace ReleaseBoard.Web.Controllers
{
    /// <summary>
    /// Api контроллер для работы с билдами.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BuildsController : Controller
    {
        private readonly IMapper mapper;
        private readonly IMediator mediator;
        private readonly IBuildSyncService buildSyncService;
        private readonly IAuthorizedUserProvider authorizedUserProvider;
        private readonly IReadOnlyRepository<BuildMatchPatternReadModel> buildMatchPatternReadOnlyRepository;
        private readonly IReadOnlyRepository<BuildReadModel> buildReadOnlyRepository;
        private readonly IReadOnlyRepository<DistributionReadModel> distributionReadOnlyRepository;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="mapper">Маппер типов.</param>
        /// <param name="mediator"><see cref="IMediator"/>.</param>
        /// <param name="buildSyncService">Сервис BuildSync.</param>
        /// <param name="authorizedUserProvider">Провайдер для доступа к текущему пользователю.</param>
        /// <param name="buildMatchPatternReadOnlyRepository">a.</param>
        /// <param name="buildReadOnlyRepository">Репозиторий сборок.</param>
        /// <param name="distributionReadOnlyRepository">.</param>
        public BuildsController(
            IMapper mapper,
            IMediator mediator,
            IBuildSyncService buildSyncService,
            IAuthorizedUserProvider authorizedUserProvider,
            IReadOnlyRepository<BuildMatchPatternReadModel> buildMatchPatternReadOnlyRepository,
            IReadOnlyRepository<BuildReadModel> buildReadOnlyRepository,
            IReadOnlyRepository<DistributionReadModel> distributionReadOnlyRepository
        )
        {
            this.mapper = mapper;
            this.mediator = mediator;
            this.buildSyncService = buildSyncService;
            this.authorizedUserProvider = authorizedUserProvider;
            this.buildMatchPatternReadOnlyRepository = buildMatchPatternReadOnlyRepository;
            this.buildReadOnlyRepository = buildReadOnlyRepository;
            this.distributionReadOnlyRepository = distributionReadOnlyRepository;
        }

        /// <summary>
        /// Получить сборку по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор сборки.</param>
        /// <returns><see cref="Task"/>.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var build = await buildReadOnlyRepository.Query(x => x.Id == id);

            return await AccessBuild(build, b => Ok(b));
        }

        /// <summary>
        /// Получить сборку по местоположению.
        /// </summary>
        /// <param name="location">Путь до сборки.</param>
        /// <param name="sourceType">Тип сборки.</param>
        /// <returns><see cref="Task"/>.</returns>
        [HttpGet]
        [Authorize(Policy = PolicyNames.CanAccessBuildSyncApi)]
        public async Task<IActionResult> Get([Required(AllowEmptyStrings = false)] string location, BuildSourceType sourceType)
        {
            BuildReadModel build = 
                await buildReadOnlyRepository
                    .WithCaseSensitive()
                    .Query(x => x.Location == location && x.SourceType == sourceType);
            
            return Ok(new GetBuildResponse { Build = mapper.Map<BuildDto>(build) });
        }

        /// <summary>
        /// Производит поиск сборок в указанном пути и расположении.
        /// </summary>
        /// <param name="request">Модель запроса на поиск сборок.</param>
        /// <returns><see cref="Task"/>.</returns>
        [HttpGet("[action]")]
        public async Task<ActionResult<List<SearchBuildsResultModel>>> SearchBuilds([FromQuery] SearchBuildsRequestModel request)
        {
            IList<BuildMatchPatternReadModel> allPatterns = await buildMatchPatternReadOnlyRepository.GetAll();
            IList<string> searchPatterns = allPatterns.Select(x => x.Regexp).ToList();

            if (!searchPatterns.Any())
            {
                return NotFound("Couldn't found any regular expressions in DB");
            }

            IResponseMessage response = await buildSyncService.SearchBuilds(request.Path, request.SourceType, searchPatterns);

            return response.ToActionResult<ScanForBuildsListResponse, List<SearchBuildsResultModel>>(
                r => SearchBuildsResultFactory.Create(r, allPatterns));
        }

        /// <summary>
        /// Производит изменение суффиксов сборки.
        /// </summary>
        /// <param name="model">Модель описывающая изменение статуса.</param>
        /// <returns>Результат изменения суффиксов.</returns>
        [HttpPut("[action]")]
        [Authorize]
        public async Task<IActionResult> ChangeBuildState([FromBody] ChangeBuildStateRequestModel model)
        {
            var user = authorizedUserProvider.Get();
            var command = mapper.Map<ChangeBuildState>(model);
            command.User = mapper.Map<User>(user);
            await mediator.Send(command);

            return Ok();
        }

        /// <summary>
        /// Обновляет теги.
        /// </summary>
        /// <param name="model">Модель обновления тегов сборки.</param>
        /// <returns>Результат.</returns>
        [HttpPut("tags")]
        [Authorize]
        public async Task<IActionResult> UpdateBuildTags([FromBody] UpdateBuildTagsModel model)
        {
            var command = mapper.Map<UpdateBuildTags>(model);

            await mediator.Send(command);

            return Ok();
        }

        /// <summary>
        /// Производит фильтр сборок по дистрибутиву.
        /// </summary>
        /// <param name="filter">Модель фильтра сборок по дистрибутиву <see cref="DistributionBuildsFilter"/>.</param>
        /// <returns><see cref="Task"/>.</returns>
        [HttpPost("filter")]
        public async Task<IList<BuildReadModel>> FilterBuilds([FromBody] DistributionBuildsFilter filter)
        {
            var user = authorizedUserProvider.Get();

            return await mediator.Send(new GetBuildsByFilter.Query { Filter = filter, UserSid = user.Sid, IsAdmin = user.IsAdmin });
        }

        /// <summary>
        /// Генерировать пути до новой сборки.
        /// </summary>
        /// <param name="model"><see cref="GenerateNewBuildPathsRequestModel"/>.</param>
        /// <returns>Путь.</returns>
        [HttpGet("generateNewPaths")]
        public async Task<IActionResult> GenerateNewPathsByVersion([FromQuery] GenerateNewBuildPathsRequestModel model)
        {
            Dictionary<BuildSourceType, string[]> result = await mediator.Send(new GetNewBuildPaths.Query { DistributionId = model.DistributionId, VersionNumber = new VersionNumber(model.Number) });

            if (result.IsEmpty())
            {
                return BadRequest("Не удалось построить относительный путь новой сборки дистрибутива.");
            }

            return Ok(result.Select(x => new GenerateNewBuildPathsResponseModel() { BuildSourceType = x.Key, BuildPaths = x.Value }));
        }
    }
}

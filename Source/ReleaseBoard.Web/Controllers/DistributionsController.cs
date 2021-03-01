using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReleaseBoard.Application.Models.Filters;
using ReleaseBoard.Application.QueryHandlers;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.Domain.Distributions.Commands;
using ReleaseBoard.ReadModels;
using ReleaseBoard.Web.ApiModels;
using ReleaseBoard.Web.ApiModels.DistributionModels;
using ReleaseBoard.Web.Authorization.Security;
using ReleaseBoard.Web.Factories;
using ReleaseBoard.Web.Providers;

namespace ReleaseBoard.Web.Controllers
{
    /// <summary>
    /// Api контроллер для работы с дистрибутивами.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DistributionsController : Controller
    {
        private readonly IMediator mediator;
        private readonly IDistributionCommandMapper distributionCommandMapper;
        private readonly IReadOnlyRepository<DistributionReadModel> distributionsRepository;
        private readonly IMapper mapper;
        private readonly IAuthorizedUserProvider authorizedUserProvider;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="distributionsRepository"><see cref="IReadOnlyRepository{DistributionReadModel}"/>.</param>
        /// <param name="mapper">Маппер типов.</param>
        /// <param name="authorizedUserProvider">.</param>
        /// <param name="mediator"><see cref="IMediator"/>.</param>
        /// <param name="distributionCommandMapper"><see cref="IDistributionCommandMapper"/>.</param>
        public DistributionsController(
            IReadOnlyRepository<DistributionReadModel> distributionsRepository,
            IMapper mapper,
            IAuthorizedUserProvider authorizedUserProvider,
            IMediator mediator,
            IDistributionCommandMapper distributionCommandMapper)
        {
            this.distributionsRepository = distributionsRepository;
            this.mapper = mapper;
            this.authorizedUserProvider = authorizedUserProvider;
            this.mediator = mediator;
            this.distributionCommandMapper = distributionCommandMapper;
        }

        /// <summary>
        /// Вернуть все дистрибутивы.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [HttpGet]
        [Authorize(Policy = PolicyNames.CanAccessBuildSyncApi)]
        public async Task<GetDistributionsResponse> GetAll()
        {
            IList<DistributionReadModel> result = await distributionsRepository.GetAll();

            return new GetDistributionsResponse { Distributions = mapper.Map<IList<DistributionDto>>(result) };
        }

        /// <summary>
        /// Получить дистрибутив по идентификатору.
        /// </summary>
        /// <param name="distributionId">Идентификатор дистрибутива.</param>
        /// <returns><see cref="Task"/>.</returns>
        [HttpGet("{distributionId}")]
        public async Task<ActionResult<DistributionReadModel>> GetById(Guid distributionId)
        {
            DistributionReadModel distribution = await distributionsRepository.Query(x => x.Id == distributionId);

            if (distribution == null)
            {
                return NotFound();
            }
            var user = authorizedUserProvider.Get();

            if (!user.IsAdmin && distribution.Owners.All(o => o.Sid != user.Sid))
            {
                return Forbid();
            }

            return distribution;
        }

        /// <summary>
        /// Создать новый дистрибутив.
        /// </summary>
        /// <param name="model"><see cref="DistributionModel"/>.</param>
        /// <returns><see cref="Task"/>.</returns>
        [HttpPost]
        [Authorize(Policy = PolicyNames.CanManageDistributions)]
        public async Task<ActionResult> Create(
            [Required] [CustomizeValidator(RuleSet = "Create")]
            DistributionModel model
        )
        {
            CreateDistribution command = await distributionCommandMapper.MapToCreateCommand(model);

            await mediator.Send(command);

            return Ok();
        }

        /// <summary>
        /// Редактирование объекта дистрибутива.
        /// </summary>
        /// <param name="model"><see cref="DistributionModel"/>.</param>
        /// <returns><see cref="Task"/>.</returns>
        [HttpPut]
        [Authorize(Policy = PolicyNames.CanManageDistributions)]
        public async Task<IActionResult> Update(
            [Required] [CustomizeValidator(RuleSet = "Update")]
            DistributionModel model)
        {
            UpdateDistribution command = await distributionCommandMapper.MapToUpdateCommand(model);

            await mediator.Send(command);

            return Ok();
        }

        /// <summary>
        /// Производит фильтр дистрибутивов, которые соответсвуют сборкам.
        /// </summary>
        /// <param name="filter">Модель фильтра сборок по дистрибутиву <see cref="DistributionsFilter"/>.</param>
        /// <returns><see cref="Task"/>.</returns>
        [HttpPost("filter")]
        [Authorize]
        public async Task<IList<DistributionReadModel>> Filter([FromBody] DistributionsFilter filter)
        {
            AuthorizedUser user = authorizedUserProvider.Get();

            return await mediator.Send(new GetDistributionsByFilter.Query { Filter = filter, IsAdmin = user.IsAdmin, UserSid = user.Sid });
        }
    }
}

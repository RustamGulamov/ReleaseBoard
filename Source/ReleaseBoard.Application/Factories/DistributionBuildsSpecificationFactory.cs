using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReleaseBoard.Application.Models.Filters;
using ReleaseBoard.Application.Specifications;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.Domain.Specifications;
using ReleaseBoard.ReadModels;

namespace ReleaseBoard.Application.Factories
{
    /// <inheritdoc />
    public class DistributionBuildsSpecificationFactory : IDistributionBuildsSpecificationFactory
    {
        /// <inheritdoc />
        public Specification<BuildReadModel> CreateFromFilter(DistributionBuildsFilter filter)
        {
            List<Specification<BuildReadModel>> specifications = new ();
            Specification<BuildReadModel> buildsFilterSpecification = CreateFromDistributionBuildsFilter(filter);

            if (buildsFilterSpecification != null)
            {
                specifications.Add(buildsFilterSpecification);
            }

            specifications.AddRange(new List<Specification<BuildReadModel>>
            {
                new WithDistributionsIdsSpecification(filter.DistributionId), 
                new WithCreationDateRangeSpecification(filter.CreationDateRange)
            });

            return new AndSpecification<BuildReadModel>(specifications.ToArray());
        }

        private Specification<BuildReadModel> CreateFromDistributionBuildsFilter(DistributionBuildsFilter filter)
        {
            List<Specification<BuildReadModel>> specifications = new();

            if (filter.LifeCycleStates.Any())
            {
                specifications.Add(new WithLifeCycleStateSpecification(filter.LifeCycleStates));
            }

            if (filter.Tags.Any())
            {
                specifications.Add(new WithTagsSpecification(filter.Tags, filter.TagsCondition));
            }

            if (filter.Suffixes.Any())
            {
                specifications.Add(new WithSuffixesSpecification(filter.Suffixes, filter.SuffixesCondition));
            }

            if (filter.Builds.Any())
            {
                specifications.Add(new WithBuildNumbersSpecification(filter.Builds));
            }

            if (specifications.Count == 0)
            {
                return null;
            }

            return new AndSpecification<BuildReadModel>(specifications.ToArray());
        }
    }
}

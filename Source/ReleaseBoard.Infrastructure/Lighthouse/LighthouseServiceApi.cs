using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lighthouse.Contracts.ApiClients.Interfaces;
using Lighthouse.Contracts.Dtos;
using Lighthouse.Contracts.Enums;
using ReleaseBoard.Application.Interfaces;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Infrastructure.Lighthouse
{
    /// <inheritdoc />
    public class LighthouseServiceApi : ILighthouseServiceApi
    {
        private readonly IDepartmentsApiClient departmentsApiClient;
        private readonly IEmployeesApiClient employeesApiClient;
        private readonly IProjectsApiClient projectsApiClient;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="employeesApiClient"><see cref="IEmployeesApiClient"/>.</param>
        /// <param name="departmentsApiClient"><see cref="IDepartmentsApiClient"/>.</param>
        /// <param name="projectsApiClient"><see cref="IProjectsApiClient"/>.</param>
        public LighthouseServiceApi(
            IEmployeesApiClient employeesApiClient,
            IDepartmentsApiClient departmentsApiClient,
            IProjectsApiClient projectsApiClient)
        {
            this.employeesApiClient = employeesApiClient;
            this.departmentsApiClient = departmentsApiClient;
            this.projectsApiClient = projectsApiClient;
        }

        /// <inheritdoc/>
        public async Task<User> GetUserBySid(string sid)
        {
            EmployeeDto employee = await employeesApiClient.GetBySid(sid);
            
            return 
                employee == null 
                    ? null 
                    : new User(employee.Sid, employee.Name);
        }

        /// <inheritdoc />
        public async Task<bool> IsValidUsers(params string[] sids)
        {
            IList<EmployeeDto> employees = await employeesApiClient.GetAllBySids(sids);
            return employees.All(x => x != null && x.State == EmployeeStateDto.Active);
        }

        /// <inheritdoc />
        public async Task<Project> GetProjectById(Guid externalId)
        {
            var project = await projectsApiClient.GetByExternalId(externalId);
            
            return
                project == null 
                    ? null 
                    : new Project(project.ExternalId, project.ShortName, project.Name);
        }
    }
}

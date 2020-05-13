﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Domain.Repositories;
using Abp.EntityHistory;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Zinlo.Auditing.Dto;
using Zinlo.Auditing.Exporting;
using Zinlo.Authorization;
using Zinlo.Authorization.Users;
using Zinlo.Dto;
using Zinlo.EntityHistory;
using Zinlo.InstructionVersions;
using EntityHistoryHelper = Zinlo.EntityHistory.EntityHistoryHelper;

namespace Zinlo.Auditing
{
    [DisableAuditing]
    [AbpAuthorize(AppPermissions.Pages_Administration_AuditLogs)]
    public class AuditLogAppService : ZinloAppServiceBase, IAuditLogAppService
    {
        private readonly IRepository<AuditLog, long> _auditLogRepository;
        private readonly IRepository<EntityChange, long> _entityChangeRepository;
        private readonly IRepository<EntityChangeSet, long> _entityChangeSetRepository;
        private readonly IRepository<EntityPropertyChange, long> _entityPropertyChangeRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IAuditLogListExcelExporter _auditLogListExcelExporter;
        private readonly INamespaceStripper _namespaceStripper;
        private readonly IAbpStartupConfiguration _abpStartupConfiguration;
        private readonly IInstructionAppService _instructionAppService;

        public AuditLogAppService(
            IRepository<AuditLog, long> auditLogRepository,
            IRepository<User, long> userRepository,
            IAuditLogListExcelExporter auditLogListExcelExporter,
            INamespaceStripper namespaceStripper,
            IRepository<EntityChange, long> entityChangeRepository,
            IRepository<EntityChangeSet, long> entityChangeSetRepository,
            IRepository<EntityPropertyChange, long> entityPropertyChangeRepository,
            IAbpStartupConfiguration abpStartupConfiguration,
            IInstructionAppService instructionAppService)
        {
            _auditLogRepository = auditLogRepository;
            _userRepository = userRepository;
            _auditLogListExcelExporter = auditLogListExcelExporter;
            _namespaceStripper = namespaceStripper;
            _entityChangeRepository = entityChangeRepository;
            _entityChangeSetRepository = entityChangeSetRepository;
            _entityPropertyChangeRepository = entityPropertyChangeRepository;
            _abpStartupConfiguration = abpStartupConfiguration;
            _instructionAppService = instructionAppService;
        }

        #region audit logs

        public async Task<PagedResultDto<AuditLogListDto>> GetAuditLogs(GetAuditLogsInput input)
        {
            var query = CreateAuditLogAndUsersQuery(input);

            var resultCount = await query.CountAsync();
            var results = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            var auditLogListDtos = ConvertToAuditLogListDtos(results);

            return new PagedResultDto<AuditLogListDto>(resultCount, auditLogListDtos);
        }

        public async Task<FileDto> GetAuditLogsToExcel(GetAuditLogsInput input)
        {
            var auditLogs = await CreateAuditLogAndUsersQuery(input)
                .AsNoTracking()
                .OrderByDescending(al => al.AuditLog.ExecutionTime)
                .ToListAsync();

            var auditLogListDtos = ConvertToAuditLogListDtos(auditLogs);

            return _auditLogListExcelExporter.ExportToFile(auditLogListDtos);
        }

        private List<AuditLogListDto> ConvertToAuditLogListDtos(List<AuditLogAndUser> results)
        {
            return results.Select(
                result =>
                {
                    var auditLogListDto = ObjectMapper.Map<AuditLogListDto>(result.AuditLog);
                    auditLogListDto.UserName = result.User?.UserName;
                    auditLogListDto.ServiceName = _namespaceStripper.StripNameSpace(auditLogListDto.ServiceName);
                    return auditLogListDto;
                }).ToList();
        }

        private IQueryable<AuditLogAndUser> CreateAuditLogAndUsersQuery(GetAuditLogsInput input)
        {
            var query = from auditLog in _auditLogRepository.GetAll()
                        join user in _userRepository.GetAll() on auditLog.UserId equals user.Id into userJoin
                        from joinedUser in userJoin.DefaultIfEmpty()
                        where auditLog.ExecutionTime >= input.StartDate && auditLog.ExecutionTime <= input.EndDate
                        select new AuditLogAndUser { AuditLog = auditLog, User = joinedUser };

            query = query
                .WhereIf(!input.UserName.IsNullOrWhiteSpace(), item => item.User.UserName.Contains(input.UserName))
                .WhereIf(!input.ServiceName.IsNullOrWhiteSpace(), item => item.AuditLog.ServiceName.Contains(input.ServiceName))
                .WhereIf(!input.MethodName.IsNullOrWhiteSpace(), item => item.AuditLog.MethodName.Contains(input.MethodName))
                .WhereIf(!input.BrowserInfo.IsNullOrWhiteSpace(), item => item.AuditLog.BrowserInfo.Contains(input.BrowserInfo))
                .WhereIf(input.MinExecutionDuration.HasValue && input.MinExecutionDuration > 0, item => item.AuditLog.ExecutionDuration >= input.MinExecutionDuration.Value)
                .WhereIf(input.MaxExecutionDuration.HasValue && input.MaxExecutionDuration < int.MaxValue, item => item.AuditLog.ExecutionDuration <= input.MaxExecutionDuration.Value)
                .WhereIf(input.HasException == true, item => item.AuditLog.Exception != null && item.AuditLog.Exception != "")
                .WhereIf(input.HasException == false, item => item.AuditLog.Exception == null || item.AuditLog.Exception == "");
            return query;
        }

        #endregion

        #region entity changes 

        public List<NameValueDto> GetEntityHistoryObjectTypes()
        {
            var entityHistoryObjectTypes = new List<NameValueDto>();
            var enabledEntities = (_abpStartupConfiguration.GetCustomConfig()
                .FirstOrDefault(x => x.Key == EntityHistoryHelper.EntityHistoryConfigurationName)
                .Value as EntityHistoryUiSetting)?.EnabledEntities ?? new List<string>();

            if (AbpSession.TenantId == null)
            {
                enabledEntities = EntityHistoryHelper.HostSideTrackedTypes.Select(t => t.FullName).Intersect(enabledEntities).ToList();
            }
            else
            {
                enabledEntities = EntityHistoryHelper.TenantSideTrackedTypes.Select(t => t.FullName).Intersect(enabledEntities).ToList();
            }

            foreach (var enabledEntity in enabledEntities)
            {
                entityHistoryObjectTypes.Add(new NameValueDto(L(enabledEntity), enabledEntity));
            }

            return entityHistoryObjectTypes;
        }

        public async Task<PagedResultDto<EntityChangeListDto>> GetEntityChanges(GetEntityChangeInput input)
        {
            var query = CreateEntityChangesAndUsersQuery(input);

            var resultCount = await query.CountAsync();
            var results = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            var entityChangeListDtos = ConvertToEntityChangeListDtos(results);

            return new PagedResultDto<EntityChangeListDto>(resultCount, entityChangeListDtos);
        }

        public async Task<PagedResultDto<EntityChangeListDto>> GetEntityTypeChanges(GetEntityTypeChangeInput input)
        {
            // Fix for: https://github.com/aspnetzero/aspnet-zero-core/issues/2101
            var entityId = "\"" + input.EntityId + "\"";

            var query = from entityChangeSet in _entityChangeSetRepository.GetAll()
                        join entityChange in _entityChangeRepository.GetAll() on entityChangeSet.Id equals entityChange.EntityChangeSetId
                        join user in _userRepository.GetAll() on entityChangeSet.UserId equals user.Id
                        where entityChange.EntityTypeFullName == input.EntityTypeFullName &&
                              (entityChange.EntityId == input.EntityId || entityChange.EntityId == entityId)
                        select new EntityChangeAndUser
                        {
                            EntityChange = entityChange,
                            User = user
                        };

            var resultCount = await query.CountAsync();
            var results = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            var entityChangeListDtos = ConvertToEntityChangeListDtos(results);

            return new PagedResultDto<EntityChangeListDto>(resultCount, entityChangeListDtos);
        }

        public async Task<FileDto> GetEntityChangesToExcel(GetEntityChangeInput input)
        {
            var entityChanges = await CreateEntityChangesAndUsersQuery(input)
                .AsNoTracking()
                .OrderByDescending(ec => ec.EntityChange.EntityChangeSetId)
                .ThenByDescending(ec => ec.EntityChange.ChangeTime)
                .ToListAsync();

            var entityChangeListDtos = ConvertToEntityChangeListDtos(entityChanges);

            return _auditLogListExcelExporter.ExportToFile(entityChangeListDtos);
        }

        public async Task<List<EntityPropertyChangeDto>> GetEntityPropertyChanges(long entityChangeId)
        {
            var entityPropertyChanges = (await _entityPropertyChangeRepository.GetAllListAsync())
                .Where(epc => epc.EntityChangeId == entityChangeId);

            return ObjectMapper.Map<List<EntityPropertyChangeDto>>(entityPropertyChanges);
        }

        private List<EntityChangeListDto> ConvertToEntityChangeListDtos(List<EntityChangeAndUser> results)
        {
            return results.Select(
                result =>
                {
                    var entityChangeListDto = ObjectMapper.Map<EntityChangeListDto>(result.EntityChange);
                    entityChangeListDto.UserName = result.User?.UserName;
                    return entityChangeListDto;
                }).ToList();
        }

        private IQueryable<EntityChangeAndUser> CreateEntityChangesAndUsersQuery(GetEntityChangeInput input)
        {
            var query = from entityChangeSet in _entityChangeSetRepository.GetAll()
                        join entityChange in _entityChangeRepository.GetAll() on entityChangeSet.Id equals entityChange.EntityChangeSetId
                        join user in _userRepository.GetAll() on entityChangeSet.UserId equals user.Id
                        where entityChange.ChangeTime >= input.StartDate && entityChange.ChangeTime <= input.EndDate
                        select new EntityChangeAndUser
                        {
                            EntityChange = entityChange,
                            User = user
                        };

            query = query
                .WhereIf(!input.UserName.IsNullOrWhiteSpace(), item => item.User.UserName.Contains(input.UserName))
                .WhereIf(!input.EntityTypeFullName.IsNullOrWhiteSpace(), item => item.EntityChange.EntityTypeFullName.Contains(input.EntityTypeFullName));

            return query;
        }
        public async Task<List<EntityPropertyHistory>> GetEntityHistory(GetEntityHistoryInput input)
        {
            var query = from entityChangeSet in _entityChangeSetRepository.GetAll()
                        join entityChange in _entityChangeRepository.GetAll()
                                .WhereIf( input.EntityFullName.Equals("Zinlo.ChartofAccounts.ChartofAccounts"), p =>(p.EntityTypeFullName.Equals("Zinlo.ChartofAccounts.ChartofAccounts") && p.EntityId == input.EntityId )||(p.EntityId == input.AccountBalanceId && p.EntityTypeFullName.Equals("Zinlo.ChartofAccounts.AccountBalance")))
                                .WhereIf( String.IsNullOrWhiteSpace(input.AccountBalanceId), p =>p.EntityTypeFullName== "Zinlo.ClosingChecklist.ClosingChecklist" && p.EntityId == input.EntityId)
                            on entityChangeSet.Id equals entityChange
                            .EntityChangeSetId
                        join entityPropertyChange in _entityPropertyChangeRepository.GetAll()
                                .WhereIf(!String.IsNullOrWhiteSpace(input.PropertyName), p => p.PropertyName == input.PropertyName)
                                .WhereIf(input.EntityFullName.Equals("Zinlo.ClosingChecklist.ClosingChecklist") && String.IsNullOrWhiteSpace(input.PropertyName),p=>p.PropertyName != "VersionId")
                                .WhereIf(input.PropertyName!="VersionId",p=>p.OriginalValue!=null)
                            on entityChange.Id equals entityPropertyChange.EntityChangeId
                        join user in _userRepository.GetAll() on entityChangeSet.UserId equals user.Id
                        select new EntityPropertyHistory()
                        {
                            UserId = (long)entityChangeSet.UserId,
                            NewValue = entityPropertyChange.NewValue,
                            OriginalValue = entityPropertyChange.OriginalValue,
                            PropertyName = entityPropertyChange.PropertyName,
                            ChangeTime = entityChange.ChangeTime
                        };
            var result = await query.ToListAsync();
            if (!input.EntityFullName.Equals("Zinlo.ClosingChecklist.ClosingChecklist") ||
                input.PropertyName.IsNullOrEmpty() || !input.PropertyName.Equals("VersionId")) return result;
            foreach (var item in result)
            {
                item.Id = item.OriginalValue.IsNullOrEmpty() ? 0 : Convert.ToInt64(item.OriginalValue);
                item.OriginalValue = item.OriginalValue.IsNullOrEmpty() ? "None" : _instructionAppService.GetInstruction(Convert.ToInt64(item.OriginalValue)).Result.Body;
                item.NewValue = item.NewValue.IsNullOrEmpty() ? "None" :  _instructionAppService.GetInstruction(Convert.ToInt64(item.NewValue)).Result.Body;
            }
            return  result;
        }
        #endregion
    }
}

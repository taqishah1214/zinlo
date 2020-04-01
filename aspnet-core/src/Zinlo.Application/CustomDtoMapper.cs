using Zinlo.TimeManagements;
using Zinlo.Categories.Dtos;
using Zinlo.Categories;
using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.EntityHistory;
using Abp.Localization;
using Abp.Notifications;
using Abp.Organizations;
using Abp.UI.Inputs;
using AutoMapper;
using Zinlo.Auditing.Dto;
using Zinlo.Authorization.Accounts.Dto;
using Zinlo.Authorization.Permissions.Dto;
using Zinlo.Authorization.Roles;
using Zinlo.Authorization.Roles.Dto;
using Zinlo.Authorization.Users;
using Zinlo.Authorization.Users.Dto;
using Zinlo.Authorization.Users.Importing.Dto;
using Zinlo.Authorization.Users.Profile.Dto;
using Zinlo.Chat;
using Zinlo.Chat.Dto;
using Zinlo.Editions;
using Zinlo.Editions.Dto;
using Zinlo.Friendships;
using Zinlo.Friendships.Cache;
using Zinlo.Friendships.Dto;
using Zinlo.Localization.Dto;
using Zinlo.MultiTenancy;
using Zinlo.MultiTenancy.Dto;
using Zinlo.MultiTenancy.HostDashboard.Dto;
using Zinlo.MultiTenancy.Payments;
using Zinlo.MultiTenancy.Payments.Dto;
using Zinlo.Notifications.Dto;
using Zinlo.Organizations.Dto;
using Zinlo.Sessions.Dto;
using Zinlo.Comment.Dtos;
using Zinlo.Tasks.Dtos;
using Zinlo.ClosingChecklist.Dtos;
using Zinlo.Attachments.Dtos;
using Zinlo.AccountSubType.Dtos;
using Zinlo.ChartsofAccount.Dtos;
using Zinlo.Reconciliation.Dtos;
using Zinlo.Reconciliation;
using Zinlo.TimeManagements.Dto;

namespace Zinlo
{
    internal static class CustomDtoMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<CreateOrEditTimeManagementDto, TimeManagement>().ReverseMap();
            configuration.CreateMap<TimeManagementDto, TimeManagement>().ReverseMap();
            
           
           

            //Inputs
            configuration.CreateMap<CheckboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<SingleLineStringInputType, FeatureInputTypeDto>();
            configuration.CreateMap<ComboboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<IInputType, FeatureInputTypeDto>()
                .Include<CheckboxInputType, FeatureInputTypeDto>()
                .Include<SingleLineStringInputType, FeatureInputTypeDto>()
                .Include<ComboboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<StaticLocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>();
            configuration.CreateMap<ILocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>()
                .Include<StaticLocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>();
            configuration.CreateMap<LocalizableComboboxItem, LocalizableComboboxItemDto>();
            configuration.CreateMap<ILocalizableComboboxItem, LocalizableComboboxItemDto>()
                .Include<LocalizableComboboxItem, LocalizableComboboxItemDto>();

            //Chat
            configuration.CreateMap<ChatMessage, ChatMessageDto>();
            configuration.CreateMap<ChatMessage, ChatMessageExportDto>();

            //Feature
            configuration.CreateMap<FlatFeatureSelectDto, Feature>().ReverseMap();
            configuration.CreateMap<Feature, FlatFeatureDto>();

            //Role
            configuration.CreateMap<RoleEditDto, Role>().ReverseMap();
            configuration.CreateMap<Role, RoleListDto>();
            configuration.CreateMap<UserRole, UserListRoleDto>();

            //Edition
            configuration.CreateMap<EditionEditDto, SubscribableEdition>().ReverseMap();
            configuration.CreateMap<EditionCreateDto, SubscribableEdition>();
            configuration.CreateMap<EditionSelectDto, SubscribableEdition>().ReverseMap();
            configuration.CreateMap<SubscribableEdition, EditionInfoDto>();

            configuration.CreateMap<Edition, EditionInfoDto>().Include<SubscribableEdition, EditionInfoDto>();

            configuration.CreateMap<SubscribableEdition, EditionListDto>();
            configuration.CreateMap<Edition, EditionEditDto>();
            configuration.CreateMap<Edition, SubscribableEdition>();
            configuration.CreateMap<Edition, EditionSelectDto>();


            //Payment
            configuration.CreateMap<SubscriptionPaymentDto, SubscriptionPayment>().ReverseMap();
            configuration.CreateMap<SubscriptionPaymentListDto, SubscriptionPayment>().ReverseMap();
            configuration.CreateMap<SubscriptionPayment, SubscriptionPaymentInfoDto>();

            //Permission
            configuration.CreateMap<Permission, FlatPermissionDto>();
            configuration.CreateMap<Permission, FlatPermissionWithLevelDto>();

            //Language
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageEditDto>();
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageListDto>();
            configuration.CreateMap<NotificationDefinition, NotificationSubscriptionWithDisplayNameDto>();
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageEditDto>()
                .ForMember(ldto => ldto.IsEnabled, options => options.MapFrom(l => !l.IsDisabled));

            //Tenant
            configuration.CreateMap<Tenant, RecentTenant>();
            configuration.CreateMap<Tenant, TenantLoginInfoDto>();
            configuration.CreateMap<Tenant, TenantListDto>();
            configuration.CreateMap<TenantEditDto, Tenant>().ReverseMap();
            configuration.CreateMap<CurrentTenantInfoDto, Tenant>().ReverseMap();

            //User
            configuration.CreateMap<User, UserEditDto>()
                .ForMember(dto => dto.Password, options => options.Ignore())
                .ReverseMap()
                .ForMember(user => user.Password, options => options.Ignore());
            configuration.CreateMap<User, UserLoginInfoDto>();
            configuration.CreateMap<User, UserListDto>();
            configuration.CreateMap<User, ChatUserDto>();
            configuration.CreateMap<User, OrganizationUnitUserListDto>();
            configuration.CreateMap<Role, OrganizationUnitRoleListDto>();
            configuration.CreateMap<CurrentUserProfileEditDto, User>().ReverseMap();
            configuration.CreateMap<UserLoginAttemptDto, UserLoginAttempt>().ReverseMap();
            configuration.CreateMap<ImportUserDto, User>();

            //AuditLog
            configuration.CreateMap<AuditLog, AuditLogListDto>();
            configuration.CreateMap<EntityChange, EntityChangeListDto>();
            configuration.CreateMap<EntityPropertyChange, EntityPropertyChangeDto>();

            //Friendship
            configuration.CreateMap<Friendship, FriendDto>();
            configuration.CreateMap<FriendCacheItem, FriendDto>();

            //OrganizationUnit
            configuration.CreateMap<OrganizationUnit, OrganizationUnitDto>();

            /* ADD YOUR OWN CUSTOM AUTOMAPPER MAPPINGS HERE */
            //Closing checklist
            configuration.CreateMap<CreateOrEditClosingChecklistDto, ClosingChecklist.ClosingChecklist>().ReverseMap();
            configuration.CreateMap<DetailsClosingCheckListDto, ClosingChecklist.ClosingChecklist>().ReverseMap();
            configuration.CreateMap<GetTaskForEditDto, ClosingChecklist.ClosingChecklist>().ReverseMap().
                ForMember(dto=>dto.AssigniName,entity=>entity.MapFrom(p=>p.Assignee.FullName)).
                ForMember(dto=>dto.DayBeforeAfter,entity=>entity.MapFrom(p=>p.DayBeforeAfter)).
                ForMember(dto=>dto.Category, entity=>entity.MapFrom(p=>p.Category.Title)).
                ForMember(dto=>dto.Status, entity=>entity.MapFrom(p=>p.Status.ToString())).
                ForMember(dto=>dto.StatusId, entity=>entity.MapFrom(p=>(int)p.Status)).
                ForMember(dto=>dto.FrequencyId, entity=>entity.MapFrom(p=>(int)p.Frequency));
            configuration.CreateMap<Comment.Comment, CommentDto>().ReverseMap();

            //Attachment
            configuration.CreateMap<Attachment.Attachment, AttachmentsDto>().ReverseMap();


            //Category
            configuration.CreateMap<CategoryDto, Category>().ReverseMap();
            configuration.CreateMap<CreateOrEditCategoryDto, Category>().ReverseMap();
            configuration.CreateMap<GetCategoryForViewDto, Category>().ReverseMap()
                .ForMember(dto=>dto.UserId, entity=> entity.MapFrom(p=>p.CreatorUserId))
                .ForMember(dto=>dto.CreationDate, entity=> entity.MapFrom(p=>p.CreationTime))
                ;

            //AccountSubType
            configuration.CreateMap<CreateOrEditAccountSubTypeDto,AccountSubType.AccountSubType>().ReverseMap();
            configuration.CreateMap<GetAccountSubTypeForViewDto, AccountSubType.AccountSubType>().ReverseMap()
               .ForMember(dto => dto.UserId, entity => entity.MapFrom(p => p.CreatorUserId))
               .ForMember(dto => dto.CreationDate, entity => entity.MapFrom(p => p.CreationTime))
               ;

            //Comment
            configuration.CreateMap<CommentDto, Comment.Comment>().ReverseMap();
            configuration.CreateMap<Comment.Comment, CreateOrEditCommentDto>().ReverseMap();

            //ChartsofAccount
            configuration.CreateMap<CreateOrEditChartsofAccountDto, ChartofAccounts.ChartofAccounts>().ReverseMap();
            configuration.CreateMap<GetAccountForEditDto, ChartofAccounts.ChartofAccounts>().ReverseMap();

            //Reconciliation
            configuration.CreateMap<CreateOrEditItemizationDto, Itemization>().ReverseMap();
            configuration.CreateMap<CreateOrEditAmortizationDto, Amortization>().ReverseMap();

            
        }
    }
}

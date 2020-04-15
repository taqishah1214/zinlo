using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zinlo.Authorization.Users;
using Zinlo.MultiTenancy;
using Zinlo.MultiTenancy.Dto;
using Zinlo.MultiTenancy.Payments;
using Zinlo.Register_User.Dto;

namespace Zinlo.Register_User
{
    public class UserRegisterService : ZinloAppServiceBase, IUserRegisterService
    {
        
        private readonly IRepository<UserBusinessInfo, long> _userBusinessInfoRepository;
        private readonly IRepository<UserPaymentDetails, long> _userUserPaymentDetailsRepository;
        private readonly ITenantRegistrationAppService _tenantRegistrationAppService;

        public UserRegisterService(
          
            IRepository<UserBusinessInfo, long> userBusinessInfoRepository,
            IRepository<UserPaymentDetails, long> userUserPaymentDetailsRepository,
            ITenantRegistrationAppService tenantRegistrationAppService
            )
        {
            _tenantRegistrationAppService = tenantRegistrationAppService;
            _userUserPaymentDetailsRepository = userUserPaymentDetailsRepository;
            _userBusinessInfoRepository = userBusinessInfoRepository;

        }
       
        public async Task<RegisterTenantOutput> RegisterUserWithTenant(RegisterUserInput registerUser)
        {
            var registerTenant = await _tenantRegistrationAppService.RegisterTenant(new RegisterTenantInput
            {
                AdminEmailAddress = registerUser.PersonalInfo.EmailAddress,
                Name = registerUser.PersonalInfo.UserName,
                AdminPassword = registerUser.PersonalInfo.Password,
                EditionId = registerUser.SubscriptionPlans.EditionId,
                TenancyName = Regex.Replace(registerUser.BusinessInfo.BusinessName, @"\s", ""),
                SubscriptionStartType = (SubscriptionStartType)registerUser.SubscriptionPlans.SubscriptionStartType,
                CaptchaResponse = ""
            });

           await  _userBusinessInfoRepository.InsertAsync(new UserBusinessInfo
            {
                BusinessName = registerUser.BusinessInfo.BusinessName,
                Website = registerUser.BusinessInfo.Website,
                PhoneNumber = registerUser.BusinessInfo.PhoneNumber,
                AddressLineOne = registerUser.BusinessInfo.AddressLineOne,
                AddressLineTwo = registerUser.BusinessInfo.AddressLineTwo,
                City = registerUser.BusinessInfo.City,
                State = registerUser.BusinessInfo.State,
                ZipCode = registerUser.BusinessInfo.ZipCode,
                TenantId = registerTenant.TenantId
            });

           await _userUserPaymentDetailsRepository.InsertAsync(new UserPaymentDetails
            {
                CardNumber = registerUser.PaymentDetails.CardNumber,
                Commitment = registerUser.PaymentDetails.Commitment,
                CVVCode = registerUser.PaymentDetails.CVVCode,
                Email = registerUser.PaymentDetails.Email,
                ExpiryDate = registerUser.PaymentDetails.ExpiryDate,
                TenantId = registerTenant.TenantId
            });
            return registerTenant;
        }
    }
}

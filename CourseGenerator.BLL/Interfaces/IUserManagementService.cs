﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CourseGenerator.BLL.Infrastructure;
using CourseGenerator.BLL.DTO;
using CourseGenerator.Models.Entities.Identity;
using System.Security.Claims;
using CourseGenerator.Models.Entities.Security;

namespace CourseGenerator.BLL.Interfaces
{
    public interface IUserManagementService : IDisposable
    {
        Task<OperationInfo> CreateAsync(UserRegistrationDTO registrationDto, params string[] roles);
        Task<OperationInfo> ExistsWithUserNameAsync(string userName);
        Task<OperationInfo> AddToRolesAsync(User user, params string[] roles);
        Task<UserDetailsDTO> GetDetailsByUserNameAsync(string userName);
        Task<ClaimsIdentity> GetIdentityAsync(UserLoginDTO userLoginDto);
        Task<ClaimsIdentity> GetIdentityAsync(PhoneAuth phoneAuth);
        Task<OperationInfo> ConfirmPhoneNumberAsync(string userName);
        Task<OperationInfo> CreatePhoneNumberConfirmationCodeAsync(PhoneAuth phoneAuth);
        Task<OperationInfo> DeletePhoneNumberConfirmationCodeAsync(PhoneAuth phoneAuth);
    }
}

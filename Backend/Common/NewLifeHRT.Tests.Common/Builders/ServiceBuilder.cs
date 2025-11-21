using Microsoft.AspNetCore.Identity;
using Moq;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Tests.Common.Builders
{
    public abstract class ServiceBuilder<T> : BaseServiceBuilder<T> where T : class
    {
        protected Mock<IUserRepository> UserRepositoryMock { get; set;} = new();
        protected Mock<IAddressRepository> AddressRepositoryMock {get; set;} = new();
        protected Mock<IPasswordHasher<ApplicationUser>> PasswordHasherMock { get; set; } = new();
        protected Mock<UserManager<ApplicationUser>> UserManagerMock { get; set; } = new(new Mock<IUserStore<ApplicationUser>>().Object, null, null, null, null, null, null, null, null);
        protected Mock<IUserServiceLinkRepository> UserServiceLinkRepositoryMock { get; set; } = new();
        protected Mock<ClinicDbContext> ClinicDbContextMock {  get; set; } = new();
        protected Mock<ILicenseInformationService> LicenseInformationServiceMock {  get; set; } = new();
        protected Mock<IBlobService> BlobServiceMock{  get; set; } = new();
        protected Mock<AzureBlobStorageSettings> AzureBlobStorageSettingsMock{  get; set; } = new();
        protected Mock<IUserSignatureRepository> UserSignatureRepositoryMock { get; set; } = new();
        protected Mock<RoleManager<ApplicationRole>> RoleManagerMock{  get; set; } = new();

        public ServiceBuilder<T> SetParameter(Mock<IUserRepository> userRepositoryMock)
        {
            UserRepositoryMock = userRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IAddressRepository> addressRepositoryMock)
        {
            AddressRepositoryMock = addressRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IPasswordHasher<ApplicationUser>> passwordHasherMock)
        {
            PasswordHasherMock = passwordHasherMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<UserManager<ApplicationUser>> userManagerMock)
        {
            UserManagerMock = userManagerMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<IUserServiceLinkRepository> userServiceLinkRepositoryMock)
        {
            UserServiceLinkRepositoryMock = userServiceLinkRepositoryMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<ClinicDbContext> clinicDbContextMock)
        {
            ClinicDbContextMock = clinicDbContextMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<ILicenseInformationService> licenseInformationServiceMock)
        {
            LicenseInformationServiceMock = licenseInformationServiceMock;
            return this;
        }
        
        public ServiceBuilder<T> SetParameter(Mock<IBlobService> blobServiceMock)
        {
            BlobServiceMock = blobServiceMock;
            return this;
        }

        public ServiceBuilder<T> SetParameter(Mock<AzureBlobStorageSettings> azureBlobStorageSettingsMock)
        {
            AzureBlobStorageSettingsMock = azureBlobStorageSettingsMock;
            return this;
        }
        public ServiceBuilder<T> SetParameter(Mock<IUserSignatureRepository> userSignatureRepositoryMock)
        {
            UserSignatureRepositoryMock = userSignatureRepositoryMock;
            return this;
        }

    }
}

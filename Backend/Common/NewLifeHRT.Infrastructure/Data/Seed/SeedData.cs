using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Entities.Hospital;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.Infrastructure.Extensions;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace NewLifeHRT.Infrastructure.Data.Seed
{
    public static class SeedData
    {
        public static class Hospital
        {
            public static MigrationBuilder InsertInitialClinics(MigrationBuilder migrationBuilder)
            {
                var newlifeIdentityOptions = new ClinicIdentityOptions
                {
                    Password = new PasswordOptionsConfig
                    {
                        RequiredLength = 12,
                        RequireNonAlphanumeric = true,
                    },
                    Lockout = new LockoutOptionsConfig
                    {
                        MaxFailedAccessAttempts = 5,
                        DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30)
                    }
                };

                var peakWellnessIdentityOptions = new ClinicIdentityOptions
                {
                    Password = new PasswordOptionsConfig
                    {
                        RequiredLength = 8,
                        RequireUppercase = false
                    }
                };

                migrationBuilder.InsertData(
                    table: "Clinics",
                    columns: new[] { "Id", "TenantId", "ClinicName", "Description", "Domain", "Database", "IsActive", "CreatedAt", "HostUrl", "JwtBearerAudience", "IdentityOptions" },
                    values: new object[,]
                    {
                        {
                            new Guid("3f2504e0-4f89-11d3-9a0c-0305e82c3301"),
                            new Guid("0f8fad5b-d9cb-469f-a165-70867728950e"),
                            "NewLife HRT",
                            "newlifehrt Description",
                            "newlifehrt",                     // used as path segment
                            "NewLifeHRT",
                            true,
                            DateTime.UtcNow,
                            "https://localhost:5141",       // root host for all tenants
                            "https://localhost:5141",       // JwtBearerAudience can match host
                            JsonSerializer.Serialize(newlifeIdentityOptions)
                        },
                        {
                            new Guid("7c9e6679-7425-40de-944b-e07fc1f90ae7"),
                            new Guid("e3585e7a-3a92-4dbb-8b32-03e3e49e3f23"),
                            "Peak Wellness",
                            "Peak Wellness Description",
                            "peakwellness",                     // used as path segment
                            "PeakWellness",
                            true,
                            DateTime.UtcNow,
                            "https://localhost:5141",
                            "https://localhost:5141",
                            JsonSerializer.Serialize(peakWellnessIdentityOptions)
                        }
                    }
                );
                return migrationBuilder;
            }
        }

        public static class Clinic
        {
            public static MigrationBuilder InsertRolesAndPermissions(MigrationBuilder migrationBuilder)
            {
                // Insert Roles
                migrationBuilder.InsertData(
                    table: "AspNetRoles",
                    columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp", "RoleEnum" },
                    values: new object[,]
                    {
                    { (int)AppRoleEnum.SuperAdmin, AppRoleEnum.SuperAdmin.GetDisplayName(), AppRoleEnum.SuperAdmin.GetDisplayName().ToUpper(),  Guid.NewGuid().ToString(), AppRoleEnum.SuperAdmin.ToString()},
                    { (int)AppRoleEnum.Admin, AppRoleEnum.Admin.ToString(), AppRoleEnum.Admin.ToString().ToUpper() , Guid.NewGuid().ToString(), AppRoleEnum.Admin.ToString()},
                    { (int)AppRoleEnum.Receptionist, AppRoleEnum.Receptionist.ToString(), AppRoleEnum.Receptionist.ToString().ToUpper() , Guid.NewGuid().ToString(), AppRoleEnum.Receptionist.ToString()},
                    { (int)AppRoleEnum.Nurse, AppRoleEnum.Nurse.ToString(), AppRoleEnum.Nurse.ToString().ToUpper() , Guid.NewGuid().ToString(), AppRoleEnum.Nurse.ToString()},
                    { (int)AppRoleEnum.Doctor, AppRoleEnum.Doctor.ToString(), AppRoleEnum.Doctor.ToString().ToUpper() , Guid.NewGuid().ToString(), AppRoleEnum.Doctor.ToString()},
                    { (int)AppRoleEnum.SalesPerson, AppRoleEnum.SalesPerson.GetDisplayName(), AppRoleEnum.SalesPerson.GetDisplayName().ToUpper() , Guid.NewGuid().ToString(), AppRoleEnum.SalesPerson.ToString()}
                    });


                // Insert Permissions
                var createdAt = DateTime.UtcNow;
                migrationBuilder.InsertData(
                    table: "Permissions",
                    columns: new[] { "Id", "PermissionName", "Type", "Section", "IsActive", "CreatedAt", "UpdatedAt", "SectionEnum", "PermissionTypeEnum" },
                    values: new object[,]
                    {
                    // Patient Section
                    { 1, "PatientCreate", PermissionTypeEnum.Create.ToString(), SectionEnum.Patient.ToString(), true, createdAt, null, SectionEnum.Patient.ToString(),  PermissionTypeEnum.Create.ToString() },
                    { 2, "PatientRead", PermissionTypeEnum.Read.ToString(), SectionEnum.Patient.ToString(), true, createdAt, null, SectionEnum.Patient.ToString(), PermissionTypeEnum.Read.ToString()},
                    { 3, "PatientWrite", PermissionTypeEnum.Write.ToString(), SectionEnum.Patient.ToString(), true, createdAt, null , SectionEnum.Patient.ToString(), PermissionTypeEnum.Write.ToString()},
                    {4, "PatientUpdate", PermissionTypeEnum.Update.ToString(), SectionEnum.Patient.ToString(), true, createdAt, null, SectionEnum.Patient.ToString(), PermissionTypeEnum.Update.ToString()},
                    {5, "PatientDelete", PermissionTypeEnum.Delete.ToString(), SectionEnum.Patient.ToString(), true, createdAt, null, SectionEnum.Patient.ToString(), PermissionTypeEnum.Delete.ToString()},
                    {6, "PatientAssign", PermissionTypeEnum.Assign.ToString(), SectionEnum.Patient.ToString(), true, createdAt, null, SectionEnum.Patient.ToString(), PermissionTypeEnum.Assign.ToString()},
                    { 7, "PatientNotApplicable", PermissionTypeEnum.NotApplicable.GetDisplayName(), SectionEnum.Patient.ToString(), true, createdAt, null, SectionEnum.Patient.ToString(), PermissionTypeEnum.NotApplicable.ToString() },

                    // Lead Section
                    {8, "LeadCreate", PermissionTypeEnum.Create.ToString(), SectionEnum.Lead.ToString(), true, createdAt, null, SectionEnum.Lead.ToString(), PermissionTypeEnum.Create.ToString()},
                    {9, "LeadRead", PermissionTypeEnum.Read.ToString(), SectionEnum.Lead.ToString(), true, createdAt, null, SectionEnum.Lead.ToString(), PermissionTypeEnum.Read.ToString()},
                    {10, "LeadWrite", PermissionTypeEnum.Write.ToString(), SectionEnum.Lead.ToString(), true, createdAt, null, SectionEnum.Lead.ToString(), PermissionTypeEnum.Write.ToString()},
                    {11, "LeadUpdate", PermissionTypeEnum.Update.ToString(), SectionEnum.Lead.ToString(), true, createdAt, null, SectionEnum.Lead.ToString(), PermissionTypeEnum.Update.ToString()},
                    {12, "LeadDelete", PermissionTypeEnum.Delete.ToString(), SectionEnum.Lead.ToString(), true, createdAt, null, SectionEnum.Lead.ToString(), PermissionTypeEnum.Delete.ToString()},
                    {13, "LeadAssign", PermissionTypeEnum.Assign.ToString(), SectionEnum.Lead.ToString(), true, createdAt, null, SectionEnum.Lead.ToString(), PermissionTypeEnum.Assign.ToString()},
                    {14, "LeadNotApplicable", PermissionTypeEnum.NotApplicable.GetDisplayName(), SectionEnum.Lead.ToString(), true, createdAt, null, SectionEnum.Lead.ToString(), PermissionTypeEnum.NotApplicable.ToString()},

                    // Medical Recommendation Section
                    {15, "MedicalRecommendationCreate", PermissionTypeEnum.Create.ToString(), SectionEnum.MedicalRecommendation.GetDisplayName(), true, createdAt, null, SectionEnum.MedicalRecommendation.ToString(), PermissionTypeEnum.Create.ToString()},
                    {16, "MedicalRecommendationRead", PermissionTypeEnum.Read.ToString(), SectionEnum.MedicalRecommendation.GetDisplayName(), true, createdAt, null, SectionEnum.MedicalRecommendation.ToString(), PermissionTypeEnum.Read.ToString()},
                    {17, "MedicalRecommendationWrite", PermissionTypeEnum.Write.ToString(), SectionEnum.MedicalRecommendation.GetDisplayName(), true, createdAt, null, SectionEnum.MedicalRecommendation.ToString(), PermissionTypeEnum.Write.ToString()},
                    { 18, "MedicalRecommendationUpdate", PermissionTypeEnum.Update.ToString(), SectionEnum.MedicalRecommendation.GetDisplayName(), true, createdAt, null, SectionEnum.MedicalRecommendation.ToString(), PermissionTypeEnum.Update.ToString()},
                    {19, "MedicalRecommendationDelete", PermissionTypeEnum.Delete.ToString(), SectionEnum.MedicalRecommendation.GetDisplayName(), true, createdAt, null, SectionEnum.MedicalRecommendation.ToString(), PermissionTypeEnum.Delete.ToString()},
                    { 20, "MedicalRecommendationAssign", PermissionTypeEnum.Assign.ToString(), SectionEnum.MedicalRecommendation.GetDisplayName(), true, createdAt, null, SectionEnum.MedicalRecommendation.ToString(), PermissionTypeEnum.Assign.ToString()},
                    {21, "MedicalRecommendationNotApplicable", PermissionTypeEnum.NotApplicable.GetDisplayName(), SectionEnum.MedicalRecommendation.GetDisplayName(), true, createdAt, null, SectionEnum.MedicalRecommendation.ToString(), PermissionTypeEnum.NotApplicable.ToString()},

                    // Proposal Section
                    {22, "ProposalCreate", PermissionTypeEnum.Create.ToString(), SectionEnum.Proposal.ToString(), true, createdAt, null, SectionEnum.Proposal.ToString(), PermissionTypeEnum.Create.ToString()},
                    {23, "ProposalRead", PermissionTypeEnum.Read.ToString(), SectionEnum.Proposal.ToString(), true, createdAt, null, SectionEnum.Proposal.ToString(), PermissionTypeEnum.Read.ToString()},
                    {24, "ProposalWrite", PermissionTypeEnum.Write.ToString(), SectionEnum.Proposal.ToString(), true, createdAt, null, SectionEnum.Proposal.ToString(), PermissionTypeEnum.Write.ToString()},
                    {25, "ProposalUpdate", PermissionTypeEnum.Update.ToString(), SectionEnum.Proposal.ToString(), true, createdAt, null, SectionEnum.Proposal.ToString(), PermissionTypeEnum.Update.ToString()},
                    {26, "ProposalDelete", PermissionTypeEnum.Delete.ToString(), SectionEnum.Proposal.ToString(), true, createdAt, null, SectionEnum.Proposal.ToString(), PermissionTypeEnum.Delete.ToString()},
                    {27, "ProposalAssign", PermissionTypeEnum.Assign.ToString(), SectionEnum.Proposal.ToString(), true, createdAt, null, SectionEnum.Proposal.ToString(), PermissionTypeEnum.Assign.ToString()},
                    { 28, "ProposalNotApplicable", PermissionTypeEnum.NotApplicable.GetDisplayName(), SectionEnum.Proposal.ToString(), true, createdAt, null, SectionEnum.Proposal.ToString(), PermissionTypeEnum.NotApplicable.ToString() },

                    // Order Section
                    {29, "OrderCreate", PermissionTypeEnum.Create.ToString(), SectionEnum.Order.ToString(), true, createdAt, null, SectionEnum.Order.ToString(), PermissionTypeEnum.Create.ToString()},
                    {30, "OrderRead", PermissionTypeEnum.Read.ToString(), SectionEnum.Order.ToString(), true, createdAt, null, SectionEnum.Order.ToString(), PermissionTypeEnum.Read.ToString()},
                    {31, "OrderWrite", PermissionTypeEnum.Write.ToString(), SectionEnum.Order.ToString(), true, createdAt, null, SectionEnum.Order.ToString(), PermissionTypeEnum.Write.ToString()},
                    {32, "OrderUpdate", PermissionTypeEnum.Update.ToString(), SectionEnum.Order.ToString(), true, createdAt, null, SectionEnum.Order.ToString(), PermissionTypeEnum.Update.ToString()},
                    {33, "OrderDelete", PermissionTypeEnum.Delete.ToString(), SectionEnum.Order.ToString(), true, createdAt, null, SectionEnum.Order.ToString(), PermissionTypeEnum.Delete.ToString()},
                    {34, "OrderAssign", PermissionTypeEnum.Assign.ToString(), SectionEnum.Order.ToString(), true, createdAt, null, SectionEnum.Order.ToString(), PermissionTypeEnum.Assign.ToString()},
                    {35, "OrderNotApplicable", PermissionTypeEnum.NotApplicable.GetDisplayName(), SectionEnum.Order.ToString(), true, createdAt, null, SectionEnum.Order.ToString(), PermissionTypeEnum.NotApplicable.ToString()},

                    // Product Section
                    {36, "ProductCreate", PermissionTypeEnum.Create.ToString(), SectionEnum.Product.ToString(), true, createdAt, null, SectionEnum.Product.ToString(), PermissionTypeEnum.Create.ToString()},
                    {37, "ProductRead", PermissionTypeEnum.Read.ToString(), SectionEnum.Product.ToString(), true, createdAt, null, SectionEnum.Product.ToString(), PermissionTypeEnum.Read.ToString()},
                    {38, "ProductWrite", PermissionTypeEnum.Write.ToString(), SectionEnum.Product.ToString(), true, createdAt, null, SectionEnum.Product.ToString(), PermissionTypeEnum.Write.ToString()},
                    {39, "ProductUpdate", PermissionTypeEnum.Update.ToString(), SectionEnum.Product.ToString(), true, createdAt, null, SectionEnum.Product.ToString(), PermissionTypeEnum.Update.ToString()},
                    {40, "ProductDelete", PermissionTypeEnum.Delete.ToString(), SectionEnum.Product.ToString(), true, createdAt, null, SectionEnum.Product.ToString(), PermissionTypeEnum.Delete.ToString()},
                    {41, "ProductAssign", PermissionTypeEnum.Assign.ToString(), SectionEnum.Product.ToString(), true, createdAt, null, SectionEnum.Product.ToString(), PermissionTypeEnum.Assign.ToString()},
                    {42, "ProductNotApplicable", PermissionTypeEnum.NotApplicable.GetDisplayName(), SectionEnum.Product.ToString(), true, createdAt, null, SectionEnum.Product.ToString(), PermissionTypeEnum.NotApplicable.ToString()},

                    // Pharmacy Section
                    {43, "PharmacyCreate", PermissionTypeEnum.Create.ToString(), SectionEnum.Pharmacy.ToString(), true, createdAt, null, SectionEnum.Pharmacy.ToString(), PermissionTypeEnum.Create.ToString()},
                    {44, "PharmacyRead", PermissionTypeEnum.Read.ToString(), SectionEnum.Pharmacy.ToString(), true, createdAt, null, SectionEnum.Pharmacy.ToString(), PermissionTypeEnum.Read.ToString()},
                    {45, "PharmacyWrite", PermissionTypeEnum.Write.ToString(), SectionEnum.Pharmacy.ToString(), true, createdAt, null, SectionEnum.Pharmacy.ToString(), PermissionTypeEnum.Write.ToString()},
                    {46, "PharmacyUpdate", PermissionTypeEnum.Update.ToString(), SectionEnum.Pharmacy.ToString(), true, createdAt, null, SectionEnum.Pharmacy.ToString(), PermissionTypeEnum.Update.ToString()},
                    {47, "PharmacyDelete", PermissionTypeEnum.Delete.ToString(), SectionEnum.Pharmacy.ToString(), true, createdAt, null, SectionEnum.Pharmacy.ToString(), PermissionTypeEnum.Delete.ToString()},
                    {48, "PharmacyAssign", PermissionTypeEnum.Assign.ToString(), SectionEnum.Pharmacy.ToString(), true, createdAt, null, SectionEnum.Pharmacy.ToString(), PermissionTypeEnum.Assign.ToString()},
                    {49, "PharmacyNotApplicable", PermissionTypeEnum.NotApplicable.GetDisplayName(), SectionEnum.Pharmacy.ToString(), true, createdAt, null, SectionEnum.Pharmacy.ToString(), PermissionTypeEnum.NotApplicable.ToString()},

                    // Product Pharmacy Section
                    {50, "ProductPharmacyPriceCreate", PermissionTypeEnum.Create.ToString(), SectionEnum.ProductPharmacyPrice.GetDisplayName(), true, createdAt, null, SectionEnum.ProductPharmacyPrice.ToString(), PermissionTypeEnum.Create.ToString()},
                    {51, "ProductPharmacyPriceRead", PermissionTypeEnum.Read.ToString(), SectionEnum.ProductPharmacyPrice.GetDisplayName(), true, createdAt, null, SectionEnum.ProductPharmacyPrice.ToString(), PermissionTypeEnum.Read.ToString()},
                    {52, "ProductPharmacyPriceWrite", PermissionTypeEnum.Write.ToString(), SectionEnum.ProductPharmacyPrice.GetDisplayName(), true, createdAt, null, SectionEnum.ProductPharmacyPrice.ToString(), PermissionTypeEnum.Write.ToString()},
                    {53, "ProductPharmacyPriceUpdate", PermissionTypeEnum.Update.ToString(), SectionEnum.ProductPharmacyPrice.GetDisplayName(), true, createdAt, null, SectionEnum.ProductPharmacyPrice.ToString(), PermissionTypeEnum.Update.ToString()},
                    {54, "ProductPharmacyPriceDelete", PermissionTypeEnum.Delete.ToString(), SectionEnum.ProductPharmacyPrice.GetDisplayName(), true, createdAt, null, SectionEnum.ProductPharmacyPrice.ToString(), PermissionTypeEnum.Delete.ToString()},
                    {55, "ProductPharmacyPriceAssign", PermissionTypeEnum.Assign.ToString(), SectionEnum.ProductPharmacyPrice.GetDisplayName(), true, createdAt, null, SectionEnum.ProductPharmacyPrice.ToString(), PermissionTypeEnum.Assign.ToString()},
                    {56, "ProductPharmacyPriceNotApplicable", PermissionTypeEnum.NotApplicable.GetDisplayName(), SectionEnum.ProductPharmacyPrice.GetDisplayName(), true, createdAt, null, SectionEnum.ProductPharmacyPrice.ToString(), PermissionTypeEnum.NotApplicable.ToString()},

                    // Commision Rate Section
                    {57, "CommissionRatePerProductCreate", PermissionTypeEnum.Create.ToString(), SectionEnum.CommissionRatePerProduct.GetDisplayName(), true, createdAt, null, SectionEnum.CommissionRatePerProduct.ToString(), PermissionTypeEnum.Create.ToString()},
                    {58, "CommissionRatePerProductRead", PermissionTypeEnum.Read.ToString(), SectionEnum.CommissionRatePerProduct.GetDisplayName(), true, createdAt, null, SectionEnum.CommissionRatePerProduct.ToString(), PermissionTypeEnum.Read.ToString()},
                    {59, "CommissionRatePerProductWrite", PermissionTypeEnum.Write.ToString(), SectionEnum.CommissionRatePerProduct.GetDisplayName(), true, createdAt, null, SectionEnum.CommissionRatePerProduct.ToString(), PermissionTypeEnum.Write.ToString()},
                    {60, "CommissionRatePerProductUpdate", PermissionTypeEnum.Update.ToString(), SectionEnum.CommissionRatePerProduct.GetDisplayName(), true, createdAt, null, SectionEnum.CommissionRatePerProduct.ToString(), PermissionTypeEnum.Update.ToString()},
                    {61, "CommissionRatePerProductDelete", PermissionTypeEnum.Delete.ToString(), SectionEnum.CommissionRatePerProduct.GetDisplayName(), true, createdAt, null, SectionEnum.CommissionRatePerProduct.ToString(), PermissionTypeEnum.Delete.ToString()},
                    {62, "CommissionRatePerProductAssign", PermissionTypeEnum.Assign.ToString(), SectionEnum.CommissionRatePerProduct.GetDisplayName(), true, createdAt, null, SectionEnum.CommissionRatePerProduct.ToString(), PermissionTypeEnum.Assign.GetDisplayName()},
                    {63, "CommissionRatePerProductNotApplicable", PermissionTypeEnum.NotApplicable.GetDisplayName(), SectionEnum.CommissionRatePerProduct.GetDisplayName(), true, createdAt, null, SectionEnum.CommissionRatePerProduct.ToString(), PermissionTypeEnum.NotApplicable.ToString()},

                    // User Section
                    {64, "UserCreate", PermissionTypeEnum.Create.ToString(), SectionEnum.User.ToString(), true, createdAt, null, SectionEnum.User.ToString(), PermissionTypeEnum.Create.ToString()},
                    {65, "UserRead", PermissionTypeEnum.Read.ToString(), SectionEnum.User.ToString(), true, createdAt, null, SectionEnum.User.ToString(), PermissionTypeEnum.Read.ToString()},
                    {66, "UserWrite", PermissionTypeEnum.Write.ToString(), SectionEnum.User.ToString(), true, createdAt, null, SectionEnum.User.ToString(), PermissionTypeEnum.Write.ToString()},
                    {67, "UserUpdate", PermissionTypeEnum.Update.ToString(), SectionEnum.User.ToString(), true, createdAt, null, SectionEnum.User.ToString(), PermissionTypeEnum.Update.ToString()},
                    {68, "UserDelete", PermissionTypeEnum.Delete.ToString(), SectionEnum.User.ToString(), true, createdAt, null, SectionEnum.User.ToString(), PermissionTypeEnum.Delete.ToString()},
                    {69, "UserAssign", PermissionTypeEnum.Assign.ToString(), SectionEnum.User.ToString(), true, createdAt, null, SectionEnum.User.ToString(), PermissionTypeEnum.Assign.ToString()},
                    {70, "UserNotApplicable", PermissionTypeEnum.NotApplicable.GetDisplayName(), SectionEnum.User.ToString(), true, createdAt, null, SectionEnum.User.ToString(), PermissionTypeEnum.NotApplicable.ToString()},

                    // Shipping Address Section
                    {71, "ShippingAddressCreate", PermissionTypeEnum.Create.ToString(), SectionEnum.ShippingAddress.GetDisplayName(), true, createdAt, null, SectionEnum.ShippingAddress.ToString(), PermissionTypeEnum.Create.ToString()},
                    {72, "ShippingAddressRead", PermissionTypeEnum.Read.ToString(), SectionEnum.ShippingAddress.GetDisplayName(), true, createdAt, null, SectionEnum.ShippingAddress.ToString(), PermissionTypeEnum.Read.ToString()},
                    {73, "ShippingAddressWrite", PermissionTypeEnum.Write.ToString(), SectionEnum.ShippingAddress.GetDisplayName(), true, createdAt, null, SectionEnum.ShippingAddress.ToString(), PermissionTypeEnum.Write.ToString()},
                    {74, "ShippingAddressUpdate", PermissionTypeEnum.Update.ToString(), SectionEnum.ShippingAddress.GetDisplayName(), true, createdAt, null, SectionEnum.ShippingAddress.ToString(), PermissionTypeEnum.Update.ToString()},
                    {75, "ShippingAddressDelete", PermissionTypeEnum.Delete.ToString(), SectionEnum.ShippingAddress.GetDisplayName(), true, createdAt, null, SectionEnum.ShippingAddress.ToString(), PermissionTypeEnum.Delete.ToString()},
                    {76, "ShippingAddressAssign", PermissionTypeEnum.Assign.ToString(), SectionEnum.ShippingAddress.GetDisplayName(), true, createdAt, null, SectionEnum.ShippingAddress.ToString(), PermissionTypeEnum.Assign.ToString()},
                    {77, "ShippingAddressNotApplicable", PermissionTypeEnum.NotApplicable.GetDisplayName(), SectionEnum.ShippingAddress.GetDisplayName(), true, createdAt, null, SectionEnum.ShippingAddress.ToString(), PermissionTypeEnum.NotApplicable.ToString()},

                    // SMSChat Section
                    {78, "SMSChatCreate", PermissionTypeEnum.Create.ToString(), SectionEnum.SMSChat.GetDisplayName(), true, createdAt, null, SectionEnum.SMSChat.ToString(), PermissionTypeEnum.Create.ToString()},
                    {79, "SMSChatRead", PermissionTypeEnum.Read.ToString(), SectionEnum.SMSChat.GetDisplayName(), true, createdAt, null, SectionEnum.SMSChat.ToString(), PermissionTypeEnum.Read.ToString()},
                    {80, "SMSChatWrite", PermissionTypeEnum.Write.ToString(), SectionEnum.SMSChat.GetDisplayName(), true, createdAt, null, SectionEnum.SMSChat.ToString(), PermissionTypeEnum.Write.ToString()},
                    {81, "SMSChatUpdate", PermissionTypeEnum.Update.ToString(), SectionEnum.SMSChat.GetDisplayName(), true, createdAt, null, SectionEnum.SMSChat.ToString(), PermissionTypeEnum.Update.ToString()},
                    {82, "SMSChatDelete", PermissionTypeEnum.Delete.ToString(), SectionEnum.SMSChat.GetDisplayName(), true, createdAt, null, SectionEnum.SMSChat.ToString(), PermissionTypeEnum.Delete.ToString()},
                    {83, "SMSChatAssign", PermissionTypeEnum.Assign.ToString(), SectionEnum.SMSChat.GetDisplayName(), true, createdAt, null, SectionEnum.SMSChat.ToString(), PermissionTypeEnum.Assign.ToString()},
                    {84, "SMSChatNotApplicable", PermissionTypeEnum.NotApplicable.GetDisplayName(), SectionEnum.SMSChat.GetDisplayName(), true, createdAt, null, SectionEnum.SMSChat.ToString(), PermissionTypeEnum.NotApplicable.ToString()},

                    // Bulk Email Section
                    {85, "BulkEmailSMSCreate", PermissionTypeEnum.Create.ToString(), SectionEnum.BulkEmailSMS.GetDisplayName(), true, createdAt, null, SectionEnum.BulkEmailSMS.ToString(), PermissionTypeEnum.Create.ToString()},
                    {86, "BulkEmailSMSRead", PermissionTypeEnum.Read.ToString(), SectionEnum.BulkEmailSMS.GetDisplayName(), true, createdAt, null, SectionEnum.BulkEmailSMS.ToString(), PermissionTypeEnum.Read.ToString()},
                    {87, "BulkEmailSMSWrite", PermissionTypeEnum.Write.ToString(), SectionEnum.BulkEmailSMS.GetDisplayName(), true, createdAt, null, SectionEnum.BulkEmailSMS.ToString(), PermissionTypeEnum.Write.ToString()},
                    {88, "BulkEmailSMSUpdate", PermissionTypeEnum.Update.ToString(), SectionEnum.BulkEmailSMS.GetDisplayName(), true, createdAt, null, SectionEnum.BulkEmailSMS.ToString(), PermissionTypeEnum.Update.ToString()},
                    {89, "BulkEmailSMSDelete", PermissionTypeEnum.Delete.ToString(), SectionEnum.BulkEmailSMS.GetDisplayName(), true, createdAt, null, SectionEnum.BulkEmailSMS.ToString(), PermissionTypeEnum.Delete.ToString()},
                    {90, "BulkEmailSMSAssign", PermissionTypeEnum.Assign.ToString(), SectionEnum.BulkEmailSMS.GetDisplayName(), true, createdAt, null, SectionEnum.BulkEmailSMS.ToString(), PermissionTypeEnum.Assign.ToString()},
                    {91, "BulkEmailSMSNotApplicable", PermissionTypeEnum.NotApplicable.GetDisplayName(), SectionEnum.BulkEmailSMS.GetDisplayName(), true, createdAt, null, SectionEnum.BulkEmailSMS.ToString(), PermissionTypeEnum.NotApplicable.ToString()},

                    // Commision Payment Section
                    {92, "CommissionPaymentCreate", PermissionTypeEnum.Create.ToString(), SectionEnum.CommissionPayment.GetDisplayName(), true, createdAt, null, SectionEnum.CommissionPayment.ToString(), PermissionTypeEnum.Create.ToString()},
                    {93, "CommissionPaymentRead", PermissionTypeEnum.Read.ToString(), SectionEnum.CommissionPayment.GetDisplayName(), true, createdAt, null, SectionEnum.CommissionPayment.ToString(), PermissionTypeEnum.Read.ToString()},
                    {94, "CommissionPaymentWrite", PermissionTypeEnum.Write.ToString(), SectionEnum.CommissionPayment.GetDisplayName(), true, createdAt, null, SectionEnum.CommissionPayment.ToString(), PermissionTypeEnum.Write.ToString()},
                    {95, "CommissionPaymentUpdate", PermissionTypeEnum.Update.ToString(), SectionEnum.CommissionPayment.GetDisplayName(), true, createdAt, null, SectionEnum.CommissionPayment.ToString(), PermissionTypeEnum.Update.ToString()},
                    {96, "CommissionPaymentDelete", PermissionTypeEnum.Delete.ToString(), SectionEnum.CommissionPayment.GetDisplayName(), true, createdAt, null, SectionEnum.CommissionPayment.ToString(), PermissionTypeEnum.Delete.ToString()},
                    {97, "CommissionPaymentAssign", PermissionTypeEnum.Assign.ToString(), SectionEnum.CommissionPayment.GetDisplayName(), true, createdAt, null, SectionEnum.CommissionPayment.ToString(), PermissionTypeEnum.Assign.ToString()},
                    {98, "CommissionPaymentNotApplicable", PermissionTypeEnum.NotApplicable.GetDisplayName(), SectionEnum.CommissionPayment.GetDisplayName(), true, createdAt, null, SectionEnum.CommissionPayment.ToString(), PermissionTypeEnum.NotApplicable.ToString()},

                    // Appointment Booking Section
                    {99, "AppointmentBookingCreate", PermissionTypeEnum.Create.ToString(), SectionEnum.AppointmentBooking.GetDisplayName(), true, createdAt, null, SectionEnum.AppointmentBooking.ToString(), PermissionTypeEnum.Create.ToString()},
                    {100, "AppointmentBookingRead", PermissionTypeEnum.Read.ToString(), SectionEnum.AppointmentBooking.GetDisplayName(), true, createdAt, null, SectionEnum.AppointmentBooking.ToString(), PermissionTypeEnum.Read.ToString()},
                    {101, "AppointmentBookingWrite", PermissionTypeEnum.Write.ToString(), SectionEnum.AppointmentBooking.GetDisplayName(), true, createdAt, null, SectionEnum.AppointmentBooking.ToString(), PermissionTypeEnum.Write.ToString()},
                    {102, "AppointmentBookingUpdate", PermissionTypeEnum.Update.ToString(), SectionEnum.AppointmentBooking.GetDisplayName(), true, createdAt, null, SectionEnum.AppointmentBooking.ToString(), PermissionTypeEnum.Update.ToString()},
                    {103, "AppointmentBookingDelete", PermissionTypeEnum.Delete.ToString(), SectionEnum.AppointmentBooking.GetDisplayName(), true, createdAt, null, SectionEnum.AppointmentBooking.ToString(), PermissionTypeEnum.Delete.ToString()},
                    {104, "AppointmentBookingAssign", PermissionTypeEnum.Assign.ToString(), SectionEnum.AppointmentBooking.GetDisplayName(), true, createdAt, null, SectionEnum.AppointmentBooking.ToString(), PermissionTypeEnum.Assign.ToString()},
                    {105, "AppointmentBookingNotApplicable", PermissionTypeEnum.NotApplicable.GetDisplayName(), SectionEnum.AppointmentBooking.GetDisplayName(), true, createdAt, null, SectionEnum.AppointmentBooking.ToString(), PermissionTypeEnum.NotApplicable.ToString()},

                    // Appointment Calender Section
                    {106, "AppointmentCalendarCreate", PermissionTypeEnum.Create.ToString(), SectionEnum.AppointmentCalendar.GetDisplayName(), true, createdAt, null, SectionEnum.AppointmentCalendar.ToString(), PermissionTypeEnum.Create.ToString()},
                    {107, "AppointmentCalendarRead", PermissionTypeEnum.Read.ToString(), SectionEnum.AppointmentCalendar.GetDisplayName(), true, createdAt, null, SectionEnum.AppointmentCalendar.ToString(), PermissionTypeEnum.Read.ToString()},
                    {108, "AppointmentCalendarWrite", PermissionTypeEnum.Write.ToString(), SectionEnum.AppointmentCalendar.GetDisplayName(), true, createdAt, null, SectionEnum.AppointmentCalendar.ToString(), PermissionTypeEnum.Write.ToString()},
                    {109, "AppointmentCalendarUpdate", PermissionTypeEnum.Update.ToString(), SectionEnum.AppointmentCalendar.GetDisplayName(), true, createdAt, null, SectionEnum.AppointmentCalendar.ToString(), PermissionTypeEnum.Update.ToString()},
                    {110, "AppointmentCalendarDelete", PermissionTypeEnum.Delete.ToString(), SectionEnum.AppointmentCalendar.GetDisplayName(), true, createdAt, null, SectionEnum.AppointmentCalendar.ToString(), PermissionTypeEnum.Delete.ToString()},
                    {111, "AppointmentCalendarAssign", PermissionTypeEnum.Assign.ToString(), SectionEnum.AppointmentCalendar.GetDisplayName(), true, createdAt, null, SectionEnum.AppointmentCalendar.ToString(), PermissionTypeEnum.Assign.ToString()},
                    {112, "AppointmentCalendarNotApplicable", PermissionTypeEnum.NotApplicable.GetDisplayName(), SectionEnum.AppointmentCalendar.GetDisplayName(), true, createdAt, null, SectionEnum.AppointmentCalendar.ToString(), PermissionTypeEnum.NotApplicable.ToString()},

                    // Appointment Time Section
                    {113, "AppointmentTimeCreate", PermissionTypeEnum.Create.ToString(), SectionEnum.AppointmentTime.GetDisplayName(), true, createdAt, null, SectionEnum.AppointmentTime.ToString(), PermissionTypeEnum.Create.ToString()},
                    {114, "AppointmentTimeRead", PermissionTypeEnum.Read.ToString(), SectionEnum.AppointmentTime.GetDisplayName(), true, createdAt, null, SectionEnum.AppointmentTime.ToString(), PermissionTypeEnum.Read.ToString()},
                    {115, "AppointmentTimeWrite", PermissionTypeEnum.Write.ToString(), SectionEnum.AppointmentTime.GetDisplayName(), true, createdAt, null, SectionEnum.AppointmentTime.ToString(), PermissionTypeEnum.Write.ToString()},
                    {116, "AppointmentTimeUpdate", PermissionTypeEnum.Update.ToString(), SectionEnum.AppointmentTime.GetDisplayName(), true, createdAt, null, SectionEnum.AppointmentTime.ToString(), PermissionTypeEnum.Update.ToString()},
                    {117, "AppointmentTimeDelete", PermissionTypeEnum.Delete.ToString(), SectionEnum.AppointmentTime.GetDisplayName(), true, createdAt, null, SectionEnum.AppointmentTime.ToString(), PermissionTypeEnum.Delete.ToString()},
                    {118, "AppointmentTimeAssign", PermissionTypeEnum.Assign.ToString(), SectionEnum.AppointmentTime.GetDisplayName(), true, createdAt, null, SectionEnum.AppointmentTime.ToString(), PermissionTypeEnum.Assign.ToString()},
                    {119, "AppointmentTimeNotApplicable", PermissionTypeEnum.NotApplicable.GetDisplayName(), SectionEnum.AppointmentTime.GetDisplayName(), true, createdAt, null, SectionEnum.AppointmentTime.ToString(), PermissionTypeEnum.NotApplicable.ToString()},

                    // Task Section
                    {120, "TaskCreate", PermissionTypeEnum.Create.ToString(), SectionEnum.Task.ToString(), true, createdAt, null, SectionEnum.Task.ToString(), PermissionTypeEnum.Create.ToString()},
                    {121, "TaskRead", PermissionTypeEnum.Read.ToString(), SectionEnum.Task.ToString(), true, createdAt, null, SectionEnum.Task.ToString(), PermissionTypeEnum.Read.ToString()},
                    {122, "TaskWrite", PermissionTypeEnum.Write.ToString(), SectionEnum.Task.ToString(), true, createdAt, null, SectionEnum.Task.ToString(), PermissionTypeEnum.Write.ToString()},
                    {123, "TaskUpdate", PermissionTypeEnum.Update.ToString(), SectionEnum.Task.ToString(), true, createdAt, null, SectionEnum.Task.ToString(), PermissionTypeEnum.Update.ToString()},
                    {124, "TaskDelete", PermissionTypeEnum.Delete.ToString(), SectionEnum.Task.ToString(), true, createdAt, null, SectionEnum.Task.ToString(), PermissionTypeEnum.Delete.ToString()},
                    {125, "TaskAssign", PermissionTypeEnum.Assign.ToString(), SectionEnum.Task.ToString(), true, createdAt, null, SectionEnum.Task.ToString(), PermissionTypeEnum.Assign.ToString()},
                    {126, "TaskNotApplicable", PermissionTypeEnum.NotApplicable.GetDisplayName(), SectionEnum.Task.ToString(), true, createdAt, null, SectionEnum.Task.ToString(), PermissionTypeEnum.NotApplicable.ToString()},
                    });


                // Insert RolePermissions
                migrationBuilder.InsertData(
                    table: "RolePermissions",
                    columns: new[] { "Id", "RoleId", "PermissionId", "IsActive", "CreatedAt", "UpdatedAt" },
                    values: new object[,]
                    {
                    // For Super Admin
                    { 1, 1, 1, true, createdAt, null },
                    { 2, 1, 2, true, createdAt, null },
                    { 3, 1, 3, true, createdAt, null },
                    { 4, 1, 4, true, createdAt, null },
                    { 5, 1, 5, true, createdAt, null },
                    { 6, 1, 6, true, createdAt, null },
                    { 7, 1, 8, true, createdAt, null },
                    { 8, 1, 9, true, createdAt, null },
                    { 9, 1, 10, true, createdAt, null },
                    { 10, 1, 11, true, createdAt, null },
                    { 11, 1, 12, true, createdAt, null },
                    { 12, 1, 13, true, createdAt, null },
                    { 13, 1, 15, true, createdAt, null },
                    { 14, 1, 16, true, createdAt, null },
                    { 15, 1, 17, true, createdAt, null },
                    { 16, 1, 18, true, createdAt, null },
                    { 17, 1, 19, true, createdAt, null },
                    { 18, 1, 20, true, createdAt, null },
                    { 19, 1, 22, true, createdAt, null },
                    { 20, 1, 23, true, createdAt, null },
                    { 21, 1, 24, true, createdAt, null },
                    { 22, 1, 25, true, createdAt, null },
                    { 23, 1, 26, true, createdAt, null },
                    { 24, 1, 27, true, createdAt, null },
                    { 25, 1, 29, true, createdAt, null },
                    { 26, 1, 30, true, createdAt, null },
                    { 27, 1, 31, true, createdAt, null },
                    { 28, 1, 32, true, createdAt, null },
                    { 29, 1, 33, true, createdAt, null },
                    { 30, 1, 34, true, createdAt, null },
                    { 31, 1, 36, true, createdAt, null },
                    { 32, 1, 37, true, createdAt, null },
                    { 33, 1, 38, true, createdAt, null },
                    { 34, 1, 39, true, createdAt, null },
                    { 35, 1, 40, true, createdAt, null },
                    { 36, 1, 41, true, createdAt, null },
                    { 37, 1, 43, true, createdAt, null },
                    { 38, 1, 44, true, createdAt, null },
                    { 39, 1, 45, true, createdAt, null },
                    { 40, 1, 46, true, createdAt, null },
                    { 41, 1, 47, true, createdAt, null },
                    { 42, 1, 48, true, createdAt, null },
                    { 43, 1, 50, true, createdAt, null },
                    { 44, 1, 51, true, createdAt, null },
                    { 45, 1, 52, true, createdAt, null },
                    { 46, 1, 53, true, createdAt, null },
                    { 47, 1, 54, true, createdAt, null },
                    { 48, 1, 55, true, createdAt, null },
                    { 49, 1, 57, true, createdAt, null },
                    { 50, 1, 58, true, createdAt, null },
                    { 51, 1, 59, true, createdAt, null },
                    { 52, 1, 60, true, createdAt, null },

                    { 53, 1, 61, true, createdAt, null },
                    { 54, 1, 62, true, createdAt, null },
                    { 55, 1, 64, true, createdAt, null },
                    { 56, 1, 65, true, createdAt, null },
                    { 57, 1, 66, true, createdAt, null },
                    { 58, 1, 67, true, createdAt, null },
                    { 59, 1, 68, true, createdAt, null },
                    { 60, 1, 69, true, createdAt, null },
                    { 61, 1, 71, true, createdAt, null },
                    { 62, 1, 72, true, createdAt, null },
                    { 63, 1, 73, true, createdAt, null },
                    { 64, 1, 74, true, createdAt, null },
                    { 65, 1, 75, true, createdAt, null },
                    { 66, 1, 76, true, createdAt, null },
                    { 67, 1, 78, true, createdAt, null },
                    { 68, 1, 79, true, createdAt, null },
                    { 69, 1, 80, true, createdAt, null },
                    { 70, 1, 81, true, createdAt, null },
                    { 71, 1, 82, true, createdAt, null },
                    { 72, 1, 83, true, createdAt, null },
                    { 73, 1, 85, true, createdAt, null },
                    { 74, 1, 86, true, createdAt, null },
                    { 75, 1, 87, true, createdAt, null },
                    { 76, 1, 88, true, createdAt, null },
                    { 77, 1, 89, true, createdAt, null },
                    { 78, 1, 90, true, createdAt, null },
                    { 79, 1, 92, true, createdAt, null },
                    { 80, 1, 93, true, createdAt, null },
                    { 81, 1, 94, true, createdAt, null },
                    { 82, 1, 95, true, createdAt, null },
                    { 83, 1, 96, true, createdAt, null },
                    { 84, 1, 97, true, createdAt, null },
                    { 85, 1, 99, true, createdAt, null },
                    { 86, 1, 100, true, createdAt, null },
                    { 87, 1, 101, true, createdAt, null },
                    { 88, 1, 102, true, createdAt, null },
                    { 89, 1, 103, true, createdAt, null },
                    { 90, 1, 104, true, createdAt, null },
                    { 91, 1, 106, true, createdAt, null },
                    { 92, 1, 107, true, createdAt, null },
                    { 93, 1, 108, true, createdAt, null },
                    { 94, 1, 109, true, createdAt, null },
                    { 95, 1, 110, true, createdAt, null },
                    { 96, 1, 111, true, createdAt, null },
                    { 97, 1, 113, true, createdAt, null },
                    { 98, 1, 114, true, createdAt, null },
                    { 99, 1, 115, true, createdAt, null },
                    { 100, 1, 116, true, createdAt, null },
                    { 101, 1, 117, true, createdAt, null },
                    { 102, 1, 118, true, createdAt, null },
                    { 103, 1, 120, true, createdAt, null },
                    { 104, 1, 121, true, createdAt, null },
                    { 105, 1, 122, true, createdAt, null },
                    { 106, 1, 123, true, createdAt, null },
                    { 107, 1, 124, true, createdAt, null },
                    { 108, 1, 125, true, createdAt, null },

                    // For Admin Patient

                    { 109, 2, 1, true, createdAt, null },
                    { 110, 2, 2, true, createdAt, null },
                    { 111, 2, 3, true, createdAt, null },
                    { 112, 2, 4, true, createdAt, null },
                    { 113, 2, 5, true, createdAt, null },
                    { 114, 2, 6, true, createdAt, null },

                    // For Admin Lead
                    { 115, 2, 8, true, createdAt, null },
                    { 116, 2, 9, true, createdAt, null },
                    { 117, 2, 10, true, createdAt, null },
                    { 118, 2, 11, true, createdAt, null },
                    { 119, 2, 12, true, createdAt, null },
                    { 120, 2, 13, true, createdAt, null },

                    // For Admin Medical Recommendation
                    { 121, 2, 16, true, createdAt, null },

                    // For Admin Proposal
                    { 122, 2, 22, true, createdAt, null },
                    { 123, 2, 23, true, createdAt, null },
                    { 124, 2, 24, true, createdAt, null },
                    { 125, 2, 25, true, createdAt, null },
                    { 126, 2, 26, true, createdAt, null },
                    { 127, 2, 27, true, createdAt, null },

                    // For Admin Order
                    { 128, 2, 29, true, createdAt, null },
                    { 129, 2, 30, true, createdAt, null },
                    { 130, 2, 31, true, createdAt, null },
                    { 131, 2, 32, true, createdAt, null },
                    { 132, 2, 33, true, createdAt, null },
                    { 133, 2, 34, true, createdAt, null },

                    // For Admin Product
                    { 134, 2, 36, true, createdAt, null },
                    { 135, 2, 37, true, createdAt, null },
                    { 136, 2, 38, true, createdAt, null },
                    { 137, 2, 39, true, createdAt, null },
                    { 138, 2, 40, true, createdAt, null },
                    { 139, 2, 41, true, createdAt, null },

                    // For Admin Pharmacy
                    { 140, 2, 43, true, createdAt, null },
                    { 141, 2, 44, true, createdAt, null },
                    { 142, 2, 45, true, createdAt, null },
                    { 143, 2, 46, true, createdAt, null },
                    { 144, 2, 47, true, createdAt, null },
                    { 145, 2, 48, true, createdAt, null },

                    // For Admin Product Pharmacy Price
                    { 146, 2, 50, true, createdAt, null },
                    { 147, 2, 51, true, createdAt, null },
                    { 148, 2, 52, true, createdAt, null },
                    { 149, 2, 53, true, createdAt, null },
                    { 150, 2, 54, true, createdAt, null },
                    { 151, 2, 55, true, createdAt, null },

                    // For Admin Commision Rate
                    { 152, 2, 58, true, createdAt, null },

                    // For Admin User
                    { 153, 2, 64, true, createdAt, null },
                    { 154, 2, 65, true, createdAt, null },
                    { 155, 2, 66, true, createdAt, null },
                    { 156, 2, 67, true, createdAt, null },
                    { 157, 2, 68, true, createdAt, null },
                    { 158, 2, 69, true, createdAt, null },

                    // For Admin Shipping Address
                    { 159, 2, 71, true, createdAt, null },
                    { 160, 2, 72, true, createdAt, null },
                    { 161, 2, 73, true, createdAt, null },
                    { 162, 2, 74, true, createdAt, null },
                    { 163, 2, 75, true, createdAt, null },
                    { 164, 2, 76, true, createdAt, null },

                    // For Admin SMS Chat
                    { 165, 2, 79, true, createdAt, null },

                    // For Admin BulEmail/SMS
                    { 166, 2, 85, true, createdAt, null },
                    { 167, 2, 86, true, createdAt, null },
                    { 168, 2, 87, true, createdAt, null },
                    { 169, 2, 88, true, createdAt, null },
                    { 170, 2, 89, true, createdAt, null },
                    { 171, 2, 90, true, createdAt, null },

                    // For Admin Commision Payment
                    { 172, 2, 92, true, createdAt, null },
                    { 173, 2, 93, true, createdAt, null },
                    { 174, 2, 94, true, createdAt, null },
                    { 175, 2, 95, true, createdAt, null },
                    { 176, 2, 96, true, createdAt, null },
                    { 177, 2, 97, true, createdAt, null },

                    // For Admin Appointment Booking
                    { 178, 2, 99, true, createdAt, null },
                    { 179, 2, 100, true, createdAt, null },
                    { 180, 2, 101, true, createdAt, null },
                    { 181, 2, 102, true, createdAt, null },
                    { 182, 2, 103, true, createdAt, null },
                    { 183, 2, 104, true, createdAt, null },

                    // For Admin Appointment Calender
                    { 184, 2, 107, true, createdAt, null },

                    // For Admin Appointment Time
                    { 185, 2, 113, true, createdAt, null },
                    { 186, 2, 114, true, createdAt, null },
                    { 187, 2, 115, true, createdAt, null },
                    { 188, 2, 116, true, createdAt, null },
                    { 189, 2, 117, true, createdAt, null },
                    { 190, 2, 118, true, createdAt, null },

                    // For Admin Task
                    { 191, 2, 120, true, createdAt, null },
                    { 192, 2, 121, true, createdAt, null },
                    { 193, 2, 122, true, createdAt, null },
                    { 194, 2, 123, true, createdAt, null },
                    { 195, 2, 124, true, createdAt, null },
                    { 196, 2, 125, true, createdAt, null },


                    // For Receptionist Patient
                    { 197, 3, 1, true, createdAt, null },
                    { 198, 3, 2, true, createdAt, null },
                    { 199, 3, 3, true, createdAt, null },
                    { 200, 3, 4, true, createdAt, null },
                    { 201, 3, 6, true, createdAt, null },

                    // For Receptionist Lead
                    { 202, 3, 8, true, createdAt, null },
                    { 203, 3, 9, true, createdAt, null },
                    { 204, 3, 10, true, createdAt, null },
                    { 205, 3, 11, true, createdAt, null },
                    { 206, 3, 13, true, createdAt, null },

                    // For Receptionist Medical Recommendation
                    { 207, 3, 16, true, createdAt, null },

                    // For Receptionist Proposal
                    { 208, 3, 22, true, createdAt, null },
                    { 209, 3, 23, true, createdAt, null },
                    { 210, 3, 24, true, createdAt, null },
                    { 211, 3, 25, true, createdAt, null },
                    { 212, 3, 27, true, createdAt, null },

                    // For Receptionist Order
                    { 213, 3, 29, true, createdAt, null },
                    { 214, 3, 30, true, createdAt, null },
                    { 215, 3, 31, true, createdAt, null },
                    { 216, 3, 32, true, createdAt, null },
                    { 217, 3, 34, true, createdAt, null },

                    // For Receptionist Product
                    { 218, 3, 36, true, createdAt, null },
                    { 219, 3, 37, true, createdAt, null },
                    { 220, 3, 38, true, createdAt, null },
                    { 221, 3, 39, true, createdAt, null },
                    { 222, 3, 41, true, createdAt, null },

                    // For Receptionist Pharmacy
                    { 223, 3, 43, true, createdAt, null },
                    { 224, 3, 44, true, createdAt, null },
                    { 225, 3, 45, true, createdAt, null },
                    { 226, 3, 46, true, createdAt, null },
                    { 227, 3, 48, true, createdAt, null },

                    // For Receptionist Product Pharmacy Price
                    { 228, 3, 50, true, createdAt, null },
                    { 229, 3, 51, true, createdAt, null },
                    { 230, 3, 52, true, createdAt, null },
                    { 231, 3, 53, true, createdAt, null },
                    { 232, 3, 55, true, createdAt, null },

                    // For Receptionist Commision Rate
                    { 233, 3, 57, true, createdAt, null },
                    { 234, 3, 58, true, createdAt, null },
                    { 235, 3, 59, true, createdAt, null },
                    { 236, 3, 60, true, createdAt, null },
                    { 237, 3, 62, true, createdAt, null },

                    // For Receptionist User
                    { 238, 3, 64, true, createdAt, null },
                    { 239, 3, 65, true, createdAt, null },
                    { 240, 3, 66, true, createdAt, null },
                    { 241, 3, 67, true, createdAt, null },
                    { 242, 3, 69, true, createdAt, null },

                    // For Receptionist Shipping Address
                    { 243, 3, 71, true, createdAt, null },
                    { 244, 3, 72, true, createdAt, null },
                    { 245, 3, 73, true, createdAt, null },
                    { 246, 3, 74, true, createdAt, null },
                    { 247, 3, 76, true, createdAt, null },

                    // For Receptionist SMS Chat
                    { 248, 3, 79, true, createdAt, null },

                    // For Receptionist BulkEmail/SMS
                    { 249, 3, 85, true, createdAt, null },
                    { 250, 3, 86, true, createdAt, null },
                    { 251, 3, 87, true, createdAt, null },
                    { 252, 3, 88, true, createdAt, null },
                    { 253, 3, 90, true, createdAt, null },

                    // For Receptionist Commision Payment
                    { 254, 3, 98, true, createdAt, null },

                    // For Receptionist Appointment Booking
                    { 255, 3, 99, true, createdAt, null },
                    { 256, 3, 100, true, createdAt, null },
                    { 257, 3, 101, true, createdAt, null },
                    { 258, 3, 102, true, createdAt, null },
                    { 259, 3, 104, true, createdAt, null },

                    // For Receptionist Appointment Calender
                    { 260, 3, 107, true, createdAt, null },

                    // For Receptionist Appointment Time
                    { 261, 3, 119, true, createdAt, null },

                    // For Receptionist Task
                    { 262, 3, 120, true, createdAt, null },
                    { 263, 3, 121, true, createdAt, null },
                    { 264, 3, 122, true, createdAt, null },
                    { 265, 3, 123, true, createdAt, null },
                    { 266, 3, 124, true, createdAt, null },
                    { 267, 3, 125, true, createdAt, null },

                    // For Nurse Patient
                    { 268, 4, 1, true, createdAt, null },
                    { 269, 4, 2, true, createdAt, null },
                    { 270, 4, 3, true, createdAt, null },
                    { 271, 4, 4, true, createdAt, null },
                    { 272, 4, 6, true, createdAt, null },

                    // For Nurse Lead
                    { 273, 4, 8, true, createdAt, null },
                    { 274, 4, 9, true, createdAt, null },
                    { 275, 4, 10, true, createdAt, null },
                    { 276, 4, 11, true, createdAt, null },
                    { 277, 4, 13, true, createdAt, null },

                    // For Nurse Medical Recommendation
                    { 278, 4, 16, true, createdAt, null },

                    // For Nurse Proposal
                    { 279, 4, 22, true, createdAt, null },
                    { 280, 4, 23, true, createdAt, null },
                    { 281, 4, 24, true, createdAt, null },
                    { 282, 4, 25, true, createdAt, null },
                    { 283, 4, 27, true, createdAt, null },

                    // For Nurse Order
                    { 284, 4, 29, true, createdAt, null },
                    { 285, 4, 30, true, createdAt, null },
                    { 286, 4, 31, true, createdAt, null },
                    { 287, 4, 32, true, createdAt, null },
                    { 288, 4, 34, true, createdAt, null },

                    // For Nurse Product
                    { 289, 4, 36, true, createdAt, null },
                    { 290, 4, 37, true, createdAt, null },
                    { 291, 4, 38, true, createdAt, null },
                    { 292, 4, 39, true, createdAt, null },
                    { 293, 4, 41, true, createdAt, null },

                    // For Nurse Pharmacy
                    { 294, 4, 43, true, createdAt, null },
                    { 295, 4, 44, true, createdAt, null },
                    { 296, 4, 45, true, createdAt, null },
                    { 297, 4, 46, true, createdAt, null },
                    { 298, 4, 48, true, createdAt, null },

                    // For Nurse Product Pharmacy Price
                    { 299, 4, 50, true, createdAt, null },
                    { 300, 4, 51, true, createdAt, null },
                    { 301, 4, 52, true, createdAt, null },
                    { 302, 4, 53, true, createdAt, null },
                    { 303, 4, 55, true, createdAt, null },

                    // For Nurse Commision Rate
                    { 304, 4, 57, true, createdAt, null },
                    { 305, 4, 58, true, createdAt, null },
                    { 306, 4, 59, true, createdAt, null },
                    { 307, 4, 60, true, createdAt, null },
                    { 308, 4, 62, true, createdAt, null },

                    // For Nurse User
                    { 309, 4, 64, true, createdAt, null },
                    { 310, 4, 65, true, createdAt, null },
                    { 311, 4, 66, true, createdAt, null },
                    { 312, 4, 67, true, createdAt, null },
                    { 313, 4, 69, true, createdAt, null },

                    // For Nurse Shipping Address
                    { 314, 4, 71, true, createdAt, null },
                    { 315, 4, 72, true, createdAt, null },
                    { 316, 4, 73, true, createdAt, null },
                    { 317, 4, 74, true, createdAt, null },
                    { 318, 4, 76, true, createdAt, null },

                    // For Nurse SMS Chat
                    { 319, 4, 79, true, createdAt, null },

                    // For Nurse BulkEmail/SMS
                    { 320, 4, 85, true, createdAt, null },
                    { 321, 4, 86, true, createdAt, null },
                    { 322, 4, 87, true, createdAt, null },
                    { 323, 4, 88, true, createdAt, null },
                    { 324, 4, 90, true, createdAt, null },

                    // For Nurse Commision Payment
                    { 325, 4, 98, true, createdAt, null },

                    // For Nurse Appointment Booking
                    { 326, 4, 99, true, createdAt, null },
                    { 327, 4, 100, true, createdAt, null },
                    { 328, 4, 101, true, createdAt, null },
                    { 329, 4, 102, true, createdAt, null },
                    { 330, 4, 104, true, createdAt, null },

                    // For Nurse Appointment Calender
                    { 331, 4, 107, true, createdAt, null },

                    // For Nurse Appointment Time
                    { 332, 4, 119, true, createdAt, null },

                    // For Nurse Task
                    { 333, 4, 120, true, createdAt, null },
                    { 334, 4, 121, true, createdAt, null },
                    { 335, 4, 122, true, createdAt, null },
                    { 336, 4, 123, true, createdAt, null },
                    { 337, 4, 124, true, createdAt, null },
                    { 338, 4, 125, true, createdAt, null },


                    // For Doctor Patient
                    { 339, 5, 2, true, createdAt, null },

                    // For Doctor Lead
                    { 340, 5, 14, true, createdAt, null },

                    // For Doctor Medical Recommendation
                    { 341, 5, 15, true, createdAt, null },
                    { 342, 5, 16, true, createdAt, null },
                    { 343, 5, 17, true, createdAt, null },
                    { 344, 5, 18, true, createdAt, null },
                    { 345, 5, 19, true, createdAt, null },
                    { 346, 5, 20, true, createdAt, null },

                    // For Doctor Proposal
                    { 347, 5, 28, true, createdAt, null },

                    // For Doctor Order
                    { 348, 5, 30, true, createdAt, null },

                    // For Doctor Product
                    { 349, 5, 37, true, createdAt, null },

                    // For Doctor Pharmacy
                    { 350, 5, 44, true, createdAt, null },

                    // For Doctor Product Pharmacy Price
                    { 351, 5, 51, true, createdAt, null },

                    // For Doctor Commision Rate
                    { 352, 5, 63, true, createdAt, null },

                    // For Doctor User
                    { 353, 5, 70, true, createdAt, null },

                    // For Doctor Shipping Address
                    { 354, 5, 72, true, createdAt, null },

                    // For Doctor SMS Chat
                    { 355, 5, 84, true, createdAt, null },

                    // For Doctor Bulk Email
                    { 356, 5, 91, true, createdAt, null },

                    // For Doctor Commision Payment
                    { 357, 5, 98, true, createdAt, null },

                    // For Doctor Appointment Booking
                    { 358, 5, 99, true, createdAt, null },
                    { 359, 5, 100, true, createdAt, null },
                    { 360, 5, 101, true, createdAt, null },
                    { 361, 5, 102, true, createdAt, null },
                    { 362, 5, 104, true, createdAt, null },

                    // For Doctor Appointment Calender
                    { 363, 5, 107, true, createdAt, null },

                    // For Doctor Appointment Time
                    { 364, 5, 119, true, createdAt, null },

                    // For Doctor Task
                    { 365, 5, 120, true, createdAt, null },
                    { 366, 5, 121, true, createdAt, null },
                    { 367, 5, 122, true, createdAt, null },
                    { 368, 5, 123, true, createdAt, null },
                    { 369, 5, 124, true, createdAt, null },
                    { 370, 5, 125, true, createdAt, null },


                    // For SalesPerson Patient
                    { 371, 6, 2, true, createdAt, null },

                    // For SalesPerson Lead
                    { 372, 6, 9, true, createdAt, null },

                    // For SalesPerson Medical Recommendation
                    { 373, 6, 16, true, createdAt, null },

                    // For SalesPerson Proposal
                    { 374, 6, 22, true, createdAt, null },
                    { 375, 6, 23, true, createdAt, null },
                    { 376, 6, 24, true, createdAt, null },
                    { 377, 6, 25, true, createdAt, null },
                    { 378, 6, 27, true, createdAt, null },

                    // For SalesPerson Order
                    { 379, 6, 29, true, createdAt, null },
                    { 380, 6, 30, true, createdAt, null },
                    { 381, 6, 31, true, createdAt, null },
                    { 382, 6, 32, true, createdAt, null },
                    { 383, 6, 34, true, createdAt, null },

                    // For SalesPerson Product
                    { 384, 6, 37, true, createdAt, null },

                    // For SalesPerson Pharmacy
                    { 385, 6, 44, true, createdAt, null },

                    // For SalesPerson Product Pharmacy Price
                    { 386, 6, 51, true, createdAt, null },
                    { 387, 6, 55, true, createdAt, null },

                    // For SalesPerson Commision Rate
                    { 388, 6, 63, true, createdAt, null },

                    // For SalesPerson User
                    { 389, 6, 70, true, createdAt, null },

                    // For SalesPerson Shipping Address
                    { 390, 6, 71, true, createdAt, null },
                    { 391, 6, 72, true, createdAt, null },
                    { 392, 6, 73, true, createdAt, null },
                    { 393, 6, 74, true, createdAt, null },
                    { 394, 6, 75, true, createdAt, null },

                    // For SalesPerson SMS Chat
                    { 395, 6, 78, true, createdAt, null },
                    { 396, 6, 79, true, createdAt, null },
                    { 397, 6, 80, true, createdAt, null },
                    { 398, 6, 81, true, createdAt, null },

                    // For SalesPerson Bulk Email/SMS
                    { 399, 6, 85, true, createdAt, null },
                    { 400, 6, 86, true, createdAt, null },
                    { 401, 6, 87, true, createdAt, null },
                    { 402, 6, 88, true, createdAt, null },

                    // For SalesPerson Commision payment
                    { 403, 6, 98, true, createdAt, null },

                    // For SalesPerson Appointment Booking
                    { 404, 6, 99, true, createdAt, null },
                    { 405, 6, 100, true, createdAt, null },
                    { 406, 6, 101, true, createdAt, null },
                    { 407, 6, 102, true, createdAt, null },
                    { 408, 6, 104, true, createdAt, null },

                    // For SalesPerson Appointment Calender
                    { 409, 6, 107, true, createdAt, null },

                    // For SalesPerson Appointment Time
                    { 410, 6, 119, true, createdAt, null },

                    // For SalesPerson Task
                    { 411, 6, 120, true, createdAt, null },
                    { 412, 6, 121, true, createdAt, null },
                    { 413, 6, 122, true, createdAt, null },
                    { 414, 6, 123, true, createdAt, null },
                    { 415, 6, 124, true, createdAt, null },
                    { 416, 6, 125, true, createdAt, null },
                    });

                return migrationBuilder;

            }

            public static MigrationBuilder DeleteWritePermissions(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.Sql($@"
                    DELETE RP
                    FROM [dbo].[RolePermissions] RP
                    INNER JOIN [dbo].[Permissions] P ON RP.PermissionId = P.Id
                    WHERE P.Type = '{PermissionTypeEnum.Write.ToString()}';

                    DELETE FROM [dbo].[Permissions]
                    WHERE Type = '{PermissionTypeEnum.Write.ToString()}';
                ");
                return migrationBuilder;
            }

            public static MigrationBuilder InsertServicesData(MigrationBuilder migrationBuilder)
            {
                var createdAt = DateTime.UtcNow;
                migrationBuilder.InsertData(
                    table: "Services",
                    columns: new[] { "Id", "ServiceName", "ServiceType", "DisplayName", "CreatedAt", "IsActive" },
                    values: new object[,]
                    {
                        {
                            Guid.Parse("a1f5c9d2-e946-4f72-9e7c-1aa5cb4f9c01"),
                            "NewConsultation",
                            "Appointment",
                            "New Consultation",
                            createdAt,
                            true
                        },
                        {
                            Guid.Parse("b2e6d0f3-f2e3-4c8e-8c2a-234dcf9bda02"),
                            "FollowUp",
                            "Appointment",
                            "Follow Up",
                            createdAt,
                            true
                        },
                        {
                            Guid.Parse("c3a7e1a4-3f31-42df-bd98-a4567aee6a03"),
                            "BloodDraw",
                            "Appointment",
                            "Blood Draw",
                            createdAt,
                            true
                        },
                        {
                            Guid.Parse("d4b8f2b5-8e40-4cdd-91ab-19c8f3ec7c04"),
                            "Injection",
                            "Appointment",
                            "Injection",
                            createdAt,
                            true
                        },
                        {
                            Guid.Parse("e5c9a3c6-1f59-4712-963b-5a123d4a8b05"),
                            "Pellets",
                            "Appointment",
                            "Pellets",
                            createdAt,
                            true
                        },
                        {
                            Guid.Parse("f6dab4d7-1e61-4a1e-94b4-b789ea6f6c06"),
                            "GetFatLoss",
                            "Appointment",
                            "Weight Loss",
                            createdAt,
                            true
                        }
                    });
                return migrationBuilder;
            }
            public static MigrationBuilder InsertProductWebFormData(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.InsertData(
                    table: "ProductWebForms",
                    columns: new[] { "Id", "Code", "Name", "IsActive", "CreatedAt" },
                    values: new object[,]
                    {
                    { 1, "100000001", "Capsule", true, DateTime.UtcNow},
                    { 2, "100000003", "Cream" , true, DateTime.UtcNow},
                    { 3, "100000000", "Injectable", true, DateTime.UtcNow},
                    { 4, "100000005", "Liquid", true, DateTime.UtcNow},
                    { 5, "100000004", "Powder", true, DateTime.UtcNow},
                    { 6, "100000006", "Syringe", true, DateTime.UtcNow},
                    { 7, "100000002", "Troche", true , DateTime.UtcNow},
                    });

                return migrationBuilder;
            }
            public static MigrationBuilder InsertProductTypesData(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.InsertData(
                    table: "ProductTypes",
                    columns: new[] { "Id", "Code", "Name", "IsActive", "CreatedAt" },
                    values: new object[,]
                    {
                    { 1, "100000001", "Capsule", true, DateTime.UtcNow},
                    { 2, "100000003", "Cream" , true, DateTime.UtcNow},
                    { 3, "100000000", "Injectable", true, DateTime.UtcNow},
                    { 4, "100000002", "Troche", true , DateTime.UtcNow},
                    });

                return migrationBuilder;
            }
            public static MigrationBuilder InsertProductCategoriesData(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.InsertData(
                    table: "ProductCategories",
                    columns: new[] { "Id", "Code", "Name", "IsActive", "CreatedAt" },
                    values: new object[,]
                    {
                    { 1, "100000009", "Age Management", true, DateTime.UtcNow},
                    { 2, "100000000", "Brain Function" , true, DateTime.UtcNow},
                    { 3, "100000002", "Female Hormones", true, DateTime.UtcNow},
                    { 4, "100000006", "Growth Hormone", true , DateTime.UtcNow},
                    { 5, "100000012", "Hair Growth", true , DateTime.UtcNow},
                    { 6, "100000011", "Immune Support", true , DateTime.UtcNow},
                    { 7, "100000003", "Injectable Vitamins", true , DateTime.UtcNow},
                    { 8, "100000001", "Male Hormones", true , DateTime.UtcNow},
                    { 9, "100000004", "Peptides", true , DateTime.UtcNow},
                    {10, "100000008", "Sexual Dysfunction", true , DateTime.UtcNow},
                    {11, "100000010", "Standard", true , DateTime.UtcNow},
                    {12, "100000013", "Supplements", true , DateTime.UtcNow},
                    {13, "100000014", "Syringe", true , DateTime.UtcNow},
                    {14, "100000005", "Testosterone", true , DateTime.UtcNow},
                    {15, "100000007", "Weight Loss", true , DateTime.UtcNow},
                    });

                return migrationBuilder;
            }
            public static MigrationBuilder InsertProductStatusData(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.InsertData(
                    table: "ProductStatuses",
                    columns: new[] { "Id", "Code", "StatusName", "IsActive", "CreatedAt" },
                    values: new object[,]
                    {
                    { 1, 0, "Active", true, DateTime.UtcNow},
                    { 2, 1, "Retired" , true, DateTime.UtcNow},
                    { 3, 2, "Draft", true, DateTime.UtcNow}
                    });

                return migrationBuilder;
            }
            public static MigrationBuilder InsertVisitTypes(MigrationBuilder migrationBuilder)
            {
                var createdAt = DateTime.UtcNow;

                migrationBuilder.InsertData(
                    table: "VisitTypes",
                    columns: new[] { "Id", "Code", "VisitTypeName", "CreatedAt", "CreatedBy", "IsActive" },
                    values: new object[,]
                    {
                        { 1, "168680002", "NA", createdAt, "System", true },
                        { 2, "168680000", "IP", createdAt, "System", true },
                        { 3, "168680001", "H&P", createdAt, "System", true }
                    });

                return migrationBuilder;
            }

            public static MigrationBuilder InsertAgendasData(MigrationBuilder migrationBuilder)
            {
                var createdAt = DateTime.UtcNow;

                migrationBuilder.InsertData(
                    table: "Agendas",
                    columns: new[] { "Id", "Code", "AgendaName", "CreatedAt", "CreatedBy", "IsActive" },
                    values: new object[,]
                    {
            { 1, "168680000", "Nutrition", createdAt, "System", true },
            { 2, "168680001", "Diabetes", createdAt, "System", true },
            { 3, "168680002", "Sexual Function", createdAt, "System", true },
            { 4, "168680003", "Depression", createdAt, "System", true },
            { 5, "168680004", "Quality Of Life", createdAt, "System", true },
            { 6, "168680005", "Heart Health", createdAt, "System", true },
            { 7, "168680006", "Male Menopuase", createdAt, "System", true },
            { 8, "168680007", "Slowing Age", createdAt, "System", true },
            { 9, "168680008", "Hormone Replacement Therapy", createdAt, "System", true },
            { 10, "168680009", "Weight Loss", createdAt, "System", true }
                    });

                return migrationBuilder;
            }

            public static MigrationBuilder InsertCurrencyData(MigrationBuilder migrationBuilder)
            {
                var createdAt = DateTime.UtcNow;
                migrationBuilder.InsertData(
                    table: "Currencies",
                    columns: new[] { "Id", "CurrencyCode", "CurrencyName", "ExchangeRate", "CurrencyPrecision", "CurrencySymbol", "CreatedAt", "IsActive" },
                    values: new object[,]
                    {
                        {
                            1,
                            "USD",
                            "US Dollar",
                            1.00m,
                            2,
                            "$",
                            createdAt,
                            true
                        },
                        {
                            2,
                            "CAD",
                            "Canadian Dollar",
                            1.25m,
                            2,
                            "$",
                            createdAt,
                            true
                        }
                    });
                return migrationBuilder;
            }
            public static MigrationBuilder InsertMedicationTypes(MigrationBuilder migrationBuilder)
            {
                DateTime createdAt = DateTime.UtcNow;
                migrationBuilder.InsertData(
                    table: "MedicationTypes",
                    columns: new[] { "Id", "Code", "MedicationTypeName", "CreatedAt", "CreatedBy", "IsActive" },
                    values: new object[,]
                    {
                        { 1, "168680000", "Initial Consultation", createdAt, "System", true },
                        { 2, "168680001", "F/U Lab Consultation", createdAt, "System", true },
                        { 3, "168680002", "Weight Loss Consultation", createdAt, "System", true },
                        { 4, "168680003", "Pellet Procedure", createdAt, "System", true },
                        { 5, "168680004", "Post Pellet Lab Consultation", createdAt, "System", true },
                        { 6, "168680005", "Patient Email/Call", createdAt, "System", true },
                        { 7, "168680006", "General Note", createdAt, "System", true },
                        { 8, "168680007", "Other", createdAt, "System", true }
                    });
                return migrationBuilder;
            }


            public static MigrationBuilder InsertLifeFileDrugFormData(MigrationBuilder migrationBuilder)
            {
                var createdAt = DateTime.UtcNow;

                migrationBuilder.InsertData(
                    table: "LifeFileDrugForms",
                    columns: new[] { "Id", "Code", "Name", "CreatedAt", "IsActive" },
                    values: new object[,]
                    {
                        { 1, "168680000", "INJECTABLE", createdAt, true },
                        { 2, "168680001", "CAPSULE", createdAt, true },
                        { 3, "168680002", "CREAM", createdAt,true },
                        { 4, "168680003", "TROCHE", createdAt,true },
                        { 5, "168680004", "POWDER", createdAt,true },
                        { 6, "168680005", "SUSPENSION", createdAt,true },
                        { 7, "168680006", "SOLUTION", createdAt,true },
                        { 8, "168680007", "LOLLIPOP", createdAt,true },
                        { 9, "168680008", "NEEDLE", createdAt,true },
                        { 10, "168680009", "SYRINGE", createdAt,true },
                        { 11, "168680010", "TABLET", createdAt,true },
                        { 12, "168680011", "OINTMENT", createdAt,true },
                        { 13, "168680012", "ORAL RINSE", createdAt,true },
                        { 14, "168680013", "ORAL SOLN", createdAt,true },
                        { 15, "168680014", "ORAL SUSP", createdAt,true },
                        { 16, "168680015", "OPH SOLN", createdAt,true }
                    });

                return migrationBuilder;
            }

            public static MigrationBuilder InsertLifeFileQuantityUnitsData(MigrationBuilder migrationBuilder)
            {
                var createdAt = DateTime.UtcNow;

                migrationBuilder.InsertData(
                    table: "LifeFileQuantityUnits",
                    columns: new[] { "Id", "Code", "Name", "CreatedAt", "IsActive" },
                    values: new object[,]
                    {
                        { 1, "168680000", "VIAL", createdAt, true },
                        { 2, "168680001", "TROCHE", createdAt, true },
                        { 3, "168680002", "CAP", createdAt,true },
                        { 4, "168680003", "ML", createdAt,true },
                        { 5, "168680004", "GM", createdAt,true },
                        { 6, "168680005", "TAB", createdAt,true },
                        { 7, "168680006", "EA", createdAt,true },
                        { 8, "168680007", "BOTTLE", createdAt,true },
                        { 9, "168680008", "BOX", createdAt,true }
                    });

                return migrationBuilder;
            }
            public static MigrationBuilder InsertLifeFileScheduleCodeData(MigrationBuilder migrationBuilder)
            {
                var createdAt = DateTime.UtcNow;

                migrationBuilder.InsertData(
                    table: "LifeFileScheduleCodes",
                    columns: new[] { "Id", "Code", "Name", "CreatedAt", "IsActive" },
                    values: new object[,]
                    {
                        { 1, "168680000", "3", createdAt, true },
                        { 2, "168680001", "4", createdAt, true },
                        { 3, "168680002", "L", createdAt,true },
                        { 4, "168680003", "O", createdAt,true }
                    });

                return migrationBuilder;
            }

            public static MigrationBuilder InsertFollowUpLabTests(MigrationBuilder migrationBuilder)
            {
                DateTime createdAt = DateTime.UtcNow;

                migrationBuilder.InsertData(
                    table: "FollowUpLabTests",
                    columns: new[] { "Id", "Code", "Duration", "CreatedAt", "CreatedBy", "IsActive" },
                    values: new object[,]
                    {
            { 168680000, "FT0001", "4-6 Weeks", createdAt, "System", true },
            { 168680001, "FT0002", "6-8 Weeks", createdAt, "System", true },
            { 168680002, "FT0003", "8-10 Weeks", createdAt, "System", true },
            { 168680003, "FT0004", "3 Months", createdAt, "System", true },
            { 168680004, "FT0005", "6 Months", createdAt, "System", true },
            { 168680005, "FT0006", "1 Year", createdAt, "System", true },
            { 0,       "FT0000", "N/A", createdAt, "System", true }
                    });

                return migrationBuilder;
            }

            public static MigrationBuilder InsertDocumentCategoryData(MigrationBuilder migrationBuilder)
            {
                var createdAt = DateTime.UtcNow;

                migrationBuilder.InsertData(
                    table: "DocumentCategories",
                    columns: new[] { "Id", "CategoryName", "CreatedAt", "CreatedBy", "IsActive" },
                    values: new object[,]
                    {
                        { 1, "Blood work", createdAt, "System", true },
                        { 2, "Driving License", createdAt, "System", true },
                        { 3, "Medical History", createdAt, "System", true },
                        { 4, "Personal Photo", createdAt, "System", true },
                        { 5, "Other", createdAt, "System", true }
                    });

                return migrationBuilder;
            }
            public static MigrationBuilder InsertShippingMethodData(MigrationBuilder migrationBuilder)
            {
                var createdAt = DateTime.UtcNow;
                migrationBuilder.InsertData(
                    table: "ShippingMethods",
                    columns: new[] { "Id", "Name", "CreatedAt", "CreatedBy", "IsActive" },
                    values: new object[,]
                    {
                        { 1, "2 Day", createdAt, "System", true },
                        { 2, "Ground Shipping", createdAt, "System", true },
                        { 3, "OverNight Shipping", createdAt, "System", true },
                        { 4, "Pick Up", createdAt, "System", true },
                        { 5, "Saturday Delivery", createdAt, "System", true }
                    });
                return migrationBuilder;
            }

            public static MigrationBuilder UpdateServicesData(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.UpdateData(
                    table: "Services",
                    keyColumn: "Id",
                    keyValue: Guid.Parse("a1f5c9d2-e946-4f72-9e7c-1aa5cb4f9c01"), // New Consultation
                    column: "MaxDuration",
                    value: TimeSpan.FromMinutes(60)
                );

                migrationBuilder.UpdateData(
                    table: "Services",
                    keyColumn: "Id",
                    keyValue: Guid.Parse("b2e6d0f3-f2e3-4c8e-8c2a-234dcf9bda02"), // Follow Up
                    column: "MaxDuration",
                    value: TimeSpan.FromMinutes(30)
                );

                migrationBuilder.UpdateData(
                    table: "Services",
                    keyColumn: "Id",
                    keyValue: Guid.Parse("c3a7e1a4-3f31-42df-bd98-a4567aee6a03"), // Blood Draw
                    column: "MaxDuration",
                    value: TimeSpan.FromMinutes(15)
                );

                migrationBuilder.UpdateData(
                    table: "Services",
                    keyColumn: "Id",
                    keyValue: Guid.Parse("d4b8f2b5-8e40-4cdd-91ab-19c8f3ec7c04"), // Injection
                    column: "MaxDuration",
                    value: TimeSpan.FromMinutes(15)
                );

                migrationBuilder.UpdateData(
                    table: "Services",
                    keyColumn: "Id",
                    keyValue: Guid.Parse("e5c9a3c6-1f59-4712-963b-5a123d4a8b05"), // Pellets
                    column: "MaxDuration",
                    value: TimeSpan.FromMinutes(30)
                );

                migrationBuilder.UpdateData(
                    table: "Services",
                    keyColumn: "Id",
                    keyValue: Guid.Parse("f6dab4d7-1e61-4a1e-94b4-b789ea6f6c06"), // Weight Loss
                    column: "MaxDuration",
                    value: TimeSpan.FromMinutes(15)
                );

                return migrationBuilder;
            }

            public static MigrationBuilder InsertAppointmentModes(MigrationBuilder migrationBuilder)
            {
                var createdAt = DateTime.UtcNow;

                migrationBuilder.InsertData(
                    table: "AppointmentModes",
                    columns: new[]
                    {
                        "Id",
                        "ModeName",
                        "IsActive",
                        "CreatedAt",
                        "CreatedBy"
                    },
                    values: new object[,]
                    {

                        {
                            1,
                            "In-Person",
                            true,
                            createdAt,
                            "System"
                        },

                        {
                            2,
                            "Email",
                            true,
                            createdAt,
                            "System"
                        },


                        {
                            3,
                            "Text",
                            true,
                            createdAt,
                            "System"
                        }
                    }
                );

                return migrationBuilder;
            }

            public static MigrationBuilder InsertAppointmentStatuses(MigrationBuilder migrationBuilder)
            {
                var createdAt = DateTime.UtcNow;

                migrationBuilder.InsertData(
                    table: "AppointmentStatuses",
                    columns: new[]
                    {
                        "Id",
                        "StatusName",
                        "IsActive",
                        "CreatedAt",
                        "CreatedBy"
                    },
                    values: new object[,]
                    {

                        {
                            1,
                            "Pending",
                            true,
                            createdAt,
                            "System"
                        },

                        {
                            2,
                            "Confirmed",
                            true,
                            createdAt,
                            "System"
                        },
                        {
                            3,
                            "Postponed",
                            true,
                            createdAt,
                            "System"
                        },
                        {
                            4,
                            "Cancelled",
                            true,
                            createdAt,
                            "System"
                        },
                        {
                            5,
                            "Completed",
                            true,
                            createdAt,
                            "System"
                        }
                    }
                );

                return migrationBuilder;
            }

            public static MigrationBuilder InsertTimeZones(MigrationBuilder migrationBuilder)
            {
                var createdAt = DateTime.UtcNow;

                migrationBuilder.InsertData(
                    table: "TimeZones",
                    columns: new[] { "Id", "StandardName", "Abbreviation", "CreatedAt", "CreatedBy", "IsActive" },
                    values: new object[,]
                    {
                        { 1, "Hawaiian Standard Time", "HST", createdAt, "System", true },
                        { 2, "Alaska Standard Time", "AKST", createdAt, "System", true },
                        { 3, "Pacific Standard Time", "PST", createdAt, "System", true },
                        { 4, "Mountain Standard Time", "MST", createdAt, "System", true },
                        { 5, "US Mountain Standard Time (Arizona)", "MST", createdAt, "System", true },
                        { 6, "Central Standard Time", "CST", createdAt, "System", true },
                        { 7, "Eastern Standard Time", "EST", createdAt, "System", true },
                        { 8, "Atlantic Standard Time", "AST", createdAt, "System", true },
                        { 9, "Samoa Standard Time", "SST", createdAt, "System", true },
                        { 10, "Chamorro Standard Time", "ChST", createdAt, "System", true }
                    });

                return migrationBuilder;
            }
            public static MigrationBuilder InsertCountryData(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.InsertData(
                    table: "Countries",
                    columns: new[] { "Id", "Name", "IsActive", "CreatedAt" },
                    values: new object[,]
                {
                    { 1, "US", true, DateTime.UtcNow },
                    { 2, "CN", true, DateTime.UtcNow },
                    }
                );
                return migrationBuilder;
            }
            public static MigrationBuilder InsertStringeProductTypesData(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.InsertData(
                    table: "ProductTypes",
                    columns: new[] { "Id", "Code", "Name", "IsActive", "CreatedAt" },
                    values: new object[,]
                    {
                    { 5, "100000004", "Syringe", true , DateTime.UtcNow},
                    });
                return migrationBuilder;
            }

            public static MigrationBuilder UpdateProposalStatusInProgressToInReview(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.Sql(@"
                    UPDATE [dbo].[Proposals]
                    SET [Status] = 'InReview'
                    WHERE [Status] = 'InProgress';
                ");

                return migrationBuilder;
            }
            public static MigrationBuilder InsertIntegrationTypes(MigrationBuilder migrationBuilder)
            {
                var createdAt = DateTime.UtcNow;

                migrationBuilder.InsertData(
                    table: "IntegrationTypes",
                    columns: new[] { "Id", "Type", "CreatedAt", "CreatedBy", "IsActive" },
                    values: new object[,]
                    {
                        { 1, "Life File", createdAt, "System", true },
                        { 2, "Wells", createdAt, "System", true },
                        { 3, "Empower", createdAt, "System", true }
                    });

                return migrationBuilder;
            }

            public static MigrationBuilder InsertIntegrationKeys(MigrationBuilder migrationBuilder)
            {
                var createdAt = DateTime.UtcNow;

                migrationBuilder.InsertData(
                    table: "IntegrationKeys",
                    columns: new[] { "Id", "IntegrationTypeId", "KeyName", "Label", "CreatedAt", "CreatedBy", "IsActive" },
                    values: new object[,]
                    {
                        { 1, 1, "Username", "Username",createdAt, "System", true },
                        { 2, 1, "Password", "Password",createdAt, "System", true },
                        { 3, 1, "BaseUrl", "Base Url",createdAt, "System", true },
                        { 4, 2, "BaseUrl", "Base Url",createdAt, "System", true },
                        { 5, 2, "Key", "Key",createdAt, "System", true },
                        { 6, 3, "ApiKey", "Api Key",createdAt, "System", true },
                        { 7, 3, "ApiSecret", "Api Secret",createdAt, "System", true },
                        { 8, 3, "BaseUrl", "Base Url",createdAt, "System", true },
                        { 9, 1, "PracticeID", "Practice ID",createdAt, "System", true },
                        { 10, 1, "PatientEmail", "Patient Email",createdAt, "System", true },
                        { 11, 1, "ShippingEmail", "Shipping Email",createdAt, "System", true },
                        { 12, 3, "PracticeID", "Practice ID",createdAt, "System", true },
                        { 13, 3, "PatientEmail", "Patient Email",createdAt, "System", true },
                        { 14, 2, "PracticeID", "Practice ID",createdAt, "System", true },
                        { 15, 2, "PatientEmail", "Patient Email",createdAt, "System", true },
                    });

                return migrationBuilder;
            }
            public static MigrationBuilder DeleteCountryData(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.DeleteData(
                    table: "Countries",
                    keyColumn: "Id",
                    keyValue: 2
                );

                return migrationBuilder;
            }

            public static MigrationBuilder InsertStateData(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.InsertData(
                    table: "States",
                    columns: new[] { "Id", "CountryId", "Name", "Abbreviation", "IsActive", "CreatedAt" },
                    values: new object[,]
                    {
            { 1, 1, "Alabama", "AL", true, DateTime.UtcNow },
            { 2, 1, "Alaska", "AK", true, DateTime.UtcNow },
            { 3, 1, "Arizona", "AZ", true, DateTime.UtcNow },
            { 4, 1, "Arkansas", "AR", true, DateTime.UtcNow },
            { 5, 1, "California", "CA", true, DateTime.UtcNow },
            { 6, 1, "Colorado", "CO", true, DateTime.UtcNow },
            { 7, 1, "Connecticut", "CT", true, DateTime.UtcNow },
            { 8, 1, "Delaware", "DE", true, DateTime.UtcNow },
            { 9, 1, "Florida", "FL", true, DateTime.UtcNow },
            { 10, 1, "Georgia", "GA", true, DateTime.UtcNow },
            { 11, 1, "Hawaii", "HI", true, DateTime.UtcNow },
            { 12, 1, "Idaho", "ID", true, DateTime.UtcNow },
            { 13, 1, "Illinois", "IL", true, DateTime.UtcNow },
            { 14, 1, "Indiana", "IN", true, DateTime.UtcNow },
            { 15, 1, "Iowa", "IA", true, DateTime.UtcNow },
            { 16, 1, "Kansas", "KS", true, DateTime.UtcNow },
            { 17, 1, "Kentucky", "KY", true, DateTime.UtcNow },
            { 18, 1, "Louisiana", "LA", true, DateTime.UtcNow },
            { 19, 1, "Maine", "ME", true, DateTime.UtcNow },
            { 20, 1, "Maryland", "MD", true, DateTime.UtcNow },
            { 21, 1, "Massachusetts", "MA", true, DateTime.UtcNow },
            { 22, 1, "Michigan", "MI", true, DateTime.UtcNow },
            { 23, 1, "Minnesota", "MN", true, DateTime.UtcNow },
            { 24, 1, "Mississippi", "MS", true, DateTime.UtcNow },
            { 25, 1, "Missouri", "MO", true, DateTime.UtcNow },
            { 26, 1, "Montana", "MT", true, DateTime.UtcNow },
            { 27, 1, "Nebraska", "NE", true, DateTime.UtcNow },
            { 28, 1, "Nevada", "NV", true, DateTime.UtcNow },
            { 29, 1, "New Hampshire", "NH", true, DateTime.UtcNow },
            { 30, 1, "New Jersey", "NJ", true, DateTime.UtcNow },
            { 31, 1, "New Mexico", "NM", true, DateTime.UtcNow },
            { 32, 1, "New York", "NY", true, DateTime.UtcNow },
            { 33, 1, "North Carolina", "NC", true, DateTime.UtcNow },
            { 34, 1, "North Dakota", "ND", true, DateTime.UtcNow },
            { 35, 1, "Ohio", "OH", true, DateTime.UtcNow },
            { 36, 1, "Oklahoma", "OK", true, DateTime.UtcNow },
            { 37, 1, "Oregon", "OR", true, DateTime.UtcNow },
            { 38, 1, "Pennsylvania", "PA", true, DateTime.UtcNow },
            { 39, 1, "Rhode Island", "RI", true, DateTime.UtcNow },
            { 40, 1, "South Carolina", "SC", true, DateTime.UtcNow },
            { 41, 1, "South Dakota", "SD", true, DateTime.UtcNow },
            { 42, 1, "Tennessee", "TN", true, DateTime.UtcNow },
            { 43, 1, "Texas", "TX", true, DateTime.UtcNow },
            { 44, 1, "Utah", "UT", true, DateTime.UtcNow },
            { 45, 1, "Vermont", "VT", true, DateTime.UtcNow },
            { 46, 1, "Virginia", "VA", true, DateTime.UtcNow },
            { 47, 1, "Washington", "WA", true, DateTime.UtcNow },
            { 48, 1, "West Virginia", "WV", true, DateTime.UtcNow },
            { 49, 1, "Wisconsin", "WI", true, DateTime.UtcNow },
            { 50, 1, "Wyoming", "WY", true, DateTime.UtcNow },

            { 51, 1, "District of Columbia", "DC", true, DateTime.UtcNow },
            { 52, 1, "Puerto Rico", "PR", true, DateTime.UtcNow },
            { 53, 1, "Guam", "GU", true, DateTime.UtcNow },
            { 54, 1, "American Samoa", "AS", true, DateTime.UtcNow },
            { 55, 1, "U.S. Virgin Islands", "VI", true, DateTime.UtcNow },
            { 56, 1, "Northern Mariana Islands", "MP", true, DateTime.UtcNow },
            { 57, 1, "Palmyra Atoll", "UM", true, DateTime.UtcNow }
                    }
                );
                return migrationBuilder;
            }


            public static MigrationBuilder InsertReminderTypes(MigrationBuilder migrationBuilder)
            {
                var createdAt = DateTime.UtcNow;

                migrationBuilder.InsertData(
                    table: "ReminderTypes",
                    columns: new[] { "Id", "TypeName", "CreatedAt", "CreatedBy", "IsActive" },
                    values: new object[,]
                    {
                        { 1, "Check", createdAt, "System", true },
                        { 2, "Call back", createdAt, "System", true },
                        { 3, "Follow Up", createdAt, "System", true }
                    });

                return migrationBuilder;
            }

            public static MigrationBuilder InsertReminderRecurrenceRules(MigrationBuilder migrationBuilder)
            {
                var createdAt = DateTime.UtcNow;

                migrationBuilder.InsertData(
                    table: "RecurrenceRules",
                    columns: new[] { "Id", "RuleName", "IntervalDays", "IntervalMonths", "CreatedAt", "CreatedBy", "IsActive" },
                    values: new object[,]
                    {
                        { 1, "None", null ,null,createdAt, "System", true },
                        { 2, "By Week",7, null, createdAt, "System", true },
                        { 3, "By Twice Weekly",14 ,null,createdAt, "System", true },
                        { 4, "Monthly", null, 1, createdAt, "System", true }
                    });

                return migrationBuilder;
            }

            public static MigrationBuilder InsertPatientRole(MigrationBuilder migrationBuilder)
            {
                // Insert Roles
                migrationBuilder.InsertData(
                    table: "AspNetRoles",
                    columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp", "RoleEnum" },
                    values: new object[,]
                    {
                    { (int)AppRoleEnum.Patient, AppRoleEnum.Patient.ToString(), AppRoleEnum.Patient.ToString().ToUpper() , Guid.NewGuid().ToString(), AppRoleEnum.Patient.ToString()},
                    });

                return migrationBuilder;

            }

            public static MigrationBuilder InsertAPSIntegrationType(MigrationBuilder migrationBuilder)
            {
                var createdAt = DateTime.UtcNow;

                migrationBuilder.InsertData(
                    table: "IntegrationTypes",
                    columns: new[] { "Id", "Type", "CreatedAt", "CreatedBy", "IsActive" },
                    values: new object[,]
                    {
                        { 4, "APS", createdAt, "System", true },
                    });

                return migrationBuilder;
            }

            public static MigrationBuilder InsertAPSIntegrationKeys(MigrationBuilder migrationBuilder)
            {
                var createdAt = DateTime.UtcNow;

                migrationBuilder.InsertData(
                    table: "IntegrationKeys",
                    columns: new[] {"Id","IntegrationTypeId", "KeyName", "Label", "CreatedAt", "CreatedBy", "IsActive" },
                    values: new object[,]
                    {
                        {16,4, "Username", "Username",createdAt, "System", true },
                        {17,4, "Password", "Password",createdAt, "System", true },
                        {18,4, "BaseUrl", "Base Url",createdAt, "System", true },
                        {19,4, "PracticeID", "Practice ID",createdAt, "System", true },
                        {20,4, "StatusID", "Status ID",createdAt, "System", true },
                        {21,4, "PatientMobileNumber", "Patient Mobile Number",createdAt, "System", true },
                        {22,4, "PatientEmail", "Patient Email",createdAt, "System", true },
                        {23,4, "ShippingEmail", "Shipping Email",createdAt, "System", true },

                    });

                return migrationBuilder;
            }

            public static MigrationBuilder InsertPatientMobileNumberAsIntegrationKeys(MigrationBuilder migrationBuilder)
            {
                var createdAt = DateTime.UtcNow;

                migrationBuilder.InsertData(
                    table: "IntegrationKeys",
                    columns: new[] { "Id", "IntegrationTypeId", "KeyName", "Label", "CreatedAt", "CreatedBy", "IsActive" },
                    values: new object[,]
                    {
                        {24,1, "PatientMobileNumber", "Patient Mobile Number",createdAt, "System", true },
                        {25,2, "PatientMobileNumber", "Patient Mobile Number",createdAt, "System", true },
                        {26,3, "PatientMobileNumber", "Patient Mobile Number",createdAt, "System", true },
                    });

                return migrationBuilder;
            }
        }

    }
}

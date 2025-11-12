using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.External.Interfaces;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using NewLifeHRT.Jobs.Scheduler.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewLifeHRT.Jobs.Scheduler.Services
{
    public class SmsSenderService : ISmsSenderService
    {
        private readonly ClinicDbContext _clinicDbContext;
        private readonly ILogger<SmsSenderService> _logger;
        private readonly IMultiTenantContextAccessor<MultiTenantInfo> _multiTenantContextAccessor;
        private readonly ISmsService _smsService;

        public SmsSenderService(
            ClinicDbContext clinicDbContext,
            IMultiTenantContextAccessor<MultiTenantInfo> multiTenantContextAccessor,
            ILogger<SmsSenderService> logger,
            ISmsService smsService)
        {
            _clinicDbContext = clinicDbContext;
            _multiTenantContextAccessor = multiTenantContextAccessor;
            _logger = logger;
            _smsService = smsService;
        }

        /// <summary>
        /// Retrieves a list of batch messages that are currently marked as "InProgress" from the database.
        /// </summary>
        public async Task<List<BatchMessage>> GetBatchMessageAsync()
        {
            return await _clinicDbContext.BatchMessages
                .Where(bm => bm.Status == StatusEnum.InProgress)
                .Include(bm => bm.BatchMessageRecipients).ThenInclude(r => r.Patient)
                .Include(bm => bm.BatchMessageRecipients).ThenInclude(r => r.Lead)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Processes a list of batch messages and sends SMS messages to their respective recipients.
        /// </summary>
        public async Task ProcessBulkSms(List<BatchMessage> batchMessages)
        {
            foreach (var batchMessage in batchMessages)
            {
                bool allSuccess = true;

                foreach (var recipient in batchMessage.BatchMessageRecipients)
                {
                    try
                    {
                        Guid? patientId = recipient.PatientId;
                        Guid? leadId = recipient.LeadId;
                        int currentUserId = batchMessage.CreatedByUserId;

                        var conversation = patientId.HasValue
                            ? await _clinicDbContext.Conversations.FirstOrDefaultAsync(c => c.PatientId == patientId.Value)
                            : await _clinicDbContext.Conversations.FirstOrDefaultAsync(c => c.LeadId == leadId.Value);

                        if (conversation == null)
                        {
                            conversation = new Conversation
                            {
                                PatientId = patientId,
                                LeadId = leadId,
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = currentUserId.ToString(),
                            };
                            await _clinicDbContext.Conversations.AddAsync(conversation);
                            await _clinicDbContext.SaveChangesAsync();
                        }

                        string phoneNumber = patientId.HasValue
                            ? recipient.Patient?.PhoneNumber ?? string.Empty
                            : recipient.Lead?.PhoneNumber ?? string.Empty;

                        var sid = await _smsService.SendSmsAsync(phoneNumber, batchMessage.Message);

                        var message = new Message
                        {
                            ConversationId = conversation.Id,
                            UserId = currentUserId,
                            TwilioId = sid,
                            IsRead = false,
                            Timestamp = DateTime.UtcNow,
                            Direction = MessageDirectionEnum.Outbound.ToString(),
                            IsSent = true,
                            IsActive = true,
                            CreatedBy = currentUserId.ToString(),
                            CreatedAt = DateTime.UtcNow,
                        };
                        await _clinicDbContext.Messages.AddAsync(message);
                        await _clinicDbContext.SaveChangesAsync();

                        var messageContent = new MessageContent
                        {
                            MessageId = message.Id,
                            ContentType = "text",
                            Content = batchMessage.Message,
                            IsActive = true,
                            CreatedBy = currentUserId.ToString(),
                            CreatedAt = DateTime.UtcNow,
                        };
                        await _clinicDbContext.MessagesContent.AddAsync(messageContent);

                        recipient.Status = StatusEnum.Approved;
                        recipient.ErrorReason = null;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send SMS to recipient {RecipientId}", recipient.Id);
                        recipient.Status = StatusEnum.Canceled; 
                        recipient.ErrorReason = ex.Message;
                        allSuccess = false;
                    }
                    finally
                    {
                        recipient.UpdatedAt = DateTime.UtcNow;
                        recipient.UpdatedBy = batchMessage.CreatedByUserId.ToString();
                        _clinicDbContext.BatchMessageRecipients.Update(recipient);
                    }
                }

                batchMessage.Status = allSuccess ? StatusEnum.Approved : StatusEnum.InProgress ; 
                batchMessage.UpdatedAt = DateTime.UtcNow;
                batchMessage.UpdatedBy = batchMessage.CreatedByUserId.ToString();
                _clinicDbContext.BatchMessages.Update(batchMessage);
            }
            await _clinicDbContext.SaveChangesAsync();

            _logger.LogInformation("Processed {Count} batch messages for tenant {Tenant}",
                batchMessages.Count,
                _multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Name ?? "Unknown");
        }

    }
}

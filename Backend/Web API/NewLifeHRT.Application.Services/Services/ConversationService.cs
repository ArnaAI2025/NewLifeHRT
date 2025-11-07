using FFMpegCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NewLifeHRT.API.Hubs;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.External.Interfaces;
using NewLifeHRT.External.Services;
using System.Linq.Expressions;
using Twilio.TwiML;
using Twilio.Types;

namespace NewLifeHRT.Application.Services.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IMessageContentService _messageContentService;
        private readonly IMessageService _messageService;
        private readonly IConversationRepository _conversationRepository;
        private readonly ISmsService _smsService;
        private readonly IPatientService _patientService;
        private readonly ILeadService _leadService;
        private readonly IHubContext<SmsHub> _hubContext;
        private readonly IAudioConverter _audioConverter;
        private readonly IBlobService _blobService;


        public ConversationService(
            IConversationRepository conversationRepository,
            IMessageContentService messageContentService,
            IMessageService messageService,
            ISmsService smsService,
            IPatientService patientService,
            ILeadService leadService,
            IHubContext<SmsHub> hubContext,
            IAudioConverter audioConverter,
            IBlobService blobService)
        {
            _conversationRepository = conversationRepository;
            _messageContentService = messageContentService;
            _messageService = messageService;
            _smsService = smsService;
            _patientService = patientService;
            _leadService = leadService;
            _hubContext = hubContext;
            _audioConverter = audioConverter;
            _blobService = blobService;
        }

        public async Task<CommonOperationResponseDto<Guid>> CreateConversation(ConversationRequestDto dto, int userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.To) || string.IsNullOrWhiteSpace(dto.MessageContent.Content))
                {
                    return new CommonOperationResponseDto<Guid>
                    {
                        Id = Guid.Empty,
                        Message = "Phone number and message are required."
                    };
                }

                var sid = await _smsService.SendSmsAsync(dto.To, dto.MessageContent.Content);
                if (string.IsNullOrEmpty(sid))
                {
                    return new CommonOperationResponseDto<Guid>
                    {
                        Id = Guid.Empty,
                        Message = "Failed to send SMS."
                    };
                }

                var conversation = await GetOrCreateConversationAsync(dto.PatientId, dto.LeadId, userId, dto.From, userId.ToString());

                dto.Message.ConversationId = conversation.Id;
                dto.Message.TwilioId = sid;
                dto.Message.Direction = MessageDirectionEnum.Outbound.ToString();
                dto.Message.IsSent = true;
                dto.Message.IsRead = true;
                dto.Message.Timestamp = DateTime.UtcNow;

                var messageResponse = await _messageService.CreateMessageAsync(dto.Message, userId, userId.ToString());

                dto.MessageContent.MessageId = messageResponse.Id;
                var contentResponse = await _messageContentService.CreateMessageContentAsync(dto.MessageContent, userId.ToString());

                return new CommonOperationResponseDto<Guid>
                {
                    Id = contentResponse.Id,
                    Message = $"Conversation, Message, and Content created successfully. ConversationId: {conversation.Id}"
                };
            }
            catch (Exception ex)
            {
                return new CommonOperationResponseDto<Guid>
                {
                    Id = Guid.Empty,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<string> ProcessIncomingSmsAsync(SmsRequestDto request)
        {
            try
            {
                Guid? patientId = null;
                Guid? leadId = null;
                int? assignedUserId = null;
                string createdBy = null;
                var patient = await _patientService.GetPatientByMobileNumber(request.From);
                if (patient != null)
                {
                    patientId = patient.Id;
                    assignedUserId = patient.CounselorId;
                    createdBy = patient.Id.ToString();
                }
                else
                {
                    var lead = await _leadService.GetLeadByMobileNumber(request.From);
                    if (lead != null)
                    {
                        leadId = lead.Id;
                        assignedUserId = lead.OwnerId;
                        createdBy = lead.Id.ToString();
                    }
                }
                var conversation = await GetOrCreateConversationAsync(patientId, leadId, assignedUserId ?? 0,request.From,createdBy);
                var messageRequest = new MessageRequestDto
                {
                    ConversationId = conversation.Id,
                    UserId = assignedUserId,
                    TwilioId = request.SmsSid,
                    IsRead = false,
                    Timestamp = DateTime.UtcNow,
                    Direction = MessageDirectionEnum.Inbound.ToString(),
                    IsSent = null
                };
                var messageResponse = await _messageService.CreateMessageAsync(messageRequest, 0, createdBy);

                // Save text content
                if (!string.IsNullOrWhiteSpace(request.Body))
                {
                    await _messageContentService.CreateMessageContentAsync(new MessageContentRequestDto
                    {
                        MessageId = messageResponse.Id,
                        ContentType = "text",
                        Content = request.Body
                    }, createdBy);
                }

                var messageContentsDto = new List<object>();

                if (request.NumMedia.HasValue && request.NumMedia.Value > 0)
                {
                    Guid idForPath = patientId != null ? (Guid)patientId : (leadId ?? Guid.Empty);

                    foreach (var mediaUrl in request.GetAllMediaUrls())
                    {
                        var (mediaBytes, contentType) = await _smsService.GetTwilioMediaAsync(mediaUrl);

                        if (contentType == "audio/amr")
                        {
                            mediaBytes = await _audioConverter.ConvertAmrToMp3Async(mediaBytes);
                            contentType = "audio/mpeg";
                        }

                        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmssfff");
                        string extension = GetFileExtensionFromContentType(contentType);
                        string uniqueFileName = $"{timestamp}_{extension}";

                        string blobPath = $"{idForPath}/5/{uniqueFileName}";
                        string blobUrl = await _blobService.UploadMediaAsync(mediaBytes, blobPath, contentType);

                        await _messageContentService.CreateMessageContentAsync(new MessageContentRequestDto
                        {
                            MessageId = messageResponse.Id,
                            ContentType = contentType,
                            Content = blobUrl
                        }, createdBy);

                        messageContentsDto.Add(new
                        {
                            ContentType = contentType,
                            Content = blobUrl
                        });
                    }
                }


                if (!string.IsNullOrWhiteSpace(request.Body))
                {
                    messageContentsDto.Add(new
                    {
                        ContentType = "text",
                        Content = request.Body
                    });
                }

                var chatMessageDto = new
                {
                    MessageId = messageResponse.Id,
                    ConversationId = conversation.Id,
                    Direction = MessageDirectionEnum.Inbound.ToString(),
                    Timestamp = DateTime.UtcNow,
                    IsRead = false,
                    IsSent = true,
                    MessageContents = messageContentsDto
                };

                var groupName = patientId?.ToString() ?? leadId?.ToString();
                if (!string.IsNullOrEmpty(groupName))
                {
                    await _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", chatMessageDto);
                }

                return new MessagingResponse().ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RECEIVE ERROR] {ex.Message}");
                return new MessagingResponse().ToString();
            }
        }

        public async Task<Conversation> GetOrCreateConversationAsync(Guid? patientId, Guid? leadId, int? userId, string? PhoneNumber, string createdBy)
        {
            Conversation? conversation = null;

            if (patientId.HasValue)
            {
                conversation = await _conversationRepository.GetSingleAsync(p => p.PatientId == patientId.Value);
            }
            else if (leadId.HasValue)
            {
                conversation = await _conversationRepository.GetSingleAsync(p => p.LeadId == leadId.Value);
            }
            else
            {
                conversation = await _conversationRepository.GetSingleAsync(p => p.FromPhoneNumber == PhoneNumber);
            }
            if (conversation == null)
            {
                if(createdBy == null) createdBy = PhoneNumber;
                conversation = new Conversation
                {
                    PatientId = patientId,
                    LeadId = leadId,
                    FromPhoneNumber = (!patientId.HasValue && !leadId.HasValue) ? PhoneNumber : null,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy
                };

                await _conversationRepository.AddAsync(conversation);
                await _conversationRepository.SaveChangesAsync();
            }

            return conversation;
        }
        public async Task<ConversationResponseDto?> GetConversationOrCreateByPatientOrLeadAsync(Guid? patientId, Guid? leadId, int userId)
        {
            Conversation? conversation = null;
            string[] includes = new[]
            {
        "Messages", "Messages.MessageContents", "Messages.User",
        "Patient", "Patient.Counselor",
        "Lead", "Lead.Owner"
    };

            if (patientId.HasValue)
            {
                conversation = await GetConversationByPatientIdAsync(patientId.Value, includes);

                if (conversation == null)
                {
                    await GetOrCreateConversationAsync(patientId.Value, null, userId, conversation?.Patient?.PhoneNumber, userId.ToString());
                    conversation = await GetConversationByPatientIdAsync(patientId.Value, includes);
                }
            }
            else if (leadId.HasValue)
            {
                conversation = await GetConversationByLeadIdAsync(leadId.Value, includes);

                if (conversation == null)
                {
                    await GetOrCreateConversationAsync(null, leadId.Value, userId, conversation?.Lead?.PhoneNumber, userId.ToString());
                    conversation = await GetConversationByLeadIdAsync(leadId.Value, includes);
                }
            }

            if (conversation == null)
                return null;

            int? currentCounselorId = null;
            string? currentCounselorName = null;

            if (conversation.Patient?.Counselor != null)
            {
                currentCounselorId = conversation.Patient.Counselor.Id;
                currentCounselorName = $"{conversation.Patient.Counselor.FirstName} {conversation.Patient.Counselor.LastName}".Trim();
            }
            else if (conversation.Lead?.Owner != null)
            {
                currentCounselorId = conversation.Lead.Owner.Id;
                currentCounselorName = $"{conversation.Lead.Owner.FirstName} {conversation.Lead.Owner.LastName}".Trim();
            }
            else
            {
                var latestCounselorMsg = conversation.Messages
                    .Where(m => m.User != null)
                    .OrderByDescending(m => m.Timestamp)
                    .FirstOrDefault();

                if (latestCounselorMsg?.User != null)
                {
                    currentCounselorId = latestCounselorMsg.User.Id;
                    currentCounselorName = $"{latestCounselorMsg.User.FirstName} {latestCounselorMsg.User.LastName}".Trim();
                }
            }          
            return conversation.ToConversationResponseDto(currentCounselorId, currentCounselorName); ;
        }

        public async Task<Conversation?> GetConversationByPatientIdAsync(Guid patientId, string[] includes)
        {
            IQueryable<Conversation> query = _conversationRepository.Query();

            if (includes != null && includes.Any())
            {
                query = query.Include(includes[0]);
                foreach (var include in includes.Skip(1))
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync(c => c.PatientId == patientId);
        }

        public async Task<Conversation?> GetConversationByLeadIdAsync(Guid leadId, string[] includes)
        {
            IQueryable<Conversation> query = _conversationRepository.Query();

            if (includes != null && includes.Any())
            {
                query = query.Include(includes[0]);
                foreach (var include in includes.Skip(1))
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync(c => c.LeadId == leadId);
        }

        private string GetFileExtensionFromContentType(string contentType)
        {
            return contentType switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "audio/mpeg" => ".mp3",
                "audio/amr" => ".amr",
                "video/mp4" => ".mp4",
                "application/pdf" => ".pdf",
                _ => ""
            };
        }
       
    }
}

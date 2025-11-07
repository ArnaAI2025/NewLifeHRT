using System;
using System.Collections.Generic;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class SmsRequestDto
    {
        public string? SmsSid { get; set; }
        public string? SmsMessageSid { get; set; }
        public string? MessageSid { get; set; }
        public string? MessagingServiceSid { get; set; }
        public string? AccountSid { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public string? Body { get; set; }
        public int? NumMedia { get; set; }
        public string? MediaUrl0 { get; set; }
        public string? MediaUrl1 { get; set; }
        public string? MediaUrl2 { get; set; }

        public string? GetMediaUrl(int index)
        {
            return index switch
            {
                0 => MediaUrl0,
                1 => MediaUrl1,
                2 => MediaUrl2,
                _ => null
            };
        }

        public List<string> GetAllMediaUrls()
        {
            var urls = new List<string>();
            if (!string.IsNullOrWhiteSpace(MediaUrl0)) urls.Add(MediaUrl0!);
            if (!string.IsNullOrWhiteSpace(MediaUrl1)) urls.Add(MediaUrl1!);
            if (!string.IsNullOrWhiteSpace(MediaUrl2)) urls.Add(MediaUrl2!);
            return urls;
        }

        public List<MessageContentItem> GetMessageContents()
        {
            var contents = new List<MessageContentItem>();

            if (!string.IsNullOrWhiteSpace(Body))
            {
                contents.Add(new MessageContentItem
                {
                    ContentType = "text",
                    Content = Body
                });
            }

            foreach (var mediaUrl in GetAllMediaUrls())
            {
                contents.Add(new MessageContentItem
                {
                    ContentType = "media",
                    Content = mediaUrl
                });
            }

            return contents;
        }
    }

    public class MessageContentItem
    {
        public string ContentType { get; set; } = string.Empty; 
        public string Content { get; set; } = string.Empty;     
    }
}

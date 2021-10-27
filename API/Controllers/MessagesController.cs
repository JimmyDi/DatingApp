using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var userName = User.GetUserName();
            
            if(userName == createMessageDto.RecipientUserName.ToLower())
            {
                return BadRequest("You cannot send messages to yourself");
            }

            var sender = await _userRepository.GetUserByUserNameAsync(userName);
            var recipient = await _userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUserName);

            if (recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUserName = recipient.UserName,
                Content = createMessageDto.Content
            };
            
            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllSync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageForUser(
            [FromQuery] MessageParams messageParams)
        {
            var userId = User.GetUserId();
            var sender = await _userRepository.GetUserByIdAsync(userId);

            messageParams.UserName = sender.UserName;

            var messages = await _messageRepository.GetMessagesForUser(messageParams);
            
            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, 
                messages.TotalCount, messages.TotalPages);

            return messages;
        }
        
        
        [HttpGet("thread/{UserName}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string UserName)
        {
            var currentUserName = User.GetUserName();

            return Ok(await _messageRepository.GetMessageThread(currentUserName, UserName));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var UserName = User.GetUserName();

            var message = await _messageRepository.GetMessage(id);

            if (message.Sender.UserName != UserName && message.Recipient.UserName != UserName)
            {
                return Unauthorized();
            }

            if (message.Sender.UserName == UserName) message.SenderDeleted = true;

            if (message.Recipient.UserName == UserName) message.RecipientDeleted = true;
            
            if (message.SenderDeleted && message.RecipientDeleted) _messageRepository.DeleteMessage(message);

            if (await _messageRepository.SaveAllSync()) return Ok();

            return BadRequest("Problem deleting the message");
        }
    }
}
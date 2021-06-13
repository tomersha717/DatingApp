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
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;


        public MessagesController(IUserRepository userRepository
        ,IMessageRepository messageRepository
        ,IMapper mapper)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }


        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUserName();

            if (username == createMessageDto.RecipientUsername.ToLower())
                return BadRequest("You cannot send messages to yourself");

            var sender = await _userRepository.GetUserByUserNameAsync(username);
            var resipient = await _userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);

            if (resipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = resipient,
                SenderUsername = sender.UserName,
                RecipientUsername = resipient.UserName,
                Content = createMessageDto.Content
            };

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Faied to send message");
            
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageForUser([FromQuery]MessageParams messageParams)
        {
            messageParams.Username = User.GetUserName();

            var messages = await _messageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage,messages.PageSize, messages.TotalCount, messages.TotalPages);

            return messages;

        }



        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUserName();

            return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
        }

    }
}
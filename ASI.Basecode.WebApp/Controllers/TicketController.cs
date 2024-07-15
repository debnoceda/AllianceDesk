﻿using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Data.Models;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Controllers
{
    public class TicketController : ControllerBase<TicketController>
    {

        private readonly ITicketService _ticketService;
        private readonly IUserService _userService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public TicketController(ITicketService ticketService, IUserService userService,
            IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _ticketService = ticketService;
            _userService = userService;
        }

        #region Admin Methods
        [HttpGet("/Admin/Tickets")]
        public IActionResult AdminTickets()
        {
            var data = _ticketService.RetrieveAll();
            return View(data);
        }

        [HttpGet("/Admin/Tickets/{id}")]
        public IActionResult AdminDetails(string id)
        {
            var ticket = _ticketService.GetById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            ticket.TicketActivities = _ticketService.GetHistory(id);

            return View(ticket);
        }

        [HttpGet("/Admin/Tickets/{id}/Assign")]
        public IActionResult Assign(string id)
        {
            var ticket = _ticketService.GetById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            var agents = _userService.GetAgents()
                                   .Select(a => new SelectListItem
                                   {
                                       Value = a.UserId.ToString(),
                                       Text = a.Name
                                   })
                                   .ToList();

            ViewBag.Agents = new SelectList(agents, "Value", "Text");

            return View(ticket);
        }

        [HttpPost("/Admin/Tickets/{id}/Assign")]
        public IActionResult PostAssign(string id, string agentId)
        {
            var ticket = _ticketService.GetById(id);

            if (ticket == null)
            {
                return NotFound();
            }

                       
            TicketActivityViewModel activity = new TicketActivityViewModel();

            activity.TicketId = ticket.TicketId;
            activity.OperationId = 2;
            activity.ModifiedAt = DateTime.Now;

            // Replace with User.Identity.Name when authentication is implemented
            activity.ModifiedBy = "152B4DEB-7964-4AF6-B4E3-E95F39CF7349";

            _ticketService.AddHistory(activity);

            ticket.AgentId = agentId;
            _ticketService.Update(ticket);

            return RedirectToAction(nameof(AdminDetails), new { id = ticket.TicketId });
        }

        #endregion

        #region Agent Methods

        [HttpGet("/Agent/Tickets")]
        public IActionResult AgentTickets()
        {
            // Replace with User.Identity.Name when authentication is implemented
            string agent = "3850590f-e5e1-468b-a9a2-420074e9073f";
            string agent2 = "aba6bb10-e42d-4714-907b-445f494e1dff";

            var tickets = _ticketService.RetrieveAll();

            var agentTickets = tickets.Where(t => t.AgentId == agent);

            // TO DO: Find a way to be able to see the date the ticket was assigned to the agent
            
            /*var updatedAgentTickets = new List<TicketViewModel>();

            foreach (var ticket in agentTickets)
            {
                var history = _ticketService.GetHistory(ticket.TicketId);

                if (history != null)
                {
                    var firstOperation2 = history.FirstOrDefault(s => s.OperationId == 2);

                    if (firstOperation2 != null)
                    {
                        // Create a new TicketViewModel with updated DateAssigned
                        var updatedTicket = new TicketViewModel
                        {
                            TicketId = ticket.TicketId,
                            
                            DateAssigned = firstOperation2.ModifiedAt
                        };
                        updatedAgentTickets.Add(updatedTicket);
                    }
                }
            }*/

            // Return the view with the updated agentTickets
            return View(agentTickets);
        }

        #endregion

        #region User Methods

        [HttpGet("/User/Tickets")]
        public IActionResult UserTickets(string? status)
        {
            // Replace with User.Identity.Name when authentication is implemented
            string user = "857949fe-ec30-4c0b-a514-eb0fd9262738";
            string user2 = "90122701-1c8c-40a4-8936-7717Cfaa9c14";

            var tickets = _ticketService.RetrieveAll();

            
            var userTickets = tickets.Where(t => t.CreatorId == user).ToList();


            if (userTickets.Count == 0)
            {
                Console.WriteLine("No tickets found for user");
                return NotFound();
            }

            var statuses = _ticketService.GetStatuses()
                                   .Select(c => new SelectListItem
                                   {
                                       Value = c.StatusId.ToString(),
                                       Text = c.StatusName
                                   })
                                   .ToList();

            var categories = _ticketService.GetCategories()
                                  .Select(c => new SelectListItem
                                  {
                                      Value = c.CategoryId.ToString(),
                                      Text = c.CategoryName
                                  })
                                  .ToList();

            var priorities = _ticketService.GetPriorities()
                                           .Select(p => new SelectListItem
                                           {
                                               Value = p.PriorityId.ToString(),
                                               Text = p.PriorityName
                                           })
                                           .ToList();

            // Pass data to ViewBag
            ViewBag.Statuses = new SelectList(statuses, "Value", "Text");
            ViewBag.Categories = new SelectList(categories, "Value", "Text");
            ViewBag.Priorities = new SelectList(priorities, "Value", "Text");


            if (!string.IsNullOrEmpty(status))
            {

                var statusTickets = userTickets.Where(t => t.StatusId == status).ToList();
                return View(statusTickets);
            }
            else
            {
                var model = new TicketPageViewModel
                {
                    Tickets = userTickets,
                    Ticket = new TicketViewModel() // Initialize a new ticket for the form
                };
                return View(model);
            }
            
        }


        [HttpGet("/User/Tickets/Create")]
        public IActionResult UserCreate()
        {
            var categories = _ticketService.GetCategories()
                                   .Select(c => new SelectListItem
                                   {
                                       Value = c.CategoryId.ToString(),
                                       Text = c.CategoryName
                                   })
                                   .ToList();

            var priorities = _ticketService.GetPriorities()
                                           .Select(p => new SelectListItem
                                           {
                                               Value = p.PriorityId.ToString(),
                                               Text = p.PriorityName
                                           })
                                           .ToList();

            // Pass data to ViewBag
            ViewBag.Categories = new SelectList(categories, "Value", "Text");
            ViewBag.Priorities = new SelectList(priorities, "Value", "Text");

            return View();
        }

        [HttpPost("/User/Tickets/Create")]
        public IActionResult PostUserCreate(TicketViewModel ticket)
        {
            // Replace with User.Identity.Name when authentication is implemented
            ticket.CreatorId = "857949FE-EC30-4C0B-A514-EB0FD9262738";
            _ticketService.Add(ticket);
            
            return RedirectToAction(nameof(UserTickets));
        }

        [HttpGet("/User/Tickets/{id}")]
        public IActionResult UserDetails(string id)
        {
            var ticket = _ticketService.GetById(id);

            ticket.TicketMessages = _ticketService.GetMessages(id);

            if (ticket == null)
            {
                return NotFound(); // Handle ticket not found scenario
            }

            return View(ticket);
        }

        [HttpGet("/User/Tickets/{id}/Edit")]
        public IActionResult UserEdit(string id)
        {
            var ticket = _ticketService.GetById(id);

            if (ticket == null)
            {
                return NotFound(); // Handle ticket not found scenario
            }

            var categories = _ticketService.GetCategories()
                                   .Select(c => new SelectListItem
                                   {
                                       Value = c.CategoryId.ToString(),
                                       Text = c.CategoryName
                                   })
                                   .ToList();

            var priorities = _ticketService.GetPriorities()
                                           .Select(p => new SelectListItem
                                           {
                                               Value = p.PriorityId.ToString(),
                                               Text = p.PriorityName
                                           })
                                           .ToList();
            // Pass data to ViewBag
            ViewBag.Categories = new SelectList(categories, "Value", "Text", ticket.CategoryId.ToString());
            ViewBag.Priorities = new SelectList(priorities, "Value", "Text", ticket.PriorityId.ToString());

            var ticketViewModel = new TicketViewModel
            {
                TicketId = ticket.TicketId,
                Title = ticket.Title,
                Description = ticket.Description,
                CategoryId = ticket.CategoryId,
                PriorityId = ticket.PriorityId
            };

            return View(ticketViewModel);
        }
         
        [HttpPost("/User/Tickets/{id}/Edit")]
        public IActionResult PostUserEdit(TicketViewModel ticket)
        {

            _ticketService.Update(ticket);
            
            TicketActivityViewModel activity = new TicketActivityViewModel();
            activity.TicketId = ticket.TicketId;
            activity.OperationId = 1;
            activity.ModifiedAt = DateTime.Now;

            // Replace with User.Identity.Name when authentication is implemented
            activity.ModifiedBy = "857949FE-EC30-4C0B-A514-EB0FD9262738";

            _ticketService.AddHistory(activity);

            return RedirectToAction(nameof(UserDetails), new { id = ticket.TicketId });
        }

        [HttpGet("/User/Tickets/{id}/Delete")]

        public IActionResult UserDelete(string id)
        {

            var ticket = _ticketService.GetById(id);
            if (ticket == null)
            {
                return NotFound();
            }

            _ticketService.Delete(id);

            return RedirectToAction(nameof(UserTickets));
        }
        
        [HttpPost("/User/Tickets/{id}/Message")]
        public IActionResult PostMessage(string id, TicketViewModel model)
        {
            var message = new TicketMessageViewModel
            {
                SentById = "857949FE-EC30-4C0B-A514-EB0FD9262738", // Replace with User.Identity.Name when authentication is implemented
                Message = model.NewMessageBody,
                PostedAt = DateTime.Now,
                TicketId = id
            };
           
            _ticketService.AddMessage(message);

            return RedirectToAction(nameof(UserDetails), new { id = message.TicketId });
        }

        #endregion
    }
}
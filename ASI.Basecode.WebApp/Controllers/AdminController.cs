﻿using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("Admin")]
    public class AdminController : ControllerBase<AdminController>
    {
        private readonly IUserService _userService;
        private readonly ITicketService _ticketService;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public AdminController(IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IUserService userService,
                              ITicketService ticketService,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._userService = userService;
            this._ticketService = ticketService;
        }

        /// <summary>
        /// Returns User View
        /// </summary>
        /// <returns> Home View </returns>
        [HttpGet("/ViewUser")]
        [AllowAnonymous]
        public ActionResult ViewUser()
        {
            ViewBag.IsLoginOrRegister = false;
            ViewBag.AdminSidebar = "ViewUser";
            var users = _userService.GetUsers()
                                        .Select(u => new UserViewModel
                                        {
                                            Name = u.Name,
                                            Email = u.Email,
                                            RoleId = u.RoleId,
                                            UserId = u.UserId.ToString(),
                                        })
                                        .ToList();

            var viewModel = new UserViewModel
            {
                Users = users
            };

            return View(viewModel);
        }

        /// <summary>
        /// Returns Tickets View.
        /// </summary>
        /// <returns> Tickets View </returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("Tickets/{id?}")]
        public ActionResult Tickets(string? id, string? status)
        {
            ViewBag.IsLoginOrRegister = false;
            ViewBag.AdminSidebar = "Tickets";

            if(status != null)
            {
                ViewBag.ShowStatus = status;

                if(status == "Resolved")
                {
                    var resolvedTickets = _ticketService.RetrieveAll()
                        .Where(t => t.StatusId == "4" || t.StatusId == "5")
                        .OrderByDescending(t => t.DateCreated);
                    return View("/Views/Admin/Tickets.cshtml", resolvedTickets);
                }

                var unresolvedTickets = _ticketService.RetrieveAll()
                    .Where(t => t.StatusId == "1" || t.StatusId == "2" || t.StatusId == "3")
                    .OrderByDescending(t => t.DateCreated);

                return View("/Views/Admin/Tickets.cshtml", unresolvedTickets);
            } 

            if (id != null)
            {

                var ticket = _ticketService.RetrieveAll()
                    .Where(t => t.TicketId.ToString() == id)
                    .FirstOrDefault();

                TicketViewModel ticketViewModel = new TicketViewModel
                {
                    TicketId = ticket.TicketId,
                    Title = ticket.Title,
                    Description = ticket.Description,
                    StatusId = ticket.StatusId,
                    Status = ticket.Status,
                    CategoryId = ticket.CategoryId,
                    Category = ticket.Category,
                    PriorityId = ticket.PriorityId,
                    Priority = ticket.Priority,
                    CreatorId = ticket.CreatorId,
                    DateCreated = ticket.DateCreated,
                    TicketActivities = ticket.TicketActivities
                };

                return this.View("/Views/Admin/TicketDetail.cshtml", ticketViewModel);
            }
            else
            {
                var tickets = _ticketService.RetrieveAll()
                    .OrderByDescending(t => t.DateCreated);

                // Handle the case where no ID is provided
                return this.View("/Views/Admin/Tickets.cshtml", tickets);
            }
        }

        [HttpGet("/TicketAssignment")]
        [AllowAnonymous]
        public ActionResult TicketAssignment(string id)
        {
            var ticket = _ticketService.RetrieveAll()
                .Where(t => t.TicketId.ToString() == id)
                .FirstOrDefault();

            var tickets = _ticketService.RetrieveAll();

            // Retrieve all agents (assuming RoleId 2 corresponds to agents)
            var agents = _userService.GetAgents().ToList();

            foreach (var agent in agents) {
                if(agent.TeamId != null)
                {
                    agent.TeamName = _userService.GetTeams().Where(t => t.TeamId.ToString() == agent.TeamId).FirstOrDefault().TeamName;
                }
            }
            
            var assignedTicketCounts = agents
                .Select(agent => new
                {
                    Agent = agent,
                    TicketCount = tickets.Count(t => t.AgentId.ToString() == agent.UserId.ToString())
                })
                .OrderByDescending(agent => agent.TicketCount)
                .ToList();

            var model = new AgentAssignmentViewModel
            {
                TicketId = ticket.TicketId,
                Title = ticket.Title,
                CreatedAt = ticket.DateCreated,
                Description = ticket.Description,
                Agents = agents,
                AssignedTicketCounts = assignedTicketCounts
                    .Select(agent => new AgentTicketCountViewModel
                    {
                        Agent = agent.Agent,
                        TicketCount = agent.TicketCount
                    })
                    .ToList()
            };
            
            ViewBag.AdminSidebar = "Tickets";
            return View(model);
        }

        [HttpGet("/AnalyticsTeamMetric")]
        [AllowAnonymous]
        public ActionResult AnalyticsTeamMetric()
        {
            ViewBag.AdminSidebar = "Analytics";
            return this.View();
        }

        [HttpGet("Analytics/OverallMetrics")]
        [AllowAnonymous]
        public ActionResult AnalyticsOverallMetrics()
        {
            ViewBag.AdminSidebar = "Analytics";

            ViewBag.WeeklyStatus = new
            {
                open = 5,
                resolved = 10,
                inProgress = 15,
                closed = 5
            };

            return this.View();
        }

        [HttpGet("/AnalyticsAgentMetric")]
        [AllowAnonymous]
        public ActionResult AgentMetric()
        {
            ViewBag.AdminSidebar = "Analytics";
            return this.View();
        }


        [HttpGet("/TicketReassignment")]
        [AllowAnonymous]
        public ActionResult TicketReassignment()
        {
            ViewBag.AdminSidebar = "Tickets";
            ViewBag.UserID = "Tickets";
            return this.View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult TicketResolved()
        {
            ViewBag.AdminSidebar = "Tickets";
            return this.View();
        }

        [HttpGet("Tickets/TicketOccupied")]
        [AllowAnonymous]
        public ActionResult TicketOccupied()
        {
            ViewBag.AdminSidebar = "Tickets";
            return this.View();
        }

        [HttpGet]
        [Route("UserDetails")]
        /// <summary>
        /// Go to the User Details View
        /// </summary>
        /// <returns> User Details</returns>
        /// 
        public IActionResult UserDetails(string UserId)
        {
            var data = _userService.GetUsers().Where(x => x.UserId.ToString() == UserId).FirstOrDefault();
            var team = _userService.GetTeams().Where(t => t.TeamId.Equals(data.TeamId)).FirstOrDefault();

            var userModel = new UserViewModel
            {
                UserId = UserId,
                Name = data.Name,
                Email = data.Email,
                RoleId = data.RoleId,
                TeamName = team.TeamName,
            };

            return PartialView("UserDetails", userModel);
        }


        [HttpGet("/dashboard")]
        [AllowAnonymous]
        public ActionResult Dashboard()
        {
            ViewBag.AdminSidebar = "Overview";
            return this.View();
        }


        [HttpGet("/AddUser")]
        [AllowAnonymous]
        /// <summary>
        /// Go to the Add a User View
        /// </summary>
        /// <returns> Add User</returns>
        public IActionResult AddUser()
        {
            var teams = _userService.GetTeams()
                                   .Select(t => new SelectListItem
                                   {
                                       Value = t.TeamId.ToString(),
                                       Text = t.TeamName
                                   }) 
                                   .ToList();

            var userRoles = _userService.GetUserRoles()
                                   .Select(u => new SelectListItem
                                   {
                                       Value = u.RoleId.ToString(),
                                       Text = u.RoleName
                                   })
                                   .ToList();

            // Pass data to ViewBag
            ViewBag.Teams = new SelectList(teams, "Value", "Text");
            ViewBag.UserRoles = new SelectList(userRoles, "Value", "Text");

            return PartialView("AddUser");
        }

        [HttpPost]
        [Route("AddUser")]
        [AllowAnonymous]
        /// <summary>
        /// Post Request for Adding a User
        /// </summary>
        /// <returns> View User </returns>
        public IActionResult PostUserAdd(UserViewModel user)
        {
            _userService.AddUser(user);

            return RedirectToAction("ViewUser");
        }

        [HttpGet]
        [Route("UserEdit")]
        /// <summary>
        /// Go to the User Details View
        /// </summary>
        /// <returns> User Details</returns>
        /// 
        public IActionResult UserEdit(string UserId)
        {
            // Fetch user data
            var user = _userService.GetUsers().FirstOrDefault(x => x.UserId.ToString() == UserId);
            if (user == null)
            {
                return NotFound();
            }

            var userModel = new UserViewModel
            {
                UserId = user.UserId.ToString(),
                UserName = user.Username,
                Name = user.Name,
                Email = user.Email,
                Password = PasswordManager.DecryptPassword(user.Password),
                RoleId = user.RoleId,
                TeamId = user.TeamId.ToString(),
                RoleName = _userService.GetUserRoles().FirstOrDefault(r => r.RoleId == user.RoleId)?.RoleName,
                TeamName = _userService.GetTeams().FirstOrDefault(t => t.TeamId == user.TeamId)?.TeamName
            };

            var teams = _userService.GetTeams()
                                   .Select(t => new SelectListItem
                                   {
                                       Value = t.TeamId.ToString(),
                                       Text = t.TeamName
                                   })
                                   .ToList();

            var userRoles = _userService.GetUserRoles()
                                   .Select(u => new SelectListItem
                                   {
                                       Value = u.RoleId.ToString(),
                                       Text = u.RoleName
                                   })
                                   .ToList();

            // Pass data to ViewBag
            ViewBag.Teams = new SelectList(teams, "Value", "Text", userModel.TeamId);
            ViewBag.UserRoles = new SelectList(userRoles, "Value", "Text", userModel.RoleId);

            return PartialView("UserEdit", userModel);
        }

        [HttpPost]
        [Route("UserEdit")]
        /// <summary>
        /// Post Request for Adding a User
        /// </summary>
        /// <returns> View User </returns>
        public IActionResult PostUserEdit(UserViewModel user)
        {
            _userService.UpdateUser(user);

            return RedirectToAction("ViewUser");
        }

        [HttpGet]
        [Route("UserDelete")]
        /// <summary>
        /// Post Request for Adding a User
        /// </summary>
        /// <returns> View User </returns>
        public IActionResult UserDelete(string UserId)
        {
            var userToDelete = new UserViewModel
            {
                UserId = UserId,
            };

            return PartialView("UserDelete", userToDelete);
        }

        [HttpPost]
        [Route("UserDelete")]
        /// <summary>
        /// Post Request for Adding a User
        /// </summary>
        /// <returns> View User </returns>
        public IActionResult PostUserDelete(string UserId)
        {
            _userService.DeleteUser(UserId);

            return RedirectToAction("ViewUser");
        }

        public IActionResult ViewTeams()
        {
            ViewBag.IsLoginOrRegister = false;
            var teams = _userService.GetTeams()
                                   .Select(t => new SelectListItem
                                   {
                                       Value = t.TeamId.ToString(),
                                       Text = t.TeamName
                                   })
                                   .ToList();
            ViewBag.Teams = new SelectList(teams, "Value", "Text");
            ViewBag.AdminSidebar = "ViewUser";
            return View();
        }

        [HttpGet("/AddTeam")]
        /// <summary>
        /// Go to the Add a User View
        /// </summary>
        /// <returns> Add User</returns>
        public IActionResult AddTeam()
        {
            return PartialView("AddTeam");
        }

        [HttpPost("/AddTeam")]
        /// <summary>
        /// Post Request for Adding a User
        /// </summary>
        /// <returns> View User </returns>
        public IActionResult PostTeamAdd(UserViewModel team)
        {
            _userService.AddTeam(team);

            return RedirectToAction("ViewTeams");
        }
    }
}

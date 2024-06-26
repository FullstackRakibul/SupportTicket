﻿using Microsoft.EntityFrameworkCore;
using SupportApp.DTO;
using SupportApp.Models;

namespace SupportApp.Service
{
    public class TargetService
    {
        private readonly SupportAppDbContext _context;
        public TargetService(SupportAppDbContext context)
        {
            _context = context;
        }
        public void InitialTargetCreate(TicketAndTargetDto ticketAndTargetDto)
        {
            try
            {
                var targetCreate = new Target
                {
                   TicketId = ticketAndTargetDto.TicketId,
                   DepartmentId = ticketAndTargetDto.DepartmentId,
                   UnitId = ticketAndTargetDto.UnitId,
                   Objective = ticketAndTargetDto.Objective,
                };

                _context.Target.Add(targetCreate);
                _context.SaveChanges();

                Console.WriteLine("Target Initialized");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Target Initial service error !!", ex.Message);

            }
        }

        public async void AssignSupportEngineer(int ticketId , int agentId)
        {

            // find target record where the ticket id matched , then update that target and only assign AgentId 

            try {
                var selectTarget = _context.Target.FirstOrDefault(t => t.TicketId == ticketId);

                if (selectTarget != null)
                {
                    selectTarget.AgentId = agentId;
                    _context.SaveChanges();
                    Console.WriteLine("Assign support engineer service successfully !");


					int newTargetId = selectTarget.Id;
					var newNotification = new Notification
					{
						UserId = selectTarget.AgentId.ToString(),
						IsRead = false,
						TargetId = newTargetId,
						Message = $"A new ticket Ticket has been created.",
						CreatedAt = DateTime.UtcNow.AddHours(6)
					};
					_context.Notification.Add(newNotification);
					await _context.SaveChangesAsync();
				}
                else {

                    var newTarget = new Target
                    {
                        TicketId = ticketId,
                        AgentId = agentId,
                        DepartmentId = _context.Department.Where(d => d.DepartmentName == "Information Technology").FirstOrDefault().Id,
                        UnitId = _context.Unit.Where(u => u.Name == "Corporate Office").FirstOrDefault().Id,
                    };

                     _context.Target.Add(newTarget);
                    await _context.SaveChangesAsync();

					int newTargetId = newTarget.Id;
					var newNotification = new Notification
					{
						UserId = newTarget.AgentId.ToString(),
						IsRead = false,
						TargetId = newTargetId,
						Message = $"A new ticket Ticket has been created.",
						CreatedAt = DateTime.UtcNow.AddHours(6)
					};
					_context.Notification.Add(newNotification);
					await _context.SaveChangesAsync();
				}


            }
            catch (Exception ex) {
                Console.WriteLine("Assign upport engineer service error !",ex.Message);
            }
        }

        
    }
}

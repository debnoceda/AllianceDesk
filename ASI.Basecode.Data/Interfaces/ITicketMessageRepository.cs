﻿using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface ITicketMessageRepository
    {
            IEnumerable<TicketMessage> RetrieveAll();

            void Add(TicketMessage ticketMessage);

            IEnumerable<TicketMessage> GetMessagesByTicketId(Guid ticketId);
    }
}

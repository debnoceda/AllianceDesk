﻿using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface ITicketActivityOperationRepository
    {
        IEnumerable<TicketActivityOperation> RetrieveAll();
        IEnumerable<TicketActivityOperation> GetOperationsByIds(IEnumerable<byte> operationIds);
    }
}


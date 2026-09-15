using System;
using System.Collections.Generic;
using System.Text;

namespace BackOffice.Application.DTOs {
    public class CreateServiceRequestInput {
        public string Title { get; }

        public Guid CreatedByUserId { get; }

        public CreateServiceRequestInput(string title, Guid createdByUserId) {
            Title = title;
            CreatedByUserId = createdByUserId;
        }
    }
}

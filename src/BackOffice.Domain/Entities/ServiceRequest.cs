using BackOffice.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackOffice.Domain.Entities {
    public class ServiceRequest {
        public Guid Id { get; private set; }

        public string Title { get; private set; }

        public User CreatedBy { get; private set; }

        public ServiceRequestStatus Status { get; private set; }

        public User? AssignedTo { get; private set; }

        public string? CancellationReason {  get; private set; }

        public string? ReopeningReason {  get; private set; }

        private ServiceRequest() {
            Title = null;
            CreatedBy = null;
        }

        public ServiceRequest(string title, User createdBy) {
            if (string.IsNullOrWhiteSpace(title)) {
                throw new ArgumentException(
                        "O título da solicitação é obrigatório.",
                        nameof(title)
                    );
            }

            ArgumentNullException.ThrowIfNull(createdBy);

            Id = Guid.NewGuid();
            Title = title.Trim();
            CreatedBy = createdBy;
            Status = ServiceRequestStatus.Open;
        }

        public void AssignTo(User assigedTo) {
            ArgumentNullException.ThrowIfNull(assigedTo);

            if(Status == ServiceRequestStatus.Completed ||
                Status == ServiceRequestStatus.Cancelled) {
                throw new InvalidOperationException(
                    "Não é possível atribuir responsável a uma solicitação encerrada."
                );
            }

            AssignedTo = assigedTo;
        }

        public void StartProgressing() {
            if (Status != ServiceRequestStatus.Open) {
                throw new InvalidOperationException("Apenas solicitações abertas podem iniciar o atendimento.");
            }

            if(AssignedTo is null) {
                throw new InvalidOperationException (
                    "A solicitação precisa ter um responsável atribuído antes de iniciar o atendimento."
                );
            }

            Status = ServiceRequestStatus.InProgress;
        }

        public void Complete() {
            if(Status != ServiceRequestStatus.InProgress) {
                throw new InvalidOperationException("Apenas solicitações em atendimento pode ser concluídas");
            }
            Status = ServiceRequestStatus.Completed;
        }

        public void Cancel(string reason) {
            if(Status != ServiceRequestStatus.Open &&
                Status != ServiceRequestStatus.InProgress) { 
                throw new InvalidOperationException("A solicitação não pode ser cancelada no status atual.");
            }

            if (string.IsNullOrWhiteSpace(reason)) {
                throw new ArgumentException("O motivo do cancelamento é obrigatório.",
                    nameof(reason));
            }

            CancellationReason = reason.Trim();
            Status = ServiceRequestStatus.Cancelled;
        }

        public void Reopen(string reason) {
            if(Status != ServiceRequestStatus.Completed) {
                throw new InvalidOperationException(
                    "Apenas solicitações concluídas podem ser reabertas."
                );
            }

            if (string.IsNullOrWhiteSpace(reason)) {
                throw new ArgumentException(
                    "A justiça da reabertura é obrigatória.",
                    nameof(reason)
                );
            }

            ReopeningReason = reason.Trim();
            Status = ServiceRequestStatus.Open;
        }
    }
}
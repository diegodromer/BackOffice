using BackOffice.Domain.Entities;
using BackOffice.Domain.Enums;

namespace BackOffice.Domain.Tests {
    public class ServiceRequestTests {

        private static User CreateValidUser() {
            return new User("Diego Lima", "diego@estudoscsharpe.com");
        }

        [Fact]
        public void Create_WithValidTitle_ShouldCreateOpenRequest() {
            var createBy = CreateValidUser();
            var request = new ServiceRequest("Computador does not turn on ", createBy);

            Assert.NotEqual(Guid.Empty, request.Id);
            Assert.Equal("Computador does not turn on", request.Title);
            Assert.Same(createBy, request.CreatedBy);
            Assert.Equal(ServiceRequestStatus.Open, request.Status);
        }

        [Fact]
        public void Create_WithEmptyTitle_ShouldThrowArgumentException() {
            var exception = Assert.Throws<ArgumentException>(() => {
                new ServiceRequest("", CreateValidUser());
            });
            Assert.Equal("title", exception.ParamName);
        }

        [Fact]
        public void StartProcessing_WhenRequestIsOpen_ShouldChangeStatusToInProgress() {
            var request = new ServiceRequest("Computador does not turn on", CreateValidUser());

            request.AssignTo(CreateValidUser());

            request.StartProgressing();
            
            Assert.Equal(ServiceRequestStatus.InProgress, request.Status);
        }

        [Fact]
        public void StartProcessing_WhenRequestIsAlreadyInProgress_ShouldThrowInvalidOperationException() {
            var request = new ServiceRequest("Computer does not turn on", CreateValidUser());
            
            request.AssignTo(CreateValidUser());
            request.StartProgressing();

            Assert.Throws<InvalidOperationException>(() => {
                request.StartProgressing();
            });
        }

        [Fact]
        public void Complete_WhenRequestIsInProgress_ShouldChangeStatusToCompleted() {
            var request = new ServiceRequest("Computer does not turn on", CreateValidUser());
            
            request.AssignTo(CreateValidUser());
            request.StartProgressing();
            request.Complete();

            Assert.Equal(ServiceRequestStatus.Completed, request.Status);
        }

        [Fact]
        public void Complete_WhenRequestIsOpen_ShouldThrowInvalidOperationException() {
            var request = new ServiceRequest("Computer does not turn on", CreateValidUser());
            Assert.Throws<InvalidOperationException>(() => {
                request.Complete();
            });
        }

        [Fact]
        public void Cancel_WhenRequestIsOpenWithReason_ShouldChangeStatusToCancelled() {
            var request = new ServiceRequest("Computer does not turn on", CreateValidUser());

            request.Cancel("Request is no longer necessary");

            Assert.Equal(ServiceRequestStatus.Cancelled, request.Status);
        }

        [Fact]
        public void Cancel_WhenRequestIsInProgressWithReason_ShouldChangeStatusToCancelled() {
            var request = new ServiceRequest("Computer does not turn on", CreateValidUser());

            request.AssignTo(CreateValidUser());
            request.StartProgressing();
            request.Cancel("Equipament was replaced.");

            Assert.Equal(ServiceRequestStatus.Cancelled, request.Status);
        }

        [Fact]
        public void Cancel_WhenReasonIsEmpty_ShouldThrowArgumentException() {
            var request = new ServiceRequest("Computer does not turn on", CreateValidUser());
            var exception = Assert.Throws<ArgumentException>(() => {
                request.Cancel("");
            });
            Assert.Equal(ServiceRequestStatus.Open, request.Status);
            Assert.Null(request.CancellationReason);
        }

        [Fact]
        public void Cancel_WhenRequestIsCompleted_ShouldThrowInvalidOperationException() {
            var request = new ServiceRequest("Computer does not turn on", CreateValidUser());

            request.AssignTo(CreateValidUser());
            request.StartProgressing();
            request.Complete();

            Assert.Throws<InvalidOperationException>(() => {
                request.Cancel("Canceling after completion.");
            });

            Assert.Equal(ServiceRequestStatus.Completed, request.Status);
        }

        [Fact]
        public void Create_WithNullCreatedBy_ShouldThrowArgumentNullException() {
            Assert.Throws<ArgumentNullException>(() => {
                new ServiceRequest("Impressora sem toner", null);
            });
        }

        [Fact]
        public void AssignTo_WhenRequestIsOpen_ShouldAssignUser() {
            var request = new ServiceRequest(
                "Computer does not turn on",
                CreateValidUser()
            );

            var assignedTo = CreateValidUser();

            request.AssignTo(assignedTo);

            Assert.Same(assignedTo, request.AssignedTo);
        }

        [Fact]
        public void AssignTo_WithNullUser_ShouldThrowArgumentNullException() {
            var request = new ServiceRequest(
                "Computer does not turn on",
                CreateValidUser()
            );

            Assert.Throws<ArgumentNullException>(() => {
                request.AssignTo(null);
            });
        }

        [Fact]
        public void AssingTo_WhenRequestIsCompleted_ShouldThrowInvalidOperationException() {
            var request = new ServiceRequest(
                "Computer does not turn on",
                CreateValidUser()
            );

            request.AssignTo(CreateValidUser());
            request.StartProgressing();
            request.Complete();

            Assert.Throws<InvalidOperationException>(() => {
                request.AssignTo(CreateValidUser());
            });
        }

        [Fact]
        public void AssignTo_WhenRequestIsCancelled_ShouldThrowInvalidOperationException() {
            var request = new ServiceRequest(
                "Computer does not turn on",
                CreateValidUser()
            );

            request.Cancel("Request is no longer necessary");

            Assert.Throws<InvalidOperationException>(() => {
                request.AssignTo(CreateValidUser());
            });
        }

        [Fact]
        void StartProcessing_WhenRequestHasNoAssignedUser_ShouldThrowInvalidOperationException() {
            var request = new ServiceRequest(
                "Computer does not turn on",
                CreateValidUser()
            );

            Assert.Throws<InvalidOperationException>(() => {
                request.StartProgressing();
            });

            Assert.Equal(ServiceRequestStatus.Open, request.Status);
        }

        [Fact]
        public void Cancel_WhenRequestIsOpen_ShouldChangeStatusToCancelledAndSetCancellationReason() {
            var request = new ServiceRequest(
                "Computer does not turn on",
                CreateValidUser()
            );

            request.Cancel(" Request is no longer necessary ");

            Assert.Equal(ServiceRequestStatus.Cancelled, request.Status);
            Assert.Equal("Request is no longer necessary", request.CancellationReason);
        }

        [Fact]
        public void Reopen_WhenRequestIsCompletedWithValidReason_ShouldChangeStatusToOpen() {
            var request = new ServiceRequest(
                "Computer does not turn on",
                CreateValidUser()
            );

            request.AssignTo(CreateValidUser());
            request.StartProgressing();
            request.Complete();

            request.Reopen(" The problem happened again ");

            Assert.Equal(ServiceRequestStatus.Open, request.Status);
            Assert.Equal("The problem happened again", request.ReopeningReason);
        }

        [Fact]
        public void Reopen_WhenReasonIsEmpty_ShouldThrowArgumentException() {
            var request = new ServiceRequest(
                "Computer does not turn on",
                CreateValidUser()
            );

            request.AssignTo(CreateValidUser());
            request.StartProgressing();
            request.Complete();

            Assert.Throws<ArgumentException>( () => {
                request.Reopen(" ");
            });

            Assert.Equal(ServiceRequestStatus.Completed, request.Status);
            Assert.Null(request.ReopeningReason);
        }

        [Fact]
        public void Reopen_WhenRequestIsOpen_ShouldThrowInvalidOperationException() {
            var request = new ServiceRequest(
                "Computer does not turn on",
                CreateValidUser()
            );

            Assert.Throws<InvalidOperationException>(() => {
                request.Reopen("The problem happened again");
            });

            Assert.Equal(ServiceRequestStatus.Open, request.Status);
            Assert.Null(request.ReopeningReason);
        }
    }
}

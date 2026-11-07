using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using Portfolio.Application.Common.Interfaces;
using Portfolio.Application.Common.Options;
using Portfolio.Application.Features.Contact.Commands;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interfaces;
using Shouldly;

namespace Portfolio.UnitTests.Application.CommandHandlerTests;

[TestFixture]
public class SendContactMessageCommandHandlerTests
{
    private Mock<IUnitOfWork> _unitOfWork = null!;
    private Mock<IRepository<ContactMessage>> _messagesRepository = null!;
    private Mock<IEmailService> _emailService = null!;
    private SendContactMessageCommandHandler _handler = null!;

    private const string RecipientEmail = "adib@portfolio.local";

    [SetUp]
    public void SetUp()
    {
        _messagesRepository = new Mock<IRepository<ContactMessage>>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _unitOfWork.Setup(w => w.ContactMessages).Returns(_messagesRepository.Object);
        _unitOfWork
            .Setup(w => w.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _emailService = new Mock<IEmailService>();

        var emailSettings = new EmailSettings
        {
            SmtpHost = "smtp.test.local",
            SenderEmail = "noreply@portfolio.local",
            RecipientEmail = RecipientEmail
        };

        _handler = new SendContactMessageCommandHandler(
            _unitOfWork.Object,
            _emailService.Object,
            Microsoft.Extensions.Options.Options.Create(emailSettings));
    }

    [Test]
    public async Task Handle_PersistsMessageAndNotifiesOwner_ReturnsTrue()
    {
        var command = new SendContactMessageCommand(
            "Jane Doe", "jane@example.com", "Project opportunity", "Hello, are you available?", "10.0.0.1");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldBeTrue();

        _messagesRepository.Verify(r => r.AddAsync(
            It.Is<ContactMessage>(m =>
                m.SenderName == command.SenderName &&
                m.SenderEmail == command.SenderEmail &&
                m.Subject == command.Subject &&
                m.MessageBody == command.MessageBody &&
                m.IpAddress == "10.0.0.1" &&
                !m.IsRead),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWork.Verify(w => w.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _emailService.Verify(e => e.SendEmailAsync(
            RecipientEmail,
            It.Is<string>(s => s.Contains(command.Subject)),
            It.Is<string>(b => b.Contains(command.SenderName) && b.Contains(command.SenderEmail) && b.Contains(command.MessageBody)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_PersistsMessage_BeforeSendingEmail()
    {
        var callOrder = new List<string>();

        _messagesRepository
            .Setup(r => r.AddAsync(It.IsAny<ContactMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback(() => callOrder.Add("add"));
        _emailService
            .Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback(() => callOrder.Add("send"));

        _unitOfWork
            .Setup(w => w.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1)
            .Callback(() => callOrder.Add("save"));

        await _handler.Handle(
            new SendContactMessageCommand("A", "a@example.com", "S", "B", null),
            CancellationToken.None);

        callOrder.IndexOf("add").ShouldBeLessThan(callOrder.IndexOf("save"), "message must persist before save");
        callOrder.IndexOf("save").ShouldBeLessThan(callOrder.IndexOf("send"), "email must be dispatched only after the message persisted");
    }
}

[TestFixture]
public class CreateProjectCommandHandlerTests
{
    [Test]
    public async Task Handle_AddsProjectAndReturnsNewId()
    {
        var repository = new Mock<IRepository<Project>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(w => w.Projects).Returns(repository.Object);
        unitOfWork.Setup(w => w.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new Portfolio.Application.Features.Projects.Commands.CreateProjectCommandHandler(unitOfWork.Object);
        var command = new Portfolio.Application.Features.Projects.Commands.CreateProjectCommand(
            "MaleFashion", "E-commerce app", "Web", null, null, null, true, 1, ["A"], ["B"]);

        var id = await handler.Handle(command, CancellationToken.None);

        id.ShouldNotBe(Guid.Empty);
        repository.Verify(r => r.AddAsync(
            It.Is<Project>(p => p.Id == id && p.Title == "MaleFashion" && p.IsFeatured),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(w => w.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

[TestFixture]
public class UpdateProjectCommandHandlerTests
{
    [Test]
    public async Task Handle_WhenProjectExists_UpdatesAndReturnsTrue()
    {
        var existing = new Project("Old", "Old description", "Old", null, null, null, false, 0, [], []);
        var repository = new Mock<IRepository<Project>>();
        repository.Setup(r => r.GetByIdAsync<Guid>(existing.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(w => w.Projects).Returns(repository.Object);
        unitOfWork.Setup(w => w.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new Portfolio.Application.Features.Projects.Commands.UpdateProjectCommandHandler(unitOfWork.Object);
        var command = new Portfolio.Application.Features.Projects.Commands.UpdateProjectCommand(
            existing.Id, "New", "New description", "New category", null, null, null, true, 7, ["H"], ["T"]);

        var result = await handler.Handle(command, CancellationToken.None);

        result.ShouldBeTrue();
        existing.Title.ShouldBe("New");
        existing.IsFeatured.ShouldBeTrue();
        existing.DisplayOrder.ShouldBe(7);
        unitOfWork.Verify(w => w.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_WhenProjectIsMissing_ThrowsKeyNotFound()
    {
        var repository = new Mock<IRepository<Project>>();
        repository.Setup(r => r.GetByIdAsync<Guid>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(w => w.Projects).Returns(repository.Object);

        var handler = new Portfolio.Application.Features.Projects.Commands.UpdateProjectCommandHandler(unitOfWork.Object);
        var command = new Portfolio.Application.Features.Projects.Commands.UpdateProjectCommand(
            Guid.NewGuid(), "T", "D", "C", null, null, null, false, 0, [], []);

        await Should.ThrowAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}

[TestFixture]
public class DeleteProjectCommandHandlerTests
{
    [Test]
    public async Task Handle_WhenProjectExists_RemovesAndReturnsTrue()
    {
        var existing = new Project("T", "D", "C", null, null, null, false, 0, [], []);
        var repository = new Mock<IRepository<Project>>();
        repository.Setup(r => r.GetByIdAsync<Guid>(existing.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(w => w.Projects).Returns(repository.Object);
        unitOfWork.Setup(w => w.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new Portfolio.Application.Features.Projects.Commands.DeleteProjectCommandHandler(unitOfWork.Object);

        var result = await handler.Handle(
            new Portfolio.Application.Features.Projects.Commands.DeleteProjectCommand(existing.Id),
            CancellationToken.None);

        result.ShouldBeTrue();
        repository.Verify(r => r.Remove(existing), Times.Once);
        UnitOfWorkDisposedIsNotEnforcedByMock(unitOfWork);
    }

    private static void UnitOfWorkDisposedIsNotEnforcedByMock(Mock<IUnitOfWork> unitOfWork)
    {
        unitOfWork.Verify(w => w.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_WhenProjectIsMissing_ThrowsKeyNotFound()
    {
        var repository = new Mock<IRepository<Project>>();
        repository.Setup(r => r.GetByIdAsync<Guid>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(w => w.Projects).Returns(repository.Object);

        var handler = new Portfolio.Application.Features.Projects.Commands.DeleteProjectCommandHandler(unitOfWork.Object);

        await Should.ThrowAsync<KeyNotFoundException>(() => handler.Handle(
            new Portfolio.Application.Features.Projects.Commands.DeleteProjectCommand(Guid.NewGuid()),
            CancellationToken.None));
    }
}

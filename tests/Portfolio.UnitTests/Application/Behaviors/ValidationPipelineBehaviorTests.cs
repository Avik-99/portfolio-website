using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Portfolio.Application.Common.Behaviors;
using Shouldly;

namespace Portfolio.UnitTests.Application.Behaviors;

internal sealed record TestPipelineRequest(string Value) : IRequest<string>;

internal sealed class TestPipelineRequestValidator : AbstractValidator<TestPipelineRequest>
{
    public TestPipelineRequestValidator()
    {
        RuleFor(r => r.Value)
            .NotEmpty()
            .WithMessage("Value must not be empty.");
    }
}

internal sealed class CapturingValidator(bool returnFailure) : AbstractValidator<TestPipelineRequest>
{
    public List<CancellationToken> ObservedTokens { get; } = [];

    public override Task<ValidationResult> ValidateAsync(
        ValidationContext<TestPipelineRequest> context,
        CancellationToken cancellation = default)
    {
        ObservedTokens.Add(cancellation);

        return Task.FromResult(returnFailure
            ? new ValidationResult([new ValidationFailure("Value", "Captured failure.")])
            : new ValidationResult());
    }
}

[TestFixture]
public class ValidationPipelineBehaviorTests
{
    private delegate Task<string> NextDelegate(CancellationToken cancellationToken = default);

    private static ValidationPipelineBehavior<TestPipelineRequest, string> CreateBehavior(
        params IValidator<TestPipelineRequest>[] validators) =>
        new(validators);

    private static NextDelegate CreateNext(List<int> callLog) =>
        _ => { callLog.Add(1); return Task.FromResult("handler-result"); };

    [Test]
    public async Task Handle_NoValidatorsRegistered_CallsNextReturnsHandlerResult()
    {
        var callLog = new List<int>();
        var behavior = CreateBehavior();

        var result = await behavior.Handle(
            new TestPipelineRequest("anything"),
            CreateNext(callLog).Invoke,
            CancellationToken.None);

        result.ShouldBe("handler-result");
        callLog.Count.ShouldBe(1, "the handler must run when no validators are registered");
    }

    [Test]
    public async Task Handle_ValidRequest_CallsNextAndReturnsHandlerResult()
    {
        var callLog = new List<int>();
        var behavior = CreateBehavior(new TestPipelineRequestValidator());

        var result = await behavior.Handle(
            new TestPipelineRequest("valid"),
            CreateNext(callLog).Invoke,
            CancellationToken.None);

        result.ShouldBe("handler-result");
        callLog.Count.ShouldBe(1);
    }

    [Test]
    public async Task Handle_InvalidRequest_ThrowsValidationExceptionInsteadOfCallingNext()
    {
        var callLog = new List<int>();
        var behavior = CreateBehavior(new TestPipelineRequestValidator());

        var exception = await Should.ThrowAsync<ValidationException>(() =>
            behavior.Handle(
                new TestPipelineRequest(""),
                CreateNext(callLog).Invoke,
                CancellationToken.None));

        exception.Errors.ShouldContain(f => f.PropertyName == "Value" && f.ErrorMessage == "Value must not be empty.");
        callLog.Count.ShouldBe(0, "the pipeline must short-circuit and never invoke the handler");
    }

    [Test]
    public async Task Handle_MultipleValidators_AggregatesAllFailures()
    {
        var callLog = new List<int>();
        var failing = new CapturingValidator(returnFailure: true);
        var alsoFailing = new CapturingValidator(returnFailure: true);

        var behavior = CreateBehavior(
            new TestPipelineRequestValidator(),
            failing,
            alsoFailing);

        var exception = await Should.ThrowAsync<ValidationException>(() =>
            behavior.Handle(
                new TestPipelineRequest(""),
                CreateNext(callLog).Invoke,
                CancellationToken.None));

        exception.Errors.Count(f => f.PropertyName == "Value").ShouldBe(3);
        exception.Errors.ShouldContain(f => f.ErrorMessage == "Value must not be empty.");
        exception.Errors.Count(f => f.ErrorMessage == "Captured failure.").ShouldBe(2);
        callLog.Count.ShouldBe(0);
    }

    [Test]
    public async Task Handle_PassesTheCancellationTokenDownToValidators()
    {
        var validator = new CapturingValidator(returnFailure: false);
        var behavior = CreateBehavior(validator);

        using var cts = new CancellationTokenSource();
        var nextCalled = false;
        await behavior.Handle(
            new TestPipelineRequest("valid"),
            _ => { nextCalled = true; return Task.FromResult("handler-result"); },
            cts.Token);

        validator.ObservedTokens.ShouldContain(cts.Token);
        nextCalled.ShouldBeTrue();
    }
}

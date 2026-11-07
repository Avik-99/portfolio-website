using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Features.Contact.Commands;
using Portfolio.Web.ViewModels;

namespace Portfolio.Web.Controllers;

public class ContactController : Controller
{
    private readonly IMediator _mediator;

    public ContactController(IMediator mediator) => _mediator = mediator;

    [HttpPost("contact/send")]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> SendMessage(ContactFormViewModel model)
    {
        // Honeypot anti-spam: hidden field must stay empty for real users.
        var honeypot = Request.Form["_hp"].ToString();
        if (!string.IsNullOrWhiteSpace(honeypot))
        {
            return Json(new { success = false, message = "Message could not be sent." });
        }

        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Validation failed. Please check your input." });
        }

        var command = new SendContactMessageCommand(
            model.Name,
            model.Email,
            model.Subject,
            model.Message,
            HttpContext.Connection.RemoteIpAddress?.ToString());

        var result = await _mediator.Send(command);

        return Json(new
        {
            success = result,
            message = result ? "Message sent successfully!" : "Failed to send message. Please try again later."
        });
    }
}

using System;
using Microsoft.AspNetCore.Identity;
using WebAPI.Models;

namespace WebAPI.Helpers;

// Class for email sender and inherite from IEmailSender
public class FakeEmailSender : IEmailSender<UserModel>
{
    // Send confirmation link to user
    public Task SendConfirmationLinkAsync(UserModel user, string email, string confirmationLink)
    {
        Console.WriteLine("Send confirmation link to {0} with {1}", email, confirmationLink);
        return Task.CompletedTask;
    }
    // Send password reset code to user
    public Task SendPasswordResetCodeAsync(UserModel user, string email, string resetCode)
    {
        Console.WriteLine("Send password reset code to {0} with {1}", email, resetCode);
        return Task.CompletedTask;
    }
    // Send password reset link to user
    public Task SendPasswordResetLinkAsync(UserModel user, string email, string resetLink)
    {
        Console.WriteLine("Send password reset link to {0} with {1}", email, resetLink);
        return Task.CompletedTask;
    }
}

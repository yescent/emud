﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Notifications;
using Emprise.Domain.User.Models;
using Emprise.Infra.Authorization;
using Emprise.Infra.Extensions;
using Emprise.MudServer.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Emprise.Web.Pages.User
{
    [AllowAnonymous]
    public class RegModel : BasePageModel
    {
        private readonly IAccountContext _accountContext;
        private readonly IMediatorHandler _bus;
        private readonly DomainNotificationHandler _notifications;

        public RegModel(IAccountContext accountContext,
            IOptionsMonitor<AppConfig> appConfig,
            INotificationHandler<DomainNotification> notifications,
            IMediatorHandler bus) : base(appConfig)
        {
            _accountContext = accountContext;
            _notifications = (DomainNotificationHandler)notifications;
            _bus = bus;
        }


        public string Email { get; set; }

        public string Code { get; set; }

        public async Task<IActionResult> OnGetAsync(string email, string code)
        {

            Email = email;
            Code = code;
            if (await HttpContext.HasLogin())
            {
                return RedirectToPage("/User/Index");
            }

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/User/VerifyEmail");
            }
            if (!email.IsEmail())
            {
                return RedirectToPage("/User/VerifyEmail");
            }
            
            return Page();
        }


        public async Task<IActionResult> OnPostAsync([FromBody]UserRegDto dto)
        {
            var userId = _accountContext.UserId;

            var command = new RegCommand(dto.Email, dto.Password, dto.Code);
            await _bus.SendCommand(command);

            if (_notifications.HasNotifications())
            {
                var errorMessage = string.Join("；", _notifications.GetNotifications().Select(x => x.Content));
                return await Task.FromResult(new JsonResult(new
                {
                    status = false,
                    errorMessage
                }));
            }

            return await Task.FromResult(new JsonResult(new
            {
                status = true
            }));

        }
    }
}
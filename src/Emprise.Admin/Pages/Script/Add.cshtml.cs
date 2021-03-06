﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Api;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Emprise.Admin.Models.Script;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Admin.Pages.NpcScript
{
    public class AddModel : BasePageModel
    {
        public AddModel(IMudClient mudClient,
            IMapper mapper,
            ILogger<AddModel> logger,
            EmpriseDbContext db,
            IOptionsMonitor<AppConfig> appConfig,
            IHttpContextAccessor httpAccessor)
            : base(db, appConfig, httpAccessor, mapper, logger, mudClient)
        {

        }

        [BindProperty]
        public ScriptInput Script { get; set; }

        public string ErrorMessage { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }

        public void OnGet()
        {
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Script/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }



            try
            {
                var script = _mapper.Map<ScriptEntity>(Script);
                await _db.Scripts.AddAsync(script);
                await _db.SaveChangesAsync();

                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.添加脚本,
                    Content = JsonConvert.SerializeObject(Script)
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.添加脚本,
                    Content = $"Data={JsonConvert.SerializeObject(Script)},ErrorMessage={ErrorMessage}"
                });
                return Page();
            }

            return Redirect(UrlReferer);
        }
    }
}
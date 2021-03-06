﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Api;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Emprise.Admin.Models.NpcScript;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Admin.Pages.ScriptCommand
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
        public ScriptCommandInput ScriptCommand { get; set; }


        public string ErrorMessage { get; set; }

        public Array Conditions { get; set; }

        public Array Fields { get; set; }

        public Array Relations { get; set; }

        public Array Events { get; set; }

        public Array Commands { get; set; }

        public Array Activities { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }

        public async Task OnGetAsync(int sId)
        {
            Conditions = Enum.GetNames(typeof(ConditionTypeEnum));

            Fields = Enum.GetNames(typeof(PlayerConditionFieldEnum));

            Relations = Enum.GetNames(typeof(LogicalRelationTypeEnum));

            Events = Enum.GetNames(typeof(PlayerEventTypeEnum));

            Commands = Enum.GetNames(typeof(CommandTypeEnum));

            Activities = Enum.GetNames(typeof(ActivityTypeEnum));

            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/ScriptCommand/Index", new { sId = sId });
            }

        }

        public async Task<IActionResult> OnPostAsync(int sId)
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }



            try
            {
                var scriptCommand = _mapper.Map<ScriptCommandEntity>(ScriptCommand);
                scriptCommand.ScriptId = sId;
                await _db.ScriptCommands.AddAsync(scriptCommand);

                if (scriptCommand.IsEntry)
                {
                    var scriptCommands = _db.ScriptCommands.Where(x => x.ScriptId == scriptCommand.ScriptId).ToList();

                    foreach (var command in scriptCommands.Where(x => x.Id != scriptCommand.Id))
                    {
                        command.IsEntry = false;
                    }
                }
                await _db.SaveChangesAsync();

                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.添加脚本分支,
                    Content = JsonConvert.SerializeObject(ScriptCommand)
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.添加脚本分支,
                    Content = $"Data={JsonConvert.SerializeObject(ScriptCommand)},ErrorMessage={ErrorMessage}"
                });
                return Page();
            }

            return Redirect(UrlReferer);
        }
    }
}
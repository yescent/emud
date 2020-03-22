﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Models.NpcScript;
using Emprise.Domain.Core.Enum;
using Emprise.Domain.Npc.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emprise.Admin.Pages.ScriptCommand
{
    public class AddModel : PageModel
    {
        protected readonly EmpriseDbContext _db;
        private readonly IMapper _mapper;

        public AddModel(EmpriseDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [BindProperty]
        public ScriptCommandInput ScriptCommand { get; set; }

        public string Tips { get; set; }
        public string SueccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public Array Types { get; set; }

        public Array Fields { get; set; }

        public Array Relations { get; set; }

        public Array Events { get; set; }

        public Array Commonds { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }

        public async Task OnGetAsync(int sId)
        {
            Types =  Enum.GetNames(typeof(ConditionTypeEnum));

            Fields = Enum.GetNames(typeof(PlayerConditionFieldEnum));

            Relations = Enum.GetNames(typeof(LogicalRelationTypeEnum));

            Events = Enum.GetNames(typeof(PlayerEventTypeEnum));

            Commonds = Enum.GetNames(typeof(CommondTypeEnum));

            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/ScriptCommand/Index", new { sId = sId });
            }

        }

        public async Task<IActionResult> OnPostAsync(int sId)
        {
            SueccessMessage = "";
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                ErrorMessage = ModelState.Where(e => e.Value.Errors.Count > 0).Select(e => e.Value.Errors.First().ErrorMessage).First();
                return Page();
            }

            var scriptCommand = _mapper.Map<ScriptCommandEntity>(ScriptCommand);
            scriptCommand.ScriptId = sId;
            await _db.ScriptCommands.AddAsync(scriptCommand);

            await _db.SaveChangesAsync();



            SueccessMessage = $"添加成功！";

            //return RedirectToPage("Edit", new { id = scriptCommand.Id });

            return Redirect(UrlReferer);
        }
    }
}
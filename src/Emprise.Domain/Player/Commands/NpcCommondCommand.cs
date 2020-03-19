﻿using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Player.Commands
{

    public class NpcActionCommand : Command
    {
        public int NpcId { get; set; }

        public string Action { get; set; }

        public int PlayerId { get; set; }

        public int ScriptId { get; set; }


        public NpcActionCommand(int playerId, int npcId, int scriptId, string action)
        {
            NpcId = npcId;
            PlayerId = playerId;
            Action = action;
            ScriptId = scriptId;
        }

    }
}

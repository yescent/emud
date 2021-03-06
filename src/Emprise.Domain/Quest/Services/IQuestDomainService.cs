﻿using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Quest.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Quest.Services
{
    public interface IQuestDomainService : IBaseService
    {
        Task<QuestEntity> Get(Expression<Func<QuestEntity, bool>> where);

        Task<IQueryable<QuestEntity>> GetAll();

        Task<QuestEntity> Get(int id);

        Task Add(QuestEntity user);

        Task Update(QuestEntity user);

        Task Delete(QuestEntity quest);
    }
}

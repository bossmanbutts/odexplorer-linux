using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ODUtils.Database.DTOs;
using ODUtils.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ODExplorer.Database
{
    public static class DbSetExtensions
    {
        public static EntityEntry<T>? AddIfNotExists<T>(this DbSet<T> dbSet, T entity, Expression<Func<T, bool>>? predicate = null) where T : class, new()
        {
            var exists = predicate != null ? dbSet.Any(predicate) : dbSet.Any();
            return !exists ? dbSet.Add(entity) : null;
        }
    }

    public static class JournalEntryQueryExtensions
    {
        public static IQueryable<JournalEntryDTO> EventTypeCompare(this IQueryable<JournalEntryDTO> query, List<JournalTypeEnum> types)
            => query.Where(x => types.Contains((JournalTypeEnum)x.EventTypeId));
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Data
{
    public static class MTSDataHelper
    {
        /// <summary>
        /// Create and save a new instance of shift to database usgin given context <paramref name="context"/>
        /// </summary>
        /// <param name="context">Context used to save shift to database</param>
        /// <param name="start">Date and time when shift has been started</param>
        /// <param name="finish">Date and time then shift has been finished</param>
        /// <param name="mirrorId">Id of mirror which has been tested in current shift</param>
        /// <param name="operatorId">Id of operator who has executed current shift</param>
        /// <returns>New generated id of saved shift</returns>
        public static int SaveShift(MTSContext context, DateTime start, DateTime finish, int mirrorId, int operatorId)
        {
            Data.Shift dbShift = context.Shifts.Add(new Data.Shift
            {
                Start = start,
                Finish = finish,
                MirrorId = mirrorId,
                OperatorId = operatorId
            });
            context.SaveChanges();

            return dbShift.Id;
        }

    }
}

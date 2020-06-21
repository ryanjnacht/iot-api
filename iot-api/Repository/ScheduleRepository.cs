using System.Collections.Generic;
using System.Linq;
using iot_api.DataAccess;
using iot_api.Scheduler;

namespace iot_api.Repository
{
    public static class ScheduleRepository
    {
        public static void Insert(Schedule scheduleObj)
        {
            DataAccess<Schedule>.Insert(scheduleObj);
        }

        public static Schedule Get(string id)
        {
            var json = DataAccess<Schedule>.Get(id);
            return new Schedule(json);
        }

        public static List<Schedule> Get()
        {
            return DataAccess<Schedule>.Get().Select(json => new Schedule(json)).ToList();
        }

        public static void DeleteRecord(Schedule scheduleObj)
        {
            DataAccess<Schedule>.Delete(scheduleObj.Id);
        }

        public static void Clear()
        {
            DataAccess<Schedule>.Clear();
        }
    }
}
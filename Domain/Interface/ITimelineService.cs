using System.Collections.Generic;
using Model.Timeline.External;
using Model.Timeline.Data;
using DataAccess;

namespace Domain.Interface
{
    public interface ITimelineService
    {
        void AddTimelineData(User user, IEnumerable<SemanticTimeline> semantictimelines);

        TimelineData GetTimelineData(User user, RetrievalMethod method = RetrievalMethod.Id);

        long BenchmarkGetTimelineData(User user, RetrievalMethod method = RetrievalMethod.Id);
    }
}

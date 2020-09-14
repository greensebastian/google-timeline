using DataAccess;
using Model.Timeline.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Model.Timeline.External;
using Domain.Interface;
using System;
using System.Linq.Expressions;
using System.Diagnostics;
using Common;
using System.ComponentModel;

namespace Domain
{
    public class TimelineService : ITimelineService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public TimelineService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public void AddTimelineData(User user, IEnumerable<SemanticTimeline> semantictimelines)
        {
            var semantictimeline = new SemanticTimeline
            {
                timelineObjects = semantictimelines.SelectMany(timeline => timeline.timelineObjects).ToList()
            };

            var newPlaceVisits = semantictimeline.timelineObjects
                .Where(timelineObject => timelineObject.placeVisit != null)
                .Select(timelineObject => new DbPlaceVisit(timelineObject.placeVisit));

            var newActivitySegments = semantictimeline.timelineObjects
                .Where(timelineObject => timelineObject.activitySegment != null)
                .Select(timelineObject => new DbActivitySegment(timelineObject.activitySegment));

            var timelineDataUser = _applicationDbContext.Users
                .Where(dbUser => user.Id == dbUser.Id)
                .Include(dbUser => dbUser.TimelineData)
                .ThenInclude(timelineData => timelineData.ActivitySegments)
                .Include(dbUser => dbUser.TimelineData)
                .ThenInclude(timelineData => timelineData.PlaceVisits)
                .Single();

            if (user.TimelineData == null)
            {
                user.TimelineData = new TimelineData();
            }
            var timelineData = user.TimelineData;

            var oldPlaceVisits = _applicationDbContext.TimelineData
                .Where(data => data.Id == timelineData.Id)
                .Include(data => data.PlaceVisits)
                .SelectMany(data => data.PlaceVisits)
                .ToList();

            var oldActivitySegments = _applicationDbContext.TimelineData
                .Where(data => data.Id == timelineData.Id)
                .Include(data => data.ActivitySegments)
                .SelectMany(data => data.ActivitySegments)
                .ToList();

            var oldLocations = _applicationDbContext.Locations.ToList();

            // Filter out existing place visits
            newPlaceVisits = newPlaceVisits
                .Where(visit => !oldPlaceVisits.Any(oldVisit => oldVisit.StartDateTime == visit.StartDateTime && oldVisit.EndDateTime == visit.EndDateTime))
                .Select(visit =>
                {
                    // Check for existing location of main visit
                    var existingLocation = oldLocations.FirstOrDefault(location => !string.IsNullOrEmpty(location.PlaceId) && location.PlaceId.Equals(visit.LocationVisit.Location.PlaceId));
                    if (existingLocation != null)
                    {
                        visit.LocationVisit.Location = existingLocation;
                    }
                    else
                    {
                        oldLocations.Add(visit.LocationVisit.Location);
                    }

                    // Check for existing locations of child visits
                    visit.ChildVisits = visit.ChildVisits.Select(childVisit =>
                    {
                        var existingLocation = oldLocations.FirstOrDefault(location => !string.IsNullOrEmpty(location.PlaceId) && location.PlaceId.Equals(childVisit.LocationVisit.Location.PlaceId));
                        if (existingLocation != null)
                        {
                            childVisit.LocationVisit.Location = existingLocation;
                        }
                        else
                        {
                            oldLocations.Add(childVisit.LocationVisit.Location);
                        }
                        return childVisit;
                    }).ToList();
                    oldPlaceVisits.Add(visit);
                    return visit;
                });

            // Filter out existing activity segments
            newActivitySegments = newActivitySegments
                .Where(segment => !oldActivitySegments.Any(oldSegment => oldSegment.StartDateTime == segment.StartDateTime && oldSegment.EndDateTime == segment.EndDateTime))
                .Select(segment =>
                {
                    // Check for existing locations of transit location visits
                    segment.TransitLocationVisits = segment.TransitLocationVisits.Select(visit =>
                    {
                        var existingLocation = oldLocations.FirstOrDefault(location => !string.IsNullOrEmpty(location.PlaceId) && location.PlaceId.Equals(visit.Location.PlaceId));
                        if (existingLocation != null)
                        {
                            visit.Location = existingLocation;
                        }
                        else
                        {
                            oldLocations.Add(visit.Location);
                        }
                        return visit;
                    }).ToList();
                    oldActivitySegments.Add(segment);
                    return segment;
                });

            timelineData.ActivitySegments.AddRange(newActivitySegments);
            timelineData.PlaceVisits.AddRange(newPlaceVisits);

            _applicationDbContext.Update(user);
            _applicationDbContext.SaveChanges();
        }

        public TimelineData GetTimelineData(User user, RetrievalMethod method = RetrievalMethod.Id)
        {
            switch (method)
            {
                case RetrievalMethod.Id: return LoadTimelineDataByIdAggregation(user);
                case RetrievalMethod.Include: return LoadTimelineDataByInclude(user);
                case RetrievalMethod.Load: return LoadTimelineDataByExplicitLoad(user);
                case RetrievalMethod.Predicate: return LoadTimelineDataByPredicateAggregation(user);
            }
            throw new InvalidEnumArgumentException($"Method does not support provided Enum {method}");
        }

        public long BenchmarkGetTimelineData(User user, RetrievalMethod method = RetrievalMethod.Id)
        {
            var timer = Stopwatch.StartNew();
            GetTimelineDataInternal(user, method);
            return timer.ElapsedMilliseconds;
        }

        private TimelineData GetTimelineDataInternal(User user, RetrievalMethod method)
        {
            switch (method)
            {
                case RetrievalMethod.Id: return LoadTimelineDataByIdAggregation(user);
                case RetrievalMethod.Include: return LoadTimelineDataByInclude(user);
                case RetrievalMethod.Load: return LoadTimelineDataByExplicitLoad(user);
                case RetrievalMethod.Predicate: return LoadTimelineDataByPredicateAggregation(user);
            }
            throw new InvalidEnumArgumentException($"Method does not support provided Enum {method}");
        }

        /// <summary>
        /// Load all timeline data by incrementally loading batches through chaining predicates
        /// Low amount of database calls, and low data duplication, but more complex retrieval logic
        /// </summary>
        private TimelineData LoadTimelineDataByPredicateAggregation(User user)
        {
            var data = _applicationDbContext.Users
                .Where(dbUser => dbUser.Id == user.Id)
                .Include(dbUser => dbUser.TimelineData)
                .Single()
                .TimelineData;

            var waypointPredicates = new List<Expression<Func<DbWaypoint, bool>>>();
            var locationVisitPredicates = new List<Expression<Func<DbLocationVisit, bool>>>();
            var placeVisitPredicates = new List<Expression<Func<DbPlaceVisit, bool>>>();
            var locationPredicates = new List<Expression<Func<DbLocation, bool>>>();

            _applicationDbContext.Entry(data).Collection(d => d.ActivitySegments).Load();
            foreach (var segment in data.ActivitySegments ?? new List<DbActivitySegment>())
            {
                waypointPredicates.Add(waypoint => waypoint.DbActivitySegmentId == segment.Id || waypoint.DbActivitySegmentId == segment.StartWaypointId || waypoint.DbActivitySegmentId == segment.EndWaypointId);
                locationVisitPredicates.Add(locationVisit => locationVisit.DbActivitySegmentId == segment.Id);
            }

            _applicationDbContext.Entry(data).Collection(d => d.PlaceVisits).Load();
            foreach (var visit in data.PlaceVisits ?? new List<DbPlaceVisit>())
            {
                locationVisitPredicates.Add(locationVisit => locationVisit.Id == visit.LocationVisitId);
                placeVisitPredicates.Add(placeVisit => placeVisit.DbPlaceVisitId == visit.Id);
            }

            var loadedChildVisits = LoadFromPredicates(_applicationDbContext.PlaceVisits, placeVisitPredicates);
            foreach (var visit in loadedChildVisits)
            {
                locationVisitPredicates.Add(locationVisit => locationVisit.Id == visit.LocationVisitId);
            }

            var loadedLocationVisits = LoadFromPredicates(_applicationDbContext.LocationVisits, locationVisitPredicates);
            foreach (var locationVisit in loadedLocationVisits)
            {
                locationPredicates.Add(location => location.Id == locationVisit.LocationId);
            }

            LoadFromPredicates(_applicationDbContext.Waypoints, waypointPredicates);
            LoadFromPredicates(_applicationDbContext.Locations, locationPredicates);

            return data;
        }

        /// <summary>
        /// Load all timeline data by incrementally loading batches through collecting ids
        /// Low amount of database calls, and low data duplication, but more complex retrieval logic
        /// </summary>
        private TimelineData LoadTimelineDataByIdAggregation(User user)
        {
            var data = _applicationDbContext.Users
                .Where(dbUser => dbUser.Id == user.Id)
                .Include(dbUser => dbUser.TimelineData)
                .Single()
                .TimelineData;

            // Direct ids
            var waypointIds = new HashSet<int?>();
            var locationVisitIds = new HashSet<int?>();
            var locationIds = new HashSet<int?>();

            // Ids by principal key
            var waypointIdsByActivitySegmentId = new HashSet<int?>();
            var locationVisitIdsByActivitySegmentId = new HashSet<int?>();
            var placeVisitIdsByPlaceVisitId = new HashSet<int?>();

            // Load all activity segments
            _applicationDbContext.Entry(data).Collection(d => d.ActivitySegments).Load();
            foreach (var segment in data.ActivitySegments ?? new List<DbActivitySegment>())
            {
                waypointIds.Add(segment.StartWaypointId);
                waypointIds.Add(segment.EndWaypointId);
                waypointIdsByActivitySegmentId.Add(segment.Id);
                locationVisitIdsByActivitySegmentId.Add(segment.Id);
            }

            // Load root place visits
            _applicationDbContext.Entry(data).Collection(d => d.PlaceVisits).Load();
            foreach (var visit in data.PlaceVisits ?? new List<DbPlaceVisit>())
            {
                locationVisitIds.Add(visit.LocationVisitId);
                placeVisitIdsByPlaceVisitId.Add(visit.Id);
            }

            // Load nested place visits
            placeVisitIdsByPlaceVisitId = placeVisitIdsByPlaceVisitId.NotNull().ToHashSet();
            var loadedChildVisits = _applicationDbContext.PlaceVisits
                .Where(placeVisit => placeVisitIdsByPlaceVisitId.Contains(placeVisit.DbPlaceVisitId))
                .ToList();
            foreach (var visit in loadedChildVisits)
            {
                locationVisitIds.Add(visit.LocationVisitId);
            }

            // Load location visits
            locationVisitIds = locationVisitIds.NotNull().ToHashSet();
            locationVisitIdsByActivitySegmentId = locationVisitIdsByActivitySegmentId.NotNull().ToHashSet();
            var loadedLocationVisits = _applicationDbContext.LocationVisits
                .Where(locationVisit => locationVisitIds.Contains(locationVisit.Id) || locationVisitIdsByActivitySegmentId.Contains(locationVisit.DbActivitySegmentId))
                .ToList();
            foreach (var locationVisit in loadedLocationVisits)
            {
                locationIds.Add(locationVisit.LocationId);
            }

            // Load waypoints
            waypointIds = waypointIds.NotNull().ToHashSet();
            waypointIdsByActivitySegmentId = waypointIdsByActivitySegmentId.NotNull().ToHashSet();
            _applicationDbContext.Waypoints
                .Where(waypoint => waypointIds.Contains(waypoint.Id) || waypointIdsByActivitySegmentId.Contains(waypoint.DbActivitySegmentId))
                .ToList();

            // Load locations
            locationIds = locationIds.NotNull().ToHashSet();
            _applicationDbContext.Locations
                .Where(location => locationIds.Contains(location.Id))
                .ToList();

            return data;
        }

        /// <summary>
        /// Load all matching entries from the set by union of the provided predicates
        /// </summary>
        private List<T> LoadFromPredicates<T>(DbSet<T> dbSet, IEnumerable<Expression<Func<T, bool>>> predicateExpressions) where T : class
        {
            var queryable = dbSet.AsQueryable();
            foreach (var expression in predicateExpressions)
            {
                queryable = queryable.Union(dbSet.Where(expression));
            }
            return queryable.ToList();
        }

        /// <summary>
        /// Load all the timeline data by including all relevant properties
        /// One single database call, but absurd data duplication, and as a result extreme row count
        /// </summary>
        private TimelineData LoadTimelineDataByInclude(User user)
        {
            return _applicationDbContext.Users
                .Where(dbUser => dbUser.Id == user.Id)
                .Include(dbUser => dbUser.TimelineData)
                .ThenInclude(data => data.ActivitySegments).ThenInclude(data => data.StartWaypoint)
                .Include(dbUser => dbUser.TimelineData)
                .ThenInclude(data => data.ActivitySegments).ThenInclude(data => data.EndWaypoint)
                .Include(dbUser => dbUser.TimelineData)
                .ThenInclude(data => data.ActivitySegments).ThenInclude(data => data.TransitLocationVisits).ThenInclude(data => data.Location)
                .Include(dbUser => dbUser.TimelineData)
                .ThenInclude(data => data.ActivitySegments).ThenInclude(data => data.Waypoints)
                .Include(dbUser => dbUser.TimelineData)
                .ThenInclude(data => data.PlaceVisits).ThenInclude(data => data.LocationVisit).ThenInclude(data => data.Location)
                .Include(dbUser => dbUser.TimelineData)
                .ThenInclude(data => data.PlaceVisits).ThenInclude(data => data.ChildVisits).ThenInclude(data => data.LocationVisit).ThenInclude(data => data.Location)
                .Single()
                .TimelineData;
        }

        /// <summary>
        /// Load all the timeline data by cascading load calls through the data tree
        /// Many database calls, but low data duplication
        /// </summary>
        private TimelineData LoadTimelineDataByExplicitLoad(User user)
        {
            var data = _applicationDbContext.Users
                .Where(dbUser => dbUser.Id == user.Id)
                .Include(dbUser => dbUser.TimelineData)
                .Single()
                .TimelineData;

            _applicationDbContext.Entry(data).Collection(d => d.ActivitySegments).Load();
            foreach (var segment in data.ActivitySegments ?? new List<DbActivitySegment>())
            {
                _applicationDbContext.Entry(segment).Reference(d => d.StartWaypoint).Load();
                _applicationDbContext.Entry(segment).Reference(d => d.EndWaypoint).Load();
                _applicationDbContext.Entry(segment).Collection(d => d.TransitLocationVisits).Load();
                foreach (var visit in segment.TransitLocationVisits ?? new List<DbLocationVisit>())
                {
                    _applicationDbContext.Entry(visit).Reference(d => d.Location).Load();
                }
                _applicationDbContext.Entry(segment).Collection(d => d.Waypoints).Load();
            }

            _applicationDbContext.Entry(data).Collection(d => d.PlaceVisits).Load();
            foreach (var visit in data.PlaceVisits ?? new List<DbPlaceVisit>())
            {
                _applicationDbContext.Entry(visit).Reference(d => d.LocationVisit).Load();
                _applicationDbContext.Entry(visit.LocationVisit).Reference(d => d.Location).Load();
                foreach (var childVisit in visit.ChildVisits ?? new List<DbPlaceVisit>())
                {
                    _applicationDbContext.Entry(childVisit).Reference(d => d.LocationVisit).Load();
                    _applicationDbContext.Entry(childVisit.LocationVisit).Reference(d => d.Location).Load();
                }
            }

            return data;
        }
    }
}

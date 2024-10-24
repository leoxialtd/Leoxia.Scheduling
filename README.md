# Leoxia Scheduling

## Purpose

This library is needed because there is no alternative that provides the following features:

- Built with testability in mind (all classes and schedule of jobs is testable by design)
- No static holder (Hangfire and Quartz.Net uses static which hinders testability)
- ASP.NET Core and Dependency Injection compatible.
- Fluent features in mind
- Extendability in mind.
- Scheduling focused (no events or other unrelated features)
- Ability to trigger job manually
- Ability to retrieve all registered jobs, running jobs and job history
- Ensure that jobs schedule is honored even if timer is late (for any reason)

## Features

- [x] Testability: timer, time, jobs are mockable to be able to test the scheduling engine
- [x] Dependency injection based.
- [x] Parameters support.
- [x] Fast time provider (only 1 second granularity by default). 
- [x] Configurable Timer increment (can have a more precise time and timer raised more often)
- [x] Fluent job configuration builder (easy to extend).
- [x] Can trigger a scheduled job
- [x] Can wait for engine start
- [x] Job retrieval
- [x] Can attach a successor task (several task not tested)
- [x] Custom job resolver (not tested)
- [ ] Cron schedule
- [x] Max times of schedule (not sufficiently tested)
- [ ] Prevent overlap
- [ ] Job run history

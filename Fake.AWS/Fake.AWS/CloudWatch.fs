module Fake.AWS.CloudWatch

type Alarm = { x: string }
type For = For
type Periods = Periods
type Operation = GreaterThan | LessThan | GreaterOrEqualThan | LessOrEqualThan
type Statistic = Min | Max | Sum | Avg | Samples
type Interval = Of1Minute | Of5Minutes | Of15Minutes | Of1Hour | Of6Hours | Of1Day

let c = Clients.cloudwatch();
type CloudWatchAlarm = Amazon.CloudWatch.Model.PutMetricAlarmRequest
type Alarms = CloudWatchAlarm list

type AlarmBuilder() = 
    member x.Yield (()) = 
        []

    [<CustomOperation("alarm")>]
    member x.Alarm(curentList: Alarms, name) =
        let current = new CloudWatchAlarm()
        current.AlarmName <- name
        current :: curentList

    [<CustomOperation("threshold")>]
    member x.Threshold(curentList: Alarms, statistic: Amazon.CloudWatch.Statistic, metric, operation : Operation, threshold, forKeyword : For, periods, periodsKeyword: Periods, interval: Interval) =
        let current = curentList.Head
        current.Statistic <- statistic
        curentList

    [<CustomOperation("dimension")>]
    member x.Dimension(curentList: Alarms, name, value) =
        curentList

    [<CustomOperation("statistic")>]
    member x.Statistic(curentList: Alarms, statistic: Statistic) =
        curentList

    [<CustomOperation("period")>]
    member x.Period(curentList: Alarms, metric, operation : Operation, threshold, forKeyword : For, duration, durationUnit) =
        curentList

let alarmspec = AlarmBuilder()

//let alarm =
//    alarmspec {
//        alarm "No live frames generated"
//        threshold Sum "LiveFramesPerCamera" LessThan 10 For 2 Periods Of5Minutes
//        dimension "Env" "prod"
//    }

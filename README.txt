David Jeffery Oct 2021
dj@drnj.co.uk
www.drnj.co.uk

INTRODUCTION
=============

I am time limited on this project as I have been in/out of hospital for a minor op 
and have been dosed up on pain-killers so not running at 100%!

From the brief I have tried to construct a solution with individual projects all loosely coupled by Depending Injection,
The reason being that this is the "standard" way that I like to develop code and allows easy unit testing.

LIMITATIONS
===========

Because of limited time (+painkillers) I have taken a simple approach to "scheduling" the aggregation at X minute intervals in 
the main Worker "loop".
The simple approach "fires off" a Task to do the aggreation at the specified time then waits for the time interval - the
assumption is the aggregation "process" is fairly quick so that "the wait for X mins" should allow the next aggregation to run 
after X minutes. This may not be the case but for the sake of demonstration here I think it is sufficient to work.

I did look at Timers and async "spawning" of Tasks at each X minute increment but I could not come up with a simple clean 
solution in the time permitted

Apologies for this - I do realise the shortcomings of my code


TESTING
========
Testing - I've used XUnit but could use NUnit or MS-Test
In unit tests I try to follow the old-fashioned AAA approach - assign, act, assert. I have used BDD (Specflow) in the past but did not like it

I could write more tests etc but I wanted to give you a feel for my development skills and approach


I have tried to make the classes as robust as possible and avoided exposing methods as public simply to be able to
call them in unit tests. I have a bit of Jeffery magic picked up over the years with a unit test helper class which allows
calling of protected and private methods in classes purely for test purposes. It doesn't work to well with Generic methods though.
There is, of course, the philosophical debate as to whether protected/private methods should ever be called in a test as they form
part of the internals of the class and that only the public methods should be called - I just think that testing is king - everything possible
should be tested - it's saved my skin a number of times.

I have also used a Builder approach to testing - helps with fake setups.
I have utilised NSubstitute and I would use NBuilder given enough time...



DEPENDENCY INJECTION
====================

The modern approach is "programming by injection" rather than class derivation. I am not sure that I agree 100% with this approach as it
can lead to "death by dependency injection"  where class constructors have a large number of items injected. Apparently the "mediator pattern"
gets around this issue, however, I have not had time, so far, to investigate this pattern. So...I try to use class derivation where necessary
and DI where necessary or a combination.

I have put the interfaces in the same files as the classes -really for expediency, but, of course, they could be in separate files or even
a separate project

LOGGING
=======
Logging - I have used "simple" Serilog - no sinks etc as per full-blown Serilog, just logs to a file

The log file location is configurable in appsettings


WINDOWS SERVICE
===============

I have not "coded" the solution as a service - it's so it's easy to run from Visual Studio as-is
To make into a service:

    /// To run as a service look at https://csharp.christiannagel.com/2019/10/15/windowsservice/
    ///                             https://docs.microsoft.com/en-us/dotnet/core/extensions/windows-service
    ///                             
    /// Apply .UseWindowsService(); to CreateDeefaultBuilderChain
    /// 
    /// Install with sc create “DJ Sample Service” binPath=c:\sampleservice\DRNJ.Petro.Worker.exe
    /// sc start "DJ Sample Service"

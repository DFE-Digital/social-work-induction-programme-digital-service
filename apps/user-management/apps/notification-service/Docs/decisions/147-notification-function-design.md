---
# These are optional elements. Feel free to remove any of them.
status {proposed  rejected  accepted  deprecated  …  superseded by [ADR-0005](0005-example.md)}
date {YYYY-MM-DD when the decision was last updated}
deciders {list everyone involved in the decision}
consulted {list everyone whose opinions are sought (typically subject-matter experts); and with whom there is a two-way communication}
informed {list everyone who is kept up-to-date on progress; and with whom there is a one-way communication}
---
# Implementation of exponential backoff and pattern for calling Gov Notify

## Context and Problem Statement

When we call the Gov Notify client we need to add functionality to our code that allows it to retry the request *X* times if it fails initially. In order to do this we need alter how we call the client.

## Considered Options

 Implement a retry policy
 Implement an exponential backoff policy
 Implement the decorator pattern
 Implement the command pattern

## Decision Outcome

Chosen option Implement an exponential backoff policy & Implement the command pattern, because using the exponential backoff allows for more time to pass imbetween retries. This will give the Gov Notify client more time to recover and increase the chances of our request being successful. We have chosen the command pattern as this allows us to alter how we call the client, so we can add the exponential back off logic when we call the request


!-- This is an optional element. Feel free to remove. --
### Confirmation

{Describe how the implementation ofcompliance with the ADR is confirmed. E.g., by a review or an ArchUnit test.
 Although we classify this element as optional, it is included in most ADRs.}

!-- This is an optional element. Feel free to remove. --
## Pros and Cons of the Options

### Implement a retry policy

 - Good, because it will attempt to resend the request to Gov Notify should the initial request fail
 - Bad, because it will retry the client call *X* times, every *Y* seconds (both customisable), usually in very quick sucsession. If the client is down for more than a few seconds we will stop retrying and the notification will never be sent.

### Implement an exponential backoff policy

 - Good, because it will attempt to resend the request to Gov Notify should the initial request fail
 - Good, because for every notifcaiton request failure the time before we send the next request doubles. This gives the client more time recover and successfully send notification again. e.g. first retry is 1 second later, then 2 seconds, then 4 etc. 
 
### Implement the decorator pattern

 - Good, because it allows us to alter how to call the client, which means we can add the exponential back off
 - Bad, because we have to implement *all* methods in the Notification Client interace (~20 methods) in our decorator class. We only use 1 method (SendEmailAsync), so this causes a lot of irrelevant bloat in the class

### Implement the command pattern

 - Good, because it allows us to alter how to call the client, which means we can add the exponential back off
 - Good, because we can implement only the methods we use, which makes the class much less bloated. 
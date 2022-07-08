# TheOmenDen's EventStore
## A simple, lightweight Event Sourcing solution
- Borrowing concepts heavily from [Daniel Miller's article](https://www.codeproject.com/Articles/5264244/A-Fast-and-Lightweight-Solution-for-CQRS-and-Event) on CQRS and Event sourcing, we've packaged up some of that solution, and mixed with Asynchronous streams to bring hopefully the best of both worlds. 
- We aim to provide a persistence method that is _suggested_ but not _enforced_ with this, thus allowing the end user to define their own way to persist events within their own methods.
- We aim to provide a way to interface with popular messaging queues/service busses in the `.NET` environment.

## The case for our implementation
  ### Performance
  - Hopefully by using `IAsyncEnumerable<T>`, we can take advantage of the potentially long-running retrivals of reconstructing entities from events
  - Providing overrides for common methods with `async ... await` patterns to help delegate concurrency issues
  - Minimizing the use of `Concurrent Collections` to help reduce overhead - these may prove to be necessary for integrity
  
  ### Extensibility
  - Using generics, and being a bit more liberal on the use of pipeline-esque interfaces we aim to have a more "plug and play" messaging/subscribing system
  - By working with this concept we aim to provide a modest way to integrate our "out of process" events with an "in-process" library like [MediatR](https://github.com/jbogard/MediatR)
  
  ### Minimal external dependencies
  - While we can't avoid certain dependencies, we do aim have as few as possible to prevent rough loading times
  - We also aim to use _only relevant_ dependencies for our end goals. 
  
  ### Continuing Development
  - As a work in progress (WIP) this library will be updated, and improved on as time goes on, and our knowledge gets deeper.
  - This is of course a work in progress, all code is "as-is", and does not provide any coverage, warranties, or guarantees. 
  - Use at your own risk, and under your own discretion

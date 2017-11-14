Services
========


## Overview

Each service in the list below works in union with each other to deliver a full Homeserver implementation.

Each service at it's core relies on a message queue to send and receive processing requests.

Some services will have incoming and outgoing connections and some will process and pass on requests.

This architecture allows for multiple services of the same type to run in parallel. For instance, it would be useful to run multiple instances of the DatastoreService, RoomService and FederationService.


## Services

### ClientServerAPIService

This service handles incoming requests from a client and feeds them to an appropriate node.

### StorageService

### UserAccountService

### RoomService

### DatastoreService

### FederationService

### PresenceService

### DeviceService

### PushService

### ThirdpartyService

### SearchService

### ServerAdminService

### ApplicationAPIService

### GroupService

### ConfigurationService

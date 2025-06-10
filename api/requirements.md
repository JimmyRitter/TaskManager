Order:
- specifications 
- product requirements (crud, share list, tick/untick task)
- feature breakdown
- prioritization
- technical considerations (tdd, solid, design patterns)
- technical requirements (migration scripts, iac, docker)  
- create git

# Specifications
## Problem
- Too many tasks to finish
- I can't organize myself properly
- Lists are spread across different places (laptop, phone, paper notes around the house)
- "Things to buy", "things to do" and "places to visit" all on the same list 

## Solution
- Make an application that manages these lists of things to do

---

# Product Requirements
## Core Features
### Task Management
- Create and manage multiple to-do lists
- Add and remove tasks within lists
- Mark tasks as complete (tick) or incomplete (untick)
- Track history of task status changes (tick/untick events)

### List Organization
- Ability to create and delete lists
- Lists must have a category assigned from pre-defined options (no custom categories allowed)
- Each list represents a specific goal/purpose within its category
  Example: 
  - List: "Places to visit in Paris" → Category: "Travel"
  - List: "Weekly Groceries" → Category: "Shopping"

### List Sharing
- Share lists with other users
- Configurable sharing permissions:
    - Read-only (tick/untick only)
    - Full access (tick/untick/add/remove items)

## User Management
### Authentication
- User registration and login system
- Secure authentication

### User Profile
- Profile management features:
    - Personal information (name, contact)
    - Notification preferences
    - Password management
    - Account settings

---

# Feature breakdown
## Task Management
- As a user, I want to create and delete lists, so I can organize my tasks effectively
- As a user, I want to add items to my lists, so I can track what needs to be done
- As a user, I want to remove items from my lists, so I can keep my lists clean and relevant
- As a user, I want to mark items as complete or incomplete, so I can track my progress
- As a user, I want to see the history of my completed items, so I can review when tasks were completed

## List Organization
- As a user, I want to assign a pre-defined category to my lists, so I can better organize them by purpose
- As a user, I want to view my lists by their categories, so I can easily find specific lists

## List Sharing
- As a user, I want to share my lists with other users, so we can collaborate on the same tasks
- As a user, I want to set permissions when sharing lists, so I can control what others can do with my lists
- As a list owner, I want to revoke sharing access, so I can manage who has access to my lists

## User Management
- As a user, I want to sign up for an account, so I can access the application securely
- As a user, I want to log in to my account, so I can access my personal lists
- As a user, I want to manage my profile information, so I can keep my details up to date
- As a user, I want to change my password, so I can keep my account secure
- As a user, I want to manage my notification preferences, so I can control how I receive updates


# Technical Architecture Overview

## Core Architecture Principles
- Clean Architecture implementation for clear separation of concerns
- Domain-Driven Design (DDD) approach for complex business logic
- CQRS pattern for better scalability and separation of read/write operations
- Event-Sourcing for comprehensive audit trail requirements

## API Design
### API Standards
- RESTful API design following OpenAPI/Swagger specifications
- Versioning through URL (e.g., /api/v1/)
- Rate Limiting:
  - Implementation using Redis for distributed rate limiting
  - Per-user and per-endpoint configuration
  - Sliding window algorithm for better accuracy

### Documentation
- OpenAPI/Swagger for API documentation
- DocFX integration for code documentation
- XML documentation in code for auto-generation
- Comprehensive README files for each project component
- Architecture Decision Records (ADRs) for major technical decisions

## Implementation Details

### Domain Layer
#### Entities
- Strong domain models with encapsulated business logic
- Value Objects for immutable concepts
- Rich domain events for tracking changes

#### Core Entities:
```markdown
Task
- Properties: Id, Name, Completed, CompletedAt, ListId
- Behavior: Complete(), Uncomplete()
- Events: TaskCreated, TaskCompleted, TaskUncompleted

List
- Properties: Id, Name, Description, CategoryId, OwnerId
- Behavior: AddTask(), RemoveTask(), Share(), RevokeAccess()
- Events: ListCreated, ListUpdated, ListShared, ListAccessRevoked

Category
- Properties: Id, Name, Description
- Predefined values managed through database

User
- Properties: Id, Email, ProfileInfo
- Behavior: UpdateProfile(), ChangePassword()
- Events: UserCreated, ProfileUpdated
```

### Application Layer
#### Design Patterns
- Repository Pattern for data access abstraction
- Unit of Work for transaction management
- Mediator Pattern (using MediatR) for decoupling
- Specification Pattern for complex queries
- Factory Pattern for complex object creation

#### CQRS Implementation
- Commands: Handling state changes
- Queries: Optimized for read operations
- Event Handlers: Async processing of domain events

### Infrastructure Layer
#### Data Persistence
- Entity Framework Core for main database operations
- Redis for caching and rate limiting
- Event Store for event sourcing

#### Cross-Cutting Concerns
- Logging (structured logging with Serilog)
- Error handling (global exception middleware)
- Authentication/Authorization (JWT with refresh tokens)
- Request validation (FluentValidation)

## Testing Strategy
### Unit Testing
- TDD approach with comprehensive test coverage
- Test categories:
  - Domain Logic Tests
  - Command/Query Handler Tests
  - Validation Tests
  - Authorization Tests
- Mock frameworks for isolation
- Builder pattern for test data construction

### Integration Testing
- API endpoint testing
- Database integration testing
- Authentication flow testing

## Quality Assurance
- Static code analysis
- Code coverage requirements (minimum 80%)
- Automated API documentation testing
- Performance benchmarking

## CI/CD Considerations
- Docker containerization
- Automated deployment pipelines
- Database migration scripts
- Environment-specific configurations
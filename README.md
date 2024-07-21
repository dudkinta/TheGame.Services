# TheGame.Services

[Русская версия](README_RU.md)

A game in the format of TelegramMiniApps.<br/>
Services for a pet project.
<br/><br/>
In this project, I am testing microservices architecture, dependency injection, programming patterns, and CI/CD in GitHub Actions.

## Brief Description of Services 
[See DFT](DTF.pdf)
* LoginService - service for user registration and authentication.
* InnerTokenService - internal service for microservices authentication when communicating with each other. This service is not accessible from outside and is intended only for internal interaction.
* FriendService - service for managing referral links.
* StatisticService - service for player statistics.
* HuntingService - service for organizing the "hunting" game. Something similar to 'Duck Hunter'.
* QueueService - service for executing tasks received from RabbitMQ queues.

## Dependencies

The project depends on the following technologies:

### Nginx

[Nginx](https://nginx.org/) is an open-source web server that can also function as a reverse proxy server, load balancer, mail proxy server, and HTTP cache. It is known for its high performance, stability, and low resource consumption.

### RabbitMQ

[RabbitMQ](https://www.rabbitmq.com/) is an open-source message broker that implements the Advanced Message Queuing Protocol (AMQP). It allows your applications to send and receive messages and helps manage queues, route messages, and ensure reliable delivery.

### Consul

[Consul](https://www.consul.io/) is a service discovery and configuration tool that makes it easy to manage distributed systems and microservices. Consul provides service registration, health checks, distributed key-value storage, and multi-datacenter support.

### PostgreSQL
[PostgreSQL](https://www.postgresql.org/) is a powerful, open-source relational database management system. PostgreSQL supports extensibility, scalability, and adheres to SQL standards, providing reliable data storage and the ability to handle large volumes of information.
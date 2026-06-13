<div align="center">

<img src="https://img.shields.io/badge/EduAdvisor-Intelligent%20Academic%20Platform-1a3c6e?style=for-the-badge&logo=graduation-cap&logoColor=white" alt="EduAdvisor"/>

### 🎓 Intelligent Academic Advising & Recommendation Platform
*Empowering educational pathways through data-driven mentor guidance and automated intelligence.*

[![Target Framework](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Architecture Pattern](https://img.shields.io/badge/Architecture-Clean%20%2B%20CQRS-007acc?style=for-the-badge&logo=blueprint&logoColor=white)](#-architecture)
[![Database Infrastructure](https://img.shields.io/badge/SQL%20Server-Enterprise-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)

**Fayoum University — Faculty of Computers & Artificial Intelligence — Graduation Project 2026**

---

[🌐 Live API Documentation](https://eduadvisor.runasp.net/swagger) • [📱 Mobile App Repository](#) • [💻 Web Dashboard Repos](#)

</div>

## 📋 Table of Contents

- [📌 Overview](#-overview)
- [✨ Core Capabilities](#-core-capabilities)
- [🛠️ Technical Ecosystem](#%EF%B8%8F-technical-ecosystem)
- [🏗️ Architectural Blueprint](#%EF%B8%8F-architectural-blueprint)
- [📁 Layered Project Directory](#-project-structure)
- [🚀 Local Deployment & Getting Started](#-getting-started)
- [⚙️ Environment Configuration](#%EF%B8%8F-configuration)
- [📡 API Gateway Specification](#-api-endpoints)
- [👥 Engineering Team](#-team)

---

## 📌 Overview

**EduAdvisor** is an enterprise-grade academic advising ecosystem engineered specifically for the credit-hour system constraints at Fayoum University. By migrating legacy manual verification models into an automated hub, it dynamically minimizes advisory backlogs, prevents prerequisite sequence failures, and tracks graduation eligibility indexes in real time.

### System Solution Mapping

| Legacy Operational Bottleneck | EduAdvisor Intelligent Engine Paradigm |
| :--- | :--- |
| High student-to-advisor ratios leading to course mapping errors. | **Hybrid ML Recommendation Engine:** Resolves optimal course paths based on historical GPA metrics and strict prerequisite validations. |
| Fragmented cross-communication channels & late alert updates. | **SignalR Live Mesh:** Instant notification framework and direct mentor-to-student low-latency chat routing. |
| Inflexible monolithic systems causing registration lockouts. | **CQRS Architecture Architecture:** Separates query loads from strict transactional write logs to support elastic throughput during registration weeks. |

---

## ✨ Core Capabilities

### 👨‍🎓 Student Ecosystem
- **AI-Driven Course Orchestration:** Suggests optimal credit loads dynamically adjusted to the student’s cumulative GPA and remaining core plans.
- **Visual Analytics Dashboard:** Real-time GPA projection tracking, prerequisite dependency maps, and risk factor flags for warning hours.
- **NLP Cognitive Assistant:** 24/7 localized support bot addressing questions about internal university bylaws, registration timelines, and cross-course requirements.
- **Academic Pathway Predictor:** Recommends specialized tracks based on historical performance vectors in relevant foundational modules.

### 👨‍🏫 Mentor Portal
- **Consolidated Student Tracking Profiles:** High-fidelity view of assignees' complete timelines, transcripts, and registration histories.
- **Workflow Approvals Grid:** One-click approval pipelines for custom credit overrides, tracking adjustments, and final schedule validations.
- **Performance Analytics Matrix:** Early-warning tracking filters isolating students falling below standard retention indexes.

### ⚙️ Institutional Governance
- **Academic Plan Configurations:** Micro-management tools for universities, branch faculties, active semesters, and complex multi-tiered course prerequisite dependencies.
- **Granular RBAC System:** Claim-based custom authorization structures to secure identity assertions between system operators, registrars, and advisors.

---

## 🛠️ Technical Ecosystem

### Backend Architecture
* **Core Framework:** ASP.NET Core 9.0 Web API (Enterprise Edition Pipeline)
* **Data Access Layer:** Entity Framework Core 9.0 (LINQ Expressions, Compilable Compiled Queries, Explicit Eager Loading)
* **Message Bus & MediatR:** Internal In-Memory Command/Query decoupling for strict boundary insulation.
* **Real-Time Pipeline:** SignalR Hub Topology using highly cohesive WebSockets fallbacks.
* **Security & Token Handler:** Cryptographically signed JWT Identity tokens utilizing custom claim assertions.
* **Validation Subsystem:** FluentValidation pipeline interception via generic MediatR `IPipelineBehavior` pipelines.

### Client Interfaces
* **Cross-Platform Mobile:** Flutter (Dart Framework utilizing Bloc/Cubit cleanly segmented states).
* **Administrative Web Hub:** React.js Dashboard tailored with custom scannable telemetry hooks.

---

## 🏗️ Architectural Blueprint

The backend engine strictly conforms to **Clean Architecture** combined with **CQRS (Command Query Responsibility Segregation)** design principles to ensure complete database layer insulation from core domain business rules.
